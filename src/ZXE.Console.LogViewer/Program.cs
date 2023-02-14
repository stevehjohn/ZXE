namespace ZXE.Console.LogViewer
{
    public static class Program
    {
        public static void Main()
        {
            var viewer = new Viewer.LogViewer();

            viewer.Run();
        }
    }
}