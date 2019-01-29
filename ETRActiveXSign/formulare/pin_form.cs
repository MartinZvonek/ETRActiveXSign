using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ETRActiveXSign.formulare
{
    public partial class pin_form : Form
    {

        public string pin_token = "";
        private string obrazek_str = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAD8ElEQVR4Xu1XXWibVRh+zs/3fUlqTJPQrllHGtPqOrrO2rVTJ8sozisF8UYUxIsJ0ykom4LMG70qCMOB80YmCKJCHSL+4E3R7nZDpjdz6pBusJ/KbDPXkjVNk+N5Tgirade6rdku9IWHHN73eZ/3OT98EGGMwe0MafHfNoAbvAL5/RuDzx19c9AQXDN3y07gxR3pZLQp+GDLC7tBcM3crTLgPXV/6+ft/ZuB8985cM0ca402ID7ctfGBpubYtlTfBmD8CHD6CFIbM2CONXIaacDvbouMdj2cA37/CqiUgbLFmVEwxxo5jTIgv3m1f29sbSq4M2GAiR8BHRBuzRxr5JDbCAOhRFgNdz86BJz6GvACC01w7XKskUPuahtQY/sGPs1svQ+icAqYzQO+D4RlFYEPFPOuRg657FktA+Kj3Zu2xuLRx1ObssD4WPXYfYmH3hnCtgND+PWPOKB94PQYyCGXPexdDQN+ZzIYyTzYC/x5HC60dgZiYYHmiEBg17wKBjnksoe9N2tAfvva5r3xdS2p+F1R4OLPgOdXDSgFz9MWCtASUJqn4Djksoe91LgZA6F4SA13br8XOHcMUD4gtYVy8JR2EKqW0+Q4LnvYS40bNaBGXx84uLYvCz+4DMxOAdrjTi2UhXQnoDUNKMBBk0Ou62EvNah1vQbEgWe674ndEexMb+kALp4AZGDhWega3HDPGbiaI4dc9rCXGtSi5vUY8AY6ooe7cuuBwlmgPF+7939A2ZzyNERdnlz2sJca1KLmvzUgv9zT/2yyPdETv7sZmL5Qd/RXoZUHZQFZy9Vdhe2lBrWoudQ8jcURtDZ5hzq3Z4H8OCCERd0VCgPAoCIC91sWfpUj6/UrToNaf52/dAjAJxZXljPAh/dea3cL/GgZmJzhbhZfn4DLvfXEceevvXmWPJpdfMBzM/CTZVCT2o+8/cMuAOUlDPDhbXAPL5tbB0yfA6R/7Y+ZVEinZ6rlkgDmPSwdxl0FNSfH8zvtjP17Pj75Cwv1BryBTPRwZrDNillhI9yulgwODSrIvZSCFAbvv5LH+sw8UL6GWWOcJrWLxRIfZL/F3EID4rOX+x6LJSM9Lb1xYGaKw5f/eiiBtjWtzoAfugIoi+U+/aUCWnoTmDg52cNZT7770xe0VpsSpOPhka7cGkeEVBZYIST2P3+h+gYSFUDq2vxlTXBG4VJxBEDUYrZmICIAHe4IAXMVQEusqCYM0tkSGG4fFb1CjwGkQLjDI4vkyEIDarpUPnH04G89pmLIbUwIQEiBOWPOcubCN1DcMXzsaQBJV2hsVCym6h9hweKMxYTz2dgwHM6ZCw3MW1zGbYj//x3/DY0aIaoCq/CdAAAAAElFTkSuQmCC";
        private byte[] obrazek_base64 = null;
        private MemoryStream obrazek_stream = null;

        public pin_form()
        {
            InitializeComponent();
            Application.Idle += Application_Idle;

            obrazek_base64 = Convert.FromBase64String(obrazek_str);
            obrazek_stream = new MemoryStream(obrazek_base64);
            obrazek_str = "";
            obrazek_base64 = null;

            pictureBox1.Image = new Bitmap(obrazek_stream);
        }

        void Application_Idle(object sender, EventArgs e)
        {
            if (Control.IsKeyLocked(Keys.CapsLock))
            {
                pictureBox1.Visible = true;
                label2.Visible = true;
                this.button3.Location = new System.Drawing.Point(106, 98);
                this.button4.Location = new System.Drawing.Point(210, 98);
                this.Size = new System.Drawing.Size(418, 175);
            }
            else
            {
                pictureBox1.Visible = false;
                label2.Visible = false;
                this.button3.Location = new System.Drawing.Point(106, 66);
                this.button4.Location = new System.Drawing.Point(210, 66);
                this.Size = new System.Drawing.Size(418, 143);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Application.Idle -= Application_Idle;
            base.OnFormClosed(e);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Musíte zadat heslo !");
                textBox1.Focus();
                return;
            }

            pin_token = textBox1.Text;
            DialogResult = DialogResult.OK;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void pin_form_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
    }
}
