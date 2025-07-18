using BoxExpress.Application.Dtos;
using BoxExpress.Application.Interfaces;
using ClosedXML.Excel;

namespace BoxExpress.Application.Exporters;

public class WarehouseInventoryExporter : IExcelExporter<ProductDto>
{
    public byte[] ExportToExcel(List<ProductDto> data)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Ordenes");

        int row = 1;
        worksheet.Cell(1, row++).Value = "Id";
        worksheet.Cell(1, row++).Value = "Producto";
        worksheet.Cell(1, row++).Value = "Variante";
        worksheet.Cell(1, row++).Value = "Sku";

        int rowAux = 1;
        int columnAux;
        for (int i = 0; i < data?.Count; i++)
        {
            foreach (var item in data[i].Variants)
            {
                columnAux = 1;
                rowAux++;
                worksheet.Cell(rowAux + 2, columnAux++).Value = data[i].Id;
                worksheet.Cell(rowAux + 2, columnAux++).Value = data[i].Name;
                worksheet.Cell(rowAux + 2, columnAux++).Value = item.Name;
                worksheet.Cell(rowAux + 2, columnAux++).Value = item.Sku;
            }
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
