using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryProductsController : ControllerBase
    {
        private readonly IInventoryProductsApplication _inventoryProductsApplication;

        public InventoryProductsController(IInventoryProductsApplication inventoryProductsApplication)
        {
            _inventoryProductsApplication = inventoryProductsApplication;
        }

        [Authorize]
        [HttpPost("RegisterCategoryProduct")]
        public async Task<ActionResult<BaseResponse<bool>>> RegisterCategoryProduct([FromBody] CategoryProductsRequestDto request)
        {
            var response = await _inventoryProductsApplication.RegisterCategoryProduct(request);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("GetAllCategoriesProducts")]
        public async Task<ActionResult<BaseResponse<IEnumerable<CategoryProductsResponseDto>>>> GetAllCategoriesProducts()
        {
            var response = await _inventoryProductsApplication.GetAllCategoriesProducts();

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("RegisterProduct")]
        public async Task<ActionResult<BaseResponse<bool>>> RegisterProduct([FromBody] ProductsRequestDto request)
        {
            var response = await _inventoryProductsApplication.RegisterProduct(request);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("GetAllProducts")]
        public async Task<ActionResult<BaseResponse<BaseEntityResponse<ProductsResponseDto>>>> GetAllProducts([FromBody] BaseFiltersRequest filters)
        {
            var response = await _inventoryProductsApplication.GetAllProducts(filters);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("GetProductById/{productId:int}")]
        public async Task<ActionResult<BaseResponse<ProductsResponseDto>>> GetProductById(int productId)
        {
            var response = await _inventoryProductsApplication.GetProductById(productId);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("EditProduct/{variantId:int}")]
        public async Task<ActionResult<BaseResponse<bool>>> EditProduct(int variantId, [FromBody] EditProductRequestDto request)
        {
            var response = await _inventoryProductsApplication.EditProduct(variantId, request);

            if (response.IsSuccess == false && response.Message == "No autorizado")
                return Unauthorized(response);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("DeleteProduct/{productId:int}")]
        public async Task<ActionResult<BaseResponse<bool>>> DeleteProduct(int productId)
        {
            var response = await _inventoryProductsApplication.DeleteProduct(productId);

            if (response.IsSuccess == false && response.Message == "No autorizado")
                return Unauthorized(response);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("RegisterEntryAndExitProduct")]
        public async Task<ActionResult<BaseResponse<bool>>> RegisterEntryAndExitProduct([FromBody] EntryAndExitProductRequestDto request)
        {
            var response = await _inventoryProductsApplication.RegisterEntryAndExitProduct(request);

            if (response.IsSuccess == false)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
