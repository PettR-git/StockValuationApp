using StockValuationApp.Entities.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockLib.Abstraction
{
    public interface IStockRepository
    {
        Task<List<Stock>> LoadAllAsync();
        Task SaveAllAsync(IEnumerable<Stock> stocks);
    }
}
