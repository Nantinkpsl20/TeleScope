namespace teleScope.Models
{
    public class CustomerCallsModel
    {
        public teleScope.Models.UserCustomerModel user { get; set; }

        public teleScope.Models.Call? call { get; set; }

        public teleScope.Models.Programme? program { get; set; }

    }
}
