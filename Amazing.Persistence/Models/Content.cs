using Amazing.Persistence.Enumerators;
using Amazing.Persistence.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace Amazing.Persistence.Models
{
    public class Content : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public int Sort { get; set; }
        public DateTime CreationDate { get; set; }
        public EContentType Type { get; set; }

        // Text + Quote
        public string Text { get; set; }

        // Image
        public string ImageUrl { get; set; }
        public int? Height { get; set; }
        public int? Width { get; set; }

        // Video
        public string VideoUrl { get; set; }
        public int? Duration { get; set; }

        // Google Map
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }

        // Quote
        public string Author { get; set; }
    }
}