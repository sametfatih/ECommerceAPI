using ECommerceAPI.Application.Repositories;
using ECommerceAPI.Application.RequestParameters;
using MediatR;

namespace ECommerceAPI.Application.Features.Products.Queries
{
    public class GetAllProductsQueryRequest : Pagination, IRequest<GetAllProductQueryResponse>
    {
    }

    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQueryRequest, GetAllProductQueryResponse>
    {
        readonly IProductReadRepository _productReadRepository;

        public GetAllProductsQueryHandler(IProductReadRepository productReadRepository)
        {
            _productReadRepository = productReadRepository;
        }

        public async Task<GetAllProductQueryResponse> Handle(GetAllProductsQueryRequest request, CancellationToken cancellationToken)
        {
            var totalCount = _productReadRepository.GetAll(false).Count();
            var products = _productReadRepository.GetAll(false).Skip(request.Page * request.Size).Take(request.Size).Select(p => new
            {
                p.Id,
                p.Name,
                p.Stock,
                p.Price,
                p.CreatedDate,
                p.UpdatedDate
            }).ToList();

            return new()
            {
                TotalCount = totalCount,
                Products = products
            };
        }
    }

    public class GetAllProductQueryResponse
    {
        public int TotalCount { get; set; }
        public object Products { get; set; }
    }
}
