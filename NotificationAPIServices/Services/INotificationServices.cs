using Data.Models.NotificationModel;
using Data.Models.ResultModel;

namespace NotificationAPIServices.Services
{
    public interface INotificationServices
    {
        Task<ResultModel> CreateNotification(string token, NotificationCreateReqModel notificationCreateReqModel);
        Task<ResultModel> GetAllNotifications(string token, int page);
        Task<ResultModel> UpdateNotification(Guid notificationId, string token, NotifcationUpdateModel notifcationUpdateModel);
        Task<ResultModel> GetNotificationDetails(Guid notificationId, string token);
        Task<ResultModel> DeleteNotification(Guid notificationId, string token);
    }
}