﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using SBHTTPTSPClient;
using ETRActiveXSign.formulare;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using SBX509;
using SBPDF;
using SBHTTPSClient;
using System.IO;
using SBPDFSecurity;
using SBCustomCertStorage;
using SBRDN;

namespace ETRActiveXSign
{
	[ProgId("ETRActiveXSign.ETRPDFSigner")]
	[ClassInterface(ClassInterfaceType.AutoDual)]
	[Guid("C37C51A1-916E-4AFE-A212-C52750214A4F")]
	[ComVisible(true)]

	public class ETRPDFSigner
	{
		[ComVisible(true)]


        public string _CerSerNumber = "";
        public string _DirectSign = "";


        public string _PdfFile = "";
		public string _PdfFile_1 = "";
		public string _PdfFile_2 = "";

		private byte[] pdffile = null;
		public string StatusKod = "0000";
		public string StatusText = "";

		public string _Reason = "";
		public string _Location = "";

		//public bool TimeStamp = false;
		public string _InVisible = "A";
		public string _VisibleSide = "LS"; //LH: levý horní; LS: levý spodní; PH: pravý horní; PS: pravý spodní

		public string _Storage = "";  //B - bez, F - FileSystem, W - Windows Store, T - Token
		public string _Cer_Ser_Number = "";  //Seriove čislo certifikatu, použije se pro podpis, musím ale kontrolovat jeho platnost
		public string _Cer_PFX_path = "";  //Cesta k ulozisti v pripade, že jsem ji nadefinoval

		public string _AddTimeStamp = "N";
		public string _TSProxyURL = "";   //https://mid-tsa.disig.sk/cmtsa
		public string _TSA_User = "";   //authtoken0801
		public string _TSA_Pass = "";   //eughahr7meiphiF
		public string _Sig_Page = "F"; //F-prvni, L-posledni, S - konkretni_strana
		public string _Sig_Page_By_User = ""; //F-prvni, L-posledni, S - konkretni_strana

        public string _TSA_proxy_use = "";  //A - použít
		public string _TSA_proxy_host = "";
		public string _TSA_proxy_port = "";

		public string _odsazeni_leve = "";
		public string _odsazeni_prave = "";
		public string _odsazeni_spodni = "";
		public string _odsazeni_horni = "";

		public string _podklad = "";

		public string _text_do_podpisu = "";
		public string _text_do_podpisu_doplneni = "";

		public string _cnnData = "";
		public string _cnnSoubory = "";
		public string _multi_file_id = "";



		public bool ShowErrorMessages = false;
		public string LastError = "";
		public string Ret_jmeno = "";
		public string Ret_RDM = "";
		public string Ret_Serial_Number = "";
		public string Ret_Serial_Number_RDM = "";
		public string Ret_Sig_DateTime = "";


        public string Ret_Signature_Jmeno = "";
        public string Ret_Signature_Serial = "";
        public string Ret_Signature_Serial_HEX = "";
        public string Ret_Signature_Valid_From = "";
        public string Ret_Signature_Valid_To = "";


        
		private string obrazek_etr = "AAAADGpQICANCocKAAAAHGZ0eXBqcDIgAAAAAGpwMiBqcHhianB4IAAAAH9ycmVxAv//+PAABQABgAAABUCAAAwgQAASECAALQgQAAU6DQIYCulBFbN2S8pBzg5xBARHySzM0aFFgbkEOLtUZ3E7BAS8Rad03VBOxqn286E39H6QBATXyMXvlR9DsodXBCUA9TjoBAQsTAEAhQRAuaA+ViFI1t/rBAQAAABjanAyaAAAABZpaGRyAAAAxQAAAPoAAwcHAQAAAAAPY29scgEAAQAAABAAAAAcY2RlZgADAAAAAAABAAEAAAACAAIAAAADAAAAGnJlcyAAAAAScmVzYwsTAAELEwABAAAAAAAZdXVpZDoNAhgK6UEVs3ZLykHODnEAAAAAGXV1aWRHySzM0aFFgbkEOLtUZ3E7AQAAABx1dWlkvEWndN1QTsap9vOhN/R+kAAAADIAAAAcdXVpZNfIxe+VH0Oyh1cEJQD1OOgAAAAyAAAPwHV1aWQsTAEAhQRAuaA+ViFI1t/rOEJJTQQlAAAAAAAQAAAAAAAAAAAAAAAAAAAAADhCSU0EOgAAAAAA8QAAABAAAAABAAAAAAALcHJpbnRPdXRwdXQAAAAFAAAAAFBzdFNib29sAQAAAABJbnRlZW51bQAAAABJbnRlAAAAAENscm0AAAAPcHJpbnRTaXh0ZWVuQml0Ym9vbAAAAAALcHJpbnRlck5hbWVURVhUAAAAAQAAAAAAD3ByaW50UHJvb2ZTZXR1cE9iamMAAAASAE4AYQBzAHQAYQB2AGUAbgDtACAAbgDhAHQAaQBzAGsAdQAAAAAACnByb29mU2V0dXAAAAABAAAAAEJsdG5lbnVtAAAADGJ1aWx0aW5Qcm9vZgAAAAlwcm9vZkNNWUsAOEJJTQQ7AAAAAAItAAAAEAAAAAEAAAAAABJwcmludE91dHB1dE9wdGlvbnMAAAAXAAAAAENwdG5ib29sAAAAAABDbGJyYm9vbAAAAAAAUmdzTWJvb2wAAAAAAENybkNib29sAAAAAABDbnRDYm9vbAAAAAAATGJsc2Jvb2wAAAAAAE5ndHZib29sAAAAAABFbWxEYm9vbAAAAAAASW50cmJvb2wAAAAAAEJja2dPYmpjAAAAAQAAAAAAAFJHQkMAAAADAAAAAFJkICBkb3ViQG/gAAAAAAAAAAAAR3JuIGRvdWJAb+AAAAAAAAAAAABCbCAgZG91YkBv4AAAAAAAAAAAAEJyZFRVbnRGI1JsdAAAAAAAAAAAAAAAAEJsZCBVbnRGI1JsdAAAAAAAAAAAAAAAAFJzbHRVbnRGI1B4bEBSAJOAAAAAAAAACnZlY3RvckRhdGFib29sAQAAAABQZ1BzZW51bQAAAABQZ1BzAAAAAFBnUEMAAAAATGVmdFVudEYjUmx0AAAAAAAAAAAAAAAAVG9wIFVudEYjUmx0AAAAAAAAAAAAAAAAU2NsIFVudEYjUHJjQFkAAAAAAAAAAAAQY3JvcFdoZW5QcmludGluZ2Jvb2wAAAAADmNyb3BSZWN0Qm90dG9tbG9uZwAAAAAAAAAMY3JvcFJlY3RMZWZ0bG9uZwAAAAAAAAANY3JvcFJlY3RSaWdodGxvbmcAAAAAAAAAC2Nyb3BSZWN0VG9wbG9uZwAAAAAAOEJJTQPtAAAAAAAQAEgCTgABAAIASAJOAAEAAjhCSU0EJgAAAAAADgAAAAAAAAAAAAA/gAAAOEJJTQQNAAAAAAAEAAAAHjhCSU0EGQAAAAAABAAAAB44QklNA/MAAAAAAAkAAAAAAAAAAAEAOEJJTScQAAAAAAAKAAEAAAAAAAAAAjhCSU0D9QAAAAAASAAvZmYAAQBsZmYABgAAAAAAAQAvZmYAAQChmZoABgAAAAAAAQAyAAAAAQBaAAAABgAAAAAAAQA1AAAAAQAtAAAABgAAAAAAAThCSU0D+AAAAAAAcAAA/////////////////////////////wPoAAAAAP////////////////////////////8D6AAAAAD/////////////////////////////A+gAAAAA/////////////////////////////wPoAAA4QklNBAAAAAAAAAIAADhCSU0EAgAAAAAAAgAAOEJJTQQwAAAAAAABAQA4QklNBC0AAAAAAAYAAQAAAAU4QklNBAgAAAAAABAAAAABAAACQAAAAkAAAAAAOEJJTQQeAAAAAAAEAAAAADhCSU0EGgAAAAADNQAAAAYAAAAAAAAAAAAAAMUAAAD6AAAAAAAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAQAAAAAAAAAAAAAA+gAAAMUAAAAAAAAAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAA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";
		private TElHTTPTSPClient httpTspClient = new TElHTTPTSPClient();

