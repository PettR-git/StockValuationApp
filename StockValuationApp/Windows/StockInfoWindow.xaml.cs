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


namespace StockValuationApp.Windows
{
    /// <summary>
    /// Interaction logic for StockInfoWindow.xaml
    /// </summary>
    public partial class StockInfoWindow : Window
    {
        private readonly Stock stock;
        public StockInfoWindow(Stock stock)
        {
            InitializeComponent();
            InitializeGUI();
            this.stock = stock;
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

            args.Stock = stock;
            args.Year = ValidateAndParseToInt(tbxYear.Text);
            args.Revenue = ValidateAndParseToDouble(tbxRevenue.Text);
            args.MarketValue = ValidateAndParseToDouble(tbxMarketValue.Text);
            args.CapitalExpenditures = ValidateAndParseToDouble(tbxCapitalExpenditures.Text);
            args.NumberOfShares = ValidateAndParseToDouble(tbxCashAndEquiv.Text);
            args.OperationalCashflow = ValidateAndParseToDouble(tbxOperCashflow.Text);   
            args.TotalLiabilities = ValidateAndParseToDouble(tbxTotalLiabilities.Text);
            args.CashAndEquivalents = ValidateAndParseToDouble(tbxCashAndEquiv.Text);
            args.Dividends = ValidateAndParseToDouble(tbxDividends.Text);
            args.Ebit = ValidateAndParseToDouble(tbxEbit.Text);
            args.Ebitda = ValidateAndParseToDouble(tbxEbitda.Text);
            args.LongTermDebt = ValidateAndParseToDouble(tbxLongTermDebt.Text);
            args.ShortTermDebt = ValidateAndParseToDouble(tbxShortTermDebt.Text);
            args.NetIncome = ValidateAndParseToDouble(tbxNetIncome.Text);
            args.Price = ValidateAndParseToDouble(tbxStockPrice.Text);

            foreach (var property in metricProperties)
            {
                var value = 0.0;

                //Properties of type double
                if(property.Name != "Year")
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
            stock.MetricsGiven?.Invoke(this, args);
            this.Close();
        }

        /// <summary>
        /// Try parse, string to int
        /// </summary>
        /// <param name="strVal"></param>
        /// <returns>parsed value or -1 for unparseable string and 0 for empty string</returns>
        private double ValidateAndParseToDouble(string strVal)
        {
            if (strVal == string.Empty)
                return 0;

            bool ok = double.TryParse(strVal, out double val);

            if (ok)
                return val;
            else
                return -1;
        }

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
