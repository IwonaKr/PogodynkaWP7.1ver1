using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Device.Location;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace PogodynkaWP7._1ver1
{
    public partial class MainPage : PhoneApplicationPage
    {
        GeoCoordinateWatcher geoWatcher = null;
        ObservableCollection<Sample> dataSource;
        //int wybranyIndex=-1;
        public class Sample
        {
            public string Miasto { get; set; }
        }
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            geoWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            geoWatcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(geoWatcher_PositionChanged);
            geoWatcher.Start();

            dataSource = new ObservableCollection<Sample>();
            dataSource.Add(new Sample() { Miasto="Lublin" });
            dataSource.Add(new Sample() { Miasto="Warszawa" });
            dataSource.Add(new Sample() { Miasto ="Puławy" });
            dataSource.Add(new Sample() { Miasto="Wrocław" });
            dataSource.Add(new Sample() { Miasto="Kielce" });
            dataSource.Add(new Sample() { Miasto="Poznań" });
            dataSource.Add(new Sample() { Miasto ="Kraków" });
            dataSource.Add(new Sample() { Miasto="Gdańsk" });

            this.listBox.ItemsSource=dataSource;
        }

        private void geoWatcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            GeoCoordinate currentLocation = e.Position.Location;
            double currentAltitude = e.Position.Location.Altitude;
            double currentLongitude = e.Position.Location.Longitude;
            double currnetLatitude = e.Position.Location.Latitude;

            //Dispatcher.BeginInvoke(() =>
            //{
            //    this.ContentPanel.Children.Add(new TextBlock() { Text="Latitude: "+currnetLatitude.ToString() });
            //    this.ContentPanel.Children.Add(new TextBlock() { Text="Longitude: "+currentLongitude.ToString("0.00") });
            //});
            this.dataSource.Add(new Sample() { Miasto=currnetLatitude.ToString("0.00")+","+currentLongitude.ToString("0.00") });
            this.listBox.ItemsSource=dataSource;
        }



        void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            Sample data = (sender as ListBox).SelectedItem as Sample;
            ListBoxItem selectedItem = this.listBox.ItemContainerGenerator.ContainerFromItem(data) as ListBoxItem;

            NavigationService.Navigate(new Uri("/pogoda.xaml?msg="+data.Miasto, UriKind.RelativeOrAbsolute));

        }

    }
}