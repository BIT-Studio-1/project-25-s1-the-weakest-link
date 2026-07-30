using System.Text.Json;
using static System.Console;

namespace AwesomeGame;

/// <summary>
/// A lock on an exit. Can require any number of factors (rule names),
/// matched either as "all" (AND) or "any" (OR). ForceLocked lets code
/// flip an exit locked/unlocked at runtime (a lever, a spell, etc.)
/// independent of whatever the requires list says.
/// </summary>
public class Lock
{
    public List<string> Requires { get; set; } = new();
    public string MatchType { get; set; } = "all"; // "all" or "any"
    public bool ForceLocked { get; set; } = false;
    public string? FailMessage { get; set; }

    // True if this lock currently blocks passage, checking ForceLocked first
    // and then falling back to the Requires list (per MatchType).
    public bool IsLocked()
    {
        if (ForceLocked)
        {
            return true;
        }
        if (Requires.Count == 0)
        {
            return false;
        }

        return MatchType.Equals("any", StringComparison.OrdinalIgnoreCase)
            ? !Requires.Any(MovementSystem.CheckRule)
            : !Requires.All(MovementSystem.CheckRule);
    }
}

/// <summary>
/// A single exit from a room: where it leads, what words open it,
/// and (if any) lock guards passing through.
/// </summary>
public class Exit
{
    public string Direction { get; set; } = "";
    public List<string> Aliases { get; set; } = new();
    public string Target { get; set; } = "";
    public Lock? Lock { get; set; }

    // True if the given input word matches this exit's direction or any of its aliases.
    public bool MatchesInput(string input) =>
        Direction.Equals(input, StringComparison.OrdinalIgnoreCase) ||
        Aliases.Any(a => a.Equals(input, StringComparison.OrdinalIgnoreCase));
}

// A room loaded from rooms.json: its flavour text, any collectible
// features, and the list of exits leading out of it.
public class Room
{
    public List<Exit> Exits { get; set; } = new();
    public string? Description { get; set; }
    public string? Description2 { get; set; }
    public List<string> Features { get; set; } = new();
    public int Actions { get; set; }
}

// Handles loading rooms.json and moving the player between rooms,
// including checking and managing exit locks.
public static class MovementSystem
{
    public static string currentRoom = "startroom";

    private static Dictionary<string, Room>? _rooms;

    // Lazily loads and caches rooms.json as a name -> Room lookup,
    // so the file is only read from disk once.
    private static Dictionary<string, Room> Rooms
    {
        get
        {
            if (_rooms == null)
            {
                string json = File.ReadAllText("rooms.json");
                _rooms = JsonSerializer.Deserialize<Dictionary<string, Room>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new Dictionary<string, Room>();
            }
            return _rooms;
        }
    }

    // Resolves a named factor (used by locks) to its current true/false
    // state in the game. Add new factors here as the game grows.
    public static bool CheckRule(string ruleName)
    {
        return ruleName switch
        {
            "HasTablet" => Game.Inventory.ContainsKey("tablet"),
            "VinesCut" => Game.VinesCut,
            "SacDestroyed" => Game.SacDestroyed,
            "LurkerMoved" => Game.LurkerMoved,
            "LurkerNotMoved" => !Game.LurkerMoved,
            "EyesSmashed" => Game.EyesSmashed,
            "HasKey" => Game.HasKey,
            // Add new factors here as you need them:
            // "LeverPulled" => Game.LeverPulled,
            // "KnowsSecretWord" => Game.KnowsSecretWord,
            _ => false
        };
    }

    /// Manually locks an exit at runtime.
    public static void LockExit(string roomName, string direction)
    {
        Exit? exit = FindExit(roomName, direction);
        if (exit != null)
        {
            exit.Lock ??= new Lock();
            exit.Lock.ForceLocked = true;
        }
    }

    /// Manually unlocks an exit at runtime.
    public static void UnlockExit(string roomName, string direction)
    {
        Exit? exit = FindExit(roomName, direction);
        if (exit?.Lock != null)
        {
            exit.Lock.ForceLocked = false;
        }
    }

    // Looks up a specific exit by room name and direction/alias,
    // used internally by LockExit and UnlockExit.
    private static Exit? FindExit(string roomName, string direction)
    {
        if (!Rooms.TryGetValue(roomName, out Room? room))
        {
            return null;
        }
        return room.Exits.FirstOrDefault(e => e.MatchesInput(direction));
    }

    // Attempts to move the player from currentRoomName whichever
    // exit that matches "movement". Returns the new room name on success, 
    // or the original room name if the move fails.
    public static string ChangeRoom(string currentRoomName, string movement)
    {
        try
        {
            if (!Rooms.TryGetValue(currentRoomName, out Room? room))
            {
                WriteLine($"Room '{currentRoomName}' not found.");
                return currentRoomName;
            }
            Exit? exit = room.Exits.FirstOrDefault(e => e.MatchesInput(movement));
            if (exit == null)
            {
                WriteLine("You can't go that way.");
                return currentRoomName;
            }
            if (exit.Lock != null && exit.Lock.IsLocked())
            {
                WriteLine(exit.Lock.FailMessage ?? "You cannot go this way right now.");
                return currentRoomName;
            }
            return exit.Target;
        }
        catch (Exception ex)
        {
            WriteLine($"Error in movement system: {ex.Message}");
            return currentRoomName;
        }
    }
}