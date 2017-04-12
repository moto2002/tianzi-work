using ICSharpCode.SharpZipLib.GZip;
using System;
using System.IO;
using System.Text;

public class ByteUtils
{
	public static byte[] compressString(string input)
	{
		if (input == null)
		{
			return null;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(input);
		return ByteUtils.compress(bytes);
	}

	public static string decompressString(byte[] input)
	{
		if (input == null)
		{
			return null;
		}
		byte[] data = ByteUtils.decompress(input);
		return ByteUtils.byteConverString(data);
	}

	public static byte[] compress(byte[] data)
	{
		MemoryStream memoryStream = new MemoryStream();
		GZipOutputStream gZipOutputStream = new GZipOutputStream(memoryStream);
		gZipOutputStream.Write(data, 0, data.Length);
		gZipOutputStream.Flush();
		gZipOutputStream.Finish();
		gZipOutputStream.Close();
		return memoryStream.ToArray();
	}

	public static byte[] decompress(byte[] data)
	{
		MemoryStream baseInputStream = new MemoryStream(data);
		GZipInputStream gZipInputStream = new GZipInputStream(baseInputStream);
		MemoryStream memoryStream = new MemoryStream();
		byte[] array = new byte[1024];
		while (true)
		{
			int num = gZipInputStream.Read(array, 0, array.Length);
			if (num <= 0)
			{
				break;
			}
			memoryStream.Write(array, 0, num);
		}
		gZipInputStream.Close();
		return memoryStream.ToArray();
	}

	public static string byteConverString(byte[] data)
	{
		Decoder decoder = Encoding.UTF8.GetDecoder();
		int charCount = decoder.GetCharCount(data, 0, data.Length);
		char[] array = new char[charCount];
		int chars = decoder.GetChars(data, 0, data.Length, array, 0);
		return new string(array, 0, chars);
	}

	public static string byteConverString(byte[] data, int index, int count)
	{
		Decoder decoder = Encoding.UTF8.GetDecoder();
		int charCount = decoder.GetCharCount(data, index, count);
		char[] array = new char[charCount];
		int chars = decoder.GetChars(data, index, count, array, 0);
		return new string(array, 0, chars);
	}
}
