using SBX509;
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
    public partial class podpis_form : Form
    {
        private X509Certificate2 ext_certificate;
        public TElX509Certificate tei_certificate; 
        public podpis_form()
        {
            InitializeComponent();
            //toto je výstup z daného okna
            tei_certificate = new TElX509Certificate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            vyber_certifikat();
        }

        public void nacti_certifikat()
        {
            X509Store AStore;
            X509CertificateCollection AStoreCollection;

            AStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            AStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            AStoreCollection = (X509CertificateCollection)AStore.Certificates;

            foreach (X509Certificate2 x509 in AStoreCollection)
            {
                foreach (X509Extension extension in x509.Extensions)
                {
                    //string ccc = "";
                    
                    //MessageBox.Show("FriendlyName:" + Environment.NewLine + extension.Oid.FriendlyName + Environment.NewLine + "----------------------" + Environment.NewLine +
                    //    "Value:" + Environment.NewLine + extension.Oid.Value + Environment.NewLine + "----------------------" + Environment.NewLine +
                    //    "Extension:" + Environment.NewLine+ extension.Format(true));

                    //ccc = extension.Format(true);


                    //vojsko - dočasná vyjímka

                    if ((extension.Format(true).Contains("1.2.203.27112489.1.10.1.2.6")) || (extension.Format(true).Contains("1.2.203.27112489.1.10.5.1.2")) || (extension.Format(true).Contains("1.2.203.27112489.1.10.6.1.1"))) 
                    {
                        dataGridView2.Rows.Add(x509.GetNameInfo(X509NameType.SimpleName, false), x509.GetNameInfo(X509NameType.SimpleName, true), Convert.ToDateTime(x509.GetExpirationDateString()), x509.FriendlyName, x509.SerialNumber);
                    }

                    if ((extension.Format(true).Contains("0.4.0.194112.1.0")) || (extension.Format(true).Contains("1.2.203.27112489.1.10.1.2.7")))
                    {
                        //MessageBox.Show(extension.Format(true));
                        //QECD
                        if ((x509.NotAfter > DateTime.Now) && (DateTime.Now < Convert.ToDateTime("20.09.2018 00:00")))
                        {
                            dataGridView2.Rows.Add(x509.GetNameInfo(X509NameType.SimpleName, false), x509.GetNameInfo(X509NameType.SimpleName, true), Convert.ToDateTime(x509.GetExpirationDateString()), x509.FriendlyName, x509.SerialNumber);
                        }
                    }
                    else if ((extension.Format(true).Contains("0.4.0.194112.1.2")) || (extension.Format(true).Contains("1.2.203.27112489.1.10.5.1.2")))
                    {
                        //bez QECD
                        if (x509.NotAfter > DateTime.Now)
                        {
                            dataGridView2.Rows.Add(x509.GetNameInfo(X509NameType.SimpleName, false), x509.GetNameInfo(X509NameType.SimpleName, true), Convert.ToDateTime(x509.GetExpirationDateString()), x509.FriendlyName, x509.SerialNumber);
                        }

                    }
                }
            }
            
            dataGridView2.Sort(dataGridViewTextBoxColumn1, ListSortDirection.Ascending);

            dataGridView2.ClearSelection();
        }

        public void vyber_certifikat_by_row_index(int row_index)
        {
            string cert_id = "";

            cert_id = dataGridView2.Rows[row_index].Cells[4].Value.ToString();

            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            X509Certificate2Collection certificateCollection = store.Certificates.Find(X509FindType.FindBySerialNumber, cert_id, false);

            if (certificateCollection.Count == 0)
            {
                MessageBox.Show("Nenalezen certifikát v rámci kolekce !");
                return;
            }
            
            ext_certificate = certificateCollection[0];

            try
            {
                tei_certificate.FromX509Certificate2(ext_certificate);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načtení certifikátu ! [" + ex.Message + "]");
                return;
            }

            DialogResult = DialogResult.OK;
        }

        public void vyber_certifikat()
        {
            if (dataGridView2.SelectedRows.Count == 0)
            {
                MessageBox.Show("Musíte vybrat certifikát !");
                return;
            }

            vyber_certifikat_by_row_index(dataGridView2.SelectedRows[0].Index);
        }

        private void podpis_form_Load(object sender, EventArgs e)
        {
            nacti_certifikat();

            if (DateTime.Now < Convert.ToDateTime("20.09.2018 00:00"))
            { 
                button1.Visible = true;
            }
        }
        
        private void dataGridView2_CellContentDoubleClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }

            vyber_certifikat_by_row_index(e.RowIndex);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //provedu otevření dokumentu

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
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

        private bool nacti_certifikat_from_file(string fileName, string password)
        {
            tei_certificate = new TElX509Certificate();
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
                bool mam_kvalifikovany = false;
                bool QESCD = false;
                //zde prověřím platnosti daného certifikátu
                foreach (X509Extension extension in ext_certificate.Extensions)
                {
                    if (extension.Format(true).Contains("0.4.0.194112.1.0"))
                    {
                        QESCD = false;
                        mam_kvalifikovany = true;
                        vysledek = true;
                    } else if (extension.Format(true).Contains("0.4.0.194112.1.2"))
                    {
                        QESCD = true;
                        mam_kvalifikovany = true;
                        vysledek = true;
                    }
                    else if (extension.Format(true).Contains("1.2.203.27112489.1.10.5.1.2"))
                    {
                        QESCD = true;
                        mam_kvalifikovany = true;
                        vysledek = true;
                    }
                    else if (extension.Format(true).Contains("1.2.203.27112489.1.10.1.2.7"))
                    {
                        QESCD = false;
                        mam_kvalifikovany = true;
                        vysledek = true;
                    }
                }

                if (mam_kvalifikovany == false)
                {
                    MessageBox.Show("Nebyl vybrán kvalifikovaný certifikát !");
                    vysledek = false;
                    return vysledek;

                }

                if (ext_certificate.NotAfter > DateTime.Now)
                {
                    vysledek = true;
                }
                else
                {
                    MessageBox.Show("Vybraný certifkát již není platný (platný do: " + ext_certificate.NotAfter.ToString("dd.MM.yyyy HH:mm:ss") + ") !");
                    vysledek = false;
                    return vysledek;
                }


                if ((DateTime.Now > (Convert.ToDateTime("17.09.2018 00:00"))) && (QESCD == false))
                {
                    MessageBox.Show("Vybraný podpis mus splňovat parametry pro kvalifikovaný elektronický podpis!");
                    vysledek = false;
                    return vysledek;
                }

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

            if (vysledek)
            { 
                try
                {
                    tei_certificate.FromX509Certificate2(ext_certificate);
                }
                catch (Exception ex)
                {
                    nacteno = false;
                    chyba_id = Marshal.GetHRForException(ex);
                    message = ex.Message;
                }
            }



            return vysledek;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //otevřu dialog pro Token

            token_list_form f_token = new token_list_form();
            f_token.ShowDialog();

            if (f_token.DialogResult == DialogResult.OK)
            {
                tei_certificate = f_token.X_tei_certificate;
                DialogResult = DialogResult.OK;
            }
            /*else
            {
                MessageBox.Show("NIC !");
                //vysledek = false;
            }*/
        }
    }
}
