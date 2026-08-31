# Debug: WeeklyStockPricesGiven Event Not Firing

## Symptom
The `OnWeeklyStockPricesGiven()` handler in `AnalysisVM` is never called, even though you're subscribing to the event.

## Root Cause Analysis

### The Event Flow
```
1. User selects stock in Home view
                ?
2. HomeVM.UpdateCurrentStock() fires UpdateStockEvent
                ?
3. NavigationVM receives event and sets _analysisVM.Stock = _currentStock
                ?
4. AnalysisVM.Stock setter subscribes: _stock.WeeklyStockPricesGiven += OnWeeklyStockPricesGiven
                ?
5. BUT... the event SHOULD have been fired earlier during data import!
                ?
6. If event already fired (during data import), subscription is too late!
```

### The Problem Timeline

**Scenario A: User imports data FIRST, then selects stock (WORKS)**
```
1. User clicks "Import Stock Data" ? GetMetricVals() called
2. StockManager fires WeeklyStockPricesGiven event
3. BUT... AnalysisVM not subscribed yet (user hasn't switched to Analysis tab)
4. ? Event fires before handler is attached ? EVENT LOST
```

**Scenario B: User selects stock FIRST, then imports (WORKS)**
```
1. User selects stock ? AnalysisVM subscribes to event
2. User clicks "Import Stock Data" ? GetMetricVals() called
3. StockManager fires WeeklyStockPricesGiven event
4. ? AnalysisVM handler receives it
```

**Scenario C: Stocks loaded from database, no manual import (FAILS)**
```
1. App starts ? LoadAllAsync() loads stocks from database
2. ? NO WeeklyStockPricesGiven event fired!
3. User selects stock ? AnalysisVM subscribes
4. ? No event to receive ? handler never called
```

---

## Solution: Lazy Load Price Data

We need to fire the event WHEN the chart is first displayed, not just when importing data. Here's the fix:

### Step 1: Add a method to load price data on-demand

Add this to `StockManager.cs`:

```csharp
/// <summary>
/// Load weekly prices for a specific stock (used for on-demand chart loading)
/// </summary>
public async Task LoadWeeklyPricesAsync(Stock stock)
{
    if (stock == null) throw new ArgumentNullException(nameof(stock));

    try
    {
        System.Diagnostics.Debug.WriteLine($"[StockManager] LoadWeeklyPricesAsync called for {stock.Ticker}");

        // Fetch price data from API
        var jObjs = await uriManager.GetFinanceData(stock.Ticker, FinanceCategory.StockPrice, PeriodTypes.annual);

        // Process and fire event
        AssignWeeklyPrices(jObjs, stock);
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"[StockManager] Error loading weekly prices: {ex.Message}");
    }
}
```

### Step 2: Call this when stock is selected in AnalysisVM

Modify `AnalysisVM.cs` to load data when stock changes:

```csharp
public Stock Stock
{
    get => _stock;
    set
    {
        if (_stock != value)
        {
            if (_stock != null)
            {
                _stock.PropertyChanged -= OnStockPropertyChanged;
                _stock.WeeklyStockPricesGiven -= OnWeeklyStockPricesGiven;
            }

            _stock = value;

            if (_stock != null)
            {
                _stock.PropertyChanged += OnStockPropertyChanged;
                _stock.WeeklyStockPricesGiven += OnWeeklyStockPricesGiven;

                System.Diagnostics.Debug.WriteLine($"[AnalysisVM] Stock changed to {_stock.Ticker}, subscribing to event");

                // Load price data if not already loaded
                LoadPriceDataAsync();
            }

            OnPropertyChanged();
            ParseAgentJson();
            RefreshCandleData();
        }
    }
}

private async void LoadPriceDataAsync()
{
    if (_stock == null) return;

    try
    {
        System.Diagnostics.Debug.WriteLine($"[AnalysisVM] Loading price data for {_stock.Ticker}");

        // Try to get StockManager from dependency - you'll need to inject it
        // Or get it from the current stock if it's stored

        // For now, just wait for the event to be fired externally
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"[AnalysisVM] Error: {ex.Message}");
    }
}
```

### Step 3: Inject StockManager into AnalysisVM

Update `AnalysisVM` constructor:

```csharp
private readonly StockManager _stockManager;

public AnalysisVM(StockManager stockManager)
{
    _stockManager = stockManager ?? throw new ArgumentNullException(nameof(stockManager));
    _selectedInterval = TimeInterval.OneYear;
    _allBars = Array.Empty<StockBar>();
    _filteredBars = Array.Empty<StockBar>();
    InitializeChart();
}
```

### Step 4: Update LoadPriceDataAsync

```csharp
private async void LoadPriceDataAsync()
{
    if (_stock == null || _stockManager == null) return;

    try
    {
        System.Diagnostics.Debug.WriteLine($"[AnalysisVM] Loading price data for {_stock.Ticker}");
        await _stockManager.LoadWeeklyPricesAsync(_stock);
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"[AnalysisVM] Error: {ex.Message}");
    }
}
```

