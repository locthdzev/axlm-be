using Data.Models.LectureModel;
using Data.Models.ResultModel;

namespace LectureAPIServices.Services
{
    public interface ILectureServices
    {
        Task<ResultModel> GetListLecture(int page);
        Task<ResultModel> GetLectureDetail(Guid id);
        Task<ResultModel> CreateLecture(LectureReqModel createform);
        Task<ResultModel> UpdateLecture(LectureUpdateModel UpdateLecture, Guid lectureId, string token);
        Task<ResultModel> DeleteLecture(List<Guid> ListId);
    }
}