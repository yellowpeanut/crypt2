using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace crypt2
{
    public partial class Form1 : Form
    {
        private string fileContent = String.Empty;
        public Form1()
        {
            InitializeComponent();
            generatePicture(10000);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "txt files (*.txt)|*.txt";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var fileStream = dialog.OpenFile();
                fileContent = File.ReadAllText(dialog.FileName);
                if (fileContent.Count() == 10000)
                {
                    setPicture(pictureBox1, fileContent);
                    label4.Text = $"Initial file size: {getFileSize(fileContent, 2)} bytes";
                }
                else
                {
                    MessageBox.Show($"File does not contain 10000 symbols! (Symbol count - {fileContent.Count()})", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Error opening the file", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (fileContent == String.Empty)
            {
                MessageBox.Show("Initial file is empty!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string compressedFile = compressFile(fileContent);
            richTextBox1.Text = compressedFile;
            File.WriteAllText("2.txt", compressedFile);
            label6.Text = $"Compressed size: {getFileSize(compressedFile, 2)} bytes";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string compressedFile = File.ReadAllText("2.txt");
            if (compressedFile == String.Empty)
            {
                MessageBox.Show("Compressed file is empty!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string decompressedFile = decompressFile(compressedFile);
            File.WriteAllText("3.txt", decompressedFile);
            setPicture(pictureBox2, decompressedFile);
            label5.Text = $"Decompressed size: {getFileSize(decompressedFile, 2)} bytes";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

    private void setPicture(PictureBox pb, string file)
        {
            var bitmap = new Bitmap(100, 100);
            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    if (file[x * 100 + y] == '0')
                        bitmap.SetPixel(x, y, Color.White);
                    else if (file[x * 100 + y] == '1')
                        bitmap.SetPixel(x, y, Color.Black);
                    else
                        MessageBox.Show("File contains unallowed symbols! Only symbols '1' and '0' are allowed", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            pb.Image = bitmap;
            pb.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private string compressFile(string file)
        {
            string compressedFile = String.Empty;
            int numberOfMatches = 0;
            for (int i = 0; i < file.Length / 8; i++)
            {
                int amount = 0;
                string currByte = file.Substring(i * 8, 8);
                while (amount < 64 && i + 1 < file.Length / 8)
                {
                    string nextByte = file.Substring((i + 1) * 8, 8);
                    if (currByte == nextByte)
                    {
                        i++; amount++;
                    }
                    else
                    {
                        numberOfMatches += amount;
                        break;
                    }
                }
                string counter = "11" + Format(Convert.ToString(amount, 2));
                compressedFile += (counter + currByte);
            }
            MessageBox.Show($"Success!\nAmount of matching bytes: {numberOfMatches}");
            return compressedFile.ToString();
        }
        private string decompressFile(string file)
        {
            string decompressedFile = String.Empty;
            int amount = 0;
            string currByte = String.Empty;
            int len = file.Length;
            for (int i = 0; i < file.Length; i++)
            {
                amount = Convert.ToInt32(file.Substring(i + 2, 6), 2)+1;
                i += 8;
                currByte = file.Substring(i, 8);
                i += 7;
                for (int j = 0; j < amount; j++)
                {
                    decompressedFile += currByte;
                }
            }
            return decompressedFile;
        }

        private string Format(string str)
        {
            string newStr = String.Empty;
            for (int i = 0; i < 6 - str.Length; i++)
            {
                newStr += "0";
            }
            newStr += str;
            return newStr;
        }

        private void generatePicture(int length)
        {
            string text = String.Empty;
            Random rand = new Random();
            for (int i = 0; i < length; i++)
            {
                text += rand.Next(0, 2).ToString();
            }
            File.WriteAllText("1.txt", text);
        }

        private int getFileSize(string file, int bitsPerSymbol)
        {
            return Convert.ToInt32(Math.Ceiling(Convert.ToDouble(file.Length * bitsPerSymbol) / 8.0));
        }
    }
}
