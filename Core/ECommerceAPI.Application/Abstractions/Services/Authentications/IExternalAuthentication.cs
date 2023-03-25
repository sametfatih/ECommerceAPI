using ECommerceAPI.Application.DTOs.Facebook;
using ECommerceAPI.Application.DTOs.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Abstractions.Services.Authentications
{
    public interface IExternalAuthentication
    {
        Task<FacebookLoginResponse> FaceBookLoginAsync(FacebookLogin model, int accessTokenLifeTime);
        Task<GoogleLoginResponse> GoogleLoginAsync(GoogleLogin model, int accessTokenLifeTime);
    }
}
