//for grabbing values from json
using System.Text.Json;
using AwesomeGame;
using static System.Console;

namespace AwesomeGame;

internal static class Game
{
    public static Dictionary<string, object> Inventory = new Dictionary<string, object>();

    // Flags to show that an action has been completed
    public static bool VinesCut = false, SpiderSacBurst = false, LurkerMoved = false;

    //To use this, make a string and split different lines with | to alter speed do scrolltext('example string', 100), this will slow it
    public static void scrolltext(string Text, int speed = 50)
    {
        int i = 0;
        bool skipped = false;
        while (i < Text.Length && !skipped)
        {
            char o = Text[i];
            // Checks if key has been pressed without blocking the rest of the code from running
            if (Console.KeyAvailable)
            {
                var key = ReadKey(true).Key;

                // Skip if pressing space or enter
                skipped = (key == ConsoleKey.Spacebar || key == ConsoleKey.Enter);
            }

            if (skipped)
            {
                string restOfText = Text.Substring(i);
                Write(restOfText);
            }
            else
            {
                Write(o);
                Thread.Sleep(speed);
            }

            i++;
        }
        WriteLine("");
    }
    // uses regex to find text within two <> tags
    public static void tagtext(string input)
    {
        var match = Regex.Match(input, @"(.*?)<g>(.*?)<g>(.*)");
        // gets input string, checks for <g> and gets wildcard inside <g>
        if (!match.Success)
        {
            Write(input);
            return;
        }
        // turns text inside tags green and uppercase, could add something that makes text [LIKE THIS], in brackets?
        Console.Write(match.Groups[1].Value);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(match.Groups[2].Value.ToUpper());
        Console.ResetColor();
        Console.Write(match.Groups[3].Value);
    }
    public static void Main()
    {
        // interprets the json as a list of , so we can have a list of items in there for simplicties sake.to get values, needs to be deserialised later
        string items_import = File.ReadAllText("items.json");
        Dictionary<string, object> Items = JsonSerializer.Deserialize<Dictionary<string, object>>(items_import);
        string rooms_import = File.ReadAllText("rooms.json");
        Dictionary<string, object> Rooms = JsonSerializer.Deserialize<Dictionary<string, object>>(rooms_import);

        scrolltext("Waking up disoriented, you open your eyes.\n" +
                    "Everything is dark, in your panic you flail your limbs until you feel something around you.\n" +
                    "you are blind.");
        scrolltext("input h, or help for a current list of actions", 10);
        int actionsCompleted = 0;
        bool condition = true, secretsEnabled = false;
        
        while (condition == true)
        {
            var roomtemp = (JsonElement)Rooms[MovementSystem.currentRoom];
            int room_actions = roomtemp.GetProperty("actions").GetInt32();

            if (room_actions != 0)
            {
                if (actionsCompleted > room_actions)
                    condition = false;
                {
                    if (actionsCompleted > room_actions / 2 && actionsCompleted < room_actions)
                        scrolltext("You hear something loud approaching");
                    if (actionsCompleted >= room_actions - 1)
                        scrolltext("You should move on");
                }
            }

            string inputString = ReadLine().ToLower();
            string[] input = inputString.Split(' ');
            switch (input[0])
            {
                case "help":
                case "h":
                    // please add any commands you add to the program to this help section !!!

                    WriteLine("inspect: inspects item or room with more detail than the description, inspect room");
                    WriteLine("stats: shows your current EXP");
                    WriteLine("help: shows a list and description of commands");
                    WriteLine("quit, kill, exit: closes the game");
                    WriteLine("inventory: prints contents of the inventory");
                    if (MovementSystem.currentRoom.Contains("features"))
                        WriteLine("loot: takes a given item in the current room");
                    //these are dev commands, activated by typing 'secret2'
                    if (MovementSystem.currentRoom == "vinesroom" && VinesCut == false)
                        WriteLine("cut vines: cuts the vines covering the door");
                    if (secretsEnabled)
                    {
                        WriteLine("goto: sends you to a room");
                        WriteLine("give: gives a provided item");
                        WriteLine("do_damage: command to test property damage system");
                        WriteLine("show_bill: command to show the current property damage");
                    }
                    break;
                case "inventory":
                    if (Inventory.Count > 0)
                    {
                        WriteLine("you have:");
                        foreach (KeyValuePair<string, object> Inv in Inventory)
                        {
                            WriteLine(Inv.Key);
                        }
                    }
                    else
                    {
                        WriteLine("you don't have any items");
                    }
                    break;
                case "inspect":
                    if (input.Length > 1 && Inventory.ContainsKey(input[1])) //this looks for items in the player's command
                    {
                        //retrieves values from json
                        var item = (JsonElement)Items[input[1]];
                        foreach (var property in item.EnumerateObject())
                            WriteLine($"{property.Name}: {property.Value}");
                    }
                    else if (input.Length > 1 && input[1] == "room") //this looks for the word 'room' in the player's command and then inspects the room
                    {
                        JsonElement room = (JsonElement)Rooms[MovementSystem.currentRoom];
                        string description = null;
                        // i will add more rooms with changing conditions to this if statement once we have the conditions sorted
                        if (
                            (MovementSystem.currentRoom == "startroom" && Inventory.ContainsKey("book")) ||
                            (MovementSystem.currentRoom == "kniferoom" && Inventory.ContainsKey("dagger")) ||
                            (MovementSystem.currentRoom == "vinesroom" && VinesCut == true) ||
                            (MovementSystem.currentRoom == "hallway2" && VinesCut == true && LurkerMoved == false) ||
                            (MovementSystem.currentRoom == "tabletroom" && Inventory.ContainsKey("tablet")) ||
                            (MovementSystem.currentRoom == "smashingroom" && LurkerMoved == true) ||
                            (MovementSystem.currentRoom == "spidersroom" && SpiderSacBurst)
                        )
                        {
                            description = room.GetProperty("description2").GetString();
                        }
                        else
                        {
                            description = room.GetProperty("description").GetString();
                        }
                        WriteLine(description);
                        foreach (var features in room.GetProperty("features").EnumerateArray())
                        {
                            if (Inventory.ContainsKey(features.GetString()))
                                break;
                            else
                                WriteLine($"You feel: {features.GetString()}");
                        }
                    }
                    else
                    {
                        WriteLine("you don't have that item");
                    }
                    actionsCompleted++;
                    break;
                case "stats":
                    WriteLine($"You have {PropertyDamage.TotalCost} EXP");
                    break;
                //give command, takes the value from the item dictionary and copies it into inventory
                case "give":
                    if (secretsEnabled)
                    {
                        if (input.Length > 1 && Items.ContainsKey(input[1]))
                        {
                            Inventory[input[1]] = Items[input[1]];
                            WriteLine($"you now have {input[1]}");
                        }
                        else
                        {
                            WriteLine("this item does not exist");
                        }
                    }
                    else { WriteLine("you can't do that right now"); }
                    break;
                case "cut":
                    if (input[1] == "vines" && MovementSystem.currentRoom == "vinesroom")
                    {
                        if (Inventory.ContainsKey("dagger"))
                        {
                            VinesCut = true;
                            PropertyDamage.CauseDamage("Destroyed cabling in network room", 2000);
                            WriteLine("you cut the vines on the door");
                        }
                        else
                        {
                            WriteLine("you need something sharp to cut these vines");
                        }
                    }

                    break;
                // Debug commands
                case "do_damage":
                    if (secretsEnabled)
                    {
                        PropertyDamage.CauseDamage("Did a scary test thing that cost $200", 200);
                        WriteLine("You did a test, you gained 200 EXP!");
                    }
                    else
                    {
                        WriteLine("you can't do that right now");
                    }
                    break;
                case "show_bill":
                    if (secretsEnabled)
                    {
                        PropertyDamage.WriteBill();
                    }
                    else
                    {
                        WriteLine("you can't do that right now");
                    }
                    break;
                case "goto":
                    if (secretsEnabled)
                    {
                        if (input.Length > 1 && Rooms.ContainsKey(input[1]))
                        {
                            MovementSystem.currentRoom = input[1];
                            if (Rooms.ContainsKey(input[1]))
                            {
                                scrolltext($"you are now in: {MovementSystem.currentRoom}");
                                actionsCompleted = 0;
                            }
                        }
                        else
                        {
                            WriteLine("this room does not exist");
                        }
                    }
                    else
                    {
                        WriteLine("you can't do that right now");
                    }
                    break;
                // ^ End of debug commands
                case "quit":
                case "exit":
                case "kill":
                    condition = false;
                    break;
                case "secret":
                    WriteLine("you thought lol");
                    break;
                case "secret2":
                    if (!secretsEnabled) { secretsEnabled = true; WriteLine("enabled"); }
                    else { secretsEnabled = false; WriteLine("disabled"); }
                    break;

                case "attack":
                    // Spider room, Abby's responsibility
                    if (MovementSystem.currentRoom == "spidersroom")
                    {
                        if (Inventory.ContainsKey("dagger"))
                        {
                            SpiderSacBurst = true;

                            scrolltext("You stab at the sac, slashing your way through...");
                            Thread.Sleep(500);

                            scrolltext("The sac bursts open, releasing hundreds, possibly thousands of eggs! You can barely walk without crushing dozens of eggs.");

                            PropertyDamage.CauseDamage("Shredded bean bag", 60);
                            PropertyDamage.CauseDamage("Cleanup of bean bag beans in common room", 300);

                        }
                    }
                    break;
                //switch for looting items
                case "loot":
                    switch (MovementSystem.currentRoom)
                    {
                        case "tabletroom":
                            if (input.Length > 1 && input[1] == "corpse")
                            {
                                if (Inventory.ContainsKey("tablet"))
                                {
                                    WriteLine("You have already looted the corpse.");
                                }
                                else
                                {
                                    Inventory["tablet"] = Items["tablet"];
                                    Inventory["coins"] = Items["coins"];
                                    WriteLine($"From the corpse you loot some sort of tablet, and an array of coins.");
                                }
                            }
                            break;
                        case "kniferoom":
                            if (input.Length > 1 && input[1] == "dagger")
                            {
                                if (Inventory.ContainsKey("dagger"))
                                {
                                    WriteLine("You already have the dagger.");
                                }
                                else
                                {
                                    Inventory["dagger"] = Items["dagger"];
                                    WriteLine($"You take a dagger from its position on the bench");
                                }
                            }
                            break;
                        case "startroom":
                            if (input.Length > 1 && input[1] == "book")
                            {
                                if (Inventory.ContainsKey("book"))
                                {
                                    WriteLine("You already have the book.");
                                }
                                else
                                {
                                    Inventory["book"] = Items["book"];
                                    WriteLine($"You take the book from the table");
                                }
                            }
                            break;
                        case "renovatedroom":
                            if (input.Length > 1 && input[1] == "hammer")
                            {
                                if (Inventory.ContainsKey("hammer"))
                                {
                                    WriteLine("You already have the hammer.");
                                }
                                else
                                {
                                    Inventory["hammer"] = Items["hammer"];
                                    WriteLine($"You take the hammer from its place on the ground, it is cumbersome but comforting");
                                }
                            }

                            break;
                        case "keyroom":
                            if (input.Length > 1 && input[1] == "key")
                            {
                                if (Inventory.ContainsKey("key"))
                                {
                                    WriteLine("You already have the key.");
                                }
                                else
                                {
                                    Inventory["key"] = Items["key"];
                                    WriteLine($"You take the key");
                                }
                            }
                            break;
                        default:
                            WriteLine("there is nothing to loot here");
                            break;
                    }
                    break;
                default:
                    bool movementSucceeded = MovementSystem.Move(inputString);
                    if (movementSucceeded)
                    {
                        actionsCompleted = 0;
                        scrolltext("You move....");
                    }

                    break;
            }
        }
    }
}
