using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;


string rooms_import = File.ReadAllText("rooms.json");
Dictionary<string, Room> Rooms = JsonSerializer.Deserialize<Dictionary<string, Room>>(rooms_import);

public class Room
{
    List<string> items;
    List<Connection> Connections;
}

public class Connection
{
    string command;
    List<string> requirements;
}

public Move(string movement)
{
    string currentRoom = Rooms["startRoom"];

    if (Rooms["startRoom"].Connections == Connection.Command)
    {
        currentRoom = ?
    }
} 




