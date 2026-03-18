namespace Flowey.CORE.DTO.Notification
{
    public class UserNotificationGetDTO
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string? ActionUrl { get; set; }
        public bool IsRead { get; set; } = false;
    }
}
