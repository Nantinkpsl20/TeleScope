namespace teleScope.Models
{
    public class CustomerBill
    {
        public teleScope.Models.User user { get; set; }

        public teleScope.Models.PhoneNumber? phoneNumber { get; set; }

        public teleScope.Models.Programme? programme { get; set; }

        public teleScope.Models.Bill bill { get; set; }
        
    }
}
