using System;

namespace ExeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 1)
                Console.WriteLine("<h1>Bonjour " + args[0] + "</h1>");
            else
                Console.WriteLine("ExeTest <string parameter>");
        }
    }
}
