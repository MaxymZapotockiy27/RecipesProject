using System.ComponentModel.DataAnnotations.Schema;

namespace Recipesbook.Models
{
    [Table("FloursANDBakingIngredients")]
    public class FloursANDBakingIngredients
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }
}
