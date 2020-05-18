using System;

namespace TicTacTorus.Source.Generator
{
    public class AnonymPlayerNameGenerator
    {
        // Database 
        private static readonly string[] _names = {"Exe", "Person", "Bill Clinton"};
        
        public static string GetString()
        {
            var ran = new Random();
            var index = ran.Next(_names.Length);

            return _names[index];
        }
    }
}