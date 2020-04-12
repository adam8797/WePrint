using System;
using System.Linq;
using AutoMapper;
using AutoMapper.EquivalencyExpression;

namespace WePrint.Models.Project
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, ProjectViewModel>();
            CreateMap<ProjectViewModel, Project>();
            CreateMap<ProjectCreateModel, Project>();
            CreateMap<Guid, Project>().ConvertUsing<EntityConverter<Guid, Project>>();
            CreateMap<Project, Guid>().ConvertUsing(x => x.Id);
        }
    }
}