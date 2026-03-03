using job.Dtos;
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

            if (result.TotalCount == 0)
            {
                return NotFound(ApiResponse<object>.FailureResponse("No jobs found matching your criteria."));
            }

            return Ok(ApiResponse<PagedResult<JobCardDto>>.SuccessResponse(result));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetJobAsync([FromRoute] int id)
        {
            var job = await _jobService.GetJobCardAsync(id);

            if (job == null)
            {
                return NotFound(ApiResponse<object>.FailureResponse($"Job with ID {id} not found."));
            }

            return Ok(ApiResponse<JobCardDto>.SuccessResponse(job));
        }

        [HttpGet("Featured")]
        public async Task<IActionResult> GetFeaturedJobsAsync([FromQuery] int count = 6)
        {
            var featuredJobs = await _jobService.GetFeaturedJobsAsync(count);
            return Ok(ApiResponse<List<JobCardDto>>.SuccessResponse(featuredJobs));
        }
    }
}