namespace PracticeCoreMVC.Models
{
    public class UserRoleMappingModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public string? salt { get; set; }
    }
}
