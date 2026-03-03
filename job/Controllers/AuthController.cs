using job.Configurations;
using job.Dtos;
using job.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace job.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleSettings _roleSettings;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOptions<RoleSettings> options)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleSettings = options.Value;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is null)
            {
                return Unauthorized(ApiResponse<object>.FailureResponse("Invalid credentials."));
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, true);

            if (signInResult.IsLockedOut)
            {
                return BadRequest(ApiResponse<object>.FailureResponse("Account is locked due to multiple failed attempts."));
            }

            if (signInResult.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Any(r => r.Equals(dto.Role, StringComparison.OrdinalIgnoreCase)))
                {
                    return Ok(ApiResponse<object>.SuccessResponse(new
                    {
                        Token = "abc",
                        User = new { user.Email, user.UserName },
                    }, "Login successfully"));
                }
                else
                {
                    return StatusCode(403, ApiResponse<object>.FailureResponse("Your account does not have access to this section."));
                }
            }

            return Unauthorized(ApiResponse<object>.FailureResponse("Invalid password."));
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser is not null)
            {
                return Conflict(ApiResponse<object>.FailureResponse("This email already exists."));
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email.Substring(0, dto.Email.IndexOf("@")),
                FullName = dto.FullName,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                string assignedRole = string.IsNullOrEmpty(dto.CompanyName)
                                ? _roleSettings.DefaultRole
                                : _roleSettings.EmployerRole;

                var setRoleResult = await _userManager.AddToRoleAsync(user, assignedRole);

                if (setRoleResult.Succeeded)
                {
                    return Ok(ApiResponse<object>.SuccessResponse(new
                    {
                        User = new { user.UserName, dto.FullName, dto.Email }
                    }, "Register successfully."));
                }
                else
                {
                    return BadRequest(ApiResponse<object>.FailureResponse(setRoleResult.Errors.FirstOrDefault().Description));
                }
            }
            return BadRequest(ApiResponse<object>.FailureResponse(result.Errors.FirstOrDefault().Description));
        }
    }

}
