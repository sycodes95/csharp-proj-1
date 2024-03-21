using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Portfolio;
using api.Models;

namespace api.Interfaces
{
    public interface IPortfolioRepository
    {
        Task<List<Stock>> GetUserPortfolio(AppUser user);

        Task<Portfolio> AddPortfolio(string AppUserId, int StockId);

        Task<Portfolio?> DeletePortfolio(string AppUserId, int StockId);

    }
}