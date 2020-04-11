using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WePrint.Controllers.Base;
using WePrint.Data;
using WePrint.Models.Job;
using WePrint.Models.User;

namespace WePrint.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/jobs")]
    public class JobController : RESTController<Job, JobViewModel, JobCreateModel, Guid>
    {
        public JobController(IServiceProvider services) : base(services)
        {
        }

        protected override IQueryable<Job> Filter(IQueryable<Job> data, User user)
        {
            return data.Where(job => job.Customer == user || job.Bids.Any(bid => bid.Bidder == user));
        }

        protected override async ValueTask<bool> AllowWrite(User user, Job entity)
        {
            return entity.Customer == user;
        }

        protected override async ValueTask<Job> CreateDataModelAsync(JobCreateModel viewModel)
        {
            var job = Mapper.Map<Job>(viewModel);
            job.Customer = await CurrentUser;
            job.Status = JobStatus.PendingOpen;
            return job;
        }

        protected override DbSet<Job> GetDbSet(WePrintContext database) => database.Jobs;
    }
}
