using System;
using System.Linq;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using WePrint.Data;

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
        }
    }
}