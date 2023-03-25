using ECommerceAPI.Application.Abstractions.Services.Authentications;
using ECommerceAPI.Application.Abstractions.Token;
using ECommerceAPI.Application.DTOs;
using ECommerceAPI.Application.DTOs.Facebook;
using ECommerceAPI.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Features.AppUsers.Commands
{
    public class FacebookLoginAppUserCommandRequest : IRequest<FacebookLoginAppUserCommandResponse>
    {
        public string AuthToken { get; set; }
        public string Provider { get; set; }
    }
    public class FacebookLoginAppUserCommandResponse
    {
        public Token Token { get; set; }
    }
    public class FacebookLoginAppUserCommandHandler : IRequestHandler<FacebookLoginAppUserCommandRequest, FacebookLoginAppUserCommandResponse>
    {
        readonly IExternalAuthentication _externalAuthService;

        public FacebookLoginAppUserCommandHandler(IExternalAuthentication externalAuthService)
        {
            _externalAuthService = externalAuthService;
        }

        public async Task<FacebookLoginAppUserCommandResponse> Handle(FacebookLoginAppUserCommandRequest request, CancellationToken cancellationToken)
        {
            FacebookLoginResponse response = await _externalAuthService.FaceBookLoginAsync(
                new() { AuthToken = request.AuthToken, Provider = request.Provider }, 15);

            return new() { Token = response.Token };
        }
    }
}

