# SQLite WAL Issue - Root Cause and Fix

## The Real Problem

The issue of deleted stocks reappearing after app restart was caused by **SQLite's Write-Ahead Logging (WAL) mode** leaving uncommitted transactions in the WAL files (`StockValuation.db-wal` and `StockValuation.db-shm`).

### How WAL Works
SQLite's WAL mode improves performance by:
1. Writing changes to a `.wal` (write-ahead log) file first
2. Using a `.shm` (shared memory) file to coordinate access
3. Periodically checkpointing (writing accumulated changes back to the main database)

### The Bug
When the application was terminating:
1. Deletion operations were written to the WAL file
2. The WAL files weren't being properly flushed/checkpointed to the main database
3. On app restart, SQLite would read from the stale main database (ignoring the uncommitted WAL)
4. The deleted stock would be restored because the deletion never made it to the main database file

## The Solution

### 1. **Clean Up Old WAL Files on Startup**
```csharp
// In App.xaml.cs OnStartup:
if (File.Exists(dbPath + "-wal"))
    File.Delete(dbPath + "-wal");
if (File.Exists(dbPath + "-shm"))
    File.Delete(dbPath + "-shm");
```

This ensures a fresh start without stale WAL data.

### 2. **Disable WAL Mode via Pragmas**
```csharp
// In AppDbContext.OnConfiguring:
PRAGMA journal_mode=DELETE;  // Switch from WAL to traditional mode
PRAGMA synchronous=FULL;      // Ensure all changes are flushed
PRAGMA busy_timeout=5000;     // Handle lock contention gracefully
```

**journal_mode=DELETE**: Uses the traditional DELETE journal mode instead of WAL. This ensures changes are written directly to the database file, not left in WAL files.

**synchronous=FULL**: Ensures all data is physically written to disk before a transaction is considered complete.

**busy_timeout=5000**: Prevents errors if the database is temporarily locked by another process.

### 3. **Proper Transaction Handling in Repository**
The repository already uses separate DbContext instances for deletions, which helps, but the pragmas above ensure those deletions are fully committed.

## Why This Solves the Problem

- **No more WAL limbo**: Deletions go directly to the database file, not to a WAL file
- **Immediate persistence**: FULL synchronous mode ensures data hits disk
- **Clean slate on startup**: Old WAL files are cleaned up, preventing resurrection of deleted data
- **Backward compatible**: Doesn't change the API, just fixes the underlying persistence

## Performance Trade-off

The traditional journal mode (DELETE) is slightly slower than WAL mode because:
- Every transaction must wait for a full synchronous write to complete
- No parallel readers during writes

However, for a stock valuation app with typical usage patterns, this is acceptable and ensures data integrity is never compromised.

## Testing

To verify this fix works:
1. Add a stock ? Close app (stock should persist)
2. Delete the stock ? Close app immediately (stock should NOT come back on restart)
3. Repeat multiple times to ensure consistency

If you see deleted stocks still coming back, check:
- The database file's timestamp (it should update when you make changes)
- The WAL files (they should be cleaned up or minimal after the fix)
