namespace AwesomeGame;

// Keeping this in a seperate file for now just for ease of development, might merge into program later for simplicity
public class PropertyDamage
{
    static int TotalCost;
    static Dictionary<string, int> AllDamages = new Dictionary<string, int>();

    public static void WriteBill()
    {
        string bill = $"""
        To Mr. XXXX,
        you have incurred damages in excess of {TotalCost:c2}.
        You have until next Sunday to pay Otago Polytechnic this cost
        otherwise you risk facing legal action. Attached is an itemised bill of damages.


        """;

        bill += "Reason".PadRight(50) + "Cost".PadLeft(50) + '\n';
        foreach (KeyValuePair<string, int> pair in AllDamages)
        {
            bill += pair.Key.PadRight(50) + pair.Value.ToString("c2").PadLeft(50) + '\n';
        }

        Console.WriteLine(bill);
    }

    public static void CauseDamage(string reason, int amount)
    {
        AllDamages[reason] = amount;
        TotalCost += amount;
    }

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
                    Console.WriteLine("You cut through the vines, clearing a path forward. You gain 2000 EXP!");
                    CauseDamage("Damaged wiring in network room", 2000);
                    break;
                case "attack monster":
                    Console.WriteLine("You knock the monster to the ground, killing it. You gain 250 EXP!");
                    CauseDamage("Destroyed office chair", 250);

                    break;
                case "show stats":
                    Console.WriteLine($"You have {TotalCost} EXP.");
                    break;
                case "show bill":
                    WriteBill();
                    break;
                case "exit":
                    condition = false;
                    break;
            }
        }
    } 
}