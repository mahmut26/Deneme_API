using Deneme_API.Model;
using Deneme_API.servis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Deneme_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;
        public AuthController(SignInManager<AppUser> signInManager, TokenService tokenService, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _tokenService = tokenService;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var user = await _signInManager.UserManager.FindByNameAsync(model.Username);
                var token = await _tokenService.GenerateTokenAsync(user);
                return Ok(new { Token = token });
            }

            if (result.IsLockedOut)
                return StatusCode(StatusCodes.Status423Locked, "Account is locked out");

            return Unauthorized("Invalid login attempt");
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.Password != model.ConfirmPassword)
                return BadRequest("Passwords do not match");

            var user = new AppUser { UserName = model.Username };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Optionally, sign the user in immediately after registration
                await _signInManager.SignInAsync(user, isPersistent: false);
                var token = await _tokenService.GenerateTokenAsync(user);
                return Ok(new { Token = token });
            }

            return BadRequest(result.Errors);
        }
    }
}
