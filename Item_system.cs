using System.Collections.Generic;
using System.Net.Quic;
using static System.Console;
//for grabbing values from json
using System.Text.Json;
using System.Windows.Markup;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
/*
as far as im aware this just interprets the json as a list of objects (i think this is the best way to do it), so we can have a list of items in there for simplicties sake.
i think this does mean that we will have to do some extra stuff later to get the values out of the json
i believe a json is probably the best way to do this as it's very 'modular'
*/
string json = File.ReadAllText("items.json");
Dictionary<string, object> Items = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
//makes a new dictionary for inventory, references the 'items' dictionary, google says var is good practice in 
var Inventory = new Dictionary<string, object>();
WriteLine("write h, or help for a list of commands");
bool secretsEnabled = false;
bool condition = true;
while (condition == true)
{
    //gets the first word of input, possibility for arguments, i.e 'inspect dagger'
    string[] input = ReadLine().ToLower().Split(' ');
    //switch for commands, dont know how we will introduce commands to player right now, maybe a 'help' command?
    
    switch (input[0])
    {
        //lists all commands
        case "help":
        case "h":
            //someone make this better
            WriteLine("inventory: prints contents of the inventory");
            WriteLine("inspect: inspects item with more detail than originally shown");
            WriteLine("help: shows a list and description of commands");
            WriteLine("quit, kill, exit: closes the game");
            if (secretsEnabled)
            {
                WriteLine("give: gives a provided item");
            }

            break;
        case "inventory":

            //i think a var would work with the below but i think one of the teachers dissaproves of var
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
            /*
            checks if 'input' has more than one word, if so it checks against the key of the dictionary
            if the second word is in the dictionary as a key it will print all the values of the key
            otherwise, "you don't have that item"
            do i need to use proper grammar in these comments? like full stops or capitol letters?
            are most of these even necessary?
            */
            if (input.Length > 1 && Inventory.ContainsKey(input[1]))
            {
                //turns the item back into a json to get the values, then prints them. i think?
                JsonElement item = (JsonElement)Items[input[1]];
                foreach (JsonProperty property in item.EnumerateObject())
                    WriteLine($"{property.Name}: {property.Value}");
            }
            
            else
            {
                WriteLine("you don't have that item");
            } 
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
            else { WriteLine("unknown command");  }
                break;
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
            WriteLine("unknown command");
            break;
    }
}