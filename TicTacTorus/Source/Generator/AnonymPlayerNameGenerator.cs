using System;
using System.Collections.Generic;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.Generator
{
    /*
        Memo for the future: A marcov chain might be the best choice for nice name generation.
        Gotta train it on some wikipedia articles or other sources of pronouncable text, fist.
    */
    public class AnonymPlayerNameGenerator
    {
        private static readonly string[] RareJokeNames = { "Noob!" };

        private static readonly Distribution<char> Vocals = new Distribution<char>
        (
            ('a', 651), ('e', 1740), ('i', 755), ('o', 251), ('u', 435), ('y', 4)
        );

        private static readonly Distribution<char> Consonants = new Distribution<char>
        (
            ('b', 189), ('c', 306), ('d', 508), ('f', 166), ('g', 301),
            ('h', 476), ('j',  27), ('k', 121), ('l', 344), ('m', 253),
            ('n', 978), ('p',  79), ('q',   2), ('r', 700), ('s', 727),
            ('t', 615), ('v',  67), ('w', 189), ('x',   3), ('z', 113)
        );
        
        public static string GetString()
        {
            var rnd = new Random();
            if (rnd.Next(10000) == 0)
            {
                //welcome to the name lottery, where you can win a rare and shitty pre-determined name.
                return RareJokeNames[rnd.Next(RareJokeNames.Length)];
            }

            var result = "";
            var consonantCount = 0;
            
            var length = rnd.Next(5, 10);
            for (var i = 0; i < length; ++i)
            {
                var isConsonant = consonantCount < 3 && ChooseConsonant(rnd);
                consonantCount = isConsonant ? consonantCount + 1 : 0;
                var c = (isConsonant ? Consonants : Vocals).Choose(rnd);

                result += i == 0 ? char.ToUpper(c) : char.ToLower(c);
            }
            return result;
        }
        
        private static bool ChooseConsonant(Random rnd)
        {
            rnd ??= new Random();
            return rnd.Next(Vocals.Sum + Consonants.Sum) < Consonants.Sum;
        }
    }
}