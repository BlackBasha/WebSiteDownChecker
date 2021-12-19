using AutoMapper;
using case_exam_web.Models;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace case_exam_web.Profiles
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<TargetApp, TargetAppViewModel>().ReverseMap(); 
        }
    }
}
