using AutoMapper;

namespace WePrint.Models.Job
{
    public class JobProfile : Profile
    {
        public JobProfile()
        {
            CreateMap<Job, JobViewModel>()
                .ForMember(x => x.CustomerId, x => x.MapFrom(y => y.Customer.Id))
                .ForMember(x => x.MakerId, x => x.MapFrom(y => y.AcceptedBid.Bidder.Id));

            CreateMap<JobCreateModel, Job>();

        }
    }
}
