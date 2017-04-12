using System;
using System.Collections.Generic;
using UnityEngine;

namespace TinyBinaryXml
{
	public class TbXmlNode
	{
		public ushort id;

		public List<ushort> childrenIds;

		public ushort templateId;

		public List<int> attributeValues;

		public TbXml tbXml;

		public int text = -1;

		public string GetText()
		{
			if (this.text == -1)
			{
				return string.Empty;
			}
			return this.tbXml.stringPool[this.text];
		}

		public string GetStringValue(string name)
		{
			object value = this.GetValue(ref name);
			if (value == null)
			{
				return string.Empty;
			}
			if (value is double)
			{
				return value.ToString();
			}
			return value as string;
		}

		public double GetDoubleValue(string name)
		{
			object value = this.GetValue(ref name);
			if (value is double)
			{
				return (double)value;
			}
			return 0.0;
		}

		public float GetFloatValue(string name, float defaultValue = 0f)
		{
			object obj = this.GetValue(ref name);
			if (obj is double)
			{
				return (float)((double)obj);
			}
			string text = obj as string;
			if (string.IsNullOrEmpty(text))
			{
				return defaultValue;
			}
			obj = text.Trim();
			float result;
			if (float.TryParse(text, out result))
			{
				return result;
			}
			return defaultValue;
		}

		public int GetIntValue(string name, int defaultValue = 0)
		{
			object obj = this.GetValue(ref name);
			if (obj is double)
			{
				return (int)((double)obj);
			}
			string text = obj as string;
			if (string.IsNullOrEmpty(text))
			{
				return defaultValue;
			}
			obj = text.Trim();
			int result;
			if (int.TryParse(text, out result))
			{
				return result;
			}
			return defaultValue;
		}

		public uint GetUIntValue(string name)
		{
			object value = this.GetValue(ref name);
			if (value is double)
			{
				return (uint)((double)value);
			}
			return 0u;
		}

		public byte GetByteValue(string name)
		{
			object value = this.GetValue(ref name);
			if (value is double)
			{
				return (byte)((double)value);
			}
			return 0;
		}

		public ushort GetUShortValue(string name)
		{
			object value = this.GetValue(ref name);
			if (value is double)
			{
				return (ushort)((double)value);
			}
			return 0;
		}

		public short GetShortValue(string name)
		{
			object value = this.GetValue(ref name);
			if (value is double)
			{
				return (short)((double)value);
			}
			return 0;
		}

		public bool GetBooleanValue(string name)
		{
			object value = this.GetValue(ref name);
			if (value == null)
			{
				return false;
			}
			if (value is double)
			{
				return !Mathf.Approximately(0f, (float)((double)value));
			}
			return value.ToString() == "true";
		}

		public object GetValue(ref string name)
		{
			TbXmlNodeTemplate tbXmlNodeTemplate = this.tbXml.nodeTemplates[(int)this.templateId];
			int index;
			if (!tbXmlNodeTemplate.attributeNameIndexMapping.TryGetValue(name, out index))
			{
				return null;
			}
			if (tbXmlNodeTemplate.attributeTypes[index] == TB_XML_ATTRIBUTE_TYPE.DOUBLE)
			{
				return this.tbXml.valuePool[this.attributeValues[index]];
			}
			return this.tbXml.stringPool[this.attributeValues[index]];
		}

		public List<TbXmlNode> GetNodes(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}
			List<TbXmlNode> result = null;
			int num = (this.childrenIds != null) ? this.childrenIds.Count : 0;
			string[] array = path.Split(new char[]
			{
				'/'
			});
			for (int i = 0; i < num; i++)
			{
				TbXmlNode currentNode = this.tbXml.nodes[(int)this.childrenIds[i]];
				this.GetNodesRecursive(array, 0, ref array[0], currentNode, ref result);
			}
			return result;
		}

		private void GetNodesRecursive(string[] pathBlocks, int pathBlockIndex, ref string pathBlock, TbXmlNode currentNode, ref List<TbXmlNode> resultNodes)
		{
			if (this.tbXml.nodeTemplates[(int)currentNode.templateId].name.Equals(pathBlock))
			{
				if (pathBlockIndex == pathBlocks.Length - 1)
				{
					if (resultNodes == null)
					{
						resultNodes = new List<TbXmlNode>();
					}
					resultNodes.Add(currentNode);
				}
				else
				{
					List<ushort> list = currentNode.childrenIds;
					int num = (list != null) ? list.Count : 0;
					for (int i = 0; i < num; i++)
					{
						this.GetNodesRecursive(pathBlocks, pathBlockIndex + 1, ref pathBlocks[pathBlockIndex + 1], this.tbXml.nodes[(int)list[i]], ref resultNodes);
					}
				}
			}
		}
	}
}
