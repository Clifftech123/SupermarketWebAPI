﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupermarketWebAPI.Domain.Models.Queries;
using SupermarketWebAPI.Domain.Services;
using SupermarketWebAPI.Resources;

namespace SupermarketWebAPI.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductsController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(QueryResultResource<ProductResource>), 200)]
        public async Task<QueryResultResource<ProductResource>> ListAsync([FromQuery] ProductsQueryResource query)
        {
            var productsQuery = _mapper.Map<ProductsQuery>(query);
            var queryResult = await _productService.ListAsync(productsQuery);

            return _mapper.Map<QueryResultResource<ProductResource>>(queryResult);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProductResource), 201)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> PostAsync([FromBody] SaveProductResource resource)
        {
            var product = _mapper.Map<Product>(resource);
            var result = await _productService.SaveAsync(product);

            if (!result.Success)
            {
                return BadRequest(new ErrorResource(result.Message!));
            }

            var productResource = _mapper.Map<ProductResource>(result.Resource!);
            return Ok(productResource);
        }

        // POST: products/draft
        // Creates a new product and saves it as a draft.
        [HttpPost("draft")]
        [ProducesResponseType(typeof(ProductResource), 201)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> SaveDraftAsync([FromBody] SaveProductResource resource)
        {
            // Map the SaveProductResource to a Product model.
            var product = _mapper.Map<Product>(resource);

            // Set the product as a draft and save it.
            var result = await _productService.SaveDraftAsync(product);

            // If the product was not saved successfully, return a 400 Bad Request
            // status with the error message.
            if (!result.Success)
            {
                return BadRequest(new ErrorResource(result.Message!));
            }

            // If the product was saved successfully, map the Product model to a
            // ProductResource, and return a 201 Created status with the product data.
            var productResource = _mapper.Map<ProductResource>(result.Resource!);
            return CreatedAtAction(nameof(SaveDraftAsync), new { id = productResource.Id }, productResource);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProductResource), 201)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> PutAsync(int id, [FromBody] SaveProductResource resource)
        {
            var product = _mapper.Map<Product>(resource);
            var result = await _productService.UpdateAsync(id, product);

            if (!result.Success)
            {
                return BadRequest(new ErrorResource(result.Message!));
            }

            var productResource = _mapper.Map<ProductResource>(result.Resource!);
            return Ok(productResource);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ProductResource), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _productService.DeleteAsync(id);

            if (!result.Success)
            {
                return BadRequest(new ErrorResource(result.Message!));
            }

            var categoryResource = _mapper.Map<ProductResource>(result.Resource!);
            return Ok(categoryResource);
        }
    }
}
