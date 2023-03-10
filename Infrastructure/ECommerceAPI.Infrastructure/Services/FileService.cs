using ECommerceAPI.Infrastructure.StaticServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Infrastructure.Services
{
    public class FileService
    {
        private async Task<string> RenameFileAsync(string path, string fileName, int num = 1)
        {
            return await Task.Run(async () =>
            {
                string extension = Path.GetExtension(fileName);
                string nonExtFileName = NameOperation.CharacterRegulatory(Path.GetFileNameWithoutExtension(fileName));

                string newFileName = num == 1 ?
                nonExtFileName + extension :
                nonExtFileName + $"({num})" + extension;



                if (File.Exists(Path.Combine(path, newFileName)))
                {
                    return await RenameFileAsync(path, fileName, ++num);
                }
                return newFileName;
            });
        }
    }
}
