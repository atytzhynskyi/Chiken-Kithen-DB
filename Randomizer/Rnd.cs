using System;

namespace Randomizer
{
    public class Rnd : IRnd
    {
        private readonly Random _rnd;

        public Rnd()
        {
            _rnd = new Random();
        }

        public Rnd(int seed)
        {
            _rnd = new Random(seed);
        }

        public double GetRandomDouble()
        {
            return _rnd.NextDouble();
        }

        public int GetRandomInt(int min = 0, int max = int.MaxValue)
        {
            return _rnd.Next(1, max);
        }
    }
}
