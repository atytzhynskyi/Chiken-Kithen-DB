
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
            //return _values[(_idx++) % _values.Length] / 100;
            var value = (double)_values[_idx] / 100;
            UpdataIndex();
            return value;
        }

        public int GetRandomInt(int min = 0, int max = int.MaxValue)
        {
            //return _values[(_idx++) % _values.Length];
            var value = _values[_idx];
            UpdataIndex();
            return value;
        }

        private void UpdataIndex()
        {
            _idx++;
            _idx = _idx != _values.Length ? _idx : 0;
        }
    }

}
