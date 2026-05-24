namespace AwesomeGame;

public class MovementSystem
{
    public static string currentRoom = "startroom";
    public static void StartRoom(string movement)
    {
        switch (movement)
        {
            case "side room":
                currentRoom = "sideroom";
                break;
            case "main entrance":
                currentRoom = "hallway1";
                break;
            case "second entrance":
                if (Game.Inventory.ContainsKey("tablet"))
                {
                    currentRoom = "hallway4";
                    break;
                }
                else
                {
                    Console.WriteLine("You do not have a tablet.");
                    break;
                }
        }
    }

    public static void hallway1(string movement)
    {
        switch (movement)
        {
            case "starting room":
                currentRoom = "startroom";
                break;
            case "small room":
                currentRoom = "vinesroom";
                break;
            case "open room":
                currentRoom = "kniferoom";
                break;
            case "locked door":
                if (Game.Inventory.ContainsKey("tablet"))
                {
                    currentRoom = "tabletroom";
                    break;
                }
                else
                {
                    Console.WriteLine("You do not have a tablet.");
                    break;
                }
        }
    }

    public static void kniferoom(string movement)
    {
        if (movement == "hallway")
        {
            currentRoom = "hallway1";
        }
    }

    public static void vinesroom(string movement)
    {
        switch (movement)
        {
            case "main entrance":
                currentRoom = "hallway1";
                break;
            case "main room":
                if (Game.VinesCut)
                {
                    currentRoom = "tabletroom";
                    break;
                }
                else
                {
                    Console.WriteLine("Vines have not been cut.");
                    break;
                }
        }
    }

    public static void tabletroom(string movement)
    {
        switch (movement)
        {
            case "side room":
                currentRoom = "vinesroom";
                break;
            case "main entrance":
                currentRoom = "hallway1";
                break;
        }
    }

    public static void hallway2(string movement)
    {
        switch (movement)
        {
            case "glass door":
                if (Game.Inventory.ContainsKey("key"))
                {
                    currentRoom = "stairs";
                    break;
                }
                else
                {
                    Console.WriteLine("You do not have a key.");
                    break;
                }
            case "north":
                currentRoom = "hallway1";
                break;
            case "west":
                currentRoom = "hallway3";
                break;
        }
    }

    public static void hallway3(string movement)
    {
        switch (movement)
        {
            case "east":
                currentRoom = "hallway2";
                break;
            case "north":
                currentRoom = "hallway4";
                break;
        }
    }

    public static void hallway4(string movement)
    {
        switch (movement)
        {
            case "starting room":
                if (Game.Inventory.ContainsKey("tablet"))
                {
                    currentRoom = "startroom";
                }
                else
                {
                    Console.WriteLine("You do not have a tablet.");
                }
                break;
            case "locked door":
                if (Game.Inventory.ContainsKey("tablet"))
                {
                    currentRoom = "keyroom";
                }
                else
                {
                    Console.WriteLine("You do not have a tablet.");
                }
                break;
            case "south":
                currentRoom = "hallway3";
                break;
        }
    }

    public static void keyroom(string movement)
    {
        switch (movement)
        {
            case "hallway":
                currentRoom = "hallway4";
                break;
        }
    }
}

