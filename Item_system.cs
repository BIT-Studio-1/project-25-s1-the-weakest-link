using System.Collections.Generic;
using System.Net.Quic;
using static System.Console;
//for grabbing values from json
using System.Text.Json;
/*
as far as im aware this just interprets the json as a list of objects (i think this is the best way to do it), so we can have a list of items in there for simplicties sake.
i think this does mean that we will have to do some extra stuff later to get the values out of the json
i believe a json is probably the best way to do this as it's very 'modular'
*/
string json = File.ReadAllText("items.json");
Dictionary<string, object> Items = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
while (true)
{
    //gets the first word of input, possibility for arguments, i.e 'inspect dagger'
    string[] input = ReadLine().Split(' ');
    //switch for commands, dont know how we will introduce commands to player right now, maybe a 'help' command?
    switch (input[0])
    {
        case "inventory":
            WriteLine("you have:");
            //i think a var would work with the below but i think one of the teachers dissaproves of var
            foreach (KeyValuePair<string, object> item in Items)
            {
                WriteLine(item.Key);
            }
            break;
        case "inspect":
        /*
        checks if 'input' has more than one word, if so it checks agains the key of the dictionary
        if the second word is in the dictionary as a key it will print all the values of the key
        otherwise, "you dont have that item"
        do i need to use proper grammer in these comments? like full stops or capitol letters?
        are most of these even necessary?
        */
            if (input.Length > 1 && Items.ContainsKey(input[1]))
            {
                //turns the item back into a json to get the values, then prints them. i think?
                JsonElement item = (JsonElement)Items[input[1]];
                foreach (JsonProperty property in item.EnumerateObject())
                    WriteLine($"{property.Name}: {property.Value}");
            }
            else
            {
                WriteLine("you dont have that item");
            }
            break;
        default:
            WriteLine("unknown command");
            break;
    }
}