		private string Kultura = "cs-CZ";

		[ComVisible(true)]
		public string UkazForm()
		{

			token_list_form f_podpis = new token_list_form();
			f_podpis.ShowDialog();

			if (f_podpis.DialogResult == DialogResult.OK)
			{
				//MessageBox.Show(f_podpis.cert_heslo);
			}

			return "OK";

			/*
			podpis_form f_podpis = new podpis_form();
			f_podpis.ShowDialog();

			if (f_podpis.DialogResult == DialogResult.OK)
			{
				//ext_X509Certificate2 = f_podpis.ext_certificate;
				return "OK";
			}
			else
			{
				LastError = "Nebyl vybrán certifikát k podpisu !";
				return LastError;
			} */
		}

		[ComVisible(true)]
		public string Registrace()
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);


			//vyberu certifikát
			SBUtils.Unit.SetLicenseKey("5E1C594FD406A87D1AD78BD5FCEA6FEEE38E810EB2FBDBC0D2256E9DBAD4D41776F64EF9252CBB192C73CDDB50C9E16E77D0635DC51244BFECC4F1A8539AE70505B7C95FE34A43CCBA155DA8730346C7154F294051EDFBE3F83637A09981F7D82F3F37FFF4AE8C89640094BE8BCCE8CE98966FAC32130DF51ED9AA38EE29EDCEC2F81866424B90656003B0532F65CB2C88A99F4EA7252A57CAF018913C3452963ECE32020D1BF1EFBDE1B16FA6BA0DAF3D0536ABBC24F9C95E4AF8FE300CF2B3F32EC767279CC091ECDC4B7E4A7D7AFAE9164C1117A9FAC8A4307D36C9A1552FD47247B429B6C1AB5E0291B60FAAFEBC2E79A392537A8CD408664CAC20D8F5D3");
			//volám metodu .Sign()

			//1. vybrat dokument

			TElPDFDocument PDFdocument = new TElPDFDocument();
			TElX509Certificate cert = new TElX509Certificate();
									
			//2. vybrat certifikát
			podpis_form f_podpis = new podpis_form();
			f_podpis.ShowDialog();

			if (f_podpis.DialogResult == DialogResult.OK)
			{
				cert = f_podpis.tei_certificate;
				//return "OK";
			}
			else
			{
				LastError = "Nebyl vybrán certifikát k podpisu !";
				return LastError;
			}

            Ret_Signature_Serial = SBUtils.Unit.BinaryToString(cert.SerialNumber);
			Ret_Serial_Number_RDM = GetSerNumByRDM(cert.SubjectRDN);

			Ret_Signature_Valid_From = cert.ValidFrom.ToString("dd.MM.yyyy HH:mm:ss");
			Ret_Signature_Valid_To = cert.ValidTo.ToString("dd.MM.yyyy HH:mm:ss");

			TName cert_info = cert.SubjectName;
            //cert.ValidFrom
            Ret_Signature_Jmeno = cert_info.CommonName;
            
            Ret_RDM = RDNToString(cert.SubjectRDN);

