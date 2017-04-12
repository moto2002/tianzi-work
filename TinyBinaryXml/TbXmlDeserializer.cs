using System;
using System.Collections.Generic;
using System.IO;

namespace TinyBinaryXml
{
	public class TbXmlDeserializer
	{
		public TbXml DeserializeXmlBytes(byte[] xmlBytes)
		{
			if (xmlBytes == null || xmlBytes.Length == 0)
			{
				return null;
			}
			TbXml tbXml = new TbXml();
			using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(xmlBytes)))
			{
				ushort num = binaryReader.ReadUInt16();
				tbXml.nodeTemplates = new List<TbXmlNodeTemplate>((int)num);
				for (ushort num2 = 0; num2 < num; num2 += 1)
				{
					this.DeserializeNodeTemplate(binaryReader, num2, tbXml);
				}
				ushort num3 = binaryReader.ReadUInt16();
				tbXml.nodes = new List<TbXmlNode>((int)num3);
				for (ushort num4 = 0; num4 < num3; num4 += 1)
				{
					this.DeserializeNode(binaryReader, num4, tbXml);
					tbXml.nodes[(int)num4].tbXml = tbXml;
				}
				this.DeserializeStringPool(binaryReader, tbXml);
				this.DeserializeValuePool(binaryReader, tbXml);
				tbXml.docNode = new TbXmlNode();
				tbXml.docNode.childrenIds = new List<ushort>();
				tbXml.docNode.childrenIds.Add(0);
				tbXml.docNode.tbXml = tbXml;
				binaryReader.BaseStream.Close();
				binaryReader.BaseStream.Dispose();
				binaryReader.Close();
			}
			return tbXml;
		}

		private void DeserializeStringPool(BinaryReader binaryReader, TbXml tbXml)
		{
			int num = binaryReader.ReadInt32();
			tbXml.stringPool = new List<string>(num);
			for (int i = 0; i < num; i++)
			{
				tbXml.stringPool.Add(binaryReader.ReadString());
			}
		}

		private void DeserializeValuePool(BinaryReader binaryReader, TbXml tbXml)
		{
			int num = binaryReader.ReadInt32();
			tbXml.valuePool = new List<double>(num);
			for (int i = 0; i < num; i++)
			{
				tbXml.valuePool.Add(binaryReader.ReadDouble());
			}
		}

		private void DeserializeNodeTemplate(BinaryReader binaryReader, ushort index, TbXml tbXml)
		{
			TbXmlNodeTemplate tbXmlNodeTemplate = new TbXmlNodeTemplate();
			tbXml.nodeTemplates.Add(tbXmlNodeTemplate);
			tbXmlNodeTemplate.id = binaryReader.ReadUInt16();
			tbXmlNodeTemplate.name = binaryReader.ReadString();
			ushort num = binaryReader.ReadUInt16();
			if (num > 0)
			{
				tbXmlNodeTemplate.attributeNames = new List<string>((int)num);
				tbXmlNodeTemplate.attributeNameIndexMapping = new Dictionary<string, int>((int)num);
				for (int i = 0; i < (int)num; i++)
				{
					string text = binaryReader.ReadString();
					tbXmlNodeTemplate.attributeNames.Add(text);
					tbXmlNodeTemplate.attributeNameIndexMapping[text] = i;
				}
				tbXmlNodeTemplate.attributeTypes = new List<TB_XML_ATTRIBUTE_TYPE>((int)num);
				for (int j = 0; j < (int)num; j++)
				{
					tbXmlNodeTemplate.attributeTypes.Add((TB_XML_ATTRIBUTE_TYPE)binaryReader.ReadByte());
				}
			}
		}

		private void DeserializeNode(BinaryReader binaryReader, ushort index, TbXml tbXml)
		{
			TbXmlNode tbXmlNode = new TbXmlNode();
			tbXml.nodes.Add(tbXmlNode);
			tbXmlNode.id = binaryReader.ReadUInt16();
			tbXmlNode.templateId = binaryReader.ReadUInt16();
			ushort num = binaryReader.ReadUInt16();
			if (num > 0)
			{
				tbXmlNode.childrenIds = new List<ushort>((int)num);
				for (int i = 0; i < (int)num; i++)
				{
					tbXmlNode.childrenIds.Add(binaryReader.ReadUInt16());
				}
			}
			TbXmlNodeTemplate tbXmlNodeTemplate = tbXml.nodeTemplates[(int)tbXmlNode.templateId];
			ushort num2 = (ushort)((tbXmlNodeTemplate.attributeNames != null) ? tbXmlNodeTemplate.attributeNames.Count : 0);
			if (num2 > 0)
			{
				tbXmlNode.attributeValues = new List<int>((int)num2);
				for (ushort num3 = 0; num3 < num2; num3 += 1)
				{
					tbXmlNode.attributeValues.Add(binaryReader.ReadInt32());
				}
			}
			byte b = binaryReader.ReadByte();
			if (b == 1)
			{
				tbXmlNode.text = binaryReader.ReadInt32();
			}
		}
	}
}
