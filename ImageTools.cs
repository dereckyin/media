using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace media
{
    public static class ImageTools
    {
        public static Image ResizeImage(Image originalBitmap, int newWidth, int newHeight)
        {
            
            originalBitmap.Mutate(x => x.Resize(newWidth, newHeight));
            return originalBitmap;
        }

        public static byte[] ToByteArray(Image image)
        {
            MemoryStream memoryStream = new MemoryStream();
            image.SaveAsPng(memoryStream);

            return memoryStream.ToArray();
        }

        public static Image GetNoImage()
        {
            string _root = Directory.GetCurrentDirectory();
         
            Image image = Image.Load(_root + @"/images/noImage_218x160_notfind.jpg");
            return image;
        }

    }
}
