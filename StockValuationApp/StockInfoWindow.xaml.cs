using StockValuationApp.Entities.Enums;
using StockValuationApp.Entities.Stocks;
using StockValuationApp.Main.Enums;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace StockValuationApp
{
    /// <summary>
    /// Interaction logic for StockInfoWindow.xaml
    /// </summary>
    public partial class StockInfoWindow : Window
    {
        public EventHandler<MetricEventArgs> MetricsGiven;
        public StockInfoWindow()
        {
            InitializeComponent();
            InitializeGUI();
        }

        /// <summary>
        /// Label content differ depending on metric type
        /// </summary>
        private void InitializeGUI()
        {
        }

        /// <summary>
        /// On click, create metric event args and intantiate 
        /// properties depending on current metric type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            MetricEventArgs args = new MetricEventArgs();
            PropertyInfo[] allEventProperties = typeof(MetricEventArgs).GetProperties();
            PropertyInfo[] metricProperties = allEventProperties.Skip(1).ToArray();

            args.Year = ValidateAndParseToInt(tbxYear.Text);
            args.Revenue = ValidateAndParseToInt(tbxRevenue.Text);
            args.MarketValue = ValidateAndParseToInt(tbxMarketValue.Text);
            args.CapitalExpenditures = ValidateAndParseToInt(tbxCapitalExpenditures.Text);
            args.NumberOfShares = ValidateAndParseToInt(tbxCashAndEquiv.Text);
            args.OperationalCashflow = ValidateAndParseToInt(tbxOperCashflow.Text);   
            args.TotalLiabilities = ValidateAndParseToInt(tbxTotalLiabilities.Text);
            args.CashAndEquivalents = ValidateAndParseToInt(tbxCashAndEquiv.Text);
            args.Dividends = ValidateAndParseToInt(tbxDividends.Text);
            args.Ebit = ValidateAndParseToInt(tbxEbit.Text);
            args.Ebitda = ValidateAndParseToInt(tbxEbitda.Text);
            args.LongTermDebt = ValidateAndParseToInt(tbxLongTermDebt.Text);
            args.ShortTermDebt = ValidateAndParseToInt(tbxShortTermDebt.Text);
            args.NetIncome = ValidateAndParseToInt(tbxNetIncome.Text);
            args.Price = ValidateAndParseToInt(tbxStockPrice.Text);

            foreach (var property in metricProperties)
            {
                var value = 0.0;

                //Properties of type double
                if(property.Name == "Dividends")
                {
                     value = (double)property.GetValue(args, null);
                }
                //Properties of type int
                else
                {
                    value = (int)property.GetValue(args, null);
                }

                if (value == -1)
                {
                    MessageBox.Show("One or more input values are invalid.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }       
                else if(args.Year == 0)
                {
                    MessageBox.Show("Enter a valid year.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            //Publish event with the metric arguments
            MetricsGiven?.Invoke(this, args);
            this.Close();
        }

        /// <summary>
        /// Try parse, string to int
        /// </summary>
        /// <param name="strVal"></param>
        /// <returns>parsed value or -1 for unparseable string and 0 for empty string</returns>
        private int ValidateAndParseToInt(string strVal)
        {
            if (strVal == string.Empty)
                return 0;

            bool ok = int.TryParse(strVal, out int val);

            if (ok)
                return val;
            else
                return -1;
        }
    }
}
