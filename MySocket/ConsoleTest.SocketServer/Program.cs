using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
            int port = 28001;
            string host = "192.168.255.24";

            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ip, port);

            Socket sSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sSocket.Bind(ipe);
            sSocket.Listen(0);
            //sSocket.ReceiveTimeout = 3000;
            //sSocket.IOControl(IOControlCode.KeepAliveValues, KeepAlive(1, 1000, 1000), null);

            //for (int i = 1; i < 10; i++)
            //{
                Console.WriteLine("准备就绪，等待接入客户端");
                Socket serverSocket = sSocket.Accept();
                serverSocket.ReceiveTimeout = 5000;

                Task.Factory.StartNew(() =>
                {
                    //SetKeepAlive(serverSocket, 1000, 1000);
                    int index = 1;
                    Console.WriteLine("接入客户端 " + index + " timeout = " + serverSocket.ReceiveTimeout);
                    while (serverSocket.Connected)
                    {
                        if (serverSocket.Connected)
                        {
                            try
                            {
                                byte[] recByte = new byte[4096];
                                int bytes = serverSocket.Receive(recByte, recByte.Length, 0);
                                string recStr = Encoding.GetEncoding("GBK").GetString(recByte, 0, bytes);
                                if (recStr.Length > 0)
                                    Console.WriteLine("接收到信息：" + recStr);

                                //string sendStr = ">";
                                //byte[] sendByte = Encoding.GetEncoding("GBK").GetBytes(sendStr);
                                //serverSocket.Send(sendByte, sendByte.Length, 0);
                            }
                            catch (Exception e)
                            {
                                break;
                            }
                        }
                        Thread.Sleep(500);
                    }
                    Console.WriteLine(index + " 失去连接");

                    //send message
                    //Console.WriteLine("服务器端获得信息:{0}", recStr);
                    //string sendStr = "send to client :hello > " + i;
                    //byte[] sendByte = Encoding.ASCII.GetBytes(sendStr);
                    //serverSocket.Send(sendByte, sendByte.Length, 0);
                    //serverSocket.Close();
                });
            //}
        }
        static byte[] KeepAlive(int onOff, int keepAliveTime, int keepAliveInterval)
        {
            byte[] buffer = new byte[12];
            BitConverter.GetBytes(onOff).CopyTo(buffer, 0);
            BitConverter.GetBytes(keepAliveTime).CopyTo(buffer, 4);
            BitConverter.GetBytes(keepAliveInterval).CopyTo(buffer, 8);
            return buffer;
        }
        static void SetKeepAlive(Socket socket, ulong keepalive_time, ulong keepalive_interval)
        {
            int bytes_per_long = 32 / 8;
            byte[] keep_alive = new byte[3 * bytes_per_long];
            ulong[] input_params = new ulong[3];
            int i1;
            int bits_per_byte = 8;

            if (keepalive_time == 0 || keepalive_interval == 0)
                input_params[0] = 0;
            else
                input_params[0] = 1;
            input_params[1] = keepalive_time;
            input_params[2] = keepalive_interval;
            for (i1 = 0; i1 < input_params.Length; i1++)
            {
                keep_alive[i1 * bytes_per_long + 3] = (byte)(input_params[i1] >> ((bytes_per_long - 1) * bits_per_byte) & 0xff);
                keep_alive[i1 * bytes_per_long + 2] = (byte)(input_params[i1] >> ((bytes_per_long - 2) * bits_per_byte) & 0xff);
                keep_alive[i1 * bytes_per_long + 1] = (byte)(input_params[i1] >> ((bytes_per_long - 3) * bits_per_byte) & 0xff);
                keep_alive[i1 * bytes_per_long + 0] = (byte)(input_params[i1] >> ((bytes_per_long - 4) * bits_per_byte) & 0xff);
            }
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, keep_alive);
        } /* method AsyncSocket SetKeepAlive */
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
