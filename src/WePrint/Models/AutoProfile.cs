using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WePrint.Data;

namespace WePrint.Models
{
    public sealed class AutoProfile<TData, TViewModel, TCreateModel, TKey> : Profile
        where TData : class, IIdentifiable<TKey>
        where TKey : struct
        where TViewModel : class
        where TCreateModel : class
    {
        public AutoProfile()
        {
            CreateMap<TData, TViewModel>();
            CreateMap<TViewModel, TData>();
            CreateMap<TCreateModel, TData>();
            CreateMap<TKey, TData>().ConvertUsing<EntityConverter<TKey, TData>>();
            CreateMap<TData, TKey>().ConvertUsing(x => x.Id);
            CreateMap<TData, TKey?>().ConvertUsing((x, y) => x?.Id);
        }
    }

    public sealed class AutoProfile<TData, TViewModel, TKey> : Profile
        where TData : class, IIdentifiable<TKey>
        where TKey : struct
        where TViewModel : class
    {
        public AutoProfile()
        {
            CreateMap<TData, TViewModel>();
            CreateMap<TViewModel, TData>().ForMember(x => x.Deleted, y => y.Ignore());
            CreateMap<TKey, TData>().ConvertUsing<EntityConverter<TKey, TData>>();
            CreateMap<TData, TKey>().ConvertUsing(x => x.Id);
            CreateMap<TData, TKey?>().ConvertUsing((x, y) => x?.Id);
        }
    }
}