---

## How to Test

### Test 1: Check Debug Output
1. Build the solution
2. Run the app
3. Open Debug Output window (Debug ? Windows ? Output)
4. Select a stock in the Technical tab
5. Look for these messages:
   ```
   [AnalysisVM] Stock changed to AAPL, subscribing to event
   [AnalysisVM] Loading price data for AAPL
   [StockManager] LoadWeeklyPricesAsync called for AAPL
   [StockManager] OnWeeklyPricesLoaded called. Invoking event with 1825 bars for AAPL
   [AnalysisVM] OnWeeklyStockPricesGiven fired! Received 1825 bars for AAPL
   ```

### Test 2: Verify Chart Updates
1. Select different stocks
2. See the candlestick chart populate with data
3. Change time intervals
4. Chart should filter and update

---

## Common Pitfalls

? **Pitfall 1: Event fired before handler subscribed**
- Cause: `GetMetricVals()` fires event before `AnalysisVM` subscribes
- Fix: Lazy-load data when chart is displayed

? **Pitfall 2: No data loaded at startup**
- Cause: `LoadAllAsync()` doesn't fire weekly price events
- Fix: Fire event on-demand when stock is selected for viewing

? **Pitfall 3: Handler not actually attached**
- Cause: `WeeklyStockPricesGiven` is null or not properly defined
- Fix: Verify `EventHandler<WeeklyStockPricesEventArgs>` is defined in Stock.cs

? **Pitfall 4: Wrong event args type**
- Cause: Passing `EventArgs` instead of `WeeklyStockPricesEventArgs`
- Fix: Ensure all event firing uses correct type

---

## Full Solution Code

### Complete Modified LoadPriceDataAsync

```csharp
private async void LoadPriceDataAsync()
{
    if (_stock == null || _stockManager == null)
    {
        System.Diagnostics.Debug.WriteLine("[AnalysisVM] Cannot load price data - stock or manager is null");
        return;
    }

    try
    {
        System.Diagnostics.Debug.WriteLine($"[AnalysisVM] Loading price data for {_stock.Ticker}");

        // Load the weekly prices - this will trigger the WeeklyStockPricesGiven event
        await _stockManager.LoadWeeklyPricesAsync(_stock);

        System.Diagnostics.Debug.WriteLine($"[AnalysisVM] Price data load initiated for {_stock.Ticker}");
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"[AnalysisVM] Error loading price data: {ex.Message}\n{ex.StackTrace}");
    }
}
```

### Complete Modified Stock Setter

```csharp
public Stock Stock
{
    get => _stock;
    set
    {
        if (_stock != value)
        {
            // Unsubscribe from old stock
            if (_stock != null)
            {
                _stock.PropertyChanged -= OnStockPropertyChanged;
                _stock.WeeklyStockPricesGiven -= OnWeeklyStockPricesGiven;
                System.Diagnostics.Debug.WriteLine($"[AnalysisVM] Unsubscribed from {_stock.Ticker}");
            }

            _stock = value;

            // Subscribe to new stock
            if (_stock != null)
            {
                _stock.PropertyChanged += OnStockPropertyChanged;
                _stock.WeeklyStockPricesGiven += OnWeeklyStockPricesGiven;
                System.Diagnostics.Debug.WriteLine($"[AnalysisVM] Subscribed to {_stock.Ticker}, loading price data...");

                // Load price data immediately
                LoadPriceDataAsync();
            }

            OnPropertyChanged();
            ParseAgentJson();
            RefreshCandleData();
        }
    }
}
```

---

## Expected Behavior After Fix

1. ? User selects stock in Home view
2. ? Navigates to Analysis > Technical tab
3. ? AnalysisVM.Stock setter fires
4. ? LoadPriceDataAsync() called
5. ? StockManager.LoadWeeklyPricesAsync() fetches data
6. ? AssignWeeklyPrices() processes data
7. ? OnWeeklyPricesLoaded() fires event
8. ? AnalysisVM handler receives event
9. ? Chart updates with candlesticks
10. ? User sees data!

---

## Debugging Checklist

- [ ] Added debug output to Stock.WeeklyStockPricesGiven definition
- [ ] Added debug output to AnalysisVM.Stock setter
- [ ] Added debug output to OnWeeklyStockPricesGiven handler
- [ ] Added debug output to StockManager.OnWeeklyPricesLoaded
- [ ] Check Output window shows all expected debug messages
- [ ] Chart populates with data after selecting stock
- [ ] Different stocks show different data
- [ ] Time intervals filter correctly

---

## Next Steps

1. Apply the debug logging changes I've made
2. Build and run the application
3. Check the Debug Output window
4. Look for the debug messages to trace where the event is failing
5. Come back with the output and I'll help you fix the specific issue!

The debug output will show you exactly where the problem is occurring.
