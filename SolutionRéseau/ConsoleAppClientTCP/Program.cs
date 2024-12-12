using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppClientTCP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry("Anzi");
                IPAddress ip = host.AddressList[1];
                int port = Convert.ToInt32(args[0]);
                IPEndPoint endPoint = new IPEndPoint(ip, port);
                Socket client = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    client.Connect(endPoint);
                    string messageToSend = "Bonjour serveur !";
                    byte[] msg = Encoding.ASCII.GetBytes(messageToSend);
                    int byteSent = client.Send(msg);
                    Console.WriteLine("Message envoyé : {0}", messageToSend);
                    byte[] messageReceived = new byte[1024];
                    Console.WriteLine("Attente d'une reponse du serveur...");
                    int byteRecv = client.Receive(messageReceived);
                    string response = Encoding.ASCII.GetString(messageReceived, 0, byteRecv);
                    Console.WriteLine("Message du serveur -> {0}", response);
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
                catch (SocketException e1)
                {
                    Console.WriteLine("SocketException : {0}", e1.ToString());
                }
                catch (ArgumentNullException e2)
                {
                    Console.WriteLine("ArgumentNullException : {0}", e2.ToString());
                }
                catch (Exception e3)
                {
                    Console.WriteLine("Unexpected exception : {0}", e3.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

}
