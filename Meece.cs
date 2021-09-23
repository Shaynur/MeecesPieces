using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace MeecesPieces
{
    public class Meece
    {
        /// <summary>
        /// 3 свойства фишки:
        /// [0]: Фигура,
        /// [1]: Цвет,
        /// [2]: Число.
        /// </summary>
        public int[] Value = new int[3];

        /// <summary>
        /// Максимально возможные значения свойств фишки
        /// ( Value = [0...MaxValue-1] )
        /// </summary>
        public static int[] MaxValue = new int[] { 3, 4, 5 };

        /// <summary>
        /// Свойство экстра-фишки
        /// 0 - не экстра,
        /// 1 - Супер-фишка (может присоединиться к любым фишкам),
        /// 2 - Фишка-стиратель (может удалить существующую фишку с поля).
        /// </summary>
        public int Extra;

        /// <summary>
        /// Фишка может стать "экстра" при инициализации
        /// </summary>
        public static bool CanBeExtra = true;

        /// <summary>
        /// Шанс рождения экстра-фишки
        /// rand.Next(ChanceOfExtra) :
        /// 1 - Супер-фишка
        /// 2 - "Стиратель"
        /// _ - Все остальные значения диапазона [0...ChanceOfExtra-1] - простая фишка
        /// </summary>
        public static int ChanceOfExtra = 50;

        /// <summary>
        /// Генератор случайных чисел (инициализируется от текущего значения времени)
        /// </summary>
        public static Random rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

        /// <summary>
        /// Изображение фишки
        /// </summary>
        public Bitmap bitmap;

        //-----------------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        public Meece()
        {
            Init();
        }

        /// <summary>
        /// Инициализатор. Генерирует свойства фишки с ГСЧ
        /// </summary>
        public void Init()
        {
            Extra = 0;
            if (CanBeExtra)
            {
                int chance = rand.Next(ChanceOfExtra - 1);
                if (chance == 1)     //  1 of ChanceOfExtra
                {
                    Extra = 1;
                }
                else if (chance == 2)
                {
                    Extra = 2;
                }
            }
            if (Extra == 0)
            {
                Value[0] = rand.Next(MaxValue[0]);
                Value[1] = rand.Next(MaxValue[1]);
                Value[2] = rand.Next(MaxValue[2]);
            }
            else
            {
                Value[0] = 0;
                Value[1] = 0;
                Value[2] = 0;
            }
            SetBitmap();
        }

        /// <summary>
        /// Проверяет совместимы ли фишка текущего экземпляра и фишка переданная в параметре.
        /// Если любая из двух фишек - экстра, считается что совместимы.
        /// </summary>
        /// <param name="meece">Проверяемая на совместимость фишка</param>
        /// <returns>true - совместима,
        /// false - не совместима</returns>
        public bool CanAttach(Meece meece)
        {
            if (this.Extra == 1 || meece.Extra == 1)
            {
                return true;
            }
            for (int i = 0; i < Value.Length; i++)
            {
                if (this.Value[i] == meece.Value[i])
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Генерация изображения фишки
        /// </summary>
        private void SetBitmap()
        {
            if(Extra == 1)
            {
                bitmap = new Bitmap(Properties.Resources.Extra1);
                return;
            }
            else if(Extra == 2)
            {
                bitmap = new Bitmap(Properties.Resources.Extra2);
                return;
            }

            bitmap = Value[0] switch
            {
                0 => new Bitmap(Properties.Resources._00),
                1 => new Bitmap(Properties.Resources._01),
                _ => new Bitmap(Properties.Resources._02)
            };

            Color clr = Value[1] switch
            {
                0 => Color.Blue,
                1 => Color.Red,
                2 => Color.Lime,
                3 => Color.Yellow,
                _ => Color.White
            };

            ColorMap[] colorMap = new ColorMap[1];
            colorMap[0] = new ColorMap();
            colorMap[0].OldColor = Color.Magenta;
            colorMap[0].NewColor = clr;
            ImageAttributes attr = new ImageAttributes();
            attr.SetRemapTable(colorMap);
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(bitmap, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, attr);
                if (Value[2] > 0)
                {
                    Bitmap bitmap2 = Value[2] switch
                    {
                        1 => new Bitmap(Properties.Resources._21),
                        2 => new Bitmap(Properties.Resources._22),
                        3 => new Bitmap(Properties.Resources._23),
                        _ => new Bitmap(Properties.Resources._24)
                    };
                    g.DrawImage(bitmap2, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, attr);
                }
            }
        }
    }
}
