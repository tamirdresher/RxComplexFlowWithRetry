using System.Collections.Concurrent;
using System.Reactive;
using System.Reactive.Subjects;

namespace RxComplexFlowWithRetry
{
    using System;
    using System.Reactive.Linq;
    using System.Threading;

    public class Uploader
    {
        private static Random random = new Random((int)DateTime.Now.Ticks);
        private Subject<Item> _failedSubject;
        private ConcurrentQueue<Item> _failingQueue = new ConcurrentQueue<Item>();
        Subject<Unit> _retries = new Subject<Unit>();


        private const int UploadFailureProbability = 80;

        public Uploader(IObservable<Item> items)
        {
            _failedSubject = new Subject<Item>();

            UploadedItems = _failingQueue.ToObservable().Concat(items).Select(item => Upload(item))
               .TakeUntil(Failed)
               .SkipUntil(_retries.StartWith(Unit.Default))
               .Repeat();
        }

        private static bool ShouldThrow
        {
            get
            {                
                return random.Next(0, 100) > UploadFailureProbability;
            }
        }

        public IObservable<UploadResults> UploadedItems { get; private set; }

        public IObservable<Item> Failed
        {
            get { return _failedSubject.AsObservable(); }
        }

        private UploadResults Upload(Item item)
        {
            Thread.Sleep(7000);

            if (ShouldThrow)
            {
                _failingQueue.Enqueue(item);
                _failedSubject.OnNext(item);
            }

            return new UploadResults(item);
        }


        public void RetryFailedItem()
        {
            _retries.OnNext(Unit.Default);
        }
    }
}