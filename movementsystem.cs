namespace AwesomeGame;

// Class containing all code for moving between rooms, 
public class MovementSystem
{

    // Assigns string "startroom" to the currentRoom variable
    public static string currentRoom = "startroom";
    private static bool cleanerpresent = false;
    // Movement system for Start Room
    public static bool startroom(string movement)
    {
        bool succeeded = true;
        switch (movement)
        {
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
    // Movement system for hallway1
    public static bool hallway1(string movement)
    {
        bool succeeded = true;
        switch (movement)
        {
            case "starting room":
                if (!Game.Inventory.ContainsKey("dagger"))
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
                if (Game.Inventory.ContainsKey("tablet") && !cleanerpresent)
                {
                    currentRoom = "tabletroom";
                    succeeded = false;
                }
                else if (Game.Inventory.ContainsKey("tablet"))
                {
                    Game.scrolltext("listening closely the noise of the beast still trembles from inside this room. \nyou refuse to enter");
                    succeeded = false;
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
                    Game.scrolltext("Luckily, the tablet you found broke the seal on the door.\nYou run through and quickly shut it behind you before the beast enters the room.\nYou seem to have escaped its wrath for now.");
                    currentRoom = "hallway1";
                    cleanerpresent = true;
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
                    succeeded = false;
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
                if (Game.LurkerMoved)
                {
                    currentRoom = "hallway3";
                    succeeded = true;
                }
                else
                {
                    Game.scrolltext("the lurker is further down the hallway, best not to approach it");
                    succeeded = false;
                }
                break;
            case "first door":
                if (Game.Inventory.ContainsKey("tablet"))
                {
                    currentRoom = "renovatedroom";
                    succeeded = true;
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
                    if (Game.SpiderSacBurst)
                    {
                        currentRoom = "spidersroom";
                    }
                    else
                    {
                        Game.scrolltext("This door won't budge.");
                        succeeded = false;
                    }
                }
                else
                {
                    Game.scrolltext("This door is sealed with some kind of dark magic, you will need some sort of artifact to access it.");
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
                    Game.scrolltext("How the fuck did you even get here without a tablet you dirty cheater <r>fuck<r> you");
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
            case "up":
                if (Game.VinesCut && !Game.LurkerMoved)
                {
                    Game.scrolltext("you can hear something grumbling further up the hallway, best not to approach");
                    succeeded = false;
                }
                else
                {
                    currentRoom = "hallway3";
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
            case "first door":
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
                if (Game.LurkerMoved)
                {
                    Game.scrolltext("The door seemed to lock itself behind you.");
                    succeeded = false;
                }
                else
                {
                    Game.scrolltext("how are you even here bro");
                    currentRoom = "smashingroom";
                }
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

