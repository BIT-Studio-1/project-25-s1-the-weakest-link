namespace AwesomeGame;

// Class containing all code for moving between rooms, 
public class MovementSystem
{
    // Assigns string "startroom" to the currentRoom variable
    public static string currentRoom = "startroom";

    // Movement system for Start Room
    public static bool startroom(string movement)
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
                    Game.scrolltext("This door is sealed with some kind of dark magic, you will need some sort of artifact to access it.");
                    succeeded = false;
                    break;
                }
            default:
                succeeded = false;
                break;
        }
        return succeeded;
    }
    public static bool sideroom(string movement)
    {
        bool succeeded = true;
        if (movement == "door")
        {
            currentRoom = "startroom";
        }
        else
        {
            succeeded = false;
        }
        return succeeded;
    }

    // Movement system for hel
    public static bool hallway1(string movement)
    {
        bool succeeded = true;
        switch (movement)
        {
            case "starting room":
                if (!Game.Inventory.ContainsKey("dagger") || Game.Inventory.ContainsKey("tablet"))
                {
                    currentRoom = "startroom";
                }
                else
                {
                    Game.scrolltext("There is a presence in this room, best not to enter.");
                    succeeded = false;
                }
                break;
            case "small room":
                currentRoom = "vinesroom";
                break;
            case "open room":
                currentRoom = "kniferoom";
                break;
            case "down":
                currentRoom = "hallway2";
                break;
            case "locked door":
                if (Game.Inventory.ContainsKey("tablet"))
                {
                    currentRoom = "tabletroom";
                }
                else
                {
                    Game.scrolltext("This door is sealed with some kind of dark magic, you will need some sort of artifact to access it.");
                    succeeded = false;
                }
                break;
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
        if (movement == "doorway")
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
            case "corridor":
                currentRoom = "hallway1";
                break;
            case "side entrance":
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
            case "side entrance":
                currentRoom = "vinesroom";
                break;
            case "locked door":
                if (Game.Inventory.ContainsKey("tablet"))
                {
                    Game.scrolltext("Luckily, the tablet you found broke the seal on the door.\nYou run through and quickly shut it behind you before the beast enters the room.\nYou seem to have escaped its wraith for now");
                    currentRoom = "hallway1";
                }
                else
                {
                    Game.scrolltext("This door is sealed with some kind of dark magic, you will need some sort of artifact to access it.");
                    succeeded = false;
                }
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
                    Game.EndGame();
                }
                else
                {
                    Game.scrolltext("This door is locked by an ancient mechanism. You will need a key.");
                    succeeded = false;
                }
                break;
            case "up":
                currentRoom = "hallway1";
                break;
            case "down":
                currentRoom = "hallway3";
                break;
            case "first door":
                if (Game.Inventory.ContainsKey("tablet"))
                {
                    currentRoom = "renovatedroom";
                }
                else
                {
                    Game.scrolltext("This door is sealed with some kind of dark magic, you will need some sort of artifact to access it.");
                    succeeded = false;
                }
                break;
            case "side door":
                if (Game.Inventory.ContainsKey("tablet"))
                {
                    currentRoom = "spidersroom";
                }
                else
                {
                    Game.scrolltext("This door needs a tablet.");
                    succeeded = false;
                }
                break;
            case "fourth door":
                if (Game.Inventory.ContainsKey("tablet"))
                {
                    if (!Game.LurkerMoved)
                    {
                        currentRoom = "smashingroom";
                    }
                    else
                    {
                        Game.scrolltext("You hear the lurker in this room. You shouldn't go in.");
                        succeeded = false;
                    }
                }
                else
                {
                    Game.scrolltext("This door needs a tablet.");
                    succeeded = false;
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
            case "up":
                currentRoom = "hallway2";
                break;
            case "down":
                currentRoom = "hallway4";
                break;
            case "door":
                if (Game.Inventory.ContainsKey("tablet"))
                {
                    currentRoom = "eyesroom";
                }
                else
                {
                    Game.scrolltext("This door is sealed with some kind of dark magic, you will need some sort of artifact to access it.");
                    succeeded = false;
                }
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
            case "forward door":
                if (Game.Inventory.ContainsKey("tablet"))
                {
                    currentRoom = "startroom";
                }
                else
                {
                    Game.scrolltext("This door is sealed with some kind of dark magic, you will need some sort of artifact to access it.");
                    succeeded = false;
                }
                break;
            case "side door":
                if (Game.Inventory.ContainsKey("tablet"))
                {
                    currentRoom = "keyroom";
                }
                else
                {
                    Game.scrolltext("This door is sealed with some kind of dark magic, you will need some sort of artifact to access it.");
                    succeeded = false;
                }
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
        switch (movement)
        {
            case "main entrance":
                currentRoom = "hallway2";
                break;
            case "side entrance":
                if (Game.LurkerMoved)
                {
                    currentRoom = "eyesroom";
                }
                else 
                {
                    Game.scrolltext("Something feels strange about this door. You can't bring yourself to step through yet."); 
                    succeeded = false; 
                }
                break;
            default:
                succeeded = false;
                break;
        }
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
                    Game.scrolltext("The exit is blocked by an egg sac.");
                    succeeded = false;
                }
                else
                {
                    currentRoom = "hallway2";
                }
                break;
            case "side entrance":
                currentRoom = "renovatedroom";
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
            case "door":
                currentRoom = "hallway4";
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
            case "side entrance":
                currentRoom = "spidersroom";
                break;
            case "main entrance":
                currentRoom = "hallway2";
                break;
            default:
                succeeded = false;
                break;
        }
        return succeeded;
    }

    public static bool eyesroom(string movement)
    {
        bool succeeded = true;
        switch (movement)
        {
            case "main entrance":
                currentRoom = "hallway3";
                break;
            case "side entrance":
                Game.scrolltext("The door seemed to lock itself behind you.");
                break;
            default:
                succeeded = false;
                break;
        }
        return succeeded;
    }

    public static bool move(string movement)
    {
        switch (currentRoom)
        {
            case "startroom":
                return startroom(movement);
            case "sideroom":
                return sideroom(movement);
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
            case "renovatedroom":
                return renovatedroom(movement);
            case "eyesroom":
                return eyesroom(movement);
            default:
                return false;
        }
    }
}

