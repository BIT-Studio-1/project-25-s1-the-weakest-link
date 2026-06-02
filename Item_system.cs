using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using AwesomeGame;
using static System.Console;
namespace AwesomeGame;
internal static class Game
{
    public static Dictionary<string, object> Inventory = new Dictionary<string, object>();
    // Flags to show that an action has been completed
    public static bool VinesCut = false, SpiderSacBurst = false, LurkerMoved = false, EyesSmashed = false;
    public static Dictionary<string, object> Items;
    public static Dictionary<string, object> Rooms;
    public static int actionscompleted = 0;
    public static bool condition = true, secretsenabled = false;
    public static JsonElement currentroomjson;
    static string[] input;
    public static void scrolltext(string Text, int speed = 10)
    /*  
    this sucks and i hate it but it works
    its responsible for printing text in colour and in caps when important, it does this by using tags <g> similar to html
    it detects a specific string of text in input via regex and then colours that specific piece in a colour determined by the dictionary of colours
    add to it as you please
    */
    {
        var colour = new Dictionary<string, ConsoleColor>
        {
            { "g", ConsoleColor.Green },
            { "r", ConsoleColor.Red },
            { "b", ConsoleColor.Blue },
            { "y", ConsoleColor.Yellow },
        };
        var matches = Regex.Matches(Text, @"<(\w)>(.*?)<\1>");
        bool skipped = false;
        void Writeportion(string portion, ConsoleColor? colour = null)
        {
            for (int i = 0; i < portion.Length; i++)
            {
                if (!skipped && Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Spacebar || key == ConsoleKey.Enter)
                        skipped = true;
                }
                if (colour.HasValue)
                    Console.ForegroundColor = colour.Value;
                if (skipped)
                {
                    Console.Write(portion.Substring(i));
                    break;
                }
                Console.Write(portion[i]);
                Thread.Sleep(speed);
            }
            Console.ResetColor();
        }
        int lastIndex = 0;
        foreach (Match match in matches)
        {
            if (match.Index > lastIndex)
                Writeportion(Text.Substring(lastIndex, match.Index - lastIndex));
            string tag = match.Groups[1].Value;
            string content = match.Groups[2].Value;
            if (colour.TryGetValue(tag, out var color))
                Writeportion(content.ToUpper(), color);
            else
                Writeportion(content);
            lastIndex = match.Index + match.Length;
            lastIndex = match.Index + match.Length;
        }
        if (lastIndex < Text.Length)
            Writeportion(Text.Substring(lastIndex));
        Console.WriteLine();
    }
    public static void help()
    {
        // please add any commands you add to the program to this help section !!!

        scrolltext("<g>inspect<g> (item name/<g>room<g>): Inspects item or room with more detail than the description, inspect room");
        scrolltext("<g>stats<g>: Shows your current EXP");
        scrolltext("<g>help<g>: Shows a list and description of commands");
        scrolltext("<g>inventory<g>: Prints contents of the inventory");
        scrolltext("<g>door name<g>: Enter the name of a door to move rooms");
        if (currentroomjson.TryGetProperty("features", out _))
            scrolltext("<g>loot<g> (<g>item<g>/<g>object<g>): Takes an item from the room");
        //these are dev commands, activated by typing 'secret2'
        if (MovementSystem.currentRoom == "vinesroom" && VinesCut == false)
            scrolltext("<g>cut vines<g>: cuts the vines covering the door");
        if (secretsenabled)
        {
            scrolltext("<g>goto<g>: sends you to a room");
            scrolltext("<g>give<g>: gives a provided item");
            scrolltext("<g>do_damage<g>: command to test property damage system");
            scrolltext("<g>show_bill<g>: command to show the current property damage");
        }
        scrolltext("<g>exit<g>: Closes the game");
    }
    public static void inventory()
    {
        if (Inventory.Count > 0)
        {
            scrolltext("You have:");
            foreach (KeyValuePair<string, object> Inv in Inventory)
                scrolltext($"<y>{Inv.Key}<y>");
        }
        else
            scrolltext("You don't have any items");
    }
    public static void inspect()
    {
        if (input.Length > 1 && Inventory.ContainsKey(input[1]))
        {
            var item = (JsonElement)Items[input[1]];
            foreach (var property in item.EnumerateObject())
                scrolltext($"<g>{property.Name}<g>: {property.Value}");
        }
        else if (input.Length > 1 && input[1] == "room" || input.Length == 1) //this looks for the word 'room' in the player's command and then inspects the room
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
            (MovementSystem.currentRoom == "spidersroom" && SpiderSacBurst) ||
            (MovementSystem.currentRoom == "eyesroom" && EyesSmashed))
                description = room.GetProperty("description2").GetString() ?? throw new MissingFieldException($"rooms.json has no description2 for {MovementSystem.currentRoom}");
            else
                description = room.GetProperty("description").GetString() ?? throw new MissingFieldException($"rooms.json has no description for {MovementSystem.currentRoom}");
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
                            scrolltext($"You feel: <y>{featureStr}<y>");
                    }
                }
            }
        }
        else
            scrolltext("You don't have that item");
        actionscompleted++;
    }
    public static void stats()
    {
        scrolltext($"You have {PropertyDamage.totalcost} EXP");
    }
    public static void give()
    {
        if (secretsenabled)
        {
            if (input.Length > 1 && Items.ContainsKey(input[1]))
            {
                takeitem(input[1]);
                scrolltext($"You now have {input[1]}");
            }
            else
            {
                scrolltext("This item does not exist");
            }
        }
        else { scrolltext("You can't do that right now"); }
    }
    public static void cut()
    {
        if (input[1] == "vines" && MovementSystem.currentRoom == "vinesroom")
        {
            if (Inventory.ContainsKey("dagger"))
            {
                VinesCut = true;
                PropertyDamage.causedamage("Destroyed cabling in network room", 2000);
                scrolltext("You cut the vines on the door");
            }
            else
                scrolltext("You need something sharp to cut these vines");
        }
        else
            scrolltext("You can't do that right now");
    }
    public static void do_damage()
    {
        if (secretsenabled)
        {
            PropertyDamage.causedamage("Did a scary test thing that cost $200", 200);
            scrolltext("You did a test, you gained 200 EXP!");
        }
        else
            scrolltext("You can't do that right now");
    }
    public static void show_bill()
    {
        if (secretsenabled)
            PropertyDamage.writebill();
        else
            scrolltext("You can't do that right now");
    }
    public static void go_to()
    {
        if (secretsenabled)
        {
            if (input.Length > 1 && Rooms.ContainsKey(input[1]))
            {
                MovementSystem.currentRoom = input[1];
                if (Rooms.ContainsKey(input[1]))
                {
                    scrolltext($"You are now in: {MovementSystem.currentRoom}");
                    actionscompleted = 0;
                }
            }
            else
                scrolltext("This room does not exist");
        }
        else
            scrolltext("You can't do that right now");
    }
    public static void exit()
    {
        condition = false;
    }
    public static void enabledebug()
    {
        if (!secretsenabled) { secretsenabled = true; scrolltext("enabled"); }
        else { secretsenabled = false; scrolltext("disabled"); }
    }
    public static void attack()
    {
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
            else scrolltext("How did you get here without a knife?");
        }
        else if (MovementSystem.currentRoom == "eyesroom" && !EyesSmashed)
        {
            if (Inventory.ContainsKey("hammer"))
            {
                EyesSmashed = true;

                scrolltext("you begin to attack the strange eyes with your hammer \nas you bring it down upon the eyes it meets with more of those strange monoliths, smashing them apart \nyou keep smashing until all the eyes are gone, and the moniliths they were on lie in pieces");
                PropertyDamage.causedamage("destroyed 20 computers and several monitors in another classroom", 30000);
                PropertyDamage.causedamage("seriously dude what the fuck, these cleaners don't pay for themselves", 200);
            }
            else scrolltext("how did you get here without a hammer?");
        }
        else scrolltext("You can't do that right now");
    }
    public static void smash()
    {
        if (MovementSystem.currentRoom == "smashingroom" && !LurkerMoved)
        {
            if (Inventory.ContainsKey("hammer"))
            {
                LurkerMoved = true;

                scrolltext("With a heave, you lift up the warhammer and bring it down upon one of the strange obelisks, \nit smashes into pieces that scatter across the table \nyou smash another, and then another, you can hear the lurker, startled, begin to make its way to the main door\n");
                scrolltext("Tt's time to get moving");

                PropertyDamage.causedamage("Destroyed two PCs and a monitor in D201", 5300);
                PropertyDamage.causedamage("more work for the cleaners, overtime", 100);
            }
            else scrolltext("You tried to smash one of the obelisks, but you just hurt your hand instead, ouch");
        }
        else scrolltext("You can't do that right now");
    }
    public static void loot()
    {
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
                        takeitem("tablet");
                        takeitem("coint");
                        scrolltext($"From the corpse you loot some sort of <y>tablet<y>, and an array of <y>coins<y>.");
                        Thread.Sleep(500);
                        scrolltext("you hear a loud roar from the side room you cut your way through earlier, and loud angry footsteps \nthe beast is coming, you need to find a way out of the room NOW");
                    }
                }
                break;
            case "kniferoom":
                if (input.Length > 1 && input[1] == "dagger")
                {
                    if (Inventory.ContainsKey("dagger"))
                    {
                        scrolltext("You already have the <y>dagger<y>.");
                    }
                    else
                    {
                        takeitem("dagger");
                        scrolltext($"You take a <y>dagger<y> from its position on the bench");
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
                        takeitem("book");
                        scrolltext($"You take the <y>book<y> from the table");
                    }
                }
                break;
            case "renovatedroom":
                if (input.Length > 1 && (input[1] == "hammer" || input[1] == "warhammer"))
                {
                    if (Inventory.ContainsKey("hammer"))
                    {
                        scrolltext("You already have the <y>hammer<y>.");
                    }
                    else
                    {
                        takeitem("hammer");
                        scrolltext($"You take the <y>hammer<y> from its place on the ground, it is cumbersome but comforting");
                    }
                }

                break;
            case "keyroom":
                if (input.Length > 1 && input[1] == "key")
                {
                    if (Inventory.ContainsKey("key"))
                    {
                        scrolltext("You already have the <y>key<y>.");
                    }
                    else
                    {
                        takeitem("key");
                        scrolltext($"You take the <y>key<y>");
                    }
                }
                break;
            default:
                scrolltext("there is nothing to loot here");
                break;
        }
    }
    private static void takeitem(string item)
    {
        JsonElement itemJSON = (JsonElement)Items[item];
        string realName = itemJSON.GetProperty("real_name").GetString();
        int cost = itemJSON.GetProperty("cost").GetInt32();

        Inventory[item] = Items[item];
        PropertyDamage.causedamage("Stole " + itemJSON.GetProperty("real_name").GetString(), cost);
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
        ReadLine();
        Thread.Sleep(60000);
        Clear();

        condition = true;
    }
    public static void Main()
    {
        // interprets the json as a list of , so we can have a list of items in there for simplicties sake.to get values, needs to be deserialised later
        string items_import = File.ReadAllText("items.json");
        string rooms_import = File.ReadAllText("rooms.json");
        Items = JsonSerializer.Deserialize<Dictionary<string, object>>(items_import) ?? throw new FileNotFoundException("items.json could not be found");
        Rooms = JsonSerializer.Deserialize<Dictionary<string, object>>(rooms_import) ?? throw new FileNotFoundException("rooms.json could not be found");
        scrolltext("You find yourself dazed and confused in a room that is completely pitch black.\nAs you struggle to your feet, your hands meet cold, unforgiving surfaces.\nPanic sets in as you wave a hand before your face and see nothing. Have you gone blind, or have you awoken within some forgotten catacomb?", 50);
        while (condition == true)
        {
            WriteLine("===============================================");
            currentroomjson = (JsonElement)Rooms[MovementSystem.currentRoom];
            int room_actions = currentroomjson.GetProperty("actions").GetInt32();
            if (room_actions > 0)
            {
                if (actionscompleted > room_actions)
                    condition = false;
                {
                    if (actionscompleted > room_actions / 2 && actionscompleted < room_actions)
                        scrolltext("You hear something loud approaching");
                    if (actionscompleted >= room_actions - 1)
                        scrolltext("You should move on");
                }
            }
            scrolltext("(Input <g>help<g> for a current list of actions)", 10);
            Write("> ");
            // The "??" is to stop everything from breaking if for some reason the game can't read an input
            string inputString = (ReadLine() ?? "").ToLower();
            input = inputString.Split(' ');
            switch (input[0])
            {
                case "help":
                    help();
                    break;
                case "inventory":
                    inventory();
                    break;
                case "inspect":
                    inspect();
                    break;
                case "stats":
                    stats();
                    break;
                //give command, takes the value from the item dictionary and copies it into inventory
                case "give":
                    give();
                    break;
                case "cut":
                    cut();
                    break;
                // Debug commands
                case "do_damage":
                    do_damage();
                    break;
                case "show_bill":
                    show_bill();
                    break;
                case "goto":
                    go_to();
                    break;
                // ^ End of debug commands
                case "exit":
                    exit();
                    break;
                case "secret":
                    scrolltext("you thought lol");
                    break;
                case "secret2":
                    enabledebug();
                    break;
                case "attack":
                    attack();
                    break;
                case "smash":
                    smash();
                    break;
                //switch for looting items
                case "loot":
                    loot();
                    break;
                case "endgame":
                    if (secretsenabled == true)
                        EndGame();
                    break;
                default:
                    bool movementSucceeded = MovementSystem.move(inputString);
                    if (movementSucceeded)
                    {
                        actionscompleted = 0;
                        scrolltext("You move....");
                    }
                    break;
            }
        }
        scrolltext("game is over");
    }
}
