using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OracleClient;
using System.IO;



namespace JPubGfx
{
    public partial class frmExportCSV : Form
    {

        private String csvSeparator = "#";

        public frmExportCSV()
        {
            InitializeComponent();
            tabControl2.TabPages.Remove(tabPage6);
            tabControl2.TabPages.Remove(tabPage7);


        }

        private void btnChiudiCsv_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnEsportaCSV_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            int nChecked = 0;
            int nErrors = 0;
            Boolean bRc = false;
            String strFilename = "";

            if (checkBox1.Checked == false && checkBox2.Checked == false && checkBox3.Checked == false && checkBox4.Checked == false && checkBox5.Checked == false)
            {
                MessageBox.Show("Nessun file da esportare selezionato!", "Esporta file CSV", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                folderBrowserDialog1.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    Console.WriteLine("OUTPUTH FOLDER:" + folderBrowserDialog1.SelectedPath);
                }
                else
                    return;

                foreach (Control c in groupBox1.Controls)
                {
                    if ((c is CheckBox) && ((CheckBox)c).Checked)
                    {
                        if (c.Name.CompareTo("checkBox1") == 0)
                        {
                            strFilename = "job_catalog.csv";
                        }
                        else if (c.Name.CompareTo("checkBox2") == 0)
                        {
                            strFilename = "condition_catalog.csv";
                        }
                        else if (c.Name.CompareTo("checkBox3") == 0)
                        {
                            strFilename = "operation_catalog.csv";
                        }
                        else if (c.Name.CompareTo("checkBox4") == 0)
                        {
                            strFilename = "job_config.csv";
                        }
                        else if (c.Name.CompareTo("checkBox5") == 0)
                        {
                            strFilename = "ticket_config.csv";
                        }


                        bRc = exportFileCSV(strFilename, folderBrowserDialog1.SelectedPath);
                        if (bRc == false)
                            nErrors++;

                        nChecked++;
                    }

                }

                if (nChecked > 0)
                {
                    if (nErrors > 0)
                        MessageBox.Show("Nr." + nErrors + " errori durante l\'esportazione!! ", "Esporta file CSV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show("Esportazione terminata con successo!", "Esporta file CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                textBox1.Text = "";

                Console.WriteLine("Selezionati:" + nChecked);




            }//else
        }


        /// <summary>
        /// Questo metodo esporta in formato CSV con separatore definito nella variabile csvSeparator i dati di una tabella dato il nome del file 
        /// </summary>
        /// <param name="strFilename">Nome File CSV</param>
        /// <param name="strPath">Path</param>
        /// <return>Boolean</return>
        private Boolean exportFileCSV(String strFilename, String strPath)
        {
            Boolean bRc = false;
            String strSql = "";
            int sqlRc = 0;
            String strOut = "";

            List<string> lstRows = new List<string>();


            textBox1.Text = "Export " + strFilename + " ...";
            try
            {
                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {

                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleDataReader reader;
                    OracleDataReader reader2;
                    OracleDataReader reader3;
                    OracleCommand cmd = new OracleCommand();

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = cn;

                    if (strFilename.CompareTo("job_catalog.csv") == 0)
                    {
                        strOut = "ID_JOB#DESCRIZIONE#ACTIVE#JOB_TYPE#CODE_TIPO_CONTR#MASTERTABLE";
                        lstRows.Add(strOut);

                        strSql = "select id_job,descrizione,active,job_type,nvl(code_tipo_contr,'ALL'),mastertable from wp1_job order by id_job";
                        cmd.CommandText = strSql;
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            strOut = reader[0].ToString() + csvSeparator + reader[1].ToString() + csvSeparator + reader[2].ToString() + csvSeparator + reader[3].ToString() + csvSeparator + reader[4].ToString() + csvSeparator + reader[5].ToString();
                            lstRows.Add(strOut);
                        }
                        reader.Close();

                    }
                    else if (strFilename.CompareTo("condition_catalog.csv") == 0)
                    {
                        strOut = "ID_CONDITION#FIELD#OPERATOR#OPERANDS#FIELD_TYPE#COND_TYPE";
                        lstRows.Add(strOut);

                        strSql = "select id_condition,field,operator,operands,field_type,cond_type from wp1_conditions order by id_condition";
                        cmd.CommandText = strSql;
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            strOut = reader[0].ToString() + csvSeparator + reader[1].ToString() + csvSeparator + reader[2].ToString() + csvSeparator + reader[3].ToString() + csvSeparator + reader[4].ToString() + csvSeparator + reader[5].ToString();
                            lstRows.Add(strOut);
                        }
                        reader.Close();

                    }
                    else if (strFilename.CompareTo("operation_catalog.csv") == 0)
                    {
                        strOut = "ID_OPERATION#FIELD#OPERATOR#OPERAND#OP_TYPE";
                        lstRows.Add(strOut);

                        strSql = "select id_operation,field,operator,operand,op_type from wp1_operations order by id_operation";
                        cmd.CommandText = strSql;
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            strOut = reader[0].ToString() + csvSeparator + reader[1].ToString() + csvSeparator + reader[2].ToString() + csvSeparator + reader[3].ToString() + csvSeparator + reader[4].ToString();
                            lstRows.Add(strOut);
                        }
                        reader.Close();
                    }
                    else if (strFilename.CompareTo("job_config.csv") == 0)
                    {


                        strSql = "select id_job from wp1_job order by id_job";
                        cmd.CommandText = strSql;
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            strOut = "JOB_ROW" + csvSeparator + reader[0].ToString() + csvSeparator + reader[0].ToString();
                            lstRows.Add(strOut);

                            OracleParameter id_job = new OracleParameter("id_job", reader[0].ToString());

                            strSql = "select id_condition,optional from wp1_job_condition where id_job = :id_job ";
                            cmd.CommandText = strSql;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(id_job);
                            reader2 = cmd.ExecuteReader();

                            while (reader2.Read())
                            {
                                strOut = "CONDITION_ROW" + csvSeparator + reader2[0].ToString() + csvSeparator + reader2[1].ToString();
                                lstRows.Add(strOut);
                            }
                            reader2.Close();

                            strSql = "select id_operation from wp1_job_operation where id_job = :id_job ";
                            cmd.CommandText = strSql;
                            reader2 = cmd.ExecuteReader();
                            while (reader2.Read())
                            {
                                strOut = "OPERATION_ROW" + csvSeparator + reader2[0].ToString();
                                lstRows.Add(strOut);
                            }
                            reader2.Close();

                        }
                        reader.Close();


                    }
                    else if (strFilename.CompareTo("ticket_config.csv") == 0)
                    {
                        strSql = "select distinct id_ticket from wp1_instance_exec order by id_ticket";

                        cmd.CommandText = strSql;
                        reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {

                            OracleParameter id_ticket = new OracleParameter("id_ticket", reader[0].ToString());

                            strSql = "select id_ticket, to_char(data_emissione,'yyyymmdd'),to_char(data_inizio_val,'yyyymmdd'),to_char(data_fine_val,'yyyymmdd'),user_nws,user_ga,system,descrizione,ordinamento,active,ticket_type, nvl(code_tipo_contr,'null')  from wp1_ticketga where id_ticket = :id_ticket ";
                            cmd.CommandText = strSql;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(id_ticket);
                            reader2 = cmd.ExecuteReader();
                            while (reader2.Read())
                            {
                                strOut = "TICKET_ROW" + csvSeparator;
                                strOut += reader2[0].ToString() + csvSeparator;
                                strOut += reader2[1].ToString() + csvSeparator;
                                strOut += reader2[2].ToString() + csvSeparator;
                                strOut += reader2[3].ToString() + csvSeparator;
                                strOut += reader2[4].ToString() + csvSeparator;
                                strOut += reader2[5].ToString() + csvSeparator;
                                strOut += reader2[6].ToString() + csvSeparator;
                                strOut += reader2[7].ToString() + csvSeparator;
                                strOut += reader2[8].ToString() + csvSeparator;
                                strOut += reader2[9].ToString() + csvSeparator;
                                strOut += reader2[10].ToString() + csvSeparator;
                                strOut += reader2[11].ToString();
                            }
                            reader2.Close();
                            lstRows.Add(strOut);

                            strSql = "select id_job, counter from wp1_instance_exec where id_ticket = :id_ticket ";
                            cmd.CommandText = strSql;
                            reader2 = cmd.ExecuteReader();
                            while (reader2.Read())
                            {
                                strOut = "JOB_ROW" + csvSeparator + reader2[0].ToString();
                                lstRows.Add(strOut);

                                OracleParameter id_job = new OracleParameter("id_job", reader2[0].ToString());
                                OracleParameter counter = new OracleParameter("counter", reader2[1].ToString());
                                cmd.Parameters.Add(id_job);
                                cmd.Parameters.Add(counter);

                                strSql = "select name_param, val_param, is_macro from wp1_instance_parameters where id_ticket = :id_ticket and id_job = :id_job and counter= :counter ";
                                cmd.CommandText = strSql;
                                reader3 = cmd.ExecuteReader();
                                while (reader3.Read())
                                {
                                    strOut = "JOB_PARAM_ROW" + csvSeparator;
                                    strOut += reader3[0].ToString() + csvSeparator;
                                    strOut += reader3[1].ToString() + csvSeparator;
                                    strOut += reader3[2].ToString();
                                    lstRows.Add(strOut);

                                }
                                reader3.Close();
                            }
                            reader2.Close();
                        }
                        reader.Close();
                    }


