using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IntroSEProject.API.Models
{
    public class ItemModel
    {
        public int Id { get; set; }
        [StringLength(1000)]
        public string Thumbnail { get; set; }
        [StringLength(100)]
        [Required]
        public string Name { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int Stock { get; set; }
        public string? Images { get; set; }
        public int? CategoryId { get; set; }
    }
}
