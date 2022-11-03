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

        public IndexModel(ILogger<IndexModel> logger, JsonFileService movieService)
        {
            _logger = logger;
            MovieService = movieService;
        }

        public void OnGet()
        {
            Movies = MovieService.GetMovies();
        }
    }
}