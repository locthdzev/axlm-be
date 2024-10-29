using Data.Models.FilterModel;
using Data.Models.ResultModel;
using Data.Models.UserModel;

namespace UserAPIServices.Services
{
    public interface IUserServices
    {
        Task<ResultModel> CreateAccount(UserCreateReqModel LoginForm);
        Task<ResultModel> GetAllUser(int page, FilterModel reqModel);
        Task<ResultModel> GetUserProfile(Guid userId);
        // Task<ResultModel> ViewAccountsInfo(Guid userId);
        Task<ResultModel> GetTrainerList(int page);
        // Task<ResultModel> GetAttendanceById(Guid attendanceId);
        Task<ResultModel> UpdateAccountsStatus(UpdateAccountsStatusModel reqModel);
    }
}