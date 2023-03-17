using ECommerceAPI.Application.Repositories;
using ECommerceAPI.Domain.Entities;
using MediatR;

namespace ECommerceAPI.Application.Features.Products.Queries
{
    public class GetProductByIdQueryRequest : IRequest<GetProductByIdQueryResponse>
    {
        public string Id { get; set; }
    }
    public class GetProductByIdQueryResponse
    {
        public string Name { get; set; }
        public int Stock { get; set; }
        public float Price { get; set; }
    }
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQueryRequest, GetProductByIdQueryResponse>
    {
        readonly IProductReadRepository _productReadRepository;

        public GetProductByIdQueryHandler(IProductReadRepository productReadRepository)
        {
            _productReadRepository = productReadRepository;
        }

        public async Task<GetProductByIdQueryResponse> Handle(GetProductByIdQueryRequest request, CancellationToken cancellationToken)
        {
            Product product = await _productReadRepository.GetByIdAsync(request.Id, false);

            return new()
            {
                Name = product.Name,
                Stock = product.Stock,
                Price = product.Price,
            };
        }
    }

}
