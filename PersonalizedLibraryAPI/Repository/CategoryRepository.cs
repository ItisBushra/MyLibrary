using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalizedLibraryAPI.Data;
using PersonalizedLibraryAPI.Models;
using PersonalizedLibraryAPI.Repository.IRepository;

namespace PersonalizedLibraryAPI.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DBContext _dBContext;
        public CategoryRepository(DBContext dBContext)
        {
            _dBContext = dBContext;
        }
        public bool CategoryExists(int id)
        {
            return _dBContext.Categories.Any(c=>c.Id == id);
        }

        public ICollection<Book> GetBooksByCategory(int categoryId)
        {
            //nestd entities / navigation properites
            return _dBContext.BookCategories.Where(c=> c.CategoryId == categoryId).Select(b=>b.Book).ToList();
        }

        public ICollection<Category> GetCategories()
        {
            return _dBContext.Categories.ToList();
        }

        public Category GetCategory(int id)
        {
            return _dBContext.Categories.Where(c=>c.Id == id).FirstOrDefault();
        }
    }
}