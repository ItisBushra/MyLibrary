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
            CreateMap<Book, BookDto>().ReverseMap();
            CreateMap<Category, CategoryDto>();
            CreateMap<Status, StatusDto>();
            CreateMap<Review, ReviewDto>().ReverseMap();
            CreateMap<ReadingTracking, ReadingTrackingDto>().ReverseMap();

            CreateMap<Book, BookDetailsDto>()
            .ForMember(dest => dest.Book, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Review, opt => opt.MapFrom(src => src.Review))
            .ForMember(dest => dest.ReadingTracking, opt => opt.MapFrom(src => src.ReadingTracking))
            .ForMember(dest => dest.BookCategories, opt => opt.MapFrom(src => src.BookCategories))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ReverseMap();

            CreateMap<BookCategory, CategoryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Category.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Category.Name));
        }
    }
}