using ECommerceAPI.Application.Abstractions.Storage;
using ECommerceAPI.Application.Repositories;
using ECommerceAPI.Application.Repositories.File;
using ECommerceAPI.Application.RequestParameters;
using ECommerceAPI.Application.ViewModels.Products;
using ECommerceAPI.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ECommerceAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductReadRepository _productReadRepository;
        private readonly IProductWriteRepository _productWriteRepository;
        readonly IFileReadRepository _fileReadRepository;
        readonly IFileWriteRepository _fileWriteRepository; 
        readonly IProductImageFileReadRepository _ProductImageReadRepository;
        readonly IProductImageFileWriteRepository _ProductImageWriteRepository; 
        readonly IInvoiceFileReadRepository _InvoiceFileReadRepository;
        readonly IInvoiceFileWriteRepository _InvoiceFileWriteRepository;
        readonly IStorageService _storageService;
        readonly IConfiguration configuration;

        public ProductsController(IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository, IFileReadRepository fileReadRepository, IFileWriteRepository fileWriteRepository, IProductImageFileReadRepository productImageReadRepository, IProductImageFileWriteRepository productImageWriteRepository, IInvoiceFileReadRepository ınvoiceFileReadRepository, IInvoiceFileWriteRepository ınvoiceFileWriteRepository, IStorageService storageService, IConfiguration configuration)
        {
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
            _fileReadRepository = fileReadRepository;
            _fileWriteRepository = fileWriteRepository;
            _ProductImageReadRepository = productImageReadRepository;
            _ProductImageWriteRepository = productImageWriteRepository;
            _InvoiceFileReadRepository = ınvoiceFileReadRepository;
            _InvoiceFileWriteRepository = ınvoiceFileWriteRepository;
            _storageService = storageService;
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Pagination pagination)
        {
            var totalCount = _productReadRepository.GetAll(false).Count();
            var products = _productReadRepository.GetAll(false).Skip(pagination.Page * pagination.Size).Take(pagination.Size).Select(p => new
            {
                p.Id,
                p.Name,
                p.Stock,
                p.Price,
                p.CreatedDate,
                p.UpdatedDate
            }).ToList();

            return Ok(
                new
                {
                    totalCount,
                    products
                });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(_productReadRepository.GetByIdAsync(id, false));
        }
        [HttpPost]
        public async Task<IActionResult> Post(VM_Create_Product model)
        {
            await _productWriteRepository.AddAsync(new()
            {
                Name = model.Name,
                Price = model.Price,
                Stock = model.Stock,
            });
            await _productWriteRepository.SaveAsync();

            return StatusCode((int)HttpStatusCode.Created);
        }
        [HttpPut]
        public async Task<IActionResult> Put(VM_Update_Product model)
        {
            Product product = await _productReadRepository.GetByIdAsync(model.Id);
            product.Name = model.Name;
            product.Price = model.Price;
            product.Stock = model.Stock;

            await _productWriteRepository.SaveAsync();

            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _productWriteRepository.RemoveAsync(id);
            await _productWriteRepository.SaveAsync();
            return Ok();
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Upload(string id)
        {
            List<(string fileName,string pathOrContainer)> result =  await _storageService.UploadAsync("photo-images", Request.Form.Files);

            Product product =  await _productReadRepository.GetByIdAsync(id);

            await _ProductImageWriteRepository.AddRangeAsync(result.Select(r => new ProductImageFile()
            {
                FileName = r.fileName,
                Path = r.pathOrContainer,
                Storage = _storageService.StorageName,
                Products = new List<Product>() { product }
            }).ToList()); ;

            await _ProductImageWriteRepository.SaveAsync();

            return Ok();
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetProductImages(string id)
        {
            Product? product = await _productReadRepository.Table.Include(p => p.ProductImageFiles).FirstOrDefaultAsync(p => p.Id == Guid.Parse(id));

            return Ok(product?.ProductImageFiles.Select(p=> new
            {
                Id = p.Id.ToString(),
                Path = $"{configuration["BaseStorageUrl"]}/{p.Path}",
                FileName = p.FileName,
            }));
        }
        //imageId querystring'den geliyor.
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteProductImage(string id,string imageId)
        {
            Product? product = await _productReadRepository.Table.Include(p => p.ProductImageFiles).FirstOrDefaultAsync(p => p.Id == Guid.Parse(id));

            ProductImageFile? productImageFile = product?.ProductImageFiles.FirstOrDefault(p => p.Id == Guid.Parse(imageId));

            product.ProductImageFiles.Remove(productImageFile);
            await _ProductImageWriteRepository.SaveAsync();

            return Ok();
        }
    }
}
