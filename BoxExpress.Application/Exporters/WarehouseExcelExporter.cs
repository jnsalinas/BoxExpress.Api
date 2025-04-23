using BoxExpress.Application.Dtos;
using BoxExpress.Application.Interfaces;
using ClosedXML.Excel;

namespace BoxExpress.Application.Exporters;

public class WarehouseExcelExporter : IExcelExporter<WarehouseDto>
{
    public byte[] ExportToExcel(List<WarehouseDto> data)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Warehouses");

        worksheet.Cell(1, 1).Value = "ID";
        worksheet.Cell(1, 2).Value = "Nombre";
        worksheet.Cell(1, 3).Value = "Ciudad";

        for (int i = 0; i < data.Count; i++)
        {
            worksheet.Cell(i + 2, 1).Value = data[i].Id;
            worksheet.Cell(i + 2, 2).Value = data[i].Name;
            // worksheet.Cell(i + 2, 3).Value = data[i].City;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
