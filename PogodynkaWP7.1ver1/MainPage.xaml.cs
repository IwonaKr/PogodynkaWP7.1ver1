using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace PogodynkaWP7._1ver1
{
    public partial class MainPage : PhoneApplicationPage
    {
        int wybranyIndex=-1;
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
            this.listBox.ItemsSource=dataSource;

        }

        void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            Sample data = (sender as ListBox).SelectedItem as Sample;

            //Get the selected ListBoxItem container instance   
            ListBoxItem selectedItem = this.listBox.ItemContainerGenerator.ContainerFromItem(data) as ListBoxItem;

           // MessageBox.Show(data.Miasto);
            NavigationService.Navigate(new Uri("/pogoda.xaml?msg="+data.Miasto, UriKind.RelativeOrAbsolute));
            //this.listBox.SelectedIndex=-1;
        }
    }
}