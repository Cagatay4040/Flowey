namespace Flowey.CORE.DTO.User
{
    public class UserSelectDTO
    {
        public Guid Id { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
