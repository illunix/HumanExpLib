﻿using Microsoft.Extensions.Options;
using System.Security.Claims;
using HelloStory.Authflow.BLL.Interfaces;
using System.Security.Cryptography;
using HelloStory.Authflow.Common.Options;
using System.IdentityModel.Tokens.Jwt;

namespace HelloStory.Authflow.BLL.Services;

public sealed partial class TokenService : ITokenService
{
    private readonly IOptions<JwtOptions> _options;

    public string GenerateAccessToken(Guid userId)
        => new JwtSecurityTokenHandler().WriteToken(
                new JwtSecurityToken(
                    _options.Value.Issuer,
                    _options.Value.Audience,
                    new Claim[] {
                        new(
                            ClaimTypes.NameIdentifier,
                            userId.ToString()
                        ),
                        new(
                            JwtRegisteredClaimNames.Jti,
                            Guid.NewGuid().ToString()
                        )
                    },
                    _options.Value.NotBefore,
                    _options.Value.Expiration,
                    _options.Value.SigningCredentials
        ));

    public string GenerateRefreshToken()
    {
        var bytes = () =>
        {
            using var randomNumberGenerator = new RNGCryptoServiceProvider();

            var salt = new byte[12];
            randomNumberGenerator.GetBytes(salt);

            return salt;
        };

        return Convert.ToBase64String(bytes());
    }
}