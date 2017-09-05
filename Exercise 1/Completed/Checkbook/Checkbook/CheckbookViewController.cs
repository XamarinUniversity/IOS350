using System;
using Foundation;
using UIKit;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CoreGraphics;

namespace Checkbook
{
    public partial class CheckbookViewController : UITableViewController
    {
		//Need to retain cell height for later layout
		nfloat _heightForCell;

		public ObservableCollection<Check> Checks { get; set; }

		// Step 3: add search controller
		public UISearchController SearchController { get; set;}

        public CheckbookViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			// Populate the checks.
			Checks = new ObservableCollection<Check>();
			Checks.CollectionChanged += OnCollectionChanged;
			foreach (var item in CheckManager.Load())
				Checks.Add(item);

			this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, OnAddNewItem);

			NSNotificationCenter.DefaultCenter.AddObserver(
				UIApplication.ContentSizeCategoryChangedNotification,
				n => TableView.ReloadData());

			var searchResultsController = new SearchResultsViewController(this, Checks);

			// Create search updater and wire it up
			var searchUpdater = new SearchResultsUpdator();
			searchUpdater.UpdateSearchResults += searchResultsController.Search;

			// Create a new search controller
			SearchController = new UISearchController(searchResultsController);
			SearchController.SearchResultsUpdater = searchUpdater;

			// Display the search controller
			SearchController.SearchBar.Frame = new CGRect(SearchController.SearchBar.Frame.X, SearchController.SearchBar.Frame.Y, SearchController.SearchBar.Frame.Width, 44f);
			TableView.TableHeaderView = SearchController.SearchBar;
			DefinesPresentationContext = true;
        }

		private void OnAddNewItem(object sender, EventArgs e)
		{
			Check check = new Check { Number = Checks.Max(c => c.Number) + 1, Amount = 0, Date = DateTime.Now };
			Checks.Add(check);
			TableView.ReloadData();
			using (NSIndexPath indexPath = NSIndexPath.FromRowSection(Checks.Count - 1, 0))
				TableView.SelectRow(indexPath, true, UITableViewScrollPosition.None);

			PerformSegue("showDetails", this);
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return Checks.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var check = Checks[indexPath.Row];

		//			UITableViewCell cell = tableView.DequeueReusableCell("BasicCheckCell");
		//			if (cell == null) {
		//				cell = new UITableViewCell(UITableViewCellStyle.Subtitle, "BasicCheckCell");
		//				cell.DetailTextLabel.TextColor = UIColor.LightGray;
		//			}
		//
		//			cell.TextLabel.Text = check.Payee;
		//			cell.DetailTextLabel.Text = check.Amount.ToString("C");

			// TODO: Step 1: use a custom cell.
			CheckViewCell cell = (CheckViewCell) tableView.DequeueReusableCell(new NSString("CheckCell"), indexPath);
			cell.DateLabel.Text = check.Date.ToString("D");
			cell.PayeeText.Text = check.Payee;
			cell.Amount.Text = check.Amount.ToString("C");
			cell.Amount.TextColor = check.Amount > 0 ? UIColor.Green : UIColor.Red;
			cell.BackgroundColor = check.Cleared ? UIColor.FromRGB(0xf0, 0xf0, 0xf0) : UIColor.White;

			cell.DateLabel.Font = UIFont.PreferredSubheadline;
			cell.PayeeText.Font = cell.Amount.Font = UIFont.PreferredHeadline;

			_heightForCell = cell.Frame.Height;

			return cell;
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			if (_heightForCell > 0 && base.GetHeightForRow (tableView, indexPath) > _heightForCell){
				return _heightForCell;
			}
			else { 
				return base.GetHeightForRow (tableView, indexPath);
			}
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			if (tableView.CellAt(indexPath) as CheckViewCell == null)
				PerformSegue("showDetails", this);
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "showDetails") {
				var indexPath = TableView.IndexPathForSelectedRow;
				if (indexPath != null) {

					//If navigation bar is hidden we'll not be able to navigate back.
					if (NavigationController.NavigationBarHidden)
						NavigationController.NavigationBarHidden = false;

					var check = Checks[indexPath.Row];
					var dvc = ((UINavigationController)segue.DestinationViewController).TopViewController as CheckDetailsViewController;
					dvc.Check = check;
					if (SplitViewController != null) {
						dvc.NavigationItem.LeftBarButtonItem = SplitViewController.DisplayModeButtonItem;
						dvc.NavigationItem.LeftItemsSupplementBackButton = true;
					}
				}
			}
		}

		public override UITableViewRowAction[] EditActionsForRow(UITableView tableView, NSIndexPath indexPath)
		{
			var deleteAction = UITableViewRowAction.Create(UITableViewRowActionStyle.Destructive, "Delete",
				(ra, ip) => {
					Checks.RemoveAt(ip.Row);
					TableView.DeleteRows(new[] { ip }, UITableViewRowAnimation.Automatic);
				});

			var clearAction = UITableViewRowAction.Create(UITableViewRowActionStyle.Default, "Clear", 
				(ra, ip) => Checks[ip.Row].Cleared = true);
			clearAction.BackgroundColor = UIColor.Blue;

			var moreAction = UITableViewRowAction.Create(UITableViewRowActionStyle.Normal, "More", 
				(ra, ip) => {
					UIAlertController alert = UIAlertController.Create(
						"More...", "Select a choice", UIAlertControllerStyle.ActionSheet);

					alert.AddAction(UIAlertAction.Create("Delete",
						UIAlertActionStyle.Destructive, action => {
							Checks.RemoveAt(indexPath.Row);
							TableView.DeleteRows(new[] { indexPath }, UITableViewRowAnimation.Automatic);
						}));

					alert.AddAction(UIAlertAction.Create("Clear",
						UIAlertActionStyle.Default, action => {
							Checks[indexPath.Row].Cleared = !Checks[indexPath.Row].Cleared;
						}));

					alert.AddAction(UIAlertAction.Create("Cancel",
						UIAlertActionStyle.Cancel, action => { }));
					this.PresentViewController(alert, true, null); 
				});

			return new[] { deleteAction, clearAction, moreAction };
		}

		public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			// Required to enable edit actions.
			base.CommitEditingStyle(tableView, editingStyle, indexPath);
		}

		#region PropertyChange notifications
		void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null) {
				foreach (var item in e.NewItems) {
					INotifyPropertyChanged inpc = item as INotifyPropertyChanged;
					if (inpc != null)
						inpc.PropertyChanged += OnCheckPropertyChanged;
				}
			}

			if (e.OldItems != null) {
				foreach (var item in e.OldItems) {
					INotifyPropertyChanged inpc = item as INotifyPropertyChanged;
					if (inpc != null)
						inpc.PropertyChanged -= OnCheckPropertyChanged;
				}
			}
		}

		void OnCheckPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Check check = sender as Check;

			if (e.PropertyName == "Date") {
				var sorted = Checks.OrderBy(x => x.Date).ToList();
				for (int i = 0; i < sorted.Count(); i++)
					Checks.Move(Checks.IndexOf(sorted[i]), i);
				TableView.ReloadData();
			}
			else {
				int index = Checks.IndexOf(check);
				using (NSIndexPath indexPath = NSIndexPath.FromRowSection(index,0))	{
					TableView.ReloadRows(new[] { indexPath }, UITableViewRowAnimation.None);
				}
			}
		}
		#endregion
    }
}

