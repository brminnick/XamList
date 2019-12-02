using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XamList;
using XamList.iOS;

[assembly: ExportRenderer(typeof(ContactDetailPage), typeof(ContactDetailsPageCustomRenderer))]
namespace XamList.iOS
{
    public class ContactDetailsPageCustomRenderer : PageRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var thisElement = (ContactDetailPage)Element;

            var leftNavList = new List<UIBarButtonItem>();
            var rightNavList = new List<UIBarButtonItem>();

            var navigationItem = NavigationController.TopViewController.NavigationItem;

            for (var i = 0; i < thisElement.ToolbarItems.Count; i++)
            {
                var reorder = (thisElement.ToolbarItems.Count - 1);
                var itemPriority = thisElement.ToolbarItems[reorder - i].Priority;

                if (itemPriority is 1)
                {
                    UIBarButtonItem LeftNavItems = navigationItem.RightBarButtonItems[i];
                    leftNavList.Add(LeftNavItems);
                }
                else if (itemPriority is 0)
                {
                    UIBarButtonItem RightNavItems = navigationItem.RightBarButtonItems[i];
                    rightNavList.Add(RightNavItems);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            navigationItem.SetLeftBarButtonItems(leftNavList.ToArray(), false);
            navigationItem.SetRightBarButtonItems(rightNavList.ToArray(), false);
        }
    }
}