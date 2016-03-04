using System;
using System.Linq;
using System.Web;
using AspectDotNet;
using Orchard.DisplayManagement;
namespace Aspect1


{
    public class ExtractFormatting : Aspect
    {
        private const int MaxStringLength = 60;
        //метод внедряется
       //[AspectAction("%instead %call *ObjectDumper.FormatValue(object) && args(..)")]
        public static string FormatValue(object o)
        {
            if (o == null)
                return "null";

            var formatted = o.ToString();

            if (o is string)
            {
                // remove central part if tool long
                if (formatted.Length > MaxStringLength)
                {
                    formatted = formatted.Substring(0, MaxStringLength / 2) + "..." + formatted.Substring(formatted.Length - MaxStringLength / 2);
                }

                formatted = "\"" + formatted + "\"";
            }

            return formatted;
        }

        //метод внедряется
        [AspectAction("%instead %call *ObjectDumper.FormatType(object) && args(..)")]
        public static string FormatType(object item)
        {
            var shape = item as IShape;
            if (shape != null)
            {
                return shape.Metadata.Type + " Shape";
            }

            return FormatType(item.GetType());
        }

        //метод внедряется
        [AspectAction("%instead %call *ObjectDumper.FormatType(Type) && args(..)")]
        public static string FormatType(Type type)
        {
            if (type.IsGenericType)
            {
                var genericArguments = String.Join(", ", type.GetGenericArguments().Select(FormatType).ToArray());
                return String.Format("{0}<{1}>", type.Name.Substring(0, type.Name.IndexOf('`')), genericArguments);
            }

            return type.Name;
        }

        //метод внедряется
      // [AspectAction("%instead %call *ObjectDumper.FormatJsonValue(string) && args(..)")]
        public static string FormatJsonValue(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return String.Empty;
            }

            // replace " by \" in json strings
            return HttpUtility.HtmlEncode(value).Replace(@"\", @"\\").Replace("\"", @"\""").Replace("\r\n", @"\n").Replace("\r", @"\n").Replace("\n", @"\n");
        }

        //метод внедряется
       // [AspectAction("%instead %call *ShapeTracingFactory.FormatShapeFilename(string, string, string, string, string) && args(..)")]
        public static string FormatShapeFilename(string shape, string shapeType, string displayType, string themePrefix, string extension) {

            if (!String.IsNullOrWhiteSpace(displayType)) {
                if (shape.StartsWith(shapeType + "_" + displayType)) {
                    shape = shapeType + shape.Substring(shapeType.Length + displayType.Length + 1) + "_" + displayType;
                }
            }

            return themePrefix + "/Views/" + shape.Replace("__", "-").Replace("_", ".") + extension;
        }

    }
}