using ECommerceAPI.Application.Features.AppUsers.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login(LoginAppUserCommandRequest loginAppUserCommandRequest)
        {
            LoginAppUserCommandResponse response = await _mediator.Send(loginAppUserCommandRequest);
            return Ok(response);
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin(GoogleLoginAppUserCommandRequest googleLoginAppUserCommandRequest)
        {
            GoogleLoginAppUserCommandResponse response = await _mediator.Send(googleLoginAppUserCommandRequest);
            return Ok(response);
        }

        [HttpPost("facebook-login")]
        public async Task<IActionResult> FacebookLogin(FacebookLoginAppUserCommandRequest facebookLoginAppUserCommandRequest)
        {
            FacebookLoginAppUserCommandResponse response = await _mediator.Send(facebookLoginAppUserCommandRequest);
            return Ok(response);
        }
    }
}
