using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class BorrowedBook
    {
        [Key]
        public int BorrowId { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public bool ReturnStatus { get; set; }
        public DateTime BorrowedDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
