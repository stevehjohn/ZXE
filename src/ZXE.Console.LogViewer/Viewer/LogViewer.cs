using System.Net;
using System.Net.Sockets;
using System.Text;
using ZXE.Common.ConsoleHelpers;

namespace ZXE.Console.LogViewer.Viewer;

public class LogViewer
{
    public void Run()
    {
        var listener = new TcpListener(IPAddress.Loopback, 1234);

        listener.Start();

        var buffer = new byte[256];

        while (true)
        {
            var client = listener.AcceptTcpClient();

            var stream = client.GetStream();

            var builder = new StringBuilder();

            while (stream.DataAvailable)
            {
                var read = stream.Read(buffer, 0, buffer.Length);

                builder.Append(Encoding.ASCII.GetString(buffer, 0, read));
            }

            FormattedConsole.WriteLine(builder.ToString());
        }

        // ReSharper disable once FunctionNeverReturns
    }
}