using Data.Models.AssignmentModel;
using Data.Models.ResultModel;
using Data.Models.SubmissionModel;

namespace AssignmentAPIServices.Services
{
    public interface IAssignmentServices
    {
        Task<ResultModel> GetAsmList(Guid Id, string token);
        Task<ResultModel> CreateAssignment(AssignmentCreateReqModel createForm, string token);
        Task<ResultModel> UpdateAssignment(Guid id, AssignmentUpdateReqModel updateForm, string token);
        Task<ResultModel> DeleteAssignment(List<Guid> assignmentId);
        Task<ResultModel> GetAssignmentInformation(Guid assignmentId);
        Task<ExcelTemplateModel> GetExcelTemplateContent(Guid assignmentId);
        Task<ResultModel> ImportAssignmentScore(List<ScoreModel> scoreModel, Guid assignmentId);
        Task<ResultModel> GetSubmissionAssignment(Guid assignmentId, string token);
        Task<ResultModel> SubmitAssignment(SubmissionCreateResModel submissionCreateResModel, Guid assignmentId, string token);
        Task<ResultModel> UpdateSubmissionAssignment(SubmissionCreateResModel submissionUpdateResModel, Guid assignmentId, string token);
        Task<ResultModel> ScoreAssignment(ScoreAssignmentResModel scoreAssignmentResModel, Guid assignmentId);
        Task<ResultModel> UpdateScoreForAssignment(AssignmentUpdateScoreReqModel AssignmentUpdateScoreReqModel, Guid assignmentId);
        Task<ResultModel> GetSubmissionByAssignmentId(Guid id);
    }
}