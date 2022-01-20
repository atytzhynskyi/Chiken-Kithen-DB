
namespace Randomizer
{
    public class ReproducerRnd : IRnd
    {
        private readonly int[] _values;
        private int _idx = 0;

        public ReproducerRnd(int[] values)
        {
            _values = values;
        }

        public double GetRandomDouble()
        {
            return _values[(_idx++) % _values.Length] / 100;
        }

        public int GetRandomInt(int min = 0, int max = int.MaxValue)
        {
            return _values[(_idx++) % _values.Length];
        }
    }

}
