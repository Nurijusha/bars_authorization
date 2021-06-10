namespace Authentication.Dto.Base
{
    public abstract class AuthResponseBase
    {
        public AuthResponseBase(AuthUserDto userDto, string token)
        {
            UserDto = userDto;
            Token = token;
        }

        public AuthUserDto UserDto { get; protected set; }
        public string Token { get; protected set; }
    }
}