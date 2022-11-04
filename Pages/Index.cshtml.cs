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

        public IndexModel(ILogger<IndexModel> logger, JsonFileService movieService)
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

        /*public void OnGet()
        {
            Movies = MovieService.GetMovies();
        }*/
        public async void OnPost()
        {
            try
            {
                var seen = Request.Form["Seen"];
                MovieService.SeenMovie(seen);
                Movies = MovieService.GetMovies();
            }
            catch
            {
                MovieService.AddSearchedMovie();
                Movies = MovieService.GetSearchedMovie();
                await OnGetAsync();
            } 
        }
    }
}