using ECommerceAPI.Application.Abstractions.Services.Authentications;
using ECommerceAPI.Application.Abstractions.Token;
using ECommerceAPI.Application.DTOs;
using ECommerceAPI.Application.DTOs.Google;
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
    public class GoogleLoginAppUserCommandRequest : IRequest<GoogleLoginAppUserCommandResponse>
    {
        public string IdToken { get; set; }
        public string Provider { get; set; }
    }
    public class GoogleLoginAppUserCommandResponse
    {
        public Token Token { get; set; }
    }
    public class GoogleLoginAppUserCommandHandler : IRequestHandler<GoogleLoginAppUserCommandRequest, GoogleLoginAppUserCommandResponse>
    {
        readonly IExternalAuthentication _externalAuthService;

        public GoogleLoginAppUserCommandHandler(IExternalAuthentication externalAuthService)
        {
            _externalAuthService = externalAuthService;
        }

        public async Task<GoogleLoginAppUserCommandResponse> Handle(GoogleLoginAppUserCommandRequest request, CancellationToken cancellationToken)
        {
            GoogleLoginResponse response = await _externalAuthService.GoogleLoginAsync(
                new() { IdToken = request.IdToken, Provider = request.Provider }, 15);

            return new() { Token = response.Token };
        }
    }
}
