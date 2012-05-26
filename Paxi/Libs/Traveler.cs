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
using System.ComponentModel;

namespace Paxi.Libs
{
    public class Traveler : INotifyPropertyChanged, IComparable<Traveler>
    {
        public double _startDistance, _endDistance, _cost, _partialDistance;
        Boolean _active;

        public Boolean isActive
        {
            get { return _active; }
            set { _active = value;
            this.RaisePropertyChanged("isActive");
            }
        }
        public double PartialDistance
        {
            get {
                if (isActive)
                {
                    return Math.Round(_partialDistance);
                }
                return Math.Round(TotalDistance);
            }
            set { _partialDistance = value;
            this.RaisePropertyChanged("PartialDistance");
            }
        }

        public Traveler(String lbl) 
        {
            this.Label = lbl;
            Cost = 0;
            StartDistance = 0;
            EndDistance = 0;
            PartialDistance = 0;
            isActive = true;
        }
        public int CompareTo(Traveler other)
        {
            return other.isActive.CompareTo(isActive);
        }
        public double Cost
        {
            get { return _cost; }
            set { _cost = value; }
        }
        private String _label;

        public String Label
        {
            get { return _label; }
            set { _label = value; }
        }

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
        public void FinishTravel(double _endDistance){
            EndDistance = _endDistance;
            isActive = false;
            this.RaisePropertyChanged("PartialDistance");
        }
        public double TotalDistance
        {
            get { return _endDistance - _startDistance; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                                new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
