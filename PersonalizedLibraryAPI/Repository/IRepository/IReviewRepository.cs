using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalizedLibraryAPI.Models;

namespace PersonalizedLibraryAPI.Repository.IRepository
{
    public interface IReviewRepository
    {
        ICollection<Review> GetReviews();
        Review GetReview(int id);
        Review GetReviewByBook(int bookId);
        Book GetBookByReview(int reviewId);
        bool ReviewExists(int bookId);
        bool CreateReview(Review review);
        bool Save();
    }
}