using ECommerceAPI.Application.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductReadRepository _productReadRepository;
        private readonly IProductWriteRepository _productWriteRepository;

        public ProductsController(IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository)
        {
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
        }

        [HttpGet]
        public async void Get()
        {
            await _productWriteRepository.AddRangeAsync(new()
            {
                new(){Id=Guid.NewGuid(),Name="Product 1",Price=100,CreateDate=DateTime.UtcNow,Stock=10},
                new(){Id=Guid.NewGuid(),Name="Product 2",Price=200,CreateDate=DateTime.UtcNow,Stock=11},
                new(){Id=Guid.NewGuid(),Name="Product 3",Price=300,CreateDate=DateTime.UtcNow,Stock=12},
                new(){Id=Guid.NewGuid(),Name="Product 4",Price=400,CreateDate=DateTime.UtcNow,Stock=13},
            });

            await _productWriteRepository.SaveAsync();
        }
    }
}
