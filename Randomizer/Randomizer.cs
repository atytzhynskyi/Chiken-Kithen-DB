using System;

namespace Randomizer
{
    public static class Randomizer
    {
        public static Random Random { get; set; }
        public static double GetRandomDouble()
        {
            return Random.NextDouble();
        }

        public static int GetRandomInt()
        {
            return Random.Next();
        }

    }
}
