namespace AssetTracking.Application.Interfaces;

public interface IReportExportService
{
    byte[] ExportToExcel(string sheetName, IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyList<string>> rows);

    byte[] ExportToPdf(string title, IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyList<string>> rows);
}
