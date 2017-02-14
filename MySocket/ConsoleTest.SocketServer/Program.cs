using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest.SocketServer
{
    class Program
    {
        static Socket clientSocket;
        static byte[] MsgBuffer = new byte[4096];

        static void Main(string[] args)
        {
            SimpleTest2();
            End();
        }
        static void End()
        {
            Console.WriteLine("Press Enter To Exit");
            Console.ReadLine();
        }
        static void SimpleTest()
        {
            int port = 6000;
            string host = "127.0.0.1";

            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ip, port);

            Socket sSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sSocket.Bind(ipe);
            sSocket.Listen(0);
            Console.WriteLine("监听已经打开，请等待");

            //receive message
            Socket serverSocket = sSocket.Accept();
            Console.WriteLine("连接已经建立");
            string recStr = "";
            byte[] recByte = new byte[4096];
            int bytes = serverSocket.Receive(recByte, recByte.Length, 0);
            recStr += Encoding.ASCII.GetString(recByte, 0, bytes);

            //send message
            Console.WriteLine("服务器端获得信息:{0}", recStr);
            string sendStr = "send to client :hello";
            byte[] sendByte = Encoding.ASCII.GetBytes(sendStr);
            serverSocket.Send(sendByte, sendByte.Length, 0);
            serverSocket.Close();
            sSocket.Close();
        }
        static void SimpleTest2()
        {
            int port = 6000;
            string host = "127.0.0.1";

            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ip, port);

            Socket sSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sSocket.Bind(ipe);
            sSocket.Listen(2);

            for (int i = 1; i < 10; i++)
            {
                Console.WriteLine("监听已经打开，等待 " + i + " 个接入的连接");

                Socket serverSocket = sSocket.Accept();
                Console.WriteLine("第 " + i + " 个连接已经建立");
                Task.Factory.StartNew(() =>
                {
                    string recStr = "";
                    byte[] recByte = new byte[4096];
                    int bytes = serverSocket.Receive(recByte, recByte.Length, 0);
                    recStr += Encoding.ASCII.GetString(recByte, 0, bytes);

                    //send message
                    Console.WriteLine("服务器端获得信息:{0}", recStr);
                    string sendStr = "send to client :hello > " + i;
                    byte[] sendByte = Encoding.ASCII.GetBytes(sendStr);
                    serverSocket.Send(sendByte, sendByte.Length, 0);
                    //serverSocket.Close();
                });
            }
            sSocket.Close();
        }
        static void Connect(IPAddress ip, int port)
        {
            clientSocket.BeginConnect(ip, port, new AsyncCallback(ConnectCallback), clientSocket);
        }
        static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                handler.EndConnect(ar);
            }
            catch (SocketException ex)
            { }
        }
        static void Send(string data)
        {
            Send(System.Text.Encoding.UTF8.GetBytes(data));
        }
        static void Send(byte[] byteData)
        {
            try
            {
                int length = byteData.Length;
                byte[] head = BitConverter.GetBytes(length);
                byte[] data = new byte[head.Length + byteData.Length];
                Array.Copy(head, data, head.Length);
                Array.Copy(byteData, 0, data, head.Length, byteData.Length);
                clientSocket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), clientSocket);
            }
            catch (SocketException ex)
            { }
        }
        static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                handler.EndSend(ar);
            }
            catch (SocketException ex)
            { }
        }
        static void ReceiveData()
        {
            clientSocket.BeginReceive(MsgBuffer, 0, MsgBuffer.Length, 0, new AsyncCallback(ReceiveCallback), null);
        }
        static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int REnd = clientSocket.EndReceive(ar);
                if (REnd > 0)
                {
                    byte[] data = new byte[REnd];
                    Array.Copy(MsgBuffer, 0, data, 0, REnd);

                    //在此次可以对data进行按需处理

                    clientSocket.BeginReceive(MsgBuffer, 0, MsgBuffer.Length, 0, new AsyncCallback(ReceiveCallback), null);
                }
                else
                {
                    dispose();
                }
            }
            catch (SocketException ex)
            { }
        }
        static void dispose()
        {
            try
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            catch (Exception ex)
            { }
        }
    }
}
