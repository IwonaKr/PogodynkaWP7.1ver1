using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Phone.Controls;

namespace PogodynkaWP7._1ver1
{
    public partial class pogoda : PhoneApplicationPage
    {
        public static string miasto;
        string place = "";
        string obs_time = "";
        string weather1 = "";
        string temperature_string = "";
        string relative_humidity = "";
        string wind_string = "";
        string pressure_mb = "";
        string dewpoint_string = "";
        string visibility_km = "";
        string latitude = "";
        string longitude = "";
        string icon="";
        public static List<ForecastDay> dni2= new List<ForecastDay>();
        public pogoda()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string msg;
            if (NavigationContext.QueryString.TryGetValue("msg", out msg))
            {
                miasto=msg;
                this.cos1.Text=miasto;
                Thread t = new Thread(NewThread);
                t.Start();
            }

        }
        void NewThread()
        {
            string url = "http://api.wunderground.com/api/c9d15b10ff3ed303/forecast/conditions/astronomy/lang:PL/q/Poland/"+miasto+".xml";

            WebClient wc = new WebClient();
            //wc.DownloadStringCompleted += HttpsCompleted;
            wc.DownloadStringCompleted +=wc_DownloadStringCompleted; //dodane, bez tej funkcji też działa!!

            wc.DownloadStringAsync(new Uri(url));

        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            Console.WriteLine("Zabawa z XDocument");
            string weather="";
            weather = e.Result;
            XmlReader reader = XmlReader.Create(new StringReader(weather));
            XDocument doc = XDocument.Load(reader);
            var txt_forecast = (from d in doc.Descendants()
                                where (d.Name.LocalName == "txt_forecast")
                                select d).ToList();

            var forecast = (from d in txt_forecast.Descendants()
                            where (d.Name.LocalName=="forecastday")
                            select d).ToList();
            
            foreach (var item in forecast)
            {
                
                Console.WriteLine(item);
                ForecastDay d = new ForecastDay();
                d.period = item.Element("period").Value;
                d.icon=item.Element("icon").Value;
                d.iconUrl=item.Element("icon_url").Value;
                d.fcttext=item.Element("fcttext").Value;
                d.fcttextMetric=item.Element("fcttext_metric").Value;
                d.title=item.Element("title").Value;
                d.pop=item.Element("pop").Value;
                dni2.Add(d);
            }
            var dzien = (from d in dni2 where d.period=="0" select d).FirstOrDefault();
            if (!(dzien==null))
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.textBox1.Text = "Period:             " + dzien.period+
                            "\nIconUri: " + dzien.iconUrl+
                            "\nPogoda:          " + dzien.fcttext+
                            "\nfcttextMetric:     " + dzien.fcttextMetric+
                            "\ntitle:           " + dzien.title;
                    Uri uri = new Uri("Icons/"+dzien.icon+".png", UriKind.Relative);
                    ImageSource imgSource = new BitmapImage(uri);
                    this.ikonka.Source = imgSource;
                });
            }
        }

        private void HttpsCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(e.Result)))
            {
                // Parse the file and display each of the nodes.
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name.Equals("full"))
                            {
                                reader.Read();
                                place = reader.Value;
                            }
                            else if (reader.Name.Equals("observation_time"))
                            {
                                reader.Read();
                                obs_time = reader.Value;
                            }
                            else if (reader.Name.Equals("weather"))
                            {
                                reader.Read();
                                weather1 = reader.Value;
                            }
                            else if (reader.Name.Equals("temperature_string"))
                            {
                                reader.Read();
                                temperature_string = reader.Value;
                            }
                            else if (reader.Name.Equals("relative_humidity"))
                            {
                                reader.Read();
                                relative_humidity = reader.Value;
                            }
                            else if (reader.Name.Equals("wind_string"))
                            {
                                reader.Read();
                                wind_string = reader.Value;
                            }
                            else if (reader.Name.Equals("pressure_mb"))
                            {
                                reader.Read();
                                pressure_mb = reader.Value;
                            }
                            else if (reader.Name.Equals("dewpoint_string"))
                            {
                                reader.Read();
                                dewpoint_string = reader.Value;
                            }
                            else if (reader.Name.Equals("visibility_km"))
                            {
                                reader.Read();
                                visibility_km = reader.Value;
                            }
                            else if (reader.Name.Equals("latitude"))
                            {
                                reader.Read();
                                latitude = reader.Value;
                            }
                            else if (reader.Name.Equals("longitude"))
                            {
                                reader.Read();
                                longitude = reader.Value;
                            }
                            else if (reader.Name.Equals("icon"))
                            {
                                reader.Read();
                                icon=reader.Value;
                            }

                            break;
                    }
                }

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                   {
                       this.textBox1.Text = "Miejsce:             " + place+
                            "\nCzas obserwacji: " + obs_time+
                            "\nPogoda:          " + weather1+
                            "\nTemperatura:     " + temperature_string+
                            "\nWiatr:           " + wind_string+
                            "\nLokacja:         " + longitude + ", " + latitude;
                       Uri uri = new Uri("Icons/"+icon+".png", UriKind.Relative);
                       ImageSource imgSource = new BitmapImage(uri);
                       this.ikonka.Source = imgSource;
                       //this.ikonka.Source = new BitmapImage(new Uri("pack://application:,,,/PogodynkaWP7.1ver1;component/Icons/"+icon"+.png"));
                   });

                //throw new NotImplementedException();
                /*if (e.Error == null)
                {
                    XDocument xdoc = XDocument.Parse(e.Result, LoadOptions.None);
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            this.textBox1.Text = xdoc.Root.ToString();
                        });

                }*/
            }
        }
    }
}