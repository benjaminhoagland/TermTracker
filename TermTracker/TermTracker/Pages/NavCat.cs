using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TermTracker.Pages
{
    public static class NavCat
    {
        public static StackLayout CreateStackLayout()
        {
            return new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(5,0,5,0)
            };
        }
    };
}
