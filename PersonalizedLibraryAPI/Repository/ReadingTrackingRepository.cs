using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalizedLibraryAPI.Repository.IRepository;
using PersonalizedLibraryAPI.Data;
using PersonalizedLibraryAPI.Models;
using AutoMapper;

namespace PersonalizedLibraryAPI.Repository
{
    public class ReadingTrackingRepository : IReadingTrackingRepository
    {
        private readonly DBContext _dBContext;
        public ReadingTrackingRepository(DBContext dBContext)
        {
            _dBContext = dBContext;
        }
        public bool ReadingTrackingExists(int id)
        {
            return _dBContext.ReadingTrackings
            .Any(r=>r.Id == id);
        }
        public ICollection<ReadingTracking> GetReadingTrackings() 
        {
            return _dBContext.ReadingTrackings
            .OrderBy(r=>r.Id).ToList();
        }
        public ReadingTracking GetReadingTracking(int id)
        {
            return _dBContext.ReadingTrackings
                    .Where(r=>r.Id == id)
                    .FirstOrDefault();
        }
        public ReadingTracking GetReadingTrackingByBook(int bookId)
        {
            return _dBContext.Books
            .Where(b=>b.Id == bookId)
            .Select(r=>r.ReadingTracking)
            .FirstOrDefault();
        }
        public Book GetBookByReadingTracking(int readingTrackingId)
        {
            return _dBContext.ReadingTrackings
            .Where(r=>r.Id == readingTrackingId)
            .Select(b=>b.Book).FirstOrDefault();
        }
        public bool CreateReadingTracking(ReadingTracking readingTracking)
        {
            _dBContext.Add(readingTracking);
            return Save();
        }
        public bool UpdateReadingTracking(ReadingTracking readingTracking)
        {
            _dBContext.Update(readingTracking);
            return Save();
        }
        public bool DeleteReadingTracking
        (ReadingTracking readingTracking)
        {
            _dBContext.Remove(readingTracking);
            return Save();
        }
        public bool Save()
        {
            var saved = _dBContext.SaveChanges();
            return saved > 0 ? true: false;
        }  
          
    }
}