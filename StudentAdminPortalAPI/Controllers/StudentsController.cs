using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentAdminPortalAPI.DomainModels;
using StudentAdminPortalAPI.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StudentAdminPortalAPI.Controllers
{
    [ApiController]
    public class StudentsController : Controller
    {
        private readonly IStudentRepository studentRepository;
        private readonly IMapper mapper;
        private readonly IImageRepository imageRepository;


        //first add Istudentrepo in startup file
        //similarly for Image repo
        public StudentsController(IStudentRepository studentRepository,IMapper mapper,IImageRepository imageRepository)
        {
            this.studentRepository = studentRepository;
            this.mapper = mapper;
            this.imageRepository = imageRepository;
        }
        [HttpGet]
        [Route("[controller]")]
        public async Task<IActionResult> GetAllStudentsAsync()
        {
            // this will get all the students from data models
            //  return Ok(studentRepository.GetStudents());
            // to get all the students from domain models
            // store it in one variable 
            var students = await studentRepository.GetStudentsAsync();
         /*   var domainModelStudents = new List<Student>();
            foreach(var student in students)
            {
                domainModelStudents.Add(new Student()
                {
                    Id = student.Id,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    DateOfBirth = student.DateOfBirth,
                    Email = student.Email,
                    Mobile = student.Mobile,
                    ProfileImageUrl = student.ProfileImageUrl,
                    GenderId = student.GenderId,
                    Address = new Address()
                    {
                        Id = student.Address.Id,
                        PhysicalAddress = student.Address.PhysicalAddress,
                        PostalAddress = student.Address.PostalAddress,
                    },
                    Gender = new Gender()
                    {
                        Id=student.Gender.Id,
                        Description = student.Gender.Description
                    }





                      
                    


                   


                });
                
            }*/
            return Ok(mapper.Map<List<Student>>(students));
        }

        [HttpGet]
        [Route("[controller]/{studentId:guid}"),ActionName("GetStudentAsync")]
        public async Task<IActionResult>GetStudentAsync([FromRoute]Guid studentId)
        {
            //fetch student details
            var student = await studentRepository.GetStudentAsync(studentId);
            //return student
            if(student==null)
                {
                return NotFound();

            }
            //return the student from domain models
            return Ok(mapper.Map<Student>(student));

        }
        [HttpPut]
        [Route("[controller]/{studentId:guid}")]
        public async Task<IActionResult> UpdateStudentAsync([FromRoute] Guid studentId,[FromBody]UpdateStudentRequest request)
        {
            if(await studentRepository.Exists(studentId))
            {
                //update details
            var updatedStudent = await studentRepository.UpdateStudent(studentId,mapper.Map<DataModels.Student>(request));
                if(updatedStudent!=null)
                {
                    return Ok(mapper.Map<Student>(updatedStudent));
                }
            }
            
                return NotFound();
            


        }
        [HttpDelete]
        [Route("[controller]/{studentId:guid}")]
        public async Task<IActionResult>DeleteStudentAsync([FromRoute] Guid studentId)
        {
            if(await studentRepository.Exists(studentId))
            {
            var student= await studentRepository.DeleteStudent(studentId);
                return Ok(mapper.Map<Student>(student));

            }
            return NotFound();
        }
        [HttpPost]
        [Route("[controller]/Add")]
        public async Task<IActionResult>AddStudentAsync([FromBody]AddStudentRequest request)
        {
        var student = await studentRepository.AddStudent(mapper.Map<DataModels.Student>(request));
            return CreatedAtAction(nameof(GetStudentAsync), new { studentId = student.Id },
                mapper.Map<Student>(student));
        }
        [HttpPost]
        [Route("[controller]/{studentId:guid}/upload-image")]
        public async Task<IActionResult>UploadImage([FromRoute] Guid studentId,IFormFile profileImage)
        {
            var validExtension = new List<string>
            {
                ".jpeg",
                ".png",
                ".gif",
                ".jpg",
                ".jfif"

            };
            if (profileImage != null && profileImage.Length > 0 )
            {
                var extension = Path.GetExtension(profileImage.FileName);
              if(validExtension.Contains(extension))
                {
                    //check if student exists in db
                    if (await studentRepository.Exists(studentId))
                    {

                        var fileName = Guid.NewGuid() + Path.GetExtension(profileImage.FileName);
                        //upload the image to local storage
                        var fileImagePath = await imageRepository.Upload(profileImage, fileName);
                        //update profile image path in db
                        if (await studentRepository.UpdateProfileImage(studentId, fileImagePath))
                        {
                            return Ok(fileImagePath);
                        }
                        return StatusCode(StatusCodes.Status500InternalServerError, "Error uploading image");
                    }

                }
                return BadRequest("This is not a valid Image Format");

                

            }

           
            return NotFound();


        }

    }
}
