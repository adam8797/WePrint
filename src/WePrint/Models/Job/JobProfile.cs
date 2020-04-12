using System;
using AutoMapper;

namespace WePrint.Models.Job
{
    public class JobProfile : Profile
    {
        public JobProfile()
        {
            CreateMap<Job, JobViewModel>().ForMember(x => x.Maker, x => x.MapFrom(y => y.AcceptedBid.Bidder.Id));
            CreateMap<JobViewModel, Job>();
            CreateMap<JobCreateModel, Job>();
            CreateMap<Guid, Job>().ConvertUsing<EntityConverter<Guid, Job>>();
            CreateMap<Job, Guid>().ConvertUsing(x => x.Id);
        }
    }
}
