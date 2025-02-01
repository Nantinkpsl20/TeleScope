namespace teleScope.Models
{
    public class UserCustomerModel
    {
        public teleScope.Models.User user { get; set; }

        public teleScope.Models.Customer? customer { get; set; }

        public teleScope.Models.PhoneNumber phoneNumber { get; set; }

    }
}
