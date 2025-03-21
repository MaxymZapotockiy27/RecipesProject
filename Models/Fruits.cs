using System.ComponentModel.DataAnnotations.Schema;

namespace Recipesbook.Models
{
    [Table("Fruits")]  

    public class Fruits
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }
}
