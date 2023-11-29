using System.ComponentModel.DataAnnotations;

namespace WebApiTutorial.Models {


    public class User {
        [Key]
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public List<PollOption> VotedFor { get; set; } = new();
    }
}