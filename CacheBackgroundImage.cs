﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace Fantasy_Kingdoms_Battle
{
    // Класс коллекции фоновых картинок, с разным расширением
    sealed internal class CollectionBackgroundImage
    {
        private List<(string, Size, Bitmap)> cacheImages = new List<(string, Size, Bitmap)>();
        private readonly string folder;

        public CollectionBackgroundImage()
        {
            folder = Program.FolderResources + @"Icons\Backgrounds\";
        }

        internal Bitmap GetBitmap(string nameImage, Size size)
        {
            // Ищем файл в кэше
            foreach ((string, Size, Bitmap) i in cacheImages)
            {
                if ((i.Item1 == nameImage) && i.Item2.Equals(size))
                    return i.Item3;
            }

            // Получаем имя файла
            string nameImageWithSize = nameImage + $"_{size.Width}_{size.Height}.png";

            // Если такой файл уже есть, загружаем его и отдаём
            if (File.Exists(folder + nameImageWithSize))
            {
                Bitmap preparedBmp = Utils.LoadBitmap(@"Backgrounds\" + nameImageWithSize);
                Utils.Assert(preparedBmp.Size.Equals(size));
                cacheImages.Add((nameImage, size, preparedBmp));
                return preparedBmp;
            }

            // Загружаем оригинальную картинку
            Bitmap orgBmp = Utils.LoadBitmap(@"Backgrounds\" + nameImage + ".png");

            // Если её размер равен требуемому, отдаём как есть
            if (orgBmp.Size.Equals(size))
            {
                cacheImages.Add((nameImage, size, orgBmp));
                return orgBmp;
            }

            Bitmap newBmp;
            Graphics g;
            Bitmap readyBmp;

            // Подводим под требуемый размер 
            // Для этого определяем, на какой коэффициент надо изменить каждую из сторон
            double coefWidth = (double)size.Width / orgBmp.Width;
            double coefHeight = (double)size.Height / orgBmp.Height;
            double coefCommon = Math.Max(coefWidth, coefHeight);
            Size newSize = new Size(Convert.ToInt32(orgBmp.Width * coefCommon), Convert.ToInt32(orgBmp.Height * coefCommon));

            // Если размер требуемый, возвращаем измененную картинку как есть
            if (newSize.Equals(size))
            {
                newBmp = new Bitmap(newSize.Width, newSize.Height);

                g = Graphics.FromImage(newBmp);
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.DrawImage(orgBmp, 0, 0, newBmp.Width, newBmp.Height);
                g.Dispose();

                newBmp.Save(folder + nameImageWithSize);
                orgBmp.Dispose();

                cacheImages.Add((nameImage, size, newBmp));
                return newBmp;
            }

            // Создаем картинку, пропорциональную исходной
            newBmp = new Bitmap(newSize.Width, newSize.Height);

            g = Graphics.FromImage(newBmp);
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.DrawImage(orgBmp, 0, 0, newBmp.Width, newBmp.Height);
            g.Dispose();

            // Если высота больше требуемой, отрезаем низ, иначе отрезаем края
            if (newSize.Height > size.Height)
            {
                readyBmp = new Bitmap(size.Width, size.Height);
                g = Graphics.FromImage(readyBmp);
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.DrawImage(newBmp, 0, 0, new Rectangle(0, 0, size.Width, size.Height), GraphicsUnit.Pixel);
                g.Dispose();
            }
            else
            {
                readyBmp = new Bitmap(size.Width, size.Height);
                g = Graphics.FromImage(readyBmp);
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.DrawImage(newBmp, 0, 0, new Rectangle(Convert.ToInt32((newSize.Width - size.Width) / 2), 0, size.Width, size.Height), GraphicsUnit.Pixel);
                g.Dispose();
            }

            readyBmp.Save(folder + nameImageWithSize);

            orgBmp.Dispose();

            cacheImages.Add((nameImage, size, readyBmp));
            return readyBmp;
        }
    }
}