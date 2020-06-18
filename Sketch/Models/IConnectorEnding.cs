using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Sketch.Models
{
    public interface IConnectorEnding
    {
        bool IsFilled
        {
            get;
            set;
        }

        Vector Translation
        {
            get;
            set;
        }

        double Rotation
        {
            get;
            set;
        }

        double ScaleX
        {
            get;
            set;
        }

        double ScaleY
        {
            get;
            set;
        }

        Geometry Ending
        {
            get;
        }
    }
}
