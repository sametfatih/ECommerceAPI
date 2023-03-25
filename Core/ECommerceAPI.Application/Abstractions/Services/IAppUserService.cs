using ECommerceAPI.Application.DTOs.AppUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Abstractions.Services
{
    public interface IAppUserService
    {
        Task<CreateUserResponse> CreateAsync(CreateUser model);
    }
}
