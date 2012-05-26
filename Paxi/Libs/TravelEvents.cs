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
    public interface ITravelEvent 
    {
    }

    public class TravelEvent : ITravelEvent 
    {
        private double atDistance;
        public double AtDistance
        {
            get { return atDistance; }
            set { atDistance = value; }
        }
        public TravelEvent(double _distance) 
        {
            atDistance = _distance;
        }
        public virtual void beforeCalculation(TravelCalculator _travelCalculator) { }
        public virtual void afterCalculation(TravelCalculator _travelCalculator){}
    }

    public class EnterCarTravelEvent : TravelEvent
    {
        public EnterCarTravelEvent(double _distance) : base(_distance) 
        {
            System.Diagnostics.Debug.WriteLine("Added EnterCarEvent");
        }
        override public void afterCalculation(TravelCalculator _travelCalculator)
        {
            _travelCalculator.NrOfTravelers++;
        }
    }
    public class LeaveCarTravelEvent : TravelEvent
    {
        public LeaveCarTravelEvent(double _distance) : base(_distance) 
        {
            System.Diagnostics.Debug.WriteLine("Added LeaveCarEvent");
        }
        override public void afterCalculation(TravelCalculator _travelCalculator)
        {
            _travelCalculator.NrOfTravelers--;
        }
    }
    public class FeeTravelEvent : TravelEvent
    {
        public double _feeValue; 
        public FeeTravelEvent(double _distance,double _feeValue) : base(_distance) {
            System.Diagnostics.Debug.WriteLine("Added FeeTravelEvent");
            this._feeValue = _feeValue;
         }
    }
    public class ChangeRouteTypeTravelEvent : TravelEvent
    {
         public enum RouteTypes {citi,mixed,hw}
         public RouteTypes _type;
         public ChangeRouteTypeTravelEvent(double _distance,RouteTypes _type) : base(_distance) 
         {
             System.Diagnostics.Debug.WriteLine("Added ChangeRouteTypeEvent");
             this._type = _type;

         }
         override public void afterCalculation(TravelCalculator _travelCalculator)
         {
             _travelCalculator.RouteType = _type;
         }
    }
}
