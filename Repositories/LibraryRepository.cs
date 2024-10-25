using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Data;
using LibraryManagement.Models;

namespace LibraryManagement.Repositories
{
    public abstract class RepositoryBase
    {
        protected readonly LibraryContext _context;

        protected RepositoryBase(LibraryContext context)
        {
            _context = context;
        }
    }

    public interface IBookOperations
    {
        IEnumerable<Book> GetAllBooks();
        Book GetBookById(int bookId);
        User GetUserById(int? userId);
        void BorrowBook(int bookId, int userId);
        Task ReturnBook(int bookId, int userId);
    }

    public class LibraryRepository : RepositoryBase, IBookOperations
    {
        private static readonly object _borrowLock = new object();
        private static readonly object _returnLock = new object();

        public LibraryRepository(LibraryContext context) : base(context) { }

        public IEnumerable<Book> GetAllBooks()
        {
            return _context.Books.ToList();
        }

        public Book GetBookById(int bookId)
        {
            return _context.Books.FirstOrDefault(b => b.BookId == bookId);
        }

        public User GetUserById(int? userId)
        {
            return _context.Users.FirstOrDefault(b => b.UserId == userId);
        }

        public void BorrowBook(int bookId, int userId)
        {
            lock (_borrowLock) 
            {
                var book = _context.Books.FirstOrDefault(b => b.BookId == bookId);
                var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

                if (book == null)
                {
                    throw new ArgumentException("Book not found.");
                }

                if (user == null)
                {
                    throw new ArgumentException("User not found.");
                }

                if (book.AvailableCopies > 0)
                {
                    var borrowedBook = new BorrowedBook
                    {
                        BookId = bookId,
                        UserId = userId,
                        BorrowedDate = DateTime.Now
                    };

                    _context.BorrowedBooks.Add(borrowedBook);
                    book.AvailableCopies--;

                    _context.SaveChanges();
                }
                else
                {
                    throw new InvalidOperationException("This book is not available for borrowing.");
                }
            }
        }

        public async Task ReturnBook(int bookId, int userId)
        {
            await Task.Run(() =>
            {
                lock (_returnLock) 
                {
                    var book = _context.Books.FirstOrDefault(b => b.BookId == bookId);
                    var borrowedBook = _context.BorrowedBooks.FirstOrDefault(bb => bb.BookId == bookId &&
                        bb.UserId == userId && !bb.ReturnStatus && bb.ReturnDate == null);

                    if (borrowedBook == null)
                    {
                            throw new InvalidOperationException("No borrowed entry found for this book and user.");
                    }

                    borrowedBook.ReturnStatus = true;
                    borrowedBook.ReturnDate = DateTime.Now;

                    if (book == null)
                    {
                        throw new ArgumentException("Book not found.");
                    }

                    if (borrowedBook.UserId == userId)
                    {
                        book.AvailableCopies++;
                        _context.SaveChanges();
                    }
                    else
                    {
                        throw new InvalidOperationException("This book was not borrowed by this user.");
                    }
                }
            });
        }
    }
}


