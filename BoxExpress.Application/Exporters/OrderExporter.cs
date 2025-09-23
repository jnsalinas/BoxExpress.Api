using BoxExpress.Application.Dtos;
using BoxExpress.Application.Interfaces;
using ClosedXML.Excel;

namespace BoxExpress.Application.Exporters;

public class OrderExporter : IExcelExporter<OrderDto>
{
    public byte[] ExportToExcel(List<OrderDto> data)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Ordenes");

        int row = 1;
        worksheet.Cell(1, row++).Value = "Guía";
        worksheet.Cell(1, row++).Value = "Creacion";
        worksheet.Cell(1, row++).Value = "Tienda";
        worksheet.Cell(1, row++).Value = "Estado";
        worksheet.Cell(1, row++).Value = "Categoría";
        worksheet.Cell(1, row++).Value = "Cliente";
        worksheet.Cell(1, row++).Value = "Documento";
        worksheet.Cell(1, row++).Value = "Teléfono";
        worksheet.Cell(1, row++).Value = "Dirección";
        worksheet.Cell(1, row++).Value = "Complemento";
        worksheet.Cell(1, row++).Value = "Código postal";
        worksheet.Cell(1, row++).Value = "Notas";
        worksheet.Cell(1, row++).Value = "Contiene";
        worksheet.Cell(1, row++).Value = "Valor total";
        worksheet.Cell(1, row++).Value = "Flete";
        worksheet.Cell(1, row++).Value = "Ciudad";

        int rowAux;
        for (int i = 0; i < data?.Count; i++)
        {
            rowAux = 1;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].Id;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].CreatedAt;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].StoreName;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].Status;
            worksheet.Cell(i + 2, rowAux++).Value = data?[i].Category;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].ClientFullName;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].ClientDocument;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].ClientPhone;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].ClientAddress;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].ClientAddressComplement;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].ClientAddressPostalCode;
            worksheet.Cell(i + 2, rowAux++).Value = data[i].Notes;

            string contains = data[i].Contains ?? string.Empty;
            if (data[i].OrderItems?.Any() ?? false)
            {
                contains = string.Empty;
                for (int j = 0; j < data?[i]?.OrderItems?.Count; j++)
                {
                    var item = data[i]?.OrderItems?[j];
                    contains += (item?.ProductName ?? "") + " " + (item?.ProductVariantName ?? "") + " " + (item?.Quantity?.ToString() ?? "");

                    if (j < data?[i]?.OrderItems?.Count - 1)
                    {
                        contains += ", ";
                    }
                }

            }

            worksheet.Cell(i + 2, rowAux++).Value = contains;
            worksheet.Cell(i + 2, rowAux++).Value = data?[i].TotalAmount;
            worksheet.Cell(i + 2, rowAux++).Value = data?[i].DeliveryFee;
            worksheet.Cell(i + 2, rowAux++).Value = data?[i].City;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
