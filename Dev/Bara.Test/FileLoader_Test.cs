using Bara.Common;
using Bara.Model;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading;
using Xunit;
using System.Xml.Serialization;
using System.Xml;
using Bara.Abstract.Core;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using Bara.Core.Config;

namespace Bara.Test
{
    public class FileLoader_Test
    {
        [Fact]
        public void FileLoaderTest()
        {
            var fileInfo = FileLoader.GetFileInfo(@"E:\BaraMapConfig.xml");
            var fileStream = FileLoader.Load(@"E:\BaraMapConfig.xml");
            Trace.WriteLine("ok");
        }

        [Fact]
        public void FileWatchTest()
        {
            int ChangeTimes = 0;
            var fileInfo = FileLoader.GetFileInfo(@"E:\BaraMapConfig.xml");
            FileWatcherLoader.Instance.Watch(fileInfo, () =>
            {
                Trace.WriteLine("File Change Times:" + ChangeTimes);
                ChangeTimes++;
            });

            FileWatcherLoader.Instance.Clear();
            Thread.Sleep(10000);
            Trace.WriteLine("Test OK");
        }

        [Fact]
        public void ConfigDeserizeTest()
        {
            //var fileInfo = FileLoader.GetFileInfo(@"E:\BaraMapConfig.xml");
            //var fileStream = FileLoader.Load(@"E:\BaraMapConfig.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(BaraMapConfig));
            BaraMapConfig config = null;
            using (var configStream = FileLoader.Load("BaraMapConfig.xml"))
            {
                config = serializer.Deserialize(configStream) as BaraMapConfig;
                config.BaraMaps = new List<BaraMap> { };
            }

            foreach (var baramap in config.BaraMapSources)
            {
                LoadBaraMap(baramap.Path, config);
            }

            Trace.WriteLine("OK");
            //  BaraMapConfig config = JsonConvert.DeserializeObject<BaraMapConfig>(fileStream);
        }

        [Fact]
        public void XmlFileDeserize()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BaraMap));
            BaraMap config = null;
            using (var configStream = FileLoader.Load(@"Maps\T_Test.xml"))
            {
                config = serializer.Deserialize(configStream) as BaraMap;
            }

        }

        [Fact]
        public void LocalConfigLoaderTest()
        {
            LocalConfigLoader loader = new LocalConfigLoader(null);

            // IBaraMapper baraMapper;
            var config = loader.Load("BaraMapConfig.xml", null);

        }
        [Fact, Trait("Category", "A")]
        public void XmlLoaderTest()
        {
            XDocument doc = XDocument.Load(@"Maps\T_Test.xml");
            XElement xele = doc.Root;
            XNamespace ns = xele.GetDefaultNamespace();
            //XElement xStatements =new XElement(ns+"Statements");
            foreach (var item in xele.Descendants(ns + "Statement"))
            {
                Trace.WriteLine(item.Name);

            }


            //IEnumerable<XElement> elements = from ele in doc.Descendants()
            //                                 select ele;

            //var Scope = elements.Attributes("Scope");
            //IEnumerable<XElement> Statements = xele.Elements();
            //foreach (var item in Statements)
            //{
            //    Trace.WriteLine(item.Name);
            //}

            //  Trace.WriteLine(Scope);

        }

        [Fact]
        public void XRead_Test()
        {
            XmlReader xmlRdr = XmlReader.Create(@"Maps\T_Test.XML");
            while (xmlRdr.Read())
            {
                switch (xmlRdr.NodeType)
                {
                    case XmlNodeType.Attribute:
                        break;
                    case XmlNodeType.CDATA:
                        break;
                    case XmlNodeType.Comment:
                        break;
                    case XmlNodeType.Document:
                        break;
                    case XmlNodeType.DocumentFragment:
                        break;
                    case XmlNodeType.DocumentType:
                        break;
                    case XmlNodeType.Element:
                        {

                            var result = xmlRdr.Read();

                            break;
                        }
                    case XmlNodeType.EndElement:
                        break;
                    case XmlNodeType.EndEntity:
                        break;
                    case XmlNodeType.Entity:
                        break;
                    case XmlNodeType.EntityReference:
                        break;
                    case XmlNodeType.None:
                        break;
                    case XmlNodeType.Notation:
                        break;
                    case XmlNodeType.ProcessingInstruction:
                        break;
                    case XmlNodeType.SignificantWhitespace:
                        break;
                    case XmlNodeType.Text:
                        break;
                    case XmlNodeType.Whitespace:
                        break;
                    case XmlNodeType.XmlDeclaration:
                        break;
                    default:
                        break;
                }
            }
        }

        [Fact]
        public void ConfigLoader()
        {
            LocalConfigLoader loader = new LocalConfigLoader(null);

            // IBaraMapper baraMapper;
            var config = loader.Load("BaraMapConfig.xml", null);
        }

        public ConfigStream LoadConfigStream(String path)
        {
            var configStream = new ConfigStream
            {
                Path = path,
                Stream = FileLoader.Load(path)
            };
            return configStream;
        }

        public void LoadBaraMap(string filePath, BaraMapConfig baraMapConfig)
        {
            var baraMapSteam = LoadConfigStream(filePath);
            var baraMap = LoadBaraMap(baraMapSteam, baraMapConfig);
            baraMapConfig.BaraMaps.Add(baraMap);
        }

        public BaraMap LoadBaraMap(ConfigStream configStream, BaraMapConfig baraMapConfig)
        {
            using (configStream.Stream)
            {
                var baraMap = new BaraMap
                {
                    BaraMapConfig = baraMapConfig,
                    Path = configStream.Path,
                    Statements = new List<Statement> { },
                };

                XDocument xdoc = XDocument.Load(configStream.Stream);
                XElement xele = xdoc.Root;
                XNamespace ns = xele.GetDefaultNamespace();
                IEnumerable<XElement> StatementList = xele.Descendants(ns + "Statement");
                baraMap.Scope = (String)xele.Attribute("Scope");


                foreach (var statementNode in StatementList)
                {
                    var _statement = Statement.Load(statementNode, baraMap);
                    baraMap.Statements.Add(_statement);
                }
                return baraMap;
            }
        }

    }
}
