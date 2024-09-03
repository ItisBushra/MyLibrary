using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalizedLibraryAPI.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Name{ get; set; }
        public string WritersName { get; set; }
        public string? Image { get; set; }        
    }
}