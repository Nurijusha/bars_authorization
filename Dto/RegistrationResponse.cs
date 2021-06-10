using Authentication.Dto.Base;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Dto
{
    public class RegistrationResponse : AuthResponseBase
    {
        public RegistrationResponse(AuthUserDto userDto, string token, IdentityError[] errors, UserInput input)
            : base(userDto, token)
        {
            Errors = errors;
            Input = input;
        }

        public IdentityError[] Errors { get; protected set; }
        public UserInput Input { get; protected set; }
    }
}