			return "OK";
		}

		[ComVisible(true)]
		public string GetVersion()
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);

			StatusKod = "0000";
			StatusText = "--";

			return String.Format("{0}.{1}.{2}.{3}", fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart);
		}

		//[ComVisible(true)]
		public string GetText()
		{
			return "Hello ActiveX World!";
		}

		[ComVisible(true)]
		public string PDFSave()
		{
			return "";
		}


		[ComVisible(true)]
		public string PDFLoad(byte[] inData)
		{
			return "";
		}

		


		[ComVisible(true)]
		public int MultiSign()
		{

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Kultura);
			System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Kultura);

			SBUtils.Unit.SetLicenseKey("5E1C594FD406A87D1AD78BD5FCEA6FEEE38E810EB2FBDBC0D2256E9DBAD4D41776F64EF9252CBB192C73CDDB50C9E16E77D0635DC51244BFECC4F1A8539AE70505B7C95FE34A43CCBA155DA8730346C7154F294051EDFBE3F83637A09981F7D82F3F37FFF4AE8C89640094BE8BCCE8CE98966FAC32130DF51ED9AA38EE29EDCEC2F81866424B90656003B0532F65CB2C88A99F4EA7252A57CAF018913C3452963ECE32020D1BF1EFBDE1B16FA6BA0DAF3D0536ABBC24F9C95E4AF8FE300CF2B3F32EC767279CC091ECDC4B7E4A7D7AFAE9164C1117A9FAC8A4307D36C9A1552FD47247B429B6C1AB5E0291B60FAAFEBC2E79A392537A8CD408664CAC20D8F5D3");
            //volám metodu .Sign()

            //1. vybrat dokument


            //postup:
            //1. půjdu dokumentu po dokumentu
            //2. a - je-li to soubor, pak jej převedu formátu PDF 
            //2. b - je-li to formulář, pak jej převedu formátu RTF a poté do PDF
            //3. dokument podepíši podpisem
            //4. výsledný dokument zapíšu do DB s danými záznami
            //5. vrátím info o tom, jak to celé dopadlo





            if (_cnnData == "")
			{
				LastError = "Nejsou načtena data cnnData !";
				return 1;
			}

			if (_cnnSoubory == "")
			{
				LastError = "Nejsou načtena data cnnSoubory !";
				return 1;
			}

            if (_multi_file_id == "")
            {
                LastError = "Nejsou načtena data multi_file_id !";
                return 1;
            }




            TElPDFDocument PDFdocument = new TElPDFDocument();
			TElX509Certificate cert = new TElX509Certificate();


			bool Success = false;


			if (_PdfFile == "")
			{
				LastError = "Nejsou načtena base64 data !";
				return 1;
			}

			

			if (_Reason == "")
			{
				//_Reason = "Za správnost vyhotovení";
			}

			if (_Location == "")
			{
				_Location = "ETR";
			}

			//na základě volby v etr mohu volat jednotlivá okna

			//2. vybrat certifikát
			podpis_form f_podpis = new podpis_form();
			f_podpis.ShowDialog();

			if (f_podpis.DialogResult == DialogResult.OK)
			{
				cert = f_podpis.tei_certificate;
				//return "OK";
			}
			else
			{
				LastError = "Nebyl vybrán certifikát k podpisu !";
				return 1;
			}

			//3. zkontrolovat TSA
			if (_AddTimeStamp == "A")
			{

				//vojsko - vyjímka - heslo
				if (_TSProxyURL == "")
				{
					LastError = "Je povoleno přidání časového razítka (TSA), ale není zadána URL adresa serveru pro TSA !";
					return 31;
				}

				if (_TSA_User == "")
				{
					//LastError = "Je povoleno přidání časového razítka (TSA), ale není zadáno uživatelské jméno pro přihlášení k serveru TSA !";
					//return 32;
				}

				if (_TSA_Pass == "")
				{
					//LastError = "Je povoleno přidání časového razítka (TSA), ale není zadáno heslo pro přihlášení k serveru TSA !";
					//return 33;
				}
			}

			TName cert_info = cert.SubjectName;

			Ret_Serial_Number = SBUtils.Unit.BinaryToString(cert.SerialNumber);
			Ret_Serial_Number_RDM = GetSerNumByRDM(cert.SubjectRDN);


			if (_AddTimeStamp == "A")
			{
				httpTspClient.URL = _TSProxyURL;
				httpTspClient.HTTPClient = new TElHTTPSClient();
				httpTspClient.HTTPClient.RequestParameters = new TElHTTPRequestParams();
				httpTspClient.HTTPClient.RequestParameters.Username = _TSA_User;
				httpTspClient.HTTPClient.RequestParameters.Password = _TSA_Pass;
				httpTspClient.HTTPClient.UseHTTPProxy = false;
				httpTspClient.HTTPClient.SSLEnabled = true;

				httpTspClient.HashAlgorithm = SBConstants.Unit.SB_ALGORITHM_DGST_SHA512;

				httpTspClient.HTTPClient.OnCertificateValidate += CertificateValidate;
				//httpTspClient.HTTPClient.o += CertificateValidate;

				//httpTspClient.

			}

            /*_PdfFile = "";

        public string _PdfFile_1 = "";
        public string _PdfFile_2 = "";
        */
            /*try
			{
				pdffile = Convert.FromBase64String(_PdfFile);
			}
			catch (Exception fdsf)
			{
				LastError = "Chyba při sestavení BASE64[] dat (" + fdsf.Message + ") !";
				return 1;
			}*/

            //---------

            List<byte[]> seznam_dokumentu = new List<byte[]>();

            seznam_dokumentu.Add(Convert.FromBase64String(_PdfFile));
            seznam_dokumentu.Add(Convert.FromBase64String(_PdfFile_1));
            seznam_dokumentu.Add(Convert.FromBase64String(_PdfFile_2));



            for (int i = 0; i < 3; i++)
            {
                Stream fileStream = null;

                pdffile = seznam_dokumentu[i];
                try
                {
                    fileStream = new MemoryStream(pdffile.Length + 1024);
                    fileStream.Write(pdffile, 0, pdffile.Length);
                }
                catch (Exception cc)
                {
                    LastError = "Chyba při převodu byte[] na MemoryStream ! [" + cc.Message + "]";
                    return 6;
                }

                //4. podepsat
                TElPDFPublicKeySecurityHandler handler = new TElPDFPublicKeySecurityHandler();
                TElMemoryCertStorage storage = new TElMemoryCertStorage();
                storage.Add(cert, true);

                try
                {
                    PDFdocument.Open(fileStream);

                    int pdf_pocet_stran = 0;
                    int page_width = 0;
                    int page_height = 0;

                    pdf_pocet_stran = PDFdocument.PageInfoCount;
                    int sigIndex = PDFdocument.AddSignature();

                    TElPDFSignature sig = PDFdocument.get_Signatures(sigIndex);

                    sig.Handler = handler;
                    handler.CertStorage = storage;
                    handler.SignatureType = SBPDFSecurity.TSBPDFPublicKeySignatureType.pstPKCS7SHA1;
                    handler.CustomName = "Adobe.PPKMS";

                    sig.SignatureType = SBPDF.Unit.stDocument;
                    sig.AuthorName = cert_info.CommonName;
                    sig.SigningTime = DateTime.UtcNow;
                    //sig.SigningTime = DateTime.Now;


                    if (_AddTimeStamp == "A")
                    {
                        handler.TSPClient = httpTspClient;
                    }

                    sig.Reason = _Reason;

                    if (_InVisible == "N")
                    {
                        int _sirka_ramecku_spodni = 27;
                        int _sirka_ramecku_horni = 27;
                        int _sirka_ramecku_leva = 54;
                        int _sirka_ramecku_prava = 27;

                        //nactu sirku ramecku

                        if (_odsazeni_leve != "")
                        {
                            try
                            {
                                _sirka_ramecku_leva = Convert.ToInt32(_odsazeni_leve);
                            }
                            catch (Exception) { }
                        }

                        if (_odsazeni_prave != "")
                        {
                            try
                            {
                                _sirka_ramecku_prava = Convert.ToInt32(_odsazeni_prave);
                            }
                            catch (Exception) { }
                        }

                        if (_odsazeni_spodni != "")
                        {
                            try
                            {
                                _sirka_ramecku_spodni = Convert.ToInt32(_odsazeni_spodni);
                            }
                            catch (Exception) { }
                        }

                        if (_odsazeni_horni != "")
                        {
                            try
                            {
                                _sirka_ramecku_horni = Convert.ToInt32(_odsazeni_horni);
                            }
                            catch (Exception) { }
                        }

                        sig.WidgetProps.BackgroundStyle = TSBPDFWidgetBackgroundStyle.pbsCustom;
                        sig.WidgetProps.AutoPos = false;
                        sig.WidgetProps.AutoSize = false;

                        int podpis_na_stranu = 0;
                        int podpis_na_stranu_by_user = 0;

                        if (_Sig_Page == "S")
                        {
                            if (_Sig_Page_By_User != "")
                            {
                                try
                                {
                                    podpis_na_stranu_by_user = Convert.ToInt32(_Sig_Page_By_User);
                                }
                                catch (Exception)
                                { throw; }

                            }

                            if (podpis_na_stranu_by_user > PDFdocument.PageInfoCount)
                            {
                                podpis_na_stranu_by_user = 0;
                            }
                        }

                        switch (_Sig_Page)
                        {
                            case "F":
                                sig.Page = 0;
                                podpis_na_stranu = 0;

                                break;
                            case "L":
                                sig.Page = PDFdocument.PageInfoCount;
                                podpis_na_stranu = PDFdocument.PageInfoCount - 1;

                                break;
                            case "S":
                                sig.Page = podpis_na_stranu_by_user - 1;
                                if (podpis_na_stranu_by_user == 0)
                                {
                                    podpis_na_stranu = 0;
                                }
                                else
                                {
                                    podpis_na_stranu = podpis_na_stranu_by_user - 1;
                                }
                                break;
                            default:
                                sig.Page = 0;
                                break;
                        }

                        TElPDFPageInfo info_pdf = PDFdocument.get_PageInfos(podpis_na_stranu);
                        page_width = info_pdf.Width;
                        page_height = info_pdf.Height;

                        switch (_VisibleSide)
                        {
                            case "LH":
                                sig.WidgetProps.OffsetX = _sirka_ramecku_leva;
                                sig.WidgetProps.OffsetY = page_height - 60 - _sirka_ramecku_horni;
                                break;
                            case "LS":
                                sig.WidgetProps.OffsetX = _sirka_ramecku_leva;
                                sig.WidgetProps.OffsetY = _sirka_ramecku_spodni;
                                break;
                            case "PH":
                                sig.WidgetProps.OffsetX = page_width - 150 - _sirka_ramecku_prava;
                                sig.WidgetProps.OffsetY = page_height - 60 - _sirka_ramecku_horni;
                                break;
                            case "PS":
                                sig.WidgetProps.OffsetX = page_width - 150 - _sirka_ramecku_prava;
                                sig.WidgetProps.OffsetY = _sirka_ramecku_spodni;
                                break;
                            default:
                                sig.WidgetProps.OffsetX = _sirka_ramecku_leva;
                                sig.WidgetProps.OffsetY = _sirka_ramecku_spodni;
                                break;
                        }

                        sig.WidgetProps.Width = 150;
                        sig.WidgetProps.Height = 60;

                        sig.WidgetProps.Background.ImageType = TSBPDFImageType.pitJPEG2000;
                        sig.WidgetProps.Background.Height = 197;
                        sig.WidgetProps.Background.Width = 240;
                        sig.WidgetProps.StretchX = 150;
                        sig.WidgetProps.StretchY = 60;
                        sig.WidgetProps.Background.Data = Convert.FromBase64String(obrazek_etr);

                        if (_podklad == "N")
                        {
                            sig.WidgetProps.BackgroundStyle = TSBPDFWidgetBackgroundStyle.pbsNoBackground;
                        }

                        sig.WidgetProps.AutoStretchBackground = false;
                        sig.WidgetProps.SectionTextFontSize = 5;
                        sig.WidgetProps.SectionTitleFontSize = 5;
                        sig.WidgetProps.TitleFontSize = 8;

                        if (_text_do_podpisu != "")
                        {
                            sig.WidgetProps.SignerCaption = "Digitálně podepsal: ";
                        }
                        else
                        {
                            sig.WidgetProps.SignerCaption = "";
                        }

                        sig.WidgetProps.Header = cert_info.CommonName;

                        sig.WidgetProps.AlgorithmCaption = "";
                        sig.WidgetProps.SignerInfo = sestav_dolozku(cert_info, "");
                        //sig.WidgetProps.SignerInfo = "";

                        sig.WidgetProps.AlgorithmInfo = "";
                        sig.WidgetProps.AutoText = false;

                        sig.WidgetProps.AutoFontSize = false;
                        sig.WidgetProps.HideDefaultText = false;
                        sig.WidgetProps.TimestampFontSize = 6;


                        DateTime iKnowThisUTC = sig.SigningTime;
                        DateTime runtime = DateTime.SpecifyKind(iKnowThisUTC, DateTimeKind.Utc);
                        DateTime local = runtime.ToLocalTime();

                        sig.WidgetProps.DateCaptionFormat = local.ToString("dd.MM.yyyy HH:mm:ss");

                        Ret_Sig_DateTime = sig.WidgetProps.DateCaptionFormat;
                    }

                    if (_InVisible == "A")
                    {
                        sig.Invisible = true;
                    }
                    else
                    {
                        sig.Invisible = false;
                    }

                    Ret_jmeno = cert_info.CommonName;
                    Ret_RDM = RDNToString(cert.SubjectRDN);

                    Success = true;

                }
                catch (Exception ex)
                {
                    LastError = "Chyba při podepisování dokumentu ! [" + ex.Message + "]";
                    return 6;
                }
                finally
                {
                    PDFdocument.Close(Success);
                }

                _PdfFile = null;

                if (Success)
                {
                    try
                    {
                        pdffile = ((MemoryStream)fileStream).ToArray();


                        File.WriteAllBytes(@"d:\xxxxx_"+i+".pdf", pdffile);
                        //Process.Start(@"d:\a_132456789.pdf");

                    }
                    catch (Exception ccc1)
                    {
                        LastError = "Chyba při převodu MemoryStream na byte[] ! [" + ccc1.Message + "]";
                        return 7;
                    }
                }


            }
                       

            //---------

            return 0;
		}


        [ComVisible(true)]
        public int Sign()
        {

           
      

        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Kultura);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Kultura);

            SBUtils.Unit.SetLicenseKey("5E1C594FD406A87D1AD78BD5FCEA6FEEE38E810EB2FBDBC0D2256E9DBAD4D41776F64EF9252CBB192C73CDDB50C9E16E77D0635DC51244BFECC4F1A8539AE70505B7C95FE34A43CCBA155DA8730346C7154F294051EDFBE3F83637A09981F7D82F3F37FFF4AE8C89640094BE8BCCE8CE98966FAC32130DF51ED9AA38EE29EDCEC2F81866424B90656003B0532F65CB2C88A99F4EA7252A57CAF018913C3452963ECE32020D1BF1EFBDE1B16FA6BA0DAF3D0536ABBC24F9C95E4AF8FE300CF2B3F32EC767279CC091ECDC4B7E4A7D7AFAE9164C1117A9FAC8A4307D36C9A1552FD47247B429B6C1AB5E0291B60FAAFEBC2E79A392537A8CD408664CAC20D8F5D3");
            //volám metodu .Sign()

            //1. vybrat dokument

            TElPDFDocument PDFdocument = new TElPDFDocument();
            TElX509Certificate cert = new TElX509Certificate();





            bool Success = false;


            if (_PdfFile == "")
            {
                LastError = "Nejsou načtena base64 data !";
                return 1;
            }

            try
            {
                pdffile = Convert.FromBase64String(_PdfFile);
            }
            catch (Exception fdsf)
            {
                LastError = "Chyba při sestavení BASE64[] dat (" + fdsf.Message + ") !";
                return 1;
            }

            if (_Reason == "")
            {
                //_Reason = "Za správnost vyhotovení";
            }

            if (_Location == "")
            {
                _Location = "ETR";
            }

            //na základě volby v etr mohu volat jednotlivá okna


            if ((_DirectSign == "A") && (_CerSerNumber != ""))
            {
                //načtu certifikát přímo z úložiště, pokud se to nepodaří, otevírám okno pro podpis


                X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                X509Certificate2Collection certificateCollection = store.Certificates.Find(X509FindType.FindBySerialNumber, _CerSerNumber, false);

                //toto je v případě, že se nepodaří načíst
                if (certificateCollection.Count == 0)
                {
                    MessageBox.Show("Přednastavený certifikát nebyl v kolekci nalezen, je proto nutné jej vybrat ručně!");

                    podpis_form f_podpis = new podpis_form();
                    f_podpis.ShowDialog();

                    if (f_podpis.DialogResult == DialogResult.OK)
                    {
                        cert = f_podpis.tei_certificate;
                        //return "OK";

                       
                    }
                    else
                    {
                        LastError = "Nebyl vybrán certifikát k podpisu !";
                        return 1;
                    }
                }
                else
                {
                    X509Certificate2 ext_certificate;
                    ext_certificate = certificateCollection[0];

                    try
                    {
                        cert.FromX509Certificate2(ext_certificate);
                    }
                    catch (Exception ex)
                    {
                        LastError = "Chyba při načtení certifikátu ! [" + ex.Message + "]";
                        return 1;
                    }


                    if (cert.ValidTo > DateTime.Now)
                    {
                        MessageBox.Show("Přednastavený certifikát je již po platnosti, vyberte popřípadě proveďte registraci nového certifikátu!");

                        podpis_form f_podpis = new podpis_form();
                        f_podpis.ShowDialog();

                        if (f_podpis.DialogResult == DialogResult.OK)
                        {
                            cert = f_podpis.tei_certificate;
                            //return "OK";


                        }
                        else
                        {
                            LastError = "Nebyl vybrán certifikát k podpisu !";
                            return 1;
                        }
                    }


                }








            }
            else
            {
                podpis_form f_podpis = new podpis_form();
                f_podpis.ShowDialog();

                if (f_podpis.DialogResult == DialogResult.OK)
                {
                    cert = f_podpis.tei_certificate;
                    //return "OK";
                }
                else
                {
                    LastError = "Nebyl vybrán certifikát k podpisu !";
                    return 1;
                }
            }

                /*

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


                 */





                //2. vybrat certifikát
            

			//3. zkontrolovat TSA
			if (_AddTimeStamp == "A")
			{

				//vojsko - vyjímka - heslo
				if (_TSProxyURL == "")
				{
					LastError = "Je povoleno přidání časového razítka (TSA), ale není zadána URL adresa serveru pro TSA !";
					return 31;
				}

				if (_TSA_User == "")
				{
					//LastError = "Je povoleno přidání časového razítka (TSA), ale není zadáno uživatelské jméno pro přihlášení k serveru TSA !";
					//return 32;
				}

				if (_TSA_Pass == "")
				{
					//LastError = "Je povoleno přidání časového razítka (TSA), ale není zadáno heslo pro přihlášení k serveru TSA !";
					//return 33;
				}
			}

			TName cert_info = cert.SubjectName;

			Ret_Serial_Number = SBUtils.Unit.BinaryToString(cert.SerialNumber);
			Ret_Serial_Number_RDM = GetSerNumByRDM(cert.SubjectRDN);


			if (_AddTimeStamp == "A")
			{
				httpTspClient.URL = _TSProxyURL;
				httpTspClient.HTTPClient = new TElHTTPSClient();
				httpTspClient.HTTPClient.RequestParameters = new TElHTTPRequestParams();
				httpTspClient.HTTPClient.RequestParameters.Username = _TSA_User;
				httpTspClient.HTTPClient.RequestParameters.Password = _TSA_Pass;

				

                if (_TSA_proxy_use == "A")
                {
                    //proxy on                        
                    httpTspClient.HTTPClient.UseHTTPProxy = true;
                    httpTspClient.HTTPClient.HTTPProxyPort = Convert.ToInt32(_TSA_proxy_port);
                    httpTspClient.HTTPClient.HTTPProxyHost = _TSA_proxy_host;
                }
                else
                {
                    httpTspClient.HTTPClient.UseHTTPProxy = false;

                }



                httpTspClient.HTTPClient.SSLEnabled = true;

				httpTspClient.HashAlgorithm = SBConstants.Unit.SB_ALGORITHM_DGST_SHA512;

				httpTspClient.HTTPClient.OnCertificateValidate += CertificateValidate;
				//httpTspClient.HTTPClient.o += CertificateValidate;

				//httpTspClient.

			}

			Stream fileStream = null;

			try
			{
				fileStream = new MemoryStream(pdffile.Length + 1024);
				fileStream.Write(pdffile, 0, pdffile.Length);
			}
			catch (Exception cc)
			{
				LastError = "Chyba při převodu byte[] na MemoryStream ! [" + cc.Message + "]";
				return 6;
			}


            

            //4. podepsat
            TElPDFPublicKeySecurityHandler handler = new TElPDFPublicKeySecurityHandler();
			TElMemoryCertStorage storage = new TElMemoryCertStorage();
			storage.Add(cert, true);

			try
			{
				PDFdocument.Open(fileStream);

				int pdf_pocet_stran = 0;
				int page_width = 0;
				int page_height = 0;

				pdf_pocet_stran = PDFdocument.PageInfoCount;
				int sigIndex = PDFdocument.AddSignature();

				TElPDFSignature sig = PDFdocument.get_Signatures(sigIndex);

				sig.Handler = handler;
				handler.CertStorage = storage;
				handler.SignatureType = SBPDFSecurity.TSBPDFPublicKeySignatureType.pstPKCS7SHA1;
				handler.CustomName = "Adobe.PPKMS";

				sig.SignatureType = SBPDF.Unit.stDocument;
				sig.AuthorName = cert_info.CommonName;
				sig.SigningTime = DateTime.UtcNow;
				//sig.SigningTime = DateTime.Now;


				if (_AddTimeStamp == "A")
				{
					handler.TSPClient = httpTspClient;
				}

				sig.Reason = _Reason;

				if (_InVisible == "N")
				{
					int _sirka_ramecku_spodni = 27;
					int _sirka_ramecku_horni = 27;
					int _sirka_ramecku_leva = 54;
					int _sirka_ramecku_prava = 27;

					//nactu sirku ramecku

					if (_odsazeni_leve != "")
					{
						try
						{
							_sirka_ramecku_leva = Convert.ToInt32(_odsazeni_leve);
						}
						catch (Exception) { }
					}

					if (_odsazeni_prave != "")
					{
						try
						{
							_sirka_ramecku_prava = Convert.ToInt32(_odsazeni_prave);
						}
						catch (Exception) { }
					}

					if (_odsazeni_spodni != "")
					{
						try
						{
							_sirka_ramecku_spodni = Convert.ToInt32(_odsazeni_spodni);
						}
						catch (Exception) { }
					}

					if (_odsazeni_horni != "")
					{
						try
						{
							_sirka_ramecku_horni = Convert.ToInt32(_odsazeni_horni);
						}
						catch (Exception) { }
					}

					sig.WidgetProps.BackgroundStyle = TSBPDFWidgetBackgroundStyle.pbsCustom;
					sig.WidgetProps.AutoPos = false;
					sig.WidgetProps.AutoSize = false;

					int podpis_na_stranu = 0;
					int podpis_na_stranu_by_user = 0;

					if (_Sig_Page == "S")
					{
						if (_Sig_Page_By_User != "")
						{
							try
							{
								podpis_na_stranu_by_user = Convert.ToInt32(_Sig_Page_By_User);
							}
							catch (Exception)
							{ throw; }

						}

						if (podpis_na_stranu_by_user > PDFdocument.PageInfoCount)
						{
							podpis_na_stranu_by_user = 0;
						}
					}

					switch (_Sig_Page)
					{
						case "F":
							sig.Page = 0;
							podpis_na_stranu = 0;

							break;
						case "L":
							sig.Page = PDFdocument.PageInfoCount;
							podpis_na_stranu = PDFdocument.PageInfoCount - 1;

							break;
						case "S":
							sig.Page = podpis_na_stranu_by_user-1;
							if (podpis_na_stranu_by_user == 0)
							{
								podpis_na_stranu = 0;
							}
							else
							{
								podpis_na_stranu = podpis_na_stranu_by_user - 1;
							}
							break;
						default:
							sig.Page = 0;
							break;
					}

					TElPDFPageInfo info_pdf = PDFdocument.get_PageInfos(podpis_na_stranu);
					page_width = info_pdf.Width;
					page_height = info_pdf.Height;

					switch (_VisibleSide)
					{
						case "LH":
							sig.WidgetProps.OffsetX = _sirka_ramecku_leva;
							sig.WidgetProps.OffsetY = page_height - 60 - _sirka_ramecku_horni;
							break;
						case "LS":
							sig.WidgetProps.OffsetX = _sirka_ramecku_leva;
							sig.WidgetProps.OffsetY = _sirka_ramecku_spodni;
							break;
						case "PH":
							sig.WidgetProps.OffsetX = page_width - 150 - _sirka_ramecku_prava;
							sig.WidgetProps.OffsetY = page_height - 60 - _sirka_ramecku_horni;
							break;
						case "PS":
							sig.WidgetProps.OffsetX = page_width - 150 - _sirka_ramecku_prava;
							sig.WidgetProps.OffsetY = _sirka_ramecku_spodni;
							break;
						default:
							sig.WidgetProps.OffsetX = _sirka_ramecku_leva;
							sig.WidgetProps.OffsetY = _sirka_ramecku_spodni;
							break;
					}

					sig.WidgetProps.Width = 150;
					sig.WidgetProps.Height = 60;

					sig.WidgetProps.Background.ImageType = TSBPDFImageType.pitJPEG2000;
					sig.WidgetProps.Background.Height = 197;
					sig.WidgetProps.Background.Width = 240;
					sig.WidgetProps.StretchX = 150;
					sig.WidgetProps.StretchY = 60;
					sig.WidgetProps.Background.Data = Convert.FromBase64String(obrazek_etr);

					if (_podklad == "N")
					{
						sig.WidgetProps.BackgroundStyle = TSBPDFWidgetBackgroundStyle.pbsNoBackground;
					}

					sig.WidgetProps.AutoStretchBackground = false;
					sig.WidgetProps.SectionTextFontSize = 5;
					sig.WidgetProps.SectionTitleFontSize = 5;
					sig.WidgetProps.TitleFontSize = 8;

					if (_text_do_podpisu != "")
					{
						sig.WidgetProps.SignerCaption = "Digitálně podepsal: ";
					}
					else {
						sig.WidgetProps.SignerCaption = "";
					}

					sig.WidgetProps.Header = cert_info.CommonName;

					sig.WidgetProps.AlgorithmCaption = "";
					sig.WidgetProps.SignerInfo = sestav_dolozku(cert_info, "");
					//sig.WidgetProps.SignerInfo = "";

					sig.WidgetProps.AlgorithmInfo = "";
					sig.WidgetProps.AutoText = false;

					sig.WidgetProps.AutoFontSize = false;
					sig.WidgetProps.HideDefaultText = false;
					sig.WidgetProps.TimestampFontSize = 6;

					
					DateTime iKnowThisUTC = sig.SigningTime;
					DateTime runtime = DateTime.SpecifyKind(iKnowThisUTC, DateTimeKind.Utc);
					DateTime local = runtime.ToLocalTime();

					sig.WidgetProps.DateCaptionFormat = local.ToString("dd.MM.yyyy HH:mm:ss");

					Ret_Sig_DateTime = sig.WidgetProps.DateCaptionFormat;
				}

				if (_InVisible == "A")
				{
					sig.Invisible = true;
				}
				else
				{
					sig.Invisible = false;
				}

				Ret_jmeno = cert_info.CommonName;
				Ret_RDM = RDNToString(cert.SubjectRDN);

				Success = true;

			}
			catch (Exception ex)
			{
				LastError = "Chyba při podepisování dokumentu ! [" + ex.Message + "]";
				return 6;
			}
			finally
			{
				PDFdocument.Close(Success);
			}

			_PdfFile = null;

			if (Success)
			{
				try
				{
					pdffile = ((MemoryStream)fileStream).ToArray();
					_PdfFile = Convert.ToBase64String(pdffile);
				}
				catch (Exception ccc1)
				{
					LastError = "Chyba při převodu MemoryStream na byte[] ! [" + ccc1.Message + "]";
					return 7;
				}
			}
			return 0;
		}

		private string GetSerNumByRDM(TElRelativeDistinguishedName Rdn)
		{
			string mlist = "";

			for (int i = 0; i < Rdn.Count; i++)
			{
				if (SBStrUtils.__Global.OIDToStr(Rdn.get_OIDs(i)) == "2.5.4.5")
				{
					mlist = RDNValueToString(Rdn.get_Tags(i), Rdn.get_Values(i));
					return mlist;
				}

			}

			return mlist;
		}

		private void muj_cert()
		{
			X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
			store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

			X509Certificate2Collection certificateCollection = store.Certificates.Find(X509FindType.FindBySerialNumber, "1EFD23", false);

			if (certificateCollection.Count == 0)
			{
				MessageBox.Show("nenalezen");
				return;
			}
		}

		private void CertificateValidate(object Sender, TElX509Certificate X509Certificate, ref bool Validate)
		{
			Validate = true;
		}


		private List<string> RDNTolistString(TElRelativeDistinguishedName Rdn)
		{
			List<string> mlist = new List<string>();

			for (int i = 0; i < Rdn.Count; i++)
			{
				if (SBStrUtils.__Global.OIDToStr(Rdn.get_OIDs(i)) == "2.5.4.11")
				{
					mlist.Add(RDNValueToString(Rdn.get_Tags(i), Rdn.get_Values(i)));
				}

			}
			return mlist;
		}

		private string RDNToString(TElRelativeDistinguishedName Rdn)
		{
			string Result = "";
			for (int i = 0; i < Rdn.Count; i++)
				Result = Result + OID_to_NormString(SBStrUtils.__Global.OIDToStr(Rdn.get_OIDs(i))) + "=" + RDNValueToString(Rdn.get_Tags(i), Rdn.get_Values(i)) + Environment.NewLine;
			if (Rdn.Count > 0)
				Result = Result.Substring(0, Result.Length - 2);
			return Result;
		}

		private string OID_to_NormString(string oid_in)
		{
			string xx_ret = "";
			switch (oid_in)
			{
				case "2.5.4.3":
					xx_ret = "cn";
					break;
				case "2.5.4.4":
					xx_ret = "sn";
					break;
				case "2.5.4.42":
					xx_ret = "givenMane";
					break;
				case "2.5.4.5":
					xx_ret = "serialNumber";
					break;
				case "2.5.4.6":
					xx_ret = "c";
					break;
				case "2.5.4.10":
					xx_ret = "o";
					break;
				case "2.5.4.11":
					xx_ret = "ou";
					break;
				case "2.5.4.12":
					xx_ret = "title";
					break;
				default:
					xx_ret = oid_in;
					break;
			}

			return xx_ret;
		}

		private string RDNValueToString(byte tag, byte[] value)
		{
			switch (tag)
			{
				case SBASN1Tree.Unit.SB_ASN1_NUMERICSTR:
				case SBASN1Tree.Unit.SB_ASN1_PRINTABLESTRING:
				case SBASN1Tree.Unit.SB_ASN1_IA5STRING:
				case SBASN1Tree.Unit.SB_ASN1_VISIBLESTRING:
					{
						return SBUtils.Unit.StringOfBytes(value);
					}
				case SBASN1Tree.Unit.SB_ASN1_UTF8STRING:
					{
						return System.Text.Encoding.UTF8.GetString(value);
					}
				case SBASN1Tree.Unit.SB_ASN1_BMPSTRING:
					{
						if ((value.Length >= 2) && (value[0] == 254) && (value[1] == 255))
							return System.Text.Encoding.BigEndianUnicode.GetString(value, 2, value.Length - 2);
						else
							if ((value.Length >= 2) && (value[0] == 255) && (value[1] == 254))
							return System.Text.Encoding.Unicode.GetString(value, 2, value.Length - 2);
						else
							return System.Text.Encoding.BigEndianUnicode.GetString(value, 0, value.Length);
					}
				case SBASN1Tree.Unit.SB_ASN1_UNIVERSALSTRING:
					{
						if ((value.Length < 4) || (value[0] != 255) || (value[1] != 254) || (value[2] != 0) || (value[3] != 0))
							SBUtils.Unit.SwapBigEndianDWords(ref value);

						string Result = System.Text.Encoding.GetEncoding("utf-32").GetString(value, 0, value.Length);
						if ((Result.Length > 0) && (Result[0] == (Char)(0xFEFF)))
							Result = Result.Remove(0, 1);
						return Result;
					}
				default:
					{
						return SBUtils.Unit.StringOfBytes(value);
					}
			}
		}

		private string sestav_dolozku(TName x_cert_info, string SerNum)
		{
			string x_out = "";



			if (_text_do_podpisu != "")
			{
				if (x_cert_info.CommonName != "")
				{
					x_out = dopln_text(x_out, x_cert_info.CommonName);
				}

				x_out = dopln_text(x_out, _text_do_podpisu);

			}

			if (_text_do_podpisu_doplneni != "")
			{
				x_out = dopln_text(x_out, _text_do_podpisu_doplneni);
			}

			/*if (x_cert_info.Country != "")
			{
				x_out = dopln_text(x_out, "c=" + x_cert_info.Country);
			}

			if (x_cert_info.Organization != "")
			{
				x_out = dopln_text(x_out, x_cert_info.Organization);
				//x_out = dopln_text(x_out, SpliceText("Česká republika - Krajské ředitelství policie Libereckého kraje se sídlem v Liberci [IČ: 72050501]", 65));

			}
			*/
			if (_Reason != "") { 
				if (x_out == "")
				{
					x_out = dopln_text(x_out, "Důvod: " + _Reason);
				}
				else
				{
					x_out = dopln_text(x_out, Environment.NewLine + "Důvod: " + _Reason);
				}
			}

			//x_out = dopln_text(x_out, "serialNumber: " + SerNum);





			//if (x_cert_info.OrganizationUnit != "")
			//{
			//x_out = dopln_text(x_out, "ou=" + x_cert_info.OrganizationUnit);
			//}


			return x_out;
		}

		public static string SpliceText(string text, int lineLength)
		{
			var charCount = 0;
			var lines = text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)
							.GroupBy(w => (charCount += w.Length + 1) / lineLength)
							.Select(g => string.Join(" ", g));

			return String.Join(Environment.NewLine, lines.ToArray());
		}

		private string sestav_dolozku_RDN(List<string> rdn_to_use)
		{
			string x_out = "";
			string x_oec = "";
			if (rdn_to_use.Count > 0)
			{
				foreach (string x_item in rdn_to_use)
				{
					try
					{
						Convert.ToInt32(x_item);
						x_oec = x_item;
					}
					catch (Exception)
					{
						x_out = x_item;
					}
				}

			}
			else
			{
				return x_out;
			}

			return x_out;
		}

		private string dopln_text(string puvodni, string append)
		{
			if (puvodni == "")
			{
				puvodni = append;
			}
			else
			{
				puvodni = puvodni + Environment.NewLine + append;
			}

			return puvodni;

		}


	}
}
