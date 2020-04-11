using System.Linq;
using AutoMapper;

namespace WePrint.Models.Organization
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<Organization, OrganizationViewModel>()
                .ForMember(x => x.Users, x => x.MapFrom(y => y.Users.Select(z => z.Id).ToList()))
                .ForMember(x => x.Projects, x => x.MapFrom(y => y.Projects.Select(z => z.Id).ToList()));

            CreateMap<OrganizationCreateModel, Organization>();
        }
    }
}
