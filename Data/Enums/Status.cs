namespace Data.Enums
{
    public class Status
    {
        public class UserStatus
        {
            public static readonly string ACTIVE = "Active";
            public static readonly string RESETPASSWORD = "ResetPassword";
            public static readonly string INACTIVE = "Inactive";
        }

        public class ClassStatus
        {
            public static readonly string ACTIVE = "Active";
            public static readonly string INACTIVE = "Inactive";
        }

        public class GeneralStatus
        {
            public static readonly string ACTIVE = "Active";
            public static readonly string INACTIVE = "Inactive";
        }

        public class AttendanceStatus
        {
            public static readonly string PROCESSING = "Processing";
            public static readonly string LATE = "Late";
            public static readonly string DONE = "Done";
        }

        public class AttendanceStudentStatus
        {
            public static readonly string PRESENT = "Present";
            public static readonly string LATE = "Late";
        }

        public class CertificateStatus
        {
            public static readonly string ACTIVE = "Active";
            public static readonly string INACTIVE = "Inactive";
        }
    }
}