//for grabbing values from json
using System.Text.Json;
using System.Text.RegularExpressions;
using AwesomeGame;
using static System.Console;
namespace AwesomeGame;

internal static class Game
{
    public static Dictionary<string, object> Inventory = new Dictionary<string, object>();

    // Flags to show that an action has been completed
    public static bool VinesCut = false, SpiderSacBurst = false, LurkerMoved = false;


    //this sucks and i hate it but it works
    public static void scrolltext(string Text, int speed = 10)
    {
        var match = Regex.Match(Text, @"(.*?)<g>(.*?)<g>(.*)");
        if (match.Success)
        {
            bool skipped = false;
            // function recursion lmao, couldnt think if an easier way to do this
            void Writeportion(string portion, ConsoleColor? color = null)
            {
                for (int i = 0; i < portion.Length; i++)
                {
                    if (!skipped && KeyAvailable)
                    {
                        var key = ReadKey(true).Key;
                        if (key == ConsoleKey.Spacebar || key == ConsoleKey.Enter)
                            skipped = true;
                    }
                    if (color.HasValue) ForegroundColor = color.Value;
                    if (skipped)
                    {
                        Write(portion.Substring(i));
                        break;
                    }
                    Write(portion[i]);
                    Thread.Sleep(speed);
                }
                ResetColor();
            }
            Writeportion(match.Groups[1].Value);
            Writeportion(match.Groups[2].Value.ToUpper(), ConsoleColor.Green);
            Writeportion(match.Groups[3].Value);
            WriteLine();
        }
        else
        {
            bool skipped = false;
            for (int i = 0; i < Text.Length; i++)
            {
                if (!skipped && Console.KeyAvailable)
                {
                    var key = ReadKey(true).Key;
                    if (key == ConsoleKey.Spacebar || key == ConsoleKey.Enter)
                        skipped = true;
                }

                if (skipped)
                {
                    Write(Text.Substring(i));
                    break;
                }

                Write(Text[i]);
                Thread.Sleep(speed);
            }
            WriteLine();
        }
    }
    /*
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
            */
    public static void Main()
    {
        // interprets the json as a list of , so we can have a list of items in there for simplicties sake.to get values, needs to be deserialised later
        string items_import = File.ReadAllText("items.json");
        Dictionary<string, object> Items = JsonSerializer.Deserialize<Dictionary<string, object>>(items_import);
        string rooms_import = File.ReadAllText("rooms.json");
        Dictionary<string, object> Rooms = JsonSerializer.Deserialize<Dictionary<string, object>>(rooms_import);

        scrolltext("Waking up disoriented, you open your eyes.\n" +
                    "Everything is dark, in your panic you flail your limbs until you feel something around you.\n" +
                    "you are blind.", 50);
        scrolltext("input h, or help for a current list of actions", 10);
        int actionsCompleted = 0;
        bool condition = true, secretsEnabled = false;

        while (condition == true)
        {
            var roomtemp = (JsonElement)Rooms[MovementSystem.currentRoom];
            int room_actions = roomtemp.GetProperty("actions").GetInt32();

            if (room_actions > 0)
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

                    scrolltext("inspect: <g>inspects<g> item or room with more detail than the description, inspect room", 50);
                    scrolltext("stats: shows your current EXP");
                    scrolltext("help: shows a list and description of commands");
                    scrolltext("quit, kill, exit: closes the game");
                    scrolltext("inventory: prints contents of the inventory");
                    if (MovementSystem.currentRoom.Contains("features"))
                        scrolltext("loot: takes a given item in the current room");
                    //these are dev commands, activated by typing 'secret2'
                    if (MovementSystem.currentRoom == "vinesroom" && VinesCut == false)
                        scrolltext("cut vines: cuts the vines covering the door");
                    if (secretsEnabled)
                    {
                        scrolltext("goto: sends you to a room");
                        scrolltext("give: gives a provided item");
                        scrolltext("do_damage: command to test property damage system");
                        scrolltext("show_bill: command to show the current property damage");
                    }
                    break;
                case "inventory":
                    if (Inventory.Count > 0)
                    {
                        scrolltext("you have:");
                        foreach (KeyValuePair<string, object> Inv in Inventory)
                        {
                            scrolltext(Inv.Key);
                        }
                    }
                    else
                    {
                        scrolltext("you don't have any items");
                    }
                    break;
                case "inspect":
                    if (input.Length > 1 && Inventory.ContainsKey(input[1])) //this looks for items in the player's command
                    {
                        //retrieves values from json
                        var item = (JsonElement)Items[input[1]];
                        foreach (var property in item.EnumerateObject())
                            scrolltext($"{property.Name}: {property.Value}");
                    }
                    else if (input.Length > 1 && input[1] == "room") //this looks for the word 'room' in the player's command and then inspects the room
                    {
                        JsonElement room = (JsonElement)Rooms[MovementSystem.currentRoom];
                        string description = null;
                        // i will add more rooms with changing conditions to this if statement once we have the conditions sorted
                        if (
                            (MovementSystem.currentRoom == "startroom" && Inventory.ContainsKey("book")) ||
                            (MovementSystem.currentRoom == "kniferoom" && Inventory.ContainsKey("dagger")) ||
                            (MovementSystem.currentRoom == "vinesroom" && VinesCut) ||
                            (MovementSystem.currentRoom == "hallway2" && VinesCut && !LurkerMoved) ||
                            (MovementSystem.currentRoom == "tabletroom" && Inventory.ContainsKey("tablet")) ||
                            (MovementSystem.currentRoom == "smashingroom") ||
                            (MovementSystem.currentRoom == "spidersroom" && SpiderSacBurst)
                        )
                        {
                            description = room.GetProperty("description2").GetString();
                        }
                        else
                        {
                            description = room.GetProperty("description").GetString();
                        }
                        scrolltext(description);

                        if (room.TryGetProperty("features", out JsonElement featuresElement))
                        {
                            foreach (var features in room.GetProperty("features").EnumerateArray())
                            {
                                foreach (var feature in featuresElement.EnumerateArray())
                                {
                                    if (Inventory.ContainsKey(feature.GetString()))
                                        break;
                                    else
                                        WriteLine($"You feel: {feature.GetString()}");
                                }
                            }
                        }
                    }
                    else
                    {
                        scrolltext("you don't have that item");
                    }
                    actionsCompleted++;
                    break;
                case "stats":
                    scrolltext($"You have {PropertyDamage.TotalCost} EXP");
                    break;
                //give command, takes the value from the item dictionary and copies it into inventory
                case "give":
                    if (secretsEnabled)
                    {
                        if (input.Length > 1 && Items.ContainsKey(input[1]))
                        {
                            Inventory[input[1]] = Items[input[1]];
                            scrolltext($"you now have {input[1]}");
                        }
                        else
                        {
                            scrolltext("this item does not exist");
                        }
                    }
                    else { scrolltext("you can't do that right now"); }
                    break;
                case "cut":
                    if (input[1] == "vines" && MovementSystem.currentRoom == "vinesroom")
                    {
                        if (Inventory.ContainsKey("dagger"))
                        {
                            VinesCut = true;
                            PropertyDamage.CauseDamage("Destroyed cabling in network room", 2000);
                            scrolltext("you cut the vines on the door");
                        }
                        else
                        {
                            scrolltext("you need something sharp to cut these vines");
                        }
                    }

                    break;
                // Debug commands
                case "do_damage":
                    if (secretsEnabled)
                    {
                        PropertyDamage.CauseDamage("Did a scary test thing that cost $200", 200);
                        scrolltext("You did a test, you gained 200 EXP!");
                    }
                    else
                    {
                        scrolltext("you can't do that right now");
                    }
                    break;
                case "show_bill":
                    if (secretsEnabled)
                    {
                        PropertyDamage.WriteBill();
                    }
                    else
                    {
                        scrolltext("you can't do that right now");
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
                            scrolltext("this room does not exist");
                        }
                    }
                    else
                    {
                        scrolltext("you can't do that right now");
                    }
                    break;
                // ^ End of debug commands
                case "quit":
                case "exit":
                case "kill":
                    condition = false;
                    break;
                case "secret":
                    scrolltext("you thought lol");
                    break;
                case "secret2":
                    if (!secretsEnabled) { secretsEnabled = true; scrolltext("enabled"); }
                    else { secretsEnabled = false; scrolltext("disabled"); }
                    break;

                case "attack":
                    // Spider room, Abby's responsibility
                    if (MovementSystem.currentRoom == "spidersroom" && !SpiderSacBurst)
                    {
                        if (Inventory.ContainsKey("dagger"))
                        {
                            SpiderSacBurst = true;

                            scrolltext("You stab at the sac, slashing your way through...");
                            Thread.Sleep(500);

                            scrolltext("The sac bursts open, releasing hundreds, possibly thousands of eggs! You can barely walk without crushing dozens of eggs. You gain 110 EXP.");

                            PropertyDamage.CauseDamage("Shredded bean bag", 60);
                            PropertyDamage.CauseDamage("Cleanup of bean bag beans in common room", 50);

                        }
                        else
                        {
                            scrolltext("How did you get here without a knife?");
                        }
                    } 
                    else { WriteLine("you can't do that right now"); }
                    break;
                case "smash":  // SMASHING ROOM WOOP WOOP
                    if (MovementSystem.currentRoom == "smashingroom" && !LurkerMoved)
                    {
                        if (Inventory.ContainsKey("hammer"))
                        {
                            LurkerMoved = true;

                            scrolltext("with a heave, you lift up the warhammer and bring it down upon one of the strange obelisks, \nit smashes into pieces that scatter across the table \nyou smash another, and then another, you can hear the lurker, startled, begin to make its way to the main door\n");
                            scrolltext("it's time to get moving");

                            PropertyDamage.CauseDamage("destroyed Two PCs and a monitor  in D201", 5300);
                        }
                        else { scrolltext("you tried to smash one of the obelisks, but you just hurt your hand instead, ouch"); }
                    }
                    else { WriteLine("you can't do that right now"); }
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
                                    scrolltext("You have already looted the corpse.");
                                }
                                else
                                {
                                    Inventory["tablet"] = Items["tablet"];
                                    Inventory["coins"] = Items["coins"];
                                    scrolltext($"From the corpse you loot some sort of tablet, and an array of coins.");
                                }
                            }
                            break;
                        case "kniferoom":
                            if (input.Length > 1 && input[1] == "dagger")
                            {
                                if (Inventory.ContainsKey("dagger"))
                                {
                                    scrolltext("You already have the dagger.");
                                }
                                else
                                {
                                    Inventory["dagger"] = Items["dagger"];
                                    scrolltext($"You take a dagger from its position on the bench");
                                }
                            }
                            break;
                        case "startroom":
                            if (input.Length > 1 && input[1] == "book")
                            {
                                if (Inventory.ContainsKey("book"))
                                {
                                    scrolltext("You already have the book.");
                                }
                                else
                                {
                                    Inventory["book"] = Items["book"];
                                    scrolltext($"You take the book from the table");
                                }
                            }
                            break;
                        case "renovatedroom":
                            if (input.Length > 1 && (input[1] == "hammer" || input[1] == "warhammer"))
                            {
                                if (Inventory.ContainsKey("hammer"))
                                {
                                    scrolltext("You already have the hammer.");
                                }
                                else
                                {
                                    Inventory["hammer"] = Items["hammer"];
                                    scrolltext($"You take the hammer from its place on the ground, it is cumbersome but comforting");
                                }
                            }

                            break;
                        case "keyroom":
                            if (input.Length > 1 && input[1] == "key")
                            {
                                if (Inventory.ContainsKey("key"))
                                {
                                    scrolltext("You already have the key.");
                                }
                                else
                                {
                                    Inventory["key"] = Items["key"];
                                    scrolltext($"You take the key");
                                }
                            }
                            break;
                        default:
                            scrolltext("there is nothing to loot here");
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
