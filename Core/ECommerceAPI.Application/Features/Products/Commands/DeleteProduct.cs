using ECommerceAPI.Application.Repositories;
using MediatR;

namespace ECommerceAPI.Application.Features.Products.Commands
{
    public class DeleteProductCommandRequest : IRequest<DeleteProductCommandResponse>
    {
        public string Id { get; set; }
    }
    public class DeleteProductCommandResponse
    {
    }
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommandRequest, DeleteProductCommandResponse>
    {
        readonly IProductReadRepository _productReadRepository;
        readonly IProductWriteRepository _productWriteRepository;

        public DeleteProductCommandHandler(IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository)
        {
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
        }

        public async Task<DeleteProductCommandResponse> Handle(DeleteProductCommandRequest request, CancellationToken cancellationToken)
        {
            await _productWriteRepository.RemoveAsync(request.Id);
            await _productWriteRepository.SaveAsync();
            return new();
        }
    }
}
