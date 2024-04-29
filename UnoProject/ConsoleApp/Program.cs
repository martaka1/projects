using ConsoleUI;
using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;
using Uno.UnoEngine;

namespace UnoProject;

public class Program
{


    public static void Main()
    {
        //=================SET UP==============================
        var gameOptions = new GameOptions();
        var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=app.db")
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .Options;
        using var db = new AppDbContext(contextOptions);
        // apply all the migrations
        db.Database.Migrate();
        IGameRepository gameRepository = new GameRepositoryEF(db);
        //var gameRepository = new GameRepositoryFileSystem();
        var mainMenu = ProgramMenus.GetMainMenu(
            gameOptions,
            ProgramMenus.GetOptionsMenu(gameOptions),
            NewGame,
            LoadGame
        );
        
        // ================== MAIN =====================
        int selectedOption = mainMenu.Run();

// Handle the selected option
        switch (selectedOption)
        {
            case 1:
                NewGame();
                break;
            case 2:
                LoadGame();
                break;
            // Add cases for other menu options as needed
        }
        

        // ================ THE END ==================
        return;

        // ================== NEW GAME =====================
        string? NewGame()
        {
               
            gameOptions = GetGameOptions(gameOptions);
            var players = PlayerSetup.ConfigurePlayers();
            var gameSate = new GameState()
            {
                GameOptions = gameOptions,
                Players = players
            };
            // game logic, shared between console and web
            var gameEngine = new GameEngine(gameSate);

            // console controller for game loop
            var gameController = new GameController(gameEngine, gameRepository,gameSate);

            gameController.Run();
            return null;
        }
        // ================== LOAD GAME =====================
        string? LoadGame()
        {
            Console.WriteLine("Saved games");
            var saveGameList = gameRepository.GetSaveGames();
            var saveGameListDisplay = saveGameList.Select((s, i) => (i + 1) + " - " + s).ToList();

            if (saveGameListDisplay.Count == 0) return null;

            Guid gameId;
            while (true)
            {
                Console.WriteLine(string.Join("\n", saveGameListDisplay));
                Console.Write($"Select game to load (1..{saveGameListDisplay.Count}):");
                var userChoiceStr = Console.ReadLine();
                if (int.TryParse(userChoiceStr, out var userChoice))
                {
                    if (userChoice < 1 || userChoice > saveGameListDisplay.Count)
                    {
                        Console.WriteLine("Not in range...");
                        continue;
                    }

                    gameId = saveGameList[userChoice - 1].id;
                    Console.WriteLine($"Loading file: {gameId}");

                    break;
                }

                Console.WriteLine("Parse error...");
            }


            var gameState = gameRepository.LoadGame(gameId);

            var gameEngine = new GameEngine(gameState);
            gameEngine.Update();
    
            var gameController = new GameController(gameEngine, gameRepository,gameState);

            
            gameController.Run();

            return null;
        }

    }

    private static GameOptions GetGameOptions(GameOptions gameOptions)
    {
        Console.WriteLine("Game Options:");

        // Hand Size
        Console.Write($"Enter Hand Size (default: {gameOptions.HandSize}): ");
        if (int.TryParse(Console.ReadLine(), out var handSize) && handSize > 0)
        {
            gameOptions.HandSize = handSize;
        }

        // Can Stack
        Console.Write($"Can Stack Cards? (true/false, default: {gameOptions.CanStack}): ");
        if (bool.TryParse(Console.ReadLine(), out var canStack))
        {
            gameOptions.CanStack = canStack;
        }
        return gameOptions;
    }
    
}