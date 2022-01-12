using System;

namespace Randomizer
{
    public static class Randomizer
    {
        public static Random Random { get; set; }
        public static bool GetRandomBool()
        {
            return Random.Next() % 2 == 0;
        }
        public static double GetRandomDouble()
        {
            return Random.NextDouble();
        }

        public static int GetRandomInt(int max)
        {
            return Random.Next()%(max+1);
        }
    }
}
