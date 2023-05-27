namespace ArbdTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the MonoTorrent tracker");
            Console.WriteLine("Start the tracker");
            MySimpleTracker.StartTracker();
        }
    }
}