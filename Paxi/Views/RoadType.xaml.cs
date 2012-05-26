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
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Paxi.Libs;

namespace Paxi.Views
{
    public partial class RoadType : PhoneApplicationPage
    {
        public RoadType()
        {
            InitializeComponent();

        }

        override protected void OnNavigatedTo(NavigationEventArgs e)
        {
            App currentApp = (App)Application.Current;
            ChangeRouteTypeTravelEvent.RouteTypes RouteType = currentApp.Travel.getTravelState().RouteType;

            if (RouteType == ChangeRouteTypeTravelEvent.RouteTypes.citi)
            {
                this.button1.Background = new SolidColorBrush(Colors.Gray);
            }
            if (RouteType == ChangeRouteTypeTravelEvent.RouteTypes.mixed)
            {
                this.button2.Background = new SolidColorBrush(Colors.Gray);
            }
            if (RouteType == ChangeRouteTypeTravelEvent.RouteTypes.hw)
            {
                this.button3.Background = new SolidColorBrush(Colors.Gray);
            }

        }

        private void City_Click(object sender, RoutedEventArgs e)
        {
            App currentApp = (App)Application.Current;
            currentApp.Travel.ChangeRouteType(Libs.ChangeRouteTypeTravelEvent.RouteTypes.citi);
            NavigationService.GoBack();

        }
        private void Mixed_Click(object sender, RoutedEventArgs e)
        {
            App currentApp = (App)Application.Current;
            currentApp.Travel.ChangeRouteType(Libs.ChangeRouteTypeTravelEvent.RouteTypes.mixed);
            NavigationService.GoBack();
        }
        private void Highway_Click(object sender, RoutedEventArgs e)
        {
            App currentApp = (App)Application.Current;
            currentApp.Travel.ChangeRouteType(Libs.ChangeRouteTypeTravelEvent.RouteTypes.hw);
            NavigationService.GoBack();
        }
    }
}