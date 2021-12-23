using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml.Linq;

namespace Husa.Core.Uploaders.Houston
{
    internal static class XmlToDynamic
    {
        public static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            if (xmlDocument.HasElements)
                return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(RemoveAllNamespaces));
            
            var xElement = new XElement(xmlDocument.Name.LocalName) {Value = xmlDocument.Value};

            foreach (var attribute in xmlDocument.Attributes())
                xElement.Add(attribute);

            return xElement;
        }

        public static void Parse(dynamic parent, XElement node)
        {
            if (node.HasElements)
            {
                if (node.Elements(node.Elements().First().Name.LocalName).Count() > 1)
                {
                    //list
                    var item = new ExpandoObject();
                    var list = new List<dynamic>();
                    foreach (var element in node.Elements())
                    {
                        Parse(list, element);
                    }

                    AddProperty(item, node.Elements().First().Name.LocalName, list);
                    AddProperty(parent, node.Name.ToString(), item);
                }
                else
                {
                    var item = new ExpandoObject();

                    foreach (var element in node.Elements())
                    {
                        Parse(item, element);
                    }

                    AddProperty(parent, node.Name.ToString(), item);
                }
            }
            else
            {
                AddProperty(parent, node.Name.ToString(), node.Value.Trim());
            }
        }

        private static void AddProperty(dynamic parent, string name, object value)
        {
            var list = parent as List<dynamic>;
            if (list != null)
            {
                list.Add(value);
            }
            else
            {
                ((IDictionary<string, object>)parent)[name] = value;
            }
        }
    }
}
