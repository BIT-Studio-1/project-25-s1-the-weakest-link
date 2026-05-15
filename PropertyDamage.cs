namespace AwesomeGame;

// Keeping this in a seperate file for now just for ease of development, might merge into program later for simplicity
public class PropertyDamage
{
    public static void TempMain()
    {
        Dictionary<string, int> AllDamages = new Dictionary<string, int>();
        int TotalCost = 0;
        bool condition = true;

        while (condition)
        {
            Console.WriteLine("Example things you can do: cut vines, attack monster, show stats, show bill, exit");
            Console.Write("What do you want to do? ");
            string command = Console.ReadLine();

            switch (command)
            {
                case "cut vines":
                    AllDamages["Damaged wiring in network room"] = 2000;
                    TotalCost += 2000;

                    Console.WriteLine("You cut through the vines, clearing a path forward. You gain 2000 EXP!");
                    break;
                case "attack monster":
                    AllDamages["Destroyed office chair"] = 250;
                    TotalCost += 250;

                    Console.WriteLine("You knock the monster to the ground, killing it. You gain 250 EXP!");
                    break;
                case "show stats":
                    Console.WriteLine($"You have {TotalCost} EXP.");
                    break;
                case "show bill":
                    string bill = $"""
                    To Mr. XXXX,
                    you have incurred damages in excess of {TotalCost:c2}.
                    You have until next Sunday to pay Otago Polytechnic this cost
                    otherwise you risk facing legal action. Attached is an itemised bill of damages.


                    """;

                    bill += "Reason".PadRight(50) + "Cost".PadLeft(50) + '\n';
                    foreach (KeyValuePair<string, int> pair in AllDamages)
                    {
                        bill += pair.Key.PadRight(50) + pair.Value.ToString().PadLeft(50) + '\n';
                    }

                    Console.WriteLine(bill);
                    break;
                case "exit":
                    condition = false;
                    break;
            }
        }
    } 
}