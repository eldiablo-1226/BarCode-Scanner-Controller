using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using BarCodeScanner.db;
using BarCodeScanner.db.Model;
using BarCodeScanner.View;
using MaterialDesignThemes.Wpf;
using NLog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace BarCodeScanner.ViewModel
{
    public class WorkersViewModel : ReactiveObject
    {
        #region IContext
        private IDbContext _db;
        private ILogger _logger;
        #endregion
        public WorkersViewModel(IDbContext dbContext, ILogger logger)
        {
            _db = dbContext;
            _logger = logger;

            LoadWorkers();

            _isSelect = this.WhenAnyValue(m => m.SelectedIteam)
                .Select(b => b != null)
                .ToProperty(this, m => m.IsSelected);


            AddNewCommand = ReactiveCommand.CreateFromTask(AddNewWorker);
            EditCommand = ReactiveCommand.CreateFromTask(EditWorker);
            RemoveCommand = ReactiveCommand.Create(DeleteWorker);
            ClipboardCommand = ReactiveCommand.Create(() => Clipboard.SetText(SelectedIteam.BarCode));

        }
        readonly ObservableAsPropertyHelper<bool> _isSelect;
        public bool IsSelected => _isSelect.Value;
        [Reactive] public Worker SelectedIteam { get; set; }
        [Reactive] public IEnumerable<Worker> WorkersList { get; set; }
        
        [Reactive] public IReactiveCommand AddNewCommand { get; set; }
        [Reactive] public IReactiveCommand EditCommand { get; set; }
        [Reactive] public IReactiveCommand RemoveCommand { get; set; }
        [Reactive] public IReactiveCommand ClipboardCommand { get; set; }

        private async Task AddNewWorker()
        {
            var dialog = await DialogHost.Show(new AddNewWorkerView());
            if (dialog is Worker worker)
            {
                _db.Workers.Insert(worker);
                LoadWorkers();
            }
        }
        private async Task EditWorker()
        {
            var dialog = await DialogHost.Show(new AddNewWorkerView(SelectedIteam));
            if (dialog is Worker worker)
            {
                _db.Workers.Update(worker);
                LoadWorkers();
            }
        }
        private void DeleteWorker()
        {
            try
            {
                _db.Workers.Delete(SelectedIteam.Id);
                LoadWorkers();
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }


        private void LoadWorkers() =>
            WorkersList = _db.Workers.FindAll();
    }
}
