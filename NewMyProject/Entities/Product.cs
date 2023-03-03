namespace NewMyProject.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int Category { get; set; }
        public int Rating { get; set; }
        public decimal Price { get; set; }
        public List<WeightProduct>? Weights { get; set; }
        public List<TypeProduct>? Types { get; set; }
    }
}

