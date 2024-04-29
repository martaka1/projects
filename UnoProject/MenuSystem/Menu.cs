namespace MenuSystem;

public class Menu
{
    public string? Title { get; set; }
    public List<MenuItem> MenuItems { get; set; }

    private const string MenuSeparator = "=======================";
    private int _selectedOption = 0;

    public Menu(string? title, List<MenuItem> menuItems)
    {
        Title = title;
        MenuItems = menuItems;
    }

    private void Draw()
    {
        Console.Clear();

        if (!string.IsNullOrWhiteSpace(Title))
        {
            Console.WriteLine(Title);
            Console.WriteLine(MenuSeparator);
        }

        for (int i = 0; i < MenuItems.Count; i++)
        {
            Console.Write(_selectedOption == i ? "> " : "  ");
            Console.WriteLine(MenuItems[i].MenuLabel);
        }

        Console.WriteLine(MenuSeparator);
    }

    public int Run()
    {
        ConsoleKey key;
        do
        {
            Draw();
            key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.DownArrow:
                    _selectedOption = (_selectedOption + 1) % MenuItems.Count;
                    break;
                case ConsoleKey.UpArrow:
                    _selectedOption = (_selectedOption - 1 + MenuItems.Count) % MenuItems.Count;
                    break;
            }
        } while (key != ConsoleKey.Enter);

        return _selectedOption + 1;
    }
}