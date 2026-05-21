public void Move(string movement) {
   string currentRoom = "startRoom";
   switch (movement) {
      case "sideroom":
         currentRoom = "sideroom";
      case "hallway1":
         currentRoom = "hallway1";
      case "hallway4":
         if (Inventory.Contains("tablet")) {
         currentRoom = "hallway4";
         }        
   }
}