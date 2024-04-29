using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesController.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IGameRepository _repository;

    public IndexModel(ILogger<IndexModel> logger, IGameRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }
    
    public int Count { get; set; }

    public void OnGet()
    {
        Count = _repository.GetSaveGames().Count;
    }
}