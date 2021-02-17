using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
    
namespace Fantasy_Kingdoms_Battle
{
    internal sealed class M2Font
    {
        internal Bitmap[] symbols;

        public M2Font(string dirResources)
        {
            Bitmap bmpFonts = new Bitmap(dirResources + @"Fonts\font12b.png");

            // Загружаем tuv-файл
            string file = File.ReadAllText(dirResources + @"Fonts\font12b.tuv", Encoding.GetEncoding(1251));
            string [] conf = file.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            int count = Convert.ToInt32(conf[1].Substring(9));

            symbols = new Bitmap[count];

            // Получаем конфигурацию расположения символов
            int left, leftAndWidth, top, topAndHeight, width, height;
            string line;
            Bitmap sym;
            Graphics g;
            for (int i = 0; i < count; i++)
            {
                line = conf[i + 3];
                left = GetValue();
                top = GetValue();
                leftAndWidth = GetValue();
                topAndHeight = GetValue();
                width = leftAndWidth - left + 1;
                height = topAndHeight - top + 1;

                // Создаем картинку буквы
                sym = new Bitmap(width, height);
                g = Graphics.FromImage(sym);
                g.DrawImage(bmpFonts, 0, 0, new Rectangle(left, top, width, height), GraphicsUnit.Pixel);
                g.Dispose();

                symbols[i] = sym;
            }

            bmpFonts.Dispose();

            int GetValue()
            {
                string v;
                if (line.IndexOf('\t') != -1)
                {
                    v = line.Substring(0, line.IndexOf('\t'));
                    line = line.Substring(v.Length + 1);
                }
                else
                    v = line;

                return Convert.ToInt32(v);
            }
        }
    }
}
