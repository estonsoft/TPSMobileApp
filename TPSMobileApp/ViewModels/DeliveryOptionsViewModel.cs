using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
namespace TPSMobileApp.ViewModels
{
    public class DeliveryOptionsViewModel : BaseViewModel
    {
        public DeliveryOptionsViewModel()
        {
            /*
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Welcome to Xamarin.Forms!" }
                }
            };
            */

            OpenRegister = new Command(async () => await Shell.Current.GoToAsync("DeliveryOptionsPage"));
        }

        public ICommand OpenRegister { get; }
    }
}