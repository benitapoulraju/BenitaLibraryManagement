using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Repositories;

namespace LibraryManagement.Controllers
{
    [Route("[controller]")]
    public class BooksController : Controller
    {
        private readonly IBookRepository _repository;
        
        public BooksController(IBookRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("List")]
        public IActionResult List(int? userId)
        {
            var books = _repository.GetAllBooks();
            var user = _repository.GetUserById(userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid  User ID. Please try again.";
                return RedirectToAction("Index", "Home");
            }
            ViewBag.UserId = userId; 
            return View(books);
          
        }
        
        [HttpGet("Borrow/{bookId}/{userId}")]
        public IActionResult Borrow(int bookId, int userId)
        {
            var book = _repository.GetBookById(bookId); 
            ViewBag.UserId = userId;
            return View("Borrow", book);
        }

        [HttpPost("ConfirmBorrow")]
        public IActionResult ConfirmBorrow(int bookId,int userId)
        {
            _repository.BorrowBook(bookId,userId);
            TempData["SuccessMessage"] = "Borrow was successful";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("Return/{bookId}/{userId}")]
        public IActionResult Return(int bookId, int userId)
        {
            var book = _repository.GetBookById(bookId); 
            var user = _repository.GetUserById(userId);

            if (book == null || user == null)
            {
                TempData["ErrorMessage"] = "Invalid Book ID or User ID. Please try again.";
                return RedirectToAction("Index", "Home");
            }
            ViewBag.UserId = userId;
            return View("Return", book);
        }

        [HttpPost("ConfirmReturn")]
        public async Task<IActionResult> ConfirmReturn(int bookId, int userId)
        {
            await _repository.ReturnBook(bookId, userId);
            TempData["SuccessMessage"] = "Return was successful";
            return RedirectToAction("Index", "Home"); 
            
        }
    }
}
