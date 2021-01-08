using BarCodeScanner.db.Model;
using BarCodeScanner.ViewModel;
using CommonServiceLocator;

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
