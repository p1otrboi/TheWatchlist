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
        public IEnumerable<Movie> Movies { get; private set; }
        [BindProperty(SupportsGet = true)]
        public string ? SearchString { get; set; }

        public IndexModel(ILogger<IndexModel> logger, JsonFileService movieService)
        {
            _logger = logger;
            MovieService = movieService;
        }

        public async Task OnGetAsync()
        {
            Movies = MovieService.GetMovies();
            if (!string.IsNullOrEmpty(SearchString))
            {
                await JsonFileService.SearchMovie(SearchString);
            }
        }
    }
}
