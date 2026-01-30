using System.ComponentModel;
using System.Windows.Input;

namespace TPSMobileApp.ViewModels
{
    public class QualityViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand PlusCommand { private set; get; }
        public ICommand MinusCommand { private set; get; }
        private int number;

        public int Number
        {
            set
            {
                if (number != value)
                {
                    number = value;
                    OnPropertyChanged("Number");
                }
            }
            get
            {
                return number;
            }
        }

        public QualityViewModel()
        {
            PlusCommand = new Command(
            execute: () =>
            {
                number++;
                OnPropertyChanged("Number");
                RefreshCanExecutes();
            });

            MinusCommand = new Command(
            execute: () =>
            {
                number--;
                OnPropertyChanged("Number");
                RefreshCanExecutes();
            });
        }

        private void RefreshCanExecutes()
        {
            (PlusCommand as Command).ChangeCanExecute();
            (MinusCommand as Command).ChangeCanExecute();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}