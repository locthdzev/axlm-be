using Data.Models.ClassModel;

namespace Data.Models.UserModel
{
    public class StudentCreateReqModel
    {
        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Password { get; set; } = null!;

        public DateTime Dob { get; set; }

        public string Address { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public string Phone { get; set; } = null!;
    }

    public class StudentUpdateResModel
    {
        public DateTime Dob { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; } = null!;
    }

    public class StudentUpdateStatusResModel
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
    }

    public class UserLoginReqModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserCreateReqModel
    {
        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Password { get; set; } = null!;

        public DateTime Dob { get; set; }

        public string Address { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public string Role { get; set; } = null!;

        public string Phone { get; set; } = null!;
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

    public class UpdateAccountsStatusModel
    {
        public List<Guid>? UserId { get; set; }
        public string status { get; set; } = null!;
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

    public class UserLoginResModel
    {
        public UserResModel User { get; set; }

        public string Token { get; set; }
    }

    public class UserVerifyOTPResModel
    {
        public string Email { get; set; } = null!;
        public string OTPCode { get; set; } = null!;
    }

    public class StudentAccountInfoModel
    {
        public UserResModel userResModel { get; set; } = null!;

        public ClassInformationOfStudent ClassInformationOfStudent { get; set; } = null!;

    }

    public class OtherAccountInfoModel
    {
        public UserResModel userResModel { get; set; } = null!;

        public List<ClassInformationOfOthers> classInformationOfOthers { get; set; } = null!;
    }

    public class EmailSendingModel
    {
        public string email { get; set; } = null!;
        public string html { get; set; } = null!;
    }

    public class NameAndEmailModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}