using Microsoft.VisualBasic;
using StockPresentationLib.Plot;
using StockPresentationLib.ViewModel;
using StockValuationApp.Entities.Enums;
using StockValuationApp.Entities.Stocks;
using StockValuationApp.Entities.Stocks.Metrics;
using StockValuationApp.Entities.Stocks.Metrics.Earnings;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StockPresentationLib.Views
{
    /// <summary>
    /// Interaction logic for Earnings.xaml
    /// </summary>
    public partial class Earnings : UserControl
    {
        private PlotEarnings plotEarnings;
        public Earnings()
        {
            InitializeComponent();

            this.DataContextChanged += Earnings_DataContextChanged;
        }

        private void Earnings_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(this.DataContext is EarningsVM earningsVM)
            {
                if (earningsVM.Stock != null)
                {
                    var financials = earningsVM.Stock.YearlyFinancials;

                    if (financials.Count() > 0 && earningsVM.PrevPlotEarnings?.StockStr != earningsVM.Stock.ToString())
                    {
                        earningsVM.YearlyFinancials.Clear();

                        //Populate viewmodel with financial data, such as financial margins for datagrid
                        foreach (var financial in financials)
                        {
                            earningsVM.YearlyFinancials.Add(financial);
                        }
                    }
                }   

                EarningsPlot();
            }
        }

        private void EarningsPlot()
        {
            if (this.DataContext is EarningsVM earningsVM)
            {
                if (earningsVM.Stock != null)
                {
                    if (earningsVM.Stock.YearlyFinancials.Count > 0)
                    {
                        if (earningsVM.PrevPlotEarnings == null || earningsVM.Stock.ToString() != earningsVM.PrevPlotEarnings.StockStr)
                        {
                            bool newPlot = true;

                            if (earningsVM.PrevPlotEarnings?.StockStr != null)
                                newPlot = false;

                            plotEarnings = new PlotEarnings(WpfPlot1, earningsVM.Stock.ToString(), earningsVM.YearlyFinancials.ToList());

                            earningsVM.PrevPlotEarnings = plotEarnings;
                            PlotRevenueAndEarnings();

                            if (newPlot)
                            {
                                SetInitialCbxValues();
                            }
                        }
                        else
                        {
                            plotEarnings = earningsVM.PrevPlotEarnings;
                            plotEarnings.UpdatePlotAndStock(WpfPlot1, earningsVM.PrevPlotEarnings.StockStr);
                            PlotRevenueAndEarnings();
                        }
                    }
                }
            }
        }

        private void TabControl_Changed(object sender, RoutedEventArgs e)
        {
        
        }

        private void SetInitialCbxValues()
        {
            cbxRevGrowth.IsChecked = true;
            cbxEbitdaGrowth.IsChecked = false;
            cbxEbitGrowth.IsChecked = true;
            cbxNetIncGrowth.IsChecked = false;
        }

        private void PlotRevenueAndEarnings()
        {
            plotEarnings.PlotRevenueAndEarnings();
        }

        public void RenderPlot()
        {
            plotEarnings.RenderPlot();
        }

        private void cbxEbitdaGrowth_Checked(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is EarningsVM earningsVM)
            {
                if (cbxEbitdaGrowth.IsChecked == true)
                {
                    plotEarnings.PlotEbitdaGrowth(true);
                    earningsVM.CbxEbitdaGrwthChecked = true;
                }
                else
                {
                    plotEarnings.PlotEbitdaGrowth(false);
                    earningsVM.CbxEbitdaGrwthChecked = false;
                }
            }
        }

        private void cbxEbitGrowth_Checked(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is EarningsVM earningsVM)
            {
                if (cbxEbitGrowth.IsChecked == true)
                {
                    plotEarnings.PlotEbitGrowth(true);           
                    earningsVM.CbxEbitGrwthChecked= true;
                }
                else
                {
                    plotEarnings.PlotEbitGrowth(false);
                    earningsVM.CbxEbitGrwthChecked = false;
                }
            }
        }

        private void cbxRevGrowth_Checked(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is EarningsVM earningsVM)
            {
                if (cbxRevGrowth.IsChecked == true)
                {
                    plotEarnings.PlotRevenueGrowth(true);
                    earningsVM.CbxRevGrwthChecked = true;
                }                   
                else
                {
                    plotEarnings.PlotRevenueGrowth(false);
                    earningsVM.CbxRevGrwthChecked=false;
                }                
            }
        }

        private void cbxNetIncGrowth_Checked(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is EarningsVM earningsVM)
            {
                if (cbxNetIncGrowth.IsChecked == true)
                {
                    plotEarnings.PlotNetIncomeGrowth(true);
                    earningsVM.CbxNetIncGrwthChecked = true;
                }
                else
                {
                    plotEarnings.PlotNetIncomeGrowth(false);
                    earningsVM.CbxNetIncGrwthChecked= false;
                }
            }
        }
    }
}
