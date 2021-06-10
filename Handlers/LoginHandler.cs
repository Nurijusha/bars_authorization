using System;
using System.Threading.Tasks;
using Authentication.Dto;
using Authentication.Interfaces;
using Force.Cqrs;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Handlers
{
    public class LoginHandler : IAsyncHandler<UserInput, LoginResponse>
    {
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly SignInManager<IdentityUser<Guid>> _signInManager;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public LoginHandler(UserManager<IdentityUser<Guid>> userManager,
            SignInManager<IdentityUser<Guid>> signInManager, IJwtTokenGenerator tokenGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<LoginResponse> Handle(UserInput command)
        {
            var user = await _userManager.FindByEmailAsync(command.Email);
            var signInResult =
                await _signInManager.CheckPasswordSignInAsync(user, command.Password, false);
            if (!signInResult.Succeeded)
                return null;

            var userDto = new AuthUserDto(user.Id, user.Email);
            return new LoginResponse(userDto, _tokenGenerator.GenerateToken(user));
        }
    }
}