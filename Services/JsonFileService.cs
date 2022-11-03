using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TheWatchlist.Models;
using System.Linq;

namespace TheWatchlist.Services
{
    public class JsonFileService
    {
        public JsonFileService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }

        private string JsonFileName => Path.Combine(WebHostEnvironment.WebRootPath, "data", "movies.json");

        public IEnumerable<Movie> GetMovies()
        {
            using (var jsonFileReader = File.OpenText(JsonFileName))
            {
                return JsonSerializer.Deserialize<Movie[]>(jsonFileReader.ReadToEnd(),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
        }
        public IEnumerable<Movie> GetSearchedMovie()
        {
            var data = "[" + File.ReadAllText(@"./wwwroot/data/search.json") + "]";
            return JsonSerializer.Deserialize<Movie[]>(data);  
        }
        
        public void AddSearchedMovie()
        {
            var movies = GetMovies();
            var movie = GetSearchedMovie();
            var together = movie.Concat(movies);
            
            
            
            using (var outputStream = File.OpenWrite(JsonFileName))
            {
                JsonSerializer.Serialize<IEnumerable<Movie>>(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions
                    {
                        SkipValidation = true,
                        Indented = true
                    }),
                    together
                    );
            }
        }
        public static async Task SearchMovie(string searchphrase)
        {
            string apiKey = "947a6562";
            string baseUri = $"http://www.omdbapi.com/?apikey={apiKey}";

            string name = searchphrase;
            string type = "movie";
            var json = string.Empty;

            var sb = new StringBuilder(baseUri);
            sb.Append($"&t={name}");
            sb.Append($"&type={type}");

            using (var client = new HttpClient())
            {
                var endpoint = new Uri(sb.ToString());
                var result = client.GetAsync(endpoint).Result;
                json = result.Content.ReadAsStringAsync().Result;
            }
            await File.WriteAllTextAsync("./wwwroot/data/search.json", json);
        }


    }
}

