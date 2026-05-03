using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingEngine.Application.Interfaces.Symbols
{
    public interface ISymbolReadRepository
    {
        Task<List<string>> GetAllSymbolsAsync();
    }
}
