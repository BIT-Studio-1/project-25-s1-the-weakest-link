using System.Net.Quic;

namespace AwesomeGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string option = "";

            while (option != "quit" && option != "exit")
            {
                Console.WriteLine("what do you want to do");
                option = Console.ReadLine().Substring(0, 4).ToLower();
                Console.WriteLine(option);
            }
        }
    }
}
