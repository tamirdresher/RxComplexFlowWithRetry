namespace RxComplexFlowWithRetry
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reactive.Linq;
    using System.Windows.Input;
    using View.ViewModel;

    public class MainViewModel
    {
        private readonly Uploader uploader;

        public MainViewModel()
        {
            // Takes 15 Items to upload
            var itemsObservable = Observable
                .Interval(TimeSpan.FromSeconds(5))
                .Select(id => new Item(string.Format("Item {0}", id + 1)))
                .Take(15);

            // We observe the incoming items to show them into the view
            itemsObservable
                .ObserveOnDispatcher()
                .Subscribe(item => PendingItems.Add(item));

            // We also observe the items that uploaded successfully (the uploader holds them)
            uploader = new Uploader(itemsObservable);
            uploader.UploadedItems
               .ObserveOnDispatcher()
               .Subscribe(uploadResults => UploadedItems.Add(uploadResults));
            
            uploader.Failed.Subscribe(failedItem => FailedItem = failedItem);

            //The command that will be invoked when the user "fixes" an item and wants to retry it. It will be available only when there is a item that failed to upload.
            RetryCommand = new RelayCommand(_ => Retry(), _ => FailedItem != null);
        }

        // When and item fails, it will be put inside this property
        public Item FailedItem { get; set; }

        public ObservableCollection<Item> PendingItems { get; set; } = new ObservableCollection<Item>();
        public ObservableCollection<UploadResults> UploadedItems { get; set; } = new ObservableCollection<UploadResults>();

        public ICommand RetryCommand { get; }

        private void Retry()
        {
            uploader.RetryFailedItem();
        }
    }
}