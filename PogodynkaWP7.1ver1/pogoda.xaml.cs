﻿using System;
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
        public static List<ForecastDay> dni1= new List<ForecastDay>();
        public static List<ForecastDay> dni2= new List<ForecastDay>(); //txt_forecast
        public static List<ForecastDay> SFDay = new List<ForecastDay>(); //SimpleForecast


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
                this.miastoTB.Text=miasto;
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
            //Console.WriteLine("Zabawa z XDocument");
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
            var simpleForecast = (from d in doc.Descendants()
                                  where (d.Name.LocalName=="simpleforecast")
                                  select d).ToList();

            var smplFrcstDay = (from d in simpleForecast.Descendants()
                                where (d.Name.LocalName=="forecastday")
                                select d).ToList();
            foreach (var item in smplFrcstDay)
            {
                string sTmp="";
                int iTmp=0;
                Console.WriteLine("****"+item);
                ForecastDay fd = new ForecastDay();

                fd.period = item.Element("period").Value;
                fd.icon=item.Element("icon").Value;
                fd.iconUrl=item.Element("icon_url").Value;
                fd.conditions=item.Element("conditions").Value;
                fd.pop=item.Element("pop").Value;

                //DATA
                Date d = new Date();
                var data = (from x in item.Descendants()
                            where x.Name.LocalName=="date"
                            select x).FirstOrDefault();

                Console.WriteLine("XXXXXXX: "+data.Element("day").Value);
                d.day=data.Element("day").Value;
                d.epoch=data.Element("epoch").Value;
                d.hour=data.Element("hour").Value;
                d.min=data.Element("min").Value;
                d.month=data.Element("month").Value;
                d.monthName=data.Element("monthname").Value;
                d.weekDay=data.Element("weekday").Value; //albo weekday_short , czyli skrót nazwy dnia tygodnia
                d.pretty=data.Element("pretty").Value;
                d.yday=data.Element("yday").Value;
                d.year=data.Element("year").Value;
                d.prettyShort=data.Element("pretty_short").Value;

                fd.data=d;


                //WIND NIE DZIAŁA, DOKOŃCZYĆ

                //MAX WIND
                var wnd = (from x in item.Descendants()
                           where x.Name.LocalName=="maxwind"
                           select x).FirstOrDefault();
                sTmp=wnd.Element("mph").Value;
                if (int.TryParse(sTmp, out iTmp))
                    fd.maxwind_mph=iTmp;
                sTmp=wnd.Element("kph").Value;
                if (int.TryParse(sTmp, out iTmp))
                    fd.maxwind_kph=iTmp;
                fd.maxwind_dir=wnd.Element("dir").Value;
                fd.maxwind_degrees=wnd.Element("degrees").Value;

                //AVERAGE WIND
                wnd = (from x in item.Descendants()
                       where x.Name.LocalName=="avewind"
                       select x).FirstOrDefault();
                sTmp=wnd.Element("mph").Value;
                if (int.TryParse(sTmp, out iTmp))
                    fd.avewind_mph=iTmp;
                sTmp=wnd.Element("kph").Value;
                if (int.TryParse(sTmp, out iTmp))
                    fd.avewind_kph=iTmp;
                fd.avewind_dir=wnd.Element("dir").Value;
                fd.avewind_degrees=wnd.Element("degrees").Value;

                Console.WriteLine(sTmp);
                // HUMIDITY
                sTmp=item.Element("avehumidity").Value;
                if (int.TryParse(sTmp, out iTmp))
                    fd.avehumidity=iTmp;
                sTmp=item.Element("maxhumidity").Value;
                if (int.TryParse(sTmp, out iTmp))
                    fd.maxhumidity=iTmp;
                sTmp=item.Element("minhumidity").Value;
                if (int.TryParse(sTmp, out iTmp))
                    fd.minhumidity=iTmp;

                //TEMPERATURE
                var temp2 = from m in item.Descendants()
                            where (m.Name.LocalName=="high")
                            select m;
                var temeperatura = from m in temp2.Descendants()
                                   where m.Name.LocalName=="celsius"
                                   select m.Value;

                sTmp = temeperatura.First();
                fd.highTempC=sTmp;
                temp2 = from m in item.Descendants()
                        where (m.Name.LocalName=="low")
                        select m;
                temeperatura = from m in temp2.Descendants()
                               where m.Name.LocalName=="celsius"
                               select m.Value;
                sTmp = temeperatura.First();
                fd.lowTempC=sTmp;
                SFDay.Add(fd);
            }

            var dzien = (from d in SFDay where d.period=="1" select d).FirstOrDefault();
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
                    TextBlock tb = new TextBlock();
                    tb.Text = "Temp: "+dzien.lowTempC+"C-"+dzien.highTempC+"C\nWarunki: "+dzien.conditions+"\nWilgotność (min,max,śr): "+","+dzien.maxhumidity.ToString()+","+dzien.avehumidity.ToString()+"\nWiatr (m/h, km/h,kierunek): "+dzien.maxwind_mph.ToString()+","+dzien.maxwind_kph.ToString()+","+dzien.maxwind_dir;


                    this.glownyStackPanel.Children.Add(tb);
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