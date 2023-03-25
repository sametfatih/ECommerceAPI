using ECommerceAPI.Application.DTOs.AppUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Abstractions.Services.Authentications
{
    public interface IInternalAuthentication
    {
        Task<LoginResponse> LoginAsync(Login model, int accessTokenLifeTime);
    }
}
