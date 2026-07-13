# Troubleshooting the Stock Deletion Issue

## If Stocks Still Reappear After the Fix

### Step 1: Delete the Database File
The old database file with WAL history might still cause issues. Delete it and let the app recreate it:

```powershell
# Navigate to the output folder
cd "C:\Users\pette\Source\Repos\StockValuationApp\StockPresentationLib\bin\Debug\net8.0-windows\"

# Delete all database files
Remove-Item StockValuation.db* -Force
```

Then run the app again. The database will be recreated from scratch with proper settings.

### Step 2: Verify Debug Output
When you delete a stock, you should see in the debug output:

```
[App] Saving 0 stocks to database...
[EF Repository] SaveAllAsync called with 0 stocks
[EF Repository] Existing IDs in DB: 1
[EF Repository] Incoming IDs: 
[EF Repository] IDs to remove: 1
[EF Repository] Deleting 1 stocks
  - Deleting: YourStockName (ID: 1)
[EF Repository] Deletion committed
[App] Save completed
```

If you don't see "IDs to remove" with the stock ID, the deletion logic isn't detecting the stock removal.

### Step 3: Check Database Pragmas
Run this SQL query on the database to verify settings:

```sql
PRAGMA journal_mode;
-- Should return: delete

PRAGMA synchronous;
-- Should return: 3 (FULL)
```

If these don't return the expected values, the pragmas might not be executing.

### Step 4: Verify WAL Files Are Being Cleaned
After running the app and deleting a stock, check if the WAL files are small or missing:

```powershell
Get-ChildItem "C:\Users\pette\Source\Repos\StockValuationApp\StockPresentationLib\bin\Debug\net8.0-windows\StockValuation*"
```

Expected result:
- `StockValuation.db`: Main database file (grows/shrinks as you add/delete)
- `StockValuation.db-wal`: Should be very small or non-existent (< 1KB)
- `StockValuation.db-shm`: Should not exist

If WAL files are large (> 100KB), transactions are being cached instead of committed.

### Step 5: Manual Database Checkpoint
Add this code temporarily to force a checkpoint after deletion:

```csharp
// In EfStockRepository after SaveChangesAsync
using (var ctx = _factory.CreateDbContext())
{
    var connection = ctx.Database.GetDbConnection() as SqliteConnection;
    if (connection != null)
    {
        connection.Open();
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "PRAGMA wal_checkpoint(RESTART);";
            cmd.ExecuteNonQuery();
        }
        connection.Close();
    }
}
```

### Step 6: Test with Fresh Database
1. Delete `StockValuation.db*` files
2. Delete any `*.db-wal` or `*.db-shm` files
3. Rebuild and run the app
4. Create ONE test stock
5. Close the app (watch for "[App] Save completed" in debug output)
6. Delete the main `StockValuation.db` file
7. Rename `StockValuation.db-wal` to `StockValuation.db` if it exists
8. Restart the app

If the test stock appears, the WAL file has stale data. If it doesn't, the fix is working.

## Common Issues and Solutions

### Issue: Database is locked
**Cause:** Multiple contexts trying to access simultaneously
**Solution:** Already handled by `PRAGMA busy_timeout=5000`

### Issue: Pragmas not being applied
**Cause:** Connection not open when pragmas execute
**Solution:** Check that `Database.GetDbConnection()` is not returning null

### Issue: Still seeing large WAL files
**Cause:** WAL checkpoint not happening
**Solution:** Might need to call `PRAGMA wal_checkpoint(TRUNCATE);` explicitly

### Issue: Performance is very slow
**Cause:** `PRAGMA synchronous=FULL` is waiting for disk I/O
**Solution:** This is expected. For better performance, switch back to `synchronous=NORMAL` (less safe)

## Nuclear Option
If nothing else works, regenerate the database completely:

```powershell
# Delete everything database-related
Remove-Item "C:\Users\pette\Source\Repos\StockValuationApp\StockPresentationLib\bin\Debug\net8.0-windows\StockValuation*" -Force

# Clean and rebuild
cd "C:\Users\pette\Source\Repos\StockValuationApp"
dotnet clean
dotnet build
```

Then run the app fresh and test again.

## Verifying the Fix Works

Once you believe the fix is working, run this test sequence:

1. **Create**: Add stock "TEST" (ticker: T1)
2. **Close**: Exit app, check debug output for "Save completed"
3. **Reopen**: Start app, verify "TEST" is still there
4. **Delete**: Delete stock "TEST"
5. **Close**: Exit app immediately, check for "Deletion committed"
6. **Reopen**: Start app, verify "TEST" is completely gone
7. **Repeat**: Do steps 1-6 two more times
8. **Mixed**: Add 3 stocks, delete 1, add 2 more, delete 1
9. **Reopen**: Verify count is 3 (original 3 - 1 = 2, + 2 = 4, - 1 = 3)

If all tests pass, the fix is working correctly!
