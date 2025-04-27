using BoxExpress.Application.Dtos;
using BoxExpress.Application.Interfaces;
using ClosedXML.Excel;

namespace BoxExpress.Application.Exporters;

public class OrderStatusHistoryExporter : IExcelExporter<OrderStatusHistoryDto>
{
    public byte[] ExportToExcel(List<OrderStatusHistoryDto> data)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Warehouses");

        int row = 1;
        worksheet.Cell(1, row++).Value = "Id";
        worksheet.Cell(1, row++).Value = "Usuario";
        worksheet.Cell(1, row++).Value = "Estado anterior";
        worksheet.Cell(1, row++).Value = "Nuevo estado";

        int rowAux;
        for (int i = 0; i < data.Count; i++)
        {
            rowAux = 1;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].Id;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].OldStatus;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].NewStatus;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].CreatedAt;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
