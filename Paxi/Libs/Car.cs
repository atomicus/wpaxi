﻿using System;
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
using System.IO.IsolatedStorage;

namespace Paxi.Libs
{
    /// <summary>
    /// Basic contrainer for all things realted with car.
    /// </summary>
    public class Car
    {

        private string _Label;
        private Car instance;
        private int mode;
        public Double _cityConsumption, _mixedConsumption, _hwConsumption;
        private double _avgFuelConsumption;
        /// <summary>
        /// Singleton instance of car object.
        /// </summary>
        public Car getInstance
        {
            get
            {
                if (instance != null) return instance;
                else
                {
                    instance = new Car();
                    return instance;
                }
            }
        }
        /// <summary>
        /// Updates car parameters at once.
        /// </summary>
        /// <param name="_lbl">Label of car</param>
        /// <param name="_city">Fuel consumption in city</param>
        /// <param name="_mixed">Fuel consumption in mixed drive</param>
        /// <param name="_hw">Fuel consumption on highway</param>
        public void UpdateCar(String _lbl, Double _city, Double _mixed, Double _hw)
        {
            _Label = _lbl;
            _cityConsumption = _city;
            _mixedConsumption = _mixed;
            _hwConsumption = _hw;
        }
        /// <summary>
        /// Constructs car from data stored in local application storage
        /// </summary>
        public Car()
        {
            //try to load settings

            IsolatedStorageSettings.ApplicationSettings.TryGetValue<double>("_city", out _cityConsumption);
            IsolatedStorageSettings.ApplicationSettings.TryGetValue<double>("_mixed", out _mixedConsumption);
            IsolatedStorageSettings.ApplicationSettings.TryGetValue<double>("_hw", out _hwConsumption);
            IsolatedStorageSettings.ApplicationSettings.TryGetValue<int>("_mode", out mode);

            if (_cityConsumption == 0.0) _cityConsumption = 10;
            if (_mixedConsumption == 0.0) _mixedConsumption = 8;
            if (_hwConsumption == 0.0) _hwConsumption = 7;
            mode = 1;


        }

        /// <summary>
        /// Saves car setting to local storage
        /// </summary>
        public void saveSettings()
        {

        }
        /// <summary>
        /// Returns fuel consumtion for given route type [mixed,city,hw]
        /// </summary>
        /// <param name="_type">RouteType enum param</param>
        /// <returns></returns>

        public double getFuelConsumptionForRouteType(ChangeRouteTypeTravelEvent.RouteTypes _type)
        {
            if (_type == ChangeRouteTypeTravelEvent.RouteTypes.citi) return _cityConsumption;
            if (_type == ChangeRouteTypeTravelEvent.RouteTypes.mixed) return _mixedConsumption;
            if (_type == ChangeRouteTypeTravelEvent.RouteTypes.hw) return _hwConsumption;
            return 0;
        }

        public string Label
        {
            get { return _Label; }
            set { _Label = value; }
        }

    }
}
