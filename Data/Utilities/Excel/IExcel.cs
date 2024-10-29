using Data.Models.AssignmentModel;
using Data.Models.FileModel;
using Data.Models.SubmissionModel;

namespace Data.Utilities.Excel
{
    public interface IExcel
    {
        public Task<List<ScoreModel>> ReadImportScoreExcelFile(ExcelFile excelFile);
        public ExcelResultDTO GenerateExcelTemplateImportScore(ExcelTemplateModel excelModel, string name);
    }
}