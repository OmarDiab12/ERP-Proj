namespace ERP.Helpers
{
    public interface IFileStorageService
    {
        Task<FileSaveResult?> SaveFileAsync(IFormFile file, string subFolder);
        Task DeleteFolderAsync(string subFolder);
        Task<bool> DeleteFileAsync(string filePath);
    }

    public class FileStorageService : IFileStorageService
    {
        private readonly string _basePath;

        public FileStorageService(IConfiguration config)
        {
            _basePath = config["StoragePath"]
                ?? Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        }

        public async Task<FileSaveResult?> SaveFileAsync(IFormFile file, string subFolder)
        {
            if (file == null || file.Length == 0)
                return null;

            // Ensure base + sub folder exists
            string targetDir = Path.Combine(_basePath, subFolder);
            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            // Generate unique file name
            string uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            string fullPath = Path.Combine(targetDir, uniqueName);

            // Save file
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return standardized info
            return new FileSaveResult
            {
                FileName = file.FileName,
                SavedFileName = uniqueName,
                FilePath = fullPath,
                RelativePath = Path.Combine(subFolder, uniqueName)
            };
        }

        public async Task DeleteFolderAsync(string subFolder)
        {
            string targetDir = Path.Combine(_basePath, subFolder);
            if (Directory.Exists(targetDir))
            {
                await Task.Run(() => Directory.Delete(targetDir, recursive: true));
            }
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    return false;

                string fullPath = filePath;

                // If path is relative, combine with base storage path
                if (!Path.IsPathRooted(filePath))
                    fullPath = Path.Combine(_basePath, filePath);

                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                    return true;
                }

                return false; // File not found
            }
            catch (Exception ex)
            {
                // Optional: log this to your error repo if needed
                Console.WriteLine($"Failed to delete file: {ex.Message}");
                return false;
            }
        }

    }
    public class FileSaveResult
    {
        public string FileName { get; set; } = string.Empty; // Original file name
        public string SavedFileName { get; set; } = string.Empty; // Unique generated file name
        public string FilePath { get; set; } = string.Empty; // Full physical path
        public string RelativePath { get; set; } = string.Empty; // Relative path (optional for download serving)
    }
}
