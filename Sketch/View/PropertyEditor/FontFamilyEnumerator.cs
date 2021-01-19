using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Sketch.View.PropertyEditor
{
    public class FontFamilyEnumerator : IEnumerable<string>, IEnumerator<string>
    {
        List<string> _fontFamilyNames =null;
        int _currentIndex;

        public string Current => FontFamilies[_currentIndex];

        public IList<string> FontFamilies
        {
            get
            {
                if(_fontFamilyNames == null )
                {
                    _fontFamilyNames = new List<string>();
                    _fontFamilyNames.AddRange(Fonts.SystemFontFamilies.Select<FontFamily, string>((x) => x.Source));
                }
                return _fontFamilyNames;
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose(){}

        public IEnumerator<string> GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            _currentIndex++;
            return _currentIndex >= FontFamilies.Count;
        }

        public void Reset()
        {
            _currentIndex = -1;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
    }
}
