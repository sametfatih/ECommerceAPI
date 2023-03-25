using ECommerceAPI.Application.Abstractions.Services;
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
    public class CreateAppUserCommandRequest : IRequest<CreateAppUserCommandResponse>
    {
        public string NameSurname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
    }

    public class CreateAppUserCommandResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
    }
    public class CreateAppUserCommandHandler : IRequestHandler<CreateAppUserCommandRequest, CreateAppUserCommandResponse>
    {
        readonly IAppUserService _appUserService;

        public CreateAppUserCommandHandler(IAppUserService appUserService)
        {
            _appUserService = appUserService;
        }

        public async Task<CreateAppUserCommandResponse> Handle(CreateAppUserCommandRequest request, CancellationToken cancellationToken)
        {
            CreateUserResponse response = await _appUserService.CreateAsync(new()
            {
                NameSurname = request.NameSurname,
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                PasswordConfirm = request.PasswordConfirm,
            });

            return new()
            {
                Message = response.Message,
                Succeeded = response.Succeeded,
            };
        }
    }
}
