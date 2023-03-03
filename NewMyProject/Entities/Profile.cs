using System.ComponentModel.DataAnnotations;

namespace NewMyProject.Entities
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public long? PhoneNumber { get; set; }
        public int UserInfoKey { get; set; }
        public User? User { get; set; }
        public List<Order>? Orders { get; set; }
    }
}
