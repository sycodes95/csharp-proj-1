using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Portfolio;
using api.Models;

namespace api.Mappers
{
    public static class PortfolioMappers
    {
        public static Portfolio ToPortfolioFromCreateDTO(this CreatePortfolioDto portfolioModel)
        {
            return new Portfolio
            {
                AppUserId = portfolioModel.AppUserId,
                StockId = portfolioModel.StockId,
            };
        }
    
    }
}