using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.EmbeddedResources.UI.Helpers
{
    public static class ImageLoaderHelper
    {
        // Loads and shrinks the image entirely on a background thread!
        public static async Task<Image> LoadThumbnailAsync(string imagePath, int targetWidth, int targetHeight)
        {
            if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
                return null;

            return await Task.Run(() =>
            {
                using (var originalImage = Image.FromFile(imagePath))
                {
                    // Create a small blank canvas
                    var thumbnail = new Bitmap(targetWidth, targetHeight);

                    // Draw the original image onto the small canvas using High Quality math
                    using (var graphics = Graphics.FromImage(thumbnail))
                    {
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;

                        graphics.DrawImage(originalImage, 0, 0, targetWidth, targetHeight);
                    }

                    // Return the tiny, memory-friendly image
                    return thumbnail;
                }
            });
        }
    }
}