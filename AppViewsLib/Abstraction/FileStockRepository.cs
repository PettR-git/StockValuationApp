using StockValuationApp.Entities.Stocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StockLib.Abstraction
{
    public class FileStockRepository : IStockRepository
    {
        private readonly string _filePath;

        public FileStockRepository()
        {
            // store next to app executable
            var appFolder = AppContext.BaseDirectory;
            _filePath = Path.Combine(appFolder, "stocks.json");
        }

        public async Task<List<Stock>> LoadAllAsync()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return new List<Stock>();

                var json = await File.ReadAllTextAsync(_filePath).ConfigureAwait(false);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                };

                var list = JsonSerializer.Deserialize<List<Stock>>(json, options);
                return list ?? new List<Stock>();
            }
            catch(Exception e)
            {
                Debug.WriteLine($"Error loading stocks from file: {e.Message}");
                // swallow and return empty list to avoid startup crash
                return new List<Stock>();
            }
        }

        public async Task SaveAllAsync(IEnumerable<Stock> stocks)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                var json = JsonSerializer.Serialize(stocks, options);
                await File.WriteAllTextAsync(_filePath, json).ConfigureAwait(false);
            }
            catch(Exception e)
            {
                Debug.WriteLine($"Error saving stocks to file: {e.Message}");
            }
        }
    }
}
