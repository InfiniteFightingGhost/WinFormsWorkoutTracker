using System;
using System.Drawing;
using System.IO;

namespace WorkoutTracker.RealView.Services
{
    public static class PhotoService
    {
        public static string? SavePhoto(string sourcePath, string subFolder)
        {
            try
            {
                string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WorkoutTracker", "Photos", subFolder);
                if (!Directory.Exists(appData))
                {
                    Directory.CreateDirectory(appData);
                }

                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(sourcePath)}";
                string destPath = Path.Combine(appData, fileName);
                File.Copy(sourcePath, destPath);
                
                return destPath;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Image? LoadPhoto(string? path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return null;

            try
            {
                using (var img = Image.FromFile(path))
                {
                    return new Bitmap(img);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
