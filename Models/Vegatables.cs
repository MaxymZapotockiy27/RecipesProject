using System.ComponentModel.DataAnnotations.Schema;

namespace Recipesbook.Models
{
    [Table("Vegetables")]  

    public class Vegatables
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }
}
