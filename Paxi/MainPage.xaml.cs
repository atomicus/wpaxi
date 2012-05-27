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
using Paxi.Libs;
namespace Paxi
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainPage_Loaded);


        }
        private void RoadType_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/RoadType.xaml", UriKind.Relative));
        }
        private void FuelConsumption_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/FuelConsumption.xaml", UriKind.Relative));

        }
        private void Fee_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Fee.xaml", UriKind.Relative));

        }
        void MainPage_Loaded(object sender, RoutedEventArgs args) 
        {
             App currentApp = (App)Application.Current;
             currentApp.Travel.Travelers.Add(new Libs.Traveler("Janek"));
             TravelersList.ItemsSource = currentApp.Travel.Travelers;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            App currentApp = (App)Application.Current;
            currentApp.Travel.EnterCar(new Libs.Traveler("Janek"));
            TravelersList.ItemsSource = null;
            currentApp.Travel.Travelers.Sort();
            TravelersList.ItemsSource = currentApp.Travel.Travelers;
        }

        private void listElement_Click(object sender, RoutedEventArgs e)
        {
            Traveler item = (Traveler)(sender as Button).DataContext;
            App currentApp = (App)Application.Current;
            currentApp.Travel.LeaveCar(item);
            TravelersList.ItemsSource = null;
            currentApp.Travel.Travelers.Sort();
            TravelersList.ItemsSource = currentApp.Travel.Travelers;
            System.Diagnostics.Debug.WriteLine(sender.ToString());
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            App currentApp = (App)Application.Current;
            currentApp.Travel.setNewTravelState();
        }




        
    }


}