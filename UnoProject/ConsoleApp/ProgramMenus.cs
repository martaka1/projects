using Domain;
using MenuSystem;

namespace UnoProject;

public class ProgramMenus
{
    public static Menu GetOptionsMenu(GameOptions gameOptions) =>
        new Menu("Options", new List<MenuItem>()
        {
        });

    public static Menu GetMainMenu(GameOptions gameOptions, Menu optionsMenu, Func<string?> newGameMethod, Func<string?> loadGameMethod) => 
        new Menu(">> UNO <<", new List<MenuItem>()
        {
            new MenuItem()
            {
                Shortcut = "s",
                MenuLabel = "Start a new game: ",
                MethodToRun = newGameMethod
            },
            new MenuItem()
            {
                Shortcut = "l",
                MenuLabel = "Load game",
                MethodToRun = loadGameMethod
            },
            new MenuItem()
            {
                Shortcut = "o",
                MenuLabel = "Options",
            },
        });
}