using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace JPubGfx
{
    public partial class Config : Form
    {

        /// <summary>
        /// Classe per la gestione dell'interfacia grafica  per l'inserimento dei dati di congfigurazione dell'applicativo.
        /// </summary>
        /// 

        string sService = "";
        string sUsername = "";
        string sPassword = "";
        string sIpaddress = "";
        string sPort = "";
        string sTipo = "";

        /// <summary>
        /// Costruttore della classe
        /// </summary>
        public Config()
        {
            InitializeComponent();
            JpubOraDb jdb = new JpubOraDb();
            jdb.ReadConfig();
            sService = jdb.getService();
            sUsername = jdb.getUsername();
            sPassword = jdb.getPassword();
            sIpaddress = jdb.getIpAddress();
            sPort = jdb.getPort();
            sTipo = jdb.getTipoService();
            jdb = null;

            //textBox1.Text = sService;
            textBox2.Text = sUsername;
            textBox3.Text = sPassword;
            textBox4.Text = sIpaddress;
            textBox5.Text = sPort;
            textBox6.Enabled = true;
            textBox1.Enabled = false;

            if (sTipo.CompareTo("SID") == 0)
            {
                radioButton1.Checked = true;
                textBox6.Text = sService;
                textBox6.Enabled = true;
            }

            if (sTipo.CompareTo("SERVICE_NAME") == 0)
            {
                radioButton2.Checked = true;
                textBox1.Text = sService;
                textBox1.Enabled = true;
            }

        }




        /// <summary>
        /// Questo metodo esegue il controllo dei campi obbligatori della form 
        /// </summary>
        /// <returns>Boolean</returns>
        private Boolean checkFields()
        {
            Boolean bRc = false;

            if (radioButton1.Checked == true)
            {
                if (textBox6.TextLength <= 0)
                {
                    MessageBox.Show(this, "Nessun SID inserito!", "Configurazione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    textBox6.Focus();
                    return (bRc);
                }
            }


            if (radioButton2.Checked == true)
            {
                if (textBox1.TextLength <= 0)
                {
                    MessageBox.Show(this, "Nessun SERVIZIO inserito!", "Configurazione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    textBox1.Focus();
                    return (bRc);
                }
            }


            if (textBox4.TextLength <= 0)
            {
                MessageBox.Show(this, "Nessuna Indirizzo IP inserito!", "Configurazione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox4.Focus();
                return (bRc);
            }

            if (textBox4.Text.IndexOf(".") <0 )
            {
                MessageBox.Show(this, "Indirizzo IP non valido!", "Configurazione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox4.Focus();
                return (bRc);
            }


            string[] words = textBox4.Text.Split('.');
            if (words.Length != 4)
            {
                MessageBox.Show(this, "Indirizzo IP non valido!", "Configurazione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox4.Focus();
                return (bRc);
            }
            


            if (textBox5.TextLength <= 0)
            {
                MessageBox.Show(this, "Nessuna Porta per la connessione inserita!", "Configurazione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox5.Focus();
                return (bRc);
            }




            if (textBox2.TextLength <= 0)
            {
                MessageBox.Show(this, "Nessuna username inserita!", "Configurazione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox2.Focus();
                return (bRc);
            }

            if (textBox3.TextLength <= 0)
            {
                MessageBox.Show(this, "Nessuna password inserita!", "Configurazione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox3.Focus();
                return (bRc);
            }


            bRc = true;
            return (bRc);
        }

        /// <summary>
        /// Questo metodo esegue il test di connessione al databse
        /// </summary>
        private void btnTest_Click(object sender, EventArgs e)
        {
            if (checkFields() == false)
            { return; }

            String sService = "";
            String sServiceType = "";
            if (radioButton1.Checked == true)
            {
                sService = textBox6.Text;
                sServiceType = "SID";
            }
            if (radioButton2.Checked == true)
            {
                sService = textBox1.Text;
                sServiceType = "SERVICE_NAME";
            }
            JpubOraDb jdb = new JpubOraDb();
            if (jdb.Connect(sService,textBox4.Text, textBox5.Text, textBox2.Text, textBox3.Text, sServiceType) == true)
            {
                jdb.Close();
                MessageBox.Show(this, "Connessione effettuata con successo!", "Test connessione", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(this, "Errore di connessione al database!", "Test connessione", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        /// <summary>
        /// Questo metodo provvede al salvataggio dei dati di configurazione nel file jpconfig.xml
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (checkFields() == false)
            { return; }


            JpubOraDb jdb = new JpubOraDb();
            String sService = "";
            String sServiceType = "";
            if (radioButton1.Checked == true)
            {
                sService = textBox6.Text;
                sServiceType = "SID";
            }
            if (radioButton2.Checked == true)
            {
                sService = textBox1.Text;
                sServiceType = "SERVICE_NAME";
            }   

            //if (jdb.Connect(textBox1.Text, textBox4.Text, textBox5.Text, textBox2.Text, textBox3.Text,sServiceType) == true)
            //{

                jdb.WriteDbConfig(sService, textBox4.Text, textBox5.Text, textBox2.Text, textBox3.Text, sServiceType);
                //jdb.Close();
                MessageBox.Show(this, "Configurazione salvata con successo!", "Test connessione", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            //}
            //else
            //{
            //   MessageBox.Show(this, "Errore di connessione al database!", "Test connessione", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

        }

        /// <summary>
        /// Questo metodo esce dalla form senza eseguire alcuna modifica
        /// </summary>
        private void btnAnnulla_Click(object sender, EventArgs e)
        {
            if (sService != "" && sUsername != "" && sPassword != "")
            {
                this.Close();
            }
            else
            {
                MessageBox.Show(this, "Nessuna configurazione inserita!", "Configurazione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Application.Exit();
            }

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
                textBox6.Enabled = true;
                textBox1.Text = "";
                textBox1.Enabled = false; 


        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox6.Enabled = false;
            textBox6.Text = "";
            textBox1.Enabled = true; 

        }
    }
}

