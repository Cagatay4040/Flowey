using Flowey.DOMAIN.Model.Abstract;

namespace Flowey.DOMAIN.Model.Concrete
{
    public class UserSubscription : BaseEntity, IEntity
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public string PlanName { get; set; }
        public bool IsPaid { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal Price { get; set; }
    }
}
