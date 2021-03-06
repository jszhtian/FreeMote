﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

// ReSharper disable InconsistentNaming

namespace FreeMote
{
    public static class PsbConstants
    {
        /// <summary>
        /// 0x075BCD15
        /// </summary>
        public const uint Key1 = 123456789;
        /// <summary>
        /// 0x159A55E5
        /// </summary>
        public const uint Key2 = 362436069;
        /// <summary>
        /// 0x1F123BB5
        /// </summary>
        public const uint Key3 = 521288629;

        public static string ToStringForPsb(this PsbPixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PsbPixelFormat.None:
                case PsbPixelFormat.WinRGBA8:
                case PsbPixelFormat.CommonRGBA8:
                    return "RGBA8";
                case PsbPixelFormat.DXT5:
                    return "DXT5";
                case PsbPixelFormat.WinRGBA4444:
                    return "RGBA4444";
                default:
                    throw new ArgumentOutOfRangeException(nameof(pixelFormat), pixelFormat, null);
            }
        }

        /// <summary>
        /// Read a <see cref="uint"/> from <see cref="BinaryReader"/>, and then encode using <see cref="PsbStreamContext"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="br"></param>
        /// <returns></returns>
        public static uint ReadUInt32(this PsbStreamContext context, BinaryReader br)
        {
            return BitConverter.ToUInt32(context.Encode(br.ReadBytes(4)), 0);
        }

        /// <summary>
        /// Read bytes from <see cref="BinaryReader"/>, and then encode using <see cref="PsbStreamContext"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="br"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static byte[] ReadBytes(this PsbStreamContext context, BinaryReader br, int count)
        {
            return context.Encode(br.ReadBytes(count));
        }

        /// <summary>
        /// Read a <see cref="ushort"/> from <see cref="BinaryReader"/>, and then encode using <see cref="PsbStreamContext"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="br"></param>
        /// <returns></returns>
        public static ushort ReadUInt16(this PsbStreamContext context, BinaryReader br)
        {
            return BitConverter.ToUInt16(context.Encode(br.ReadBytes(2)), 0);
        }

        /// <summary>
        /// Encode a value and write using <see cref="BinaryWriter"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <param name="bw"></param>
        public static void Write(this PsbStreamContext context, uint value, BinaryWriter bw)
        {
            bw.Write(context.Encode(BitConverter.GetBytes(value)));
        }

        /// <summary>
        /// Encode a value and write using <see cref="BinaryWriter"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <param name="bw"></param>
        public static void Write(this PsbStreamContext context, ushort value, BinaryWriter bw)
        {
            bw.Write(context.Encode(BitConverter.GetBytes(value)));
        }

        public static string ReadStringZeroTrim(this BinaryReader br)
        {
            StringBuilder sb = new StringBuilder();
            while (br.PeekChar() != 0)
            {
                sb.Append(br.ReadChar());
            }
            return sb.ToString();
        }

        public static void WriteStringZeroTrim(this BinaryWriter bw, string str)
        {
            bw.Write(str.ToCharArray());
            bw.Write((byte)0);
        }

        /// <summary>
        /// Big-Endian Write
        /// </summary>
        /// <param name="bw"></param>
        /// <param name="num"></param>
        public static void WriteBE(this BinaryWriter bw, uint num)
        {
            bw.Write(BitConverter.GetBytes(num).Reverse().ToArray());
        }

        public static void Pad(this BinaryWriter bw, int length, byte paddingByte = 0x0)
        {
            if (length <= 0)
            {
                return;
            }

            if (paddingByte == 0x0)
            {
                bw.Write(new byte[length]);
                return;
            }

            for (int i = 0; i < length; i++)
            {
                bw.Write(paddingByte);
            }
        }
    }

    //REF: https://stackoverflow.com/a/24987840/4374462
    public static class ListExtras
    {
        //    list: List<T> to resize
        //    size: desired new size
        // element: default value to insert

        public static void Resize<T>(this List<T> list, int size, T element = default(T))
        {
            int count = list.Count;

            if (size < count)
            {
                list.RemoveRange(size, count - size);
            }
            else if (size > count)
            {
                if (size > list.Capacity)   // Optimization
                    list.Capacity = size;

                list.AddRange(Enumerable.Repeat(element, size - count));
            }
        }

        public static void EnsureSize<T>(this List<T> list, int size, T element = default(T))
        {
            if (list.Count < size)
            {
                list.Resize(size, element);
            }
        }
        public static void Set<T>(this List<T> list, int index, T value, T defaultValue = default(T))
        {
            if (list.Count < index + 1)
            {
                list.Resize(index + 1, defaultValue);
            }

            list[index] = value;
        }
    }
}
