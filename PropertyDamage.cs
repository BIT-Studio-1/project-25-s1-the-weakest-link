namespace AwesomeGame;
/* File for keeping track of the various things you have damaged in your adventure
 If you want to make it so that, for example, "cut vines" means damaging network cables,
 causing $2000 in damage, just add PropertyDamage.CauseDamage("Damaged network cables", 2000);
 If you want to get the total "EXP", use PropertyDamage.TotalCost
*/
public static class PropertyDamage
{
    public static long totalcost;
    public static List<string> damagereasons = new List<string>();
    public static List<int> damageamount = new List<int>();

    public static void writebill()
    {
        string bill = $"""
        To XXXX,
        you have incurred damages in excess of ${totalcost:n2}.
        You have until next Sunday to pay Otago Polytechnic this cost
        otherwise you risk facing legal action. Attached is an itemised bill of damages.


        """;

        bill += "Reason".PadRight(60) + "Cost".PadLeft(25) + '\n';
        for (int i = 0; i < damagereasons.Count; i++)
        {
            bill += damagereasons[i].PadRight(60) + ("$" + damageamount[i].ToString("n2")).PadLeft(25) + '\n';
        }

        Game.scrolltext(bill);
    }

    public static void causedamage(string reason, int amount)
    {
        damagereasons.Add(reason);
        damageamount.Add(amount);
        totalcost += amount;

        Game.scrolltext($"You gain {amount} EXP!");
    }
}