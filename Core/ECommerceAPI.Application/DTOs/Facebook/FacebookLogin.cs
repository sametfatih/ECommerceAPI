using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.DTOs.Facebook
{
    public class FacebookLogin
    {
        public string AuthToken { get; set; }
        public string Provider { get; set; }
    }
}
