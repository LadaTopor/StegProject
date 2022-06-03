using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.IO;

namespace StegProject
{
    class Cryption
    {
        const int ENCRYP_PESENT_SIZE = 1;
        const int ENCRYP_TEXT_SIZE = 3;
        const int ENCRYP_TEXT_MAX_SIZE = 999;


        /*Проверяет, зашифрован ли файл,  возвраещает true, если символ в первом пикслеле равен / иначе false */
        static private bool isEncryption(Bitmap scr)
        {
            byte[] rez = new byte[1];
            Color color = scr.GetPixel(0, 0);
            BitArray colorArray = Bits.ByteToBit(color.R); //получаем байт цвета и преобразуем в массив бит
            BitArray messageArray = Bits.ByteToBit(color.R); ;//инициализируем результирующий массив бит
            messageArray[0] = colorArray[0];
            messageArray[1] = colorArray[1];

            colorArray = Bits.ByteToBit(color.G);//получаем байт цвета и преобразуем в массив бит
            messageArray[2] = colorArray[0];
            messageArray[3] = colorArray[1];
            messageArray[4] = colorArray[2];

            colorArray = Bits.ByteToBit(color.B);//получаем байт цвета и преобразуем в массив бит
            messageArray[5] = colorArray[0];
            messageArray[6] = colorArray[1];
            messageArray[7] = colorArray[2];
            rez[0] = Bits.BitToByte(messageArray); //получаем байт символа, записанного в 1 пикселе
            string m = Encoding.GetEncoding(1251).GetString(rez);
            if (m == "/")
            {
                return true;
            }
            else return false;
        }

        /*Нормализует количество символов для шифрования,чтобы они всегда занимали ENCRYP_TEXT_SIZE байт*/
        static private byte[] NormalizeWriteCount(byte[] CountSymbols)
        {
            int PaddingByte = ENCRYP_TEXT_SIZE - CountSymbols.Length;

            byte[] WriteCount = new byte[ENCRYP_TEXT_SIZE];

            for (int j = 0; j < PaddingByte; j++)
            {
                WriteCount[j] = 0x30;
            }

            for (int j = PaddingByte; j < ENCRYP_TEXT_SIZE; j++)
            {
                WriteCount[j] = CountSymbols[j - PaddingByte];
            }
            return WriteCount;
        }

        /*Записыает количество символов для шифрования в первые биты картинки */
        static private void WriteCountText(int count, Bitmap src)
        {
            byte[] CountSymbols = Encoding.GetEncoding(1251).GetBytes(count.ToString());

            if (CountSymbols.Length < ENCRYP_TEXT_SIZE)
            {
                CountSymbols = NormalizeWriteCount(CountSymbols);
            }

            for (int i = 0; i < ENCRYP_TEXT_SIZE; i++)
            {
                BitArray bitCount = Bits.ByteToBit(CountSymbols[i]); //биты количества символов
                Color pColor = src.GetPixel(0, i + 1);
                BitArray bitsCurColor = Bits.ByteToBit(pColor.R); //бит цветов текущего пикселя
                bitsCurColor[0] = bitCount[0];
                bitsCurColor[1] = bitCount[1];
                byte nR = Bits.BitToByte(bitsCurColor); //новый бит цвета пиксея

                bitsCurColor = Bits.ByteToBit(pColor.G);//бит бит цветов текущего пикселя
                bitsCurColor[0] = bitCount[2];
                bitsCurColor[1] = bitCount[3];
                bitsCurColor[2] = bitCount[4];
                byte nG = Bits.BitToByte(bitsCurColor);//новый цвет пиксея

                bitsCurColor = Bits.ByteToBit(pColor.B);//бит бит цветов текущего пикселя
                bitsCurColor[0] = bitCount[5];
                bitsCurColor[1] = bitCount[6];
                bitsCurColor[2] = bitCount[7];
                byte nB = Bits.BitToByte(bitsCurColor);//новый цвет пиксея

                Color nColor = Color.FromArgb(nR, nG, nB); //новый цвет из полученных битов
                src.SetPixel(0, i + 1, nColor); //записали полученный цвет в картинку
            }
        }

