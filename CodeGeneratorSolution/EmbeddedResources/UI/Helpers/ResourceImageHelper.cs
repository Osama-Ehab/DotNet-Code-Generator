using CodeGeneratorSolution.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CodeGeneratorSolution.EmbeddedResources.UI.Helpers
{
    public static class ResourceImageHelper
    {
        // Cache now includes the size in the key (e.g., "icon_driver_64x64")
        private static readonly Dictionary<string, Image> _imageCache = new Dictionary<string, Image>();

        public static void ApplyEntityIcon(PictureBox pb, string entityName, string action = "")
        {
            if (pb == null || string.IsNullOrWhiteSpace(entityName)) return;

            string baseName = entityName.ToLower();
            string actionSuffix = string.IsNullOrWhiteSpace(action) ? "" : "_" + action.ToLower();

            // 1. Define our search targets
            string specificName = "icon_" + baseName + actionSuffix; // e.g., "icon_user_add"
            string fallbackName = "icon_" + baseName;                // e.g., "icon_user"

            // 2. Try to find the highly specific action icon first
            Image originalImage = Resources.ResourceManager.GetObject(specificName) as Image;
            string finalCacheName = specificName;

            // 3. First Fallback: The specific action was missing, look for the base entity icon
            if (originalImage == null)
            {
                originalImage = Resources.ResourceManager.GetObject(fallbackName) as Image;
                finalCacheName = fallbackName;
            }

            // 4. Ultimate Fallback: Developer forgot both, use the system default
            if (originalImage == null)
            {
                originalImage = Resources.icon_default;
                finalCacheName = "icon_default";
            }

            // 5. Caching and Resizing (Same perfectly sized math from before!)
            string cacheKey = $"{finalCacheName}_{pb.Width}x{pb.Height}";

            if (_imageCache.ContainsKey(cacheKey))
            {
                pb.Image = _imageCache[cacheKey];
                return;
            }

            var resizedImage = ResizeImage(originalImage, pb.Width, pb.Height);
            _imageCache[cacheKey] = resizedImage;
            pb.Image = resizedImage;
        }

        // High-Quality scaling algorithm
        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                {
                    wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }
    }
}