
namespace ChikenKitchenDataBase
{
    public class Trash
    {
        public int Id { get; set; }
        public int Count{ get; set; }

        public Trash()
        {
        }

        public Trash(int count)
        {
            Count = count;
        }
        
    }
}
