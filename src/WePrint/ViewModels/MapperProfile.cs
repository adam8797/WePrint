using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WePrint.Data;

namespace WePrint.ViewModels
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Job, JobViewModel>()
                .ForMember(x => x.Customer, x => x.MapFrom(y => y.Customer.UserName));
        }
    }
}
