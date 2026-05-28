namespace AwesomeGame;

public class MovementSystem
{
    // Assigns string "startroom" to the currentRoom variable
    public static string currentRoom = "startroom";

    // Movement system for Start Room
    public static bool StartRoom(string movement)
    {
        bool succeeded = true;
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
                    succeeded = false;
                    break;
                }
            default:
                succeeded = false;
                break;
        }
        return succeeded;
    }
    public static bool SideRoom(string movement)
    {
        bool succeeded = true;
        if (movement == "start room")
        {
            currentRoom = "startroom";
        }
        else
        {
            succeeded = false;
        }
        return succeeded;
    }

    // Movement system for hallway1
    public static bool hallway1(string movement)
    {
        bool succeeded = true;
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
                    succeeded = false;
                    break;
                }
            default:
                succeeded = false;
                break;
        }
        return succeeded;
    }

    // Movement system for Knife Room
    public static bool kniferoom(string movement)
    {
        bool succeeded = true;
        if (movement == "hallway")
        {
            currentRoom = "hallway1";
        }
        else
        {
            succeeded = false;
        }
        return succeeded;
    }

    // Movement system for Vines Room
    public static bool vinesroom(string movement)
    {
        bool succeeded = true;
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
                    succeeded = false;
                    break;
                }
            default:
                succeeded = false;
                break;
        }
        return succeeded;
    }

    // Movement system for Tablet Room
    public static bool tabletroom(string movement)
    {
        bool succeeded = true;
        switch (movement)
        {
            case "side room":
                currentRoom = "vinesroom";
                break;
            case "main entrance":
                currentRoom = "hallway1";
                break;
            default: 
                succeeded = false;
                break;
        }
        return succeeded;
    }

    // Movement system for hallway2
    public static bool hallway2(string movement)
    {
        bool succeeded = true;
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
                    succeeded = false;
                    break;
                }
            case "north":
                currentRoom = "hallway1";
                break;
            case "west":
                currentRoom = "hallway3";
                break;
            case "south":
                if (!Game.LurkerMoved)
                {
                    currentRoom = "smashingroom";
                }
                else
                {
                    Console.WriteLine("you hear the lurker in this room, so you shouldn't go in");
                }
                    break;
            default: 
                succeeded = false;
                break;
        }
        return succeeded;
    }

    // Movement system for hallway3
    public static bool hallway3(string movement)
    {
        bool succeeded = true;
        switch (movement)
        {
            case "east":
                currentRoom = "hallway2";
                break;
            case "north":
                currentRoom = "hallway4";
                break;
            default: 
                succeeded = false;
                break;     
        }
        return succeeded;
    }

    // Movement system for hallway4
    public static bool hallway4(string movement)
    {
        bool succeeded = true;
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
                    succeeded = false;
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
                    succeeded = false;
                }
                break;
            case "south":
                currentRoom = "hallway3";
                break;
        default:
            succeeded = false;
            break;
        }
        return succeeded;
    }

    // UNFINISHED movement system for smashingroom, put here as a placeholder
    public static bool smashingroom(string movement)
    {
        bool succeeded = true;
        return succeeded;
    }

    public static bool spidersroom(string movement)
    {
        bool succeeded = true;
        switch (movement)
        {
            case "main entrance":
                if (!Game.SpiderSacBurst)
                {
                    Game.scrolltext("The exit is blocked by an egg sac. You should attack it");
                    succeeded = false;
                }
                else
                {
                    currentRoom = "hallway2";
                }
                break;
            case "side entrance":
                currentRoom = "renovated room";
                succeeded = true;
                break;
            default:
                succeeded = false;
                break;
        }

        return succeeded;
    }

    // Movement system for Key Room
    public static bool keyroom(string movement)
    {
        bool succeeded = true;
        switch (movement)
        {
            case "hallway":
                currentRoom = "hallway4";
                break;
            default:
                succeeded = false;
                break;
        }
        return succeeded;
    }

    public static bool spiderroom(string movement)
    {
        bool succeeded = true;
        switch (movement)
        {
            case "main entrance":
                currentRoom = "hallway1";
                break;
            case "renovated room":
                currentRoom = "renovatedroom";
                break;
            default:
                succeeded = false;
                break;
        }
        return succeeded;
    }

    public static bool renovatedroom(string movement)
    {
        bool succeeded = true;
        switch (movement)
        {
            case 
        }
    }

    public static bool Move(string movement)
    {
        switch (currentRoom)
        {
            case "startroom":
                return StartRoom(movement);
            case "sideroom":
                return SideRoom(movement);
            case "hallway1":
                return hallway1(movement);
            case "kniferoom":
                return kniferoom(movement);
            case "vinesroom":
                return vinesroom(movement);
            case "tabletroom":
                return tabletroom(movement);
            case "hallway2":
                return hallway2(movement);
            case "hallway3":
                return hallway3(movement);
            case "hallway4":
                return hallway4(movement);
            case "keyroom":
                return keyroom(movement);
            case "smashingroom":
                return smashingroom(movement);
            case "spidersroom":
                return spidersroom(movement);
            default:
                return false;
        }
    }
}