                    JpubDb.Close();


                    if (lstRows.Count > 0)
                    {
                        strPath += Path.DirectorySeparatorChar;

                        if (File.Exists(strPath + strFilename) == true)
                        {
                            if (checkBox6.Checked == false)
                            {
                                if (MessageBox.Show("Sovrascrivere il file\n" + strPath + strFilename + " ?", "Esporta file CSV", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                                    return (bRc);
                                else
                                    File.Delete(strPath + strFilename);
                            }
                            else
                                File.Delete(strPath + strFilename);
                        }


                        TextWriter tw = new StreamWriter(strPath + strFilename, false);

                        for (int i = 0; i < lstRows.Count; i++)
                        {
                            tw.Write(lstRows[i] + "\n");
                        }
                        tw.Close();
                        bRc = true;

                    }



                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Esporta file CSV - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Esporta file CSV Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }


            return (bRc);
        }


        /// <summary>
        /// Questo metodo carica la checkedListBoxJob
        /// </summary>
        /// <return>Boolean</return>
        private Boolean caricaListaJob()
        {
            Boolean bRc = false;
            String strSql = "";
            int sqlRc = 0;
            String strOut = "";

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {

                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleDataReader reader;
                    OracleCommand cmd = new OracleCommand();

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = cn;

                    strOut = "ID_JOB#DESCRIZIONE#ACTIVE#JOB_TYPE#CODE_TIPO_CONTR#MASTERTABLE";

                    strSql = "select id_job,descrizione,active,job_type,nvl(code_tipo_contr,'ALL'),mastertable from wp1_job order by id_job";
                    cmd.CommandText = strSql;
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        strOut = reader[0].ToString() + csvSeparator + reader[1].ToString() + csvSeparator + reader[2].ToString() + csvSeparator + reader[3].ToString() + csvSeparator + reader[4].ToString() + csvSeparator + reader[5].ToString();
                        chkLstBoxJob.Items.Add(strOut, CheckState.Unchecked);
                    }

                    reader.Close();
                    JpubDb.Close();

                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Esporta file CSV - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Esporta file CSV Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }

