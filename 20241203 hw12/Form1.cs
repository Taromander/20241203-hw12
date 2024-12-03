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

            // �]�m PictureBox �� Click �ƥ�
            pictureBox1.MouseClick += pictureBox1_MouseClick;
            pictureBox2.MouseClick += pictureBox2_MouseClick;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                // Ū�����v����
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "�Ϲ����(JPeg,Gif,Bmp,etc.)|*.jpg;*.jpeg;*.gif;*.bmp;*.tif;*.tiff;*.png|�Ҧ����(*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap MyBitmap = new Bitmap(openFileDialog.FileName);
                    this.pictureBox1.Image = MyBitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "�T������");
            }

        }


        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Ū���k�v����
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "�Ϲ����(JPeg,Gif,Bmp,etc.)|*.jpg;*.jpeg;*.gif;*.bmp;*.tif;*.tiff;*.png|�Ҧ����(*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap MyBitmap = new Bitmap(openFileDialog.FileName);
                    this.pictureBox2.Image = MyBitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "�T������");
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Point translatedPoint = TranslateMouseClickToImagePoint(pictureBox1, e.Location);
            if (translatedPoint == Point.Empty)
            {
                MessageBox.Show("���I���Ϥ����������I�ϰ�I");
                return;
            }

            leftPoint = translatedPoint;
            isLeftPointSelected = true;
            label1.Text = $"���Ϯy��: ({leftPoint.X}, {leftPoint.Y})";
        }

        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            Point translatedPoint = TranslateMouseClickToImagePoint(pictureBox2, e.Location);
            if (translatedPoint == Point.Empty)
            {
                MessageBox.Show("���I���Ϥ����������I�ϰ�I");
                return;
            }

            rightPoint = translatedPoint;
            isRightPointSelected = true;
            label4.Text = $"�k�Ϯy��: ({rightPoint.X}, {rightPoint.Y})";
        }

        private Point TranslateMouseClickToImagePoint(PictureBox pictureBox, Point mouseClick)
        {
            if (pictureBox.Image == null) return Point.Empty;

            // ���o�Ϥ��M PictureBox ���ؤo
            int imgWidth = pictureBox.Image.Width;
            int imgHeight = pictureBox.Image.Height;
            int pbWidth = pictureBox.Width;
            int pbHeight = pictureBox.Height;

            // �p���Y����
            float ratioWidth = (float)pbWidth / imgWidth;
            float ratioHeight = (float)pbHeight / imgHeight;
            float scale = (pictureBox.SizeMode == PictureBoxSizeMode.Zoom) ? Math.Min(ratioWidth, ratioHeight) : Math.Max(ratioWidth, ratioHeight);

            // �p��Ϥ��b PictureBox ���������ܤj�p
            int displayWidth = (int)(imgWidth * scale);
            int displayHeight = (int)(imgHeight * scale);

            // �p��Ϥ��b PictureBox ������m
            int offsetX = (pbWidth - displayWidth) / 2;
            int offsetY = (pbHeight - displayHeight) / 2;

            // �T�{�ƹ��I���O�_�b�Ϥ���ܰϰ줺
            if (mouseClick.X < offsetX || mouseClick.X > offsetX + displayWidth ||
                mouseClick.Y < offsetY || mouseClick.Y > offsetY + displayHeight)
            {
                return Point.Empty; // �I���b�Ϥ���ܰϰ�~
            }

            // �N�ƹ��y���ഫ���Ϥ��y��
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
                        // �N��u�q�����ഫ���@��
                        double baselineMm = baselineCm * 10;

                        // �p����t�]�����^
                        double disparityPixels = Math.Abs(leftPoint.X - rightPoint.X);

                        // �N���t�q�����ഫ���@��
                        double disparityMm = disparityPixels * PixelSize;

                        // ���ҵ��t�O�_���s�A�קK���H�s���~
                        if (disparityMm == 0)
                        {
                            MessageBox.Show("���t���s�A�L�k�p��`�סC���ˬd���I����O�_���T�C", "���~", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // �p��`�ס]��쬰�@�̡^
                        double depthMm = (FocalLength * baselineMm) / disparityMm;

                        // �N�`���ഫ�������A�î榡�ƿ�X
                        double depthCm = depthMm / 10;
                        label5.Text = $"�`��: {depthCm:F2} ����";
                    }
                    else
                    {
                        MessageBox.Show("�п�J���Ī��۾����ʶZ�� (Baseline)�C", "���~", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("�Цb��i�Ϥ���������I������m�C", "���~", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "���~");
            }
        }
    }
}
