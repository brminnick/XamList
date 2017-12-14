using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using XamList;
using XamList.iOS;

[assembly: ExportRenderer(typeof(ContactsListTextCell), typeof(ContactsListTextCellCustomRenderer))]
namespace XamList.iOS
{
	public class ContactsListTextCellCustomRenderer : TextCellRenderer
	{
		public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var cell = base.GetCell(item, reusableCell, tv);

			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;

			return cell;
		}
	}
}
