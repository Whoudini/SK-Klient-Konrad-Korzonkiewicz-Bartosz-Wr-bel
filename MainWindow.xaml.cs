using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
using System.Windows.Threading;

namespace Klient
{

    class Game
    {
        public string status { get; set; }
    public string playernumber { get; set; }
    public string readynumber { get; set; }
    public int TryNumber { get; set; }
    public bool connected { get; set; }
    public string matchoutcome { get; set; }
        public Game(string s,string p,string r, int t,bool con, string m)
        {
            status = s;
            playernumber = p;
            readynumber = r;
            TryNumber = t;
            connected = con;
            matchoutcome = m;
        }
    
}
    public partial class MainWindow : Window
    {
        private static readonly Socket ClientSocket = new Socket
           (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private const int PORT = 100;
        private const int BUFFER_SIZE = 2048;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];
        static Game GameInfo = new Game("0","0","0",0,false,"");
        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Tick;
            timer.Start();

        }


        void Tick(object sender, EventArgs e)
        {
            mtext.Text = GameInfo.matchoutcome;
            status.Text = GameInfo.status;
            if (GameInfo.connected)
            {
                SendString("status");
                ReceiveResponse();
               
            }
        }



        private static void ConnectToServer(string IpAddress , int port )
        {

                try
                {
                    GameInfo.TryNumber++;
               
                    ClientSocket.Connect(IPAddress.Parse(IpAddress),port);
                    GameInfo.connected = true;
                }
            catch (SocketException ex)
            {
                
            }
            catch (ObjectDisposedException ex)
            {
                
            }

        }
        private static void ConnectCallback(IAsyncResult result)
        {
            if (ClientSocket.Connected)
            {
                ClientSocket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, ClientSocket);
            }
        }
        private static void ReceiveCallback(IAsyncResult result)
        {
            Socket current = (Socket)result.AsyncState;
            var buffer = new byte[2048];
            int received = ClientSocket.EndReceive(result);
            if (received == 0) return;
            var data = new byte[received];
            Array.Copy(buffer, data, received);
            string text = Encoding.ASCII.GetString(data);


            if (text.ToLower() == "0")
            {
                GameInfo.status = "Offline";
            }
            else if (text.ToLower() == "1")
            {
                GameInfo.status = "Online";
            }
            else if (text.ToLower() == "2")
            {
                GameInfo.status = "Wait for others";
            }
            else if (text.ToLower() == "3")
            {
               // GameInfo.status = "Draw";
                GameInfo.matchoutcome = "Draw";
            }
            else if (text.ToLower() == "4")
            {
               // GameInfo.status = "Lost";
                GameInfo.matchoutcome = "Lost";
            }
            else if (text.ToLower() == "5")
            {
                //GameInfo.status = "Win";
                GameInfo.matchoutcome = "Win";
            }
            else if (text.ToLower() == "6")
            {
                GameInfo.status = "In Game";
            }
            else if (text.ToLower() == "7")
            {

            }
        }

        private static void SendString(string text)
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes(text);
                ClientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallback, null);
            }
            catch
            {
                //ConnectToServer("192.168.0.17", 100);
            }
        }

        private static void SendCallback(IAsyncResult result)
        {
            try
            {
                ClientSocket.EndSend(result);
            }
            catch(SocketException ex)
            {

            }
        }
        private static void PlayerCounter()
        {
           

               
            
        }

        private static void ReceiveResponse()
        {
            var buffer = new byte[2048];
            int received = ClientSocket.Receive(buffer, SocketFlags.None);
            if (received == 0) return;
            var data = new byte[received];
            Array.Copy(buffer, data, received);
            string text = Encoding.ASCII.GetString(data);
            

            if (text.ToLower() =="0")
            {
                GameInfo.status = "Connected";
            }
            else if(text.ToLower() == "1")
            {
                GameInfo.status = "In Lobby";
            }
            else if (text.ToLower() == "2")
            {
                GameInfo.status = "Wait for others";
            }
            else if (text.ToLower() == "3")
            {
                GameInfo.matchoutcome = "Draw";
                GameInfo.status = "END GAME";
            }
            else if (text.ToLower() == "4")
            {
                GameInfo.matchoutcome = "Lost";
                GameInfo.status = "END GAME";
            }
            else if (text.ToLower() == "5")
            {
                GameInfo.matchoutcome = "Win";
                GameInfo.status = "END GAME";
            }
            else if (text.ToLower() == "6")
            {
                GameInfo.status = "In Game";
            }
            else if(text.ToLower() == "7")
            {

            }

        }

        private void Button_Connect(object sender, RoutedEventArgs e)
        {
            
            ConnectToServer(IP.Text, Int32.Parse(P.Text));
            SendString(Name.Text);
            GameInfo.connected = true;
        }

        private void Button_Ready(object sender, RoutedEventArgs e)
        {
            SendString("ready");
            
        }

        private void Button_Rock(object sender, RoutedEventArgs e)
        {
            SendString("1");
        }
        private void Button_Paper(object sender, RoutedEventArgs e)
        {
            SendString("2");
        }
        private void Button_Scissors(object sender, RoutedEventArgs e)
        {
            SendString("3");
        }
        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            SendString("exit");
            this.Close();
        }

    }
}
