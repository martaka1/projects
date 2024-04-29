using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class CreateModel : PageModel
{
    private IGameRepository _gameRepository;
    

    public CreateModel(IGameRepository repository)
    {
        _gameRepository = repository;
    }

    public async Task<IActionResult>? OnGetAsync()
    {
        var state = await Task.Run(() => new GameState());
        _gameRepository.SaveGame(state.Id, state);
        return RedirectToPage("./NewGame", new {id = state.Id});
    }
}