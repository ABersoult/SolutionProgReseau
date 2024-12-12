using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Runtime.CompilerServices;

namespace ServeurWPFConcurrent
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BTNDebServeur_Click(object sender, RoutedEventArgs e)
        {
            int port = Convert.ToInt32(Properties.Settings.Default.port);
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
                    Socket client = listener.Accept();
                    ThreadStart delegue = new ThreadStart(() => ThreadTask(AcuseRecept, ShutdownMessage, RequêteClient));
                    Thread thread = new Thread(delegue);
                    thread.Start();
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.ToString());
            }
        }
        static void ThreadTask(TextBox accuse, TextBox shutdown, TextBox Receive)
        {
            try
            {
                Socket client = null;
                byte[] bytes = new Byte[1024];
                string data = null;
                int b = client.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, b);
                Receive.Text = data;
                byte[] message = Encoding.ASCII.GetBytes(Properties.Settings.Default.AcuseReception);
                client.Send(message);
                if (data == Properties.Settings.Default.ShutdownMessage)
                {
                    shutdown.Text = Properties.Settings.Default.ShutdownMessage;
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Erreur lors de la communication avec le client : " + e.Message);
            }
        }

        private void BTNFinServeur_Click(object sender, RoutedEventArgs e)
        {
            Socket client = null;
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
    }
}
