using ArzonOL.Dtos.ProductDtos;
using ArzonOL.Models;
using ArzonOL.Services.ProductServeice.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO.Compression;
using System.Threading.Tasks;

namespace ArzonOL.Controllers.Product
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<IEnumerable<ProductModel>>))]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("GetWithPagination")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<IEnumerable<ProductModel>>))]
        public async Task<IActionResult> GetProductWithPagination(int page, int limit)
        {
            if (page <= 0 || limit <= 0)
                return BadRequest("Page or limit cannot be less than or equal to zero");

            var paginatedProducts = await _productService.GetWithPaginationAsync(page, limit);
            return Ok(paginatedProducts);
        }

        [HttpPost("createProduct")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<ProductModel>))]
        public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
        {
            try
            {
                if (createProductDto == null)
                    return BadRequest("Product cannot be null");

                var createdProduct = await _productService.CreateProductAsync(createProductDto);
                return Ok(createdProduct);
                
            }
            catch (System.Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost("uploadProductFiles")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result))]
        public async Task<IActionResult> UploadProductImage(IList<IFormFile> files, Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Id cannot be empty");

            if (files == null)
                return BadRequest("file cannot be null");

             await _productService.SaveFile(files, id);
            return Ok();
        }

        [HttpPut("updateProduct")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<ProductModel>))]
        public async Task<IActionResult> UpdateProduct(UpdateProductDto updateProductDto)
        {
            if (updateProductDto == null)
                return BadRequest("Product cannot be null");

            var updatedProduct = await _productService.UpdateAsync(updateProductDto);
            return Ok(updatedProduct);
        }

        [HttpDelete("deleteProduct")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result))]
        public async Task<IActionResult> DeleteProduct([FromForm]Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Id cannot be empty");

            var result = await _productService.Remove(id);
            return Ok(result);
        }

        [HttpGet("getWithId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<ProductModel>))]
        public async Task<IActionResult> GetWithId(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Id cannot be null");

            var product = await _productService.GetById(id);
            return Ok(product);
        }
        
        [HttpGet("files")]
        public async Task<IActionResult> DownloadFiles(Guid id)
        {
            var product = await _productService.GetById(id);

            var fileNamesString = product.Data!.ProductMedias;

            if (fileNamesString is null)
            {
                return NotFound();
            }

            var fileNames = _productService.NamesSplitter(fileNamesString);
 
            var filesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            
            var files = Directory.GetFiles(filesDirectory);
            var importantFiles = new List<string>();

            if (files is null)
            {
                return NotFound();
            }
            
            foreach (var path in files)
            {
                if(fileNames.Contains(Path.GetFileName(path)))
                {
                    importantFiles.Add(path);
                }
            }
            var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var filePath in importantFiles)
                {
                    var fileName = Path.GetFileName(filePath);
                    
                    var entry = archive.CreateEntry(fileName!);

                    using (var fileStream = new FileStream(filePath, FileMode.Open))
                    using (var entryStream = entry.Open())
                    {
                        fileStream.CopyTo(entryStream);
                    }
                }
            }
            
            memoryStream.Position = 0;
            return File(memoryStream, "application/octet-stream", "files.zip");
        }

    }
}
