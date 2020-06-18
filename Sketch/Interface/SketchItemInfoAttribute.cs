using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Sketch.Interface
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SketchItemInfoAttribute : Attribute
    {
        string _name;
        string _brief;
        Bitmap _bitmap;
        public SketchItemInfoAttribute(string name, string brief, Bitmap bitmap)
        {
            _name = name;
            _brief = brief;
            _bitmap = bitmap;
        }

        public string Name
        {
            get => _name;
        }

        public string Brief
        {
            get => _brief;
        }

        public Bitmap Bitmap
        {
            get => _bitmap;
        }

    }
}
