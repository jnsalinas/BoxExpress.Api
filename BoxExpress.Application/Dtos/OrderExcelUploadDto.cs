using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxExpress.Application.Dtos
{
    public class OrderExcelUploadDto
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = string.Empty;
        public int StoreId { get; set; }
        public int CreatorId { get; set; }
    }
}
