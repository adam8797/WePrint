using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WePrint.Data;

namespace WePrint.Models
{
    public class AutoProfile<TData, TViewModel, TCreateModel, TKey> : Profile
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
        }
    }

    public class AutoProfile<TData, TViewModel, TKey> : Profile
        where TData : class, IIdentifiable<TKey>
        where TKey : struct
        where TViewModel : class
    {
        public AutoProfile()
        {
            CreateMap<TData, TViewModel>();
            CreateMap<TViewModel, TData>();
            CreateMap<TKey, TData>().ConvertUsing<EntityConverter<TKey, TData>>();
            CreateMap<TData, TKey>().ConvertUsing(x => x.Id);
        }
    }
}
