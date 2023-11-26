using System.Xml;
using System.Xml.Linq;
using FCMicroservices.Components;
using Newtonsoft.Json;

namespace FCMicroservices.Extensions;

public static class XmlExtensions
    {
        public static string XmlToJson(this string value)
        {
            if (!string.IsNullOrEmpty(value) && value.TrimStart().StartsWith("<"))
            {
                try
                {
                    var xmlResponse = new XmlDocument();
                    xmlResponse.LoadXml(value);
                    var doc = ConvertToXDocument(xmlResponse);
                    if (doc.Root != null) RemoveNamespacePrefix(doc.Root);
                    value = JsonConvert.SerializeXNode(doc);
                }
                catch (Exception ex)
                {
                    throw new ApiException("Xml parse edilemedi.", new { value });
                }
            }

            return value;
        }

        public static XDocument ConvertToXDocument(XmlDocument input)
        {
            using (var reader = new XmlNodeReader(input))
            {
                reader.MoveToContent();
                return XDocument.Load(reader);
            }
        }

        static void RemoveNamespacePrefix(XElement element)
        {
            //Remove from element
            if (element.Name.Namespace != null)
                element.Name = element.Name.LocalName;

            //Remove from attributes
            var attributes = element.Attributes().ToArray();
            element.RemoveAttributes();
            foreach (var attr in attributes)
            {
                var newAttr = attr;

                if (attr.Name.Namespace != null)
                    newAttr = new XAttribute(attr.Name.LocalName, attr.Value);

                element.Add(newAttr);
            };

            //Remove from children
            foreach (var child in element.Descendants())
                RemoveNamespacePrefix(child);
        }

        private static void RemoveAllAttributes(XmlNode node)
        {
            if (node.Attributes is not null)
                node.Attributes.RemoveAll();

            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Attributes is not null)
                    childNode.Attributes.RemoveAll();

                if (childNode.HasChildNodes)
                    RemoveAllAttributes(childNode);
            }
        }

    }