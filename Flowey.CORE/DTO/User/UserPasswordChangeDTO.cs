namespace Flowey.CORE.DTO.User
{
    public class UserPasswordChangeDTO
    {
        public Guid UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirm { get; set; }
    }
}
