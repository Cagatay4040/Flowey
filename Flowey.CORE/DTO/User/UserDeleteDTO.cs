namespace Flowey.CORE.DTO.User
{
    public class UserDeleteDTO
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
        public Guid? ModifiedBy { get; set; }
    }
}
