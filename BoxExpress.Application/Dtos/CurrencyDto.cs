using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Dtos
{
    public class CurrencyDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
    }
}
