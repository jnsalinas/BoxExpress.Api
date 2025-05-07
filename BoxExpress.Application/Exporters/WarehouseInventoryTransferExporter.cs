using BoxExpress.Application.Dtos;
using BoxExpress.Application.Interfaces;
using ClosedXML.Excel;

namespace BoxExpress.Application.Exporters;

public class WarehouseInventoryTransferExporter : IExcelExporter<WarehouseInventoryTransferDto>
{
    public byte[] ExportToExcel(List<WarehouseInventoryTransferDto> data)
    {
        var colombiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"); //todo: poner en un helper para hacer la exportacion con hora colombia

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("MovimientoInventario");

        int row = 1;
        worksheet.Cell(1, row++).Value = "Id";
        worksheet.Cell(1, row++).Value = "Origen";
        worksheet.Cell(1, row++).Value = "Destino";
        worksheet.Cell(1, row++).Value = "Creado por";
        worksheet.Cell(1, row++).Value = "Fecha";
        worksheet.Cell(1, row++).Value = "Estado";
        worksheet.Cell(1, row++).Value = "Aceptado por";
        worksheet.Cell(1, row++).Value = "Motivo de rechazo";
        worksheet.Cell(1, row++).Value = "Productos";
        worksheet.Cell(1, row++).Value = "Fecha de actualizacion";

        int rowAux;
        for (int i = 0; i < data?.Count; i++)
        {
            rowAux = 1;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].Id;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].FromWarehouse;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].ToWarehouse;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].Creator;
            worksheet.Cell(i + 2, rowAux++).Value = TimeZoneInfo.ConvertTimeFromUtc(data[i].CreatedAt.Value.ToUniversalTime(), colombiaTimeZone);
            worksheet.Cell(i + 2, rowAux++).Value = data[i].StatusName;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].AcceptedBy;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].RejectionReason;

            string contains = string.Empty;
            if (data[i].TransferDetails?.Any() ?? false)
            {
                contains = string.Empty;
                for (int j = 0; j < data?[i]?.TransferDetails?.Count; j++)
                {
                    var item = data[i]?.TransferDetails?[j];
                    contains += (item?.Product ?? "") + " " + (item?.ProductVariant ?? "") + " " + item?.Quantity;

                    if (j < data?[i]?.TransferDetails?.Count - 1)
                    {
                        contains += ", ";
                    }
                }

            }
            worksheet.Cell(i + 2, rowAux++).Value = contains;
            string updateDatedAt = string.Empty;
            if (data != null && data[i].UpdatedAt != null)
            {
                updateDatedAt = TimeZoneInfo.ConvertTimeFromUtc(data[i].UpdatedAt.Value.ToUniversalTime(), colombiaTimeZone).ToString();
            }
            worksheet.Cell(i + 2, rowAux++).Value = updateDatedAt;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
