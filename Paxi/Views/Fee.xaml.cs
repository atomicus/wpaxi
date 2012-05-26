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

namespace Paxi.Views
{
    public partial class Fee : PhoneApplicationPage
    {
        public Fee()
        {
            InitializeComponent();
            var numericScope = new InputScope();
            var numericScopeName = new InputScopeName();
            numericScopeName.NameValue = InputScopeNameValue.Number;
            numericScope.Names.Add(numericScopeName);
            feeBox.InputScope = numericScope;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                App currentApp = (App)Application.Current;
                currentApp.Travel.AddFee(Double.Parse(feeBox.Text));
            }
            catch (Exception ex) { }
            NavigationService.GoBack();
            
        }
    }
}