using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class CreateCategoryDto
    {
        [Required]
        [MaxLength(15)]
        public string CategoryName { get; set; }
        [Required]
        public string Description { get; set; }
        public byte[] Picture { get; set; }
        public virtual List<Product> CreateProducts { get; set; }
    }

    public class UpdateCategoryDto
    {
        [Required]
        public int CategoryId { get; set; }
        [Required]
        [MaxLength(15)]
        public string CategoryName { get; set; }
        [Required]
        public string Description { get; set; }
        public byte[] Picture { get; set; }
    }
}
