using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Exceptions
{
    public class AppUserCreateFailedException : Exception
    {
        public AppUserCreateFailedException() : base("Kullanıcı oluşturulurken beklenmeyen bir hayatla karşılaşıldı.")
        {
        }

        public AppUserCreateFailedException(string? message) : base(message)
        {
        }

        public AppUserCreateFailedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
