namespace teleScope.Models
{
    public class BillsUserModel
    {
        public teleScope.Models.User user { get; set; }

        public teleScope.Models.Customer? customer { get; set; }

        public teleScope.Models.PhoneNumber phoneNumber { get; set; }

        public teleScope.Models.Bill bill { get; set; }

    }
}
