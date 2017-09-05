using System;
using UIKit;

namespace Checkbook
{
	public class SearchResultsUpdator : UISearchResultsUpdating
	{
		public event Action<string> UpdateSearchResults = delegate {};

		public override void UpdateSearchResultsForSearchController(UISearchController searchController)
		{
			this.UpdateSearchResults (searchController.SearchBar.Text);
		}
	}
}
