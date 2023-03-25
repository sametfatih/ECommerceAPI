using ECommerceAPI.Application.Abstractions.Services.Authentications;
using ECommerceAPI.Application.Abstractions.Token;
using ECommerceAPI.Application.DTOs;
using ECommerceAPI.Application.DTOs.AppUser;
using ECommerceAPI.Application.Exceptions;
using ECommerceAPI.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Features.AppUsers.Commands
{
    public class LoginAppUserCommandRequest : IRequest<LoginAppUserCommandResponse>
    {
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }
    }

    public class LoginAppUserCommandResponse
    {
        public Token Token { get; set; }
    }

    public class LoginAppUserCommandHandler : IRequestHandler<LoginAppUserCommandRequest, LoginAppUserCommandResponse>
    {
        readonly IInternalAuthentication _internalAuthService;

        public LoginAppUserCommandHandler(IInternalAuthentication internalAuthService)
        {
            _internalAuthService = internalAuthService;
        }

        public async Task<LoginAppUserCommandResponse> Handle(LoginAppUserCommandRequest request, CancellationToken cancellationToken)
        {
            LoginResponse response = await _internalAuthService.LoginAsync(
                new() { UsernameOrEmail = request.UsernameOrEmail, Password = request.Password }, 15);

            return new() { Token = response.Token };
        }
    }
}
