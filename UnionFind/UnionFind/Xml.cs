using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace UnionFind
{
    public static class XmlUtils
    {
        /// <summary>
        /// 是否省略XML声明
        /// </summary>
        public static bool IsOmitXmlDeclaration { get; set; } = false;

        /// <summary>
        /// 指定XML命名空间
        /// </summary>
        public static XmlSerializerNamespaces XmlNamespaces { get; set; } =
            new Func<XmlSerializerNamespaces>(
                () =>
                {
                    XmlSerializerNamespaces x = new XmlSerializerNamespaces();
                    x.Add(string.Empty, string.Empty);
                    return x;
                }
                )();

        private static void XmlSerializeInternal(Stream stream, object o, Encoding encoding)
        {
            if (o == null)
                throw new ArgumentNullException("o");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            XmlSerializer serializer = new XmlSerializer(o.GetType());

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineChars = "\r\n";
            settings.Encoding = encoding;
            settings.IndentChars = "    ";

            //不生成声明头
            settings.OmitXmlDeclaration = IsOmitXmlDeclaration;

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, o, XmlNamespaces);
                writer.Close();
            }
        }

        /// <summary>
        /// 将一个对象序列化为XML字符串
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>序列化产生的XML字符串</returns>
        public static string XmlSerialize(object o, Encoding encoding)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializeInternal(stream, o, encoding);

                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// 将一个对象按XML序列化的方式写入到一个文件
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="path">保存文件路径</param>
        /// <param name="encoding">编码方式</param>
        public static void XmlSerializeToFile(object o, string path, Encoding encoding)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                XmlSerializeInternal(file, o, encoding);
            }
        }

        /// <summary>
        /// 从XML字符串中反序列化对象
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="s">包含对象的XML字符串</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>反序列化得到的对象</returns>
        public static T XmlDeserialize<T>(string s, Encoding encoding)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentNullException("s");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            XmlSerializer mySerializer = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(encoding.GetBytes(s)))
            {
                using (StreamReader sr = new StreamReader(ms, encoding))
                {
                    return (T)mySerializer.Deserialize(sr);
                }
            }
        }

        /// <summary>
        /// 读入一个文件，并按XML的方式反序列化对象。
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>反序列化得到的对象</returns>
        public static T XmlDeserializeFromFile<T>(string path, Encoding encoding)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            string xml = File.ReadAllText(path, encoding);
            return XmlDeserialize<T>(xml, encoding);
        }
    }
    
    public class UFRoot
    {
        public Header Header { get; set; }
        public List<Connection> Data { get; set; }
    }

    public class Header
    {
        public string Time { get; set; } = DateTime.Now.ToString();
        public string UserName { get; set; } = Environment.UserName;
    }

    public class Connection
    {
        public MyPoint Point1 { get; set; }
        public MyPoint Point2 { get; set; }

        public bool ConnectionEquals(Connection connection) =>
            this.Point1.ValueEquals(connection.Point1) && this.Point2.ValueEquals(connection.Point2) ||
            this.Point1.ValueEquals(connection.Point2) && this.Point2.ValueEquals(connection.Point1);
    }

    public class MyPoint
    {
        public MyPoint() { }
        public MyPoint(MyPoint p)
        {
            X = p.X;
            Y = p.Y;
        }

        [XmlAttribute]
        public int X { get; set; }

        [XmlAttribute]
        public int Y { get; set; }

        public static MyPoint operator +(MyPoint left, MyPoint right) =>
            new MyPoint { X = left.X + right.X, Y = left.Y + right.Y };

        public static MyPoint operator -(MyPoint left, MyPoint right) =>
            new MyPoint { X = left.X - right.X, Y = left.Y - right.Y };

        public bool ValueEquals(MyPoint point)
        {
            //System.Diagnostics.Debug.WriteLine($"{this.ToString()} ValueEquals {point}, {this.X == point.X && this.Y == point.Y}");
            return this.X == point.X && this.Y == point.Y;
        }
           

        public override string ToString() => $"X:{X},Y:{Y}";
    }
}
