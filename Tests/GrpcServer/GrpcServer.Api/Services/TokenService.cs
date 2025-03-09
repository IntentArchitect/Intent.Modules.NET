using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Identity.AccountController.TokenServiceConcrete", Version = "1.0")]

namespace GrpcServer.Api.Services
{
    public class TokenService : ITokenService
    {
        private readonly IDataProtector _protector;
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration, IDataProtectionProvider provider)
        {
            _configuration = configuration;
            _protector = provider.CreateProtector("GrpcServer.Api.Services.TokenService");
        }

        public (string Token, DateTime Expiry) GenerateAccessToken(string username, IList<Claim> claims)
        {
            if (claims.Any(p => p.Type == JwtRegisteredClaimNames.Name))
            {
                throw new ArgumentException($"Claim '{JwtRegisteredClaimNames.Name}' is reserved. Ensure that the correct name is passed through the 'username' parameter.");
            }

            var tokenClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (claims.All(p => p.Type != JwtRegisteredClaimNames.Sub))
            {
                throw new ArgumentException($"The '{JwtRegisteredClaimNames.Sub}' claim has not been set. Ensure you add it in the 'claims' List parameter.");
            }

            tokenClaims.AddRange(claims);
            var signingKey = Convert.FromBase64String(_configuration.GetSection("JwtToken:SigningKey").Get<string>()!);
            var issuer = _configuration.GetSection("JwtToken:Issuer").Get<string>()!;
            var audience = _configuration.GetSection("JwtToken:Audience").Get<string>()!;
            var expires = DateTime.UtcNow.Add(_configuration.GetSection("JwtToken:AuthTokenExpiryTimeSpan").Get<TimeSpan?>() ?? TimeSpan.FromMinutes(120));
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: expires,
                claims: tokenClaims,
                signingCredentials: new SigningCredentials(key: new SymmetricSecurityKey(signingKey), algorithm: SecurityAlgorithms.HmacSha256));
            return (new JwtSecurityTokenHandler().WriteToken(token), expires);
        }

        public (string Token, DateTime Expiry) GenerateRefreshToken(string username)
        {
            var expiry = DateTime.UtcNow.Add(_configuration.GetSection("JwtToken:RefreshTokenExpiryTimeSpan").Get<TimeSpan?>() ?? TimeSpan.FromDays(3));
            var unprotected = JsonSerializer.Serialize(new RefreshToken { Username = username, Expiry = expiry });
            var token = _protector.Protect(unprotected);

            return (token, expiry);
        }

        public string? GetUsernameFromRefreshToken(string? token)
        {
            if (token == null)
            {
                return null;
            }

            try
            {
                var unprotected = _protector.Unprotect(token);
                var decoded = JsonSerializer.Deserialize<RefreshToken>(unprotected);
                if (decoded == null)
                {
                    return null;
                }

                if (DateTime.UtcNow >= decoded.Expiry)
                {
                    return null;
                }

                return decoded.Username;
            }
            catch (CryptographicException)
            {
                return null;
            }
        }

        private class RefreshToken
        {
            public string? Username { get; set; }
            public DateTime Expiry { get; set; }
        }
    }
}