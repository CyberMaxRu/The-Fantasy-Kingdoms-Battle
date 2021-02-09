using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
    
namespace Fantasy_King_s_Battle
{
    // Контейнер, содержащий в себе все ImageList'ы
    internal sealed class ImageListContainer
    {
        private string folderResources;
        private ImageList imageListGui;

        public ImageListContainer(string folderResources)
        {
            this.folderResources = folderResources;
            imageListGui = PrepareImageList("Gui.png", 48, 48, true, true);
        }

        internal Image GetImageButton(int imageIndex, ImageState state)
        {
            return imageListGui.Images[imageIndex + (int)state * (int)imageListGui.Tag];
        }

        internal static Image GetImage(ImageList imageList, int imageIndex, ImageState state)
        {
            Debug.Assert(imageIndex >= 0);                
            int index = imageIndex + (int)state * (int)imageList.Tag;
            Debug.Assert(index < imageList.Images.Count, "Попытка взять index=" + index.ToString() + " из " + imageList.Images.Count.ToString());

            return imageList.Images[index];
        }

        internal ImageList PrepareImageList(string filename, int width, int height, bool convertToGrey, bool addOver)
        {
            ImageList il = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(width, height)
            };

            Bitmap bmp = new Bitmap(folderResources + "Icons\\" + filename);
            // Если это многострочная картинка, нарезаем ее в однострочную картинку
            if (bmp.Height % height != 0)
                throw new Exception("Высота многострочной картинки не кратна высоте строки: " + filename);

            AddBitmapToImageList(il, bmp, height);
            il.Tag = il.Images.Count;

            if (convertToGrey == true)
                AddBitmapToImageList(il, GreyBitmap(bmp), height);

            if (addOver == true)
                AddBitmapToImageList(il, BrightBitmap(bmp), height);

            return il;
        }

        private void AddBitmapToImageList(ImageList il, Bitmap bitmap, int height)
        {
            int lines = bitmap.Height / height;
            if (lines > 1)
            {
                for (int i = 0; i < lines; i++)
                {
                    int pics = bitmap.Width / il.ImageSize.Width;
                    for (int j = 0; j < pics; j++)
                    {
                        Bitmap bmpSingleline = new Bitmap(il.ImageSize.Width, height);
                        Graphics g = Graphics.FromImage(bmpSingleline);
                        g.DrawImage(bitmap, 0, 0, new Rectangle(0, i * height, j * il.ImageSize.Width, height), GraphicsUnit.Pixel);
                        il.Images.Add(bmpSingleline);
                        g.Dispose();
                    }
                }
            }
            else
            {
                _ = il.Images.AddStrip(bitmap);
            }
        }

        private Bitmap GreyBitmap(Bitmap bmp)
        {
            Bitmap output = new Bitmap(bmp.Width, bmp.Height);

            // Перебираем в циклах все пиксели исходного изображения
            for (int j = 0; j < bmp.Height; j++)
                for (int i = 0; i < bmp.Width; i++)
                {
                    // получаем (i, j) пиксель
                    uint pixel = (uint)(bmp.GetPixel(i, j).ToArgb());

                    // получаем компоненты цветов пикселя
                    float R = (pixel & 0x00FF0000) >> 16; // красный
                    float G = (pixel & 0x0000FF00) >> 8; // зеленый
                    float B = pixel & 0x000000FF; // синий
                                                  // делаем цвет черно-белым (оттенки серого) - находим среднее арифметическое
                    R = G = B = (R + G + B) / 3.0f;

                    // собираем новый пиксель по частям (по каналам)
                    uint newPixel = ((uint)bmp.GetPixel(i, j).A << 24) | ((uint)R << 16) | ((uint)G << 8) | ((uint)B);

                    // добавляем его в Bitmap нового изображения
                    output.SetPixel(i, j, Color.FromArgb((int)newPixel));
                }

            return output;
        }

        private Bitmap BrightBitmap(Bitmap bmp)
        {
            Bitmap output = new Bitmap(bmp.Width, bmp.Height);

            // Перебираем в циклах все пиксели исходного изображения
            for (int j = 0; j < bmp.Height; j++)
                for (int i = 0; i < bmp.Width; i++)
                {
                    // получаем (i, j) пиксель
                    uint pixel = (uint)(bmp.GetPixel(i, j).ToArgb());

                    // получаем компоненты цветов пикселя
                    float R = (pixel & 0x00FF0000) >> 16; // красный
                    float G = (pixel & 0x0000FF00) >> 8; // зеленый
                    float B = pixel & 0x000000FF; // синий
                                                  // делаем цвет черно-белым (оттенки серого) - находим среднее арифметическое
                    R = G = B = (R + G + B) / 3.0f;

                    // собираем новый пиксель по частям (по каналам)
                    uint newPixel = ((uint)bmp.GetPixel(i, j).A << 24) | ((uint)R << 16) | ((uint)G << 8) | ((uint)B);

                    // добавляем его в Bitmap нового изображения
                    output.SetPixel(i, j, Color.FromArgb((int)newPixel));
                }

            return output;
        }
    }
}
