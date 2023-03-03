namespace NewMyProject.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public OrderStatus Status { get;set; }
        public List<Item>? Items { get; set; }
    }

    public enum OrderStatus
    {
        CART,
        COOKING,
        DELIVERING,
        COMPLETED
    } 
}
