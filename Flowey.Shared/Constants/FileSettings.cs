namespace Flowey.SHARED.Constants
{
    public static class FileSettings
    {
        public const int MaxFileSizeMB = 5;
        public const int MaxFileSystemBytes = MaxFileSizeMB * 1024 * 1024; // 5 MB in Bytes
        public static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx" };
        public static readonly string[] AllowedProfileImageExtensions = { ".jpg", ".jpeg", ".png" };
    }
}
