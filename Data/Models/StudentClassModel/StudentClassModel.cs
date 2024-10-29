using Data.Models.AssignmentModel;

namespace Data.Models.StudentClassModel
{
    public class AddStudentToClassReqModel
    {
        public List<Guid> StudentId { get; set; }
    }

    public class DeleteClassReqModel
    {
        public List<Guid> ClassId { get; set; }
    }

    public class AddTrainerModel
    {
        public Guid TrainerId { get; set; }
    }

    public class IdAndNameModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class StudentResultRecordResModel
    {
        public string StudentName { get; set; } = null!;
        public List<AssignmentTitleAndScoreResModel> AssignmentModelList { get; set; }
        public double Average { get; set; }
        public int Rank { get; set; }
    }

    public class StudentResultRecordListByClassAndModule
    {
        public string ClassName { get; set; } = null!;
        public List<StudentResultRecordResModel> StudentModelList { get; set; }
    }
}