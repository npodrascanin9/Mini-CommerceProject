using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using Domain;

namespace API.Controllers
{
    [Route("api/suppliers")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        #region Fields
        private readonly ISupplierRepository _repository;
        #endregion

        #region Constructors
        public SuppliersController(ISupplierRepository repository)
        {
            _repository = repository;
        }
        #endregion

        #region Services
        [HttpGet]
        public IActionResult GetSuppliers()
            => Ok(_repository.GetAll());

        [HttpGet("{id:int}")]
        public IActionResult GetSupplierById([FromRoute] int id)
        {
            var supplier = _repository.GetById(id);

            if (supplier is null)
            {
                return NotFound($"{nameof(Supplier)} with id='{id}' does not exist!");
            }

            return Ok(supplier);
        }
        #endregion
    }
}
