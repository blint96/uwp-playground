using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization.DateTimeFormatting;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Kryptowaluty
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SimulatorPage : Page
    {
        private StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        private ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;

        public SimulatorPage()
        {
            this.InitializeComponent();

            //GetFromFile();
            GetValues();

            txtAmountName.Text = "Ilość posiadanych " + SelectedCurrency.name;
        }

        static public async void Show(string mytext)
        {
            var dialog = new MessageDialog(mytext, "Informacja");
            await dialog.ShowAsync();
        }

        private void SellAmount()
        {
            Int32 amount = Int32.Parse(txtSellAmount.Text);
            if (amount <= 0)
            {
                txtSellAmount.Text = "";
                Show("Wprowadzono nieprawidłową wartość!");
                return;
            }

            try
            {
                Int32 currentAmount = Int32.Parse(settings.Values[SelectedCurrency.name + "_Amount"].ToString());
                if(currentAmount - amount < 0)
                {
                    txtSellAmount.Text = "";
                    Show("Wprowadzono nieprawidłową wartość!");
                    return;
                }

                Double income = amount * SelectedCurrency.courseNow;
                currentAmount -= amount;
                settings.Values[SelectedCurrency.name + "_Amount"] = currentAmount;
                settings.Values[SelectedCurrency.name + "_Income"] = Double.Parse(settings.Values[SelectedCurrency.name + "_Income"].ToString()) + income;
                settings.Values[SelectedCurrency.name + "_Cost"] = Double.Parse(settings.Values[SelectedCurrency.name + "_Cost"].ToString()) - income;

                txtIncome.Text = settings.Values[SelectedCurrency.name + "_Income"].ToString() + " PLN";
                txtCost.Text = settings.Values[SelectedCurrency.name + "_Cost"].ToString() + " PLN";
            } catch(Exception e) { }
        }

        private void BuyAmount()
        {
            Int32 amount = Int32.Parse(txtBuyAmount.Text);
            if(amount <= 0)
            {
                txtBuyAmount.Text = "";
                Show("Wprowadzono nieprawidłową wartość!");
                return;
            }

            try
            {
                // amount
                Int32 tmp_amount = Int32.Parse(settings.Values[SelectedCurrency.name + "_Amount"].ToString()) + amount;
                settings.Values[SelectedCurrency.name + "_Amount"] = tmp_amount;
                txtAmount.Text = tmp_amount.ToString();

                // value
                Double cost = tmp_amount * SelectedCurrency.courseNow;
                settings.Values[SelectedCurrency.name + "_Cost"] = Double.Parse(settings.Values[SelectedCurrency.name + "_Cost"].ToString()) + cost;
                txtCost.Text = settings.Values[SelectedCurrency.name + "_Cost"].ToString() + " PLN";
            } catch (Exception e) { }
        }

        private void GetValues()
        {
            try
            {
                var currencyAmount = settings.Values[SelectedCurrency.name + "_Amount"];
                var currencyCost = settings.Values[SelectedCurrency.name + "_Cost"];
                var currencyIncome = settings.Values[SelectedCurrency.name + "_Income"];   

                if (currencyAmount == null)
                {
                    currencyAmount = settings.Values[SelectedCurrency.name + "_Amount"] = 0;
                }
                if (currencyCost == null)
                {
                    currencyCost = settings.Values[SelectedCurrency.name + "_Cost"] = 0;
                }
                if (currencyIncome == null)
                {
                    currencyIncome = settings.Values[SelectedCurrency.name + "_Income"] = 0;
                }

                var currencyValue = Int32.Parse(currencyAmount.ToString()) * SelectedCurrency.courseNow;

                txtAmount.Text = currencyAmount.ToString();
                txtValue.Text = currencyValue.ToString() + " PLN";
                txtCost.Text = currencyCost.ToString() + " PLN";
                txtIncome.Text = currencyIncome.ToString() + " PLN";
            }
            catch (Exception e) { }
        }

        /*private async void GetFromFile()
        {
            try
            {
                StorageFile file = await localFolder.GetFileAsync("dataFile.txt");
                String value = await FileIO.ReadTextAsync(file);
                textBoxTest.Text = value;
            }
            catch (Exception e)
            {

            }
        }

        private async void SaveToFile()
        {
            try
            {
                StorageFile file = await localFolder.CreateFileAsync("dataFile.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, textBoxTest.Text);
            }
            catch(Exception e)
            {

            }
        }*/

        private void btnMain_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void btnBuy_Click(object sender, RoutedEventArgs e)
        {
            BuyAmount();
        }

        private void btnSell_Click(object sender, RoutedEventArgs e)
        {
            SellAmount();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            settings.Values[SelectedCurrency.name + "_Amount"] = 0;
            settings.Values[SelectedCurrency.name + "_Cost"] = 0;
            settings.Values[SelectedCurrency.name + "_Income"] = 0;

            txtAmount.Text = "";
            txtValue.Text = "";
            txtCost.Text = "";
            txtIncome.Text = "";
        }
    }
}
