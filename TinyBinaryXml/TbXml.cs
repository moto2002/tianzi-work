using System;
using System.Collections.Generic;

namespace TinyBinaryXml
{
	public class TbXml
	{
		public List<TbXmlNodeTemplate> nodeTemplates;

		public List<TbXmlNode> nodes;

		public List<string> stringPool;

		public List<double> valuePool;

		public TbXmlNode docNode;

		public static TbXml Load(byte[] xmlBytes)
		{
			TbXmlDeserializer tbXmlDeserializer = new TbXmlDeserializer();
			return tbXmlDeserializer.DeserializeXmlBytes(xmlBytes);
		}
	}
}
