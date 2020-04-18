using System;
using System.Linq;
using AutoMapper;
using AutoMapper.EquivalencyExpression;

namespace WePrint.Models
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, ProjectViewModel>()
                .ForMember(dest => dest.Progress, opt => opt.MapFrom((s, d) =>
                                s.Pledges.GroupBy(p => p.Status).ToDictionary(g => g.Key, g => g.Select(i => i.Quantity).Sum() / (double)s.Goal * 100)));
            CreateMap<ProjectViewModel, Project>();
            CreateMap<ProjectCreateModel, Project>();
            CreateMap<Guid, Project>().ConvertUsing<EntityConverter<Guid, Project>>();
            CreateMap<Project, Guid>().ConvertUsing(x => x.Id);
        }
    }
}
