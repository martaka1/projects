using Domain;

namespace UnoProject;

public static class PlayerSetup
{
    public static List<Player> ConfigurePlayers()
    {
        List<Player>returnList = new List<Player>();
        var playerCount = 0;

        while (true)
        {
            Console.Write($"How many players (2 - 10)[2]:");
            var playerCountStr = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(playerCountStr)) playerCountStr = "2";
            if (int.TryParse(playerCountStr, out playerCount))
            {
                if (playerCount > 1 && playerCount <= 10) break;
                Console.WriteLine("Number not in range...");
            }
        }


        for (int i = 0; i < playerCount; i++)
        {
            string? playerType = "";
            while (true)
            {
                Console.Write($"Player {i + 1} type (A - ai / H - human)[{(i % 2 == 0 ? "h" : "a")}]:");
                playerType = Console.ReadLine()?.ToLower().Trim();
                if (string.IsNullOrWhiteSpace(playerType))
                {
                    playerType = i % 2 == 0 ? "h" : "a";
                }

                if (playerType == "a" || playerType == "h") break;
                Console.WriteLine("Parse error...");
            }

            string? playerName = "";
            while (true)
            {
                Console.Write($"Player {i + 1} name (min 1 letter)[{playerType + (i + 1)}]:");
                playerName = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(playerName))
                {
                    playerName = playerType + (i + 1);
                }

                if (!string.IsNullOrWhiteSpace(playerName) && playerName.Length > 0) break;
                Console.WriteLine("Parse error...");
            }

            returnList.Add(new Player()
            {
                NickName = playerName,
                PlayerType = playerType == "h" ? EPlayerType.Human : EPlayerType.AI
            });
        }

        return returnList;
    }
}
