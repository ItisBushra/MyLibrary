using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalizedLibraryAPI.Models;

namespace PersonalizedLibraryAPI.Repository.IRepository
{
    public interface IReadingTrackingRepository
    {
        ICollection<ReadingTracking> GetReadingTrackings();
        ReadingTracking GetReadingTracking(int id);
        ReadingTracking GetReadingTrackingByBook(int bookId);
        Book GetBookByReadingTracking(int readingTrackingId);
        bool ReadingTrackingExists(int readingTrackingId);
        bool CreateReadingTracking(ReadingTracking readingTracking);
        bool UpdateReadingTracking(ReadingTracking readingTracking);
        bool Save();
    }
}