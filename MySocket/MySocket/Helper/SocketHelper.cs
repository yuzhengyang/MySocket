using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MySocket.Helper
{
    public class SocketHelper : IDisposable
    {
        const int ReceiveBufferSize = 1024;
        const int ContentLengthSize = 4;

        byte[] ContentHead = new byte[] { 0x4A, 0x50 };

        private string _IP;
        private int _Port;
        private bool _IsConnected;
        private CancellationTokenSource ReceiveCts = new CancellationTokenSource();
        private List<byte> ReceiveByte = new List<byte>();
        private readonly object ReceiveLock = new object();

        public string IP { get { return _IP; } }
        public int Port { get { return _Port; } }
        public bool IsConnected { get { return _IsConnected; } }
        private IPAddress IPAddress { get; set; }
        private IPEndPoint IPEndPoint { get; set; }
        public Socket Socket { get; set; }

        public delegate void GetByteDelegate(byte[] b);
        public GetByteDelegate ReceiveByteContent;

        private SocketHelper() { }

        public SocketHelper(string ip, int port)
        {
            _IP = ip;
            _Port = port;
            IPAddress = IPAddress.Parse(ip);
            IPEndPoint = new IPEndPoint(IPAddress, port);
        }

        public bool Connect()
        {
            try
            {
                if (Socket != null)
                {
                    return true;
                }
                else
                {
                    Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    Socket.Connect(IPEndPoint);
                    _IsConnected = true;
                    return true;
                }
            }
            catch { }
            _IsConnected = false;
            return false;
        }

        public bool ImReceive(int length)
        {
            return Send(BitConverter.GetBytes(length));
        }
        public bool Send(string s)
        {
            try
            {
                byte[] sb = Encoding.ASCII.GetBytes(s);
                return Send(sb);
            }
            catch { }
            return false;
        }
        public bool Send(byte[] b)
        {
            try
            {
                //计算要发送的内容长度
                byte[] length = BitConverter.GetBytes(b.Length);
                //创建承载数据的byte[]数组
                byte[] sendByte = new byte[ContentHead.Length + length.Length + b.Length];
                //将信息头copy到数组
                ContentHead.CopyTo(sendByte, 0);
                //将信息长度copy到数组
                length.CopyTo(sendByte, ContentHead.Length);
                //将信息体copy到数组
                b.CopyTo(sendByte, sendByte.Length - b.Length);

                //发送
                int rs = Socket.Send(sendByte.ToArray());
                if (rs > 0) return true;
            }
            catch { }
            return false;
        }

        public void Receive()
        {
            Task.Factory.StartNew(() =>
            {
                lock (ReceiveLock)
                {
                    while (!ReceiveCts.IsCancellationRequested)
                    {
                        ReceiveContent();
                    }
                }
            }, ReceiveCts.Token);
        }
        private void ReceiveContent()
        {
            try
            {
                byte[] recByte = new byte[ReceiveBufferSize];
                int recLength = Socket.Receive(recByte, recByte.Length, 0);
                //保存并整理接收到的数据
                for (int k = 0; k < recLength; k++)
                    ReceiveByte.Add(recByte[k]);

                if (ReceiveByte.Count > 6 && ReceiveByte[0] == ContentHead[0] && ReceiveByte[1] == ContentHead[1])
                {
                    //标准Head数据的处理
                    int msgBodyLength = BitConverter.ToInt32(new byte[] { ReceiveByte[2], ReceiveByte[3], ReceiveByte[4], ReceiveByte[5] }, 0);

                    //数据接收完整处理
                    if (ReceiveByte.Count >= 6 + msgBodyLength)
                    {
                        //分离数据（Body）
                        byte[] body = ReceiveByte.GetRange(6, msgBodyLength).ToArray();
                        string bodyToGBK = Encoding.GetEncoding("GBK").GetString(body);

                        //接收消息通知委托方法
                        ReceiveByteContent(body);

                        //返回接收状态
                        ImReceive(msgBodyLength);
                        ReceiveByte.RemoveRange(0, 6 + msgBodyLength);
                    }
                }
                else
                {
                    //不正确的Head数据，清除数据区，等待新数据
                    ReceiveByte.Clear();
                    Socket.Send(new byte[] { 0 });
                }
            }
            catch (Exception e)
            { }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    ReceiveCts.Cancel();
                    Socket?.Shutdown(SocketShutdown.Both);
                    Socket?.Close();
                    Socket?.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~SocketTool() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
