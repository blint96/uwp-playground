using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using LiveCharts;
using LiveCharts.Uwp;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Kryptowaluty
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public Dictionary<int, string> currencies = new Dictionary<int, string>();
        private Double course = 0.0;

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        enum CurrencyTypes
        {
            BITCOIN = 0,
            LITECOIN = 1,
            BITCOIN_CASH = 2
        };

        /**
         * 
         */
        public MainPage()
        {
            this.InitializeComponent();
            stackAfter.Visibility = Visibility.Collapsed;
            txtCourse.Text = "";

            currencies.Add((int)CurrencyTypes.BITCOIN, "BTC");
            currencies.Add((int)CurrencyTypes.LITECOIN, "LTC");
            currencies.Add((int)CurrencyTypes.BITCOIN_CASH, "BCC");
        
            cBoxSelectCurrency.ItemsSource = currencies;
            cBoxSelectCurrency.DisplayMemberPath = "Value";
            cBoxSelectCurrency.SelectedValuePath = "Key";

            SeriesCollection = new SeriesCollection() {};

            // if coming back from other page
            // select index and refresh this view
            if(SelectedCurrency.id != -1)
            {
                cBoxSelectCurrency.SelectedIndex = SelectedCurrency.id;
            }
        }

        /**
         * 
         */
        static public async void Show(string mytext)
        {
            var dialog = new MessageDialog(mytext, "Informacja");
            await dialog.ShowAsync();
        }

        /**
         * 
         */
        private void FillChart(JsonArray j)
        {
            SeriesCollection.Clear();

            List<double> d = new List<double>();
            foreach(var item in j.GetArray())
            {
                var obj = item.GetObject();
                d.Add(Double.Parse(obj["high"].GetString()));
            }

            ChartValues<double> v = new ChartValues<double>();

            foreach(double i in d)
            {
                v.Add(i);
            }

            SeriesCollection.Add(
                new LineSeries
                {
                    Title = "2018",
                    Values = v
                }
            );

            DataContext = this;
        }

        /**
         * 
         */
        private async void GetCourse()
        {
            string c = currencies.SingleOrDefault(x => x.Key == cBoxSelectCurrency.SelectedIndex).Value;
            using (var webClient = new System.Net.Http.HttpClient())
            {
                var json = await webClient.GetStringAsync("https://www.bitmarket.pl/json/" + c + "PLN/ticker.json");
                txtCourse.Text = json.ToString();

                JsonObject j = JsonObject.Parse(json);
                txtCourse.Text = "Aktualny kurs: 1x " + c + " = " + j["bid"].ToString() + " PLN";
                course = Double.Parse(j["bid"].ToString());

                var day90 = await webClient.GetStringAsync("https://www.bitmarket.pl/graphs/" + c + "PLN/3m.json");
                JsonArray array = JsonArray.Parse(day90);

                // save data tmp
                SelectedCurrency.name = c;
                SelectedCurrency.id = cBoxSelectCurrency.SelectedIndex;
                SelectedCurrency.courseNow = course;

                FillChart(array);
            }
        }

        /**
         * 
         */
        private void cBoxSelectCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stackAfter.Visibility = Visibility.Visible;
            txtResult.Text = "";
            txtAmount.Text = "";

            GetCourse();
        }

        /**
         * 
         */
        private void btnSimulator_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SimulatorPage));
        }

        /**
         * 
         */
        private void btnCount_Click(object sender, RoutedEventArgs e)
        {
            Double amount = 0;
            amount = Double.Parse(txtAmount.Text);
            txtResult.Text = "";

            if(amount > 0)
            {
                string c = currencies.SingleOrDefault(x => x.Key == cBoxSelectCurrency.SelectedIndex).Value;
                txtResult.Text = "Wynik: " + amount + "x " + c + " = " + (course * amount) + " PLN";
            }
        }
    }
}
