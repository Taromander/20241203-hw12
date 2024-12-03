using System;
using System.Drawing;
using System.Windows.Forms;

namespace _20241203_hw12
{
    public partial class Form1 : Form
    {
        private Point leftPoint;
        private Point rightPoint;
        private bool isLeftPointSelected = false;
        private bool isRightPointSelected = false;

        private const double FocalLength = 12.07; // in mm
        private const double PixelSize = 0.0033450704225352; // in mm

        public Form1()
        {
            InitializeComponent();

            // 設置 PictureBox 的 Click 事件
            pictureBox1.MouseClick += pictureBox1_MouseClick;
            pictureBox2.MouseClick += pictureBox2_MouseClick;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                // 讀取左影像檔
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "圖像文件(JPeg,Gif,Bmp,etc.)|*.jpg;*.jpeg;*.gif;*.bmp;*.tif;*.tiff;*.png|所有文件(*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap MyBitmap = new Bitmap(openFileDialog.FileName);
                    this.pictureBox1.Image = MyBitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息提示");
            }

        }


        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // 讀取右影像檔
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "圖像文件(JPeg,Gif,Bmp,etc.)|*.jpg;*.jpeg;*.gif;*.bmp;*.tif;*.tiff;*.png|所有文件(*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap MyBitmap = new Bitmap(openFileDialog.FileName);
                    this.pictureBox2.Image = MyBitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息提示");
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Point translatedPoint = TranslateMouseClickToImagePoint(pictureBox1, e.Location);
            if (translatedPoint == Point.Empty)
            {
                MessageBox.Show("請點擊圖片內部的紅點區域！");
                return;
            }

            leftPoint = translatedPoint;
            isLeftPointSelected = true;
            label1.Text = $"左圖座標: ({leftPoint.X}, {leftPoint.Y})";
        }

        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            Point translatedPoint = TranslateMouseClickToImagePoint(pictureBox2, e.Location);
            if (translatedPoint == Point.Empty)
            {
                MessageBox.Show("請點擊圖片內部的紅點區域！");
                return;
            }

            rightPoint = translatedPoint;
            isRightPointSelected = true;
            label4.Text = $"右圖座標: ({rightPoint.X}, {rightPoint.Y})";
        }

        private Point TranslateMouseClickToImagePoint(PictureBox pictureBox, Point mouseClick)
        {
            if (pictureBox.Image == null) return Point.Empty;

            // 取得圖片和 PictureBox 的尺寸
            int imgWidth = pictureBox.Image.Width;
            int imgHeight = pictureBox.Image.Height;
            int pbWidth = pictureBox.Width;
            int pbHeight = pictureBox.Height;

            // 計算縮放比例
            float ratioWidth = (float)pbWidth / imgWidth;
            float ratioHeight = (float)pbHeight / imgHeight;
            float scale = (pictureBox.SizeMode == PictureBoxSizeMode.Zoom) ? Math.Min(ratioWidth, ratioHeight) : Math.Max(ratioWidth, ratioHeight);

            // 計算圖片在 PictureBox 中的實際顯示大小
            int displayWidth = (int)(imgWidth * scale);
            int displayHeight = (int)(imgHeight * scale);

            // 計算圖片在 PictureBox 中的位置
            int offsetX = (pbWidth - displayWidth) / 2;
            int offsetY = (pbHeight - displayHeight) / 2;

            // 確認滑鼠點擊是否在圖片顯示區域內
            if (mouseClick.X < offsetX || mouseClick.X > offsetX + displayWidth ||
                mouseClick.Y < offsetY || mouseClick.Y > offsetY + displayHeight)
            {
                return Point.Empty; // 點擊在圖片顯示區域外
            }

            // 將滑鼠座標轉換為圖片座標
            int imgX = (int)((mouseClick.X - offsetX) / scale);
            int imgY = (int)((mouseClick.Y - offsetY) / scale);

            return new Point(imgX, imgY);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (isLeftPointSelected && isRightPointSelected)
                {
                    if (double.TryParse(textBox1.Text, out double baselineCm))
                    {
                        // 將基線從公分轉換為毫米
                        double baselineMm = baselineCm * 10;

                        // 計算視差（像素）
                        double disparityPixels = Math.Abs(leftPoint.X - rightPoint.X);

                        // 將視差從像素轉換為毫米
                        double disparityMm = disparityPixels * PixelSize;

                        // 驗證視差是否為零，避免除以零錯誤
                        if (disparityMm == 0)
                        {
                            MessageBox.Show("視差為零，無法計算深度。請檢查紅點選取是否正確。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // 計算深度（單位為毫米）
                        double depthMm = (FocalLength * baselineMm) / disparityMm;

                        // 將深度轉換為公分，並格式化輸出
                        double depthCm = depthMm / 10;
                        label5.Text = $"深度: {depthCm:F2} 公分";
                    }
                    else
                    {
                        MessageBox.Show("請輸入有效的相機移動距離 (Baseline)。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("請在兩張圖片中選取紅點對應位置。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "錯誤");
            }
        }
    }
}
