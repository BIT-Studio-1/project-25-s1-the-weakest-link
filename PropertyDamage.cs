namespace AwesomeGame;

/* File for keeping track of the various things you have damaged in your adventure
 If you want to make it so that, for example, "cut vines" means damaging network cables,
 causing $2000 in damage, just add PropertyDamage.CauseDamage("Damaged network cables", 2000);
 If you want to get the total "EXP", use PropertyDamage.TotalCost
*/
public static class PropertyDamage
{
    public static int TotalCost;
    public static Dictionary<string, int> AllDamages = new Dictionary<string, int>();

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
}