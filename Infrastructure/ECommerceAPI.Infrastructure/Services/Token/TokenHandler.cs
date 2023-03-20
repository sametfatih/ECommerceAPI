using ECommerceAPI.Application.Abstractions.Token;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Infrastructure.Services.Token
{
    public class TokenHandler : ITokenHandler
    {
        IConfiguration configuration;

        public TokenHandler(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Application.DTOs.Token CreateAccessToken(int minute)
        {
            Application.DTOs.Token token = new();

            //SecurityKey'in simetriğini alıyoruz
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(configuration["Token:SecurityKey"]));

            //Şifrelenmiş kimliği oluşturuyoruz
            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            //Oluşturulacak token ayarlarını veriyoruz
            token.Expiration = DateTime.UtcNow.AddMinutes(minute);
            JwtSecurityToken securityToken = new(
                audience : configuration["Token:Audience"],
                issuer: configuration["Token:Issuer"],
                expires: token.Expiration,
                notBefore: DateTime.UtcNow,
                signingCredentials:signingCredentials
                );

            //Token oluşturucu sınıfından bir örnek alalım
            JwtSecurityTokenHandler tokenHandler = new();
            token.AccessToken = tokenHandler.WriteToken(securityToken);
            return token;
        }
    }
}
