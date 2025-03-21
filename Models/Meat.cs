using System.ComponentModel.DataAnnotations.Schema;

namespace Recipesbook.Models
{
    [Table("Meat")]  

    public class Meat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }
}
