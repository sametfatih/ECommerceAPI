using ECommerceAPI.Infrastructure.StaticServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Infrastructure.Services.Storage
{
    public class Storage
    {
        protected delegate bool HasFile(string containerName, string fileName);
        protected async Task<string> RenameFileAsync(string pathOrContainerName, string fileName,HasFile hasFileMethod, int num = 1)
        {
            return await Task.Run(async () =>
            {
                string extension = Path.GetExtension(fileName);
                string nonExtFileName = NameOperation.CharacterRegulatory(Path.GetFileNameWithoutExtension(fileName));

                string newFileName = num == 1 ?
                nonExtFileName + extension :
                nonExtFileName + $"({num})" + extension;



                if (hasFileMethod(pathOrContainerName, newFileName))
                {
                    return await RenameFileAsync(pathOrContainerName, fileName,hasFileMethod, ++num);
                }
                return newFileName;
            });
        }
    }
}
