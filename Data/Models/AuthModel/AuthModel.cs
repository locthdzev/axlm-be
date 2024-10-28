namespace Data.Models.AuthModel
{
    public class UserLoginReqModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginResModel
    {
        public UserResModel User { get; set; }

        public string Token { get; set; }
    }

    public class UserResModel
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime Dob { get; set; }

        public string Address { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public string Role { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Status { get; set; } = null!;
    }

    public class UserResetPasswordReqModel
    {
        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }

    public class ChangePasswordReqModel
    {
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}