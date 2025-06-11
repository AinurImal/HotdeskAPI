namespace Hotdesk.Components.Models
{
    public class BookFinder
    {
        public int BookingId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty; 
        public string PhoneNumber { get; set; } = string.Empty;
        public string DeskName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public string DurationType { get; set; } = string.Empty;
    }

}

