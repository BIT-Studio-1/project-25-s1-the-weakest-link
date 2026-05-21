public class Movement 
{
   string moveEast;
   string moveWest;
   string moveSouth;
   string moveNorth;
}

Movement direction = new Movement();

public string MoveEast(string moveEast) 
{
   if (moveEast == "Go East") 
   {
      Console.WriteLine("You move east.");
   }
}