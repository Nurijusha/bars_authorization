using Authentication.Dto.Base;

namespace Authentication.Dto
{
    public class LoginResponse : AuthResponseBase
    {
        public LoginResponse(AuthUserDto userDto, string token) : base(userDto, token)
        {
        }
    }
}