using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;
namespace Paxi.Views
{
    public partial class FuelConsumption : PhoneApplicationPage
    {
        public FuelConsumption()
        {
            InitializeComponent();

            var numericScope = new InputScope();
            var numericScopeName = new InputScopeName();
            numericScopeName.NameValue = InputScopeNameValue.Number;
            numericScope.Names.Add(numericScopeName);
            cityBox.InputScope = numericScope;
            mixedBox.InputScope = numericScope;
            hwBox.InputScope = numericScope;     

        }
        override protected void OnNavigatedTo(NavigationEventArgs e)
        {
            App currentApp = (App)Application.Current;
            this.cityBox.Text = currentApp.Travel.Car.getFuelConsumptionForRouteType(Libs.ChangeRouteTypeTravelEvent.RouteTypes.citi).ToString();
            this.mixedBox.Text = currentApp.Travel.Car.getFuelConsumptionForRouteType(Libs.ChangeRouteTypeTravelEvent.RouteTypes.mixed).ToString();
            this.hwBox.Text = currentApp.Travel.Car.getFuelConsumptionForRouteType(Libs.ChangeRouteTypeTravelEvent.RouteTypes.hw).ToString();


        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            App currentApp = (App)Application.Current;
            currentApp.Travel.Car._cityConsumption = Double.Parse(this.cityBox.Text);
            currentApp.Travel.Car._mixedConsumption = Double.Parse(this.mixedBox.Text);
            currentApp.Travel.Car._hwConsumption = Double.Parse(this.hwBox.Text);
        }
    }
}