using Microsoft.AspNetCore.Mvc;
using Recipesbook.Models;
using Recipesbook.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Http;

namespace Recipesbook.Controllers
{
     public class IngredientConverter : JsonConverter<List<string>>
    {
        public override List<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var ingredients = new List<string>();

            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("Expected start of array");
            }

            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.String:
                        ingredients.Add(reader.GetString());
                        break;

                    case JsonTokenType.StartObject:
                         var objIngredient = ProcessIngredientObject(ref reader);
                        if (!string.IsNullOrEmpty(objIngredient))
                        {
                            ingredients.Add(objIngredient);
                        }
                        break;

                    default:
                        reader.Skip();
                        break;
                }
            }

            return ingredients;
        }

        private string ProcessIngredientObject(ref Utf8JsonReader reader)
        {
            string result = null;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString();
                    reader.Read();  

                   
                    if (propertyName == "name" || propertyName == "ingredient" ||
                        propertyName == "text" || propertyName == "description")
                    {
                        result = reader.GetString();
                    }
                }
            }

            return result ?? "Unknown ingredient";
        }

        public override void Write(Utf8JsonWriter writer, List<string> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
            {
                writer.WriteStringValue(item);
            }
            writer.WriteEndArray();
        }
    }

    public class HomeController : Controller
    {
        private readonly string spoonacularApiKey;
        private readonly string spoonacularApiUrl;
        private readonly string apiKey;
        private readonly string apiUrl;
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _context = context;
            apiKey = "YOUR_API";
            apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=" + apiKey;
            spoonacularApiKey = "YOU_API";
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Fruits()
        {
            var stopwatch = Stopwatch.StartNew();
            var fruits = await _context.Fruits
                   .AsNoTracking()
                   .ToListAsync();
            stopwatch.Stop();

            Console.WriteLine($"DB Query Time: {stopwatch.ElapsedMilliseconds} ms");

            return View(fruits);
        }
        public async Task<IActionResult> GrainAndPasta()
        {
            var stopwatch = Stopwatch.StartNew();
            var grainAndpasta = await _context.GrainAndPasta
                   .AsNoTracking()
                   .ToListAsync();
            stopwatch.Stop();

            Console.WriteLine($"DB Query Time: {stopwatch.ElapsedMilliseconds} ms");

            return View(grainAndpasta);
        }
        public async Task<IActionResult> Dairy()
        {
            var stopwatch = Stopwatch.StartNew();
            var dairy = await _context.Dairy
                   .AsNoTracking()
                   .ToListAsync();
            stopwatch.Stop();

            Console.WriteLine($"DB Query Time: {stopwatch.ElapsedMilliseconds} ms");

            return View(dairy);
        }
        public async Task<IActionResult> Bakery()
        {
            var stopwatch = Stopwatch.StartNew();
            var bakery = await _context.Bakery
                   .AsNoTracking()
                   .ToListAsync();
            stopwatch.Stop();

            Console.WriteLine($"DB Query Time: {stopwatch.ElapsedMilliseconds} ms");

            return View(bakery);
        }
        public async Task<IActionResult> Meat()
        {
            var stopwatch = Stopwatch.StartNew();
            var meat = await _context.Meat
                   .AsNoTracking()
                   .ToListAsync();
            stopwatch.Stop();

            Console.WriteLine($"DB Query Time: {stopwatch.ElapsedMilliseconds} ms");

            return View(meat);
        }
        public async Task<IActionResult> FloursANDBakingIngredients()
        {
            var stopwatch = Stopwatch.StartNew();
            var floursANDBakingIngredients = await _context.FloursANDBakingIngredients
                   .AsNoTracking()
                   .ToListAsync();
            stopwatch.Stop();

            Console.WriteLine($"DB Query Time: {stopwatch.ElapsedMilliseconds} ms");

            return View(floursANDBakingIngredients);
        }
        public async Task<IActionResult> SpicesAndCondiments()
        {
            var stopwatch = Stopwatch.StartNew();
            var spicesAndCondiments = await _context.SpicesAndCondiments
                   .AsNoTracking()
                   .ToListAsync();
            stopwatch.Stop();

            Console.WriteLine($"DB Query Time: {stopwatch.ElapsedMilliseconds} ms");

            return View(spicesAndCondiments);
        }
        public async Task<IActionResult> Vegatables()
        {
            var stopwatch = Stopwatch.StartNew();
            var vegatables = await _context.Vegetables
                   .AsNoTracking()
                   .ToListAsync();
            stopwatch.Stop();

            Console.WriteLine($"DB Query Time: {stopwatch.ElapsedMilliseconds} ms");

            return View(vegatables);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult AIResults()
        {
            return View();
        }
        public IActionResult OurPropose()
        {
            ViewBag.ShowHeart = true;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetAIRecipes([FromBody] IngredientsRequest request)
        {
            if (request == null || request.Ingredients == null || request.Ingredients.Count == 0)
            {
                return BadRequest("No ingredients provided");
            }

            try
            {
                var recipes = await FetchRecipesFromAI(string.Join(", ", request.Ingredients));
                return Json(recipes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Recipe generation error: {ex.Message}");
                return StatusCode(500, $"Error generating recipes: {ex.Message}");
            }
        }

        private async Task<List<Recipe>> FetchRecipesFromAI(string ingredients)
        {
            // More explicit prompt to get desired format
            var requestData = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = $"Provide recipes that can be made with the following ingredients: {ingredients}. " +
                                      "Return the response in JSON format as an array of recipe objects. Each recipe " +
                                      "should have these fields: title (string), description (string), " +
                                      "instructions (array of strings), and ingredients (array of SIMPLE strings only, " +
                                      "not objects). For example, ingredients should be like [\"flour\", \"sugar\", \"eggs\"] " +
                                      "not [{\"name\":\"flour\"}, {\"name\":\"sugar\"}]. " +
                                      "Do not add any additional text, just the JSON array."
                            }
                        }
                    }
                }
            };

            string jsonRequest = JsonSerializer.Serialize(requestData);
            using var client = new HttpClient();
            var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(apiUrl, requestContent);
            string jsonResponse = await response.Content.ReadAsStringAsync();

            try
            {
                using JsonDocument doc = JsonDocument.Parse(jsonResponse);
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("candidates", out JsonElement candidates) &&
                    candidates.GetArrayLength() > 0 &&
                    candidates[0].TryGetProperty("content", out JsonElement contentElement) &&
                    contentElement.TryGetProperty("parts", out JsonElement parts) &&
                    parts.GetArrayLength() > 0 &&
                    parts[0].TryGetProperty("text", out JsonElement text))
                {
                    string recipeText = text.GetString();
                    string recipeJson = ExtractJsonFromMarkdown(recipeText);

                     var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new IngredientConverter() }
                    };

                    try
                    {
                        var recipes = JsonSerializer.Deserialize<List<Recipe>>(recipeJson, options);
                        return recipes ?? new List<Recipe>();
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError($"JSON Deserialization error: {ex.Message}. JSON response: {recipeJson}");
                        throw new Exception($"Failed to parse recipe data: {ex.Message}");
                    }
                }
                else
                {
                    if (root.TryGetProperty("error", out JsonElement error))
                    {
                        string errorMessage = error.TryGetProperty("message", out JsonElement message)
                            ? message.GetString()
                            : "Error";
                        throw new Exception($"API response is error: {errorMessage}");
                    }

                    throw new Exception("Structure error in API response");
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError($"JSON parse error: {ex.Message}. Raw response: {jsonResponse}");
                throw new Exception($"Invalid JSON response from API: {ex.Message}");
            }
        }

        private string ExtractJsonFromMarkdown(string text)
        {
            if (text.Contains("```json"))
            {
                int start = text.IndexOf("```json") + 7;
                int end = text.IndexOf("```", start);
                return text.Substring(start, end - start).Trim();
            }
            return text;
        }

        [HttpPost]
        public async Task<IActionResult> GetRecipesByIngredients([FromBody] List<string> ingredients)
        {
            if (ingredients == null || ingredients.Count == 0)
            {
                return Json(new List<Recipe>());
            }
            try
            {
                var client = _httpClientFactory.CreateClient();
                string ingredientsString = string.Join(",+", ingredients);
                string apiUrl = $"https://api.spoonacular.com/recipes/findByIngredients?ingredients={ingredientsString}&number=10&ranking=1&apiKey={spoonacularApiKey}";
                var response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var spoonacularRecipes = JsonSerializer.Deserialize<List<SpoonacularRecipe>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var recipes = await ConvertToRecipes(spoonacularRecipes, client);
                return Json(recipes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching recipes: {ex.Message}");
                return StatusCode(500, "Error fetching recipes from API");
            }
        }

        private async Task<List<Recipe>> ConvertToRecipes(List<SpoonacularRecipe> spoonacularRecipes, HttpClient client)
        {
            var recipes = new List<Recipe>();

            foreach (var spoonRecipe in spoonacularRecipes)
            {
                 try
                {
                    string detailsUrl = $"https://api.spoonacular.com/recipes/{spoonRecipe.Id}/information?apiKey={spoonacularApiKey}";
                    var detailsResponse = await client.GetAsync(detailsUrl);
                    detailsResponse.EnsureSuccessStatusCode();

                    var detailsContent = await detailsResponse.Content.ReadAsStringAsync();
                    var recipeDetails = JsonSerializer.Deserialize<RecipeDetails>(detailsContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    var recipe = new Recipe
                    {
                        Title = spoonRecipe.Title,
                        Imageurl = spoonRecipe.Image,
                        Description = recipeDetails.Summary ?? "Description is Empty",
                        Instructions = ParseInstructions(recipeDetails.Instructions),
                        Ingredients = spoonRecipe.UsedIngredients
                            .Concat(spoonRecipe.MissedIngredients)
                            .Select(i => i.Original)
                            .ToList()
                    };

                    recipes.Add(recipe);
                }
                catch (Exception ex)
                {
                     Console.WriteLine($"Error fetching details for recipe {spoonRecipe.Id}: {ex.Message}");
                }
            }

            return recipes;
        }

        private List<string> ParseInstructions(string instructions)
        {
            if (string.IsNullOrEmpty(instructions))
            {
                return new List<string> { "Haven't instructions" };
            }

             var steps = instructions
                .Split(new[] { ". ", ".\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .ToList();

            return steps.Count > 0 ? steps : new List<string> { instructions };
        }

         public IActionResult SavedRecipes()
        {
            return View();
        }
    }

    public class IngredientsRequest
    {
        public List<string> Ingredients { get; set; } = new List<string>();
    }

    public class Recipe
    {
        public string Title { get; set; }
        public string? Imageurl { get; set; }
        public string Description { get; set; }
        public List<string> Instructions { get; set; } = new List<string>();
        public List<string> Ingredients { get; set; } = new List<string>();
    }

    public class SpoonacularRecipe
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public List<SpoonacularIngredient> UsedIngredients { get; set; } = new List<SpoonacularIngredient>();
        public List<SpoonacularIngredient> MissedIngredients { get; set; } = new List<SpoonacularIngredient>();
    }

    public class SpoonacularIngredient
    {
        public string Original { get; set; }
    }

    public class RecipeDetails
    {
        public string Summary { get; set; }
        public string Instructions { get; set; }
    }
}