using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Models.EmailModel
{
    public class EmailSendingModel
    {
        public string email { get; set; } = null!;
        public string html { get; set; } = null!;
    }
}