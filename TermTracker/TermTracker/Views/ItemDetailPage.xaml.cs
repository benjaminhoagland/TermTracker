using System.ComponentModel;
using TermTracker.ViewModels;
using Xamarin.Forms;

namespace TermTracker.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}