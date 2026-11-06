using System.Net.Mime;
using Backend.Api.Files;
using Backend.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers;

[ApiController]
[Route("api/v1/products/{productCode}/image")]
public sealed class ProductImageController : Controller
{
    private const long maxFileBytes = 5L * 1024 *1024;

    private IProductService _productService;
    private readonly IProductImageStorage _imageStorage;

    public ProductImageController(IProductService productService,IProductImageStorage imageStorage)
    {
        _productService=productService;
        _imageStorage=imageStorage;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(6_291_456)]
    [RequestFormLimits(MultipartBodyLengthLimit = 6_291_456)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upload([FromRoute]string productCode, [FromForm] IFormFile? file, CancellationToken ct) 
    {
        var safeCode = productCode.Trim().ToUpperInvariant();

        var validCode = safeCode.Length is >=1 and <=30 && safeCode.All(character => (character >= 'A' && character <= 'Z') || (character >= '0' && character <= '9') || (character is '-' or '_'));

        if (!validCode)
        {
            return Problem(statusCode:400, title:"Invalid ProductCOde", detail:"Format kode product tidak valid");
        }

        if(file is null)
        {
            return Problem(statusCode: 400,title:"File is required",detail:"filenya diisi lah bro");
        }

        // * Tolak file kosong atau lebih dari 5mb
        if(file.Length <=0 || file.Length > maxFileBytes)
        {
            return Problem(statusCode: 400,title:"Invalid size file",detail: "ukurannya ga cocok bro");    
        }

        var extention = Path.GetExtension(file.FileName);

        if(!string.Equals(extention, ".jpg", StringComparison.OrdinalIgnoreCase))
        {
            return Problem(statusCode: 400,title:"Invalid file extention",detail:"cuma jpg bang, yg lain gabisa");
        }

        // * memanggil ProductService
        var product = await _productService.GetByCodeAsync(safeCode,ct);

        if(product is null)
        {
            return Problem(statusCode:404,title:"Product not found",detail:$"Product {safeCode} tidak ditemukan");
        }

        var savedImage = await _imageStorage.SaveAsync(safeCode,file,ct);

        return StatusCode(StatusCodes.Status201Created, new
        {
            message= "product image uploaded successfully",
            productCode= safeCode,
            fileName= savedImage.StoredFileName,
            fileSize= savedImage.SizeInBytes,
            contentType=savedImage.ContentType   
        });
    }



}