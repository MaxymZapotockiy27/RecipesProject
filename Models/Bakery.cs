using System.ComponentModel.DataAnnotations.Schema;

namespace Recipesbook.Models
{
    [Table("Bakery")]  
    public class Bakery
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }
}
