using System.Runtime.Serialization;
using System.Text.Json;
using DAL;
using Domain;
using Helpers;
using Microsoft.EntityFrameworkCore;

namespace Uno.DAL;

public class GameRepositoryFileSystem : IGameRepository
{
    // TODO: figure out system dependent location - maybe Path.GetTempPath()
    private const string SaveLocation = "/Users/martakask/RiderProjects/icd0008-23f/UnoConsoleApp/UnoProject/saved_games";
    

    public List<(Guid id, DateTime dt)> GetSaveGames()
    {
        var data = Directory.EnumerateFiles(SaveLocation);
        var res = data
            .Select(
                path => (
                    Guid.Parse(Path.GetFileNameWithoutExtension(path)),
                    File.GetLastWriteTime(path)
                )
            ).ToList();
        
        return res;
    }

    public void SaveGame(Guid id, GameState game)
    {
        var content = JsonSerializer.Serialize(game, JsonHelpers.JsonSerializerOptions);
                                            
        var fileName = Path.ChangeExtension(id.ToString(), ".json");
                                            
        if (!Path.Exists(SaveLocation))
        {
            Directory.CreateDirectory(SaveLocation);
        }
                                            
        File.WriteAllText(Path.Combine(SaveLocation, fileName), content);
    }

    public GameState LoadGame(Guid id)
    {
        var fileName = Path.ChangeExtension(id.ToString(), ".json");

        var jsonStr = File.ReadAllText(Path.Combine(SaveLocation, fileName));
        var res = JsonSerializer.Deserialize<GameState>(jsonStr, JsonHelpers.JsonSerializerOptions);
        if (res == null) throw new SerializationException($"Cannot deserialize {jsonStr}");

        return res;
    }
}