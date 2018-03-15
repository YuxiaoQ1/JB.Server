using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Common
{
    public class MessageHelper
    {
        const int buffer_init_size = 100;

        public static ST_FIGHT_ID DeserializeWithBinary(object dta)
        {
            throw new NotImplementedException();
        }

        private byte[] buffer;  //接收缓冲区
        private int processPos, currentPos;            //处理指针，记录未处理的数据的起始位置
        private Queue<byte[]> msgs = new Queue<byte[]>();              //接收到的消息队列
        public byte[] Buffer => buffer;
        public int GetStartIndex => currentPos;
        public int GetRemainBytes => buffer.Length - currentPos;

        public MessageHelper()
        {
            _ResetBuffer();
        }

        /// <summary>
        /// 接收数据后需调用此方法来执行消息的解包处理
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public void AddCount(int count)                            //每次接收到数据之后需要执行
        {
            if (count <= 0) return;
            currentPos += count;
            _ResolutionMsg();
            int remain = buffer.Length - currentPos;
            if (remain == 0)
            {
                int new_size = buffer.Length * 2;
                byte[] new_buffer = new byte[new_size];
                buffer.Skip(processPos).ToArray().CopyTo(new_buffer, 0);
                currentPos = currentPos - processPos;
                processPos = 0;
                buffer = new_buffer;
                //Console.WriteLine("Message::GetRemainBytes = 0, Expand buffer size to {0}!", buffer.Length);
            }
        }
        
        public bool HasMessage()
        {
            return msgs.Count != 0;
        }

        /// <summary>
        /// 将对象序列化为二进制数据 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] SerializeToBinary(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, obj);

            byte[] data = stream.ToArray();
            stream.Close();

            return data;
        }

        /// <summary>
        /// 将二进制数据反序列化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object DeserializeWithBinary(byte[] data)
        {
            if(data.Length == 0)
            {
                return string.Empty;
            }
            MemoryStream stream = new MemoryStream();
            stream.Write(data, 0, data.Length);
            stream.Position = 0;
            BinaryFormatter bf = new BinaryFormatter();
            object obj = bf.Deserialize(stream);

            stream.Close();

            return obj;
        }


        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public void ReadMessage(out NetCmd cmd, out byte[] data)   //读取一条消息，调用一次读一条 
        {
            cmd = NetCmd.RAWSTRING;
            data = null;
            if (msgs.Count == 0) return;
            byte[] msgdata = msgs.Dequeue();
            NetCmd netCmd = (NetCmd)BitConverter.ToInt32(msgdata, 0);
            if(netCmd >= NetCmd.NetCmdMin && netCmd <= NetCmd.NetCmdMax)
            {
                cmd = netCmd;
                data = msgdata.Skip(4).ToArray();
            }
        }

        /// <summary>
        /// 打包数据
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] PackData(NetCmd cmd, byte[] data)   //打包数据
        {
            /* [0...3] + [4...7] + [8...]
             * Length  + NetCmd  + data
             * */
            byte[] CmdByte = BitConverter.GetBytes((UInt32)cmd);
            byte[] dataBytes = data;
            int Length = CmdByte.Length + dataBytes.Length;
            byte[] LengthBytes = BitConverter.GetBytes(Length);
            byte[] PackBytes = LengthBytes.Concat(CmdByte).ToArray<byte>();//Concat(dataBytes);
            return PackBytes.Concat(dataBytes).ToArray<byte>();
        }

        private void _ResolutionMsg()
        {
            while (true)
            {
                if (currentPos - processPos <= 4)
                {
                    return;
                }
                int count = BitConverter.ToInt32(buffer, processPos);
                if ((currentPos - processPos) >= count + 4 && processPos + 4 + count < buffer.Length)  //接收完整，加入消息列表
                {
                    msgs.Enqueue(buffer.Skip(processPos + 4).Take(count).ToArray());
                    processPos += (count + 4);      //处理指针后移
                    if(processPos == currentPos)    //消息全部处理完毕，初始化缓存
                    {
                        _ResetBuffer();
                    }
                }
                else
                {
                    break;  //接收不完整，等待下一次调用再做处理
                }
            }
        }

        private void _ResetBuffer()
        {
            buffer = new byte[buffer_init_size];
            currentPos = 0;
            processPos = 0;
        }
    }
}
