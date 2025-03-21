using System.ComponentModel.DataAnnotations.Schema;

namespace Recipesbook.Models
{
    [Table("Dairy")]

    public class Dairy
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }
}
