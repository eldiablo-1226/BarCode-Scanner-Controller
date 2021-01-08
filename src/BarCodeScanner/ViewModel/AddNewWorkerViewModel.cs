﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarCodeScanner.db.Model;
using LiteDB;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace BarCodeScanner.ViewModel
{
    public class AddNewWorkerViewModel : ReactiveObject
    {
        private bool _isEditMode = false;
        public string ButtonTextContext { get => _isEditMode ? "Изменить" : "Добаить"; }

        private ObjectId _id;
        [Reactive] public string FullName { get; set; }
        [Reactive] public string BarCode { get; set; }

        public IReactiveCommand ContextActionCommand { get; init; }
        public IReactiveCommand GenerateBarcodeCommand { get; init; }

        public AddNewWorkerViewModel()
        {
            ContextActionCommand = ReactiveCommand.Create(EditAndSaveValue,
                this.WhenAnyValue(
                    m => m.BarCode, m => m.FullName,
                    (b, n) => 
                        !string.IsNullOrWhiteSpace(b) &&
                        !string.IsNullOrWhiteSpace(n)));
            GenerateBarcodeCommand = ReactiveCommand.Create(() => );
        }
        public AddNewWorkerViewModel(Worker selectediteam) : this()
        {
            _isEditMode = true;

            _id = selectediteam.Id;
            FullName = selectediteam.FullName;
            BarCode = selectediteam.BarCode;
        }

        private void EditAndSaveValue()
        {
            var newWorker = new Worker
            {
                Id = _isEditMode ? _id : null,
                FullName = FullName,
                BarCode = BarCode
            };
            DialogHost.Close("DialogHost", newWorker);
        }
    }
}
