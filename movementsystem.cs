using System.Data;
using System.Text.Json;
using static System.Console;
namespace AwesomeGame;
//this is a comment
// Class containing all code for moving between rooms, 
public class MovementSystem
{
    // Assigns string "startroom" to the currentRoom variable
    public static string currentRoom = "startroom";
    public static string ChangeRoom(string currentRoom, string movement)
    {
        try
        {
            string json = File.ReadAllText("rooms.json");
            using (JsonDocument temp = JsonDocument.Parse(json))
            {
                JsonElement room = temp.RootElement.GetProperty(currentRoom);
                JsonElement neighbours = room.GetProperty("neighbours");
                foreach (JsonElement neighbourName in neighbours.EnumerateArray())
                {
                    string neighbour = neighbourName.GetString() ?? throw new MissingFieldException("Null neighbour in rooms.json");
                    JsonElement neighbourRoom = temp.RootElement.GetProperty(neighbour);
                    JsonElement aliases = neighbourRoom.GetProperty("aliases");
                    JsonElement Rules = neighbourRoom.GetProperty("rule");
                    JsonElement Failedentries = neighbourRoom.GetProperty("FailedEntry");
                    foreach (JsonElement alias in aliases.EnumerateArray())
                    {
                        if ((alias.GetString() ?? "").Equals(movement, StringComparison.OrdinalIgnoreCase))
                        {
                            bool allRulesPassed = true;
                            foreach (JsonElement rule in Rules.EnumerateArray())
                            {
                                if (!rule.GetBoolean())
                                {
                                    allRulesPassed = false;
                                    break;
                                }
                            }
                            if (allRulesPassed)
                            {
                                return neighbour;
                            }
                            else
                            {
                                WriteLine(Failedentries.GetString());
                                return currentRoom;
                            }
                        }//if the input matches neighbour aliases return room change
                    }
                }
            }
        }
        catch { }
        return currentRoom;
    }
}