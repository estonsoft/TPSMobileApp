using System;
using System.Collections.Generic;
using System.Text;

namespace TPSMobileApp.ViewModels
{
    public class PdfViewModel : BaseViewModel
    {
        private Stream _pdfDocumentStream;
        private bool _isBusy;

        public Stream PdfDocumentStream
        {
            get => _pdfDocumentStream;
            set { _pdfDocumentStream = value; OnPropertyChanged(); }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public async Task LoadPdfFromBase64()
        {
            IsBusy = true; // Show loading

            try
            {
                string fileName = "OrderID" + App.g_OrderNo + ".pdf"; // Added .pdf extension for the viewer
                string filePath = Path.Combine(FileSystem.Current.CacheDirectory, fileName);
                FileStream contents = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                PdfDocumentStream = contents;
            }
            finally
            {
                IsBusy = false; // Hide loading
            }
        }
    }
}
