﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxExpress.Application.Dtos
{
    public class OrderExcelUploadResponseDto
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsLoaded { get; set; } = false;
    }
}
