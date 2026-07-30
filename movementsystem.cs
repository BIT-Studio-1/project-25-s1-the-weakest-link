using System.Text.Json;
using static System.Console;

namespace AwesomeGame;

// A lock on an exit, can require any number of factors and be flipped manually at runtime
public class Lock
{
    public List<string> Requires { get; set; } = new();
    public string MatchType { get; set; } = "all"; // "all" or "any"
    public bool ForceLocked { get; set; } = false;
    public string? FailMessage { get; set; }

    // checks ForceLocked first, then falls back to the Requires list
    public bool IsLocked()
    {
        if (ForceLocked) return true;
        if (Requires.Count == 0) return false;
        return MatchType.Equals("any", StringComparison.OrdinalIgnoreCase)
            ? !Requires.Any(MovementSystem.CheckRule)
            : !Requires.All(MovementSystem.CheckRule);
    }
}

// One exit from a room - where it leads, what words open it, and any lock guarding it
public class Exit
{
    public string Direction { get; set; } = "";
    public List<string> Aliases { get; set; } = new();
    public string Target { get; set; } = "";
    public Lock? Lock { get; set; }

    public bool MatchesInput(string input) =>
        Direction.Equals(input, StringComparison.OrdinalIgnoreCase) ||
        Aliases.Any(a => a.Equals(input, StringComparison.OrdinalIgnoreCase));
}

// A room loaded from rooms.json
public class Room
{
    public List<Exit> Exits { get; set; } = new();
    public string? Description { get; set; }
    public string? Description2 { get; set; }
    public List<string> Features { get; set; } = new();
    public int Actions { get; set; }
}

public static class MovementSystem
{
    public static string currentRoom = "startroom";
    private static Dictionary<string, Room>? _rooms;
    // loads rooms.json once and caches it so that it doesnt re-read every loop
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

    // resolves a named factor to its current true/false state, add new ones here
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
            // "LeverPulled" => Game.LeverPulled,
            // "KnowsSecretWord" => Game.KnowsSecretWord,
            _ => false
        };
    }

    // forces an exit locked regardless of its requires list, optionally overriding the fail message
    public static void LockExit(string roomName, string direction, string? failMessage = null)
    {
        Exit? exit = FindExit(roomName, direction);
        if (exit == null) return;
        exit.Lock ??= new Lock();
        exit.Lock.ForceLocked = true;
        if (failMessage != null) exit.Lock.FailMessage = failMessage;
    }

    // clears a manual lock set by LockExit
    public static void UnlockExit(string roomName, string direction)
    {
        Exit? exit = FindExit(roomName, direction);
        if (exit?.Lock != null) exit.Lock.ForceLocked = false;
    }

    private static Exit? FindExit(string roomName, string direction)
    {
        if (!Rooms.TryGetValue(roomName, out Room? room)) return null;
        return room.Exits.FirstOrDefault(e => e.MatchesInput(direction));
    }

    // scripted lock/unlock events tied to specific rooms, runs whenever the player enters one
    private static void HandleRoomEnterEvents(string roomName)
    {
        if (roomName == "vinesroom")
        {
            // cleaner locks small room
            LockExit("hallway1", "open room");
            LockExit("tabletroom", "side entrance");
        }
        else if (roomName == "hallway2")
        {
            // locks tabletroom after left
            UnlockExit("hallway1", "open room");
            UnlockExit("tabletroom", "side entrance");
            LockExit("hallway1", "locked door", "You hear something growling inside.");
        }
    }

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
            string destination = exit.Target;
            HandleRoomEnterEvents(destination);
            return destination;
        }
        catch (Exception ex)
        {
            WriteLine($"Error in movement system: {ex.Message}");
            return currentRoomName;
        }
    }
}