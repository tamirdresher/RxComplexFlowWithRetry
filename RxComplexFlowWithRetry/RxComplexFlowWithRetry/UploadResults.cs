namespace RxComplexFlowWithRetry
{
    public class UploadResults
    {
        public Item Item { get; set; }

        public UploadResults(Item item)
        {
            Item = item;
        }
    }
}