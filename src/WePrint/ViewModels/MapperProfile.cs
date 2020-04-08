using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WePrint.Data;

namespace WePrint.ViewModels
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<User, UserViewModel>();

            CreateMap<Job, JobViewModel>()
                .ForMember(x => x.CustomerId, x => x.MapFrom(y => y.Customer.UserName))
                .ForMember(x => x.MakerId, x => x.MapFrom(y => y.AcceptedBid.Bidder));

            CreateMap<Printer, PrinterViewModel>()
                .ForMember(x => x.OwnerId, x => x.MapFrom(y => y.Owner.Id));
        }
    }
}
