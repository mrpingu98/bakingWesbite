using System;
using Microsoft.EntityFrameworkCore;

namespace baking_website.Models
{
	public class PieRepository : IPieRepository
	{
		private readonly BethanysPieShopDbContext _bethanysPieShopDbContext;

        public PieRepository(BethanysPieShopDbContext bethanysPieShopDBContext)
		{
			_bethanysPieShopDbContext = bethanysPieShopDBContext;
		}

        public IEnumerable<Pie> AllPies {
            get
            {
                return _bethanysPieShopDbContext.Pies.Include(c => c.Category);
            }
        }

        public IEnumerable<Pie> PiesOfTheWeek
        {
            get
            {
                return _bethanysPieShopDbContext.Pies.Include(c => c.Category).Where(p => p.IsPieOfTheWeek);
            }
        }

        public Pie? GetPieById(int pieId)
        {
            return _bethanysPieShopDbContext.Pies.FirstOrDefault(p => p.PieId == pieId);
        }
    }
}

