namespace MeecesPieces
{
    public class Meeces
    {
        public static int Width = 9;   // Кол-во фишек по горизонтали
        public static int Height = 9;  // Кол-во фишек по вертикали
        public static Meece Current;   // Текущая фишка для установки на поле
        private Meece[,] meeces;       // Двумерный массив фишек на поле
        private int[] dx = new int[] { 0, 1, 0, -1 };   // Массивы смещений координат для обхода
        private int[] dy = new int[] { -1, 0, 1, 0 };   //  четырёх соседних фишек.

        /// <summary>
        /// Конструктор
        /// </summary>
        public Meeces()
        {
            Init();
        }

        /// <summary>
        /// Инициализатор поля с текущими размерами
        /// </summary>
        public void Init()
        {
            Init(Width, Height);
        }

        /// <summary>
        /// Инициализатор поля с заданными размерами
        /// </summary>
        /// <param name="width">Кол-во фишек по горизонтали</param>
        /// <param name="height">Кол-во фишек по вертикали</param>
        public void Init(int width, int height)
        {
            if (width > 1 && height > 1)
            {
                Width = width;
                Height = height;
            }
            meeces = new Meece[Width, Height];
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    meeces[x, y] = null;
            Current = GetNewCurrent();
        }

        /// <summary>
        /// Проверка границ массива фишек на допустимость
        /// </summary>
        private bool IsValidIndexes(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Возвращает фишку по координатам или null если индексы не в допустимых пределах.
        /// ( Возврат также будет null если по координатам нет фишки
        /// </summary>
        public Meece GetMeece(int x, int y)
        {
            if (IsValidIndexes(x, y))
                return meeces[x, y];
            else
                return null;
        }

        /// <summary>
        /// Попытка поставить текущую фишку (Current) на поле
        /// </summary>
        public bool TrySetCurrentMeece(int x, int y)
        {
            if (CanSetMeece(x, y, Current))
            {
                if (Current.Extra == 2)
                {
                    meeces[x, y] = null;
                }
                else
                {
                    meeces[x, y] = Current;
                    LookForDeleteFullChains();
                }
                Current = GetNewCurrent();
                //Update();
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Проверка на возможность поставить фишку
        /// </summary>
        private bool CanSetMeece(int x, int y, Meece meece)
        {
            if (meeces[x, y] != null)           // Если это не пустое поле (уже стоит фишка) и...
            {
                if (meece.Extra == 2)           // ставится фишка-стиратель, то...
                {
                    return true;                // возвращаем true.
                }
            }
            else                                // Иначе ( если это пустое поле )...
            {
                if (meece.Extra == 2)           // и ставится фишка-стиратель, то...
                    return false;               // возвращаем false.

                bool hasNearby = false;
                for (int d = 0; d < 4; d++)     // Проверяем все 4 поля вокруг...
                {
                    Meece m = GetMeece(x + dx[d], y + dy[d]);
                    if (m != null)              // если одно из 4 проверяемых полей не пустое...
                    {
                        if (!meece.CanAttach(m))    // но при этом несовместимое, то выход
                            return false;
                        hasNearby = true;       // иначе уст. флаг наличия совместимой фишки.
                    }
                }
                if (hasNearby)                  // Если установлено, что имеется фишка к которой можно
                {                               // присоединиться, то
                    return true;                // выходим.
                }
            }                                   // Иначе - рядом нет ни одной фишки для присоединения.
            return false;
        }

        /// <summary>
        /// Проверяет все строки и колонки на заполненность
        /// и потом удаляет полные.
        /// </summary>
        private void LookForDeleteFullChains()
        {
            bool[] isFullX = new bool[Width];
            bool[] isFullY = new bool[Height];
            for (int y = 0; y < Height; y++) isFullY[y] = true;
            for (int x = 0; x < Width; x++) isFullX[x] = true;

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    if (meeces[x, y] == null)
                    {
                        isFullX[x] = false;
                        isFullY[y] = false;
                    }

            for (int y = 0; y < Height; y++)
                if (isFullY[y])
                    for (int x = 0; x < Width; x++)
                        meeces[x, y] = null;

            for (int x = 0; x < Width; x++)
                if (isFullX[x])
                    for (int y = 0; y < Height; y++)
                        meeces[x, y] = null;
        }

        /// <summary>
        /// Получаем новую текущую фишку с проверкой на возможность установки
        /// и на пустоту поля.
        /// </summary>
        /// <returns></returns>
        private Meece GetNewCurrent()
        {
            Meece ret = new Meece();
            while (true)
            {
                bool isEmptyField = true;
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        if (meeces[x, y] != null)
                            isEmptyField = false;
                        if (CanSetMeece(x, y, ret))
                            return ret;
                    }
                }
                if (isEmptyField)
                {
                    while (ret.Extra == 2)
                        ret = new Meece();
                    meeces[Width / 2, Height / 2] = ret;
                    ret = new Meece();
                }
                else
                    ret.Init();
            }
        }
    }
}