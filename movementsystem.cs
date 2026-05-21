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

public void MainEntrance(string movement) {
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