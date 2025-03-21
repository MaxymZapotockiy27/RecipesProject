using System.ComponentModel.DataAnnotations.Schema;

namespace Recipesbook.Models
{
    [Table("GrainAndPasta")]
    public class GrainAndPasta
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }
}
