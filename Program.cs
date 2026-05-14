using System.Globalization;
using System.Net.Quic;
using static System.Console;

namespace AwesomeGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string option = "";

            while (option != "quit" && option != "exit")
            {
                WriteLine("what do you want to do");
                try
                {
                    option = ReadLine().Substring(0, 4).ToLower();
                }
                catch
                {
                    WriteLine("please input a valid command");
                }

                WriteLine(option);
            }
        }
    }
}