        static private void SetCryption(Bitmap bPic)
        {
            byte[] Symbol = Encoding.GetEncoding(1251).GetBytes("/");
            BitArray ArrBeginSymbol = Bits.ByteToBit(Symbol[0]);
            Color curColor = bPic.GetPixel(0, 0);
            BitArray tempArray = Bits.ByteToBit(curColor.R);
            tempArray[0] = ArrBeginSymbol[0];
            tempArray[1] = ArrBeginSymbol[1];
            byte nR = Bits.BitToByte(tempArray);

            tempArray = Bits.ByteToBit(curColor.G);
            tempArray[0] = ArrBeginSymbol[2];
            tempArray[1] = ArrBeginSymbol[3];
            tempArray[2] = ArrBeginSymbol[4];
            byte nG = Bits.BitToByte(tempArray);

            tempArray = Bits.ByteToBit(curColor.B);
            tempArray[0] = ArrBeginSymbol[5];
            tempArray[1] = ArrBeginSymbol[6];
            tempArray[2] = ArrBeginSymbol[7];
            byte nB = Bits.BitToByte(tempArray);

            Color nColor = Color.FromArgb(nR, nG, nB);
            bPic.SetPixel(0, 0, nColor);
        }

        /*Читает количество символов для дешифрования из первых бит картинки*/
        static private int ReadCountText(Bitmap src)
        {
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

        static public void Enсrypt(Bitmap bPic, FileStream rText)
        {
            BinaryReader bText = new BinaryReader(rText, Encoding.ASCII);

            List<byte> bList = new List<byte>();
            while (bText.PeekChar() != -1)
            { //считали весь текстовый файл для шифрования в лист байт
                bList.Add(bText.ReadByte());
            }

            bText.Close();

            int CountText = bList.Count; // в CountText - количество в байтах текста, который нужно закодировать

            //проверям, что размер не выходит за рамки максимального, поскольку для хранения размера используется
            //ограниченное количество байт
            if (CountText > (ENCRYP_TEXT_MAX_SIZE - ENCRYP_PESENT_SIZE - ENCRYP_TEXT_SIZE))
            {
                MessageBox.Show("Размер текста велик для данного алгоритма, уменьшите размер", "Информация", MessageBoxButtons.OK);
                return;
            }

            //проверяем, поместится ли исходный текст в картинке
            if (CountText > (bPic.Width * bPic.Height))
            {
                MessageBox.Show("Выбранная картинка мала для размещения выбранного текста", "Информация", MessageBoxButtons.OK);
                return;
            }

            //проверяем, может быть картинка уже зашифрована
            if (isEncryption(bPic))
            {
                MessageBox.Show("Файл уже зашифрован", "Информация", MessageBoxButtons.OK);
                return;
            }

            SetCryption(bPic);
            //то есть в первом пикселе будет символ /, который говорит о том, что картика зашифрована

            WriteCountText(CountText, bPic); //записываем количество символов для шифрования


            int index = 0;
            bool st = false;
            for (int i = ENCRYP_TEXT_SIZE + 1; i < bPic.Width; i++)
            {
                for (int j = 0; j < bPic.Height; j++)
                {
                    Color pixelColor = bPic.GetPixel(i, j);
                    if (index == bList.Count)
                    {
                        st = true;
                        break;
                    }
                    BitArray colorArray = Bits.ByteToBit(pixelColor.R);
                    BitArray messageArray = Bits.ByteToBit(bList[index]);
                    colorArray[0] = messageArray[0]; //меняем
                    colorArray[1] = messageArray[1]; // в нашем цвете биты
                    byte newR = Bits.BitToByte(colorArray);

                    colorArray = Bits.ByteToBit(pixelColor.G);
                    colorArray[0] = messageArray[2];
                    colorArray[1] = messageArray[3];
                    colorArray[2] = messageArray[4];
                    byte newG = Bits.BitToByte(colorArray);

                    colorArray = Bits.ByteToBit(pixelColor.B);
                    colorArray[0] = messageArray[5];
                    colorArray[1] = messageArray[6];
                    colorArray[2] = messageArray[7];
                    byte newB = Bits.BitToByte(colorArray);

                    Color newColor = Color.FromArgb(newR, newG, newB);
                    bPic.SetPixel(i, j, newColor);
                    index++;
                }
                if (st)
                {
                    break;
                }
            }
        }

        static public byte[] Decrypt(Bitmap bPic, FileStream rFile)
        {

            if (!isEncryption(bPic))
            {
                MessageBox.Show("В файле нет зашифрованной информации", "Информация", MessageBoxButtons.OK);
                rFile.Close();
                return null;
            }

            int countSymbol = ReadCountText(bPic); //считали количество зашифрованных символов
            byte[] message = new byte[countSymbol];
           

            int index = 0;
            bool st = false;
            for (int i = ENCRYP_TEXT_SIZE + 1; i < bPic.Width; i++)
            {
                for (int j = 0; j < bPic.Height; j++)
                {
                    Color pixelColor = bPic.GetPixel(i, j);
                    if (index == message.Length)
                    {
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
                if (st)
                {
                    break;
                }
            }
            return message;
        }
    }
}
