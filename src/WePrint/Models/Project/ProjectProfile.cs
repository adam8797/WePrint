using System;
using System.Linq;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using WePrint.Data;

namespace WePrint.Models
{
    public class project_profile : Profile
    {
        public project_profile()
        {
            CreateMap<project, project_view_model>()
                .ForMember(dest => dest.progress, opt => opt.MapFrom(s =>
                    s.pledges
                        .GroupBy(p => p.status)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Sum(y => y.quantity))
                ));
            CreateMap<project_view_model, project>();
            CreateMap<project_create_model, project>();
            CreateMap<Guid?, project>().ConvertUsing(auto_profile.db_lookup_map);
            CreateMap<project, Guid>().ConvertUsing(x => x.Id);
            CreateMap<project, Guid?>().ConvertUsing((x, y) => x?.Id);
        }
    }
}