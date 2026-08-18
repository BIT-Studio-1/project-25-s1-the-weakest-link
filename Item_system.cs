using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using AwesomeGame;
using static System.Console;
namespace AwesomeGame;
internal static class Game
{
    public static Dictionary<string, object> Inventory = new Dictionary<string, object>();
    // Flags to show that an action has been completed
    public static bool VinesCut = false, SacDestroyed = false, LurkerMoved = false, EyesSmashed = false;
    public static Dictionary<string, object>? Items;
    public static Dictionary<string, object>? Rooms;
    public static int actionscompleted = 0;
    public static bool condition = true, secretsenabled = false;
    public static JsonElement currentroomjson;
    static string[]? input;
    public static void scrollText(string Text, int speed = 10)
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
        void Writeportion(string portion, ConsoleColor? textColour = null)
        {
            for (int i = 0; i < portion.Length; i++)
            {
                if (!skipped && KeyAvailable)
                {
                    var key = ReadKey(true).Key;
                    if (key == ConsoleKey.Spacebar || key == ConsoleKey.Enter)
                        skipped = true;
                }
                if (textColour.HasValue)
                    ForegroundColor = textColour.Value;
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
            string tag = match.Groups[1].Value;
            string content = match.Groups[2].Value;
            if (colour.TryGetValue(tag, out var color))
                Writeportion(content.ToUpper(), color);
            else
                Writeportion(content);
            lastIndex = match.Index + match.Length;
        }
        if (lastIndex < Text.Length)
            Writeportion(Text.Substring(lastIndex));
        WriteLine();
    }
    public static void help()
    {
        // please add any commands you add to the program to this help section !!!
        scrollText("<b>inspect<b> (<y>item name<y>): Describes item to you.\n" +
                   "<b>inspect room<b>: Describes the room to you in detail.\n" +
                   "<b>stats<b>: Shows your current EXP\n" +
                   "<b>help<b>: Shows a list and description of commands\n" +
                   "<b>inventory<b>: Prints contents of the inventory\n" +
                   "<g>door name<g>: Enter the name of a door to move rooms");
        if (currentroomjson.TryGetProperty("features", out _))
            scrollText("<b>loot<b> (<y>item<y>/<y>object<y>): Takes an item from the room");
        //these are dev commands, activated by typing 'secret2'
        if (MovementSystem.currentRoom == "vinesroom" && VinesCut == false)
            scrollText("<b>cut vines<b>: Cuts the vines covering the door");
        if (MovementSystem.currentRoom == "smashingroom" && LurkerMoved == false)
        {
            scrollText("<b>smash<b>: Smashes the obelisks");
        }
        if (secretsenabled)
        {
            scrollText("<b>goto<b>: sends you to a room");
            scrollText("<b>give<b>: gives a provided item");
            scrollText("<b>doDamage<b>: command to test property damage system");
            scrollText("<b>showBill<b>: command to show the current property damage");
        }
        scrollText("<b>exit<b>: Closes the game");
    }
    public static void inventory()
    {
        if (Inventory.Count > 0)
        {
            scrollText("You have:");
            foreach (KeyValuePair<string, object> Inv in Inventory)
                scrollText($"<y>{Inv.Key}<y>");
        }
        else
            scrollText("You don't have any items.");
    }
    public static void inspect()
    {
        void inspectroom()
        {
            JsonElement room = (JsonElement)Rooms[MovementSystem.currentRoom];
            string description;
            if (
            (MovementSystem.currentRoom == "startroom" && Inventory.ContainsKey("book")) ||
            (MovementSystem.currentRoom == "vinesroom" && VinesCut) ||
            (MovementSystem.currentRoom == "hallway2" && LurkerMoved) ||
            (MovementSystem.currentRoom == "tabletroom" && Inventory.ContainsKey("tablet")) ||
            (MovementSystem.currentRoom == "smashingroom" && LurkerMoved) ||
            (MovementSystem.currentRoom == "spidersroom" && SacDestroyed) ||
            (MovementSystem.currentRoom == "eyesroom" && EyesSmashed) ||
            (MovementSystem.currentRoom == "kniferoom" && Inventory.ContainsKey("dagger")) ||
            (MovementSystem.currentRoom == "keyroom" && Inventory.ContainsKey("key")) ||
            (MovementSystem.currentRoom == "renovatedroom" && Inventory.ContainsKey("hammer")) ||
            (MovementSystem.currentRoom == "bathroom" && Inventory.ContainsKey("elixir")) ||
            (MovementSystem.currentRoom == "office" && Inventory.ContainsKey("sheet")) ||
            (MovementSystem.currentRoom == "cupboard" && Inventory.ContainsKey("alienweaponry"))
            )
                description = room.GetProperty("description2").GetString() ?? throw new MissingFieldException($"rooms.json has no description2 for {MovementSystem.currentRoom}");
            else
                description = room.GetProperty("description").GetString() ?? throw new MissingFieldException($"rooms.json has no description for {MovementSystem.currentRoom}");
            scrollText(description, 5);
        }
        if (input != null && input.Length > 1)
        {
            if (Inventory.ContainsKey(input[1]))
            {
                var item = (JsonElement)Items[input[1]];
                string itemDescription;
                itemDescription = item.GetProperty("description").GetString() ?? throw new MissingFieldException($"items.json has no description for the requested item");
                scrollText(itemDescription);
            }
            else if (input[1] == "room")
            {
                inspectroom();
            }
            else
            {
                scrollText("You don't have that item.");
            }
        }
        else
        {
            inspectroom();
        }
        actionscompleted++;
    }
    public static void stats()
    {
        scrollText($"You have {propertyDamage.totalcost} EXP.");
    }
    public static void give()
    {
        if (secretsenabled)
        {
            if (input.Length > 1 && Items.ContainsKey(input[1]))
            {
                takeItem(input[1]);
                scrollText($"You now have {input[1]}");
            }
            else
            {
                scrollText("This item does not exist.");
            }
        }
        else { scrollText("You can't do that right now."); }
    }
    public static void cut()
    {
        void cutVines()
        {
            if (Inventory.ContainsKey("dagger") && VinesCut == false)
            {
                VinesCut = true;
                propertyDamage.causedDamage("Destroyed cabling in network room", 2000);
                propertyDamage.printDamage();
                scrollText("You slash through the vines covering the door. You should be able to get through now.", 35);
            }
            else
                scrollText("You try to cut the vines, but it seems you need something sharp.", 35);
        }
        if (input.Length > 1)
        {
            if (input[1] == "vines" && MovementSystem.currentRoom == "vinesroom")
                cutVines();
        }
        else if (input.Length == 1 && MovementSystem.currentRoom == "vinesroom")
            cutVines();
    }
    public static void doDamage()
    {
        if (secretsenabled)
        {
            propertyDamage.causedDamage("Did a scary test thing that cost $200", 200);
            propertyDamage.printDamage();
            scrollText("You did a test, you gained 200 EXP!");
        }
    }
    public static void showBill()
    {
        if (secretsenabled)
            propertyDamage.writeBill();
    }
    public static void goTo()
    {
        if (secretsenabled)
        {
            if (input.Length > 1 && Rooms.ContainsKey(input[1]))
            {
                MovementSystem.currentRoom = input[1];
                if (Rooms.ContainsKey(input[1]))
                {
                    scrollText($"You are now in: {MovementSystem.currentRoom}");
                    actionscompleted = 0;
                }
            }
            else
                scrollText("This room does not exist");
        }
    }
    public static void exit()
    {
        condition = false;
    }
    public static void enableDebug()
    {
        if (!secretsenabled) { secretsenabled = true; scrollText("Debug commands enabled"); }
        else { secretsenabled = false; scrollText("Debug commands disabled"); }
    }
    public static void attack()
    {
        // Spider room, Abby's responsibility
        if (MovementSystem.currentRoom == "spidersroom" && !SacDestroyed)
        {
            if (Inventory.ContainsKey("dagger"))
            {
                SacDestroyed = true;

                scrollText("You stab at the sac with you dagger, slashing your way through...");
                Thread.Sleep(500);

                scrollText("The sac bursts open, releasing hundreds, possibly thousands of eggs! You can barely walk without crushing dozens of eggs.", 35);

                propertyDamage.causedDamage("Shredded bean bag", 60);
                propertyDamage.causedDamage("Cleanup of bean bag beans in common room", 50);
                propertyDamage.printDamage();

            }
            else scrollText("How did you get here without a knife?");
        }
        else if (MovementSystem.currentRoom == "eyesroom" && !EyesSmashed)
        {
            if (Inventory.ContainsKey("hammer"))
            {
                EyesSmashed = true;

                scrollText("You begin to attack the strange eyes with your hammer.\nAs you bring it down upon the eyes, it meets with more strange monoliths.\nYou smash until all the eyes are gone, and the monoliths they were on lie in pieces.", 35);
                propertyDamage.causedDamage("Destroyed 20 computers and several monitors in another classroom", 30000);
                propertyDamage.causedDamage("Seriously dude what the fuck, these cleaners don't pay for themselves", 200);
                propertyDamage.printDamage();
            }
            else scrollText("how did you get here without a hammer?");
        }
    }
    public static void smash()
    {
        if (MovementSystem.currentRoom == "smashingroom" && !LurkerMoved)
        {
            if (Inventory.ContainsKey("hammer"))
            {
                LurkerMoved = true;

                scrollText("With a heave, you lift up the warhammer and bring it down upon one of the strange obelisk.\nIt smashes into pieces that scatter across the table.\nYou smash another, and then another, you can hear the lurker, startled, begin to make its way to the main door.\n", 35);
                scrollText("It's time to get moving.");

                propertyDamage.causedDamage("Destroyed two PCs and a monitor in D201", 5300);
                propertyDamage.causedDamage("More work for the cleaners, overtime", 100);
                propertyDamage.printDamage();
            }
            else scrollText("You tried to smash one of the obelisks, but you just hurt your hand instead. Ouch!", 35);
        }
    }
    public static void loot()
    {
        Room? room = MovementSystem.GetCurrentRoom();
        if (input.Length > 1 && room.Feature.Equals(input[1]))
        {
            if (room?.Feature != null && Inventory.ContainsKey(room.Feature))
                return;
            if (room.Feature != null && Items.TryGetValue(room.Feature, out object? raw))
            {
                JsonElement item = (JsonElement)raw;
                if (item.TryGetProperty("flavourtext", out JsonElement Text))
                {
                    scrollText(Text.GetString());
                    takeItem(room.Feature);
                }
            }
            // exception for tabletroom
            if (MovementSystem.currentRoom == "tabletroom")
                if (input.Length > 1 && input[1] == "corpse")
                {
                    if (Inventory.ContainsKey("tablet"))
                    {
                        scrollText("the corpse, now without its items still burns with heat, it must've been a man of great vitality.");
                    }
                    else
                    {
                        scrollText($"From the corpse you loot some sort of <y>tablet<y>.");
                        takeItem("tablet");
                        Thread.Sleep(500);
                        scrollText("You hear a loud roar and enraged footsteps from the side room you cut your way through earlier.\nThe beast is coming, you need to find a way out of the room NOW.");
                    }
                }
            //debug commands
            //WriteLine($"Feature: {room?.Feature}");
            //writeLine($"Room: {MovementSystem.currentRoom}");
        }
        else scrollText("Type 'Help' for help with commands");
    }
    public static void Cows()
    {
        scrollText("the cows are here!");
        propertyDamage.causedDamage("Extermination & removal  of cows", 1985151522);
        propertyDamage.printDamage();
    }
    private static void takeItem(string item)
    {
        JsonElement itemJSON = (JsonElement)Items[item];
        string realName = itemJSON.GetProperty("real_name").GetString();
        int cost = itemJSON.GetProperty("cost").GetInt32();

        Inventory[item] = Items[item];
        propertyDamage.causedDamage("Stole " + realName, cost);
        propertyDamage.printDamage();
    }
    public static void HandleMovement(string inputstring)
    {
        string newRoom = MovementSystem.ChangeRoom(MovementSystem.currentRoom, inputstring);
        if (newRoom != MovementSystem.currentRoom)
        {
            MovementSystem.currentRoom = newRoom;
            actionscompleted = 0;
            input = null;
            inspect();
        }
    }
    public static void EndGame()
    {
        // Called from movementsystem.cs when entering "glass door" from hallway2
        scrollText("You carefully unlock the glass door and hesitantly push it open. Could this finally be the escape from this prison you \nfind yourself in?", 35);
        scrollText("You walk inside, hanging close to the wall so as to maintain your sense of direction. Your hand connects with a slender metal bar, as a sudden drop appears before you.\r\n", 35);
        scrollText("You reach a foot down the cliff, clinging tight to the bar. Your body is bound tight with fear, your foot slowly \ndescending down the edge. Suddenly, your foot finds ground, as you realise a stairwell has appeared before you.\r\n", 35);
        scrollText("You slowly tread down the stairs, foot by foot, step by step. As you descend, you realise with a shock that your vision is returning!\nYour senses are overwhelmed by a blinding light, radiating from a closed door.\r\n", 35);
        scrollText("Psyching yourself for danger, you open the door...\r\n", 75);
        scrollText("\"Hey, the building closed to students four hours ago, it's cleaners only now.\"\r\n", 35);
        scrollText("You are on the ground floor of the Otago Polytechnic's D block, and you are staring face to face with the janitor.\r\n", 40);
        scrollText("\"It's 4am, go home.\"\r\n", 30);
        Thread.Sleep(1000);
        scrollText("THE NEXT DAY...\r\n", 75);
        scrollText("You wake up in your home at 2pm, still exhausted from last night's confusion. You yawn, then get out of bed.", 30);
        scrollText("You go to check your mailbox and see a letter addressed to you with the polytech's logo. You open it up, and read the \ncontents...\r\n", 30);
        propertyDamage.writeBill();

        WriteLine();
        WriteLine();

        scrollText("Press any key to reset.");
        ReadKey();
        Clear();

        Inventory.Clear(); // this codeblock clears everything, the inventory, the receipt, and resets all bools set at the beginning to their default values
        propertyDamage.damagereasons.Clear(); propertyDamage.damageamount.Clear(); propertyDamage.totalcost = 0;
        VinesCut = false; SacDestroyed = false; LurkerMoved = false; EyesSmashed = false; secretsenabled = false; actionscompleted = 0;
        MovementSystem.currentRoom = "startroom";
    }
    public static void Main()
    {
        // interprets the json as a list of , so we can have a list of items in there for simplicties sake.to get values, needs to be deserialised later
        string items_import = File.ReadAllText("items.json");
        string rooms_import = File.ReadAllText("rooms.json");
        Items = JsonSerializer.Deserialize<Dictionary<string, object>>(items_import) ?? throw new FileNotFoundException("items.json could not be found");
        Rooms = JsonSerializer.Deserialize<Dictionary<string, object>>(rooms_import) ?? throw new FileNotFoundException("rooms.json could not be found");
        scrollText("You find yourself dazed and confused in a room that is completely pitch black.\nAs you struggle to your feet, your hands meet cold, unforgiving surfaces.\nPanic sets in as you wave a hand before your face and see nothing. Have you gone blind?", 50);
        scrollText("(Input <b>help<b> for a current list of actions)", 10);
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
                        scrollText("You hear something loud approaching.");
                    if (actionscompleted >= room_actions - 1)
                        scrollText("You should move on.");
                }
            }
            Write("> ");
            // The "??" is to stop everything from breaking if for some reason the game can't read an input
            string inputString = (ReadLine() ?? "").ToLower();
            input = inputString.Split(' ');
            // Special case: the glass door in hallway2 is the win condition, not a normal room transition
            if (MovementSystem.currentRoom == "hallway2" && inputString == "glass door")
            {
                if (Inventory.ContainsKey("key"))
                {
                    EndGame();
                }
                else
                {
                    scrollText("This door is locked by an ancient mechanism. You will need a key.");
                }
                continue;
            }
            Action command = input[0] switch
            {
                "help" or "h" => help,
                "inventory" => inventory,
                "inspect" or "i" => inspect,
                "stats" or "s" => stats,
                "cut" => cut,
                "secret" => () => scrollText("you thought lol"),
                "secret2" => enableDebug,
                "give" => give,
                "doDamage" => doDamage,
                "showBill" => showBill,
                "goto" => goTo,
                "exit" or "quit" or "q" => exit,
                "attack" or "a" => attack,
                "smash" => smash,
                "take" or "loot" => loot,
                "clear" or "c" => Clear,
                "room" => () => WriteLine($"You are in: {MovementSystem.currentRoom}"),
                "summon" or "summoncow" or "summoncows" => Cows,
                "endgame" when secretsenabled => EndGame,
                _ => () => HandleMovement(inputString)
            };
            command();
        }
        scrollText("<r>GAME OVER<r>");
        ReadKey();
    }
}