using SBASN1;
using SBASN1Tree;
using SBCustomCertStorage;
using SBPKCS11Base;
using SBPKCS11CertStorage;
using SBX509;
using SBX509Ext;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace ETRActiveXSign.formulare
{
   

    public partial class token_list_form : Form
    {
        //tady udělám seznam tokenů
        private TElPKCS11SessionInfo Session;
        private TElX509Certificate Cert;
        private TElPKCS11CertStorage PKCS11CertStorage;
        public TElX509Certificate X_tei_certificate;
        public token_list_form()
        {
            InitializeComponent();
            SBUtils.Unit.SetLicenseKey("5E1C594FD406A87D1AD78BD5FCEA6FEEE38E810EB2FBDBC0D2256E9DBAD4D41776F64EF9252CBB192C73CDDB50C9E16E77D0635DC51244BFECC4F1A8539AE70505B7C95FE34A43CCBA155DA8730346C7154F294051EDFBE3F83637A09981F7D82F3F37FFF4AE8C89640094BE8BCCE8CE98966FAC32130DF51ED9AA38EE29EDCEC2F81866424B90656003B0532F65CB2C88A99F4EA7252A57CAF018913C3452963ECE32020D1BF1EFBDE1B16FA6BA0DAF3D0536ABBC24F9C95E4AF8FE300CF2B3F32EC767279CC091ECDC4B7E4A7D7AFAE9164C1117A9FAC8A4307D36C9A1552FD47247B429B6C1AB5E0291B60FAAFEBC2E79A392537A8CD408664CAC20D8F5D3");

            //Document = new TElPDFDocument();
            //Document.OwnActivatedSecurityHandlers = true;
            //PublicKeyHandler = new TElPDFPublicKeySecurityHandler();
            //CertStorage = new TElMemoryCertStorage();
            Cert = new TElX509Certificate();
            PKCS11CertStorage = new TElPKCS11CertStorage();
            X_tei_certificate = new TElX509Certificate();
            //HTTPClient = new TElHTTPSClient();
            //TSPClient = new TElHTTPTSPClient();
            //SystemStore = new TElWinCertStorage();
        }

        private void token_list_form_Load(object sender, EventArgs e)
        {
            //rbPKCS11Cert.Enabled = false;
            //cmbPKCS11Certificates.Items.Clear();
            Session = null;
            PKCS11CertStorage.Close();

            //PKCS11CertStorage.DLLName = @"C:\windows\System32\BIT4XPKI.DLL";
            PKCS11CertStorage.DLLName = @"c:\windows\SysWOW64\BIT4XPKI.DLL"; 
            OpenPKCS11Storage();

        }

        void OpenPKCS11Storage()
        {
            try
            {
                PKCS11CertStorage.Open();

                for (int i = 0; i < PKCS11CertStorage.Module.SlotCount; i++)
                {
                    TElPKCS11SlotInfo SlotInfo = PKCS11CertStorage.Module.get_Slot(i);
                    if (SlotInfo.TokenPresent)
                    {
                        dataGridView2.Rows.Add(SlotInfo.TokenLabel, SlotInfo.SlotDescription, i);
                    }

                    dataGridView2.ClearSelection();
                }
            }
            catch (Exception Ex)
            {

                int chyba_id = Marshal.GetHRForException(Ex);
                //2146233088
                if (Ex.Message.Contains(" - 2147483647"))
                {
                    MessageBox.Show("Pravděpodobně není nainstalován obslužný software k tokenu!\n\n [" + Ex.Message + "|" + chyba_id.ToString() + "]");
                }
                else if (chyba_id == -2147024885)
                {
                    MessageBox.Show("Pravděpodobně není zapojen token!\n\n [" + Ex.Message + "|" + chyba_id.ToString() + "]");
                }
                else
                {
                    MessageBox.Show("Došlo k chybě při zobrazování tokenu!\n\n [" + Ex.Message + "|" + chyba_id.ToString() + "]");
                }

                DialogResult = DialogResult.Cancel;
            }
            finally
            {
                PKCS11CertStorage.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //kontrola na vybraný certifikát
            vyber_certifikat();
        }

        public void vyber_certifikat()
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Musíte vybrat certifikát !");
                return;
            }

            vyber_certifikat_by_row_index(dataGridView1.SelectedRows[0].Index);
        }

        public void vyber_certifikat_by_row_index(int row_index)
        {            
            int x_k = 0;
            //int x_row_index = 0;

            //x_row_index = row_index;
            //MessageBox.Show(dataGridView1.Rows[0].Cells[5].Value.ToString());

            x_k = Convert.ToInt32(dataGridView1.Rows[row_index].Cells[5].Value.ToString());


            X_tei_certificate = PKCS11CertStorage.get_Certificates(x_k);

            //musím tady provést kontrolu na certifikáty

            //bool mam_kvalifikovany = false;
            //bool QESCD = false;
            //zde prověřím platnosti daného certifikátu
            /*foreach (TElCertificateExtensions extension in X_tei_certificate.Extensions)
            {
                if (extension.Format(true).Contains("0.4.0.194112.1.0"))
                {
                    QESCD = false;
                    mam_kvalifikovany = true;
                    vysledek = true;
                }158,,
                else if (extension.Format(true).Contains("0.4.0.194112.1.2"))
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

           
    */


            DialogResult = DialogResult.OK;
        }

        private void nacti_certifikat_by_row_index(int row_index)
        {
            Cursor = Cursors.WaitCursor;

            int k = Convert.ToInt32(dataGridView2.Rows[row_index].Cells[2].Value.ToString());

            bool RO = PKCS11CertStorage.Module.get_Slot(k).ReadOnly;
            try
            {
                Session = PKCS11CertStorage.OpenSession(k, RO);
            }
            catch (EElCertStorageError)
            {
                if (RO)
                    throw;

                Session = PKCS11CertStorage.OpenSession(k, true);
            }
            bool heslo_ok = false;
            if ((Session != null) && PKCS11CertStorage.Module.get_Slot(k).PinNeeded())
            {
                using (pin_form frmPin = new pin_form())
                {
                    while (frmPin.ShowDialog() == DialogResult.OK)
                    {
                        string Pin = frmPin.pin_token;
                        try
                        {
                            Session.Login((int)SBPKCS11Base.Unit.utUser, Pin);
                            heslo_ok = true;
                            break;
                        }
                        catch (EElPKCS11Error Ex)
                        {
                            if (Ex.ErrorCode == SBPKCS11Common.Unit.SB_CKR_PIN_INCORRECT)
                                MessageBox.Show("Nesprávný PIN", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            else
                                MessageBox.Show("Neplatný PIN: " + Ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception Ex)
                        {
                            Cursor = Cursors.Default;
                            MessageBox.Show("Neplatný PIN: " + Ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                if (heslo_ok)
                {
                    PopulateCertList(PKCS11CertStorage);
                }
                else
                {
                    Cursor = Cursors.Default;
                    return;
                }
            }
            else
            {
                PKCS11CertStorage.Close();
            }

            Cursor = Cursors.Default;
        }

        private void dataGridView2_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }

            nacti_certifikat_by_row_index(e.RowIndex);            
        }

        void PopulateCertList(TElCustomCertStorage certStorage)
        {
            
            Cursor = Cursors.WaitCursor;
            //int i = 0;
            for (int i = 0; i < certStorage.Count; i++)
            {
                TElX509Certificate Cert = certStorage.get_Certificates(i);
                
                TElCertificateExtensions extension = new TElCertificateExtensions();
                extension = Cert.Extensions;

                //zjistím ostatní extension
                int count_other = 0;

                count_other = extension.OtherCount;

                if (count_other >= 0)
                {
                    for (int ic = 0; ic < count_other; ic++)
                    {
                        TElCustomExtension xxxv = Cert.Extensions.get_OtherExtensions(ic);

                        string ccc = SBStrUtils.Unit.OIDToStr(xxxv.OID);
                        if (ccc == "1.3.6.1.5.5.7.1.3")
                        {
                            //mam element qcStatements
                            int typ_kvalifikovaneho_prostredku = 0;

                            if (over_certifikat(xxxv.Value, ref typ_kvalifikovaneho_prostredku))
                            {
                                dataGridView1.Rows.Add(Cert.SubjectName.CommonName, Cert.IssuerName.CommonName, Convert.ToDateTime(Cert.ValidTo), "", SBUtils.Unit.BinaryToString(Cert.SerialNumber), i);
                            }
                        }
                    }
                }                
            }

            Cursor = Cursors.Default;

            if (dataGridView1.RowCount > 0) { 
                dataGridView1.Sort(dataGridViewTextBoxColumn3, ListSortDirection.Ascending);

                dataGridView1.ClearSelection();
                button3.Enabled = true;
            }            
        }

        private bool over_certifikat(byte[] value, ref int typ_cert)
        {
            typ_cert = 0;
            //1-cert
            //2-pečeť

            byte[] etsiQualified = SBStrUtils.Unit.StrToOID("0.4.0.1862.1.1");
                
            byte[] etsiCert = SBStrUtils.Unit.StrToOID("0.4.0.1862.1.6.1");
            byte[] etsiSeal = SBStrUtils.Unit.StrToOID("0.4.0.1862.1.6.2");
            

            bool je_kvalifikovany = false;

            byte[] content = value;
            TElASN1ConstrainedTag tag = new TElASN1ConstrainedTag();
            if (tag.LoadFromBuffer(content))
            {
                if ((tag.Count == 1) && (tag.GetField(0).CheckType(SBASN1Tree.Unit.SB_ASN1_SEQUENCE, true)))
                {
                    TElASN1ConstrainedTag rootSeq = (TElASN1ConstrainedTag)(tag.GetField(0));
                    for (int j = 0; j < rootSeq.Count; j++)
                    {
                        if (rootSeq.GetField(j).CheckType(SBASN1Tree.Unit.SB_ASN1_SEQUENCE, true))
                        {
                            TElASN1ConstrainedTag statementSeq = (TElASN1ConstrainedTag)(rootSeq.GetField(j));
                            if ((statementSeq.Count >= 2) && (statementSeq.GetField(0).CheckType(SBASN1Tree.Unit.SB_ASN1_OBJECT, false)) &&  (statementSeq.GetField(1).CheckType(SBASN1Tree.Unit.SB_ASN1_SEQUENCE, true)))
                            {
                                byte[] statementId = ((TElASN1SimpleTag)(statementSeq.GetField(0))).Content;
                                TElASN1ConstrainedTag infoSeq = (TElASN1ConstrainedTag)(statementSeq.GetField(1));
                                if ((infoSeq.Count >= 1) && (infoSeq.GetField(0).CheckType(SBASN1Tree.Unit.SB_ASN1_OBJECT, false)))
                                {
                                    byte[] statementInfo = ((TElASN1SimpleTag)(infoSeq.GetField(0))).Content;
                                    if (SBUtils.Unit.CompareContent(statementInfo, etsiCert))
                                    {
                                        typ_cert = 1;
                                        break;
                                    }
                                    if (SBUtils.Unit.CompareContent(statementInfo, etsiSeal))
                                    {
                                        typ_cert = 2;
                                        break;
                                    }

                                }
                            }
                            else if ((statementSeq.Count >= 1) && (statementSeq.GetField(0).CheckType(SBASN1Tree.Unit.SB_ASN1_OBJECT, false)))
                            {
                                byte[] statementIRoot = ((TElASN1SimpleTag)(statementSeq.GetField(0))).Content;
                                if (SBUtils.Unit.CompareContent(statementIRoot, etsiQualified))
                                {
                                    je_kvalifikovany = true;
                                }
                            }
                        }
                    }
                }
            }
            
            return je_kvalifikovany;
        }
        

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }

            nacti_certifikat_by_row_index(e.RowIndex);
        }
    }
}
