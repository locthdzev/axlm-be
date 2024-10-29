namespace Data.Models.OtpModel
{
    public class UserVerifyOTPResModel
    {
        public string Email { get; set; } = null!;
        public string OTPCode { get; set; } = null!;
    }
}