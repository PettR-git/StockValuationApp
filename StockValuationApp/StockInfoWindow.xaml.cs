﻿using StockValuationApp.Entities.Enums;
using StockValuationApp.Entities.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StockValuationApp
{
    /// <summary>
    /// Interaction logic for StockInfoWindow.xaml
    /// </summary>
    public partial class StockInfoWindow : Window
    {
        private MetricType metricType;
        private Stock stock;
        public EventHandler<MetricEventArgs> MetricsGiven;
        public StockInfoWindow(MetricType metricType, Stock stock)
        {
            InitializeComponent();
            this.metricType = metricType;
            this.stock = stock;
            InitializeGUI();
        }

        private void InitializeGUI()
        {
            switch (metricType)
            {
                case MetricType.PriceToEarnings:
                    lblFirstMetric.Content = "Price:";
                    lblSecondMetric.Content = "Number of Shares:";
                    lblThirdMetric.Content = "Net Income:";
                    break;
                case MetricType.EvEbitda:
                    lblFirstMetric.Content = "Market value:";
                    lblSecondMetric.Content = "Net Debt:";
                    lblThirdMetric.Content = "EBITDA:";
                    break;
                case MetricType.EvEbit:
                    lblFirstMetric.Content = "Market value:";
                    lblSecondMetric.Content = "Net Debt:";
                    lblThirdMetric.Content = "EBIT:";
                    break;
                case MetricType.NetDebtToEbitda:
                    lblFirstMetric.Content = "";
                    lblSecondMetric.Content = "Net Debt:";
                    lblThirdMetric.Content = "EBITDA";
                    tbxFirstMetric.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            MetricEventArgs args = new MetricEventArgs();
            int firstMetric= 0, secondMetric= 0, thirdMetric = 0;

            switch (metricType)
            {
                case MetricType.PriceToEarnings:
                    firstMetric = ParseOk(tbxFirstMetric.Text);
                    secondMetric = ParseOk(tbxSecondMetric.Text);
                    thirdMetric = ParseOk(tbxThirdMetric.Text);

                    args.Price = firstMetric;
                    args.NumberOfShares = secondMetric;
                    args.NetIncome = thirdMetric;
                    break;

                case MetricType.EvEbitda:
                    firstMetric = ParseOk(tbxFirstMetric.Text);
                    secondMetric = ParseOk(tbxSecondMetric.Text);
                    thirdMetric = ParseOk(tbxThirdMetric.Text);

                    args.MarketValue = firstMetric;
                    args.NetDebt = secondMetric;
                    args.Ebitda = thirdMetric;
                    break;

                case MetricType.EvEbit:
                    firstMetric = ParseOk(tbxFirstMetric.Text);
                    secondMetric = ParseOk(tbxSecondMetric.Text);
                    thirdMetric = ParseOk(tbxThirdMetric.Text);

                    args.MarketValue = firstMetric;
                    args.NetDebt = secondMetric;
                    args.Ebit = thirdMetric;
                    break;

                case MetricType.NetDebtToEbitda:
                    secondMetric = ParseOk(tbxSecondMetric.Text);
                    thirdMetric = ParseOk(tbxThirdMetric.Text);

                    args.NetDebt = secondMetric;
                    args.Ebitda = thirdMetric;
                    break;
                default:
                    Console.WriteLine("Issue with invalid metrictype");
                    break;
            }

            if (firstMetric == -1 || secondMetric == -1 || thirdMetric == -1)
            {
                MessageBox.Show("One or more input values are invalid.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            args.MetricType = metricType;
            args.Year = tbxYear.Text;
            args.Stock = stock;

            MetricsGiven?.Invoke(this, args);
            this.Close();
        }

        private int ParseOk(string strVal)
        {
            bool ok = int.TryParse(strVal, out int val);

            if (ok)
                return val;
            else
            {
                return -1;
            }
        }
    }
}
