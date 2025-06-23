using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.WebUI.Utils
{
    public class FileHelper
    {
        private static readonly string BaseImagePath = "/img/Products/";
        private static readonly string MainImagePath = Path.Combine(BaseImagePath, "MainImage");
        private static readonly string OtherImagesPath = Path.Combine(BaseImagePath, "OtherImages");
        private static readonly string ColorImagesPath = Path.Combine(BaseImagePath, "ColorImages");

        // Initialize directories on first use
        static FileHelper()
        {
            EnsureDirectoriesExist();
        }

        private static void EnsureDirectoriesExist()
        {
            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            // Create base directories if they don't exist
            Directory.CreateDirectory(Path.Combine(webRootPath, MainImagePath.TrimStart('/', '\\')));
            Directory.CreateDirectory(Path.Combine(webRootPath, OtherImagesPath.TrimStart('/', '\\')));
            Directory.CreateDirectory(Path.Combine(webRootPath, ColorImagesPath.TrimStart('/', '\\')));
        }

        // Main product image upload
        public static async Task<string> FileLoaderAsync(IFormFile formFile, string filePath = null)
        {
            if (formFile == null || formFile.Length == 0)
                return string.Empty;

            filePath ??= MainImagePath;
            string fileName = GenerateUniqueFileName(formFile.FileName);
            string fullPath = GetFullPath(filePath, fileName);

            await SaveFileAsync(formFile, fullPath);
            return fileName;
        }

        // Multiple files upload for other product images
        public static async Task<List<string>> MultipleFileLoaderAsync(List<IFormFile> formFiles, string filePath = null)
        {
            var fileNames = new List<string>();

            if (formFiles == null || !formFiles.Any())
                return fileNames;

            filePath ??= OtherImagesPath;

            foreach (var file in formFiles.Where(f => f?.Length > 0))
            {
                string fileName = GenerateUniqueFileName(file.FileName);
                string fullPath = GetFullPath(filePath, fileName);

                await SaveFileAsync(file, fullPath);
                fileNames.Add(fileName);
            }

            return fileNames;
        }

        // Save multiple color-specific images
        public static async Task<List<string>> SaveColorImagesAsync(List<IFormFile> colorImages, int productId, int colorId)
        {
            var fileNames = new List<string>();

            if (colorImages == null || !colorImages.Any() || productId <= 0 || colorId <= 0)
                return fileNames;

            string colorFolderPath = Path.Combine(ColorImagesPath, $"P-{productId}", $"C-{colorId}");

            foreach (var image in colorImages.Where(i => i?.Length > 0))
            {
                string fileName = GenerateUniqueFileName(image.FileName);
                string fullPath = GetFullPath(colorFolderPath, fileName);

                await SaveFileAsync(image, fullPath);
                fileNames.Add(fileName);
            }

            return fileNames;
        }

        // Save single color-specific image
        public static async Task<string> SaveColorImageAsync(IFormFile colorImage, int productId, int colorId)
        {
            if (colorImage == null || colorImage.Length == 0 || productId <= 0 || colorId <= 0)
                return string.Empty;

            string colorFolderPath = Path.Combine(ColorImagesPath, $"P-{productId}", $"C-{colorId}");
            string fileName = GenerateUniqueFileName(colorImage.FileName);
            string fullPath = GetFullPath(colorFolderPath, fileName);

            await SaveFileAsync(colorImage, fullPath);
            return fileName;
        }

        // Delete file
        public static bool FileRemover(string fileName, string filePath = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            try
            {
                filePath ??= BaseImagePath;
                string fullPath = GetFullPath(filePath, fileName);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    CleanEmptyDirectory(Path.GetDirectoryName(fullPath));
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"File deletion error: {ex.Message}");
                return false;
            }
        }

        // Delete color folder and all its contents
        public static bool RemoveColorFolder(int productId, int colorId)
        {
            if (productId <= 0 || colorId <= 0)
                return false;

            try
            {
                string colorFolderPath = Path.Combine(ColorImagesPath, $"P-{productId}", $"C-{colorId}");
                string fullPath = GetFullPath(colorFolderPath, "");

                if (Directory.Exists(fullPath))
                {
                    Directory.Delete(fullPath, true);

                    // Clean up parent directories if they become empty
                    string productFolder = Path.GetDirectoryName(fullPath);
                    CleanEmptyDirectory(productFolder);

                    string colorImagesFolder = Path.GetDirectoryName(productFolder);
                    CleanEmptyDirectory(colorImagesFolder);

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting color folder: {ex.Message}");
                return false;
            }
        }

        #region Helper Methods

        private static string GenerateUniqueFileName(string originalFileName)
        {
            string extension = Path.GetExtension(originalFileName);
            return $"{Guid.NewGuid()}{extension}".ToLower();
        }

        private static string GetFullPath(string relativePath, string fileName)
        {
            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string normalizedPath = relativePath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
            string fullDirectoryPath = Path.Combine(webRootPath, normalizedPath);

            // Ensure directory exists
            Directory.CreateDirectory(fullDirectoryPath);

            return Path.Combine(fullDirectoryPath, fileName);
        }

        private static async Task SaveFileAsync(IFormFile file, string fullPath)
        {
            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        private static void CleanEmptyDirectory(string directoryPath)
        {
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    // Delete directory if it's empty
                    if (!Directory.EnumerateFileSystemEntries(directoryPath).Any())
                    {
                        Directory.Delete(directoryPath);

                        // Recursively clean up parent directories
                        string parentDirectory = Directory.GetParent(directoryPath)?.FullName;
                        if (parentDirectory != null)
                        {
                            CleanEmptyDirectory(parentDirectory);
                        }
                    }
                }
            }
            catch
            {
                // Silent fail on purpose
            }
        }

        #endregion
    }
}