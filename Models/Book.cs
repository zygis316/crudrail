using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCRUDApp.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        [Required(ErrorMessage = "Book name required")]
        [StringLength(250)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Author required")]
        [StringLength(150)]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = "Number of pages is required")]
        [Range(1, 1000, ErrorMessage = "Pages must be between 1 and 1000")]
        public int Pages { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 100.00, ErrorMessage = "Price must be between 0.01 and 100")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public bool Availability { get; set; }
    }
}