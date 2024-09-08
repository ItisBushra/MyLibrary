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
            CreateMap<Review, ReviewDto>();
            CreateMap<ReadingTracking, ReadingTrackingDto>();

            CreateMap<ReviewDto, Review>()
            .ForMember(r => r.Id, opt => opt.Ignore());

            CreateMap<ReadingTrackingDto, ReadingTracking>()
            .ForMember(r => r.Id, opt => opt.Ignore());

            CreateMap<BookDto, Book>()
            .ForMember(b => b.Id, opt => opt.Ignore());
        }
    }
}