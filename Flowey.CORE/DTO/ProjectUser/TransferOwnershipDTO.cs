namespace Flowey.CORE.DTO.ProjectUser
{
    public class TransferOwnershipDTO
    {
        public Guid ProjectId { get; set; }
        public Guid NewOwnerId { get; set; }
    }
}
