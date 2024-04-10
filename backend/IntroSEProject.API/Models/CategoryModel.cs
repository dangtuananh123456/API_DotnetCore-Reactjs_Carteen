using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IntroSEProject.API.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string Thumbnail { get; set; }
        [StringLength(100)]
        [Required]
        public string Name { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
    }
}
