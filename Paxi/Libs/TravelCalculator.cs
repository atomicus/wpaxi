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
using System.Collections.Generic;
namespace Paxi.Libs
{
    public class TravelCalculator
    {
        public List<Traveler> Travelers;
        public List<TravelEvent> Events;
        public Car Car;
        private double _lastEventDistance;
        private int _nrOfTravelers;
        private ChangeRouteTypeTravelEvent.RouteTypes _routeType;



        public int NrOfTravelers
        {
            get { return _nrOfTravelers; }
            set { _nrOfTravelers = value; }
        }
        public ChangeRouteTypeTravelEvent.RouteTypes RouteType
        {
            get { return _routeType; }
            set { _routeType = value; }
        }

        public TravelCalculator(List<Traveler> _Travelers, List<TravelEvent> _events, Car _car)
        {
            Travelers = _Travelers;
            Car = _car;
            Events = _events;
            _lastEventDistance = 0;
            _nrOfTravelers = 0;
        }
        public void calculateCosts()
        {
            Events.ForEach(delegate(TravelEvent _event)
            {
                _event.beforeCalculation(this);
                updateCosts(_event.AtDistance);
                _event.afterCalculation(this);
                _lastEventDistance = _event.AtDistance;
             });

        }
        public TravelCalculator getState() 
        {
            Events.ForEach(delegate(TravelEvent _event)
            {
                _event.beforeCalculation(this);
                //updateCosts(_event.AtDistance);
                _event.afterCalculation(this);
                _lastEventDistance = _event.AtDistance;
            });
            return this;
        }
        private void updateCosts(double _currentDistance) 
        {
            double eventDistance = this._lastEventDistance - _currentDistance;
            double cost = (eventDistance * Car.getFuelConsumptionForRouteType(this._routeType)) / this.NrOfTravelers;
            Travelers.ForEach(delegate(Traveler tmp)
            {
                tmp.Cost += cost;
            }
            );
        }
    }
}
