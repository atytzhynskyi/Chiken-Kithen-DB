
namespace Randomizer
{
    public interface IRnd
    {
        double GetRandomDouble();
        int GetRandomInt(int min = 0, int max = int.MaxValue);
    }
}
