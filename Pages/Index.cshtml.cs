using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TheWatchlist.Models;
using TheWatchlist.Services;

namespace TheWatchlist.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public JsonFileService MovieService;
        public IEnumerable<Movie> Movies { get; private set; }
        public IEnumerable<Movie> SearchedMovie { get; private set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public IndexModel(ILogger<IndexModel> logger, JsonFileService movieService)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _logger = logger;
            MovieService = movieService;
        }

        public async Task OnGetAsync()
        {
            MovieService.DeleteSearchedMovie();
            Movies = MovieService.GetMovies();
            if (!string.IsNullOrEmpty(SearchString))
            {
                await JsonFileService.SearchMovie(SearchString);
                await Task.Delay(200);
                SearchedMovie = MovieService.GetSearchedMovie();
            }
        }
        public void OnPostSeen()
        {
                var seen = Request.Form["Seen"];
                MovieService.SeenMovie(seen);
                Movies = MovieService.GetMovies(); 
        }
        public void OnPostDelete()
        {
            var delete = Request.Form["Delete"];
            MovieService.DeleteMovie(delete);
            Movies = MovieService.GetMovies();
        }
        public void OnPostAddToWatchList()
        {
            MovieService.AddSearchedMovie();
            Movies = MovieService.GetMovies();
        }
    }
}