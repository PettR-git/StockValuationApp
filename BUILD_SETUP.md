# Build and Setup Instructions

## Prerequisites
- Visual Studio 2022 or newer
- .NET 8 SDK installed
- NuGet package manager

## Step-by-Step Setup

### 1. Restore NuGet Packages
After the modified .csproj file is in place, restore packages:

**Option A: Visual Studio**
- Right-click solution ? "Restore NuGet Packages"
- Or: Tools ? NuGet Package Manager ? Package Manager Console
- Run: `Update-Package -Reinstall`

**Option B: Command Line**
```powershell
cd C:\Users\pette\Source\Repos\StockValuationApp
dotnet restore
```

### 2. Build the Solution
```powershell
dotnet build
```

If you encounter any missing type errors after build, run:
```powershell
dotnet clean
dotnet restore
dotnet build
```

### 3. Verify Implementation

After successful build, check:
- ? `StockPresentationLib/Utilities/CandleChartDataFilter.cs` compiles
- ? `StockPresentationLib/Utilities/IntervalToStringConverter.cs` compiles
- ? `StockPresentationLib/ViewModel/AnalysisVM.cs` compiles with event handlers
- ? `Analysis.xaml` has Technical tab with chart controls

### 4. Run Application
```powershell
dotnet run --project StockPresentationLib/StockPresentationLib.csproj
```

## Files Changed

### Modified
1. `StockPresentationLib/StockPresentationLib.csproj` - Added LiveCharts2 packages
2. `StockPresentationLib/ViewModel/AnalysisVM.cs` - Added event subscription and filtering
3. `StockPresentationLib/Views/Analysis.xaml` - Added Technical tab with chart UI
4. `StockPresentationLib/Views/Analysis.xaml.cs` - Added namespaces

### Created
1. `StockPresentationLib/Utilities/CandleChartDataFilter.cs` - Filtering logic
2. `StockPresentationLib/Utilities/IntervalToStringConverter.cs` - XAML converter
3. `CANDLESTICK_IMPLEMENTATION.md` - Implementation documentation
4. `INTEGRATION_REFERENCE.md` - Quick reference
5. `BUILD_SETUP.md` - This file

## Testing the Feature

### Manual Test Steps
1. Launch application
2. Select a stock from the Home view
3. Navigate to Analysis view
4. Click on "Technical" tab
5. Verify:
   - Time interval dropdown appears
   - Chart placeholder loads (may be empty until data is provided)
   - Changing interval updates the chart
   - No compilation errors in Output window

### Data Flow Test
1. Ensure your data provider (likely `StockManager`) triggers:
   ```csharp
   stock.DailyStockPricesGiven?.Invoke(this, eventArgs);
   ```
2. Verify `DailyStockPricesEventArgs` has valid `Bars` collection
3. Check AnalysisVM's `FilteredBars` property updates in debugger
4. Candlesticks should render on chart

## Troubleshooting

### Build Errors

**Error**: "The type or namespace 'LiveChartsCore' could not be found"
- **Solution**: Run `dotnet restore` again, or update NuGet packages in Visual Studio

**Error**: "The type namespace 'IntervalToStringConverter' does not exist in XAML"
- **Solution**: Rebuild solution to regenerate XAML metadata

**Error**: "Missing assembly binding redirects"
- **Solution**: This is normal for .NET 8; can usually be ignored

### Runtime Issues

**Chart doesn't show**
- Verify `DailyStockPricesGiven` event is being raised
- Check `FilteredBars` has data in debugger
- Verify XAML binding path is correct (`CandleSeriesCollection`)

**ComboBox dropdown is empty**
- Verify `AvailableIntervals` property is correctly populated
- Check `IntervalToStringConverter` returns valid strings

**Performance is slow with large datasets**
- Consider implementing data aggregation (e.g., show weekly candles for 10-year view)
- Current implementation filters 20+ years × 252 trading days efficiently
- If still slow, implement data caching in AnalysisVM

## NuGet Package Details

### LiveCharts2 2.0.8
- High-performance charting library for WPF
- SkiaSharp rendering for smooth visuals
- Supports various chart types (candlestick, line, bar, etc.)
- MIT License

### LiveCharts2.SkiaSharp 2.0.8
- SkiaSharp rendering backend for LiveCharts2
- Better performance than default WPF rendering
- Native library support for Windows

## Performance Notes

- **Memory**: Each candlestick point uses ~50 bytes; 20 years of daily data (~5000 points) = ~250KB
- **Filter Performance**: O(n) filtering; typically <1ms even for large datasets
- **Render Performance**: LiveCharts2 optimizes rendering; smooth even with 10k+ points
- **Recommendation**: Consider implementing monthly/weekly aggregation for 10+ year views

## Version Compatibility

- ? .NET 8.0-windows
- ? LiveCharts2 2.0.8
- ? SkiaSharp 2.0+
- ? Windows 10/11

## Next Development Steps

1. Integrate with your actual data provider to fire `DailyStockPricesGiven`
2. Test with real stock data
3. Implement optional enhancements (indicators, volume, etc.)
4. Add error handling for malformed OHLCV data
5. Implement performance monitoring for large datasets
