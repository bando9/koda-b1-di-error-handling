using Backend.Api.DTOs;
using Backend.Api.Interfaces;
using Backend.Api.Models;
using Microsoft.AspNetCore.Mvc;


namespace Backend.Api.Controllers;

[ApiController]
[Route("api/v1/products")]

public sealed class ProductController : ControllerBase
{
    private readonly IProductService _service;

    public ProductController(IProductService service)
    {
        _service=service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await _service.GetAllAsync(cancellationToken);

        return Ok(products);
    }

    // GET Product by productCode
    [HttpGet("{productCode}")]
    [ProducesResponseType(typeof(ProductResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> GetByCode(
        [FromRoute] string productCode,CancellationToken cancellationToken
    )
    {
        var product = await _service.GetByCodeAsync(productCode,cancellationToken);

        return product is null ? Problem(statusCode: StatusCodes.Status404NotFound, title: $"Sorry, product not found"):Ok(product);
    }


    // POST Product
    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductResponse>> PostProduct([FromBody] ProductRequest request,CancellationToken cancellationToken)
    {

        try
        {

            var product = await _service.CreateAsync(request,cancellationToken);

            if(product is null)
            {
                // 409 Conflict
                return Problem(statusCode:StatusCodes.Status409Conflict, title: "Product already exist");
            }

            // 201 created
            return CreatedAtAction(
                nameof(GetByCode),
                new {productCode = product.ProductCode},
                product
            );
        }
        catch (ArgumentException e)
        {
            return Problem(
                statusCode:StatusCodes.Status400BadRequest,title:"Invalid product data!",detail:e.Message
            );
            
        }

    }

    // Delete by prodcutCode 
    [HttpDelete("{productCode}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute]string productCode,CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(productCode, cancellationToken);

        return deleted ? NoContent() : Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Product not found",
            detail: $"No product was found for code {productCode}");
    }
    

    [HttpPut("{productCode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductResponse>> Update([FromRoute] string productCode, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _service.UpdateAsync(productCode, request,cancellationToken);

            return updated is null ? Problem(statusCode: StatusCodes.Status404NotFound,title: $"product with {productCode} code not found.") : Ok(updated);
        }
        catch (ArgumentException e)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title:"Invalid product data",
                detail: e.Message
            );
            
        }
    }



    

}