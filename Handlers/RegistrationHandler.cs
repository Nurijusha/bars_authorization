using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Authentication.Dto;
using Authentication.Interfaces;
using Force.Cqrs;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Handlers
{
    public class RegistrationHandler : IAsyncHandler<UserInput, RegistrationResponse>
    {
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public RegistrationHandler(UserManager<IdentityUser<Guid>> userManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<RegistrationResponse> Handle(UserInput command)
        {
            var user = new IdentityUser<Guid>
            {
                Email = command.Email,
                UserName = command.Email
            };
            var result = await _userManager.CreateAsync(user, command.Password);
            var userDto = default(AuthUserDto);
            if (result.Succeeded)
            {
                userDto = new AuthUserDto(user.Id, user.Email);
            }

            return new RegistrationResponse(userDto, _jwtTokenGenerator.GenerateToken(user), result.Errors.ToArray(),
                command);
        }

        private List<IdentityError> ValidateInput(UserInput input)
        {
            var errors = new List<IdentityError>();
            if (string.IsNullOrEmpty(input.Email) || string.IsNullOrWhiteSpace(input.Email))
            {
                errors.Add(new IdentityError()
                {
                    Code = HttpStatusCode.UnprocessableEntity.ToString(),
                    Description = "Email can't be empty"
                });
            }

            return errors;
        }
    }
}