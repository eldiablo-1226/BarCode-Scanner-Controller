using CommonServiceLocator;

namespace BarCodeScanner.View
{
    public partial class AddNewWorkerView
    {
        public AddNewWorkerView()
        {
            InitializeComponent();
            DataContext = ServiceLocator.Current.GetInstance<AddNewWorkerView>();
        }
    }
}
