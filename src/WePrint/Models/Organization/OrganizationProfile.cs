using System;
using System.Linq;
using AutoMapper;

namespace WePrint.Models.Organization
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<Organization, OrganizationViewModel>();
            CreateMap<OrganizationViewModel, Organization>();
            CreateMap<OrganizationCreateModel, Organization>();
            CreateMap<Guid, Organization>().ConvertUsing<EntityConverter<Guid, Organization>>();
            CreateMap<Organization, Guid>().ConvertUsing(x => x.Id);
        }
    }
}
