using System;
using System.Collections.Generic;

namespace TinyBinaryXml
{
	public class TbXmlNodeTemplate
	{
		public ushort id;

		public string name = string.Empty;

		public List<string> attributeNames;

		public Dictionary<string, int> attributeNameIndexMapping;

		public List<TB_XML_ATTRIBUTE_TYPE> attributeTypes;
	}
}
