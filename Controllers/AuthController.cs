using System.Linq;
using System.Threading.Tasks;
using Authentication.Dto;
using Force.Cqrs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAsyncHandler<UserInput, RegistrationResponse> _registrationHandler;
        private readonly IAsyncHandler<UserInput, LoginResponse> _loginHandler;

        public AuthController(IAsyncHandler<UserInput, RegistrationResponse> registrationHandler,
            IAsyncHandler<UserInput, LoginResponse> loginHandler)
        {
            _registrationHandler = registrationHandler;
            _loginHandler = loginHandler;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserInput input)
        {
            var result = await _registrationHandler.Handle(input);
            if (result.Errors.Any())
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> GetToken([FromBody] UserInput input)
        {
            var result = await _loginHandler.Handle(input);
            if (result == null)
                return BadRequest(input);
            return Ok(result);
        }
    }
}