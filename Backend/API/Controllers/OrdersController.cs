using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;

namespace API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        #region Fields
        private readonly IOrderRepository _repository;
        #endregion

        #region Constructors
        public OrdersController(IOrderRepository repository)
        {
            _repository = repository;
        }
        #endregion

        #region Services
        [HttpGet]
        public IActionResult GetOrders()
        {
            return Ok(_repository.GetAll().Select(c => new { 
                c.OrderId,
                ShipName = $"{c.OrderId} - {c.ShipName} - {c.ShipAddress}"
            }));
        }
        #endregion
    }
}
