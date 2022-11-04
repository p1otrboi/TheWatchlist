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

        public IndexModel(ILogger<IndexModel> logger, JsonFileService movieService)
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
