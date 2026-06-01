using System.Diagnostics;
using System.Runtime.InteropServices;
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
        var matches = Regex.Matches(Text, @"<g>(.*?)<g>");
        bool skipped = false;
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

        int lastIndex = 0;
        foreach (Match match in matches)
        {
            if (match.Index > lastIndex)
                Writeportion(Text.Substring(lastIndex, match.Index - lastIndex));
            Writeportion(match.Groups[1].Value.ToUpper(), ConsoleColor.Green);
            lastIndex = match.Index + match.Length;
        }
        if (lastIndex < Text.Length)
            Writeportion(Text.Substring(lastIndex));

        WriteLine();
    }
    public static void Main()
    {
        // interprets the json as a list of , so we can have a list of items in there for simplicties sake.to get values, needs to be deserialised later
        string items_import = File.ReadAllText("items.json");
        Dictionary<string, object> Items = JsonSerializer.Deserialize<Dictionary<string, object>>(items_import) ?? throw new FileNotFoundException("items.json could not be found");
        string rooms_import = File.ReadAllText("rooms.json");
        Dictionary<string, object> Rooms = JsonSerializer.Deserialize<Dictionary<string, object>>(rooms_import) ?? throw new FileNotFoundException("rooms.json could not be found");;

        scrolltext("You find yourself dazed and confused in a room that is completely pitch black.\nAs you struggle to your feet, your hands meet cold, unforgiving surfaces.\nPanic sets in as you wave a hand before your face and see nothing. Have you gone blind, or have you awoken within some forgotten catacomb?", 50);

        int actionsCompleted = 0;
        bool condition = true, secretsEnabled = false;
        while (condition == true)
        {
            if (MovementSystem.currentRoom == "staircase")
            {
                EndGame();
                break;
            }

            WriteLine("===============================================");
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

            scrolltext("(Input <g>help<g> for a current list of actions)", 10);

            Write("Input: ");

            // The "??" is to stop everything from breaking if for some reason the game can't read an input
            string inputString = (ReadLine() ?? "").ToLower();
            string[] input = inputString.Split(' ');

            switch (input[0])
            {
                case "help":
                    // please add any commands you add to the program to this help section !!!

                    scrolltext("<g>inspect<g> (item name/<g>room<g>): Inspects item or room with more detail than the description, inspect room");
                    scrolltext("<g>stats<g>: Shows your current EXP");
                    scrolltext("<g>help<g>: Shows a list and description of commands");
                    scrolltext("<g>inventory<g>: Prints contents of the inventory");
                    scrolltext("<g>door name<g>: Enter the name of a door to move rooms");
                    if (roomtemp.TryGetProperty("features", out _))
                        scrolltext("<g>loot<g> (<g>item<g>/<g>object<g>): Takes an item from the room");
                    //these are dev commands, activated by typing 'secret2'
                    if (MovementSystem.currentRoom == "vinesroom" && VinesCut == false)
                        scrolltext("<g>cut vines<g>: cuts the vines covering the door");
                    if (secretsEnabled)
                    {
                        scrolltext("<g>goto<g>: sends you to a room");
                        scrolltext("<g>give<g>: gives a provided item");
                        scrolltext("<g>do_damage<g>: command to test property damage system");
                        scrolltext("<g>show_bill<g>: command to show the current property damage");
                    }
                    scrolltext("<g>exit<g>: Closes the game");
                    break;
                case "inventory":
                    if (Inventory.Count > 0)
                    {
                        scrolltext("You have:");
                        foreach (KeyValuePair<string, object> Inv in Inventory)
                        {
                            scrolltext($"<g>{Inv.Key}<g>");
                        }
                    }
                    else
                    {
                        scrolltext("You don't have any items");
                    }
                    break;
                case "inspect":
                    if (input.Length > 1 && Inventory.ContainsKey(input[1]))
                    {
                        var item = (JsonElement)Items[input[1]];
                        foreach (var property in item.EnumerateObject())
                            scrolltext($"<g>{property.Name}<g>: {property.Value}");
                    }
                    else if (input.Length > 1 && input[1] == "room") //this looks for the word 'room' in the player's command and then inspects the room
                    {
                        JsonElement room = (JsonElement)Rooms[MovementSystem.currentRoom];
                        string description;
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
                            description = room.GetProperty("description2").GetString() ?? throw new MissingFieldException($"rooms.json has no description2 for {MovementSystem.currentRoom}");
                        }
                        else
                        {
                            description = room.GetProperty("description").GetString() ?? throw new MissingFieldException($"rooms.json has no description for {MovementSystem.currentRoom}");
                        }
                        scrolltext(description);

                        if (room.TryGetProperty("features", out JsonElement featuresElement))
                        {
                            foreach (var features in room.GetProperty("features").EnumerateArray())
                            {
                                foreach (var feature in featuresElement.EnumerateArray())
                                {
                                    string featureStr = feature.GetString() ?? throw new ArrayTypeMismatchException($"A feature in {MovementSystem.currentRoom} is not a string");
                                    if (Inventory.ContainsKey(featureStr))
                                        break;
                                    else
                                        scrolltext($"You feel: {featureStr}");
                                }
                            }
                        }
                    }
                    else
                    {
                        scrolltext("You don't have that item");
                    }
                    actionsCompleted++;
                    break;
                case "stats":
                    scrolltext($"You have {PropertyDamage.totalcost} EXP");
                    break;
                //give command, takes the value from the item dictionary and copies it into inventory
                case "give":
                    if (secretsEnabled)
                    {
                        if (input.Length > 1 && Items.ContainsKey(input[1]))
                        {
                            Inventory[input[1]] = Items[input[1]];
                            scrolltext($"You now have {input[1]}");
                        }
                        else
                        {
                            scrolltext("This item does not exist");
                        }
                    }
                    else { scrolltext("You can't do that right now"); }
                    break;
                case "cut":
                    if (input[1] == "vines" && MovementSystem.currentRoom == "vinesroom")
                    {
                        if (Inventory.ContainsKey("dagger"))
                        {
                            VinesCut = true;
                            PropertyDamage.causedamage("Destroyed cabling in network room", 2000);
                            scrolltext("You cut the vines on the door");
                        }
                        else
                        {
                            scrolltext("You need something sharp to cut these vines");
                        }
                    }
                    else { scrolltext("You can't do that right now"); }

                        break;
                // Debug commands
                case "do_damage":
                    if (secretsEnabled)
                    {
                        PropertyDamage.causedamage("Did a scary test thing that cost $200", 200);
                        scrolltext("You did a test, you gained 200 EXP!");
                    }
                    else
                    {
                        scrolltext("You can't do that right now");
                    }
                    break;
                case "show_bill":
                    if (secretsEnabled)
                    {
                        PropertyDamage.writebill();
                    }
                    else
                    {
                        scrolltext("You can't do that right now");
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
                                scrolltext($"You are now in: {MovementSystem.currentRoom}");
                                actionsCompleted = 0;
                            }
                        }
                        else
                        {
                            scrolltext("This room does not exist");
                        }
                    }
                    else
                    {
                        scrolltext("You can't do that right now");
                    }
                    break;
                // ^ End of debug commands
                case "exit":
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

                            PropertyDamage.causedamage("Shredded bean bag", 60);
                            PropertyDamage.causedamage("Cleanup of bean bag beans in common room", 50);

                        }
                        else
                        {
                            scrolltext("How did you get here without a knife?");
                        }
                    }
                    else { scrolltext("You can't do that right now"); }
                    break;
                case "smash":  // SMASHING ROOM POO POO
                    if (MovementSystem.currentRoom == "smashingroom" && !LurkerMoved)
                    {
                        if (Inventory.ContainsKey("hammer"))
                        {
                            LurkerMoved = true;

                            scrolltext("With a heave, you lift up the warhammer and bring it down upon one of the strange obelisks, \nit smashes into pieces that scatter across the table \nyou smash another, and then another, you can hear the lurker, startled, begin to make its way to the main door\n");
                            scrolltext("Tt's time to get moving");

                            PropertyDamage.causedamage("Destroyed two PCs and a monitor in D201", 5300);
                        }
                        else { scrolltext("You tried to smash one of the obelisks, but you just hurt your hand instead, ouch"); }
                    }
                    else { scrolltext("You can't do that right now"); }
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
                    bool movementSucceeded = MovementSystem.move(inputString);
                    if (movementSucceeded)
                    {
                        actionsCompleted = 0;
                        scrolltext("You move....");
                    }
                    break;
            }
        }

        Console.ReadLine();
    }
    public static void EndGame()
    {
        scrolltext("You carefully unlock the glass door and hesitantly push it open. Could this finally be the escape from this prison you find yourself in?", 30);
        scrolltext("You walk inside, hanging close to the wall so as to maintain your sense of direction. Your hand connects with a slender metal bar, as a sudden drop appears before you.\r\n", 30);
        scrolltext("You reach a foot down the cliff, clinging tight to the bar. Your body is bound tight with fear, your foot slowly descending down the edge. Suddenly, your foot finds ground, as you realise a stairwell has appeared before you.\r\n", 30);
        scrolltext("You slowly tread down the stairs, foot by foot, step by step. As you descend, you realise with a shock that your vision is returning! Your senses are overwhelmed by a blinding light, radiating from a closed door.\r\n", 30);
        scrolltext("Psyching yourself for danger, you open the door...........\r\n", 75);
        scrolltext("\"Hey, the building closed to students four hours ago, it's cleaners only now.\"\r\n", 30);
        scrolltext("You are in the ground floor of the Otago Polytechnic's D block, and you are staring face to face with the janitor.\r\n", 30);
        scrolltext("\"It's 4am, go home.\"\r\n", 30);
        scrolltext("THE NEXT DAY.....\r\n", 75);
        scrolltext("You wake up in your home at 2pm, still exhausted from last night's confusion. You yawn, then get out of bed.", 30);
        scrolltext("You go to check your mailbox and see a letter addressed to you with the polytech's logo. You open it up, and read the contents...\r\n", 30);
        PropertyDamage.writebill();
    }
}
