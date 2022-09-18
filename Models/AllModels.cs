namespace PracticeCoreMVC.Models
{
    public class AllModels
    {
        public LoginModel? loginModel { get; set; }
        public RegisterModel? registerModel { get; set; }
        public RoleModel? roleModel { get; set; }
        public UserRoleMappingModel? userRoleMappingModel { get; set; }

        public List<LoginModel?>? loginModelList { get; set; }
        public List<RegisterModel?>? registerModelList { get; set; }
        public List<RoleModel?>? roleModelList { get; set; }
        public List<UserRoleMappingModel?>? userRoleMappingModelList { get; set; }
    }
}
