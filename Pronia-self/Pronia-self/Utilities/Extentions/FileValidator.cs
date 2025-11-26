using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pronia_self.Models;
using Pronia_self.Utilities.Enums;

namespace Pronia_self.Utilities.Extentions
{
    public static class FileValidator
    {
        public static bool ValidateType(this IFormFile file,string type)
        {
            return file.ContentType.Contains(type);

        }

        public static bool ValidateSize(this IFormFile file ,FileSize fileSize,int size)
        {
            switch (fileSize)
            {
                case FileSize.KB:
                    return (file.Length < size * 1024);
                case FileSize.MB:
                    return (file.Length < size * 1024*1024);
                case FileSize.GB:
                    return (file.Length < size * 1024*1024*1024);
            }

            return false;
        }
        public static async Task<string> CreateFileAsync(this IFormFile file, params string[] roots )
        {
            string fileName = string.Concat(Guid.NewGuid(), Path.GetExtension(file.FileName));
            string path = fileName.CreatePath(roots);
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return fileName;

        }

        public static void DeleteFile(this string deletedName,params string[] roots)
        {
            string path = deletedName.CreatePath(roots);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
