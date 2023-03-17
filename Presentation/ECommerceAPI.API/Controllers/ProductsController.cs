using ECommerceAPI.Application.Features.ProductImageFiles.Commands;
using ECommerceAPI.Application.Features.ProductImageFiles.Queries;
using ECommerceAPI.Application.Features.Products.Commands;
using ECommerceAPI.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ECommerceAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetAllProductsQueryRequest getAllProductsQueryRequest)
        {
            GetAllProductQueryResponse response = await _mediator.Send(getAllProductsQueryRequest);
            return (Ok(response));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] GetProductByIdQueryRequest getProductByIdQueryRequest)
        {
            GetProductByIdQueryResponse response = await _mediator.Send(getProductByIdQueryRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateProductCommandRequest createProductCommandRequest)
        {
            CreateProductCommandResponse response = await _mediator.Send(createProductCommandRequest);
            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UpdateProductCommandRequest updateProductCommandRequest)
        {
            UpdateProductCommandResponse response = await _mediator.Send(updateProductCommandRequest);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] DeleteProductCommandRequest deleteProductCommandRequest)
        {
            DeleteProductCommandResponse response = await _mediator.Send(deleteProductCommandRequest);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Upload([FromQuery] UploadProductImageFileCommandRequest uploadProductImageFileCommandRequest)
        {
            UploadProductImageFileCommandResponse response = await _mediator.Send(uploadProductImageFileCommandRequest);
            return Ok();
        }


        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetProductImages([FromRoute] GetProductImageFilesQueryRequest getProductImageFilesQueryRequest)
        {
            List<GetProductImageFilesQueryResponse> response = await _mediator.Send(getProductImageFilesQueryRequest);
            return Ok(response);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteProductImage([FromRoute] DeleteProductImageFileCommandRequest deleteProductImageFileCommandRequest, [FromQuery] string imageId)
        {
            deleteProductImageFileCommandRequest.ImageId = imageId;
            DeleteProductImageFileCommandResponse response = await _mediator.Send(deleteProductImageFileCommandRequest);
            return Ok();
        }
    }
}
