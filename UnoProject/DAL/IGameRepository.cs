using System.Diagnostics.CodeAnalysis;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public interface IGameRepository
{
    public void SaveGame(Guid id, GameState game);
    List<(Guid id, DateTime dt)> GetSaveGames();
    
    GameState LoadGame(Guid id);
}