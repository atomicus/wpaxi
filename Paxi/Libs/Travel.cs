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
using System.Device.Location;
using Paxi.Libs.GPS;

namespace Paxi.Libs
{
    public class Travel
    {
        enum TravelStates { clear, started, stoped };
        private TravelStates _TravelState;
        private String _StateDesription;
        private List<Traveler> _Travelers = new List<Traveler>();
        private List<TravelEvent> _Events = new List<TravelEvent>();
        private GPS.GPS _GPS;
        private Car _car;
        /// <summary>
        /// Creates a travel object.
        /// </summary>
        /// <param name="_gps">GPS watcher</param>
        /// <param name="_car">Car object</param>
        public Travel(GPS.GPS _gps, Car _car)
        {
            this.GPS = _gps;
            this.Car = _car;
            this.TravelState = TravelStates.clear;
            this.ChangeRouteType(ChangeRouteTypeTravelEvent.RouteTypes.mixed);
            GPS.DistanceChanged += new EventHandler<DistanceEventArgs>(DistanceChanged);

        }
        /// <summary>
        /// Returns icon name for next travel state 
        /// </summary>
        /// <returns></returns>
        public String IconNameForNextState() 
        {
            if (TravelState == TravelStates.clear) { return @"t_start.png"; }
            if (TravelState == TravelStates.stoped) { return @"t_clear.png"; }
            if (TravelState == TravelStates.started) { return @"t_stop.png"; };
            return @"";
        }
        /// <summary>
        /// Sets new travel state based on actual, rotates through [stoped,started,cleared]
        /// </summary>
        public void setNewTravelState()
        {
            //start new trip;
            if (this.TravelState == TravelStates.clear) 
            {
                this.StartTravel();
                this.TravelState = TravelStates.started;
                this.StateDesription = "Stop";
            }
            else if (this.TravelState == TravelStates.stoped)
            {
                this.ClearTravel();
                this.TravelState = TravelStates.clear;
                this.StateDesription = "Start";
            }
            else if (this.TravelState == TravelStates.started)
            {
                this.StopTravel();
                this.TravelState = TravelStates.stoped;
                this.StateDesription = "Clear";
            }
        }
        /// <summary>
        /// Travel lifecycle stuff.
        /// Starts new travel
        /// </summary>
        public void StartTravel() 
        {
           // setNewTravelState();
            GPS.startTracking();
        }
        /// <summary>
        /// Travel lifecycle stuff.
        /// Stops travel
        /// </summary>
        public void StopTravel() 
        {
           // setNewTravelState();
            GPS.stopTracking();
        }
        /// <summary>
        /// Travel lifecycle stuff.
        /// Prepares new travel
        /// </summary>
        public void ClearTravel()
        {
           // setNewTravelState();
            Travelers.Clear();
        }
        /// <summary>
        /// Adds new traveler to our trip
        /// </summary>
        /// <param name="_trv">Traveler that just eneter a car!</param>
        public void EnterCar(Traveler _trv)
        {
            _trv._startDistance = GPS.TotalDistance;
            Travelers.Add(_trv);
            _Events.Add(new EnterCarTravelEvent(GPS.TotalDistance));
        }
        /// <summary>
        /// Someone has left our party, ohh what a pity.
        /// </summary>
        /// <param name="_trv"></param>
        public void LeaveCar(Traveler _trv)
        {
            _trv.FinishTravel(GPS.TotalDistance);
            _Events.Add(new LeaveCarTravelEvent(GPS.TotalDistance));
        }
        /// <summary>
        /// Highways are expenisve. That's a pity too.
        /// </summary>
        /// <param name="_feeValue"></param>
        public void AddFee(double _feeValue)
        {
            _Events.Add(new FeeTravelEvent(GPS.TotalDistance, _feeValue));
        }
        /// <summary>
        /// We've moved from city to suburbs highway, let's take it into account.
        /// </summary>
        /// <param name="_newType"></param>
        public void ChangeRouteType(ChangeRouteTypeTravelEvent.RouteTypes _newType)
        {
            _Events.Add(new ChangeRouteTypeTravelEvent(GPS.TotalDistance, _newType));
        }
        /// <summary>
        /// Calcualte costs of travel for all travelers up to current distance
        /// </summary>
        public void CalculateCosts()
        {
            //create new travel calculator.
            TravelCalculator calc = new TravelCalculator(_Travelers, _Events, Car);

        }
        /// <summary>
        /// Updates current travel state based on all events that taken place till now.
        /// </summary>
        /// <returns></returns>
        public TravelCalculator getTravelState()
        {
            TravelCalculator trv = new TravelCalculator(_Travelers, _Events, Car);
            trv.getState();
            return trv;
        }
        /// <summary>
        /// Event triggered when distance changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args">DistanceEventArgs.Diff describing distance chanege</param>
        
        public void DistanceChanged(object sender, DistanceEventArgs args)
        {
            Travelers.ForEach(delegate(Traveler tmp)
            {
                tmp.PartialDistance += args.Diff;
            });
        }
        #region Getters&Setters

        public String StateDesription
        {
            get { return _StateDesription; }
            set { _StateDesription = value; }
        }
        private TravelStates TravelState
        {
            get { return _TravelState; }
            set { _TravelState = value; }
        }

        public List<Traveler> Travelers
        {
            get { return _Travelers; }
            set { _Travelers = value; }
        }

        public GPS.GPS GPS
        {
            get { return _GPS; }
            set { _GPS = value; }
        }


        public Car Car
        {
            get { return _car; }
            set { _car = value; }
        }
#endregion
    }
}
