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

    private static bool CheckRule(string ruleName)
    {
        return ruleName switch
        {
            "HasTablet" => Game.Inventory.ContainsKey("tablet"),
            "VinesCut" => Game.VinesCut,
            "SacDestroyed" => Game.SacDestroyed,
            "LurkerMoved" => Game.LurkerMoved,
            "EyesSmashed" => Game.EyesSmashed,
            "HasKey" => Game.HasKey,
            "BeastGone" => !Game.SacDestroyed,
            "safeToReturn" => true,  // Always allow returning to starting room
            _ => false
        };
    }

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

                    foreach (JsonElement alias in aliases.EnumerateArray())
                    {
                        string aliasStr = alias.GetString() ?? "";
                        if (aliasStr.Equals(movement, StringComparison.OrdinalIgnoreCase))
                        {
                            // Found matching alias
                            if (neighbourRoom.TryGetProperty("entryRules", out JsonElement entryRules))
                            {
                                if (entryRules.TryGetProperty(aliasStr, out JsonElement rulesForAlias))
                                {
                                    // Check all rules
                                    bool allRulesPassed = true;
                                    foreach (JsonElement rule in rulesForAlias.EnumerateArray())
                                    {
                                        string ruleName = rule.GetString() ?? "";
                                        if (!CheckRule(ruleName))
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
                                        // Get the failure message
                                        if (neighbourRoom.TryGetProperty("FailedEntry", out JsonElement failedEntry))
                                        {
                                            if (failedEntry.TryGetProperty(aliasStr, out JsonElement failMessage))
                                            {
                                                WriteLine(failMessage.GetString());
                                            }
                                        }
                                        return currentRoom;
                                    }
                                }
                                else
                                {
                                    return neighbour;
                                }
                            }
                            else
                            {
                                return neighbour;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            WriteLine($"Error in movement system: {ex.Message}");
        }
        return currentRoom;
    }
}