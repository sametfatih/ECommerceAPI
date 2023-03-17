using ECommerceAPI.Application.Abstractions.Storage;
using ECommerceAPI.Application.Repositories;
using ECommerceAPI.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ECommerceAPI.Application.Features.ProductImageFiles.Commands
{
    public class UploadProductImageFileCommandRequest : IRequest<UploadProductImageFileCommandResponse>
    {
        public string Id { get; set; }
        public IFormFileCollection? Files { get; set; }
    }
    public class UploadProductImageFileCommandResponse
    {
    }
    public class UploadProductImageFileCommandHandler : IRequestHandler<UploadProductImageFileCommandRequest, UploadProductImageFileCommandResponse>
    {
        readonly IStorageService _storageService;
        readonly IProductReadRepository _productReadRepository;
        readonly IProductImageFileWriteRepository _productImageFileWriteRepository;

        public UploadProductImageFileCommandHandler(IStorageService storageService, IProductReadRepository productReadRepository, IProductImageFileWriteRepository productImageFileWriteRepository)
        {
            _storageService = storageService;
            _productReadRepository = productReadRepository;
            _productImageFileWriteRepository = productImageFileWriteRepository;
        }

        public async Task<UploadProductImageFileCommandResponse> Handle(UploadProductImageFileCommandRequest request, CancellationToken cancellationToken)
        {
            List<(string fileName, string pathOrContainer)> result = await _storageService.UploadAsync("photo-images", request.Files);

            Product product = await _productReadRepository.GetByIdAsync(request.Id);

            await _productImageFileWriteRepository.AddRangeAsync(result.Select(r => new ProductImageFile()
            {
                FileName = r.fileName,
                Path = r.pathOrContainer,
                Storage = _storageService.StorageName,
                Products = new List<Product>() { product }
            }).ToList()); ;

            await _productImageFileWriteRepository.SaveAsync();
            return new();
        }
    }
}
