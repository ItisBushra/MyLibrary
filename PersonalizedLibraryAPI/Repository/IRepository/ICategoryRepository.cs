using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalizedLibraryAPI.Models;

namespace PersonalizedLibraryAPI.Repository.IRepository
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategory(int id);
        ICollection<Book> GetBooksByCategory(int categoryId);
        ICollection<Category> GetCategoriesByBook(int BookId);
        bool CategoryExists(int id);
    }
}