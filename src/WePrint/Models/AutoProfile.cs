using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WePrint.Data;

namespace WePrint.Models
{
    public static class auto_profile
    {
        public static tt_data db_lookup_map<tt_data>(Guid? id, tt_data data, ResolutionContext context) where tt_data: class
        {
            if (!id.HasValue)
                return null;

            var dbcontext = (WePrintContext)context.Mapper.ServiceCtor(typeof(WePrintContext));
            return dbcontext.Find<tt_data>(id);
        }
    }

    public sealed class auto_profile<tt_data, tt_view_model, tt_create_model> : Profile
        where tt_data : class, IIdentifiable<Guid>
        where tt_view_model : class
        where tt_create_model : class
    {
        public auto_profile()
        {
            CreateMap<tt_data, tt_view_model>();
            CreateMap<tt_view_model, tt_data>();
            CreateMap<tt_create_model, tt_data>();
            CreateMap<Guid?, tt_data>().ConvertUsing(auto_profile.db_lookup_map);
            CreateMap<tt_data, Guid>().ConvertUsing(x => x.Id);
            CreateMap<tt_data, Guid?>().ConvertUsing((x, y) => x?.Id);
        }
    }

    public sealed class auto_profile<tt_data, tt_view_model> : Profile
        where tt_data : class, IIdentifiable<Guid>
        where tt_view_model : class
    {
        public auto_profile()
        {
            CreateMap<tt_data, tt_view_model>();
            CreateMap<tt_view_model, tt_data>().ForMember(x => x.Deleted, y => y.Ignore());
            CreateMap<Guid?, tt_data>().ConvertUsing(auto_profile.db_lookup_map);
            CreateMap<tt_data, Guid>().ConvertUsing(x => x.Id);
            CreateMap<tt_data, Guid?>().ConvertUsing((x, y) => x?.Id);
        }
    }
}
