public class MovementSystem
{
    string currentRoom = "startroom";
    public void StartRoom(string movement)
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
                if (Inventory.Contains("tablet"))
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

    public void hallway1(string movement)
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
                if (Inventory.Contains("tablet"))
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

    public void kniferoom(string movement)
    {
        if (movement == "hallway")
        {
            currentRoom = "hallway1";
        }
    }

    public void vinesroom(string movement)
    {
        switch (movement)
        {
            case "main entrance":
                currentRoom = "hallway1";
                break;
            case "main room":
                if (vinescut == true)
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

    public void tabletroom(string movement)
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

    public void hallway2(string movement)
    {
        switch (movement)
        {
            case "glass door":
                if (Inventory.Contains("key"))
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

    public void hallway3(string movement)
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

    public void hallway4(string movement)
    {
        switch (movement)
        {
            case "starting room":
                if (Inventory.Contains("tablet"))
                {
                    currentRoom = "startroom";
                    break;
                }
                else
                {
                    Console.WriteLine("You do not have a tablet.");
                }
            case "locked door":
                if (Inventory.Contains("tablet"))
                {
                    currentRoom = "keyroom";
                    break;
                }
                else
                {
                    Console.WriteLine("You do not have a tablet.");
                    break;
                }
            case "south":
                currentRoom = "hallway3";
                break;
        }
    }

    public void keyroom(string movement)
    {
        switch (movement)
        {
            case "hallway":
                currentRoom = "hallway4";
                break;
        }
    }
}

