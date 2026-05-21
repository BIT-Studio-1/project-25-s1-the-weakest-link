using System.Collections.Generic;
using System.Net.Quic;
using static System.Console;
//for grabbing values from json
using System.Text.Json;
using System.Windows.Markup;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using AwesomeGame;
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
            if (secretsEnabled)
            {
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
            if (input.Length > 1 && Inventory.ContainsKey(input[1]))
            {
                //retrieves values from json
                JsonElement item = (JsonElement)Items[input[1]];
                foreach (JsonProperty property in item.EnumerateObject())
                    WriteLine($"{property.Name}: {property.Value}");
            }
            else if (input.Length > 1 && input[1] == "room")
            {
                
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
        // Debug commands to test property damage system
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
        // ^ End of debug commands for property damage
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