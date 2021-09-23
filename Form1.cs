using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeecesPieces
{
    public partial class Form1 : Form
    {
        public int meeceWidth;
        public int meeceHeight;

        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Program.GameCore.Init();
            GameField.Refresh();
        }

        private void GameField_Paint(object sender, PaintEventArgs e)
        {
            for (int y = 0; y < Meeces.Height; y++)
            {
                for (int x = 0; x < Meeces.Width; x++)
                {
                    Meece m;
                    if ((m = Program.GameCore.GetMeece(x, y)) != null)
                    {
                        Graphics g = e.Graphics;
                        g.DrawImage(m.bitmap, x * 34 + 1, y * 34 + 1);
                    }
                }
            }
            toolStripLabel1.Image = Meeces.Current.bitmap;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            SetGameFieldSize();
        }


        private void GameField_MouseClick(object sender, MouseEventArgs e)
        {
            int x = (e.X) / meeceWidth;
            int y = (e.Y) / meeceHeight;
            if (Program.GameCore.TrySetCurrentMeece(x, y))
                GameField.Refresh();
        }

        private void SetGameFieldSize()
        {
            int w = Meeces.Width;
            int h = Meeces.Height;
            using (Bitmap bmp = Properties.Resources.Background)
            {
                meeceWidth = bmp.Width;
                meeceHeight = bmp.Height;
            }
            w *= meeceWidth;
            h *= meeceHeight;
            w -= GameField.Width;
            h -= GameField.Height;
            Width += w + 4;
            Height += h + 4;
        }
    }
}

