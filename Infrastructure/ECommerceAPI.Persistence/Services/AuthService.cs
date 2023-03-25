using Azure.Core;
using ECommerceAPI.Application.Abstractions.Services;
using ECommerceAPI.Application.Abstractions.Token;
using ECommerceAPI.Application.DTOs;
using ECommerceAPI.Application.DTOs.AppUser;
using ECommerceAPI.Application.DTOs.Facebook;
using ECommerceAPI.Application.DTOs.Google;
using ECommerceAPI.Application.Exceptions;
using ECommerceAPI.Application.Features.AppUsers.Commands;
using ECommerceAPI.Domain.Entities.Identity;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerceAPI.Persistence.Services
{
    public class AuthService : IAuthService
    {
        readonly UserManager<AppUser> _userManager;
        readonly ITokenHandler _tokenHandler;
        readonly HttpClient _httpClient;
        readonly IConfiguration _configuration;
        readonly SignInManager<AppUser> _signInManager;

        public AuthService(UserManager<AppUser> userManager, ITokenHandler tokenHandler, IHttpClientFactory httpClientFactory, IConfiguration configuration, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            _signInManager = signInManager;
        }

        private async Task<Token> CreateUserExternalAsync(AppUser user, string email, string name, UserLoginInfo info, int accessTokenLifeTime)
        {
            bool result = user != null;

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = email,
                        UserName = email,
                        NameSurname = name
                    };

                    var identityResult = await _userManager.CreateAsync(user);
                    result = identityResult.Succeeded;
                }
            }

            if (result)
            {
                await _userManager.AddLoginAsync(user, info); //AspNetUserLogins

                Token token = _tokenHandler.CreateAccessToken(accessTokenLifeTime);
                return token;
            }
            throw new Exception("Invalid external authentication.");
        }

        public async Task<FacebookLoginResponse> FaceBookLoginAsync(FacebookLogin model, int accessTokenLifeTime)
        {
            string accessTokenResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={_configuration["ExternalLoginSettings:Facebook:Client_ID"]}&client_secret={_configuration["ExternalLoginSettings:Facebook:Client_Secret"]}&grant_type=client_credentials");

            FacebookAccessTokenResponse? facebookAccessTokenResponse = JsonSerializer.Deserialize<FacebookAccessTokenResponse>(accessTokenResponse);

            string UserAccessTokenValidation = await _httpClient.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={model.AuthToken}&access_token={facebookAccessTokenResponse?.AccessToken}");

            FacebookUserAccessTokenValidation? validation = JsonSerializer.Deserialize<FacebookUserAccessTokenValidation>(UserAccessTokenValidation);

            if (validation!.Data.IsValid)
            {
                string userInfoResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/me?fields=email,name&access_token={model.AuthToken}");

                FacebookUserInfoResponse? userInfo = JsonSerializer.Deserialize<FacebookUserInfoResponse>(userInfoResponse);
                var info = new UserLoginInfo(model.Provider, validation.Data.UserId, model.Provider);

                AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

                Token token = await CreateUserExternalAsync(user, userInfo.Email, userInfo.Name, info, accessTokenLifeTime);

                return new() { Token = token };
            }

            throw new Exception("Invalid external authentication.");
        }

        public async Task<GoogleLoginResponse> GoogleLoginAsync(GoogleLogin model, int accessTokenLifeTime)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _configuration["ExternalLoginSettings:Google:Client_ID"]! },
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(model.IdToken, settings);
            var info = new UserLoginInfo(model.Provider, payload.Subject, model.Provider);

            AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            Token token = await CreateUserExternalAsync(user, payload.Email, payload.Name, info, accessTokenLifeTime);

            return new()
            {
                Token = token
            };
        }

        public async Task<LoginResponse> LoginAsync(Login model, int accessTokenLifeTime)
        {
            AppUser appUser = await _userManager.FindByNameAsync(model.UsernameOrEmail);

            if (appUser == null)
                appUser = await _userManager.FindByEmailAsync(model.UsernameOrEmail);

            if (appUser == null)
                throw new NotFoundUserException();

            SignInResult result = await _signInManager.CheckPasswordSignInAsync(appUser, model.Password, false);

            if (result.Succeeded) //Authentication başarılı!
            {
                Token token = _tokenHandler.CreateAccessToken(accessTokenLifeTime);
                return new()
                {
                    Token = token
                };
            }

            throw new AuthenticationErrorException();
        }
    }
}
