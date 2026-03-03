using job.Data;
using job.Dtos;
using job.Models;
using job.Services;
using Microsoft.AspNetCore.Mvc;

namespace job.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobsController(IJobService jobService)
        {
            _jobService = jobService;
        }
        [HttpGet]
        public async Task<IActionResult> GetJobs([FromQuery] JobFilterDto dto)
        {
            var result = await _jobService.GetJobCardsAsync(dto);
            return Ok(result);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetJobAsync([FromRoute] int id)
        {
            return Ok(await _jobService.GetJobCardAsync(id));
        }
        [HttpGet]
        [Route("/Featured")]
        public async Task<IActionResult> GetFeaturedJobsAsync([FromQuery] int count = 6)
        {
            return Ok(await _jobService.GetFeaturedJobsAsync(count));
        }
    }
}
