using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalizedLibraryAPI.Models
{
    public class Book
    { 
        public int Id { get; set; }
        public string Name{ get; set; }
        public string WritersName { get; set; }
        public string? Image { get; set; }
        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        public Status Status { get; set; }
        public ICollection<BookCategory> BookCategories { get; set; }
        public Review? Review { get; set; }
        public ReadingTracking ReadingTracking { get; set; }
    }
}