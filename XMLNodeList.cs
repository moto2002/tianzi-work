using System;
using System.Collections;

public class XMLNodeList : ArrayList
{
	public XMLNode Pop()
	{
		XMLNode xMLNode = null;
		if (this.Count > 0)
		{
			xMLNode = (XMLNode)this[this.Count - 1];
			this.Remove(xMLNode);
		}
		return xMLNode;
	}

	public int Push(XMLNode item)
	{
		this.Add(item);
		return this.Count;
	}
}
