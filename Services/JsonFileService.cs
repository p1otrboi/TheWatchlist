using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TheWatchlist.Models;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

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

        public IEnumerable<Movie> GetMovies() // Hämtar en object-list av alla filmer sparade i movies.json
        {
            using (var jsonFileReader = File.OpenText(JsonFileName))
            {
#pragma warning disable CS8603 // Possible null reference return.
                return JsonSerializer.Deserialize<Movie[]>(jsonFileReader.ReadToEnd(),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
#pragma warning restore CS8603 // Possible null reference return.
            }
        }
        
        public IEnumerable<Movie> GetSearchedMovie() // Skriver API-responsen från OMDb till search.json
        {
            var data = "[" + File.ReadAllText(@"./wwwroot/data/search.json") + "]";
            return JsonSerializer.Deserialize<Movie[]>(data);  
        }
        public void AddSearchedMovie() // Hämtar movie-object från movies.json och search.json - sen lägger ihop dem och sparar till movies.json
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
        public static async Task SearchMovie(string searchphrase)  // Söker på en film genom http request på OMDb-api, och sparar responsen i search.json 
        {
            string apiKey = "947a6562";
            string baseUri = $"http://www.omdbapi.com/?apikey={apiKey}";

            string name = searchphrase;
            string type = "movie";
            string plot = "full";
            var json = string.Empty;

            var sb = new StringBuilder(baseUri);
            sb.Append($"&t={name}");
            sb.Append($"&type={type}");
            sb.Append($"&plot={plot}");

            using (var client = new HttpClient())
            {
                var endpoint = new Uri(sb.ToString());
                var result = client.GetAsync(endpoint).Result;
                json = result.Content.ReadAsStringAsync().Result;
            }
            await File.WriteAllTextAsync("./wwwroot/data/search.json", json);
        }
        public void SeenMovie(string movieTitle) // Toggle mellan Seen/Not seen
        {
            var movies = GetMovies();
            var query = movies.First(x => x.Title == movieTitle);
            if (query.Seen == false)
                query.Seen = true; 
            else
                query.Seen = false;
            File.Delete(JsonFileName);
            using (var outputStream = File.OpenWrite(JsonFileName))
            {
                JsonSerializer.Serialize<IEnumerable<Movie>>(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions
                    {
                        SkipValidation = false,
                        Indented = true
                    }),
                    movies
                    );
            }
        }

        public void DeleteMovie(string movieTitle) // Tar bort en film ur databasen(movies.json)
        {
            var movies = GetMovies().ToList();
            var query = movies.First(x => x.Title == movieTitle);
            movies.Remove(query);
            var moviesArray = movies.ToArray();
            File.Delete(JsonFileName);
            using (var outputStream = File.OpenWrite(JsonFileName))
            {
                JsonSerializer.Serialize<IEnumerable<Movie>>(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions
                    {
                        SkipValidation = false,
                        Indented = true
                    }),
                    moviesArray
                    );
            }
        }
        public void DeleteSearchedMovie()
        {
            File.Delete("./wwwroot/data/search.json");
        }

    }
}

