namespace Flowey.CORE.DTO.User
{
    public class UserSubscriptionGetDTO
    {
        public Guid UserId { get; set; }
        public string PlanName { get; set; } 
        public bool IsPaid { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
