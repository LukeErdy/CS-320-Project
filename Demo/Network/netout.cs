using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class TcpIn
{
  public static void Main()
  {
    TcpListener server = null;
    try
    {
      Int32 port = 51000;
      IPAddress localAddr = IPAddress.Parse("127.0.0.1");

      server = new TcpListener(localAddr, port);

      server.Start();

      Byte[] bytes = new Byte[256];
      String data = null;

      while(true)
      {
        Console.Write("Waiting for a connection... ");

        using TcpClient client = server.AcceptTcpClient();
        Console.WriteLine("Connection established");

        data = null;

        NetworkStream stream = client.GetStream();

        int i;

        while((i = stream.Read(bytes, 0, bytes.Length))!=0)
        {
          data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
          Console.WriteLine("Received: {0}", data);

          data = data.ToUpper();

          byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

          stream.Write(msg, 0, msg.Length);
          Console.WriteLine("Sent: {0}", data);
        }
      }
    }
    catch(SocketException e)
    {
      Console.WriteLine("SocketException: {0}", e);
    }
    finally
    {
      server.Stop();
    }

    Console.WriteLine("\nPress enter to continue...");
    Console.Read();
  }
}
