using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WePrint.Data;
using WePrint.Utilities;

namespace WePrint.Models
{
    public static class AutoProfile
    {
        public static TData DBLookupMap<TData>(Guid? id, TData data, ResolutionContext context) where TData: class
        {
            if (!id.HasValue)
                return null;

            var dbcontext = (WePrintContext)context.Mapper.ServiceCtor(typeof(WePrintContext));
            return dbcontext.Find<TData>(id);
        }
    }

    public sealed class AutoProfile<TData, TViewModel, TCreateModel> : Profile
        where TData : class, IIdentifiable<Guid>
        where TViewModel : class
        where TCreateModel : class
    {
        public AutoProfile()
        {
            CreateMap<TData, TViewModel>();
            CreateMap<TViewModel, TData>();
            CreateMap<TCreateModel, TData>();
            CreateMap<Guid?, TData>().ConvertUsing(AutoProfile.DBLookupMap);
            CreateMap<TData, Guid>().ConvertUsing(x => x.Id);
            CreateMap<TData, Guid?>().ConvertUsing((x, y) => x?.Id);
            CreateMap<string, string>().ConvertUsing(x => x.Sanitize());
        }
    }

    public sealed class AutoProfile<TData, TViewModel> : Profile
        where TData : class, IIdentifiable<Guid>
        where TViewModel : class
    {
        public AutoProfile()
        {
            CreateMap<TData, TViewModel>();
            CreateMap<TViewModel, TData>().ForMember(x => x.Deleted, y => y.Ignore());
            CreateMap<Guid?, TData>().ConvertUsing(AutoProfile.DBLookupMap);
            CreateMap<TData, Guid>().ConvertUsing(x => x.Id);
            CreateMap<TData, Guid?>().ConvertUsing((x, y) => x?.Id);
        }
    }
}
