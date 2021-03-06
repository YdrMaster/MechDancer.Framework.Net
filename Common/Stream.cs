using System;
using System.IO;
using System.Text;

namespace MechDancer.Common {
    public static partial class Extensions {
        /// <summary>
        /// 按范围拷贝数组
        /// </summary>
        /// <param name="receiver">原数组</param>
        /// <param name="begin">起始坐标（包括）</param>
        /// <param name="end">结束坐标（不包括）</param>
        /// <typeparam name="T">数组类型，引用类型浅拷贝</typeparam>
        /// <returns>新数组</returns>
        public static T[] CopyRange<T>(
            this T[] receiver,
            int begin = 0,
            int end = int.MaxValue
        ) => new T[Math.Min(end, receiver.Length) - begin]
           .Also(it => Array.Copy(receiver, begin, it, 0, it.Length));

        /// <summary>
        /// 向流写入一个字符数组
        /// </summary>
        /// <param name="receiver">流</param>
        /// <param name="bytes">字符数组</param>
        /// <returns>流</returns>
        public static void Write(this Stream receiver, byte[] bytes)
            => receiver.Write(bytes, 0, bytes.Length);

        /// <summary>
        /// 把一个字节数组按相反的顺序写入流
        /// </summary>
        /// <param name="receiver">目标流</param>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">起始位置</param>
        /// <param name="length">写入的长度</param>
        /// <returns>所在流</returns>
        public static void WriteReversed(
            this Stream receiver,
            byte[] bytes,
            int index = 0,
            int length = int.MaxValue
        ) {
            index += Math.Min(length, bytes.Length - index);
            while (index-- > 0) receiver.WriteByte(bytes[index]);
        }

        /// <summary>
        /// 从输入流阻塞接收 n 个字节数据，或直到流关闭。
        /// </summary>
        /// <param name="receiver">输入流</param>
        /// <param name="n">数量</param>
        /// <returns>缓冲内存块</returns>
        /// <remarks>
        /// 函数会直接打开等于目标长度的缓冲区，因此不要用于实现尽量读取的功能。
        /// </remarks>
        public static byte[] WaitNBytes(this Stream receiver, int n) {
            using (var buffer = new MemoryStream(n)) {
                for (var i = 0; i < n; ++i) {
                    var temp = receiver.ReadByte();
                    if (temp > 0)
                        buffer.WriteByte((byte)temp);
                    else
                        return buffer.GetBuffer().CopyRange(0, i);
                }

                return buffer.GetBuffer();
            }
        }
           

		/// <summary>
		///     从输入流阻塞接收 n 个字节数据，并将数组按相反的方向读出。
		/// </summary>
		/// <param name="receiver">输入流</param>
		/// <param name="n">数量</param>
		/// <returns>内存块</returns>
		public static byte[] WaitReversed(this Stream receiver, uint n) {
			var buffer = new byte[n];

			while (n-- > 0) {
				var temp = receiver.ReadByte();
				if (temp >= 0)
					buffer[n] = (byte) temp;
				else
					return buffer.CopyRange((int) n + 1, buffer.Length);
			}

			return buffer;
		}

        /// <summary>
        ///     从流中读取所有数据
        /// </summary>
        /// <param name="receiver">字节流</param>
        /// <returns>剩余数据</returns>
        public static byte[] ReadRest(this Stream receiver) {
            using (var buffer = new MemoryStream()) {
                while (true) {
                    var b = receiver.ReadByte();
                    if (b == -1) return buffer.ToArray();
                    buffer.WriteByte((byte)b);
                }
            }
        }

		/// <summary>
		///     计算内存流剩余空间
		/// </summary>
		/// <param name="receiver">内存流</param>
		/// <returns>剩余空间长度</returns>
		public static long Available(this MemoryStream receiver)
			=> receiver.Capacity - receiver.Position;

		/// <summary>
		///     向流中写入字符串，再写入结尾
		/// </summary>
		/// <param name="receiver">流</param>
		/// <param name="text">字符串</param>
		/// <returns>流</returns>
		public static void WriteEnd(this Stream receiver, string text) {
			receiver.Write(text.GetBytes());
			receiver.WriteByte(0);
		}

        /// <summary>
        ///     从流读取一个带结尾的字符串
        /// </summary>
        /// <param name="receiver">流</param>
        /// <returns>字符串</returns>
        public static string ReadEnd(this Stream receiver) {
            using (var buffer = new MemoryStream(1)) {
                while (true) {
                    var b = receiver.ReadByte();
                    switch (b) {
                        case -1:
                        case 0:
                            return buffer.ToArray().GetString();
                        default:
                            buffer.WriteByte((byte)b);
                            break;
                    }
                }
            }
        }

		#region String Encode

		/// <summary>
		///     字节数组转字符串
		/// </summary>
		/// <param name="receiver">字节数组</param>
		/// <param name="encoding">字符串编码</param>
		/// <returns>字符串</returns>
		public static string GetString(this byte[] receiver, Encoding encoding = null) =>
			(encoding ?? Encoding.Default).GetString(receiver);

		/// <summary>
		///     字符串转字节数组
		/// </summary>
		/// <param name="receiver">字符串</param>
		/// <param name="encoding">字符串编码</param>
		/// <returns>字节数组</returns>
		public static byte[] GetBytes(this string receiver, Encoding encoding = null) =>
			(encoding ?? Encoding.Default).GetBytes(receiver);

		#endregion
	}
}
