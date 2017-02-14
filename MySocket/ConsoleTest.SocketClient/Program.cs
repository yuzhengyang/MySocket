using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConsoleTest.SocketClient
{
    class Program
    {
        static Socket clientSocket;
        static byte[] MsgBuffer = new byte[4096];
        static void Main(string[] args)
        {
            SimpleTest();

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
            string host = "127.0.0.1";//服务器端ip地址

            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ip, port);

            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(ipe);

            //send message
            string sendStr = "send to server : hello,ni hao";
            byte[] sendBytes = Encoding.ASCII.GetBytes(sendStr);
            clientSocket.Send(sendBytes);

            //receive message
            for (int i = 0; 1 < 60; i++)
            {
                string recStr = "";
                byte[] recBytes = new byte[4096];
                int bytes = clientSocket.Receive(recBytes, recBytes.Length, 0);
                recStr += Encoding.ASCII.GetString(recBytes, 0, bytes);
                Console.WriteLine(recStr);

                //Thread.Sleep(1000);
            }

            clientSocket.Close();
        }

        static void SimpleTest2()
        {
            int port = 6000;
            string host = "127.0.0.1";//服务器端ip地址

            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ip, port);

            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Connect(ip, port);


            Console.WriteLine(DateTime.Now.ToString());
            //send message
            string sendStr = "send to server : hello,ni hao";
            byte[] sendBytes = Encoding.ASCII.GetBytes(sendStr);
            Send(sendBytes);

            //receive message
            ReceiveData();
            string recStr = Encoding.ASCII.GetString(MsgBuffer);
            Console.WriteLine(recStr);

            dispose();
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
