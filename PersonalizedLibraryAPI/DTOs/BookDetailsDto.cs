using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalizedLibraryAPI.DTOs.Account;

namespace PersonalizedLibraryAPI.DTOs
{
    public class BookDetailsDto
    {
        public BookDto Book { get; set; }
        public ReviewDto Review { get; set; }
        public ReadingTrackingDto ReadingTracking { get; set; }
        public List<CategoryDto> BookCategories { get; set; }
        public StatusDto Status { get; set; }
        public NewUserDto NewUser { get; set; }
    }
}