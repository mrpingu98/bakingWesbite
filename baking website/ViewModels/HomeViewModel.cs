using System;
using baking_website.Models;

namespace baking_website.ViewModels
{
	public class HomeViewModel
	{
		public IEnumerable<Pie> PiesOfTheWeek { get; }

		public HomeViewModel(IEnumerable<Pie> piesOfTheWeek)
		{
			PiesOfTheWeek = piesOfTheWeek;
		}
	}
}

