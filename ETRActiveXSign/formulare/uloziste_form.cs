using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace ETRActiveXSign.formulare
{
    public partial class uloziste_form : Form
    {
        public X509Certificate2 ext_certificate;

        public uloziste_form()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //storno
            DialogResult = DialogResult.Cancel;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //win_store
            podpis_form f_podpis = new podpis_form();
            f_podpis.ShowDialog();

            if (f_podpis.DialogResult == DialogResult.OK)
            {
               // ext_certificate = f_podpis.ext_certificate;
                DialogResult = DialogResult.OK;
            }            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //pfx
            //načtu certifikát ze souboru PFX
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog1.Filter = "PKCS #12 files (*.PFX;*.P12)|*.PFX;*.P12";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (nacti_certifikat_from_file(openFileDialog1.FileName, ""))
                {
                    DialogResult = DialogResult.OK;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //token
            pin_form f_podpis = new pin_form();
            f_podpis.ShowDialog();

            if (f_podpis.DialogResult == DialogResult.OK)
            {
                //ext_certificate = f_podpis.ext_certificate;
                //DialogResult = DialogResult.OK;
            }
        }

        private bool nacti_certifikat_from_file(string fileName, string password)
        {

            //, string v, X509KeyStorageFlags machineKeySet
            bool vysledek = false;
            bool nacteno = false;
            int chyba_id = 0;
            string message = "";

            try
            {
                ext_certificate = new X509Certificate2(fileName, password, X509KeyStorageFlags.MachineKeySet);
                nacteno = true;
            }
            catch (Exception ex)
            {
                nacteno = false;
                chyba_id = Marshal.GetHRForException(ex);
                message = ex.Message;
            }

            if (nacteno)
            {
                vysledek = true;
            }
            else
            {
                if (chyba_id == -2147024810)
                {
                    pass_form f_podpis = new pass_form();
                    f_podpis.ShowDialog();

                    if (f_podpis.DialogResult == DialogResult.OK)
                    {
                        vysledek = nacti_certifikat_from_file(fileName, f_podpis.cert_heslo);
                    }
                    else
                    {
                        MessageBox.Show("Nebylo zadáno heslo k certifikátu !");
                        vysledek = false;
                    }


                }
                else
                {
                    MessageBox.Show("Cyba při načtení certifikátu (" + message + ") !");
                    vysledek = false;
                }
            }


            return vysledek;
        }
    }
}
