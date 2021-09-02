using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TermTracker.DB
{
    public class Helper
    {
        public static StackLayout GenerateDataButtons()
        {
            return new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Start,
                Padding = new Thickness(5, 10, 5, 10),
                HeightRequest = 25,

                Children =
                {
                    new Label { Text = "Debugging and Test Data Controls", VerticalOptions = LayoutOptions.Center } 
                }
            };
        }
    }
}
