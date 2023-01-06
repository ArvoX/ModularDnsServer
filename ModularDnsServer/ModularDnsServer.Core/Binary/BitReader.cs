using System;
using System.Text;

namespace ModularDnsServer.Core.Binary;

public static class BitReader
{
  public static ushort ToUInt16(this IList<byte> buffer, ref int offset)
  {
    return buffer[offset++].ToUInt16(buffer[offset++]);
  }

  public static ushort ToUInt16(this byte first, byte last)
  {
    return (ushort)((first << 8) + last);
  }
  public static short ToInt16(this IList<byte> buffer, ref int offset)
  {
    return buffer[offset++].ToInt16(buffer[offset++]);
  }

  public static short ToInt16(this byte first, byte last)
  {
    return (short)((first << 8) + last);
  }

  public static uint ToUInt32(this IList<byte> buffer, ref int offset)
  {
    return buffer[offset++].ToUInt32(buffer[offset++], buffer[offset++], buffer[offset++]);
  }

  public static uint ToUInt32(this byte first, byte second, byte third, byte last)
  {
    return (uint)((first << 24) + (second << 16) + (third << 8) + last);
  }

  public static int ToInt32(this IList<byte> buffer, ref int offset)
  {
    return buffer[offset++].ToInt32(buffer[offset++], buffer[offset++], buffer[offset++]);
  }

  public static int ToInt32(this byte first, byte second, byte third, byte last)
  {
    return (first << 24) + (second << 16) + (third << 8) + last;
  }

  public static byte[] ToArray(this byte[] buffer, ref int offset, int length)
  {
    var result = new byte[length];
    Array.Copy(buffer, offset, result, 0, length);
    offset += length;
    return result;
  }

  public static string ToString(this byte[] buffer, ref int offset)
  {
    byte length = buffer[offset++];
    string result = Encoding.ASCII.GetString(buffer, offset, length);
    offset += length;
    return result;
  }
  public static string[] ToStrings(this byte[] buffer, ref int offset, int length)
  {
    var result = new List<string>();
    var end = offset + length;
    while (offset < end)
      result.Add(ToString(buffer, ref offset));
    return result.ToArray();
  }
}

public static class BitWriter
{
  public static byte[] ToBytes(this ushort value)
  {
    return new[] { (byte)(value >> 8), (byte)value };
  }
}
