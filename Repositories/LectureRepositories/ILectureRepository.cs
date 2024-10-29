using Data.Entities;
using Repositories.GenericRepositories;

namespace Repositories.LectureRepositories
{
    public interface ILectureRepository : IRepository<Lecture>
    {
        Task<Lecture?> GetLectureByName(string lectureName);
		Task<Lecture?> GetLectureById(Guid lectureId);
        Task<Lecture> UpdateLecture(Lecture lecture);
		Task<List<Lecture>> GetListLectures();
		Task<List<Lecture>> GetListLecturesById(List<Guid> lectureId);
		Task<List<Lecture>> GetAllLectures();
		Task<List<Lecture>> GetLectureByModuleId(Guid moduleId);
    }
}