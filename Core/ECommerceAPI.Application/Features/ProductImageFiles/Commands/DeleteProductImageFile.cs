using ECommerceAPI.Application.Abstractions.Storage;
using ECommerceAPI.Application.Repositories;
using ECommerceAPI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Application.Features.ProductImageFiles.Commands
{
    public class DeleteProductImageFileCommandRequest : IRequest<DeleteProductImageFileCommandResponse>
    {
        public string Id { get; set; }
        public string? ImageId { get; set; }
    }
    public class DeleteProductImageFileCommandResponse
    {
    }
    public class DeleteProductImageFileCommandHandler : IRequestHandler<DeleteProductImageFileCommandRequest, DeleteProductImageFileCommandResponse>
    {
        readonly IProductReadRepository _productReadRepository;
        readonly IProductImageFileWriteRepository _productImageWriteRepository;
        readonly IStorageService _storageService;

        public DeleteProductImageFileCommandHandler(IProductReadRepository productReadRepository, IProductImageFileWriteRepository productImageWriteRepository, IStorageService storageService)
        {
            _productReadRepository = productReadRepository;
            _productImageWriteRepository = productImageWriteRepository;
            _storageService = storageService;
        }

        public async Task<DeleteProductImageFileCommandResponse> Handle(DeleteProductImageFileCommandRequest request, CancellationToken cancellationToken)
        {
            Product? product = await _productReadRepository.Table.Include(p => p.ProductImageFiles).FirstOrDefaultAsync(p => p.Id == Guid.Parse(request.Id));

            ProductImageFile? productImageFile = product?.ProductImageFiles.FirstOrDefault(p => p.Id == Guid.Parse(request.ImageId));



            if (productImageFile != null)
            {
                product?.ProductImageFiles.Remove(productImageFile);
                await _storageService.DeleteAsync(productImageFile.Path, productImageFile.FileName);
            }
            await _productImageWriteRepository.SaveAsync();

            return new();
        }
    }
}
