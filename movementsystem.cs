namespace AwesomeGame;

public class MovementSystem
{
    // Assigns string "startroom" to the currentRoom variable
    public static string currentRoom = "startroom";

    // Movement system for Start Room
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
    public static void SideRoom(string movement)
    {
        if (movement == "start room")
        {
            currentRoom = "startroom";
        }
    }

    // Movement system for hallway1
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

    // Movement system for Knife Room
    public static void kniferoom(string movement)
    {
        if (movement == "hallway")
        {
            currentRoom = "hallway1";
        }
    }

    // Movement system for Vines Room
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

    // Movement system for Tablet Room
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

    // Movement system for hallway2
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

    // Movement system for hallway3
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

    // Movement system for hallway4
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

    // Movement system for Key Room
    public static void keyroom(string movement)
    {
        switch (movement)
        {
            case "hallway":
                currentRoom = "hallway4";
                break;
        }
    }

    public static void Move(string movement)
    {
        switch (currentRoom)
        {
            case "startroom":
                StartRoom(movement);
                break;
            case "sideroom":
                SideRoom(movement);
                break;
            case "hallway1":
                hallway1(movement);
                break;
            case "kniferoom":
                kniferoom(movement);
                break;
            case "vinesroom":
                vinesroom(movement);
                break;
            case "tabletroom":
                tabletroom(movement);
                break;
            case "hallway2":
                hallway2(movement);
                break;
            case "hallway3":
                hallway3(movement);
                break;
            case "hallway4":
                hallway4(movement);
                break;
            case "keyroom":
                keyroom(movement);
                break;
        }
    }
}

