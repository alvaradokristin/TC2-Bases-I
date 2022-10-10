namespace TC2_Bases.Models
{
    public class InvoiceCreationBasic
    {
        public string? clientId { get; set; }
        public string? clientName { get; set; }
        public string? clientAddress { get; set; }
        public string? carLicensePlate { get; set; }
        public string? carBrand { get; set; }
        public string? carModel { get; set; }
        public string? carYear { get; set; }
        public string? carColor { get; set; }
        public string? laborSubtotal { get; set; }
        public string? repsSubtotal { get; set; }
        public string? subtotal { get; set; }
        public string? total { get; set; }
    }
}
