namespace Flowey.CORE.DTO.User
{
    public class UserUpdateDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
