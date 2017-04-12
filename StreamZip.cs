using SevenZip;
using SevenZip.Compression.LZMA;
using System;
using System.Collections.Generic;
using System.IO;

public class StreamZip
{
	public static void MultiZip(List<string> filePaths, BinaryWriter outStream)
	{
		for (int i = 0; i < filePaths.Count; i++)
		{
			MemoryStream memoryStream = new MemoryStream();
			FileStream inStream = File.Open(filePaths[i], FileMode.Open);
			StreamZip.Zip(inStream, memoryStream);
			byte[] array = memoryStream.ToArray();
			string value = filePaths[i].Substring(filePaths[i].LastIndexOf("\\") + 1);
			outStream.Write(value);
			outStream.Write(array.Length);
			outStream.Write(array);
		}
	}

	public static void MultiUnzip(byte[] zipbytes, string dir)
	{
		BinaryReader binaryReader = new BinaryReader(new MemoryStream(zipbytes));
		while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
		{
			string str = binaryReader.ReadString();
			int count = binaryReader.ReadInt32();
			byte[] array = binaryReader.ReadBytes(count);
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.Write(array, 0, array.Length);
			memoryStream.Position = 0L;
			MemoryStream memoryStream2 = new MemoryStream();
			StreamZip.Unzip(memoryStream, memoryStream2);
			if (!QFileUtils.ExistsDir(dir))
			{
				QFileUtils.CreateDir(dir);
			}
			string fileName = dir + "//" + str;
			if (!QFileUtils.Exists(fileName))
			{
				QFileUtils.CreateFile(fileName);
			}
			QFileUtils.WriteBytes(fileName, memoryStream2.ToArray());
		}
	}

	public static void Zip(Stream inStream, Stream outStream)
	{
		bool flag = false;
		int num = 2097152;
		if (!flag)
		{
			num = 8388608;
		}
		int num2 = 2;
		int num3 = 3;
		int num4 = 0;
		int num5 = 2;
		int num6 = 128;
		string text = "bt4";
		bool flag2 = false;
		CoderPropID[] propIDs = new CoderPropID[]
		{
			CoderPropID.DictionarySize,
			CoderPropID.PosStateBits,
			CoderPropID.LitContextBits,
			CoderPropID.LitPosBits,
			CoderPropID.Algorithm,
			CoderPropID.NumFastBytes,
			CoderPropID.MatchFinder,
			CoderPropID.EndMarker
		};
		object[] properties = new object[]
		{
			num,
			num2,
			num3,
			num4,
			num5,
			num6,
			text,
			flag2
		};
		Encoder encoder = new Encoder();
		encoder.SetCoderProperties(propIDs, properties);
		encoder.WriteCoderProperties(outStream);
		long length = inStream.Length;
		for (int i = 0; i < 8; i++)
		{
			outStream.WriteByte((byte)(length >> 8 * i));
		}
		encoder.Code(inStream, outStream, -1L, -1L, null);
	}

	public static void Unzip(Stream inStream, Stream outStream)
	{
		byte[] array = new byte[5];
		if (inStream.Read(array, 0, 5) != 5)
		{
			throw new Exception("input .lzma is too short");
		}
		Decoder decoder = new Decoder();
		decoder.SetDecoderProperties(array);
		long num = 0L;
		for (int i = 0; i < 8; i++)
		{
			int num2 = inStream.ReadByte();
			if (num2 < 0)
			{
				throw new Exception("Can't Read 1");
			}
			num |= (long)((byte)num2) << 8 * i;
		}
		long inSize = inStream.Length - inStream.Position;
		decoder.Code(inStream, outStream, inSize, num, null);
	}
}
