using ECommerceAPI.Application.Repositories;
using ECommerceAPI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ECommerceAPI.Application.Features.ProductImageFiles.Queries
{
    public class GetProductImageFilesQueryRequest : IRequest<List<GetProductImageFilesQueryResponse>>
    {
        public string Id { get; set; }
    }
    public class GetProductImageFilesQueryResponse
    {
        public string Id { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
    }
    public class GetProductImageFilesQueryHandler : IRequestHandler<GetProductImageFilesQueryRequest, List<GetProductImageFilesQueryResponse>>
    {
        readonly IProductReadRepository _productReadRepository;
        readonly IConfiguration configuration;

        public GetProductImageFilesQueryHandler(IProductReadRepository productReadRepository, IConfiguration configuration)
        {
            _productReadRepository = productReadRepository;
            this.configuration = configuration;
        }

        public async Task<List<GetProductImageFilesQueryResponse>> Handle(GetProductImageFilesQueryRequest request, CancellationToken cancellationToken)
        {
            Product? product = await _productReadRepository.Table.Include(p => p.ProductImageFiles).FirstOrDefaultAsync(p => p.Id == Guid.Parse(request.Id));

            return product!.ProductImageFiles.Select(p => new GetProductImageFilesQueryResponse
            {
                Id = p.Id.ToString(),
                Path = $"{configuration["BaseStorageUrl"]}/{p.Path}",
                FileName = p.FileName
            }).ToList();
        }
    }
}
