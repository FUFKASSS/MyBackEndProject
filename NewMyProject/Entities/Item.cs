namespace NewMyProject.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public WeightProduct? Weight { get; set; }
        public TypeProduct? Type { get; set; }
        public Product? Product { get; set; }
    }
}
