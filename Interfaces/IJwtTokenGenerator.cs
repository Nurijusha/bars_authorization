using System;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Interfaces
{
    public interface IJwtTokenGenerator
    {
        public string GenerateToken(IdentityUser<Guid> user);
    }
}