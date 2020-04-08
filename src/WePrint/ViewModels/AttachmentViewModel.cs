using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WePrint.ViewModels
{
    public class AttachmentViewModel
    {
        public Guid Id { get; set; }

        public string FileName { get; set; }

        public string MimeType { get; set; }

        public long FileSize { get; set; }
    }
}
