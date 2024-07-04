using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using StockValuationApp.Entities.Stocks;


namespace StockPresentationLib.Views
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
        /// Initialize GUI elements and set content for TextBlocks.
        /// </summary>
        private void InitializeGUI()
        {
            lblRevenue.Text = "Revenue:";
            lblMarketValue.Text = "Market Cap:";
            lblYear.Text = "Year:";
            lblEbitda.Text = "EBITDA:";
            lblEbit.Text = "EBIT:";
            lblNetIncome.Text = "Net Income:";
            lblPrice.Text = "Stock Price:";
            lblNmbrOfShares.Text = "Number of Shares:";
            lblCapitalExp.Text = "Capital Expenditures:";
            lblTotAssets.Text = "Total Assets:";
            lblTotLiabilities.Text = "Total Liabilities:";
            lblDividends.Text = "Dividends:";
            lblShortTermDebt.Text = "Short Term Debt:";
            lblLongTermDebt.Text = "Long Term Debt:";
            lblCashAndEquiv.Text = "Cash and Equivalents:";
            lblOperCashflow.Text = "Operational Cashflow:";
            lblSolidity.Text = "Solidity:";
        }

        /// <summary>
        /// On click, create metric event args and instantiate 
        /// properties depending on current metric type.
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
            args.NumberOfShares = ValidateAndParseToDouble(tbxNmbrOfShares.Text);
            args.OperationalCashflow = ValidateAndParseToDouble(tbxOperCashflow.Text);
            args.TotalAssets = ValidateAndParseToDouble(tbxTotalAssets.Text);
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
                double value = 0.0;

                // Properties of type double
                if (property.Name != "Year")
                {
                    value = (double)property.GetValue(args, null);
                }
                // Properties of type int
                else
                {
                    value = (int)property.GetValue(args, null);
                }

                if (value == -1)
                {
                    MessageBox.Show("One or more input values are invalid.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else if (args.Year == 0)
                {
                    MessageBox.Show("Enter a valid year.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            // Publish event with the metric arguments
            stock.MetricsGiven?.Invoke(this, args);
            this.Close();
        }

        /// <summary>
        /// Try parse, string to double.
        /// </summary>
        /// <param name="strVal"></param>
        /// <returns>Parsed value or -1 for unparseable string and 0 for empty string.</returns>
        private double ValidateAndParseToDouble(string strVal)
        {
            if (string.IsNullOrWhiteSpace(strVal))
                return 0;

            bool ok = double.TryParse(strVal, out double val);

            return ok ? val : -1;
        }

        /// <summary>
        /// Try parse, string to int.
        /// </summary>
        /// <param name="strVal"></param>
        /// <returns>Parsed value or -1 for unparseable string and 0 for empty string.</returns>
        private int ValidateAndParseToInt(string strVal)
        {
            if (string.IsNullOrWhiteSpace(strVal))
                return 0;

            bool ok = int.TryParse(strVal, out int val);

            return ok ? val : -1;
        }
    }
}
