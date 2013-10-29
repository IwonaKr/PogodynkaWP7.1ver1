using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Device.Location;
using System.Diagnostics;

namespace PogodynkaWP7._1ver1
{
    public partial class MainPage : PhoneApplicationPage
    {
        GeoCoordinateWatcher watcher;
        String temp="";

        //int wybranyIndex=-1;
        public class Sample
        {
            public string Miasto { get; set; }
        }
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            List<Sample> dataSource = new List<Sample>();
            dataSource.Add(new Sample() { Miasto="Lublin" });
            dataSource.Add(new Sample() { Miasto="Warszawa" });
            dataSource.Add(new Sample() { Miasto ="Puławy" });
            dataSource.Add(new Sample() { Miasto="Wrocław" });
            dataSource.Add(new Sample() { Miasto="Kielce" });
            dataSource.Add(new Sample() { Miasto="Poznań" });
            dataSource.Add(new Sample() { Miasto ="Kraków" });
            dataSource.Add(new Sample() { Miasto="Gdańsk" });
            dataSource.Add(new Sample() { Miasto="GPS" });
            this.listBox.ItemsSource=dataSource;
            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            watcher.MovementThreshold=20;
            watcher.PositionChanged+=watcher_PositionChanged;

        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            
            //throw new NotImplementedException();
            var coordinate = e.Position.Location;
            Debug.WriteLine("Lat: {0}, Long: {1}", coordinate.Latitude,
                coordinate.Longitude);
            // Uncomment to get only one event. 
            // watcher.Stop(); 
            temp=coordinate.Latitude+","+coordinate.Longitude;
                
        }

        void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            Sample data = (sender as ListBox).SelectedItem as Sample;
            ListBoxItem selectedItem = this.listBox.ItemContainerGenerator.ContainerFromItem(data) as ListBoxItem;
            if (data.Miasto=="GPS")
            {
                data.Miasto=temp;
            }
            NavigationService.Navigate(new Uri("/pogoda.xaml?msg="+data.Miasto, UriKind.RelativeOrAbsolute));

        }

        //private async Task<String> Geoloc(Sample data)
        //{
        //    

        //    watcher.PositionChanged += (sender2, ee) =>
        //    {
        //        var coordinate = ee.Position.Location;
        //        Debug.WriteLine("Lat: {0}, Long: {1}", coordinate.Latitude,
        //            coordinate.Longitude);
        //        // Uncomment to get only one event. 
        //        // watcher.Stop(); 
        //        temp=coordinate.Latitude+","+coordinate.Longitude;
        //    };

        //    // Begin listening for location updates.
        //    watcher.Start();

        //    cos=temp;
        //}


    }
}