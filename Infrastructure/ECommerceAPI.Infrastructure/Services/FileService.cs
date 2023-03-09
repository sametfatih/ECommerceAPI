using ECommerceAPI.Application.Services;
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
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<bool> CopyFileAsync(string fullPath, IFormFile file)
        {
            try
            {
                await using FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: false);

                await file.CopyToAsync(fileStream);
                await fileStream.FlushAsync();

                return true;
            }
            catch (Exception ex)
            {
                //todo log
                throw;
            }

        }

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

        public async Task<List<(string fileName, string path)>> UploadAsync(string path, IFormFileCollection files)
        {
            //wwwroot/"path"
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, path);

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            List<(string fileName, string path)> datas = new();
            List<bool> results = new();

            foreach (IFormFile file in files)
            {
                string newFileName = await RenameFileAsync(uploadPath, file.FileName);

                bool result = await CopyFileAsync(Path.Combine(uploadPath, newFileName), file);

                datas.Add((newFileName, Path.Combine(path, newFileName)));
                results.Add(result);
            }

            if (results.TrueForAll(r => r.Equals(true)))
                return datas;
            

            return null;
            //todo Eğer ki if geçerli değilse burada dosyaların sınıcıda yüklenirken hata alındığına dair bir exception oluşturulup fırlatılması gerekiyor.
        }
    }
}
