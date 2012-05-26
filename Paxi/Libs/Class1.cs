using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Paxi.Libs
{
    public class Traveler
    {
        public double _startDistance, _endDistance;

        public double StartDistance
        {
            get { return _startDistance; }
            set { _startDistance = value; }
        }

        public double EndDistance
        {
            get { return _endDistance; }
            set { _endDistance = value; }
        }
        public double TotalDistance
        {
            get { return _endDistance - _startDistance; }
        }
        
    }
    public class Travelers
    {

    }
}
