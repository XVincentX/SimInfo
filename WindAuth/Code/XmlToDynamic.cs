using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace WindAuth.Code
{
    public class XmlToDynamic
    {

        public static void Parse(dynamic parent, XElement node)
        {

            if (node.HasElements)
            {
                var localName = node.Elements().First().Name.LocalName;

                if (node.Elements(localName).Count() > 1)
                {
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
                    foreach (var attribute in node.Attributes())
                    {
                        AddProperty(item, attribute.Name.ToString(), attribute.Value.Trim());
                    }

                    //element

                    foreach (var element in node.Elements().GroupBy(x => x.Name.LocalName))
                    {
                        if (element.Count() > 1)
                        {
                            var list = new List<dynamic>();

                            foreach (var el in element)
                            {
                                Parse(list, el);
                            }

                            AddProperty(item, element.Key, list);
                            AddProperty(parent, node.Name.ToString(), item);
                        }
                        else
                            Parse(item, element.First());
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

            if (parent is List<dynamic>)
            {

                (parent as List<dynamic>).Add(value);

            }

            else
            {

                (parent as IDictionary<String, object>)[name] = value;

            }

        }

    }

}