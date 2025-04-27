using BoxExpress.Application.Dtos;
using BoxExpress.Application.Interfaces;
using ClosedXML.Excel;

namespace BoxExpress.Application.Exporters;

public class WalletTransactionsExporter : IExcelExporter<WalletTransactionDto>
{
    public byte[] ExportToExcel(List<WalletTransactionDto> data)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Warehouses");

        int row = 1;
        worksheet.Cell(1, row++).Value = "Id";
        worksheet.Cell(1, row++).Value = "Tienda";
        worksheet.Cell(1, row++).Value = "Usuario";
        worksheet.Cell(1, row++).Value = "Descripción";
        worksheet.Cell(1, row++).Value = "Tipo de transacción";
        worksheet.Cell(1, row++).Value = "Estado";
        worksheet.Cell(1, row++).Value = "Fecha";
        worksheet.Cell(1, row++).Value = "Monto";
        worksheet.Cell(1, row++).Value = "Id Orden";

        int rowAux;
        for (int i = 0; i < data.Count; i++)
        {
            rowAux = 1;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].Id;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].Store;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].UserName;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].Description;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].TransactionType;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].OrderStatus;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].CreatedAt;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].Amount;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].RelatedOrderId;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
