using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Concurrent;
using {{TARGET_NAMESPACE}}.UI.Properties;
using {{TARGET_NAMESPACE}}.UI.Helpers;


namespace {{TARGET_NAMESPACE}}.UI.Helpers
{
    public static class ResourceImageHelper
    {
        // ذاكرة الكاش تحتفظ بالصور بعد تغيير حجمها (مثلاً: "icon_driver_32x32")
        private static readonly ConcurrentDictionary<string, Image> _resizedCache = new ConcurrentDictionary<string, Image>();

        public static void ApplyEntityIcon(PictureBox pb, string entityName, string action = "")
        {
            if (pb == null || string.IsNullOrWhiteSpace(entityName)) return;

            string baseName = entityName.ToLower();
            string actionSuffix = string.IsNullOrWhiteSpace(action) ? "" : "_" + action.ToLower();

            string specificName = "icon_" + baseName + actionSuffix;
            string fallbackName = "icon_" + baseName;

            // 1. الفحص السريع: هل قمنا بتغيير حجم هذه الصورة لهذا المقاس من قبل؟
            string predictedCacheKey = $"{specificName}_{pb.Width}x{pb.Height}";
            if (_resizedCache.TryGetValue(predictedCacheKey, out Image cachedExact))
            {
                pb.Image = cachedExact;
                return;
            }

            // 2. طلب الصورة الأساسية (Base Image) من كلاس AppIcons بدلاً من ResourceManager
            Image baseImage = AppIcons.GetBaseImage(specificName);
            string finalCacheName = specificName;

            if (baseImage == null)
            {
                baseImage = AppIcons.GetBaseImage(fallbackName);
                finalCacheName = fallbackName;
            }

            if (baseImage == null)
            {
                baseImage = AppIcons.DefaultIcon;
                finalCacheName = "icon_default";
            }

            // الفحص النهائي للكاش باسم الصورة الفعلي
            string finalCacheKey = $"{finalCacheName}_{pb.Width}x{pb.Height}";
            if (_resizedCache.TryGetValue(finalCacheKey, out Image cachedFinal))
            {
                pb.Image = cachedFinal;
                return;
            }

            // 3. تغيير الحجم بدقة عالية وحفظها في الكاش
            var resizedImage = ResizeImage(baseImage, pb.Width, pb.Height);
            _resizedCache[finalCacheKey] = resizedImage;
            pb.Image = resizedImage;

            // ملاحظة هامة جداً: لم نعد نستخدم (baseImage.Dispose) هنا 
            // لأن baseImage هي النسخة المركزية المحفوظة في AppIcons ونريد بقاءها في الذاكرة لتطبيقات أخرى
        }

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
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }
    }
}

//```

//### كيف تتكامل هذه المنظومة في مشروعك؟

//1. * *في الشاشات الأساسية المصممة يدوياً (الـ Dashboard):**
//   لن تستخدم البحث النصي، بل ستستدعي الخصائص السريعة والمضمونة هكذا:
//   ```csharp
//   btnMainDrivers.Image = AppIcons.FlatCar;
//   ```

//2. * *في الشاشات المُولّدة آلياً (Code Generator):**
//   المولد الخاص بك يكتب فقط سطر الاستدعاء النصي، والمحرك يقوم بالباقي من بحث، وتوليد، وتغيير حجم، وكاشينج:
//   ```csharp
//   // الكود الذي ينتجه المولد داخل الشاشة
//   ResourceImageHelper.ApplyEntityIcon(this.pictureBoxHeader, "Driver");