namespace MLaw.Idp.Cosign.Models
{
    public class LoggedinUserModel
    {
        public static LoggedinUserModel Create(string userName, string name, string email)

        {

            return new LoggedinUserModel()
            {
                UserName = userName,
                Name = name,
                Email = email
            };
        }

        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

    }
}
