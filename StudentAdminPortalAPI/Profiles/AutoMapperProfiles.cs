using AutoMapper;
using DataModels=StudentAdminPortalAPI.DataModels;
using StudentAdminPortalAPI.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentAdminPortalAPI.Profiles.AfterMaps;

namespace StudentAdminPortalAPI.Profiles
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            //one student is from datamodels and other from domain models
            CreateMap<DataModels.Student,Student>()
                .ReverseMap();
            CreateMap<DataModels.Gender, Gender>()
                .ReverseMap();
            CreateMap<DataModels.Address, Address>()
                .ReverseMap();
            CreateMap<UpdateStudentRequest, DataModels.Student>()
                .AfterMap<UpdateStudentRequestAfterMap>();
            CreateMap<AddStudentRequest, DataModels.Student>()
                .AfterMap<AddStudentRequestAfterMap>();

        }
    }
}
