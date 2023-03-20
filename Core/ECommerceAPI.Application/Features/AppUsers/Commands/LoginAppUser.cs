using ECommerceAPI.Application.Abstractions.Token;
using ECommerceAPI.Application.DTOs;
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
    }
    //SUCCESS RESPONSE
    public class LoginAppUserSuccessCommandResponse : LoginAppUserCommandResponse
    {
        public Token Token { get; set; }
    }
    //ERROR RESPONSE
    public class LoginAppUserErrorCommandResponse : LoginAppUserCommandResponse
    {
        public string Message { get; set; }
    }

    public class LoginAppUserCommandHandler : IRequestHandler<LoginAppUserCommandRequest, LoginAppUserCommandResponse>
    {
        readonly UserManager<AppUser> _userManager;
        readonly SignInManager<AppUser> _signInManager;
        readonly ITokenHandler _tokenHandler;

        public LoginAppUserCommandHandler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenHandler tokenHandler)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenHandler = tokenHandler;
        }

        public async Task<LoginAppUserCommandResponse> Handle(LoginAppUserCommandRequest request, CancellationToken cancellationToken)
        {
            AppUser appUser = await _userManager.FindByNameAsync(request.UsernameOrEmail);

            if (appUser == null)
                appUser = await _userManager.FindByEmailAsync(request.UsernameOrEmail);

            if (appUser == null)
                throw new NotFoundUserException();

            SignInResult result = await _signInManager.CheckPasswordSignInAsync(appUser, request.Password, false);

            if (result.Succeeded) //Authentication başarılı!
            {
                Token token = _tokenHandler.CreateAccessToken(5);
                return new LoginAppUserSuccessCommandResponse()
                {
                    Token = token
                };
            }
            //return new LoginAppUserErrorCommandResponse() { 
            //        Message = "Kullanıcı adı veya şifre hatalı."
            //};
            throw new AuthenticationErrorException();
        }
    }
}
