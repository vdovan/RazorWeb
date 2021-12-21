using System;
using System.ComponentModel.DataAnnotations;

namespace RazorWeb.Models
{
    public class Article
    {
        [Key]
        public int ID { get; set; }
        [StringLength(200)]
        public string? Title { get; set; }
        [StringLength(1000)]
        public string? Content { get; set; }
        [StringLength(50)]
        public string? Publisher { get; set; }
        public DateTime PublishedDate { get; set; }
    }
}