using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Cryptography.X509Certificates;

namespace ConsoleAppServeurTCP
{
    internal class Program
    {
        public delegate void ThreadStart();
        static void Main(string[] args)
        {
            
            IPHostEntry host = Dns.GetHostEntry("Anzi");
            IPAddress ip = IPAddress.Any;
            int port = Convert.ToInt32(args[0]);
            IPEndPoint endPoint = new IPEndPoint(ip, 65000);
            Socket listener = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listener.Bind(endPoint);
                listener.Listen(10);
                while (true)
                {
                    Console.WriteLine("En attente connexion...");
                    Socket client = listener.Accept();
                    byte[] bytes = new Byte[1024];
                    string data = null;
                    while (true)
                    {
                        int b = client.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, b);
                        if (data.IndexOf("") > -1)
                            break;
                    }
                    Console.WriteLine("Texte reçu -> {0} ", data);
                    string acuseReception = args[1];
                    byte[] message = Encoding.ASCII.GetBytes(acuseReception);
                    client.Send(message);
                    if (args[2] == data)
                    {
                        Console.WriteLine("Message de mise hors service recu. Le serveur a ete tue.");
                        return;
                    }
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    }
}
