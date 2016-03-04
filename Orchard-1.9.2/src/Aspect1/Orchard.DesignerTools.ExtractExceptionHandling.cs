using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml.Linq;
using AspectDotNet;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Shapes;
using Orchard.DesignerTools.Services;

namespace Aspect1
{
    public class ExtractExceptionHandling : Aspect
    {
        private readonly static Stack<object> _parents = new Stack<object>();
        private static readonly XElement _current = new XElement("ul");
        public static int _levels;
        private static object _dumpO;
        private static string _dumpName;
        private static object _dumpObjectO;
        private static string _dumpObjectName;

        //конструктор не внедряется
        //[AspectAction("%after %call *ObjectDumper.ctor(int) && args(..)")]
        public static void GetCtorValue(int levels)
        {
            _levels = levels;
        }

       // метод внедряется
        //[AspectAction("%before %call *ObjectDumper.Dump(object, string) && args(..)")]
        public static void Dump(object o, string name)
        {
            _dumpO = o;
            _dumpName = name;
        }

        //метод не внедряется
        //[AspectAction("%after %call *ObjectDumper.EnterNode(string) && %withincode(*ObjectDumper.Dump(object, string))")]
        public static XElement Dump()
        {
            var a = new ObjectDumper(_levels);
            try
            {
                if (_dumpO == null)
                {
                    a.DumpValue(null, _dumpName);
                }
                else if (_dumpO.GetType().IsValueType || _dumpO is string)
                {
                    a.DumpValue(_dumpO, _dumpName);
                }
                else
                {
                    if (_parents.Count >= _levels)
                    {
                        return _current;
                    }

                    a.DumpObject(_dumpO, _dumpName);
                }
            }
            finally
            {
                _parents.Pop();
                a.RestoreCurrentNode();
            }

            return _current;
        }

        //метод не внедряется
        //[AspectAction("%after %call *ObjectDumper.EnterNode(string) && %withincode(*ObjectDumper.DumpObject(object, string))")]
        public static void DumpObject()
        {
            var a = new ObjectDumper(_levels);
            try
            {
                if (_dumpObjectO is IDictionary)
                {
                    a.DumpDictionary((IDictionary)_dumpObjectO);
                }
                else if (_dumpObjectO is IShape)
                {
                    a.DumpShape((IShape)_dumpObjectO);

                    // a shape can also be IEnumerable
                    if (_dumpObjectO is Shape)
                    {
                        var items = _dumpObjectO.GetType()
                            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            .FirstOrDefault(m => m.Name == "Items");

                        var classes = _dumpObjectO.GetType()
                            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            .FirstOrDefault(m => m.Name == "Classes");

                        var attributes = _dumpObjectO.GetType()
                            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            .FirstOrDefault(m => m.Name == "Attributes");

                        if (classes != null)
                        {
                            a.DumpMember(_dumpObjectO, classes);
                        }

                        if (attributes != null)
                        {
                            a.DumpMember(_dumpObjectO, attributes);
                        }

                        if (items != null)
                        {
                            a.DumpMember(_dumpObjectO, items);
                        }


                        // DumpEnumerable((IEnumerable) o);
                    }
                }
                else if (_dumpObjectO is IEnumerable)
                {
                    a.DumpEnumerable((IEnumerable)_dumpObjectO);
                }
                else
                {
                    a.DumpMembers(_dumpObjectO);
                }
            }
            finally
            {
                a.RestoreCurrentNode();
            }
        }

    }
}