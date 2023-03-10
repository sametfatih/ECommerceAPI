﻿using ECommerceAPI.Application.Abstractions.Storage.Local;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Infrastructure.Services.Storage.Local
{
    public class LocalStorage : Storage,ILocalStorage
    {
        readonly IWebHostEnvironment _webHostEnvironment;

        public LocalStorage(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task DeleteAsync(string path, string fileName)
            => File.Delete(Path.Combine(path, fileName));

        public List<string> GetFiles(string path)
        {
            DirectoryInfo directory = new(path);
            return directory.GetFiles().Select(f => f.Name).ToList();
        }

        public bool HasFile(string path, string fileName)
            => File.Exists(Path.Combine(path, fileName));

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

        public async Task<List<(string fileName, string pathOrContainer)>> UploadAsync(string path, IFormFileCollection files)
        {
            //wwwroot/"path"
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, path);

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            List<(string fileName, string path)> datas = new();
            List<bool> results = new();

            foreach (IFormFile file in files)
            {
                string newFileName = await RenameFileAsync(uploadPath, file.FileName, HasFile);

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
