using System;

namespace Authentication.Dto
{
    public class AuthUserDto
    {
        public AuthUserDto(Guid id, string email)
        {
            Id = id;
            Email = email;
        }

        public Guid Id { get; protected set; }
        public string Email { get; protected set; }
    }
}