using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PersonalizedLibraryAPI.DTOs;
using PersonalizedLibraryAPI.Models;

namespace PersonalizedLibraryAPI.Helper
{
    public class MappingProfiles: Profile
    {
        public MappingProfiles()
        {
            CreateMap<Book, BookDto>();
            CreateMap<Category, CategoryDto>();
            CreateMap<Status, StatusDto>();
            CreateMap<Review, ReviewDto>().ReverseMap();
            CreateMap<ReadingTracking, ReadingTrackingDto>().ReverseMap();
        }
    }
}