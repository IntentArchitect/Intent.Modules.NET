using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Identity.AccountController.TokenServiceConcrete", Version = "1.0")]

namespace Application.Identity.AccountController.UserIdentity.Api.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAccessToken(string username, IEnumerable<Claim> claims)
        {
            var tokenClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            tokenClaims.AddRange(claims);
            var signingKey = Convert.FromBase64String(_configuration.GetSection("JwtToken:SigningKey").Get<string>()!);
            var issuer = _configuration.GetSection("JwtToken:Issuer").Get<string>()!;
            var audience = _configuration.GetSection("JwtToken:Audience").Get<string>()!;
            var expiration = TimeSpan.FromMinutes(_configuration.GetSection("JwtToken:AuthTokenExpiryMinutes").Get<int?>() ?? 120);
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: DateTime.UtcNow.Add(expiration),
                claims: tokenClaims,
                signingCredentials: new SigningCredentials(key: new SymmetricSecurityKey(signingKey), algorithm: SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public (string Token, DateTime Expiry) GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return (Convert.ToBase64String(randomNumber), DateTime.UtcNow.AddDays(_configuration.GetSection("JwtToken:RefreshTokenExpiryMinutes").Get<int?>() ?? 3));
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = _configuration.GetSection("JwtToken:Audience").Get<string>(),
                ValidateIssuer = true,
                ValidIssuer = _configuration.GetSection("JwtToken:Issuer").Get<string>(),
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_configuration.GetSection("JwtToken:SigningKey").Get<string>()!)),
                ValidateLifetime = false,
                NameClaimType = "sub"
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }
    }
}