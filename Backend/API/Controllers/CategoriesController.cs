using Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using API.Repositories;
using API.DTOs;

namespace API.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        #region Fields
        private readonly ICategoryRepository _repository;
        private readonly IProductRepository _productRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        #endregion

        #region Constructors
        public CategoriesController(ICategoryRepository repository
                                    ,IProductRepository productRepository
                                    ,ISupplierRepository supplierRepository
                                    ,IOrderDetailRepository orderDetailRepository)
        {
            _repository = repository;
            _productRepository = productRepository;
            _supplierRepository = supplierRepository;
            _orderDetailRepository = orderDetailRepository;
        }
        #endregion

        #region Services


        #region Category Services
        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = _repository.GetAll().Select(c => new
            {
                c.CategoryId,
                c.CategoryName,
                c.Description,
                ProductCount = _productRepository.Find(p => p.CategoryId.Value == c.CategoryId)?.Count(),
                Picture = c.Picture?.Any() ?? false
                    ? "data:image/png;base64," + Convert.ToBase64String(c.Picture)
                    : null
            });

            return Ok(categories);
        }

        [HttpPost]
        public IActionResult CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"{nameof(Category)} has invalid data!");
            }

            var category = _repository.Add(new Category
            {
                CategoryId = default(int),
                CategoryName = createCategoryDto.CategoryName,
                Description = createCategoryDto.Description,
                Picture = createCategoryDto.Picture
            });

            if (category is null)
            {
                return BadRequest($"Failed to insert {nameof(Category)}!");
            }

            if (createCategoryDto.CreateProducts?.Any() ?? false)
            {
                _productRepository.AddRange(createCategoryDto.CreateProducts);
            }

            return CreatedAtAction(nameof(GetById), new { id = category.CategoryId }, category);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var category = _repository.GetById(id);

            if (category is null)
            {
                return NotFound($"{nameof(Category)} with id='{id}' not found!");
            }

            return Ok(new {
                category.CategoryId,
                category.CategoryName,
                category.Description,
                Picture = new {
                    Bytes = category.Picture?.Any() ?? false 
                        ? category.Picture 
                        : null,
                    ImgBase64 = category.Picture?.Any() ?? false
                        ? "data:image/png;base64," + Convert.ToBase64String(category.Picture)
                        : null
                }
            });
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateCategory([FromRoute] int id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"{nameof(Category)} has invalid data!");
            }

            if (id != updateCategoryDto.CategoryId)
            {
                return BadRequest($"{nameof(Category)} with id='{id}' is not valid!");
            }

            var category = _repository.GetById(id);

            if (category is null)
            {
                return NotFound($"{nameof(Category)} with id='{id}' does not exist!");
            }

            var updatedCategory = _repository.Update(new Category
            {
                CategoryId = category.CategoryId,
                CategoryName = updateCategoryDto.CategoryName,
                Description = updateCategoryDto.Description,
                Picture = updateCategoryDto.Picture
            });

            if (updatedCategory is null)
            {
                return BadRequest($"An error occured when updating {nameof(Category)}!");
            }

            return NoContent();
        }

        [HttpPost("upload")]
        public IActionResult UploadImage()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];

                if (postedFile?.Length > default(int))
                {
                    using (var ms = new MemoryStream())
                    {
                        postedFile.CopyTo(ms);
                        byte[] fileBytes = ms.ToArray();
                        var picture = fileBytes?.Any() ?? false
                                ? "data:image/png;base64," + Convert.ToBase64String(fileBytes)
                                : null;
                        return Ok(new { 
                            Bytes = fileBytes,
                            ImgBase64 = picture
                        });
                    }
                }

                return Ok(null);
            }
            catch (Exception)
            {

                return BadRequest("Something went wrong");
            }
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteCategoryById([FromRoute] int id)
        {
            var category = _repository.GetById(id);

            if (category is null)
            {
                return NotFound($"{nameof(Category)} with id='{id}' does not exist!");
            }

            if (!_repository.Remove(category))
            {
                return BadRequest($"An error occured when deleting ${nameof(Category)}");
            }

            return NoContent();
        }
        #endregion


        #region Product Services
        [HttpGet("{categoryId:int}/products")]
        public IActionResult GetProductsByCategoryId([FromRoute] int categoryId, [FromQuery] Product filter)
        {
            var category = _repository.GetById(categoryId);

            if (category is null)
            {
                return NotFound($"{nameof(Category)} with id='{categoryId}' is not valid!");
            }

            var filteredProducts = _productRepository.Find(c => 
                (c.CategoryId.Value == categoryId) && 
                (string.IsNullOrWhiteSpace(filter.ProductName) || c.ProductName.ToLower().StartsWith(filter.ProductName.ToLower())) &&
                (filter.SupplierId == null || c.SupplierId == filter.SupplierId)
                ).Select(p => {
                    var supplier = p.SupplierId.HasValue ? _supplierRepository.GetById(p.SupplierId.Value) : null;
                    
                    return new
                    {
                        p.ProductId,
                        p.UnitsOnOrder,
                        p.UnitsInStock,
                        p.UnitPrice,
                        p.ReorderLevel,
                        p.QuantityPerUnit,
                        p.ProductName,
                        Discontinued = p.Discontinued ? "yes" : "no",
                        p.CategoryId,
                        CategoryName = p.CategoryId.HasValue ? _repository.GetById(p.CategoryId.Value)?.CategoryName : String.Empty,
                        p.SupplierId,
                        SupplierName = supplier != null ? string.Concat(supplier.CompanyName, "-", supplier.Country) : String.Empty,
                        OrderDetailCount = _orderDetailRepository.Find(od => od.ProductId == p.ProductId)?.Count()
                    };
                });

            return Ok(filteredProducts);
        }
        #endregion

#endregion

    }
}
