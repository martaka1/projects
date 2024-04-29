using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class StartModel : PageModel
{
    
    private IGameRepository _repository;
    
    public StartModel(IGameRepository repository)
    {
        _repository = repository;
    }
    
    [BindProperty]
    public Guid GameId { get; set; }
    
    public async Task<IActionResult> OnGet(Guid id)
    {
        GameId = id;
        
        var state = await Task.Run(() => _repository.LoadGame(GameId));
        
        _repository.SaveGame(GameId, state);
        
        return RedirectToPage("./GamesPage", new {id = GameId});
    }
}