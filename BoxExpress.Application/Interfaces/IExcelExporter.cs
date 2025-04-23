namespace BoxExpress.Application.Interfaces;
public interface IExcelExporter<T>
{
    byte[] ExportToExcel(List<T> data);
}