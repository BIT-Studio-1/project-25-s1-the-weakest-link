using System.Collections.Generic;
using System.Net.Quic;
using static System.Console;
//for grabbing values from json
using System.Text.Json;
using System.Windows.Markup;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using AwesomeGame;
using System.Security;
//To use this, make a string and split different lines with | to alter speed do scrolltext('example string', 100), this will slow it
static void scrolltext(string Text, int speed = 50)
{
        foreach (char o in Text)
        {
            // Checks if key has been pressed without blocking the rest of the code from running
            if (Console.KeyAvailable)
            {
                var key = ReadKey().Key;
                if (key == ConsoleKey.Spacebar || key == ConsoleKey.Enter)
                {
                    speed = 1;
                }
            }
            Write(o);
            Thread.Sleep(speed);
        }
        WriteLine("");
}
/*
interprets the json as a list of , so we can have a list of items in there for simplicties sake.
to get values, needs to be deserialised later
*/
string items_import = File.ReadAllText("items.json");
Dictionary<string, object> Items = JsonSerializer.Deserialize<Dictionary<string, object>>(items_import);
string rooms_import = File.ReadAllText("rooms.json");
Dictionary<string, object> Rooms = JsonSerializer.Deserialize<Dictionary<string, object>>(rooms_import);
//makes a new dictionary for inventory, references the 'items' dictionary, google says var is good practice in 
var Inventory = new Dictionary<string, object>();
scrolltext( "Waking up disoriented, you open your eyes.\n" +
            "Everything is dark, in your panic you flail your limbs until you feel something around you.\n" +
            "you are blind.");
scrolltext("input h, or help for a current list of actions", 10);
int actionsCompleted = 0;
bool secretsEnabled = false;
bool condition = true;
Boolean vinescut = false;
//temp variable for current room, will be replaced when josh is finished the movement system
string currentRoomTemp = "startRoom";
while (condition == true)
{
    if (actionsCompleted >5) //Replace value 5 with however many actions are in the room
    {
        Console.WriteLine("You hear something loud approaching");
        Console.WriteLine("You should move on");
    }
    string[] input = ReadLine().ToLower().Split(' ');
    switch (input[0])
    { 
        case "help":
        case "h":
            // please add any commands you add to the program to this help section !!!
            WriteLine("inventory: prints contents of the inventory");
            WriteLine("inspect: inspects item with more detail than originally shown");
            WriteLine("stats: shows your current EXP");
            WriteLine("help: shows a list and description of commands");            
            WriteLine("quit, kill, exit: closes the game");
            //these are dev commands, activated by typing 'secret2'
            if (currentRoomTemp == "vinesroom" && vinescut == false)
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
                JsonElement item = (JsonElement)Items[input[1]];
                foreach (JsonProperty property in item.EnumerateObject())
                    WriteLine($"{property.Name}: {property.Value}");
            }
            else if (input.Length > 1 && input[1] == "room") //this looks for the word 'room' in the player's command and then inspects the room
            {
                JsonElement room = (JsonElement)Rooms[currentRoomTemp];
                string description = null;
                // i will add more rooms with changing conditions to this if statement once we have the conditions sorted
                if ((currentRoomTemp == "startRoom" && Inventory.ContainsKey("book")) || 
                    (currentRoomTemp == "knifeRoom" && Inventory.ContainsKey("dagger")) || 
                    (currentRoomTemp == "vinesRoom" && vinescut == true) ||
                    (currentRoomTemp == "tabletRoom" && Inventory.ContainsKey("tablet")))
                {
                    description = room.GetProperty("description2").GetString();
                }
                else { description = room.GetProperty("description").GetString(); }                    
                WriteLine(description);
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
            else { WriteLine("you can't do that right now");  }
                break;
        case "cut":
            if (input[1] == "vines")
                if (Inventory.ContainsKey("dagger"))
                {
                    vinescut = true;
                    scrolltext("you cut the vines on the door");
                }
                else
                    scrolltext("you need something sharp to cut these vines");
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
                currentRoomTemp = input[1];
                if (Rooms.ContainsKey(input[1]))
                    scrolltext($"you are now in: {currentRoomTemp}");
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
        
        
        
        
        
        default:
            WriteLine("you can't do that right now");
            break;
    }
}