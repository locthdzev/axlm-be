using ClosedXML.Excel;
using Data.Models.AssignmentModel;
using Data.Models.FileModel;
using Data.Models.SubmissionModel;
using OfficeOpenXml;

namespace Data.Utilities.Excel
{
    public class Excel : IExcel
    {
        public async Task<List<ScoreModel>> ReadImportScoreExcelFile(ExcelFile excelFile)
        {
            var scoreList = new List<ScoreModel>();
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    await excelFile.FileStream.CopyToAsync(ms);
                    using (var package = new ExcelPackage(ms))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;
                        for (int row = 2; row <= rowCount; row++)
                        {
                            scoreList.Add(new ScoreModel
                            {
                                email = worksheet.Cells[row, 3].Value.ToString(),
                                score = worksheet.Cells[row, 4].Value != null ? decimal.Parse(worksheet.Cells[row, 4].Value.ToString()) : 0,
                                isSubmit = worksheet.Cells[row, 5].Value.ToString(),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return scoreList;
        }

        public ExcelResultDTO GenerateExcelTemplateImportScore(ExcelTemplateModel excelModel, string name)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                var sheet = wb.AddWorksheet();
                sheet.Cell(1, 1).Value = "No";
                sheet.Cell(1, 2).Value = "Student";
                sheet.Cell(1, 3).Value = "Email";
                sheet.Cell(1, 4).Value = "Score";
                sheet.Cell(1, 5).Value = "Status";

                for (int col = 1; col <= 5; col++)
                {
                    sheet.Cell(1, col).Style.Font.Bold = true;
                }

                var row = 2;

                foreach (var check in excelModel.checks)
                {
                    sheet.Cell(row, 1).Value = row - 1;
                    sheet.Cell(row, 2).Value = check.Student.FullName;
                    sheet.Cell(row, 3).Value = check.Student.Email;
                    sheet.Cell(row, 4).Value = check.Score;
                    sheet.Cell(row, 5).Value = check.isSubmit ? "Submitted" : "Not yet";
                    row++;
                }

                sheet.Cell(1, 30).Value = "Submitted";
                sheet.Cell(2, 30).Value = "Not yet";
                var dataValid = sheet.Range(sheet.Cell(2, 5), sheet.Cell(excelModel.checks.Count + 1, 5)).SetDataValidation();
                dataValid.List(sheet.Range("AD1:AD2"), true);
                dataValid.IgnoreBlanks = false;
                dataValid.InCellDropdown = true;
                dataValid.ErrorMessage = "Just input \"Submitted\" or \"Not yet\"";
                dataValid.ShowErrorMessage = true;

                sheet.Columns().AdjustToContents();

                sheet.Cell(1, 7).Value = "Notice:";
                sheet.Cell(2, 7).Value = "After the deadline of assignment, please change all status to \"Submitted\" and fill in the score with 0 value.";
                sheet.Cell(3, 7).Value = "Then import the file again.";
                sheet.Cell(1, 7).Style.Font.Bold = true;

                sheet.Style.Fill.BackgroundColor = XLColor.White;
                sheet.Style.Font.FontColor = XLColor.FromTheme(XLThemeColor.Text1);

                sheet.Cell(1, 30).Style.Font.FontColor = XLColor.White;
                sheet.Cell(2, 30).Style.Font.FontColor = XLColor.White;

                for (int rowNumber = 1; rowNumber <= sheet.Rows().Count(); rowNumber++)
                {
                    for (int colNumber = 1; colNumber <= 5; colNumber++)
                    {
                        var cell = sheet.Cell(rowNumber, colNumber);
                        cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                        cell.Style.Border.TopBorderColor = XLColor.FromTheme(XLThemeColor.Text1);
                        cell.Style.Border.BottomBorderColor = XLColor.FromTheme(XLThemeColor.Text1);
                        cell.Style.Border.LeftBorderColor = XLColor.FromTheme(XLThemeColor.Text1);
                        cell.Style.Border.RightBorderColor = XLColor.FromTheme(XLThemeColor.Text1);
                    }
                }

                return CreateExcelFile(wb, name);
            }
        }


        private ExcelResultDTO CreateExcelFile(XLWorkbook wb, string name)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                wb.SaveAs(ms);
                var fileContent = ms.ToArray();
                var fileName = name;

                return new ExcelResultDTO
                {
                    FileContent = fileContent,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = fileName
                };
            }
        }
    }
}