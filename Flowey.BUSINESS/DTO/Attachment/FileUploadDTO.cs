using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.DTO.Attachment
{
    public class FileUploadDTO
    {
        public IFormFile File { get; set; }
    }
}
