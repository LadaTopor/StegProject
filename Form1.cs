using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace StegProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        const int ENCRYP_PESENT_SIZE = 1;
        const int ENCRYP_TEXT_SIZE = 3;
        const int ENCRYP_TEXT_MAX_SIZE = 999;


        /*Читает количество символов для дешифрования из первых бит картинки*/
        private int ReadCountText(Bitmap src) {
            byte[] rez = new byte[ENCRYP_TEXT_SIZE]; 
            for (int i = 0; i < ENCRYP_TEXT_SIZE; i++)
            {
                Color color = src.GetPixel(0, i + 1); 
                BitArray colorArray = Bits.ByteToBit(color.R); //биты цвета
                BitArray bitCount = Bits.ByteToBit(color.R); ; //инициализация результирующего массива бит
                bitCount[0] = colorArray[0];
                bitCount[1] = colorArray[1];

                colorArray = Bits.ByteToBit(color.G);
                bitCount[2] = colorArray[0];
                bitCount[3] = colorArray[1];
                bitCount[4] = colorArray[2];

                colorArray = Bits.ByteToBit(color.B);
                bitCount[5] = colorArray[0];
                bitCount[6] = colorArray[1];
                bitCount[7] = colorArray[2];
                rez[i] = Bits.BitToByte(bitCount);
            }
            string m = Encoding.GetEncoding(1251).GetString(rez);
            return Convert.ToInt32(m, 10);
        }

        /* Открыть файл для шифрования */
        private void Enrypt_Click(object sender, EventArgs e)
        {
            string FilePic;
            string FileText;
            OpenFileDialog dPic = new OpenFileDialog();
            dPic.Filter = "Файлы изображений (*.bmp)|*.bmp|Все файлы (*.*)|*.*";
            if (dPic.ShowDialog() == DialogResult.OK)
            {
                FilePic = dPic.FileName;
            }
            else
            {
                FilePic = "";
                return;
            }

            FileStream rFile;
            rFile = new FileStream(FilePic, FileMode.Open); //открываем поток
            Bitmap bPic = new Bitmap(rFile);

            OpenFileDialog dText = new OpenFileDialog();
            dText.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            if (dText.ShowDialog() == DialogResult.OK)
            {
                FileText = dText.FileName;
            }
            else
            {
                FileText = "";
                return;
            }

            FileStream rText;

            rText = new FileStream(FileText, FileMode.Open); //открываем поток текстовым файлом
            
            BinaryReader bText = new BinaryReader(rText, Encoding.ASCII);

            List<byte> bList = new List<byte>();
            while (bText.PeekChar() != -1) { //считали весь текстовый файл для шифрования в лист байт
                bList.Add(bText.ReadByte());
            }
            int CountText = bList.Count; // в CountText - количество в байтах текста, который нужно закодировать
            bText.Close();
            rFile.Close();

            //проверям, что размер не выходит за рамки максимального, поскольку для хранения размера используется
            //ограниченное количество байт
            if (CountText > (ENCRYP_TEXT_MAX_SIZE - ENCRYP_PESENT_SIZE - ENCRYP_TEXT_SIZE)) {
                MessageBox.Show("Размер текста велик для данного алгоритма, уменьшите размер", "Информация", MessageBoxButtons.OK);
                return;
            }

            //проверяем, поместится ли исходный текст в картинке
            if (CountText > (bPic.Width * bPic.Height)) {
                MessageBox.Show("Выбранная картинка мала для размещения выбранного текста", "Информация", MessageBoxButtons.OK);
                return;
            }

            //проверяем, может быть картинка уже зашифрована
            if (Cryption.isEncryption(bPic))
            {
                MessageBox.Show("Файл уже зашифрован", "Информация", MessageBoxButtons.OK);
                return;
            }

            Cryption.SetCryption(bPic);
            //то есть в первом пикселе будет символ /, который говорит о том, что картика зашифрована

            Cryption.WriteCountText(CountText, bPic); //записываем количество символов для шифрования

            Cryption.Enrypt(bPic, bList); // Кодируем сообщение
            pictureBox1.Image = bPic;

            String sFilePic;
            SaveFileDialog dSavePic = new SaveFileDialog();
            dSavePic.Filter = "Файлы изображений (*.bmp)|*.bmp|Все файлы (*.*)|*.*";
            if (dSavePic.ShowDialog() == DialogResult.OK)
            {
                sFilePic = dSavePic.FileName;
            }
            else
            {
                sFilePic = "";
                return;
            };

            FileStream wFile;
            wFile = new FileStream(sFilePic, FileMode.Create); //открываем поток на запись результатов

            bPic.Save(wFile, System.Drawing.Imaging.ImageFormat.Bmp);
            wFile.Close(); //закрываем поток
        }

        /*Открыть файл для дешифрования */
        private void Decrypt_Click(object sender, EventArgs e)
        {
            string FilePic;
            OpenFileDialog dPic = new OpenFileDialog();
            dPic.Filter = "Файлы изображений (*.bmp)|*.bmp|Все файлы (*.*)|*.*";
            if (dPic.ShowDialog() == DialogResult.OK)
            {
                FilePic = dPic.FileName;
            }
            else
            {
                FilePic = "";
                return;
            }

            FileStream rFile;
            try
            {
                rFile = new FileStream(FilePic, FileMode.Open); //открываем поток
            }
            catch (IOException)
            {
                MessageBox.Show("Ошибка открытия файла", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Bitmap bPic = new Bitmap(rFile);
            if (!Cryption.isEncryption(bPic)) {
                MessageBox.Show("В файле нет зашифрованной информации", "Информация", MessageBoxButtons.OK);
                rFile.Close();
                return;
            }

            int countSymbol = ReadCountText(bPic); //считали количество зашифрованных символов
            byte[] message = new byte[countSymbol];
            int index = 0;
            bool st = false;
            for (int i = ENCRYP_TEXT_SIZE + 1; i < bPic.Width; i++) {
                for (int j = 0; j < bPic.Height; j++) {
                    Color pixelColor = bPic.GetPixel(i, j);
                    if (index == message.Length) {
                        st = true;
                        break;
                    }
                    BitArray colorArray = Bits.ByteToBit(pixelColor.R);
                    BitArray messageArray = Bits.ByteToBit(pixelColor.R); ;
                    messageArray[0] = colorArray[0];
                    messageArray[1] = colorArray[1];

                    colorArray = Bits.ByteToBit(pixelColor.G);
                    messageArray[2] = colorArray[0];
                    messageArray[3] = colorArray[1];
                    messageArray[4] = colorArray[2];

                    colorArray = Bits.ByteToBit(pixelColor.B);
                    messageArray[5] = colorArray[0];
                    messageArray[6] = colorArray[1];
                    messageArray[7] = colorArray[2];
                    message[index] = Bits.BitToByte(messageArray);
                    index++;
                }
                if (st) {
                    break;
                }
            }
            string strMessage = Encoding.GetEncoding(1251).GetString(message);

            string sFileText;
            SaveFileDialog dSaveText = new SaveFileDialog();
            dSaveText.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            if (dSaveText.ShowDialog() == DialogResult.OK)
            {
                sFileText = dSaveText.FileName;
            }
            else
            {
                sFileText = "";
                rFile.Close();
                return;
            };

            FileStream wFile;
            try
            {
                wFile = new FileStream(sFileText, FileMode.Create); //открываем поток на запись результатов
            }
            catch (IOException)
            {
                MessageBox.Show("Ошибка открытия файла на запись", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                rFile.Close();
                return;
            }
            StreamWriter wText = new StreamWriter(wFile, Encoding.Default);
            wText.Write(strMessage);
            MessageBox.Show("Текст записан в файл", "Информация", MessageBoxButtons.OK);
            wText.Close();
            wFile.Close(); //закрываем поток
            rFile.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
