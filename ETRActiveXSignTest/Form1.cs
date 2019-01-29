using ETRActiveXSign;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace ETRActiveXSignTest
{
    public partial class Form1 : Form
    {

        public int vysledek;
        public string nazev_souboru;
        public string cesta_souboru;
        public byte[] data_souboru = null;
        public ETRPDFSigner ETRPDFSigner = null;
        public string zzz = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ETRPDFSigner = new ETRPDFSigner();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ID.Text = "";
            //ID.Text = ETRPDFSigner.UkazForm();
            
            dopln_text("certifikát načten");
            
        }

        private void button9_Click(object sender, EventArgs e)
        {
            cesta_souboru = @"d:\recorder_chart.pdf";

            textBox1.Text = cesta_souboru;
            dopln_text("Soubor načten");
            nazev_souboru = Path.GetFileName(cesta_souboru);

            data_souboru = File.ReadAllBytes(cesta_souboru);

            dopln_text("PdfFile nastaveno");
            //ETRPDFSigner.PdfFile = data_souboru.ToString();
            //ETRPDFSigner.PdfFile = System.Text.Encoding.UTF8.GetString(data_souboru);
            ETRPDFSigner._PdfFile = Convert.ToBase64String(data_souboru);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            
            ETRPDFSigner._TSProxyURL = "https://mid-tsa.disig.sk/cmtsa";
            ETRPDFSigner._TSA_User = "authtoken0801";
            ETRPDFSigner._TSA_Pass = "eughahr7meiphiF";
            ETRPDFSigner._AddTimeStamp = "A";

            dopln_text("TSA nastaveno");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            cesta_souboru = @"c:\work\374_2011.pdf";

            textBox1.Text = cesta_souboru;
            dopln_text("Soubor načten");
            nazev_souboru = Path.GetFileName(cesta_souboru);

            data_souboru = File.ReadAllBytes(cesta_souboru);
        }

        private void button12_Click(object sender, EventArgs e)
        {

            ETRPDFSigner._CerSerNumber = "3880C2";
            ETRPDFSigner._DirectSign = "A";


            //Load file
            //cesta_souboru = @"c:\Users\Martin\Documents\Order confirmed _ ASOS.pdf";
            //cesta_souboru = @"d:\recorder_chart.pdf";
            //cesta_souboru = @"d:\a_132456789_sig1.pdf";
            //cesta_souboru = @"d:\Testovací dokument.pdf";
            cesta_souboru = @"c:\work\374_2011.pdf";

            textBox1.Text = cesta_souboru;
            nazev_souboru = Path.GetFileName(cesta_souboru);
            data_souboru = File.ReadAllBytes(cesta_souboru);
            dopln_text("Soubor načten");

            //set byte
            //ETRPDFSigner.PdfFile = data_souboru.ToString();
            //ETRPDFSigner.PdfFile = System.Text.Encoding.UTF8.GetString(data_souboru);
            ETRPDFSigner._PdfFile = Convert.ToBase64String(data_souboru);
            ETRPDFSigner._PdfFile_1 = Convert.ToBase64String(data_souboru);
            ETRPDFSigner._PdfFile_2 = Convert.ToBase64String(data_souboru);
            

            dopln_text("PdfFile nastaveno");

            if (radioButton1.Checked) { 
                ETRPDFSigner._InVisible = "N";
            }
            else
            {
                ETRPDFSigner._InVisible = "A";
            }


            if (radioButton3.Checked)
            {
                ETRPDFSigner._VisibleSide = "LH";
            }
            if (radioButton4.Checked)
            {
                ETRPDFSigner._VisibleSide = "LS";
            }
            if (radioButton5.Checked)
            {
                ETRPDFSigner._VisibleSide = "PH";
            }
            if (radioButton6.Checked)
            {
                ETRPDFSigner._VisibleSide = "PS";
            }

            if (radioButton8.Checked)
            {
                ETRPDFSigner._Sig_Page = "F";
            }
            if (radioButton7.Checked)
            {
                ETRPDFSigner._Sig_Page = "L";
            }
            /*if (radioButton9.Checked)
            {
                ETRPDFSigner._Sig_Page = "A";
            }
            */
            if (radioButton14.Checked)
            {
                ETRPDFSigner._Sig_Page = "S";

                ETRPDFSigner._Sig_Page_By_User = textBox7.Text;
            }

            if (radioButton10.Checked)
            {
                ETRPDFSigner._podklad = "N";
            }
            if (radioButton11.Checked)
            {
                ETRPDFSigner._podklad = "";
            }

            if (radioButton12.Checked)
            {
                ETRPDFSigner._AddTimeStamp = "N";
            }







            if (radioButton13.Checked)
            {
                //ETRPDFSigner._AddTimeStamp = "A";
                ETRPDFSigner._TSProxyURL = "https://mid-tsa.disig.sk/cmtsa";
                ETRPDFSigner._TSA_User = "authtoken0801";
                ETRPDFSigner._TSA_Pass = "eughahr7meiphiF";
                ETRPDFSigner._AddTimeStamp = "A";

                dopln_text("TSA nastaveno");
            }


            ETRPDFSigner._text_do_podpisu = textBox4.Text;
            ETRPDFSigner._text_do_podpisu_doplneni = textBox6.Text;

            //ETRPDFSigner.

            //dopln_text("InVisible - false");
            //dopln_text("Podpis");

            try
            {
                vysledek = ETRPDFSigner.Sign();
            }
            catch (Exception xxx)
            {
                MessageBox.Show(xxx.Message);
                //throw;
            }
            

            if (vysledek == 0)
            {
                textBox1.Text = "0";
                dopln_text("podepsáno");
                
                textBox3.Text = ETRPDFSigner.Ret_jmeno;
                textBox2.Text = ETRPDFSigner.Ret_RDM;
                textBox1.Text = ETRPDFSigner.Ret_Sig_DateTime;
                File.WriteAllBytes(@"d:\a_132456789.pdf", Convert.FromBase64String(ETRPDFSigner._PdfFile));
                Process.Start(@"d:\a_132456789.pdf");
            }
            else
            {
                textBox1.Text = vysledek.ToString();
                dopln_text(ETRPDFSigner.LastError);
            }          
        }

        private void dopln_text(string vvvstup)
        {
            if (textBox2.Text == "")
            {
                textBox2.Text = vvvstup;
            }
            else
            {
                textBox2.Text = textBox2.Text + Environment.NewLine + vvvstup;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ID.Text = "";
            ID.Text = ETRPDFSigner.GetVersion();
            //MessageBox.Show(ID.Text);
            MessageBox.Show(ETRPDFSigner.StatusKod + " " + ETRPDFSigner.StatusText);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //image

            string aaa = "";


            data_souboru = File.ReadAllBytes(@"D:\ETR8_15.jpf");
            aaa = Convert.ToBase64String(data_souboru);


            MemoryStream stream = new MemoryStream(data_souboru);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ETRPDFSigner.UkazForm();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ETRPDFSigner.Registrace();

            MessageBox.Show(ETRPDFSigner.Ret_jmeno + " " + ETRPDFSigner.Ret_RDM + " " + ETRPDFSigner.Ret_Serial_Number + " " + ETRPDFSigner.Ret_Serial_Number_RDM);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            //Load file
            //cesta_souboru = @"c:\Users\Martin\Documents\Order confirmed _ ASOS.pdf";
            //cesta_souboru = @"d:\recorder_chart.pdf";
            //cesta_souboru = @"d:\a_132456789_sig1.pdf";
            //cesta_souboru = @"d:\Testovací dokument.pdf";
            cesta_souboru = @"d:\PDF-grafický manuál.pdf";

            byte[] data_souboru_1 = null;
            byte[] data_souboru_2 = null;
            textBox1.Text = cesta_souboru;
            nazev_souboru = Path.GetFileName(cesta_souboru);
            data_souboru = File.ReadAllBytes(@"d:\Informace_o_zpracovani_osobnich_udaju.pdf");
            data_souboru_1 = File.ReadAllBytes(@"d:\PT_prohlaseni.pdf");
            data_souboru_2 = File.ReadAllBytes(@"d:\PT_zdravotni_formular.pdf");
            dopln_text("Soubor načten");

            //set byte
            //ETRPDFSigner.PdfFile = data_souboru.ToString();
            //ETRPDFSigner.PdfFile = System.Text.Encoding.UTF8.GetString(data_souboru);
            ETRPDFSigner._PdfFile = Convert.ToBase64String(data_souboru);
            ETRPDFSigner._PdfFile_1 = Convert.ToBase64String(data_souboru_1);
            ETRPDFSigner._PdfFile_2 = Convert.ToBase64String(data_souboru_2);


            dopln_text("PdfFile nastaveno");

            if (radioButton1.Checked)
            {
                ETRPDFSigner._InVisible = "N";
            }
            else
            {
                ETRPDFSigner._InVisible = "A";
            }


            if (radioButton3.Checked)
            {
                ETRPDFSigner._VisibleSide = "LH";
            }
            if (radioButton4.Checked)
            {
                ETRPDFSigner._VisibleSide = "LS";
            }
            if (radioButton5.Checked)
            {
                ETRPDFSigner._VisibleSide = "PH";
            }
            if (radioButton6.Checked)
            {
                ETRPDFSigner._VisibleSide = "PS";
            }

            if (radioButton8.Checked)
            {
                ETRPDFSigner._Sig_Page = "F";
            }
            if (radioButton7.Checked)
            {
                ETRPDFSigner._Sig_Page = "L";
            }
            /*if (radioButton9.Checked)
            {
                ETRPDFSigner._Sig_Page = "A";
            }
            */
            if (radioButton14.Checked)
            {
                ETRPDFSigner._Sig_Page = "S";

                ETRPDFSigner._Sig_Page_By_User = textBox7.Text;
            }

            if (radioButton10.Checked)
            {
                ETRPDFSigner._podklad = "N";
            }
            if (radioButton11.Checked)
            {
                ETRPDFSigner._podklad = "";
            }

            if (radioButton12.Checked)
            {
                ETRPDFSigner._AddTimeStamp = "N";
            }

            if (radioButton13.Checked)
            {
                //ETRPDFSigner._AddTimeStamp = "A";
                ETRPDFSigner._TSProxyURL = "https://mid-tsa.disig.sk/cmtsa";
                ETRPDFSigner._TSA_User = "authtoken0801";
                ETRPDFSigner._TSA_Pass = "eughahr7meiphiF";
                ETRPDFSigner._AddTimeStamp = "A";

                dopln_text("TSA nastaveno");
            }

            ETRPDFSigner._cnnData = "N";
            ETRPDFSigner._cnnSoubory = "N";
            ETRPDFSigner._multi_file_id = "N";

            ETRPDFSigner._text_do_podpisu = textBox4.Text;
            ETRPDFSigner._text_do_podpisu_doplneni = textBox6.Text;

            //ETRPDFSigner.

            //dopln_text("InVisible - false");
            //dopln_text("Podpis");

            try
            {
                vysledek = ETRPDFSigner.MultiSign();
            }
            catch (Exception xxx)
            {
                MessageBox.Show(xxx.Message);
                //throw;
            }


            if (vysledek == 0)
            {
                textBox1.Text = "0";
                dopln_text("podepsáno");

                textBox3.Text = ETRPDFSigner.Ret_jmeno;
                textBox2.Text = ETRPDFSigner.Ret_RDM;
                textBox1.Text = ETRPDFSigner.Ret_Sig_DateTime;
                //File.WriteAllBytes(@"d:\a_132456789.pdf", Convert.FromBase64String(ETRPDFSigner._PdfFile));
                //Process.Start(@"d:\a_132456789.pdf");
            }
            else
            {
                textBox1.Text = vysledek.ToString();
                dopln_text(ETRPDFSigner.LastError);
            }
        }
    }
}
