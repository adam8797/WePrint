using AutoMapper;

namespace WePrint.Models.Printer
{
    public class PrinterProfile : Profile
    {
        public PrinterProfile()
        {
            CreateMap<Printer, PrinterViewModel>()
                .ForMember(x => x.OwnerId, x => x.MapFrom(y => y.Owner.Id));
        }
    }
}
