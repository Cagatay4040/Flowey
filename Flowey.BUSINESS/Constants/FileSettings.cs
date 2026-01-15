using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Constants
{
    public static class FileSettings
    {
        public const int MaxFileSizeMB = 5;
        public const int MaxFileSystemBytes = MaxFileSizeMB * 1024 * 1024; // 5 MB in Bytes
        public static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx" };
    }
}
