using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PersonalizedLibraryAPI.Data;
using PersonalizedLibraryAPI.Repository.IRepository;
using PersonalizedLibraryAPI.Models;
using AutoMapper;

namespace PersonalizedLibraryAPI.Repository
{
    public class StatusRepository : IStatusRepository
    {
        private readonly DBContext _dBContext;
        public StatusRepository(DBContext dBContext)
        {
            _dBContext = dBContext;
        }
        public ICollection<Status> GetStatuses()
        {
            return _dBContext.Statuses.ToList();
        }
        public Status GetStatus(int id)
        {
            return _dBContext.Statuses
            .Where(s=>s.Id == id).FirstOrDefault();
        }
        public Status GetStatusByBook(int bookId)
        {
            return _dBContext.Books.Where(b=>b.Id == bookId)
            .Select(s=>s.Status).FirstOrDefault();
        }
        public ICollection<Book> GetBooksFromAStatus(int statusId)
        {
            return _dBContext.Books.
            Where(s=>s.Status.Id == statusId).ToList();
        }
        public bool StatusExists(int id)
        {
            return _dBContext.Statuses
            .Any(s=>s.Id == id);
        }
    }
}