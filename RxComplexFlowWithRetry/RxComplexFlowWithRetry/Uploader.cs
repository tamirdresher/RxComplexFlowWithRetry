namespace RxComplexFlowWithRetry
{
    using System;
    using System.Reactive.Linq;
    using System.Threading;

    public class Uploader
    {
        private const int UploadFailureProbability = 80;

        public Uploader(IObservable<Item> items)
        {
            UploadedItems = items.Select(item => Upload(item));
            Failed = Observable.Empty<Item>();
        }

        private static bool ShouldThrow
        {
            get
            {
                var random = new Random((int)DateTime.Now.Ticks);
                return random.Next(0, 100) > UploadFailureProbability;
            }
        }

        public IObservable<UploadResults> UploadedItems { get; private set; }

        public IObservable<Item> Failed
        {
            get; private set;
        }

        private UploadResults Upload(Item item)
        {
            Thread.Sleep(7000);

            if (ShouldThrow)
            {
                throw new InvalidOperationException("Upload failed");
            }

            return new UploadResults(item);
        }


        public void RetryFailedItem()
        {
            throw new NotImplementedException();
        }
    }
}