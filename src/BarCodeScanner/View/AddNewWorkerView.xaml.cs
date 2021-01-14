using BarCodeScanner.ViewModel;

using DataBase.Model;

namespace BarCodeScanner.View
{
    public partial class AddNewWorkerView
    {
        public AddNewWorkerView()
        {
            InitializeComponent();
            DataContext = new AddNewWorkerViewModel();
        }

        public AddNewWorkerView(Worker selectedIteam)
        {
            InitializeComponent();
            DataContext = new AddNewWorkerViewModel(selectedIteam);
        }
    }
}