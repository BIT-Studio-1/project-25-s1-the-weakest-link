string currentRoom = "startroom";

public void StartRoom(string movement) {
   switch (movement) {
      case "side room":
         currentRoom = "sideroom";
      case "main entrance":
         currentRoom = "hallway1";
      case "second entrance":
         if (Inventory.Contains("tablet")) {
         currentRoom = "hallway4";
         }        
   }
}

public void hallway1(string movement) {
   switch (movement) {
      case "starting room":
         currentRoom = "startroom";
      case "small room":
         currentRoom = "vinesroom";
      case "open room":
         currentRoom = "kniferoom";
      case "locked door":
      if (Inventory.Contains("tablet")) {
         currentRoom = "tabletroom";
      }
   }
}

public void kniferoom(string movement) {
   if (movement == "hallway") {
      currentRoom = "hallway1"
   }
}

public void vinesroom(string movement) 
{
   switch (movement) 
   {
      case "main entrance":
         currentRoom = "hallway1";
      case "main room":
         if (vinescut == true) 
         {
            currentRoom = "tabletroom";
         }
         else 
         {
            Console.WriteLine("Vines have not been cut.")
         }         
   }
}

public void tabletroom(string movement) 
{
   switch (movement) 
   {
      case "side room":
      currentRoom = "vinesroom";
      case "main entrance":
      currentRoom = "hallway1";
   }
}

public void hallway2(string movement)
{
   switch (movement)
   {
      case "glass door":
      if (Inventory.Contains("key"))
      {
         currentRoom = "stairs";
      }
      case "north":
      currentRoom = "hallway1";
      case "west":
      currentRoom = "hallway3";
   }
}

public void hallway3(string movement) 
{
   switch (movement)
   {
      case "east":
      currentRoom = "hallway2";
      case "north":
      currentRoom = "hallway4";
   }
}

public void hallway4(string movement) 
{
   switch (movement) 
   {
      case "starting room":
      if (Inventory.Contains("tablet"))
      {
         currentRoom = "startroom";
      }
      case "locked door":
      if (Inventory.Contains("tablet"))
      {
         currentRoom = "keyroom";
      }
      case "south":
      currentRoom = "hallway3";
   }
}

public void keyroom(string movement)
{
   switch (movement)
   {
      case "hallway":
      currentRoom = "hallway4";
   }
}