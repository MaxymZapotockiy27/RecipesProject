namespace Recipesbook.Models
{
    public class Recipe
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Instructions { get; set; }
        public List<string> Ingredients { get; set; }
    }
}
