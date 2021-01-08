using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using BarCodeScanner.db;
using BarCodeScanner.db.Model;
using BarCodeScanner.Helps;
using BarCodeScanner.View;
using LiteDB;
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

            var canExecute = this.WhenAny(
                model => model.SelectedIteam,
                worker => worker != null);


            RemoveCommand = ReactiveCommand.Create(DeleteWorker, canExecute);
            AddNewCommand = ReactiveCommand.CreateFromTask(AddNewWorker);
            EditCommand = ReactiveCommand.CreateFromTask(EditWorker);
        }

        [Reactive] public Worker SelectedIteam { get; set; }
        [Reactive] public IEnumerable<Worker> WorkersList { get; set; }
        
        [Reactive] public IReactiveCommand AddNewCommand { get; set; }
        [Reactive] public IReactiveCommand EditCommand { get; set; }
        [Reactive] public IReactiveCommand RemoveCommand { get; set; }

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
