using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bluebottle.Base.Interfaces;

namespace Bluebottle.Base.Behaviors
{
    public class TextAccessor : ITextAccessor
    {
        Action<string> _setText;
        Func<string> _getText;
        public TextAccessor(Action<string> setText, Func<string> getText)
        {
            _setText = setText;
            _getText = getText;
        }
        public string GetText()
        {
            return _getText();
        }

        public void SetText(string text)
        {
            _setText(text);
        }
    }
}
