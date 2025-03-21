using System.ComponentModel.DataAnnotations.Schema;

namespace Recipesbook.Models
{
    [Table("SpicesAndCondiments")]  

    public class SpicesAndCondiments
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }
}
