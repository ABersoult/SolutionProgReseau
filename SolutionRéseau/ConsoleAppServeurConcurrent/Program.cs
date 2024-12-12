using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Policy;

namespace ConsoleAppServeurConcurrent
{
    internal class Program
    {
        struct paramServeur
        {
            string acuseReception;
            public string AcuseReception { get { return acuseReception; } set { acuseReception = value; } }
            string shutdownMessage;
            public string ShutdownMessage { get { return shutdownMessage; } set { shutdownMessage = value; } }
            Socket client;
            public Socket Client { get { return client; }set { client = value; } }
            public paramServeur(string acuse,string shutdown, Socket client)
            {
                this.acuseReception = acuse;
                this.shutdownMessage = shutdown;
                this.client = client;
            }
        }
        static void Main(string[] args)
        {
            int port = Convert.ToInt32(args[0]);
            IPHostEntry host = Dns.GetHostEntry("Anzi");
            IPAddress ip = IPAddress.Any;
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
                    Console.WriteLine("Client connecté : {0}:{1}", ((IPEndPoint)client.RemoteEndPoint).Address, ((IPEndPoint)client.RemoteEndPoint).Port);
                    string acuseRecption = args[1];
                    string shutdownMessage = args[2];
                    paramServeur param = new paramServeur(acuseRecption, shutdownMessage, client);
                    ThreadStart delegue = new ThreadStart(() => ThreadTask(param));
                    Thread thread = new Thread(delegue);
                    thread.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        static void ThreadTask(paramServeur param)
        {
            Socket client = param.Client;
            try
            {
                byte[] bytes = new Byte[1024];
                string data = null;
                int b = client.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, b);
                Console.WriteLine("Texte reçu du client : {0}", data);
                byte[] message = Encoding.ASCII.GetBytes(param.AcuseReception);
                client.Send(message);
                if (data == param.ShutdownMessage)
                {
                    Console.WriteLine("Message de mise hors service reçu. Le serveur va se fermer.");
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur lors de la communication avec le client : " + e.Message);
            }
            finally
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
        }
    }
}
