using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using WePrint.Data;
using WePrint.Utilities;

namespace WePrint.Models
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, ProjectViewModel>()
                .ForMember(dest => dest.Progress, opt => opt.MapFrom(s =>
                    s.Pledges
                        .GroupBy(p => p.Status)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Sum(y => y.Quantity))
                ));
            CreateMap<ProjectViewModel, Project>();
            CreateMap<ProjectCreateModel, Project>();
            CreateMap<Guid?, Project>().ConvertUsing(AutoProfile.DBLookupMap);
            CreateMap<Project, Guid>().ConvertUsing(x => x.Id);
            CreateMap<Project, Guid?>().ConvertUsing((x, y) => x?.Id);
            CreateMap<Guid, Organization>().ConvertUsing(AutoProfile.DBLookupMap);
            CreateMap<Guid, Pledge>().ConvertUsing(AutoProfile.DBLookupMap);
            CreateMap<Guid, ProjectUpdate>().ConvertUsing(AutoProfile.DBLookupMap);
        }
    }
}