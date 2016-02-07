namespace RxComplexFlowWithRetry
{
    public class Item
    {
        private static int itemNumber;

        public Item(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}