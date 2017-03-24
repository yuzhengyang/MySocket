/********************************************************************
 * * 使本项目源码前请仔细阅读以下协议内容，如果你同意以下协议才能使用本项目所有的功能,
 * * 否则如果你违反了以下协议，有可能陷入法律纠纷和赔偿，作者保留追究法律责任的权利。
 * *
 * * Copyright (C) 2014-? cskin Corporation All rights reserved.
 * * 作者： Amos Li    QQ：443061626   .Net项目技术组群:Amos Li 出品
 * * 请保留以上版权信息，否则作者将保留追究法律责任。
 * * 创建时间：2014-08-05
********************************************************************/
using System;
using System.Text;

namespace SocketClientCommonLib
{
    /// <summary>
    /// 缓存处理机制
    /// </summary>
    public class DynamicBufferManager
    {
        public byte[] Buffer { get; set; } //存放内存的数组
        public int DataCount { get; set; } //写入数据大小

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="bufferSize">缓存大小</param>
        public DynamicBufferManager(int bufferSize)
        {
            DataCount = 0;
            Buffer = new byte[bufferSize];
        }

        /// <summary>
        /// 获取当前用户没处理完的缓存大小
        /// </summary>
        /// <returns></returns>
        public int GetDataCount()
        {
            return DataCount;
        }

        /// <summary>
        /// 获取还剩余缓存大小
        /// </summary>
        /// <returns>int</returns>
        public int GetReserveCount()
        {
            return Buffer.Length - DataCount;
        }
        /// <summary>
        /// 清除缓存全部大小(0)
        /// </summary>
        public void Clear()
        {
            DataCount = 0;
        }
        /// <summary>
        /// 清除已经处理的缓存
        /// </summary>
        /// <param name="count">已经计算过的byte大小</param>
        public void Clear(int count)
        {
            if (count >= DataCount)
            {
                DataCount = 0;
            }
            else
            {
                for (int i = 0; i < DataCount - count; i++) //否则后面的数据往前移
                {
                    Buffer[i] = Buffer[count + i];
                }
                DataCount = DataCount - count;
            }
        }

        /// <summary>
        /// 清除指定的缓存
        /// </summary>
        /// <param name="curre">缓存起始位置</param>
        /// <param name="count">已经计算过的byte大小</param>
        public void Clear(int curre, int count)
        {
            if (count >= DataCount)
            {
                DataCount = 0;
            }
            else
            {
                for (int i = curre; i < DataCount - count; i++) //数据往前移
                {
                    Buffer[i] = Buffer[count + i];
                }
                DataCount = DataCount - count;
            }
        }
        /// <summary>
        /// 扩大接收缓存:请设置大于初始缓存的数字
        /// 扩充后原历史数据不变
        /// </summary>
        /// <param name="size">缓存大小Byte</param>
        public void SetBufferSize(int size)
        {
            if (Buffer.Length < size)
            {
                byte[] tmpBuffer = new byte[size];
                Array.Copy(Buffer, 0, tmpBuffer, 0, DataCount);
                Buffer = tmpBuffer;
            }
        }

        /// <summary>
        /// 写入缓存(缓冲区空间不够,自动扩大缓存)
        /// </summary>
        /// <param name="buffer">byte[]</param>
        /// <param name="offset">写入缓存初始位置</param>
        /// <param name="count">写入缓存大小</param>
        public void WriteBuffer(byte[] buffer, int offset, int count)
        {
            if (GetReserveCount() >= count)
            {
                Array.Copy(buffer, offset, Buffer, DataCount, count);
                DataCount = DataCount + count;
            }
            else //缓冲区空间不够，需要申请更大的内存，并进行移位 
            {
                int totalSize = Buffer.Length + count - GetReserveCount(); //总大小-空余大小
                byte[] tmpBuffer = new byte[totalSize];
                Array.Copy(Buffer, 0, tmpBuffer, 0, DataCount); //复制以前的数据
                Array.Copy(buffer, offset, tmpBuffer, DataCount, count); //复制新写入的数据
                DataCount = DataCount + count;
                Buffer = tmpBuffer; //替换
            }
        }
        /// <summary>
        /// 写入缓存(缓冲区空间不够,自动扩大缓存)自动判断缓存大小
        /// </summary>
        public void WriteBuffer(byte[] buffer)
        {
            WriteBuffer(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 网络传输时,避免粘包现象,先确定长度
        /// </summary>
        /// <param name="value">长度</param>
        /// <param name="convert">长度写入到缓存中(一个字节byte)</param>
        public void WriteInt(int value)
        {
            byte[] tmpBuffer = BitConverter.GetBytes(value);
            WriteBuffer(tmpBuffer);
        }

        /// <summary>
        /// 文本全部转成UTF8，UTF8兼容性好
        /// </summary>
        /// <param name="value"></param>
        public void WriteString(string value)
        {
            byte[] tmpBuffer = Encoding.UTF8.GetBytes(value);
            WriteBuffer(tmpBuffer);
        }
    }
}
