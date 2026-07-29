using System.Text.Json;
using static System.Console;

namespace AwesomeGame;

public class MovementSystem
{
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
            _ => false
        };
    }
    public static string ChangeRoom(string currentRoomName, string movement)
    {
        try
        {
            string json = File.ReadAllText("rooms.json");
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement root = doc.RootElement;
                if (!root.TryGetProperty(currentRoomName, out JsonElement currentRoom))
                {
                    WriteLine($"Room '{currentRoomName}' not found.");
                    return currentRoomName;
                }
                JsonElement neighbours = currentRoom.GetProperty("neighbours");
                // Check all neighbor rooms
                foreach (JsonElement neighbourElement in neighbours.EnumerateArray())
                {
                    string neighbourKey = neighbourElement.GetString() ?? "";
                    if (!root.TryGetProperty(neighbourKey, out JsonElement neighbourRoom))
                        continue;
                    bool isMatch = false;
                    if (neighbourKey.Equals(movement, StringComparison.OrdinalIgnoreCase))
                    {
                        isMatch = true;
                    }
                    else if (neighbourRoom.TryGetProperty("aliases", out JsonElement aliases))
                    {
                        foreach (JsonElement alias in aliases.EnumerateArray())
                        {
                            if (alias.GetString()?.Equals(movement, StringComparison.OrdinalIgnoreCase) ?? false)
                            {
                                isMatch = true;
                                break;
                            }
                        }
                    }

                    if (isMatch)
                    {
                        if (CanEnterRoom(neighbourRoom, movement, out string failMessage))
                        {
                            return neighbourKey;
                        }
                        else
                        {
                            WriteLine(failMessage);
                            return currentRoomName;
                        }
                    }
                }
                WriteLine("You can't go that way.");
            }
        }
        catch (Exception ex)
        {
            WriteLine($"Error in movement system: {ex.Message}");
        }
        return currentRoomName;
    }
    private static bool CanEnterRoom(JsonElement targetRoom, string inputAlias, out string failMessage)
    {
        failMessage = "You cannot go this way right now.";

        if (!targetRoom.TryGetProperty("entryRules", out JsonElement entryRules))
        {
            return true;
        }

        foreach (JsonProperty ruleProperty in entryRules.EnumerateObject())
        {
            if (ruleProperty.Name.Equals(inputAlias, StringComparison.OrdinalIgnoreCase))
            {
                foreach (JsonElement rule in ruleProperty.Value.EnumerateArray())
                {
                    if (!CheckRule(rule.GetString() ?? ""))
                    {
                        if (targetRoom.TryGetProperty("FailedEntry", out JsonElement failedEntry) &&
                            failedEntry.TryGetProperty(ruleProperty.Name, out JsonElement msg))
                        {
                            failMessage = msg.GetString() ?? failMessage;
                        }
                        return false;
                    }
                }
            }
        }
        return true;
    }
}