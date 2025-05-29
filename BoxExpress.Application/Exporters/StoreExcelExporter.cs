using BoxExpress.Application.Dtos;
using BoxExpress.Application.Interfaces;
using ClosedXML.Excel;

namespace BoxExpress.Application.Exporters;

public class StoreExcelExporter : IExcelExporter<StoreDto>
{
    public byte[] ExportToExcel(List<StoreDto> data)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Balance");

        worksheet.Cell(1, 1).Value = "Id";
        worksheet.Cell(1, 2).Value = "Nombre";
        worksheet.Cell(1, 3).Value = "Ciudad";
        worksheet.Cell(1, 3).Value = "País";
        worksheet.Cell(1, 3).Value = "Balance";
        worksheet.Cell(1, 3).Value = "Disponible para retiro";
        worksheet.Cell(1, 3).Value = "Pendiente por retiro";

        int columnAux;
        for (int i = 0; i < data.Count; i++)
        {
            columnAux = 1;
            worksheet.Cell(i + 2, columnAux++).Value = data[i].Id;
            worksheet.Cell(i + 2, columnAux++).Value = data[i].Name;
            worksheet.Cell(i + 2, columnAux++).Value = data[i].City;
            worksheet.Cell(i + 2, columnAux++).Value = data[i].Country;
            worksheet.Cell(i + 2, columnAux++).Value = data[i].Balance;
            worksheet.Cell(i + 2, columnAux++).Value = data[i].AvailableToWithdraw;
            worksheet.Cell(i + 2, columnAux++).Value = data[i].PendingWithdrawals;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
