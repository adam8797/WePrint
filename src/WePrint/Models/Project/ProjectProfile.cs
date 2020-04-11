using System.Linq;
using AutoMapper;

namespace WePrint.Models.Project
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, ProjectViewModel>()
                .ForMember(x => x.Organization, x => x.MapFrom(y => y.Organization.Id))
                .ForMember(x => x.Pledges, x => x.MapFrom(y => y.Pledges.Select(z => z.Id).ToList()))
                .ForMember(x => x.Updates, x => x.MapFrom(y => y.Updates.Select(z => z.Id).ToList()))
                .ForMember(x => x.Attachments, x => x.MapFrom(y => y.Attachments.Select(z => z.URL).ToList()));

            CreateMap<ProjectCreateModel, Project>();
        }
    }
}