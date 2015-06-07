using System.Drawing;
using System.Drawing.Imaging;

namespace Utility
{
	public class FastBitmap
	{
        public BitmapData BitmapData;
        public Bitmap Bitmap;
        private Rectangle bounds;

        public FastBitmap(Bitmap bitmap)
        {
            this.Bitmap = bitmap;

            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF boundsF = bitmap.GetBounds(ref unit);

            Point size = new Point((int)boundsF.Width, (int)boundsF.Height);

            bounds = new Rectangle( (int)boundsF.X,
                                    (int)boundsF.Y,
                                    (int)boundsF.Width,
                                    (int)boundsF.Height);
        }

        public void Dispose()
        {
            Bitmap.Dispose();
        }

        public void LockBitmap()
        {
            BitmapData = Bitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
        }

        public void UnlockBitmap()
        {
            Bitmap.UnlockBits(BitmapData);
            BitmapData = null;
        }
	}
}
