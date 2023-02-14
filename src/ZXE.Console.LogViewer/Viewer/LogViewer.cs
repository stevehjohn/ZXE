using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ZXE.Console.LogViewer.Viewer;

public class LogViewer
{
    public void Run()
    {
        var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 1234);

        listener.Start();

        var buffer = new byte[256];

        while (true)
        {
            var client = listener.AcceptTcpClient();

            var stream = client.GetStream();

            var builder = new StringBuilder();

            while (stream.Read(buffer, 0, buffer.Length) != 0)
            {
                builder.Append(Encoding.ASCII.GetString(buffer));
            }

            System.Console.WriteLine(builder.ToString());
        }
    }
}