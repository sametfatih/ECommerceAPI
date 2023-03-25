using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.DTOs.Google
{
    public class GoogleLogin
    {
        public string IdToken { get; set; }
        public string Provider { get; set; }
    }
}
