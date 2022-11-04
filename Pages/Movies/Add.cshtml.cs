using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TheWatchlist.Models;
using TheWatchlist.Services;

namespace TheWatchlist.Pages.Movies
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public JsonFileService MovieService;
        public IEnumerable<Movie> Movie { get; private set; }

        [BindProperty(SupportsGet = true)]
        public string ? SearchString { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public IndexModel(ILogger<IndexModel> logger, JsonFileService movieService)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _logger = logger;
            MovieService = movieService;
        }
        public async Task OnGetAsync()
        {
            if (!string.IsNullOrEmpty(SearchString))
            {
                await JsonFileService.SearchMovie(SearchString);
            }
            await Task.Delay(200);
            try
            {
                Movie = MovieService.GetSearchedMovie();
            }
            catch
            {
                Movie = MovieService.GetMovies();
            }
            
        }
        public void OnPost()
        {
            MovieService.AddSearchedMovie();
            Movie = MovieService.GetSearchedMovie();
        }
    }
}
