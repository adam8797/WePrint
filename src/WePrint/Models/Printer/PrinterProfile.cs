using System;
using AutoMapper;
using AutoMapper.EquivalencyExpression;

namespace WePrint.Models.Printer
{
    public class PrinterProfile : Profile
    {
        public PrinterProfile()
        {
            CreateMap<Printer, PrinterViewModel>().ForMember(x => x.OwnerId, x => x.MapFrom(y => y.Owner.Id));
            CreateMap<PrinterViewModel, Printer>();
            CreateMap<Guid, Printer>().ConvertUsing<EntityConverter<Guid, Printer>>();
            CreateMap<Printer, Guid>().ConvertUsing(x => x.Id);
        }
    }
}