            Cursor.Current = Cursors.Default;


            return (bRc);
        }


        /// <summary>
        /// Questo metodo carica la checkedListBoxCondition
        /// </summary>
        /// <return>Boolean</return>
        private Boolean caricaListaCondition()
        {
            Boolean bRc = false;
            String strSql = "";
            int sqlRc = 0;
            String strOut = "";

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {

                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleDataReader reader;
                    OracleCommand cmd = new OracleCommand();

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = cn;

                    strSql = "select id_condition,field,operator,operands,field_type,cond_type from wp1_conditions order by id_condition";
                    cmd.CommandText = strSql;
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        strOut = reader[0].ToString() + csvSeparator + reader[1].ToString() + csvSeparator + reader[2].ToString() + csvSeparator + reader[3].ToString() + csvSeparator + reader[4].ToString() + csvSeparator + reader[5].ToString();
                        checkedListBoxCondition.Items.Add(strOut, CheckState.Unchecked);
                    }
                    reader.Close();
                    JpubDb.Close();

                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Esporta file CSV - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Esporta file CSV Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }

            Cursor.Current = Cursors.Default;


            return (bRc);
        }


        /// <summary>
        /// Questo metodo carica la checkedListBoxOperation
        /// </summary>
        /// <return>Boolean</return>
        private Boolean caricaListaOperation()
        {
            Boolean bRc = false;
            String strSql = "";
            int sqlRc = 0;
            String strOut = "";

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {

                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleDataReader reader;
                    OracleCommand cmd = new OracleCommand();

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = cn;

                    strSql = "select id_operation,field,operator,operand,op_type from wp1_operations order by id_operation";
                    cmd.CommandText = strSql;
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        strOut = reader[0].ToString() + csvSeparator + reader[1].ToString() + csvSeparator + reader[2].ToString() + csvSeparator + reader[3].ToString() + csvSeparator + reader[4].ToString();
                        checkedListBoxOperation.Items.Add(strOut, CheckState.Unchecked);
                    }
                    reader.Close();
                    JpubDb.Close();

                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Esporta file CSV - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Esporta file CSV Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }

            Cursor.Current = Cursors.Default;


            return (bRc);
        }



        /// <summary>
        /// Questo metodo carica la checkedListBoxJobConfig
        /// </summary>
        /// <return>Boolean</return>
        private Boolean caricaListaJobConfig()
        {
            Boolean bRc = false;
            String strSql = "";
            int sqlRc = 0;
            String strOut = "";

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {

                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleDataReader reader;
                    OracleDataReader reader2;
                    OracleCommand cmd = new OracleCommand();

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = cn;


                    strSql = "select id_job from wp1_job order by id_job";
                    cmd.CommandText = strSql;
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        strOut = "JOB_ROW" + csvSeparator + reader[0].ToString() + csvSeparator + reader[0].ToString();
                        checkedListBoxJobConfig.Items.Add(strOut, CheckState.Unchecked);


                        OracleParameter id_job = new OracleParameter("id_job", reader[0].ToString()); cmd.Parameters.Add(id_job);
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(id_job);

                        strSql = "select id_condition,optional from wp1_job_condition where id_job = :id_job ";
                        cmd.CommandText = strSql;
                        reader2 = cmd.ExecuteReader();
                        while (reader2.Read())
                        {
                            strOut = "CONDITION_ROW" + csvSeparator + reader2[0].ToString() + csvSeparator + reader2[1].ToString();
                            checkedListBoxJobConfig.Items.Add(strOut, CheckState.Unchecked);

                        }
                        reader2.Close();

                        strSql = "select id_operation from wp1_job_operation where id_job = :id_job ";
                        cmd.CommandText = strSql;
                        reader2 = cmd.ExecuteReader();
                        while (reader2.Read())
                        {
                            strOut = "OPERATION_ROW" + csvSeparator + reader2[0].ToString();
                            checkedListBoxJobConfig.Items.Add(strOut, CheckState.Unchecked);
                        }
                        reader2.Close();

                    }
                    reader.Close();
                    JpubDb.Close();

                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Esporta file CSV - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Esporta file CSV Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }

            Cursor.Current = Cursors.Default;


            return (bRc);
        }



        /// <summary>
        /// Questo metodo carica la checkedListBoxTicketConfig
        /// </summary>
        /// <return>Boolean</return>
        private Boolean caricaListaTicketConfig()
        {
            Boolean bRc = false;
            String strSql = "";
            int sqlRc = 0;
            String strOut = "";

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {

                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleDataReader reader;
                    OracleDataReader reader2;
                    OracleDataReader reader3;
                    OracleCommand cmd = new OracleCommand();

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = cn;


                    strSql = "select id_ticket from wp1_instance_exec order by id_ticket";

                    cmd.CommandText = strSql;
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {

                        OracleParameter id_ticket = new OracleParameter("id_ticket", reader[0].ToString());
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(id_ticket);

                        strSql = "select id_ticket, to_char(data_emissione,'yyyymmdd'),to_char(data_inizio_val,'yyyymmdd'),to_char(data_fine_val,'yyyymmdd'),user_nws,user_ga,system,descrizione,ordinamento,active,ticket_type, nvl(code_tipo_contr,'null')  from wp1_ticketga where id_ticket = :id_ticket ";
                        cmd.CommandText = strSql;
                        reader2 = cmd.ExecuteReader();
                        while (reader2.Read())
                        {
                            strOut = "TICKET_ROW" + csvSeparator;
                            strOut += reader2[0].ToString() + csvSeparator;
                            strOut += reader2[1].ToString() + csvSeparator;
                            strOut += reader2[2].ToString() + csvSeparator;
                            strOut += reader2[3].ToString() + csvSeparator;
                            strOut += reader2[4].ToString() + csvSeparator;
                            strOut += reader2[5].ToString() + csvSeparator;
                            strOut += reader2[6].ToString() + csvSeparator;
                            strOut += reader2[7].ToString() + csvSeparator;
                            strOut += reader2[8].ToString() + csvSeparator;
                            strOut += reader2[9].ToString() + csvSeparator;
                            strOut += reader2[10].ToString() + csvSeparator;
                            strOut += reader2[11].ToString();
                        }
                        reader2.Close();
                        checkedListTicketConfig.Items.Add(strOut, CheckState.Unchecked);

                        strSql = "select id_job, counter from wp1_instance_exec where id_ticket = :id_ticket ";
                        cmd.CommandText = strSql;
                        reader2 = cmd.ExecuteReader();
                        while (reader2.Read())
                        {
                            strOut = "JOB_ROW" + csvSeparator + reader2[0].ToString();
                            checkedListTicketConfig.Items.Add(strOut, CheckState.Unchecked);


                            OracleParameter id_job = new OracleParameter("id_job", reader2[0].ToString());
                            OracleParameter counter = new OracleParameter("counter", reader2[1].ToString());
                            cmd.Parameters.Add(id_job);
                            cmd.Parameters.Add(counter);

                            strSql = "select name_param, val_param, is_macro from wp1_instance_parameters where id_ticket = :id_ticket and id_job = :id_job and counter= :counter ";
                            cmd.CommandText = strSql;
                            reader3 = cmd.ExecuteReader();
                            while (reader3.Read())
                            {
                                strOut = "JOB_PARAM_ROW" + csvSeparator;
                                strOut += reader3[0].ToString() + csvSeparator;
                                strOut += reader3[1].ToString() + csvSeparator;
                                strOut += reader3[2].ToString();
                                checkedListTicketConfig.Items.Add(strOut, CheckState.Unchecked);

                            }
                            reader3.Close();
                        }
                        reader2.Close();
                    }
                    reader.Close();
                    JpubDb.Close();

                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Esporta file CSV - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Esporta file CSV Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }

            Cursor.Current = Cursors.Default;


            return (bRc);
        }






        /// <summary>
        /// Questo metodo carica la checkedListBoxTicket
        /// </summary>
        /// <return>Boolean</return>
        private Boolean caricaListaTicket()
        {
            Boolean bRc = false;
            String strSql = "";
            int sqlRc = 0;
            String strOut = "";

            int recCount = 0;

            lvTicket.Clear();
            lvTicket.View = View.Details;
            lvTicket.AllowColumnReorder = true;
            lvTicket.FullRowSelect = true;
            lvTicket.GridLines = true;
            lvTicket.Scrollable = true;
            lvTicket.Columns.Add("ID_TICKET", "ID_TICKET", 200, HorizontalAlignment.Left, 0);
            lvTicket.Columns.Add("DESCRIZIONE", "DESCRIZIONE", 360, HorizontalAlignment.Left, 0);



            try
            {
                Cursor.Current = Cursors.WaitCursor;

                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {

                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleDataReader reader;
                    OracleCommand cmd = new OracleCommand();

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = cn;


                    strSql = "select id_ticket, descrizione from wp1_ticketga order by id_ticket";

                    cmd.CommandText = strSql;
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {

                        Console.WriteLine("Letto:" + reader[0]);
                        ListViewItem i = new ListViewItem(reader[0].ToString());
                        i.SubItems.Add(reader[1].ToString());
                        lvTicket.Items.Add(i);
                        recCount++;

                    }

                    reader.Close();
                    JpubDb.Close();

                    lvTicket.Show();
                    lvTicket.Refresh();
                    bRc = true;

                    if (recCount > 0)
                    {
                        lvTicket.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                    }


                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Esporta file CSV - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Esporta file CSV Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }

            Cursor.Current = Cursors.Default;


            return (bRc);
        }



        /// <summary>
        /// Questo metodo carica la lvJobs
        /// </summary>
        /// <return>Boolean</return>
        private Boolean caricaListaJobs()
        {
            Boolean bRc = false;
            String strSql = "";
            int sqlRc = 0;
            String strOut = "";

            int recCount = 0;

            lvJob.Clear();
            lvJob.View = View.Details;
            lvJob.AllowColumnReorder = true;
            lvJob.FullRowSelect = true;
            lvJob.GridLines = true;
            lvJob.Scrollable = true;
            lvJob.Columns.Add("ID_JOB", "ID_JOB", 200, HorizontalAlignment.Left, 0);
            lvJob.Columns.Add("DESCRIZIONE", "DESCRIZIONE", 360, HorizontalAlignment.Left, 0);



            try
            {
                Cursor.Current = Cursors.WaitCursor;

                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {

                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleDataReader reader;
                    OracleCommand cmd = new OracleCommand();

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = cn;


                    strSql = "select id_job, descrizione from wp1_job order by id_job";

                    cmd.CommandText = strSql;
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {

                        Console.WriteLine("Letto:" + reader[0]);
                        ListViewItem i = new ListViewItem(reader[0].ToString());
                        i.SubItems.Add(reader[1].ToString());
                        lvJob.Items.Add(i);
                        recCount++;

                    }

                    reader.Close();
                    JpubDb.Close();

                    lvJob.Show();
                    lvJob.Refresh();
                    bRc = true;

                    if (recCount > 0)
                    {
                        lvJob.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                    }


                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Esporta file CSV - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Esporta file CSV Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }

            Cursor.Current = Cursors.Default;


            return (bRc);
        }





        private void btnAvanzateExport_Click(object sender, EventArgs e)
        {
            esportaListaAvanzata(chkLstBoxJob, "job_catalog.csv", "ID_JOB#DESCRIZIONE#ACTIVE#JOB_TYPE#CODE_TIPO_CONTR#MASTERTABLE\n");
        }

        private bool esportaListaAvanzata(CheckedListBox cb, String strFilename, String strHeader)
        {
            bool bRc = false;

            String strPath = "";
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();

            if (cb.CheckedItems.Count > 0)
            {
                folderBrowserDialog1.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    Console.WriteLine("OUTPUTH FOLDER:" + folderBrowserDialog1.SelectedPath);
                    strPath = folderBrowserDialog1.SelectedPath;
                }
                else
                    return (bRc);


                strFilename = Path.DirectorySeparatorChar + strFilename;

                if (File.Exists(strPath + strFilename) == true)
                {
                    if (checkBox6.Checked == false)
                    {
                        if (MessageBox.Show("Sovrascrivere il file\n" + strPath + strFilename + " ?", "Esporta file CSV", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                            return (bRc);
                        else
                            File.Delete(strPath + strFilename);
                    }
                    else
                        File.Delete(strPath + strFilename);
                }

                try
                {
                    TextWriter tw = new StreamWriter(strPath + strFilename, false);

                    if (strHeader.CompareTo("") != 0)
                    {
                        tw.Write(strHeader);
                    }

                    for (int i = 0; i < cb.Items.Count; i++)
                    {
                        if (cb.GetItemChecked(i) == true)
                            tw.Write(cb.Items[i] + "\n");
                    }
                    tw.Close();

                    MessageBox.Show("Esportazione del file terminata con successo!\n" + strPath + strFilename, "Esporta file CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Errore durante l\'esportazione del file!\n" + ex.Message, "Esporta file CSV - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }

            return (bRc);
        }//esportaListaAvanzata


        /// <summary>
        /// Questo metodo seleziona/deseleziona tutti gli elementi di un controllo ChecedkListBox
        /// </summary>
        /// <return>Void</return>
        private void selectAllChkBoxLst(CheckedListBox cb, bool bSelect)
        {
            cb.Visible = false;
            for (int i = 0; i < cb.Items.Count; i++)
            {
                cb.SetItemChecked(i, bSelect);
            }
            cb.Visible = true;
        }

        private void btnSelectAllJob_Click(object sender, EventArgs e)
        {
            selectAllChkBoxLst(chkLstBoxJob, true);

        }

        private void btnDeselectAllJob_Click(object sender, EventArgs e)
        {
            selectAllChkBoxLst(chkLstBoxJob, false);
        }


        /// <summary>
        /// Metodo che gestisce le chiamate ai Tab della form
        /// </summary>
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            Console.WriteLine("TAB:" + e.TabPageIndex);


            switch (e.TabPageIndex)
            {
                case (1):
                    caricaListaJob();
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Metodo che gestisce le chiamate ai Tab della form
        /// </summary>
        private void tabControl2_Selected(object sender, TabControlEventArgs e)
        {
            Console.WriteLine("TAB:" + e.TabPageIndex);


            switch (e.TabPageIndex)
            {
                case (0):
                    caricaListaJob();
                    break;
                case (1):
                    caricaListaCondition();
                    break;
                case (2):
                    caricaListaOperation();
                    break;
                case (3):
                    //caricaListaJobConfig();
                    caricaListaTicket();
                    break;
                case (4):
                    //caricaListaTicketConfig();
                    caricaListaJobs();
                    break;
                case (5):
                    //caricaListaTicket();
                    break;
                case (6):
                    //caricaListaJobs();
                    break;
                default:
                    break;
            }

        }

        private void btnSelectAllCondition_Click(object sender, EventArgs e)
        {
            selectAllChkBoxLst(checkedListBoxCondition, true);
        }

        private void btnDeselectAllCondition_Click(object sender, EventArgs e)
        {
            selectAllChkBoxLst(checkedListBoxCondition, false);

        }

        private void btnSelectAllJobConfig_Click(object sender, EventArgs e)
        {
            selectAllChkBoxLst(checkedListBoxJobConfig, true);
        }

        private void btnDeselectAllJobConfig_Click(object sender, EventArgs e)
        {
            selectAllChkBoxLst(checkedListBoxJobConfig, false);

        }

        private void btnSelectAllOperation_Click(object sender, EventArgs e)
        {
            selectAllChkBoxLst(checkedListBoxOperation, true);

        }

        private void btnDeselectAllOperation_Click(object sender, EventArgs e)
        {
            selectAllChkBoxLst(checkedListBoxOperation, false);

        }

        private void btnSelectAllTicketConfig_Click(object sender, EventArgs e)
        {
            selectAllChkBoxLst(checkedListTicketConfig, true);

        }

        private void btnDeselectAllTicketConfig_Click(object sender, EventArgs e)
        {
            selectAllChkBoxLst(checkedListTicketConfig, false);

        }

        private void btnAvanzateExportCondition_Click(object sender, EventArgs e)
        {
            esportaListaAvanzata(checkedListBoxCondition, "condition_catalog.csv", "ID_CONDITION#FIELD#OPERATOR#OPERANDS#FIELD_TYPE#COND_TYPE\n");
        }

        private void btnAvanzateExportOperation_Click(object sender, EventArgs e)
        {
            esportaListaAvanzata(checkedListBoxOperation, "operation_catalog.csv", "ID_OPERATION#FIELD#OPERATOR#OPERAND#OP_TYPE\n");
        }

        private void btnAvanzateExportJobConfig_Click(object sender, EventArgs e)
        {
            esportaListaAvanzata(checkedListBoxJobConfig, "job_config.csv", "");
        }

        private void btnAvanzateExportTicketConfig_Click(object sender, EventArgs e)
        {
            esportaListaAvanzata(checkedListTicketConfig, "ticket_config.csv", "");
        }

        private void btnSelectAllTicket_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lvTicket.Items.Count; i++)
            {
                lvTicket.Items[i].Checked = true;
            }
        }

        private void btnDeselectAllTicket_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lvTicket.Items.Count; i++)
            {
                lvTicket.Items[i].Checked = false;
            }


        }

        private void btnExportTicket_Click(object sender, EventArgs e)
        {

            String strSql = "";
            String strOut = "";
            String strPath = "";
            String strFilename = "ticket_config";

            String strOld = "";




            if (lvTicket.CheckedItems.Count <= 0)
            {
                MessageBox.Show("Nessun elemento selezionato in lista!", "Esporta file CSV - Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;

            }

            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine("OUTPUTH FOLDER:" + folderBrowserDialog1.SelectedPath);
            }
            else
                return;

            strPath = folderBrowserDialog1.SelectedPath;




            try
            {
                Cursor.Current = Cursors.WaitCursor;

                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {

                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleDataReader reader;
                    OracleDataReader reader2;
                    OracleDataReader reader3;
                    OracleCommand cmd = new OracleCommand();

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = cn;


                    if (File.Exists(strPath + "\\" + strFilename + ".csv") == true)
                    {
                        File.Delete(strPath + "\\" + strFilename + ".csv");
                    }

                    TextWriter tw = new StreamWriter(strPath + "\\" + strFilename + ".csv", false);

                    for (int i = 0; i < lvTicket.Items.Count; i++)
                    {
                        if (lvTicket.Items[i].Checked == true)
                        {



                            OracleParameter id_ticket = new OracleParameter("id_ticket", lvTicket.Items[i].Text);
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(id_ticket);

                            strSql = "select id_ticket , counter from wp1_instance_exec where id_ticket = :id_ticket order by counter";

                            cmd.CommandText = strSql;
                            reader = cmd.ExecuteReader();

                            while (reader.Read())
                            {

                                if (strOld.CompareTo(lvTicket.Items[i].Text) != 0)
                                {
                                    strOld = lvTicket.Items[i].Text;

                                    //OracleParameter id_ticket_r = new OracleParameter("id_ticket_r", reader[0].ToString());
                                    //cmd.Parameters.Clear();
                                    //cmd.Parameters.Add(id_ticket_r);

                                    strSql = "select id_ticket, to_char(data_emissione,'yyyymmdd'),to_char(data_inizio_val,'yyyymmdd'),to_char(data_fine_val,'yyyymmdd'),user_nws,user_ga,system,descrizione,ordinamento,active,ticket_type, nvl(code_tipo_contr,'null')  from wp1_ticketga where id_ticket = :id_ticket ";
                                    cmd.CommandText = strSql;
                                    reader2 = cmd.ExecuteReader();
                                    while (reader2.Read())
                                    {
                                        strOut = "TICKET_ROW" + csvSeparator;
                                        strOut += reader2[0].ToString() + csvSeparator;
                                        strOut += reader2[1].ToString() + csvSeparator;
                                        strOut += reader2[2].ToString() + csvSeparator;
                                        strOut += reader2[3].ToString() + csvSeparator;
                                        strOut += reader2[4].ToString() + csvSeparator;
                                        strOut += reader2[5].ToString() + csvSeparator;
                                        strOut += reader2[6].ToString() + csvSeparator;
                                        strOut += reader2[7].ToString() + csvSeparator;
                                        strOut += reader2[8].ToString() + csvSeparator;
                                        strOut += reader2[9].ToString() + csvSeparator;
                                        strOut += reader2[10].ToString() + csvSeparator;
                                        strOut += reader2[11].ToString();
                                    }
                                    reader2.Close();
                                    tw.Write(strOut + "\n");
                                }


                                id_ticket = new OracleParameter("id_ticket", lvTicket.Items[i].Text);
                                OracleParameter counter_r = new OracleParameter("counter_r", reader[1].ToString());
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add(id_ticket);
                                cmd.Parameters.Add(counter_r);
                                strSql = "select id_job, counter from wp1_instance_exec where id_ticket = :id_ticket and counter = :counter_r ";
                                cmd.CommandText = strSql;
                                reader2 = cmd.ExecuteReader();
                                while (reader2.Read())
                                {
                                    strOut = "JOB_ROW" + csvSeparator + reader2[0].ToString();
                                    tw.Write(strOut + "\n");

                                    OracleParameter[] parameters = new OracleParameter[]{
                                    new OracleParameter("id_ticket_2",reader[0].ToString()),
                                    new OracleParameter("id_job_2",reader2[0].ToString()),
                                    new OracleParameter("counter_2",reader2[1].ToString())
                                     };


                                    strSql = "select name_param, val_param, is_macro from wp1_instance_parameters where id_ticket = :id_ticket_2 and id_job = :id_job_2 and counter= :counter_2 ";
                                    cmd.CommandText = strSql;
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.AddRange(parameters);
                                    reader3 = cmd.ExecuteReader();
                                    while (reader3.Read())
                                    {
                                        strOut = "JOB_PARAM_ROW" + csvSeparator;
                                        strOut += reader3[0].ToString() + csvSeparator;
                                        strOut += reader3[1].ToString() + csvSeparator;
                                        strOut += reader3[2].ToString();
                                        tw.Write(strOut + "\n");


                                    }
                                    reader3.Close();
                                }
                                reader2.Close();
                            }
                            reader.Close();


                        }

                    }

                    tw.Close();



                    JpubDb.Close();
                    MessageBox.Show("Esportazione terminata con successo!", "Esporta file CSV ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Esporta file CSV - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Esporta file CSV Error:" + ex.Message + "\nSQL:" + strSql);
            }

            Cursor.Current = Cursors.Default;


        }

        private void btnSelectAllJobs_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lvJob.Items.Count; i++)
            {
                lvJob.Items[i].Checked = true;
            }
        }

        private void btnDeselectAllJobs_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lvJob.Items.Count; i++)
            {
                lvJob.Items[i].Checked = false;
            }
        }

        private void btnEsportaJobs_Click(object sender, EventArgs e)
        {


            String strSql = "";
            String strOut = "";
            String strPath = "";
            String strFilename = "job_config";






            if (lvJob.CheckedItems.Count <= 0)
            {
                MessageBox.Show("Nessun elemento selezionato in lista!", "Esporta file CSV - Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;

            }

            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine("OUTPUTH FOLDER:" + folderBrowserDialog1.SelectedPath);
            }
            else
                return;

            strPath = folderBrowserDialog1.SelectedPath;




            try
            {
                Cursor.Current = Cursors.WaitCursor;

                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {

                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleDataReader reader;
                    OracleDataReader reader2;
                    OracleDataReader reader3;
                    OracleCommand cmd = new OracleCommand();

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = cn;


                    if (File.Exists(strPath + "\\" + strFilename + ".csv") == true)
                    {
                        File.Delete(strPath + "\\" + strFilename + ".csv");
                    }

                    TextWriter tw = new StreamWriter(strPath + "\\" + strFilename + ".csv", false);




                    for (int i = 0; i < lvJob.Items.Count; i++)
                    {
                        if (lvJob.Items[i].Checked == true)
                        {



                            OracleParameter id_job = new OracleParameter("id_job", lvJob.Items[i].Text);
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(id_job);
                            strSql = "select id_job  from wp1_job where id_job = :id_job ";
                            cmd.CommandText = strSql;
                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {

                                OracleParameter id_job_r = new OracleParameter("id_job_r", reader[0].ToString());
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add(id_job_r);

                                strOut = "JOB_ROW" + csvSeparator + reader[0].ToString() + csvSeparator + reader[0].ToString();
                                tw.Write(strOut + "\n");

                                strSql = "select id_condition,optional from wp1_job_condition where id_job = :id_job_r ";
                                cmd.CommandText = strSql;
                                reader2 = cmd.ExecuteReader();
                                while (reader2.Read())
                                {
                                    strOut = "CONDITION_ROW" + csvSeparator + reader2[0].ToString() + csvSeparator + reader2[1].ToString();
                                    tw.Write(strOut + "\n");
                                }
                                reader2.Close();

                                strSql = "select id_operation from wp1_job_operation where id_job = :id_job_r ";
                                cmd.CommandText = strSql;
                                reader2 = cmd.ExecuteReader();
                                while (reader2.Read())
                                {
                                    strOut = "OPERATION_ROW" + csvSeparator + reader2[0].ToString();
                                    tw.Write(strOut + "\n");
                                }
                                reader2.Close();

                            }
                            reader.Close();


                        }

                    }

                    tw.Close();

                    JpubDb.Close();
                    MessageBox.Show("Esportazione terminata con successo!", "Esporta file CSV ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Esporta file CSV - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Esporta file CSV Error:" + ex.Message + "\nSQL:" + strSql);
            }

            Cursor.Current = Cursors.Default;

        }



    }
}
