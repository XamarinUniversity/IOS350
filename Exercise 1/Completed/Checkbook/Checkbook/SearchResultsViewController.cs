using System;
using UIKit;
using System.Collections.Generic;
using System.Linq;
using Foundation;

namespace Checkbook
{
	public class SearchResultsViewController : UITableViewController
	{
		private readonly UIViewController parentViewController;
		private readonly List<Check> visibleChecks = new List<Check>();
		private readonly IList<Check> checks;

		public SearchResultsViewController(UIViewController controller, IList<Check> checks)
		{
			this.parentViewController = controller;
			this.checks = checks;
		}

		public void Search(string searchText) 
		{
			visibleChecks.Clear();
			foreach (var item in checks.Where(c => c.Payee.Contains(searchText)))
				visibleChecks.Add(item);

			this.TableView.ReloadData();
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return visibleChecks.Count;
		}

		public override bool ShouldHighlightRow (UITableView tableView, NSIndexPath rowIndexPath)
		{
			return true;
		}

		public override string TitleForHeader(UITableView tableView, nint section)
		{
			return "Search Results";
		}

		public override string TitleForFooter(UITableView tableView, nint section)
		{
			return string.Format("Found {0} matches", visibleChecks.Count);
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell searchCell = tableView.DequeueReusableCell("SearchCell"); 
			if (searchCell == null )
				searchCell = new UITableViewCell (UITableViewCellStyle.Default, "SearchCell");

			searchCell.TextLabel.Text = visibleChecks[indexPath.Row].ToString();
			return searchCell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var check = visibleChecks[indexPath.Row];
			parentViewController.ShowDetailViewController(new CheckDetailsViewController() { Check = check }, parentViewController);
		}
	}
}

