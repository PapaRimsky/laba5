using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;

namespace laba5
{
    public partial class Form1 : Form
    {
        public class Rootobject
        {
            public Parsedresult[] ParsedResults { get; set; }
            public int OCRExitCode { get; set; }
        }
        public class Parsedresult
        {
            public object FileParseExitCode { get; set; }
            public string ParsedText { get; set; }
        }
        public string bit { get; set; }
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 8;
        }
        private string getSelectedLanguage()
        {
            string lang = "";
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    lang = "ara";
                    break;

                case 1:
                    lang = "bul";
                    break;

                case 2:
                    lang = "chs";
                    break;
                case 3:
                    lang = "cht";
                    break;
                case 4:
                    lang = "hrv";
                    break;
                case 5:
                    lang = "cze";
                    break;
                case 6:
                    lang = "dan";
                    break;
                case 7:
                    lang = "dut";
                    break;
                case 8:
                    lang = "eng";
                    break;
                case 9:
                    lang = "fin";
                    break;
                case 10:
                    lang = "fre";
                    break;
                case 11:
                    lang = "ger";
                    break;
                case 12:
                    lang = "gre";
                    break;
                case 13:
                    lang = "hun";
                    break;
                case 14:
                    lang = "kor";
                    break;
                case 15:
                    lang = "ita";
                    break;
                case 16:
                    lang = "jpn";
                    break;
                case 17:
                    lang = "pol";
                    break;
                case 18:
                    lang = "por";
                    break;
                case 19:
                    lang = "rus";
                    break;
                case 20:
                    lang = "slv";
                    break;
                case 21:
                    lang = "spa";
                    break;
                case 22:
                    lang = "swe";
                    break;
                case 23:
                    lang = "tur";
                    break;
            }
            return lang;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            bit = "";
            OpenFileDialog fd = new OpenFileDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(fd.FileName);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                bit = fd.FileName;
            }
        }
        private byte[] ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                return imageBytes;
            }
        }
        private async void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            button1.Enabled = false;
            button2.Enabled = false;
                HttpClient httpClient = new HttpClient();
                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new StringContent("d2ef7c2e1f88957"), "apikey"); 
                form.Add(new StringContent(getSelectedLanguage()), "language");
                form.Add(new StringContent("2"), "ocrengine");
                form.Add(new StringContent("true"), "scale");
                form.Add(new StringContent("true"), "istable");
                if (string.IsNullOrEmpty(bit) == false)
                {
                    byte[] imageData = File.ReadAllBytes(bit);
                    form.Add(new ByteArrayContent(imageData, 0, imageData.Length), "image", "image.jpg");
                }
                HttpResponseMessage answer = await httpClient.PostAsync("https://api.ocr.space/Parse/Image", form);
                string strContent = await answer.Content.ReadAsStringAsync();
                Rootobject res = JsonConvert.DeserializeObject<Rootobject>(strContent);
                if (res.OCRExitCode == 1)
                {
                    for (int i = 0; i < res.ParsedResults.Count(); i++)
                    {
                        richTextBox1.Text = richTextBox1.Text + res.ParsedResults[i].ParsedText;
                    }
                }
                else
                {
                    MessageBox.Show("ERROR: " + strContent);
                }
            button1.Enabled = true;
            button2.Enabled = true;
        }
    }
}
