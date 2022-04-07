using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using API.DTOs;

namespace API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        #region Fields
        private readonly IProductRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IOrderRepository _orderRepository;
        #endregion

        #region Constructors
        public ProductsController(IProductRepository repository
                                ,ICategoryRepository categoryRepository
                                ,ISupplierRepository supplierRepository
                                ,IOrderDetailRepository orderDetailRepository
                                ,IOrderRepository orderRepository)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _supplierRepository = supplierRepository;
            _orderDetailRepository = orderDetailRepository;
            _orderRepository = orderRepository;
        }
        #endregion

        #region Product Services
        [HttpGet]
        public IActionResult GetProducts()
            => Ok(_repository.GetAll().Select(p => new
            {
                p.ProductId,
                p.ProductName,
                p.UnitsOnOrder,
                p.UnitsInStock,
                p.UnitPrice,
                p.ReorderLevel,
                p.QuantityPerUnit,
                p.Discontinued,
                p.CategoryId,
                CategoryName = p.CategoryId.HasValue ? _categoryRepository.GetById(p.CategoryId.Value)?.CategoryName : null,
                p.SupplierId,
                SupplierName = p.SupplierId.HasValue ? _supplierRepository.GetById(p.SupplierId.Value)?.ContactName : null
            }));

        [HttpGet("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var product = _repository.GetById(id);

            return product is null 
                ? NotFound($"{nameof(Product)} with id='{id}' does not exist!") 
                : Ok(new Product() { 
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        CategoryId = product.CategoryId,
                        Category = product.CategoryId.HasValue ? _categoryRepository.GetById(product.CategoryId.Value) : null,
                        SupplierId = product.SupplierId,
                        Supplier = product.SupplierId.HasValue ? _supplierRepository.GetById(product.SupplierId.Value) : null,
                        Discontinued = product.Discontinued,
                        QuantityPerUnit = product.QuantityPerUnit,
                        ReorderLevel = product.ReorderLevel,
                        UnitPrice = product.UnitPrice,
                        UnitsInStock = product.UnitsInStock,
                        UnitsOnOrder = product.UnitsOnOrder
                    });
        }

        [HttpPost]
        public IActionResult CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"{nameof(Product)} has invalid data!");
            }

            var createProduct = _repository.Add(new Product()
            {
                ProductId = default(int),
                ProductName = createProductDto.ProductName,
                Discontinued = createProductDto.Discontinued,
                QuantityPerUnit = createProductDto.QuantityPerUnit,
                ReorderLevel = createProductDto.ReorderLevel,
                UnitPrice = createProductDto.UnitPrice,
                UnitsInStock = createProductDto.UnitsInStock,
                UnitsOnOrder = createProductDto.UnitsOnOrder,
                CategoryId = createProductDto.CategoryId,
                SupplierId = createProductDto.SupplierId
            });

            if (createProduct is null)
            {
                return BadRequest($"An error ocurred when inserting {nameof(Product)}!");
            }

            if (createProductDto.CreateOrderDetails?.Any() ?? false)
            {
                _orderDetailRepository.AddRange(createProductDto.CreateOrderDetails.Select(od => new OrderDetail { 
                    OrderId = od.OrderId,
                    Discount = od.Discount > default(int) ? 1 : default(int),
                    ProductId = createProduct.ProductId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }));
            }

            return CreatedAtAction(nameof(GetById), new { id = createProduct.ProductId }, createProduct);
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateProduct([FromRoute] int id, [FromBody] UpdateProductDto updateProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"{nameof(Product)} has invalid data!");
            }

            if (id != updateProductDto.ProductId)
            {
                return BadRequest($"{nameof(Product)} with id='{id}' is not valid!");
            }

            var product = _repository.GetById(id);

            if (product is null)
            {
                return NotFound($"{nameof(Product)} with id='{id}' does not exist!");
            }

            var updateProduct = _repository.Update(new Product
            {
                ProductId = updateProductDto.ProductId,
                ProductName = updateProductDto.ProductName,
                Discontinued = updateProductDto.Discontinued,
                QuantityPerUnit = updateProductDto.QuantityPerUnit,
                ReorderLevel = updateProductDto.ReorderLevel,
                UnitPrice = updateProductDto.UnitPrice,
                UnitsInStock = updateProductDto.UnitsInStock,
                UnitsOnOrder = updateProductDto.UnitsOnOrder,
                CategoryId = updateProductDto.CategoryId,
                SupplierId = updateProductDto.SupplierId
            });

            if (updateProduct is null)
            {
                return BadRequest($"An error occured when updating {nameof(Product)} with id='{id}'!");
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteProductById([FromRoute] int id)
        {
            var product = _repository.GetById(id);

            if (product is null)
            {
                return NotFound($"{nameof(Product)} with id='{id}' does not exist!");
            }

            if (!_repository.Remove(product))
            {
                return BadRequest($"An error occured when removing {nameof(Product)} with id='{id}'!");
            }

            return NoContent();
        }

        #endregion

        #region OrderDetail Services
        [HttpGet("{productId:int}/orderDetails")]
        public IActionResult GetOrderDetailsByProductId([FromRoute] int productId)
        {
            var product = _repository.GetById(productId);

            if (product is null)
            {
                return NotFound($"{nameof(Product)} with id='{productId}' does not exist!");
            }

            var orderDetails = _orderDetailRepository.Find(od => od.ProductId == productId).Select(od => new { 
                od.OrderId,
                ShipName = od.OrderId != default(int) ? _orderRepository.GetById(od.OrderId)?.ShipName : null,
                Discount = od.Discount > default(int) ? 1 : default(float),
                od.ProductId,
                product.ProductName,
                od.UnitPrice,
                od.Quantity,
            });

            return Ok(orderDetails);
        }

        [HttpGet("{productId:int}/orderDetails/{id:int}")]
        public IActionResult GetOrderDetailByProductIdAndId([FromRoute] int productId, [FromRoute] int id)
        {
            var orderDetail = _orderDetailRepository.Find(od => od.ProductId == productId && od.OrderId == id).FirstOrDefault();

            if (orderDetail is null)
            {
                return NotFound($"{nameof(OrderDetail)} with id='{id}' does not exist!");
            }

            if (productId != orderDetail.ProductId)
            {
                return BadRequest($"{nameof(OrderDetail)}'s ProductId and {nameof(Product)} Id's do not match");
            }

            return Ok(orderDetail);
        }

        [HttpGet("{productId:int}/orderDetails/{id:int}/check")]
        public IActionResult checkIfOrderDetailExists([FromRoute] int productId, [FromRoute] int id)
        {
            var product = _repository.GetById(productId);

            if (product is null)
            {
                return NotFound($"{nameof(Product)} with id='{id}' does not exist!");
            }

            var order = _orderRepository.GetById(id);

            if (order is null)
            {
                return NotFound($"{nameof(Order)} with id='{id}' does not exist!");
            }

            var orderDetail = _orderDetailRepository.Find(od => od.ProductId == productId && od.OrderId == id).FirstOrDefault();

            return Ok(orderDetail != null);
        }

        [HttpPost("{productId:int}/orderDetails")]
        public IActionResult CreateOrderDetailByProductId([FromRoute] int productId, [FromBody] CreateOrderDetailDto createOrderDetailDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"{nameof(OrderDetail)} has invalid data!");
            }

            var product = _repository.GetById(productId);

            if (product is null)
            {
                return NotFound($"{nameof(Product)} with id='{productId}' does not exist!");
            }

            var order = _orderRepository.GetById(createOrderDetailDto.OrderId);

            if (order is null)
            {
                return NotFound($"{nameof(Order)} with id='{createOrderDetailDto.OrderId}' does not exist!");
            }

            var createdOrderDetail = _orderDetailRepository.Add(new OrderDetail
            {
                OrderId = createOrderDetailDto.OrderId, // default(int)
                Discount = createOrderDetailDto.Discount > default(int) ? 1 : default(float),
                ProductId = productId,
                Quantity = createOrderDetailDto.Quantity,
                UnitPrice = createOrderDetailDto.UnitPrice
            });

            if (createdOrderDetail is null)
            {
                return BadRequest($"An error occured when inserting {nameof(OrderDetail)}!");
            }

            return CreatedAtAction(nameof(GetOrderDetailByProductIdAndId), new { productId = product.ProductId, id = createdOrderDetail.OrderId }, createdOrderDetail);
        }

        [HttpPut("{productId:int}/orderDetails/{id:int}")]
        public IActionResult UpdateOrderDetailByProductIdAndId([FromRoute] int productId, [FromRoute] int id, [FromBody] UpdateOrderDetailDto updateOrderDetailDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"{nameof(OrderDetail)} has invalid data!");
            }

            if (id != updateOrderDetailDto.OrderId)
            {
                return BadRequest($"{nameof(Order.OrderId)}s do not match!");
            }

            var product = _repository.GetById(productId);

            if (product is null)
            {
                return NotFound($"{nameof(Product)} with id='{productId}' does not exist!");
            }

            var order = _orderRepository.GetById(updateOrderDetailDto.OrderId);

            if (order is null)
            {
                return NotFound($"{nameof(Order)} with id='{updateOrderDetailDto.OrderId}' does not exist!");
            }

            var orderDetail = _orderDetailRepository.Find(od => od.OrderId == updateOrderDetailDto.OrderId && od.ProductId == product.ProductId).FirstOrDefault();
            if (orderDetail is null)
            {
                return NotFound($"{nameof(OrderDetail)} with {nameof(Product.ProductId)}='{product.ProductId}' and {nameof(OrderDetail.OrderId)}='{updateOrderDetailDto.OrderId} does not exist!'");
            }

            var updatedOrderDetail = _orderDetailRepository.Update(new OrderDetail { 
                OrderId = updateOrderDetailDto.OrderId,
                Discount = updateOrderDetailDto.Discount > default(int) ? 1 : default(float),
                ProductId = product.ProductId,
                Quantity = updateOrderDetailDto.Quantity,
                UnitPrice = updateOrderDetailDto.UnitPrice
            });

            if (updatedOrderDetail is null)
            {
                return BadRequest($"An error occured when updating {nameof(OrderDetail)}!");
            }

            return NoContent();
        }

        [HttpDelete("{productId:int}/orderDetails/{id:int}")]
        public IActionResult DeleteOrderDetailByProductIdAndId([FromRoute] int productId, [FromRoute] int id)
        {
            var product = _repository.GetById(productId);

            if (product is null)
            {
                return NotFound($"{nameof(Product)} with id='{productId}' does not exist!");
            }

            var orderDetail = _orderDetailRepository.Find(od => od.OrderId == id && od.ProductId == productId).FirstOrDefault();

            if (orderDetail is null)
            {
                return NotFound($"{nameof(OrderDetail)} with id='{id}' does not exist!");
            }

            if (orderDetail.ProductId != productId)
            {
                return BadRequest($"{nameof(OrderDetail.ProductId)}s do not match!");
            }

            if (!_orderDetailRepository.Remove(orderDetail))
            {
                return BadRequest($"An error occured when removing {nameof(OrderDetail)} with id='{orderDetail.OrderId}'");
            }

            return NoContent();
        }
        #endregion
    }
}
