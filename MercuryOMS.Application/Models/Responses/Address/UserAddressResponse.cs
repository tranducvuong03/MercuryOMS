namespace MercuryOMS.Application.Models.Responses
{
    public class UserAddressResponse
    {
        public Guid Id { get; set; }

        public string Label { get; set; } = null!;

        public string ReceiverName { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Street { get; set; } = null!;

        public string District { get; set; } = null!;

        public string City { get; set; } = null!;

        public string Province { get; set; } = null!;

        public bool IsDefault { get; set; }
    }
}
