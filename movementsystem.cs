namespace AwesomeGame;
//this is a comment
// Class containing all code for moving between rooms, 
public class MovementSystem
{
    // Assigns string "startroom" to the currentRoom variable
    public static string currentRoom = "startroom";
    private static bool cleanerpresent = false;
    string json = File.ReadAllText("rooms.json");
    public static string ChangeRoom(string currentRoom, string movement)
    {
        try
        {
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
                        if ((alias.GetString() ?? "").Equals(movement, StringComparison.OrdinalIgnoreCase))
                            return neighbour;
                    }//if the input matches neighbour aliases return room change
                }
            }
        }
        catch { }
        return currentRoom;
    }
}