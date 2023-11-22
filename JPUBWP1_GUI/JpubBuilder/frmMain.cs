using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SqlClient;
using System.Xml;
using System.Data.OracleClient;

using System.Text.RegularExpressions;


namespace JPubGfx
{


    public partial class frmMain : Form
    {

        /// <summary>
        /// Classe che gestisce l'interfaccia grafica del main.
        /// </summary>

        int indexSelectedTicket = -1;
        int indexSelectedTab = -1;
        string currJobSelected = "";
        string lastPathSelected = "";
        string APP_NAME = "JPub Builder ";
        string VERSION = "Ver 1.0";
        Boolean contextMenuConditionItem = false;
        string strSelectJob = "- SELEZIONA JOB -                ";
        string strSelezionaCondition = "- SELEZIONA CONDITION -                ";
        string strSelezionaOperation = "- SELEZIONA OPERATION -                ";
        Job currJob;

        string STR_CLIPBOARD_UTILITY = "";

        Boolean bCarica = false;


        List<Condition> lstCondition = new List<Condition>();
        List<Operation> lstOperation = new List<Operation>();


        /// <summary>
        /// Costruttore della classe
        /// </summary>
        public frmMain()
        {
            InitializeComponent();
            //InitializeJpubComponents();
        }


        /// <summary>
        /// Metodo che inizializza i componenti della form
        /// </summary>
        private void InitializeJpubComponents()
        {
            DropDownTreeNode tn1 = new DropDownTreeNode("-- nessuna condizione --");
            tn1.ComboBox.Items.Add("-- nessuna condizione --");
            tn1.ComboBox.Items.Add("Condition 001");
            tn1.ComboBox.Items.Add("Condition 002");
            tn1.ComboBox.Items.Add("Condition 003");
            tn1.ComboBox.SelectedIndex = 0;
            //tn1.BackColor = Color.AliceBlue;
            //tn1.ComboBox.BackColor = Color.AliceBlue;

            DropDownTreeNode tn2 = new DropDownTreeNode("-- nessuna condizione --");
            tn2.ComboBox.Items.Add("-- nessuna condizione --");
            tn2.ComboBox.Items.Add("Condition 001");
            tn2.ComboBox.Items.Add("Condition 002");
            tn2.ComboBox.Items.Add("Condition 003");
            tn2.ComboBox.SelectedIndex = 0;

            TreeNode jobNode = new TreeNode("Job 001");
            jobNode.Nodes.Add(tn1);
            jobNode.Nodes.Add(tn2);
            this.dropDownTreeView1.Nodes.Add(jobNode);


            DropDownTreeNode pickleNode = new DropDownTreeNode("No pickles");
            pickleNode.ComboBox.Items.Add("No pickles");
            pickleNode.ComboBox.Items.Add("Add pickles");
            pickleNode.ComboBox.SelectedIndex = 0;

            DropDownTreeNode lettuceNode = new DropDownTreeNode("No lettuce");
            lettuceNode.ComboBox.Items.Add("No lettuce");
            lettuceNode.ComboBox.Items.Add("Add lettuce");
            lettuceNode.ComboBox.SelectedIndex = 0;

            DropDownTreeNode onionNode = new DropDownTreeNode("No onions");
            onionNode.ComboBox.Items.Add("No onions");
            onionNode.ComboBox.Items.Add("Add onions");
            onionNode.ComboBox.SelectedIndex = 0;

            TreeNode condimentsNode = new TreeNode("Condiments");
            condimentsNode.Nodes.Add(pickleNode);
            condimentsNode.Nodes.Add(lettuceNode);
            condimentsNode.Nodes.Add(onionNode);


            DropDownTreeNode weightNode = new DropDownTreeNode("1/4 lb.");
            weightNode.ComboBox.Items.Add("1/4 lb.");
            weightNode.ComboBox.Items.Add("1/2 lb.");
            weightNode.ComboBox.Items.Add("3/4 lb.");
            weightNode.ComboBox.SelectedIndex = 0;

            DropDownTreeNode pattyNode = new DropDownTreeNode("All beef patty");
            pattyNode.ComboBox.Items.Add("All beef patty");
            pattyNode.ComboBox.Items.Add("All chicken patty");
            pattyNode.ComboBox.SelectedIndex = 0;

            TreeNode meatNode = new TreeNode("Meat Selection");
            meatNode.Nodes.Add(weightNode);
            meatNode.Nodes.Add(pattyNode);


            TreeNode burgerNode = new TreeNode("Hamburger Selection");
            burgerNode.Nodes.Add(condimentsNode);
            burgerNode.Nodes.Add(meatNode);
            this.dropDownTreeView1.Nodes.Add(burgerNode);


            this.dropDownTreeView1.ExpandAll();

        }




        /// <summary>
        /// Questo metodo inizializza il Tab Ticket
        /// </summary>
        private void initTicketTab()
        {

            Cursor.Current = Cursors.WaitCursor;

            comboBox1.Items.Clear();
            comboBox3.Items.Clear();
            comboBox5.Items.Clear();
            comboBox25.Items.Clear();


            textBox1.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";

            textBox2.Text = "";

            //System

            cbAdd(comboBox1, "");
            cbAdd(comboBox1, "ALL");
            cbAdd(comboBox1, "REG");
            cbAdd(comboBox1, "XDSL");
            cbAdd(comboBox1, "CPSNP");
            comboBox1.SelectedIndex = 0;


            //Flag_attivo
            cbAdd(comboBox3, "NO");
            cbAdd(comboBox3, "SI");
            cbAdd(comboBox3, "");
            comboBox3.SelectedIndex = 2;

            //Ticket_Type
            cbAdd(comboBox5, "");
            cbAdd(comboBox5, "PREVAL");
            cbAdd(comboBox5, "POSTVAL");
            //cbAdd(comboBox5, "ALL");
            cbAdd(comboBox5, "TREND");
            cbAdd(comboBox5, "SAP");
            comboBox5.SelectedIndex = 0;

            dateTimePicker1.Value = DateTime.Today;
            dateTimePicker2.Value = DateTime.Today;
            dateTimePicker3.Value = new DateTime(2099, 12, 31);




            refreshTicketsList();

            Cursor.Current = Cursors.Default;

        }

        /// <summary>
        /// Questo metodo inizializza il Tab Job
        /// </summary>
        private void initJobTab()
        {

            Cursor.Current = Cursors.WaitCursor;

            comboBox4.Items.Clear();
            comboBox6.Items.Clear();
            comboBox7.Items.Clear();
            comboBox18.Items.Clear();
            textBox7.Text = "";
            textBox8.Text = "";
            textBox3.Text = "";

            //System
            ComboboxItem item = new ComboboxItem();

            //Flag_attivo
            item = new ComboboxItem();
            item.Text = "NO";
            item.Value = 0;
            comboBox4.Items.Add(item);
            item = new ComboboxItem();
            item.Text = "SI";
            item.Value = 1;
            comboBox4.Items.Add(item);
            comboBox4.SelectedIndex = 0;


            cbAdd(comboBox6, "");
            cbAdd(comboBox6, "PREVAL");
            cbAdd(comboBox6, "POSTVAL");
            cbAdd(comboBox6, "TREND");
            cbAdd(comboBox6, "SAP");

            comboBox6.SelectedIndex = 0;

            caricaMasterTableJob(comboBox7);
            caricaCodeTipoContrJob(comboBox18, true);
            refreshJobsList();





            Cursor.Current = Cursors.Default;

        }

        /// <summary>
        /// Questo metodo inizializza il Tab Utility
        /// </summary>
        private void initUtilityTab()
        {

            Cursor.Current = Cursors.WaitCursor;

            refreshUtilityList();

            Cursor.Current = Cursors.Default;

        }


        /// <summary>
        /// Questo metodo inizializza il Tab Ticket->Job
        /// </summary>
        private void initTicketJobTab()
        {

            Cursor.Current = Cursors.WaitCursor;

            comboBox20.Items.Clear();
            comboBox21.Items.Clear();
            comboBox22.Items.Clear();
            comboBox19.Items.Clear();

            comboBox20.Text = "";
            comboBox21.Text = "";
            comboBox22.Text = "";
            comboBox19.Text = "";

            textBox14.Text = "";
            caricaTicket(comboBox20);
            caricaJob(comboBox19);
            refreshTicketsJobsList();
            listView6.Items.Clear();


            Cursor.Current = Cursors.Default;

        }


        /// <summary>
        /// Questo metodo esegue l'aggiornamento della lista associazioni Ticket->Job
        /// </summary>
        private void refreshTicketsJobsList()
        {
            //Lista Tickets->Jobs
            listView5.Clear();
            listView6.Clear();
            listView5.View = View.Details;
            listView5.AllowColumnReorder = true;
            listView5.FullRowSelect = true;
            listView5.GridLines = true;
            listView5.Scrollable = true;
            listView5.Columns.Add("ID_TICKET", "ID_TICKET", 260, HorizontalAlignment.Left, 0);
            listView5.Columns.Add("ID_JOB", "ID_JOB", 260, HorizontalAlignment.Left, 0);
            listView5.Columns.Add("PROGRESSIVO", "PROGRESSIVO", 120, HorizontalAlignment.Center, 0);

            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();
                int recCount = 0;

                cmd.CommandText = "SELECT id_ticket,id_job,counter FROM wp1_instance_exec where id_ticket = :idTicket order by counter";

                OracleParameter idTicket = new OracleParameter("idTicket", comboBox20.Text);
                cmd.Parameters.Add(idTicket);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine("Letto:" + reader[0]);
                    ListViewItem i = new ListViewItem(reader[0].ToString());
                    i.SubItems.Add(reader[1].ToString());
                    i.SubItems.Add(reader[2].ToString());
                    listView5.Items.Add(i);

                    recCount++;
                }

                if (recCount > 0)
                {
                    listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                }

                reader.Close();

                JpubDb.Close();

                listView5.Show();
                listView5.Refresh();

            }
            else
            {
                MessageBox.Show(JpubDb.getLastError(), "Errore Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                JpubDb = null;
            }

        }


        /// <summary>
        /// Questo metodo esegue l'aggiornamento della lista dei ticket
        /// </summary>
        private void refreshTicketsList()
        {
            //Lista Tickets
            listView1.Clear();
            listView1.View = View.Details;
            listView1.AllowColumnReorder = true;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Scrollable = true;
            listView1.Columns.Add("ID_TICKET", "ID_TICKET", 50, HorizontalAlignment.Center, 0);
            listView1.Columns.Add("DATA_EMISSIONE", "DATA_EMISSIONE", 100, HorizontalAlignment.Center, 0);
            listView1.Columns.Add("DATA_INIZO", "DATA_INIZO", 100, HorizontalAlignment.Center, 0);
            listView1.Columns.Add("DATA_FINE", "DATA_FINE", 100, HorizontalAlignment.Center, 0);
            listView1.Columns.Add("USER_NWS", "USER_NWS", 80, HorizontalAlignment.Center, 0);
            listView1.Columns.Add("USER_GA", "USER_GA", 80, HorizontalAlignment.Center, 0);
            listView1.Columns.Add("SYSTEM", "SYSTEM", 80, HorizontalAlignment.Center, 0);
            listView1.Columns.Add("DESCRIZIONE", "DESCRIZIONE", 150, HorizontalAlignment.Left, 0);
            listView1.Columns.Add("ORDINAMENTO", "ORDINAMENTO", 80, HorizontalAlignment.Center, 0);
            listView1.Columns.Add("COD.TIPO.CONTR", "COD.TIPO.CONTR", 50, HorizontalAlignment.Center, 0);
            listView1.Columns.Add("ATTIVO", "ATTIVO", 50, HorizontalAlignment.Center, 0);
            listView1.Columns.Add("TYPE", "TYPE", 50, HorizontalAlignment.Center, 0);

            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();
                int recCount = 0;

                cmd.CommandText = "SELECT id_ticket,to_char(data_emissione,'dd-MM-YYYY') ,to_char(data_inizio_val,'dd-MM-YYYY'),to_char(data_fine_val,'dd-MM-YYYY'),user_nws,user_ga, nvl(system,'ALL') ,descrizione,ordinamento,nvl(code_tipo_contr,'null'),active,ticket_type FROM wp1_ticketga order by upper(id_ticket)";


                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine("Letto:" + reader[0]);
                    ListViewItem i = new ListViewItem(reader[0].ToString());
                    i.SubItems.Add(reader[1].ToString());
                    i.SubItems.Add(reader[2].ToString());
                    i.SubItems.Add(reader[3].ToString());
                    i.SubItems.Add(reader[4].ToString());
                    i.SubItems.Add(reader[5].ToString());
                    i.SubItems.Add(reader[6].ToString());
                    i.SubItems.Add(reader[7].ToString());
                    i.SubItems.Add(reader[8].ToString());
                    i.SubItems.Add(reader[9].ToString());
                    i.SubItems.Add(reader[10].ToString());
                    i.SubItems.Add(reader[11].ToString());
                    listView1.Items.Add(i);

                    recCount++;
                }

                if (recCount > 0)
                {
                    listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                }

                reader.Close();

                JpubDb.Close();

                listView1.Show();
                listView1.Refresh();

            }
            else
            {
                MessageBox.Show(JpubDb.getLastError(), "Errore Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                JpubDb = null;
            }

        }

        /// <summary>
        /// Questo metodo esegue l'aggiornamento della lista dei job
        /// </summary>
        private void refreshJobsList()
        {
            //Lista Tickets
            listView2.Clear();
            listView2.View = View.Details;
            listView2.AllowColumnReorder = true;
            listView2.FullRowSelect = true;
            listView2.GridLines = true;
            listView2.Scrollable = true;
            listView2.Columns.Add("ID_JOB", "ID_JOB", 50, HorizontalAlignment.Left, 0);
            listView2.Columns.Add("DESCRIZIONE", "DESCRIZIONE", 150, HorizontalAlignment.Left, 0);
            listView2.Columns.Add("ATTIVO", "ATTIVO", 50, HorizontalAlignment.Center, 0);
            listView2.Columns.Add("TIPO", "TIPO", 50, HorizontalAlignment.Center, 0);
            listView2.Columns.Add("TIPO CONTR.", "TIPO CONTR.", 50, HorizontalAlignment.Center, 0);
            listView2.Columns.Add("MASTERTABLE", "MASTERTABLE", 50, HorizontalAlignment.Left, 0);

            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();
                int recCount = 0;

                cmd.CommandText = "SELECT id_job,descrizione,active,job_type,nvl(code_tipo_contr,'ALL') ,mastertable FROM wp1_job order by upper(id_job)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ListViewItem i = new ListViewItem(reader[0].ToString());
                    i.SubItems.Add(reader[1].ToString());
                    i.SubItems.Add(reader[2].ToString());
                    i.SubItems.Add(reader[3].ToString());
                    i.SubItems.Add(reader[4].ToString());
                    i.SubItems.Add(reader[5].ToString());
                    listView2.Items.Add(i);

                    recCount++;
                }

                if (recCount > 0)
                {
                    listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                }

                reader.Close();

                JpubDb.Close();

                listView2.Show();
                listView2.Refresh();

            }
            else
            {
                MessageBox.Show(JpubDb.getLastError(), "Errore Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                JpubDb = null;
            }

        }





        /// <summary>
        /// Questo metodo esegue l'aggiornamento della lista Utility
        /// </summary>
        private void refreshUtilityList()
        {

            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();


                cmd.CommandText = "select * from  I5_1ACCOUNT order by CODE_ACCOUNT";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;
                DataSet dsAccount = new DataSet();
                OracleDataAdapter adapterAccount = new OracleDataAdapter(cmd);
                adapterAccount.Fill(dsAccount);
                dataGridView1.DataSource = dsAccount.Tables[0];


                cmd.CommandText = "select * from  I5_1CONTR order by CODE_CONTR";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;
                DataSet dsContr = new DataSet();
                OracleDataAdapter adapterContr = new OracleDataAdapter(cmd);
                adapterContr.Fill(dsContr);
                dataGridView2.DataSource = dsContr.Tables[0];


                cmd.CommandText = "select * from  I5_3GEST_TLC order by CODE_GEST ";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;
                DataSet dsGest = new DataSet();
                OracleDataAdapter adapterGest = new OracleDataAdapter(cmd);
                adapterGest.Fill(dsGest);
                dataGridView3.DataSource = dsGest.Tables[0];

                JpubDb.Close();

            }
            else
            {
                MessageBox.Show(JpubDb.getLastError(), "Errore Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                JpubDb = null;
            }

        }





        /// <summary>
        ///Questo metodo esegue l'aggiornamento della lista delle Conditions
        /// </summary>
        private void refreshConditionsList()
        {
            //Lista Conditions
            listView3.Clear();
            listView3.View = View.Details;
            listView3.AllowColumnReorder = true;
            listView3.FullRowSelect = true;
            listView3.GridLines = true;
            listView3.Scrollable = true;
            listView3.Columns.Add("ID_CONDITION", "ID_CONDITION", 50, HorizontalAlignment.Center, 0);
            listView3.Columns.Add("FIELD", "FIELD", 100, HorizontalAlignment.Left, 0);
            listView3.Columns.Add("OPERATOR", "OPERATOR", 100, HorizontalAlignment.Center, 0);
            listView3.Columns.Add("OPERANDS", "OPERANDS", 100, HorizontalAlignment.Left, 0);
            listView3.Columns.Add("FIELD_TYPE", "FIELD_TYPE", 80, HorizontalAlignment.Center, 0);
            listView3.Columns.Add("COND_TYPE", "COND_TYPE", 80, HorizontalAlignment.Center, 0);

            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();
                int recCount = 0;

                cmd.CommandText = "SELECT id_condition,field,operator,operands,field_type,cond_type FROM wp1_conditions order by upper(id_condition)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ListViewItem i = new ListViewItem(reader[0].ToString());
                    i.SubItems.Add(reader[1].ToString());
                    i.SubItems.Add(reader[2].ToString());
                    i.SubItems.Add(reader[3].ToString());
                    i.SubItems.Add(reader[4].ToString());
                    i.SubItems.Add(reader[5].ToString());
                    listView3.Items.Add(i);

                    recCount++;
                }

                if (recCount > 0)
                {
                    listView3.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                }

                reader.Close();

                JpubDb.Close();

                listView1.Show();
                listView1.Refresh();

            }
            else
            {
                MessageBox.Show(JpubDb.getLastError(), "Errore Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                JpubDb = null;
            }

        }//refreshConditionsList




        /// <summary>
        /// Questo metodo esegue l'aggiornamento della lista delle Operations
        /// </summary>
        private void refreshOperationsList()
        {
            //Lista Operations
            listView4.Clear();
            listView4.View = View.Details;
            listView4.AllowColumnReorder = true;
            listView4.FullRowSelect = true;
            listView4.GridLines = true;
            listView4.Scrollable = true;
            listView4.Columns.Add("ID_OPERATION", "ID_OPERATION", 50, HorizontalAlignment.Center, 0);
            listView4.Columns.Add("FIELD", "FIELD", 100, HorizontalAlignment.Left, 0);
            listView4.Columns.Add("OPERATOR", "OPERATOR", 100, HorizontalAlignment.Center, 0);
            listView4.Columns.Add("OPERANDS", "OPERANDS", 100, HorizontalAlignment.Left, 0);
            listView4.Columns.Add("OP_TYPE", "OP_TYPE", 80, HorizontalAlignment.Center, 0);

            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();
                int recCount = 0;

                cmd.CommandText = "SELECT id_operation,field,operator,operand,op_type FROM wp1_operations order by upper(id_operation)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ListViewItem i = new ListViewItem(reader[0].ToString());
                    i.SubItems.Add(reader[1].ToString());
                    i.SubItems.Add(reader[2].ToString());
                    i.SubItems.Add(reader[3].ToString());
                    i.SubItems.Add(reader[4].ToString());
                    listView4.Items.Add(i);

                    recCount++;
                }

                if (recCount > 0)
                {
                    listView4.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                }

                reader.Close();

                JpubDb.Close();

                listView4.Show();
                listView4.Refresh();

            }
            else
            {
                MessageBox.Show(JpubDb.getLastError(), "Errore Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                JpubDb = null;
            }

        }//refreshOperationsList


        /// <summary>
        /// Questa classe gestisce l'item per l'oggetto ComboBox
        /// </summary>
        /// <returns>ComboBoxItem</returns>
        public class ComboboxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }


        /// <summary>
        /// Questo metodo inserisce un ticket all'interno del database 
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {



            if (checkDateTicket(dateTimePicker2.Text, dateTimePicker3.Text) == false)
                return;


            int sqlRc = -1;
            String strSql = "";
            String strDescrizione = "";


            if (textBox6.Text.IndexOf(" ") != -1)
            {
                MessageBox.Show("Il campo chiave ID TICKET non può contenere spazi!", "Aggiungi nuovo ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox6.Focus();
                return;
            }

            if (textBox6.Text.Length <= 0)
            {
                MessageBox.Show("Campo ID Ticket Obbligatorio!", "Aggiungi nuovo ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox6.Focus();
                return;
            }
            if (textBox4.Text.Length <= 0)
            {
                MessageBox.Show("Campo User NWS Obbligatorio!", "Aggiungi nuovo ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox4.Focus();
                return;
            }
            if (textBox2.Text.Length <= 0)
            {
                MessageBox.Show("Campo Descrizione Obbligatorio!", "Aggiungi nuovo ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox2.Focus();
                return;
            }

            if (textBox1.Text.Length <= 0)
            {
                MessageBox.Show("Campo User GA Obbligatorio!", "Aggiungi nuovo ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox1.Focus();
                return;
            }


            if (comboBox1.Text.Length <= 0)
            {
                MessageBox.Show("Campo System Obbligatorio!", "Aggiungi nuovo ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox1.Focus();
                return;
            }


            if (textBox5.Text.Length <= 0)
            {
                MessageBox.Show("Campo Ordinamento Obbligatorio!", "Aggiungi nuovo ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox5.Focus();
                return;
            }
            else
            {
                if (IsNumeric(textBox5.Text) == false)
                {
                    MessageBox.Show("Campo Ordinamento deve essere un numerico!", "Aggiungi nuovo ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    textBox5.Focus();
                    return;
                }
            }


            if (comboBox25.Text.Length <= 0)
            {
                MessageBox.Show("Campo Code Tipo Contr. Obbligatorio!", "Aggiungi nuovo ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox25.Focus();
                return;
            }




            if (comboBox5.Text.CompareTo("") == 0)
            {
                MessageBox.Show("Campo Tipo Obbligatorio!", "Aggiungi nuovo ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox5.Focus();
                return;
            }

            if (comboBox3.Text.CompareTo("") == 0)
            {
                MessageBox.Show("Campo Attivo Obbligatorio!", "Aggiungi nuovo ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox3.Focus();
                return;
            }



            if (MessageBox.Show("Inserire il nuovo ticket nel database?", "Aggiungi nuovo ticket", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }


            try
            {
                //Inserimento nuovo ticket 
                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {


                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleCommand cmd = new OracleCommand();

                    strDescrizione = textBox2.Text;


                    strSql = "insert into wp1_ticketga (id_ticket,data_emissione,data_inizio_val,data_fine_val,user_nws,user_ga,system,descrizione,ordinamento,active,ticket_type,code_tipo_contr ) values (";
                    strSql += ":id_ticket,:data_emissione,:data_inizio_val,:data_fine_val,:user_nws,:user_ga,:system,:descrizione,:ordinamento,:active,:ticket_type,:code_tipo_contr ) ";


                    object str_p_system = null;
                    object str_p_code_tipo_contr = null;

                    if (comboBox1.Text.CompareTo("ALL") == 0)
                        str_p_system = DBNull.Value;
                    else
                        str_p_system = comboBox1.Text.ToString();

                    if (comboBox25.Text.CompareTo("null") == 0)
                        str_p_code_tipo_contr = DBNull.Value;
                    else
                        str_p_code_tipo_contr = (comboBox25.SelectedItem as ComboboxItem).Value.ToString();


                    OracleParameter[] parameters = new OracleParameter[]{
                                    new OracleParameter("id_ticket",textBox6.Text),
                                    new OracleParameter("data_emissione",DateTime.Parse(dateTimePicker1.Value.ToString("yyyy-MM-dd"))),
                                    new OracleParameter("data_inizio_val",DateTime.Parse(dateTimePicker2.Value.ToString("yyyy-MM-dd"))),
                                    new OracleParameter("data_fine_val",DateTime.Parse(dateTimePicker3.Value.ToString("yyyy-MM-dd"))),
                                    new OracleParameter("user_nws",textBox4.Text),
                                    new OracleParameter("user_ga",textBox1.Text),
                                    new OracleParameter("system",str_p_system),
                                    new OracleParameter("descrizione",strDescrizione),
                                    new OracleParameter("ordinamento", textBox5.Text),
                                    new OracleParameter("active",comboBox3.SelectedIndex),
                                    new OracleParameter("ticket_type",comboBox5.Text),
                                    new OracleParameter("code_tipo_contr",str_p_code_tipo_contr)
                                };
                    try
                    {

                        cmd.CommandText = strSql;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = cn;
                        cmd.Parameters.AddRange(parameters);
                        sqlRc = cmd.ExecuteNonQuery();
                    }
                    catch (Exception exsql)
                    {
                        MessageBox.Show(exsql.Message, "Aggiungi nuovo ticket - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }



                    Console.WriteLine("SQL:" + strSql + " - RC:" + sqlRc);

                    JpubDb.Close();

                    refreshTicketsList();
                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Aggiungi nuovo ticket - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("inserisciTicket Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }







        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            string theDate = dateTimePicker1.Value.ToString("yyyy-MM-dd hh.mm.ss");

            Console.WriteLine(theDate);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count <= 0)
                return;

            indexSelectedTicket = listView1.SelectedIndices[0];

            Console.WriteLine("Hai selezionato Ticket ID:" + indexSelectedTicket);

            setDateTimePicker(dateTimePicker1, listView1.SelectedItems[0].SubItems[1].Text);
            setDateTimePicker(dateTimePicker2, listView1.SelectedItems[0].SubItems[2].Text);
            setDateTimePicker(dateTimePicker3, listView1.SelectedItems[0].SubItems[3].Text);
            textBox6.Text = listView1.SelectedItems[0].SubItems[0].Text;
            textBox4.Text = listView1.SelectedItems[0].SubItems[4].Text;
            textBox1.Text = listView1.SelectedItems[0].SubItems[5].Text;
            comboBox1.Text = listView1.SelectedItems[0].SubItems[6].Text;
            textBox2.Text = listView1.SelectedItems[0].SubItems[7].Text;
            textBox5.Text = listView1.SelectedItems[0].SubItems[8].Text;
            comboBox5.Text = listView1.SelectedItems[0].SubItems[11].Text;

            selectInCombo(listView1.SelectedItems[0].SubItems[9].Text, comboBox25);

            comboBox3.SelectedIndex = Int32.Parse(listView1.SelectedItems[0].SubItems[10].Text);

            if (comboBox5.Text.CompareTo("SAP") == 0)
            {
                selectInCombo(listView1.SelectedItems[0].SubItems[9].Text, comboBox25);
            }


        }

        /// <summary>
        /// Questo metodo seleziona un item composto in una ComboBox
        /// </summary>
        private void selectInCombo(String strSelected, ComboBox cb)
        {
            int i = 0;
            cb.SelectedIndex = -1;
            foreach (ComboboxItem item in cb.Items)
            {

                if (item.Text.ToString().CompareTo(strSelected) == 0)
                {
                    cb.SelectedIndex = i;
                    break;
                }

                if (strSelected.CompareTo("ALL") == 0 && item.Value.ToString().CompareTo("null") == 0)
                {
                    cb.SelectedIndex = i;
                    break;
                }

                if (item.Value.ToString().CompareTo(strSelected) == 0)
                {
                    cb.SelectedIndex = i;
                    break;
                }

                i++;
            }
        }




        /// <summary>
        /// Questo metodo imposta la data e ora del controllo dateTimePicker
        /// </summary>
        /// <param name="dtPicker">Oggetto DateTimePicker</param>
        /// <param name="strDateTime">Stringa Data e Ora</param>
        /// <returns>Nothing</returns>
        private void setDateTimePicker(DateTimePicker dtPicker, String strDateTime)
        {


            string[] ar_parts = strDateTime.Split(' ');
            string[] ar_data = ar_parts[0].Split('-');

            int dd = Int32.Parse(ar_data[0]);
            int mm = Int32.Parse(ar_data[1]);
            int yyyy = Int32.Parse(ar_data[2]);

            if (strDateTime.IndexOf(".") > -1)
            {
                string[] ar_ora = ar_parts[1].Split('.');
                int hh = Int32.Parse(ar_ora[0]);
                int nn = Int32.Parse(ar_ora[1]);
                int ss = Int32.Parse(ar_ora[2]);
                dtPicker.Value = new DateTime(yyyy, mm, dd, hh, nn, ss);
            }
            else
            {
                dtPicker.Value = new DateTime(yyyy, mm, dd);
            }

        }//setDateTimePicker



        /// <summary>
        /// Questo metodo viene richiamato al caricamento della form e completa l'inizializzazione dei campi 
        /// </summary>
        private void frmMain_Load(object sender, EventArgs e)
        {

            this.Text = APP_NAME + VERSION;
            label39.Text = this.Text;

            dropDownTreeView1.Height = 476;
            Ridisegna();
            visualizzaToolStripMenuItem.Visible = false;
            // tabControl1.SelectedTab = tabPage1;
            indexSelectedTab = 0;
            ClearTreeView();

            JpubOraDb jpdb = new JpubOraDb();
            if (jpdb.checkDb() == false)
            {
                MessageBox.Show("E' necessario configurare la connessione al database prima di continuare!", "Configurazione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                var frmCfg = new Config();
                frmCfg.ShowDialog(this);

            }
            else
            {
                tabControl1.SelectedTab = tabPage6;
                //   initTicketTab();
            }

            jpdb = null;


        }

        private void frmMain_Paint(object sender, EventArgs e)
        {
            Ridisegna();
        }

        /// <summary>
        /// Metodo che gestisce le chiamate ai Tab della form
        /// </summary>
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            indexSelectedTab = e.TabPageIndex;
            Console.WriteLine("TAB:" + e.TabPageIndex);
            visualizzaToolStripMenuItem.Visible = false;


            switch (indexSelectedTab)
            {
                case (1):
                    initTicketTab();
                    break;
                case (2):
                    initJobTab();
                    break;
                case (3):
                    initConditionsTab();
                    break;
                case (4):
                    initOperationsTab();
                    break;
                case (5):
                    initTicketJobTab();
                    break;

                case (6):
                    initBuilderTab();
                    break;
                case (7):
                    initUtilityTab();
                    break;
                default:
                    break;
            }
            Ridisegna();

        }



        /// <summary>
        /// Questo metodo riposiziona gli oggetti sulla Form
        /// </summary>
        /// <returns>Nothing</returns>
        private void Ridisegna()
        {
            try
            {
                if (indexSelectedTab == 1) // TicketTab
                {
                    listView1.SetBounds(21, 354, (frmMain.ActiveForm.Size.Width - 90), (frmMain.ActiveForm.Size.Height - 510));
                }
                if (indexSelectedTab == 2) //JobTab
                {
                    listView2.SetBounds(21, 354, (frmMain.ActiveForm.Size.Width - 90), (frmMain.ActiveForm.Size.Height - 510));
                }
                if (indexSelectedTab == 3) //Condition Tab
                {
                    listView3.SetBounds(21, 354, (frmMain.ActiveForm.Size.Width - 90), (frmMain.ActiveForm.Size.Height - 510));
                }
                if (indexSelectedTab == 4) //OperationTab
                {
                    listView4.SetBounds(21, 354, (frmMain.ActiveForm.Size.Width - 90), (frmMain.ActiveForm.Size.Height - 510));
                }
                if (indexSelectedTab == 5) //TicketJobTab
                {
                    listView4.SetBounds(21, 354, (frmMain.ActiveForm.Size.Width - 90), (frmMain.ActiveForm.Size.Height - 510));
                }
                if (indexSelectedTab == 6) //BuilderTab
                {
                    textBox8.SetBounds(dropDownTreeView1.Left, dropDownTreeView1.Top + dropDownTreeView1.Height + 4, dropDownTreeView1.Width, 194);
                    //btnSalvaCfgJob.Left = (dropDownTreeView1.Left + dropDownTreeView1.Width) - btnSalvaCfgJob.Width;
                }



                Application.DoEvents();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ridisegna:" + ex.Message);
            }

        }


        private void disponiCtrl()
        {
            textBox8.SetBounds(dropDownTreeView1.Left, dropDownTreeView1.Top + dropDownTreeView1.Height + 4, dropDownTreeView1.Width, 194);
            btnSalvaCfgJob.Left = (dropDownTreeView1.Left + dropDownTreeView1.Width) - btnSalvaCfgJob.Width;

        }

        /// <summary>
        /// Questo metodo provvede ad aggiornare un ticket all'interno del database
        /// </summary>
        private void btnModificaTicket_Click(object sender, EventArgs e)
        {


            int sqlRc = -1;
            String strSql = "";
            String strDescrizione = "";
            int countTicket = 0;

            if (textBox6.Text.IndexOf(" ") != -1)
            {
                MessageBox.Show("Il campo chiave ID TICKET non può contenere spazi!", "Aggiungi nuovo ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox6.Focus();
                return;
            }

            if (textBox6.Text.Length <= 0)
            {
                MessageBox.Show("Campo ID Ticket Obbligatorio!", "Modifica ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox6.Focus();
                return;
            }
            if (textBox4.Text.Length <= 0)
            {
                MessageBox.Show("Campo User NWS Obbligatorio!", "Modifica ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox4.Focus();
                return;
            }
            if (textBox1.Text.Length <= 0)
            {
                MessageBox.Show("Campo User GA Obbligatorio!", "Modifica ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox1.Focus();
                return;
            }

            if (comboBox1.Text.Length <= 0)
            {
                MessageBox.Show("Campo System Obbligatorio!", "Modifica ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox1.Focus();
                return;
            }


            if (textBox5.Text.Length <= 0)
            {
                MessageBox.Show("Campo Ordinamento Obbligatorio!", "Modifica ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox5.Focus();
                return;
            }

            if (textBox2.Text.Length <= 0)
            {
                MessageBox.Show("Campo Descrizione Obbligatorio!", "Modifica ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox2.Focus();
                return;
            }

            if (comboBox25.Text.Length <= 0)
            {
                MessageBox.Show("Campo Code Tipo Contr. Obbligatorio!", "Modifica ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox25.Focus();
                return;
            }




            if (comboBox5.Text.CompareTo("") == 0)
            {
                MessageBox.Show("Campo Tipo Obbligatorio!", "Modifica ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox5.Focus();
                return;
            }

            if (comboBox3.Text.CompareTo("") == 0)
            {
                MessageBox.Show("Campo Attivo Obbligatorio!", "Modifica ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox3.Focus();
                return;
            }



            try
            {
                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {


                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleCommand cmd = new OracleCommand();
                    OracleDataReader reader;


                    strDescrizione = textBox2.Text;




                    strSql = "update wp1_ticketga set ";
                    strSql += "data_emissione = :data_emissione,";
                    strSql += "data_inizio_val = :data_inizio_val,";
                    strSql += "data_fine_val = :data_fine_val,";
                    strSql += "user_nws = :user_nws,";
                    strSql += "user_ga = :user_ga,";
                    strSql += "system = :system,";
                    strSql += "descrizione = :descrizione,";
                    strSql += "ordinamento = :ordinamento,";
                    strSql += "code_tipo_contr = :code_tipo_contr,";
                    strSql += "active = :active, ";
                    strSql += "ticket_type = :ticket_type ";
                    strSql += "where id_ticket = :id_ticket ";


                    object str_p_system = null;
                    object str_p_code_tipo_contr = null;

                    if (comboBox1.Text.CompareTo("ALL") == 0)
                        str_p_system = DBNull.Value;
                    else
                        str_p_system = comboBox1.Text.ToString();

                    if (comboBox25.Text.CompareTo("null") == 0)
                        str_p_code_tipo_contr = DBNull.Value;
                    else
                        str_p_code_tipo_contr = (comboBox25.SelectedItem as ComboboxItem).Value.ToString();


                    OracleParameter[] parameters = new OracleParameter[]{
                        new OracleParameter("id_ticket",listView1.SelectedItems[0].SubItems[0].Text),
                        new OracleParameter("data_emissione",DateTime.Parse(dateTimePicker1.Value.ToString("yyyy-MM-dd"))),
                        new OracleParameter("data_inizio_val",DateTime.Parse(dateTimePicker2.Value.ToString("yyyy-MM-dd"))),
                        new OracleParameter("data_fine_val",DateTime.Parse(dateTimePicker3.Value.ToString("yyyy-MM-dd"))),
                        new OracleParameter("user_nws",textBox4.Text),
                        new OracleParameter("user_ga",textBox1.Text),
                        new OracleParameter("system",str_p_system),
                        new OracleParameter("descrizione",strDescrizione),
                        new OracleParameter("ordinamento", textBox5.Text),
                        new OracleParameter("active",comboBox3.SelectedIndex),
                        new OracleParameter("ticket_type",comboBox5.Text),
                        new OracleParameter("code_tipo_contr",str_p_code_tipo_contr)
                    };








                    try
                    {

                        //Controllo se esiste il TICKET nel DB
                        cmd.CommandText = "SELECT count(id_ticket) as conteggio  FROM wp1_ticketga where id_ticket = :idTicket ";
                        OracleParameter idTicket = new OracleParameter("idTicket", textBox6.Text);
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = cn;
                        cmd.Parameters.Add(idTicket);

                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            countTicket = Int32.Parse(reader[0].ToString());
                        }
                        reader.Close();


                        if (countTicket <= 0)
                        {
                            MessageBox.Show("Nessun Ticket presente nel database con chiave '" + textBox6.Text + "' !!", "Modifica Ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            JpubDb.Close();
                            return;

                        }


                        if (MessageBox.Show("Modificare il ticket id:" + listView1.SelectedItems[0].SubItems[0].Text + " nel database?", "Modifica ticket", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                        {
                            return;
                        }

                        cmd.CommandText = strSql;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = cn;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddRange(parameters);
                        sqlRc = cmd.ExecuteNonQuery();
                    }
                    catch (Exception exsql)
                    {
                        MessageBox.Show(exsql.Message, "Modifica Ticket - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    Console.WriteLine("SQL:" + strSql + " - RC:" + sqlRc);

                    JpubDb.Close();

                    MessageBox.Show("Modifica effettuata con successo!", "Modifica ticket", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    initTicketTab();
                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Modifica ticket - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("inserisciTicket Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }




        }
        /// <summary>
        /// Questo metodo provvede ad eliminare un ticet all'interno del database
        /// </summary>
        private void btnEliminaTicket_Click(object sender, EventArgs e)
        {

            int sqlRc = -1;
            String strSql = "";
            String strDescrizione = "";
            int countTicket = 0;

            if (listView1.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Nessun ticket selezionato dalla lista!", "Elimina ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (textBox6.Text.IndexOf(" ") != -1)
            {
                MessageBox.Show("Il campo chiave ID TICKET non può contenere spazi!", "Aggiungi nuovo ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox6.Focus();
                return;
            }





            try
            {
                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {


                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleCommand cmd = new OracleCommand();
                    OracleDataReader reader;


                    strDescrizione = textBox2.Text;




                    try
                    {

                        //Controllo se esiste il TICKET nel DB
                        cmd.CommandText = "SELECT count(id_ticket) as conteggio  FROM wp1_ticketga where id_ticket = :idTicket ";
                        OracleParameter idTicket = new OracleParameter("idTicket", textBox6.Text);
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = cn;
                        cmd.Parameters.Add(idTicket);

                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            countTicket = Int32.Parse(reader[0].ToString());
                        }
                        reader.Close();


                        if (countTicket <= 0)
                        {
                            MessageBox.Show("Nessun Ticket presente nel database con chiave '" + textBox6.Text + "' !!", "Modifica job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            JpubDb.Close();
                            return;

                        }



                        if (MessageBox.Show("Eliminare definitivamente il ticket id:" + listView1.SelectedItems[0].SubItems[0].Text + " dal database?", "Elimina ticket", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                        {
                            return;
                        }




                        strSql = "delete from wp1_instance_parameters where id_ticket = :idTicket ";
                        cmd.CommandText = strSql;
                        sqlRc = cmd.ExecuteNonQuery();

                        strSql = "delete from wp1_instance_exec where id_ticket = :idTicket ";
                        cmd.CommandText = strSql;
                        sqlRc = cmd.ExecuteNonQuery();


                        strSql = "delete from wp1_ticketga ";
                        strSql += "where id_ticket = :idTicket ";
                        cmd.CommandText = strSql;
                        sqlRc = cmd.ExecuteNonQuery();

                        MessageBox.Show("Ticket '" + textBox6.Text + "' eliminato con successo !!", "Elimina Ticket", MessageBoxButtons.OK, MessageBoxIcon.Information);



                    }
                    catch (Exception exsql)
                    {
                        MessageBox.Show(exsql.Message, "Elimina Ticket - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    Console.WriteLine("SQL:" + strSql + " - RC:" + sqlRc);

                    JpubDb.Close();

                    refreshTicketsList();
                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Elimina ticket - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("inserisciTicket Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }

        }



        /// <summary>
        /// Questo metodo inserisce un Job all'interno del database
        /// </summary>
        private void btnAggiungiJob_Click(object sender, EventArgs e)
        {
            int sqlRc = -1;
            String strSql = "";
            String strDescrizione = "";
            String dataOraCorrente = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");


            if (textBox7.Text.IndexOf(" ") != -1)
            {
                MessageBox.Show("Il campo chiave ID JOB non può contenere spazi!", "Aggiungi nuovo job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox7.Focus();
                return;
            }


            if (comboBox18.Text.Length <= 0)
            {
                MessageBox.Show("Campo Tipo Contr. Obbligatorio!", "Aggiungi nuovo job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox18.Focus();
                return;
            }


            String strCOD = (comboBox18.SelectedItem as ComboboxItem).Value.ToString();

            if (strCOD.CompareTo("ALL") == 0 || strCOD.CompareTo("null") == 0)
                strCOD = "NULL";


            if (textBox7.Text.Length <= 0)
            {
                MessageBox.Show("Campo Id Job Obbligatorio!", "Aggiungi nuovo job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox7.Focus();
                return;
            }

            if (comboBox7.Text.Length <= 0)
            {
                MessageBox.Show("Campo Mastertable Obbligatorio!", "Aggiungi nuovo job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox7.Focus();
                return;
            }



            if (comboBox6.Text.Length <= 0)
            {
                MessageBox.Show("Campo Tipo Obbligatorio!", "Aggiungi nuovo job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox6.Focus();
                return;
            }

            if (textBox3.Text.Length <= 0)
            {
                MessageBox.Show("Campo Descrizione Obbligatorio!", "Aggiungi nuovo job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox3.Focus();
                return;
            }



            if (MessageBox.Show("Inserire il nuovo job nel database?", "Aggiungi nuovo job", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }


            try
            {
                //Inserimento nuovo job
                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {


                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleCommand cmd = new OracleCommand();

                    strDescrizione = textBox3.Text;


                    strSql = "insert into wp1_job (id_job,descrizione,active,job_type,code_tipo_contr,mastertable ) values (";
                    strSql += ":id_job,:descrizione,:active,:job_type,:code_tipo_contr,:mastertable ) ";

                    object objCOD = null;
                    if (strCOD.CompareTo("NULL") == 0)
                        objCOD = DBNull.Value;
                    else
                        objCOD = strCOD.ToString();


                    OracleParameter[] parameters = new OracleParameter[]{
                                    new OracleParameter("id_job",textBox7.Text),
                                    new OracleParameter("descrizione",strDescrizione),
                                    new OracleParameter("active",comboBox4.SelectedIndex),
                                    new OracleParameter("job_type",comboBox6.Text),
                                    new OracleParameter("code_tipo_contr",objCOD),
                                    new OracleParameter("mastertable",comboBox7.Text)
                                };

                    try
                    {
                        cmd.CommandText = strSql;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = cn;
                        cmd.Parameters.AddRange(parameters);
                        sqlRc = cmd.ExecuteNonQuery();
                    }
                    catch (Exception exsql)
                    {
                        MessageBox.Show(exsql.Message, "Aggiungi nuovo job - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    Console.WriteLine("SQL:" + strSql + " - RC:" + sqlRc);

                    JpubDb.Close();

                    refreshJobsList();
                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Aggiungi nuovo job - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("inserisciJob Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }


        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (this.listView2.SelectedItems.Count <= 0)
                return;

            textBox7.Text = listView2.SelectedItems[0].SubItems[0].Text;
            comboBox7.Text = listView2.SelectedItems[0].SubItems[5].Text;
            comboBox4.SelectedIndex = Int32.Parse(listView2.SelectedItems[0].SubItems[2].Text);
            comboBox6.Text = listView2.SelectedItems[0].SubItems[3].Text;

            selectInCombo(listView2.SelectedItems[0].SubItems[4].Text, comboBox18);
            textBox3.Text = listView2.SelectedItems[0].SubItems[1].Text;


        }//listView2_SelectedIndexChanged


        /// <summary>
        /// Questo metodo carica i Ticket in una ComboBox
        /// </summary>
        /// <param name="cb">Oggetto ComboBox</param>
        private void caricaTicket(ComboBox cb)
        {

            cb.Items.Clear();

            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();
                int recCount = 0;

                cmd.CommandText = "select distinct id_ticket from wp1_ticketga  order by upper(id_ticket)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                reader = cmd.ExecuteReader();

                cbAdd(cb, "");
                while (reader.Read())
                {

                    cbAdd(cb, reader[0].ToString());
                    recCount++;
                }

                if (recCount > 0)
                {
                    cb.SelectedIndex = 0;
                }
                reader.Close();
                JpubDb.Close();
            }

        }





        /// <summary>
        /// Questo metodo carica i Job in una ComboBox
        /// </summary>
        /// <param name="cb">Oggetto ComboBox</param>
        private void caricaJob(ComboBox cb)
        {

            cb.Items.Clear();

            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();
                int recCount = 0;

                cmd.CommandText = "select distinct id_job from wp1_job  order by upper(id_job)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                reader = cmd.ExecuteReader();

                cbAdd(cb, "");
                while (reader.Read())
                {

                    cbAdd(cb, reader[0].ToString());
                    recCount++;
                }

                if (recCount > 0)
                {
                    cb.SelectedIndex = 0;
                }
                reader.Close();
                JpubDb.Close();
            }

        }

        /// <summary>
        /// Questo metodo provvede a caricare il valore code_tipo_contr in una combobox
        /// </summary>
        /// <param name="cb">Oggetto ComboBox</param>
        private void caricaCodeTipoContrJob(ComboBox cb, bool bAll)
        {

            cb.Items.Clear();

            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();
                int recCount = 0;

                cmd.CommandText = "select wp1_services.id_service , i5_1tipo_contr.desc_tipo_contr from  wp1_services left outer join i5_1tipo_contr on wp1_services.id_service = i5_1tipo_contr.code_tipo_contr  where i5_1tipo_contr.flag_sys = 'S' order by to_number(wp1_services.id_service)";


                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                reader = cmd.ExecuteReader();

                cbAdd(cb, "");
                if (bAll == true)
                    cbAdd(cb, "ALL");

                while (reader.Read())
                {
                    cbAdd(cb, reader[0].ToString() + " - " + reader[1].ToString(), reader[0].ToString());
                    recCount++;
                }

                if (recCount > 0)
                {
                    cb.SelectedIndex = 0;
                }
                reader.Close();
                JpubDb.Close();
            }

        }





        /// <summary>
        /// Questo metodo provvede a caricare il valore code_tipo_contr dato un valore di system
        /// </summary>
        /// <param name="cb">Oggetto ComboBox</param>
        /// <param name="strSystem">System name</param>
        private void caricaCodeTipoContrTicket(ComboBox cb, string strSystem, bool bTutti)
        {

            cb.Items.Clear();

            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();
                int recCount = 0;

                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                if (bTutti == true)
                    cmd.CommandText = "select wp1_services.id_service , i5_1tipo_contr.desc_tipo_contr from  wp1_services left outer join i5_1tipo_contr on wp1_services.id_service = i5_1tipo_contr.code_tipo_contr  where i5_1tipo_contr.flag_sys = 'S' order by to_number(wp1_services.id_service)";
                else
                {

                    cmd.CommandText = "select wp1_services.id_service , i5_1tipo_contr.desc_tipo_contr from  wp1_services left outer join i5_1tipo_contr on wp1_services.id_service = i5_1tipo_contr.code_tipo_contr  where i5_1tipo_contr.flag_sys = 'S'  and wp1_services.id_macro_service = :id_macro_service  order by to_number(wp1_services.id_service)";
                    OracleParameter p_id_macro_service = new OracleParameter("id_macro_service", strSystem);
                    cmd.Parameters.Add(p_id_macro_service);

                }

                reader = cmd.ExecuteReader();

                cbAdd(cb, "");
                if (cb.Name.CompareTo("comboBox18") != 0)
                    cbAdd(cb, "null");
                else
                    cbAdd(cb, "ALL");


                while (reader.Read())
                {

                    cbAdd(cb, reader[0].ToString() + " - " + reader[1].ToString(), reader[0].ToString());

                    recCount++;
                }

                if (recCount > 0)
                {
                    cb.SelectedIndex = 0;
                }
                reader.Close();
                JpubDb.Close();
            }

        }




        /// <summary>
        /// Questo metodo provvede a caricare le mastertable in una ComboBox
        /// </summary>
        /// <param name="cb">Oggetto ComboBox</param>
        private void caricaMasterTableJob(ComboBox cb)
        {

            cb.Items.Clear();

            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();
                int recCount = 0;

                cmd.CommandText = "select table_name from user_tables where table_name like 'I5_%' order by table_name";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                reader = cmd.ExecuteReader();

                cbAdd(cb, "");

                while (reader.Read())
                {

                    cbAdd(cb, reader[0].ToString());
                    recCount++;
                }

                if (recCount > 0)
                {
                    cb.SelectedIndex = 0;
                }
                reader.Close();
                JpubDb.Close();
            }

        }



        /// <summary>
        /// Questo metodo carica la lista dei parametri di Ticket->Jobs
        /// </summary>
        private void caricaParametriTicketJob(String strTicket, String strJob, String strCounter, ListView lv)
        {




            lv.Clear();
            lv.View = View.Details;
            lv.AllowColumnReorder = true;
            lv.FullRowSelect = true;
            lv.GridLines = true;
            lv.Scrollable = true;
            lv.Columns.Add("NAME_PARAM", "NAME_PARAM", 260, HorizontalAlignment.Left, 0);
            lv.Columns.Add("ID_TICKET", "ID_TICKET", 260, HorizontalAlignment.Left, 0);
            lv.Columns.Add("ID_JOB", "ID_JOB", 120, HorizontalAlignment.Left, 0);
            lv.Columns.Add("VAL_PARAM", "VAL_PARAM", 120, HorizontalAlignment.Center, 0);
            lv.Columns.Add("IS_MACRO", "IS_MACRO", 120, HorizontalAlignment.Center, 0);
            lv.Columns.Add("COUNTER", "COUNTER", 120, HorizontalAlignment.Center, 0);

            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();
                int recCount = 0;

                cmd.CommandText = "SELECT name_param,id_ticket,id_job,val_param,is_macro,counter from wp1_instance_parameters where id_ticket= :id_ticket and id_job= :id_job and counter = :counter  order by name_param";
                OracleParameter[] parameters = new OracleParameter[]{
                                    new OracleParameter("id_ticket",strTicket),
                                    new OracleParameter("id_job",strJob),
                                    new OracleParameter("counter",strCounter)
                                };
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddRange(parameters);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine("Letto:" + reader[0]);
                    ListViewItem i = new ListViewItem(reader[0].ToString());
                    i.SubItems.Add(reader[1].ToString());
                    i.SubItems.Add(reader[2].ToString());
                    i.SubItems.Add(reader[3].ToString());
                    i.SubItems.Add(reader[4].ToString());
                    i.SubItems.Add(reader[5].ToString());
                    lv.Items.Add(i);

                    recCount++;
                }

                if (recCount > 0)
                {
                    lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                }

                reader.Close();

                JpubDb.Close();

                lv.Show();
                lv.Refresh();

            }
            else
            {
                MessageBox.Show(JpubDb.getLastError(), "Errore Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                JpubDb = null;
            }


        }


        /// <summary>
        /// Questo metodo carica i dati del Job selezionato nel caso vi sia presente un associazione
        /// </summary>
        private void caricaDatiJobSelezionato(ComboBox cbTicket, ComboBox cbJob)
        {

            JpubOraDb JpubDb = new JpubOraDb();
            String strMaxCounter = "0";
            int nMaxCounter = 0;


            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();

                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;


                //cmd.CommandText = "SELECT max(counter) as massimo from wp1_instance_exec where id_ticket = '" + cbTicket.Text + "' and id_job = '" + cbJob.Text  + "' ";
                cmd.CommandText = "SELECT max(counter) as massimo from wp1_instance_exec where id_ticket = :id_ticket and id_job = :id_job ";
                OracleParameter[] parameters = new OracleParameter[]{
                                    new OracleParameter("id_ticket",cbTicket.Text),
                                    new OracleParameter("id_job",cbJob.Text)
                                };


                cmd.Parameters.AddRange(parameters);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    strMaxCounter = reader[0].ToString();
                }
                reader.Close();
                if (strMaxCounter.CompareTo("") == 0)
                    strMaxCounter = "0";

                nMaxCounter = Int32.Parse(strMaxCounter);
                nMaxCounter++;

                JpubDb.Close();
            }
            else
            {
                MessageBox.Show(JpubDb.getLastError(), "Errore Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                JpubDb = null;
            }


        }






        /// <summary>
        /// Questo metodo carica i Job  a seconda della tipologia di Ticket Selezionato
        /// </summary>
        private void caricaJobDelTicket(ComboBox cbTicket, ComboBox cbJob)
        {

            cbJob.Items.Clear();

            JpubOraDb JpubDb = new JpubOraDb();
            String strTipoTicket = "";
            String strMaxCounter = "";
            int nMaxCounter = 0;

            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();
                int recCount = 0;


                OracleParameter id_ticket = new OracleParameter("id_ticket", cbTicket.Text);
                cmd.Parameters.Add(id_ticket);

                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                //Ricavo il tipo di ticket
                cmd.CommandText = "SELECT ticket_type from wp1_ticketga where id_ticket = :id_ticket ";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    strTipoTicket = reader[0].ToString();
                }
                reader.Close();
                textBox14.Text = strTipoTicket;

                //Ricavo il max counter 
                cmd.CommandText = "SELECT max(counter) as massimo from wp1_instance_exec where id_ticket = :id_ticket ";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    strMaxCounter = reader[0].ToString();
                }
                reader.Close();
                if (strMaxCounter.CompareTo("") == 0)
                    strMaxCounter = "-1";

                nMaxCounter = Int32.Parse(strMaxCounter);
                nMaxCounter++;


                //Ricavo i job di tipo nn
                OracleParameter job_type = new OracleParameter("job_type", strTipoTicket);
                cmd.CommandText = "SELECT id_job from wp1_job where job_type= :job_type order by upper(id_job)";
                cmd.Parameters.Clear();
                cmd.Parameters.Add(job_type);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cbAdd(cbJob, reader[0].ToString());
                    recCount++;
                }

                if (recCount > 0)
                {
                    cbJob.SelectedIndex = 0;
                }
                reader.Close();

                JpubDb.Close();
            }
            else
            {
                MessageBox.Show(JpubDb.getLastError(), "Errore Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                JpubDb = null;
            }


        }






        /// <summary>
        /// Questo metodo carica il combo Tabella del Tab Conditions
        /// </summary>
        private void caricaField(ComboBox cb)
        {

            cb.Items.Clear();

            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                Cursor.Current = Cursors.WaitCursor;

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();
                int recCount = 0;

                cmd.CommandText = "select table_name from user_tables where table_name like 'I5_%'  order by table_name";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                reader = cmd.ExecuteReader();

                cbAdd(cb, "");
                while (reader.Read())
                {

                    cbAdd(cb, reader[0].ToString());
                    recCount++;
                }

                if (recCount > 0)
                {
                    cb.SelectedIndex = 0;
                }
                reader.Close();
                JpubDb.Close();
                Cursor.Current = Cursors.Default;
            }

        }


        /// <summary>
        /// Questo metodo carica il combo cb con le colonne della tabella strTableName
        /// </summary>
        private void caricaColumns(ComboBox cb, string strTableName)
        {

            cb.Items.Clear();

            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();
                int recCount = 0;

                OracleParameter table_name = new OracleParameter("table_name", strTableName);
                cmd.CommandText = "Select COLUMN_NAME from user_tab_columns where table_name= :table_name order by COLUMN_NAME";
                cmd.Connection = cn;
                cmd.Parameters.Add(table_name);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {

                    cbAdd(cb, reader[0].ToString());
                    recCount++;
                }

                if (recCount > 0)
                {
                    cb.SelectedIndex = 0;
                }
                reader.Close();
                JpubDb.Close();
            }

        }





        /// <summary>
        /// Questo metodo carica il combo Field del Tab Operations
        /// </summary>
        private void caricaFieldOperations(ComboBox cb)
        {

            cb.Items.Clear();

            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();
                int recCount = 0;

                cmd.CommandText = "select distinct field from wp1_operations order by field";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cbAdd(cb, reader[0].ToString());
                    recCount++;
                }

                if (recCount > 0)
                {
                    cb.SelectedIndex = 0;
                }
                reader.Close();
                JpubDb.Close();
            }

        }//caricaFieldOperations




        /// <summary>
        /// Questo metodo carica il combo Operands del Tab Conditions
        /// </summary>
        private void caricaOperandsConditions(ComboBox cb)
        {

            cb.Items.Clear();

            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();
                int recCount = 0;

                cmd.CommandText = "select distinct operands from wp1_conditions order by operands";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                cbAdd(cb, "");
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cbAdd(cb, reader[0].ToString());
                    recCount++;
                }

                if (recCount > 0)
                {
                    cb.SelectedIndex = 0;
                }
                reader.Close();
                JpubDb.Close();
            }

        }//caricaOperandsConditions


        /// <summary>
        /// Questo metodo carica il combo Operands del Tab Operations
        /// </summary>
        private void caricaOperandsOperations(ComboBox cb)
        {

            cb.Items.Clear();

            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();
                int recCount = 0;

                cmd.CommandText = "select distinct operand from wp1_operations order by operand";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                reader = cmd.ExecuteReader();
                cbAdd(cb, "");

                while (reader.Read())
                {
                    cbAdd(cb, reader[0].ToString());
                    recCount++;
                }

                if (recCount > 0)
                {
                    cb.SelectedIndex = 0;
                }
                reader.Close();
                JpubDb.Close();
            }

        }//caricaOperandsOperations




        /// <summary>
        /// Questo metodo carica un valore in un oggetto ComboBox
        /// </summary>
        private void cbAdd(ComboBox cb, String strDati)
        {
            ComboboxItem item = new ComboboxItem();
            item.Text = strDati;
            item.Value = strDati;
            cb.Items.Add(item);
        }

        /// <summary>
        /// Questo metodo carica un valore in un oggetto ComboBox differenziando item e text
        /// </summary>
        private void cbAdd(ComboBox cb, String strTesto, String strValue)
        {
            ComboboxItem item = new ComboboxItem();
            item.Text = strTesto;
            item.Value = strValue;
            cb.Items.Add(item);
        }

        /// <summary>
        /// Questo metodo carica il combo Operator del Tab Conditions
        /// </summary>
        private void caricaOperatorConditions(ComboBox cb)
        {

            cb.Items.Clear();
            cbAdd(cb, "");
            cbAdd(cb, "=");
            cbAdd(cb, ">");
            cbAdd(cb, "<");
            cbAdd(cb, "<=");
            cbAdd(cb, ">=");
            cbAdd(cb, "!=");
            cbAdd(cb, "RAWSTATEMENT");
            cbAdd(cb, "COMPLEX");
            //cbAdd(cb, "POSTCOMPLEX");
            //cbAdd(cb, "IN");
            cbAdd(cb, "LIKE");
            //cbAdd(cb, "NOT IN");
            cb.SelectedIndex = 0;


        }//caricOperatorConditions

        /// <summary>
        /// Questo metodo carica Operator generici dato l'oggetto ComboBox
        /// </summary>
        private void caricaOperator(ComboBox cb)
        {

            cb.Items.Clear();
            cbAdd(cb, "");
            cbAdd(cb, "SET");
            cbAdd(cb, "COMPLEX");
            cbAdd(cb, "RAWSTATEMENT");
            cbAdd(cb, "DELETE");
            cbAdd(cb, "POSTCOMPLEX");
            cb.SelectedIndex = 0;


        }//caricOperator


        /// <summary>
        /// Questo metodo carica il combo Operator del Tab Operations
        /// </summary>
        private void caricaOperatorOperations(ComboBox cb)
        {

            cb.Items.Clear();
            cbAdd(cb, "SET");
            cbAdd(cb, "RAWSTATEMENT");
            cb.SelectedIndex = 0;

        }//caricOperatorOperations


        /// <summary>
        /// Questo metodo carica il combo FieldType del Tab Conditions
        /// </summary>
        private void caricFieldTypeConditions(ComboBox cb)
        {

            cb.Items.Clear();
            cbAdd(cb, "");
            cbAdd(cb, "DATE");
            cbAdd(cb, "NUMBER");
            cbAdd(cb, "STRING");
            cbAdd(cb, "TABLE");
            cb.SelectedIndex = 0;


        }//caricFieldTypeConditions


        /// <summary>
        /// Questo metodo inizializza il Tab Conditions
        /// </summary>
        private void initConditionsTab()
        {

            Cursor.Current = Cursors.WaitCursor;

            comboBox8.Width = 472;

            textBox9.Text = "";
            comboBox8.Items.Clear();
            comboBox9.Items.Clear();
            comboBox10.Items.Clear();
            comboBox11.Items.Clear();
            comboBox12.Items.Clear();
            comboBox2.Items.Clear();
            comboBox2.Text = "";

            caricaField(comboBox8);
            caricaOperatorConditions(comboBox9);
            caricaOperandsConditions(comboBox10);
            caricFieldTypeConditions(comboBox11);

            cbAdd(comboBox12, "");
            cbAdd(comboBox12, "ALL");
            cbAdd(comboBox12, "PREVAL");
            cbAdd(comboBox12, "POSTVAL");
            cbAdd(comboBox12, "SAP");
            cbAdd(comboBox12, "TREND");
            comboBox12.SelectedIndex = 0;

            label49.Visible = false;
            label50.Visible = false;
            label51.Visible = false;
            comboBox24.Visible = false;
            comboBox23.Visible = false;



            refreshConditionsList();

            Cursor.Current = Cursors.Default;


        }

        /// <summary>
        /// Questo metodo inizializza il Tab Operations
        /// </summary>
        private void initOperationsTab()
        {

            Cursor.Current = Cursors.WaitCursor;

            textBox10.Text = "";
            comboBox3.Items.Clear();
            comboBox15.Items.Clear();
            comboBox16.Items.Clear();
            comboBox13.Items.Clear();
            comboBox17.Items.Clear();

            caricaField(comboBox17);
            caricaOperator(comboBox16);
            caricaOperandsOperations(comboBox15);

            cbAdd(comboBox13, "");
            cbAdd(comboBox13, "PREVAL");
            cbAdd(comboBox13, "POSTVAL");
            cbAdd(comboBox13, "ALL");
            cbAdd(comboBox13, "SAP");
            cbAdd(comboBox13, "TREND");
            comboBox13.SelectedIndex = 0;

            comboBox16.Text = "";
            comboBox13.Text = "";



            refreshOperationsList();

            Cursor.Current = Cursors.Default;


        }//initOperationsTab

        private void listView3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView3.SelectedItems.Count <= 0)
                return;


            try
            {

                textBox9.Text = listView3.SelectedItems[0].SubItems[0].Text;
                comboBox9.Text = listView3.SelectedItems[0].SubItems[2].Text;
                comboBox10.Text = listView3.SelectedItems[0].SubItems[3].Text;
                comboBox11.Text = listView3.SelectedItems[0].SubItems[4].Text;
                comboBox12.Text = listView3.SelectedItems[0].SubItems[5].Text;

                string strField = listView3.SelectedItems[0].SubItems[1].Text;
                string strField2 = listView3.SelectedItems[0].SubItems[3].Text;

                if (listView3.SelectedItems[0].SubItems[2].Text.CompareTo("COMPLEX") == 0)
                {
                    comboBox8.Text = strField;
                }
                else
                {
                    if (strField.IndexOf(".") != -1)
                    {
                        string[] arField = strField.Split('.');
                        comboBox8.Text = arField[0];
                        comboBox2.Text = arField[1];
                    }
                    else
                    {
                        comboBox8.Text = strField;
                    }


                    if (listView3.SelectedItems[0].SubItems[4].Text.CompareTo("TABLE") == 0)
                    {
                        string[] arField2 = strField2.Split('.');
                        comboBox24.Text = arField2[0];
                        comboBox23.Text = arField2[1];
                    }


                }

            }
            catch (Exception ex)
            {
            }


        }

        private void listView4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView4.SelectedItems.Count <= 0)
                return;

            string strField = listView4.SelectedItems[0].SubItems[1].Text;






            textBox10.Text = listView4.SelectedItems[0].SubItems[0].Text;
            comboBox16.Text = listView4.SelectedItems[0].SubItems[2].Text;
            comboBox15.Text = listView4.SelectedItems[0].SubItems[3].Text;
            comboBox13.Text = listView4.SelectedItems[0].SubItems[4].Text;

            if (listView4.SelectedItems[0].SubItems[2].Text.CompareTo("COMPLEX") != 0 && listView4.SelectedItems[0].SubItems[2].Text.CompareTo("POSTCOMPLEX") != 0)
            {
                if (strField.IndexOf(".") != -1)
                {
                    string[] arField = strField.Split('.');
                    comboBox17.Text = arField[0];
                    comboBox14.Text = arField[1];
                }
                else
                {
                    comboBox17.Text = "";
                    comboBox14.Text = strField;
                }
            }
            else
            {
                comboBox17.Text = strField;
            }


        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {

            /*
            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {
                Console.WriteLine("Connessione eseguita con successo!");
                JpubDb.Close();

            }*/




        }

        /// <summary>
        /// Questo metodo inserisce una condition all'interno del database
        /// </summary>
        private void btnAggiungiCondition_Click(object sender, EventArgs e)
        {
            int sqlRc = -1;
            String strSql = "";
            String dataOraCorrente = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");
            string strField = "";



            if (textBox9.Text.IndexOf(" ") != -1)
            {
                MessageBox.Show("Il campo chiave ID CONDITION non può contenere spazi!", "Aggiungi nuova fscondition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox9.Focus();
                return;
            }

            if (textBox9.Text.Length <= 0)
            {
                MessageBox.Show("Campo ID Condition Obbligatorio!", "Aggiungi nuova Condition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox9.Focus();
                return;
            }
            if (comboBox8.SelectedIndex < 0)
            {
                MessageBox.Show("Campo Field Obbligatorio!", "Aggiungi nuova Condition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox8.Focus();
                return;
            }
            if (comboBox9.Text.CompareTo("") == 0)
            {
                MessageBox.Show("Campo Operator Obbligatorio!", "Aggiungi nuova Condition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox9.Focus();
                return;
            }
            if (comboBox10.Text.Length <= 0)
            {
                MessageBox.Show("Campo Operand Obbligatorio!", "Aggiungi nuova Condition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox10.Focus();
                return;
            }
            if (comboBox11.Text.Length <= 0)
            {
                MessageBox.Show("Campo Field Type Obbligatorio!", "Aggiungi nuova Condition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox11.Focus();
                return;
            }

            if (comboBox12.Text.CompareTo("") == 0)
            {
                MessageBox.Show("Campo Cond Type Obbligatorio!", "Aggiungi nuova Condition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox12.Focus();
                return;
            }







            if (comboBox11.Text.CompareTo("DATE") == 0)
            {

                if (comboBox10.Text.Substring(0, 1).CompareTo("$") != 0 && comboBox10.Text.Substring(comboBox10.Text.Length - 1, 1).CompareTo("$") != 0)
                    if (IsDateTime(comboBox10.Text) == false)
                    {
                        MessageBox.Show("Data non valida nel campo Operands!\nControllare la validità della data. ", "Aggiungi nuova Condition", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        comboBox10.Focus();
                        return;
                    }

            }


            if (MessageBox.Show("Inserire la nuova Condition nel database?", "Aggiungi nuova Condition", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }



            try
            {
                //Inserimento nuova Condition
                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {
                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleCommand cmd = new OracleCommand();
                    strField = comboBox10.Text;
                    strField = strField;


                    string s_field = "";
                    object obj_operands = null;


                    if (comboBox9.Text.CompareTo("COMPLEX") == 0)
                    {
                        s_field = comboBox8.Text;
                    }
                    else
                    {

                        if (comboBox2.Text.Length <= 0)
                            s_field = comboBox8.Text;
                        else
                            s_field = comboBox8.Text + "." + comboBox2.Text;
                    }


                    obj_operands = strField.ToString();


                    strSql = "insert into wp1_conditions (id_condition,field,operator,operands,field_type,cond_type ) values (";
                    strSql += ":id_condition,:field,:operator,:operands,:field_type,:cond_type )";
                    OracleParameter[] parameters = new OracleParameter[]{
                                    new OracleParameter("id_condition",textBox9.Text),
                                    new OracleParameter("field",s_field),
                                    new OracleParameter("operator",comboBox9.Text),
                                    new OracleParameter("operands",obj_operands),
                                    new OracleParameter("field_type",comboBox11.Text),
                                    new OracleParameter("cond_type",comboBox12.Text)
                                };


                    try
                    {
                        cmd.CommandText = strSql;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = cn;
                        cmd.Parameters.AddRange(parameters);
                        sqlRc = cmd.ExecuteNonQuery();

                        MessageBox.Show("Record inserito con successo!", "Aggiungi nuova Condition", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (OracleException ex)
                    {
                        MessageBox.Show(ex.Message, "Aggiungi nuova Condition - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    Console.WriteLine("SQL:" + strSql + " - RC:" + sqlRc);

                    JpubDb.Close();
                    refreshConditionsList();
                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Aggiungi nuova Condition - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Aggiungi Condition Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }

        }

        /// <summary>
        /// Questo metodo provvede ad aggiornare una condition all'interno del database
        /// </summary>
        private void btnModificaCondition_Click(object sender, EventArgs e)
        {
            int sqlRc = -1;
            String strSql = "";
            String dataOraCorrente = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");


            if (textBox9.Text.IndexOf(" ") != -1)
            {
                MessageBox.Show("Il campo chiave ID CONDITION non può contenere spazi!", "Modifica condition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox9.Focus();
                return;
            }



            if (textBox9.Text.IndexOf(" ") != -1)
            {
                MessageBox.Show("Il campo chiave ID CONDITION non può contenere spazi!", "Modifica  fscondition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox9.Focus();
                return;
            }

            if (textBox9.Text.Length <= 0)
            {
                MessageBox.Show("Campo ID Condition Obbligatorio!", "Modifica Condition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox9.Focus();
                return;
            }
            if (comboBox8.SelectedIndex < 0)
            {
                MessageBox.Show("Campo Field Obbligatorio!", "Modifica Condition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox8.Focus();
                return;
            }
            if (comboBox9.Text.CompareTo("") == 0)
            {
                MessageBox.Show("Campo Operator Obbligatorio!", "Modifica Condition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox9.Focus();
                return;
            }
            if (comboBox10.Text.Length <= 0)
            {
                MessageBox.Show("Campo Operand Obbligatorio!", "Modifica Condition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox10.Focus();
                return;
            }
            if (comboBox11.Text.Length <= 0)
            {
                MessageBox.Show("Campo Field Type Obbligatorio!", "Aggiungi nuova Condition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox11.Focus();
                return;
            }

            if (comboBox12.Text.CompareTo("") == 0)
            {
                MessageBox.Show("Campo Cond Type Obbligatorio!", "Modifica Condition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox12.Focus();
                return;
            }



            if (comboBox11.Text.CompareTo("DATE") == 0)
            {

                if (comboBox10.Text.Substring(0, 1).CompareTo("$") == 0 && comboBox10.Text.Substring(comboBox10.Text.Length - 1, 1).CompareTo("$") == 0)
                {
                }
                else
                {
                    if (IsDateTime(comboBox10.Text) == false)
                    {
                        MessageBox.Show("Data non valida nel campo Operands!\nControllare la validità della data. ", "Aggiungi nuova Condition", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        comboBox10.Focus();
                        return;
                    }
                }

            }




            try
            {
                //Aggiornamento Condition
                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {
                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleCommand cmd = new OracleCommand();
                    OracleDataReader reader;
                    int countCondition = 0;

                    string strField = comboBox10.Text;
                    strField = strField;

                    strSql = "update wp1_conditions set ";
                    strSql += "field = :field,";
                    strSql += "operator = :operator,";
                    strSql += "operands = :operands,";
                    strSql += "field_type = :field_type,";
                    strSql += "cond_type = :cond_type ";
                    strSql += " where id_condition = :id_condition ";


                    string s_field = "";
                    object obj_operands = null;

                    if (comboBox9.Text.CompareTo("COMPLEX") == 0)
                    {
                        s_field = comboBox8.Text;
                    }
                    else
                    {

                        if (comboBox2.Text.Length <= 0)
                            s_field = comboBox8.Text;
                        else
                            s_field = comboBox8.Text + "." + comboBox2.Text;
                    }

                    obj_operands = strField.ToString();

                    OracleParameter[] parameters = new OracleParameter[]{
                                    new OracleParameter("id_condition",textBox9.Text),
                                    new OracleParameter("field",s_field),
                                    new OracleParameter("operator",comboBox9.Text),
                                    new OracleParameter("operands",obj_operands),
                                    new OracleParameter("field_type",comboBox11.Text),
                                    new OracleParameter("cond_type",comboBox12.Text)
                                };

                    try
                    {


                        //Controllo se esiste la Condition nel DB
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = cn;
                        OracleParameter id_condition = new OracleParameter("id_condition", textBox9.Text);
                        cmd.Parameters.Add(id_condition);
                        cmd.CommandText = "SELECT count(id_condition) as conteggio  FROM wp1_conditions where id_condition = :id_condition ";

                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            countCondition = Int32.Parse(reader[0].ToString());
                        }
                        reader.Close();


                        if (countCondition <= 0)
                        {
                            MessageBox.Show("Nessuna Condition presente nel database con chiave '" + textBox9.Text + "' !!", "Modifica Condition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            JpubDb.Close();
                            return;

                        }




                        if (MessageBox.Show("Aggiornare la Condition nel database?", "Modifica Condition", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                        {
                            JpubDb.Close();
                            return;
                        }



                        cmd.CommandText = strSql;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddRange(parameters);
                        sqlRc = cmd.ExecuteNonQuery();

                        MessageBox.Show("Aggiornamento effettuato con successo!", "Modifica Condition", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Console.WriteLine("SQL:" + strSql + " - RC:" + sqlRc);

                        JpubDb.Close();
                        refreshConditionsList();
                    }
                    catch (OracleException ex)
                    {
                        MessageBox.Show(ex.Message, "Aggiungi nuova Condition - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Aggiungi nuova Condition - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Modifica Condition Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }
        }

        /// <summary>
        /// Questo metodo elimina una condition all'interno del database
        /// </summary>
        private void btnEliminaCondition_Click(object sender, EventArgs e)
        {
            int sqlRc = -1;
            String strSql = "";
            String dataOraCorrente = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");
            int countCondition = 0;

            if (textBox9.Text.Length <= 0)
            {
                MessageBox.Show("Campo ID Condition Obbligatorio!", "Elimina Condition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox9.Focus();
                return;
            }







            try
            {
                //Elimina Condition
                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {
                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleCommand cmd = new OracleCommand();
                    OracleDataReader reader;


                    //Controllo se esiste la Condition nel DB
                    OracleParameter idTicket = new OracleParameter("id_condition", textBox9.Text);
                    cmd.CommandText = "SELECT count(id_condition) as conteggio  FROM wp1_conditions where id_condition = :id_condition ";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = cn;
                    cmd.Parameters.Add(idTicket);

                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        countCondition = Int32.Parse(reader[0].ToString());
                    }
                    reader.Close();


                    if (countCondition <= 0)
                    {
                        MessageBox.Show("Nessuna Condition presente nel database con chiave '" + textBox9.Text + "' !!", "Modifica Condition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        JpubDb.Close();
                        return;

                    }

                    if (MessageBox.Show("Attenzione! Eliminando la condition selezionata verranno eliminate anche le configurazioni job ad essa associate.\nEliminare la Condition " + textBox9.Text + " dal database?", "Elimina Condition", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                    {
                        JpubDb.Close();
                        return;
                    }



                    try
                    {
                        strSql = "delete from wp1_job_condition where id_condition = :id_condition";
                        cmd.CommandText = strSql;
                        sqlRc = cmd.ExecuteNonQuery();

                        strSql = "delete from wp1_conditions ";
                        strSql += " where id_condition = :id_condition ";

                        cmd.CommandText = strSql;
                        sqlRc = cmd.ExecuteNonQuery();
                        MessageBox.Show("Condition eliminata con successo!", "Elimina Condition", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    catch (OracleException ex)
                    {
                        MessageBox.Show(ex.Message, "Elimina Condition - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }


                    Console.WriteLine("SQL:" + strSql + " - RC:" + sqlRc);

                    JpubDb.Close();
                    refreshConditionsList();
                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Elimina Condition - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Elimina Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }
        }//btnEliminaCondition_Click






        /// <summary>
        /// Questo metodo inseriscie una operation all'interno del database
        /// </summary>
        private void btnAggiungiOperation_Click(object sender, EventArgs e)
        {
            int sqlRc = -1;
            String strSql = "";
            String dataOraCorrente = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");
            int countOperation = 0;





            if (textBox10.Text.Length <= 0)
            {
                MessageBox.Show("Campo ID Operation Obbligatorio!", "Aggiungi nuova Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox10.Focus();
                return;
            }

            if (textBox10.Text.IndexOf(" ") != -1)
            {
                MessageBox.Show("Il campo chiave ID OPERATION non può contenere spazi!", "Aggiungi nuova Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox10.Focus();
                return;
            }

            if (comboBox16.Text.Length <= 0)
            {
                MessageBox.Show("Campo Operator Obbligatorio!", "Aggiungi nuova Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox16.Focus();
                return;
            }


            if (comboBox16.Text.CompareTo("DELETE") != 0)
            {
                if (comboBox17.SelectedIndex < 0)
                {
                    MessageBox.Show("Campo Table Obbligatorio!", "Aggiungi nuova Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    comboBox17.Focus();
                    return;
                }

                if (comboBox17.Text.Length <= 0)
                {
                    MessageBox.Show("Campo Table Obbligatorio!", "Aggiungi nuova Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    comboBox17.Focus();
                    return;
                }

                if (comboBox14.SelectedIndex < 0 && label33.Text.CompareTo("Table") == 0)
                {
                    MessageBox.Show("Campo Field Obbligatorio!", "Aggiungi nuova Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    comboBox14.Focus();
                    return;
                }
            }


            if (comboBox16.SelectedIndex < 0)
            {
                MessageBox.Show("Campo Operator Obbligatorio!", "Aggiungi nuova Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox16.Focus();
                return;
            }
            if (comboBox15.Text.Length <= 0)
            {
                MessageBox.Show("Campo Operand Obbligatorio!", "Aggiungi nuova Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox15.Focus();
                return;
            }
            if (comboBox13.SelectedIndex < 0)
            {
                MessageBox.Show("Campo Op Type Obbligatorio!", "Aggiungi nuova Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox13.Focus();
                return;
            }


            if (comboBox13.Text.Length <= 0)
            {
                MessageBox.Show("Campo Op Type Obbligatorio!", "Aggiungi nuova Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox13.Focus();
                return;
            }

            try
            {
                //Inserimento nuova Operation
                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {
                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleCommand cmd = new OracleCommand();
                    OracleDataReader reader;


                    string strField = comboBox15.Text;
                    strField = strField;

                    strSql = "insert into wp1_operations (id_operation,field,operator,operand,op_type ) values (";
                    strSql += ":id_operation,:field,:operator,:operand,:op_type ) ";

                    string s_field = "";
                    if (comboBox14.Text.Length <= 0)
                        s_field = comboBox17.Text;
                    else
                        s_field = comboBox17.Text + "." + comboBox14.Text;

                    OracleParameter[] parameters = new OracleParameter[]{
                                    new OracleParameter("id_operation",textBox10.Text),
                                    new OracleParameter("field",s_field),
                                    new OracleParameter("operator",comboBox16.Text),
                                    new OracleParameter("operand",strField),
                                    new OracleParameter("op_type",comboBox13.Text)
                                };

                    try
                    {

                        if (MessageBox.Show("Inserire la nuova Operation nel database?", "Aggiungi nuova Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                        {
                            JpubDb.Close();
                            return;
                        }

                        cmd.CommandText = strSql;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = cn;
                        cmd.Parameters.AddRange(parameters);
                        sqlRc = cmd.ExecuteNonQuery();

                        MessageBox.Show("Record inserito con successo!", "Aggiungi nuova Operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception exsql)
                    {
                        MessageBox.Show(exsql.Message, "Aggiungi nuova Operation - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    Console.WriteLine("SQL:" + strSql + " - RC:" + sqlRc);

                    JpubDb.Close();
                    refreshOperationsList();
                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Aggiungi nuova Operation - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Aggiungi Operation Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }
        }



        /// <summary>
        /// Questo metodo provvede ad aggiornare una operation all'interno del database
        /// </summary>
        private void btnModificaOperation_Click(object sender, EventArgs e)
        {
            int sqlRc = -1;
            String strSql = "";
            String dataOraCorrente = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");
            int countOperation = 0;



            if (textBox10.Text.Length <= 0)
            {
                MessageBox.Show("Campo ID Operation Obbligatorio!", "Modifica Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox10.Focus();
                return;
            }
            if (textBox10.Text.IndexOf(" ") != -1)
            {
                MessageBox.Show("Il campo chiave ID OPERATION non può contenere spazi!", "Aggiungi nuova Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox10.Focus();
                return;
            }


            if (comboBox16.SelectedIndex < 0)
            {
                MessageBox.Show("Campo Operator Obbligatorio!", "Modifica Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox16.Focus();
                return;
            }

            if (comboBox16.Text.CompareTo("DELETE") != 0)
            {
                if (comboBox17.SelectedIndex < 0)
                {
                    MessageBox.Show("Campo Table Obbligatorio!", "Aggiungi nuova Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    comboBox17.Focus();
                    return;
                }

                if (comboBox17.Text.Length <= 0)
                {
                    MessageBox.Show("Campo Table Obbligatorio!", "Aggiungi nuova Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    comboBox17.Focus();
                    return;
                }

                if (comboBox14.SelectedIndex < 0 && label33.Text.CompareTo("Table") == 0)
                {
                    MessageBox.Show("Campo Field Obbligatorio!", "Aggiungi nuova Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    comboBox14.Focus();
                    return;
                }
            }





            if (comboBox15.Text.Length <= 0)
            {
                MessageBox.Show("Campo Operand Obbligatorio!", "Modifica Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox15.Focus();
                return;
            }
            if (comboBox13.SelectedIndex < 0)
            {
                MessageBox.Show("Campo Op Type Obbligatorio!", "Aggiungi nuova Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox13.Focus();
                return;
            }

            if (comboBox13.Text.Length <= 0)
            {
                MessageBox.Show("Campo Op Type Obbligatorio!", "Aggiungi nuova Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox13.Focus();
                return;
            }

            if (MessageBox.Show("Modificare l'operation " + textBox10.Text + " nel database?", "Modifica Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            try
            {
                //Aggiornamento Operation
                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {
                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleCommand cmd = new OracleCommand();
                    OracleDataReader reader;

                    string strField = comboBox15.Text;
                    strField = strField;

                    string s_field = "";
                    if (comboBox14.Text.Length <= 0)
                        s_field = comboBox17.Text;
                    else
                        s_field = comboBox17.Text + "." + comboBox14.Text;

                    OracleParameter[] parameters = new OracleParameter[]{
                                    new OracleParameter("id_operation",textBox10.Text),
                                    new OracleParameter("field",s_field),
                                    new OracleParameter("operator",comboBox16.Text),
                                    new OracleParameter("operand",strField),
                                    new OracleParameter("op_type",comboBox13.Text)
                                };

                    strSql = "update wp1_operations set ";
                    strSql += " field = :field ,";
                    strSql += "operator = :operator,";
                    strSql += "operand = :operand,";
                    strSql += "op_type = :op_type ";
                    strSql += " where id_operation = :id_operation ";


                    OracleParameter id_operation = new OracleParameter("id_operation", textBox10.Text);

                    try
                    {

                        //Controllo se esiste la Operation nel DB
                        cmd.CommandText = "SELECT count(id_operation) as conteggio  FROM wp1_operations where id_operation = :id_operation ";
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(id_operation);
                        cmd.Connection = cn;

                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            countOperation = Int32.Parse(reader[0].ToString());
                        }
                        reader.Close();


                        if (countOperation <= 0)
                        {
                            MessageBox.Show("Nessuna Operation presente nel database con chiave '" + textBox10.Text + "' !!", "Modifica Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            JpubDb.Close();
                            return;

                        }




                        cmd.CommandText = strSql;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddRange(parameters);
                        sqlRc = cmd.ExecuteNonQuery();
                    }
                    catch (Exception exsql)
                    {
                        MessageBox.Show(exsql.Message, "Modifica Operation - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    Console.WriteLine("SQL:" + strSql + " - RC:" + sqlRc);

                    MessageBox.Show("Aggiornamento effettuato con successo!", "Modifica Operation", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    JpubDb.Close();
                    refreshOperationsList();
                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Aggiungi nuova Operation - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Modifica Operation Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }
        }

        /// <summary>
        /// Questo metodo provvede ad eliminare una operation all'interno del database
        /// </summary>
        private void btnEliminaOperation_Click(object sender, EventArgs e)
        {
            int sqlRc = -1;
            int countOperation = 0;
            String strSql = "";
            String dataOraCorrente = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");


            if (textBox10.Text.Length <= 0)
            {
                MessageBox.Show("Campo ID Operation Obbligatorio!\nInserire l\'Id Operation oppure selezionare l\'item dalla griglia e riprovare.", "Elimina Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox10.Focus();
                return;
            }



            if (MessageBox.Show("Attenzione! Eliminando la operation selezionata verranno eliminate anche le configurazioni job ad essa associate.\nEliminare la Operation " + textBox10.Text + " nel database?", "Elimina Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }



            try
            {
                //Elimina Condition
                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {
                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleCommand cmd = new OracleCommand();
                    OracleDataReader reader;

                    OracleParameter id_operation = new OracleParameter("id_operation", textBox10.Text);


                    //Controllo se esiste la Operation nel DB
                    cmd.CommandText = "SELECT count(id_operation) as conteggio  FROM wp1_operations where id_operation = :id_operation ";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = cn;
                    cmd.Parameters.Add(id_operation);


                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        countOperation = Int32.Parse(reader[0].ToString());
                    }
                    reader.Close();


                    if (countOperation <= 0)
                    {
                        MessageBox.Show("Nessuna Operation presente nel database con chiave '" + textBox10.Text + "' !!", "Modifica Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        JpubDb.Close();
                        return;

                    }


                    try
                    {
                        strSql = "delete from wp1_job_operation where id_operation = :id_operation ";
                        cmd.CommandText = strSql;
                        sqlRc = cmd.ExecuteNonQuery();

                        strSql = "delete from wp1_operations ";
                        strSql += " where id_operation = :id_operation ";

                        cmd.CommandText = strSql;
                        sqlRc = cmd.ExecuteNonQuery();

                        MessageBox.Show("Operation eliminata con successo!", "Elimina Operation", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    catch (Exception exsql)
                    {
                        MessageBox.Show(exsql.Message, "Elimina Operation - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }


                    Console.WriteLine("SQL:" + strSql + " - RC:" + sqlRc);

                    JpubDb.Close();
                    refreshOperationsList();
                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Elimina Condition - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Elimina Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }
        }

        private void configurazioneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frmCfg = new Config();
            frmCfg.ShowDialog(this);

        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox9.Text.CompareTo("COMPLEX") != 0)
                caricaColumns(comboBox2, comboBox8.Text);

        }

        private void btnCopia_Click(object sender, EventArgs e)
        {

            string strToCopy = comboBox8.Text + "." + comboBox2.Text;
            //Clipboard.SetText(strToCopy);
            comboBox10.Text = strToCopy;
        }



        /// <summary>
        /// Questo metodo provvede ad aggiornare un job all'interno del database
        /// </summary>
        private void btnModificaJob_Click(object sender, EventArgs e)
        {

            int sqlRc = -1;
            String strSql = "";
            String strDescrizione = "";
            String dataOraCorrente = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");


            if (textBox7.Text.IndexOf(" ") != -1)
            {
                MessageBox.Show("Il campo chiave ID JOB non può contenere spazi!", "Aggiungi nuovo job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox7.Focus();
                return;
            }

            if (textBox7.Text.Length <= 0)
            {
                MessageBox.Show("Campo Id Job Obbligatorio!", "Modifica job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox7.Focus();
                return;
            }


            if (comboBox18.Text.Length <= 0)
            {
                MessageBox.Show("Campo Tipo Contr. Obbligatorio!", "Aggiungi nuovo job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox18.Focus();
                return;
            }

            String strCOD = (comboBox18.SelectedItem as ComboboxItem).Value.ToString();

            if (strCOD.CompareTo("ALL") == 0 || strCOD.CompareTo("null") == 0)
                strCOD = "NULL";



            if (comboBox7.Text.CompareTo("") == 0)
            {
                MessageBox.Show("Campo Mastertable Obbligatorio!", "Modifica job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox7.Focus();
                return;
            }

            if (comboBox6.Text.CompareTo("") == 0)
            {
                MessageBox.Show("Campo Tipo Obbligatorio!", "Modifica job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox6.Focus();
                return;
            }
            if (textBox3.Text.Length <= 0)
            {
                MessageBox.Show("Campo Descrizione Obbligatorio!", "Modifica job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox3.Focus();
                return;
            }








            try
            {
                //Modifica nuovo job
                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {


                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleCommand cmd = new OracleCommand();

                    OracleDataReader reader;

                    int countJob = 0;
                    String strIdTicket = "";
                    String strCodeTipoContr = "";
                    String strSystem = "";



                    strDescrizione = textBox3.Text;


                    object obj_code = "";
                    if (strCOD.CompareTo("NULL") == 0)
                        obj_code = DBNull.Value;
                    else
                        obj_code = strCOD.ToString();


                    strSql = "update wp1_job set ";
                    strSql += "descrizione = :descrizione,";
                    strSql += "active = :active,";
                    strSql += "job_type = :job_type,";
                    strSql += "code_tipo_contr = :code_tipo_contr ,";
                    strSql += "mastertable = :mastertable ";
                    strSql += "where id_job = :id_job ";


                    OracleParameter[] parameters = new OracleParameter[]{
                                    new OracleParameter("id_job",textBox7.Text),
                                    new OracleParameter("descrizione",strDescrizione),
                                    new OracleParameter("active", comboBox4.SelectedIndex),
                                    new OracleParameter("job_type",comboBox6.Text),
                                    new OracleParameter("code_tipo_contr",obj_code),
                                    new OracleParameter("mastertable",comboBox7.Text)
                                };

                    OracleParameter id_job = new OracleParameter("id_job", textBox7.Text);
                    string strCTP = comboBox18.Text;
                    string[] arPart = strCTP.Split(new string[] { " - " }, StringSplitOptions.None);
                    if (arPart.Length > 0)
                        strCTP = arPart[0];
                    else
                        strCTP = "";

                    OracleParameter id_service = new OracleParameter("id_service", strCTP);

                    try
                    {


                        //Controllo se esiste il JOB nel DB
                        cmd.CommandText = "SELECT count(id_job) as conteggio  FROM wp1_job where id_job = :id_job ";
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = cn;
                        cmd.Parameters.Add(id_job);

                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            countJob = Int32.Parse(reader[0].ToString());
                        }
                        reader.Close();


                        if (countJob <= 0)
                        {
                            MessageBox.Show("Nessun Job presente nel database con chiave '" + textBox7.Text + "' !!", "Modifica job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            JpubDb.Close();
                            return;

                        }



                        //Se valorizzato eseguo il check sul Code Tipo Contr 

                        /*
                        if (comboBox18.Text.CompareTo("ALL") != 0)
                        {

                            //Ricavo il Code Tipo Contr selezionato 
                            cmd.CommandText = "SELECT id_macro_service  FROM wp1_services  where id_service   = :id_service and rownum = 1 ";
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(id_service);
                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                strCodeTipoContr = reader[0].ToString();
                            }
                            reader.Close();



                            //Controllo se esiste associazione Ticket/Job
                            cmd.CommandText = "SELECT id_ticket  FROM wp1_instance_exec where id_job  = :id_job and rownum=1 ";
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(id_job);
                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                strIdTicket = reader[0].ToString();
                            }
                            reader.Close();



                            if (strIdTicket.CompareTo("") != 0)
                            {

                                OracleParameter id_ticket = new OracleParameter("id_ticket", strIdTicket);
                                cmd.CommandText = "SELECT system  FROM wp1_ticketga  where id_ticket  = :id_ticket and rownum = 1 ";
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add(id_ticket);
                                reader = cmd.ExecuteReader();
                                while (reader.Read())
                                {
                                    strSystem = reader[0].ToString();
                                }
                                reader.Close();


                                if (strSystem.CompareTo(strCodeTipoContr) != 0)
                                {
                                    MessageBox.Show("Il Job è associato ad un Ticket di tipo '" + strSystem + "' non compatibile con il campo Tipo Contr. selezionato!!", "Modifica job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    comboBox18.Focus();
                                    JpubDb.Close();
                                    return;                                         
                                }


                            }



                        }//if code tipo contr

                        */


                        if (MessageBox.Show("Modifcare il job " + textBox7.Text + " nel database?", "Modifica job", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                        {
                            JpubDb.Close();
                            return;
                        }


                        cmd.CommandText = strSql;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddRange(parameters);
                        sqlRc = cmd.ExecuteNonQuery();

                        MessageBox.Show("Aggiornamento effettuato con successo!", "Modifica job", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception exsql)
                    {
                        MessageBox.Show(exsql.Message, "Modifica job - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    Console.WriteLine("SQL:" + strSql + " - RC:" + sqlRc);

                    JpubDb.Close();

                    refreshJobsList();
                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Modifica job - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("ModificaJob Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }

        }

        /// <summary>
        /// Questo metodo provvede ad eliminare un job dal database
        /// </summary>
        private void btnEliminaJob_Click(object sender, EventArgs e)
        {
            int sqlRc = -1;
            String strSql = "";
            String strDescrizione = "";
            String dataOraCorrente = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");
            String strListTicket = "";
            int countJob = 0;


            if (textBox7.Text.Length <= 0)
            {
                MessageBox.Show("Nessun Job nel campo Id Job!\nInserire il nome del Job nel campo Id Job oppure selezionare il Job dalla griglia e riprovare.", "Elimina job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox7.Focus();
                return;
            }




            try
            {
                //Elimina nuovo job
                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {


                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleCommand cmd = new OracleCommand();

                    OracleDataReader reader;



                    //Controllo se esiste il JOB nel DB
                    cmd.CommandText = "SELECT count(id_job) as conteggio  FROM wp1_job where id_job = :id_job ";

                    OracleParameter id_job = new OracleParameter("id_job", textBox7.Text);

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = cn;
                    cmd.Parameters.Add(id_job);

                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        countJob = Int32.Parse(reader[0].ToString());
                    }
                    reader.Close();


                    if (countJob <= 0)
                    {
                        MessageBox.Show("Nessun Job presente nel database con chiave '" + textBox7.Text + "' !!", "Modifica job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        JpubDb.Close();
                        return;

                    }

                    cmd.CommandText = "SELECT id_ticket from wp1_instance_exec where id_job = :id_job order by id_ticket ";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = cn;

                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        strListTicket += reader[0].ToString() + "\n";
                    }
                    reader.Close();


                    if (strListTicket.CompareTo("") != 0)
                    {
                        if (MessageBox.Show("Attenzione! Questo Job è associato ai seguenti ticket:\n" + strListTicket + "\nEliminare il job " + textBox7.Text + " nel database?", "Elimina job", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                        {
                            JpubDb.Close();
                            return;
                        }
                    }
                    else
                    {
                        if (MessageBox.Show("Attenzione! Eliminando questo job verranno eliminate anche le configurazioni ad esso associate.\nEliminare il job " + textBox7.Text + " nel database?", "Elimina job", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                        {
                            JpubDb.Close();
                            return;
                        }
                    }


                    try
                    {
                        strSql = "delete from wp1_job_condition where id_job = :id_job ";
                        cmd.CommandText = strSql;
                        sqlRc = cmd.ExecuteNonQuery();

                        strSql = "delete from wp1_job_operation where id_job = :id_job ";
                        cmd.CommandText = strSql;
                        sqlRc = cmd.ExecuteNonQuery();


                        strSql = "delete from wp1_instance_parameters where id_job = :id_job ";
                        cmd.CommandText = strSql;
                        sqlRc = cmd.ExecuteNonQuery();

                        strSql = "delete from wp1_instance_exec where id_job = :id_job ";
                        cmd.CommandText = strSql;
                        sqlRc = cmd.ExecuteNonQuery();


                        strDescrizione = textBox3.Text;

                        strSql = "delete from wp1_job ";
                        strSql += "where id_job = :id_job ";

                        cmd.CommandText = strSql;
                        sqlRc = cmd.ExecuteNonQuery();

                        MessageBox.Show("Job eliminato con successo!", "Elimina Job", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    catch (Exception exsql)
                    {
                        MessageBox.Show(exsql.Message, "Elimina Job - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    Console.WriteLine("SQL:" + strSql + " - RC:" + sqlRc);

                    JpubDb.Close();

                    initJobTab();

                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Elimina job - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Elimina Job Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }
        }

        private void comboBox17_SelectedIndexChanged(object sender, EventArgs e)
        {
            caricaColumns(comboBox14, comboBox17.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strToCopy = comboBox17.Text + "." + comboBox14.Text;
            comboBox15.Text = strToCopy;
            //Clipboard.SetText(strToCopy);
        }



        /// <summary>
        /// Questo metodo inserisce un job all'interno del DropDownTreeView
        /// </summary>
        /// <param name="sender">Oggetto</param>
        /// <param name="e">Oggetto EventArgs</param>
        private void aggiungiJob_Click(object sender, EventArgs e)
        {

            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();
                int recCount = 0;

                cmd.CommandText = "SELECT id_job,descrizione FROM wp1_job where active = 1 order by upper(id_job)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                reader = cmd.ExecuteReader();


                DropDownTreeNode tn2 = new DropDownTreeNode(strSelectJob);

                while (reader.Read())
                {
                    tn2.ComboBox.Items.Add(reader[0].ToString());
                    recCount++;
                }
                reader.Close();
                JpubDb.Close();

                //TreeNode jobNode = new TreeNode(test.Text);

                tn2.ComboBox.SelectedIndex = 1;
                dropDownTreeView1.Nodes.Add(tn2);
                //dropDownTreeView1.Nodes[0].Nodes.Add(tn2);


            }


        }

        private void mnuPulisci_Click(object sender, EventArgs e)
        {
            if (dropDownTreeView1.Nodes.Count <= 0)
                return;

            if (dropDownTreeView1.Nodes.Count > 0)
            {
                if (MessageBox.Show("Cancellare il contenuto dell'alberatura?", "Job Builder", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }


            ClearTreeView();

        }

        private void newProject_Click(object sender, EventArgs e)
        {
            InputBoxResult test = InputBox.Show("Inserisci il nome del nuovo progetto:", "Nuovo Progetto", "Progetto1");

            if (test.ReturnCode == DialogResult.Cancel)
            {
                return;
            }

            TreeNode jobNode = new TreeNode(test.Text);
            dropDownTreeView1.Nodes.Add(jobNode);

        }






        /// <summary>
        /// Questo metodo inserisce una condition all'interno di un DropDownTreeView
        /// </summary>
        /// <param name="sender">Oggetto</param>
        /// <param name="e">Oggetto EventArgs</param>
        private void aggiungiCondition_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(this, dropDownTreeView1.Nodes.Count.ToString());

            if (dropDownTreeView1.SelectedNode == null)
            {
                MessageBox.Show(this, "Nessun nodo selezionato!", "Aggiungi Condition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (dropDownTreeView1.SelectedNode.Text.CompareTo(strSelectJob) == 0)
            {
                MessageBox.Show(this, "Nessun Job selezionato!", "Aggiungi Condition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            string[] arPath = dropDownTreeView1.SelectedNode.FullPath.Split('\\');
            if (arPath.Length == 2)
            {
                if (arPath[1].IndexOf("Condition") < 0)
                {
                    Console.WriteLine("PAth:" + dropDownTreeView1.SelectedNode.FullPath);
                    MessageBox.Show(this, "E' necessario selezionare un nodo Condition per aggiungere la condizione!", "Aggiungi Condition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            if (dropDownTreeView1.Nodes.Count > 0)
            {
                //MessageBox.Show(this, dropDownTreeView1.SelectedNode.FullPath );
            }

            if (dropDownTreeView1.Nodes.Count <= 0)
            {
                MessageBox.Show(this, "Nessun Job aggiunto!\nE\' necessario inserire il Job per proseguire.", "Aggiungi Condition", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();
                int recCount = 0;

                cmd.CommandText = "SELECT ID_CONDITION ,FIELD,OPERATOR,OPERANDS,FIELD_TYPE,COND_TYPE FROM wp1_conditions  order by upper(id_condition)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                reader = cmd.ExecuteReader();


                DropDownTreeNode tn2 = new DropDownTreeNode(strSelezionaCondition);

                while (reader.Read())
                {
                    tn2.ComboBox.Items.Add(reader[0].ToString());
                    recCount++;
                }
                reader.Close();
                JpubDb.Close();

                tn2.ComboBox.SelectedIndex = 1;
                dropDownTreeView1.Nodes[0].Nodes[1].Nodes.Add(tn2);
                dropDownTreeView1.Nodes[0].Nodes[1].Text = "Conditions(" + dropDownTreeView1.Nodes[0].Nodes[1].Nodes.Count + ")";

                dropDownTreeView1.HideCB();

            }


        }


        private void dropDownTreeView1_OnBeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
            return;
        }





        private void dropDownTreeView1_OnChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Click After Select Node:" + dropDownTreeView1.currText);

            //dropDownTreeView1.HideCB();
        }


        private void dropDownTreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

            try
            {
                //Console.WriteLine("Click After Select Node:" + e.Node.Text + dropDownTreeView1.currText);
                Console.WriteLine("dropDownTreeView1_AfterSelect CLICK:" + dropDownTreeView1.currText);
                Console.WriteLine("dropDownTreeView1_AfterSelect PATH:" + dropDownTreeView1.SelectedNode.FullPath);
                Console.WriteLine("dropDownTreeView1_AfterSelect INDEX:" + dropDownTreeView1.SelectedNode.Index);

                //dropDownTreeView1.HideCB();



                if (dropDownTreeView1.SelectedNode.Index == 0 && dropDownTreeView1.SelectedNode.Text.ToUpper().CompareTo(strSelectJob) != 0 && dropDownTreeView1.SelectedNode.FullPath.IndexOf("\\") < 0)
                {
                    loadJob(dropDownTreeView1.SelectedNode.Text.ToUpper());
                }

                aggiungiJob.Visible = false;

                dropDownTreeView1.ExpandAll();

                createSqlString();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Operazione non consentita! Selezionare l\'item dalla lista utilizzando esclusivamente il mouse.", "Jpub Builder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }
        }



        private void dropDownTreeView1_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("dropDownTreeView1_MouseDown ");

        }


        private void dropDownTreeView1_MouseUp(object sender, MouseEventArgs e)
        {


            if (dropDownTreeView1.SelectedNode == null)
                return;


            Console.WriteLine("dropDownTreeView1_MouseUp " + dropDownTreeView1.SelectedNode.Text);


            if (e.Button.ToString().ToUpper() == "RIGHT")
            {
                dropDownTreeView1.HideCB();
                Console.WriteLine("SELECT NODE:" + dropDownTreeView1.SelectedNode.Text + " : " + dropDownTreeView1.SelectedNode.Index + " : " + dropDownTreeView1.SelectedNode.Text.ToUpper());
                aggiungiJob.Visible = false;


            }

            if (e.Button.ToString().ToUpper() == "LEFT")
            {

                var hitTest = dropDownTreeView1.HitTest(e.Location);
                if (hitTest.Location == TreeViewHitTestLocations.PlusMinus)
                {
                    Console.WriteLine("Click");
                }
            }


        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Questo metodo inserisce una operation all'interno di un DropDownTreeView
        /// </summary>
        /// <param name="sender">Oggetto</param>
        /// <param name="e">Oggetto EventArgs</param>
        private void aggiungiOperation_Click(object sender, EventArgs e)
        {
            if (dropDownTreeView1.SelectedNode == null)
            {
                MessageBox.Show(this, "Nessun nodo selezionato!", "Aggiungi Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (dropDownTreeView1.SelectedNode.Text.CompareTo(strSelectJob) == 0)
            {
                MessageBox.Show(this, "Nessun Job selezionato!", "Aggiungi Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }


            string[] arPath = dropDownTreeView1.SelectedNode.FullPath.Split('\\');
            if (arPath.Length == 2)
            {
                if (arPath[1].IndexOf("Operation") < 0)
                {
                    Console.WriteLine("PAth:" + dropDownTreeView1.SelectedNode.FullPath);
                    MessageBox.Show(this, "E' necessario selezionare un nodo Operation per aggiungere l'operazione!", "Aggiungi Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }


            if (dropDownTreeView1.Nodes.Count <= 0)
            {
                MessageBox.Show(this, "Nessun Job aggiunto!\nE\' necessario inserire il Job per proseguire.", "Aggiungi Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();
                int recCount = 0;

                cmd.CommandText = "SELECT ID_OPERATION ,FIELD,OPERATOR,OPERAND,OP_TYPE FROM wp1_operations  order by upper(id_operation)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                reader = cmd.ExecuteReader();


                DropDownTreeNode tn2 = new DropDownTreeNode(strSelezionaOperation);

                while (reader.Read())
                {
                    tn2.ComboBox.Items.Add(reader[0].ToString());
                    recCount++;
                }
                reader.Close();
                JpubDb.Close();

                tn2.ComboBox.SelectedIndex = 1;
                dropDownTreeView1.Nodes[0].Nodes[0].Nodes.Add(tn2);
                dropDownTreeView1.Nodes[0].Nodes[0].Text = "Operations(" + dropDownTreeView1.Nodes[0].Nodes[0].Nodes.Count + ")";

                dropDownTreeView1.HideCB();

            }

        }




        /// <summary>
        /// Questo metodo Carica la lista delle Operations utilizzata nel Builder
        /// </summary>
        /// <returns>Boolean</returns>
        private Boolean loadOperationList()
        {
            Boolean bRc = false;
            Operation oper;
            lstOperation.Clear();
            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();
                int recCount = 0;

                cmd.CommandText = "SELECT ID_OPERATION ,FIELD,OPERATOR,OPERAND,OP_TYPE FROM wp1_operations  order by id_operation";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    oper = new Operation();
                    oper.id_operation = reader[0].ToString();
                    oper.o_field = reader[1].ToString();
                    oper.o_operator = reader[2].ToString();
                    oper.o_operand = reader[3].ToString();
                    oper.o_op_type = reader[4].ToString();
                    lstOperation.Add(oper);
                    recCount++;
                }
                reader.Close();
                JpubDb.Close();
                bRc = true;


            }
            else
            {
                MessageBox.Show(JpubDb.getLastError(), "Errore Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                JpubDb = null;
            }
            return (bRc);

        }//loadOperationList


        /// <summary>
        /// Questo metodo Carica la lista delle Conditions utilizzata nel Builder
        /// </summary>
        /// <returns>Boolean</returns>
        private Boolean loadConditionsList()
        {
            Boolean bRc = false;
            Condition cond;
            lstCondition.Clear();
            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();
                int recCount = 0;

                cmd.CommandText = "SELECT id_condition,field,operator,operands,field_type,cond_type FROM wp1_conditions order by id_condition";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cond = new Condition();
                    cond.id_condition = reader[0].ToString();
                    cond.c_field = reader[1].ToString();
                    cond.c_operator = reader[2].ToString();
                    cond.c_operands = reader[3].ToString();
                    cond.c_field_type = reader[4].ToString();
                    cond.c_cond_type = reader[5].ToString();
                    lstCondition.Add(cond);
                    recCount++;
                }

                reader.Close();
                JpubDb.Close();
                bRc = true;
            }
            else
            {
                MessageBox.Show(JpubDb.getLastError(), "Errore Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                JpubDb = null;
            }
            return (bRc);


        }//loadConditionsList



        /// <summary>
        /// Questo metodo inizializza il Tab Builder
        /// </summary>
        private void initBuilderTab()
        {
            visualizzaToolStripMenuItem.Visible = true;
            visualizzaToolStripMenuItem.Checked = false;
            ClearTreeView();
            textBox8.Visible = false;


            if (sQLScriptToolStripMenuItem.Checked == true)
            {
                textBox8.Visible = true;
            }

            Ridisegna();
            if (loadConditionsList() == false)
                return;

            if (loadOperationList() == false)
                return;


            //loadJob("JOB_COD_CAUS_ULL");


        }//initBuilderTab





        private void copiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dropDownTreeView1.SelectedNode != null)
            {
                Clipboard.SetText(dropDownTreeView1.SelectedNode.Text);
            }
        }


        private void label38_Click(object sender, EventArgs e)
        {

        }//label38_Click




        private void ColourRrbText()
        {
            Regex regExp = new Regex("select|insert|delete|update|where|and|=|<|>|in|not in|order|by|group|!=|to_char|to_date|set|like|SELECT|INSERT|DELETE|UPDATE|WHERE|AND|=|<|>|IN|NOT IN|ORDER|BY|GROUP|!=|TO_CHAR|TO_DATE|SET|LIKE");

            foreach (Match match in regExp.Matches(textBox8.Text))
            {
                textBox8.Select(match.Index, match.Length);
                textBox8.SelectionColor = Color.Blue;
            }
            textBox8.DeselectAll();
        }




        private String getTableFrom(string strCondition)
        {
            String strTable = "";

            for (int i = 0; i < lstCondition.Count; i++)
            {
                if (strCondition.ToUpper().CompareTo(lstCondition[i].id_condition.ToUpper()) == 0)
                {
                    if (lstCondition[i].c_field_type.CompareTo("TABLE") == 0)
                    {
                        string[] arTbl = lstCondition[i].c_operands.Split('.');
                        strTable = arTbl[0];
                    }
                }
            }
            return (strTable);
        }//getTableFrom




        /// <summary>
        /// Questo metodo ritorna un ListArray contenente tutte le Condition del Job Corrente
        /// </summary>
        private List<Condition> getListCurrConditions()
        {


            List<Condition> lstCC = new List<Condition>();

            if (dropDownTreeView1.Nodes[0].Nodes.Count <= 0)
                return (lstCC);

            for (int i = 0; i < dropDownTreeView1.Nodes[0].Nodes[1].Nodes.Count; i++)
            {
                String strCC = dropDownTreeView1.Nodes[0].Nodes[1].Nodes[i].Text;
                for (int n = 0; n < lstCondition.Count; n++)
                {
                    if (lstCondition[n].id_condition.ToUpper().CompareTo(strCC.ToUpper().Replace("*", "")) == 0)
                    {
                        Condition cc = new Condition();
                        cc = lstCondition[n];
                        if (strCC.Substring(strCC.Length - 1, 1).CompareTo("*") == 0)
                            cc.c_cond_optional = "1";
                        else
                            cc.c_cond_optional = "0";

                        lstCC.Add(cc);
                        break;
                    }
                }

            }
            return (lstCC);
        }//getListCurrConditions


        /// <summary>
        /// Questo metodo ritorna un ListArray contenente tutte le Operation del Job Corrente
        /// </summary>
        private List<Operation> getListCurrOperations()
        {
            List<Operation> lstOO = new List<Operation>();

            if (dropDownTreeView1.Nodes[0].Nodes.Count <= 0)
                return (lstOO);

            for (int i = 0; i < dropDownTreeView1.Nodes[0].Nodes[0].Nodes.Count; i++)
            {
                String strOO = dropDownTreeView1.Nodes[0].Nodes[0].Nodes[i].Text;
                for (int n = 0; n < lstOperation.Count; n++)
                {
                    if (lstOperation[n].id_operation.ToUpper().CompareTo(strOO.ToUpper()) == 0)
                    {
                        Operation cc = new Operation();
                        cc = lstOperation[n];
                        lstOO.Add(cc);
                        break;
                    }
                }

            }
            return (lstOO);
        }//getListCurrOperations



        private void ClearTreeView()
        {
            for (int i = 0; i < dropDownTreeView1.Nodes.Count; i++)
                dropDownTreeView1.Nodes[i].Nodes.Clear();

            dropDownTreeView1.Nodes.Clear();
            dropDownTreeView1.currText = "";
            textBox8.Text = "";
            currJob = null;
            aggiungiJob.Visible = true;



        }




        private void sQLScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sQLScriptToolStripMenuItem.Checked == true)
            {
                sQLScriptToolStripMenuItem.Checked = false;
                textBox8.Visible = false;
                dropDownTreeView1.Height = dropDownTreeView1.Height + 200;
            }
            else
            {
                sQLScriptToolStripMenuItem.Checked = true;
                dropDownTreeView1.Height = dropDownTreeView1.Height - 200;
                Ridisegna();
                textBox8.Visible = true;
            }
        }

        private void proprietàToolStripMenuItem_Click(object sender, EventArgs e)
        {


            if (dropDownTreeView1.SelectedNode == null) return;

            if (dropDownTreeView1.SelectedNode.Text.CompareTo(strSelectJob) == 0)
                return;

            try
            {
                string[] arStr = dropDownTreeView1.SelectedNode.FullPath.ToString().Split('\\');
                string strMsg = "";

                if (arStr[1].ToString().IndexOf("Condition") != -1)
                {
                    for (int i = 0; i < lstCondition.Count; i++)
                    {
                        if (lstCondition[i].id_condition.ToUpper().CompareTo(arStr[2].ToUpper()) == 0)
                        {
                            strMsg = "ID_CONDITION: " + lstCondition[i].id_condition + "\n";
                            strMsg += "FIELD: " + lstCondition[i].c_field + "\n";
                            strMsg += "OPERATOR: " + lstCondition[i].c_operator + "\n";
                            strMsg += "OPERANDS: " + lstCondition[i].c_operands + "\n";
                            strMsg += "FIELD_TYPE: " + lstCondition[i].c_field_type + "\n";
                            strMsg += "COND_TYPE: " + lstCondition[i].c_cond_type + "\n";
                            break;
                        }
                    }
                }

                if (arStr[1].ToString().IndexOf("Operation") != -1)
                {
                    for (int i = 0; i < lstOperation.Count; i++)
                    {
                        if (lstOperation[i].id_operation.ToUpper().CompareTo(arStr[2].ToUpper()) == 0)
                        {
                            strMsg = "ID_OPERATION: " + lstOperation[i].id_operation + "\n";
                            strMsg += "FIELD: " + lstOperation[i].o_field + "\n";
                            strMsg += "OPERATOR: " + lstOperation[i].o_operator + "\n";
                            strMsg += "OPERAND: " + lstOperation[i].o_operand + "\n";
                            strMsg += "OP_TYPE: " + lstOperation[i].o_op_type + "\n";
                            break;
                        }
                    }
                }

                MessageBox.Show(this, strMsg, "Proprietà", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
            }


        }//proprietàToolStripMenuItem_Click



        /// <summary>
        /// Questo metodo è utilizzato per comporre la stringa SQL real time
        /// </summary>
        private void createSqlString()
        {

            textBox8.Text = "";
            textBox8.SelectAll();
            textBox8.SelectionColor = Color.Black;

            if (currJob == null)
                return;


            //if (dropDownTreeView1.Nodes[0].Nodes.Count <=0)
            //  return; 


            List<Condition> lstCurrCond = new List<Condition>();
            List<Operation> lstCurrOper = new List<Operation>();
            List<string> lstTabelle = new List<string>();


            //Carico le liste Operations/Conditions del Job Corrente
            lstCurrCond = getListCurrConditions();
            lstCurrOper = getListCurrOperations();




            //Analizzo e compongo la string SQL 
            textBox8.Text = "update " + currJob.j_mastertable + " ";

            for (int j = 0; j < lstCurrCond.Count; j++)
            {
                for (int i = 0; i < lstCondition.Count; i++)
                {
                    if (lstCondition[i].id_condition.CompareTo(lstCurrCond[j].id_condition) == 0 && lstCondition[i].c_field_type.CompareTo("TABLE") == 0)
                    {
                        string[] arT = lstCondition[i].c_operands.Split('.');

                        if (arT.Length > 0)
                        {
                            if (arT[0].CompareTo(currJob.j_mastertable) != 0)
                            {
                                lstTabelle.Add(arT[0]);
                            }
                        }

                        break;
                    }
                }
            }

            /*
            for (int i = 0; i < lstTabelle.Count; i++)
            {
                textBox8.Text += "," + lstTabelle[i] + " ";
            }*/

            textBox8.Text += " set ";

            Boolean bCsv = true;
            for (int i = 0; i < lstCurrOper.Count; i++)
            {
                Operation oo = new Operation();
                oo = lstCurrOper[i];

                if (bCsv == false)
                    textBox8.Text += ", ";

                if (oo.o_operator.CompareTo("COMPLEX") != 0)
                {
                    textBox8.Text += oo.o_field + " = " + oo.o_operand;
                    bCsv = false;
                }


            }


            textBox8.Text += " where ";

            textBox8.Text += "( " + currJob.j_mastertable + "." + currJob.j_primarykey + ") IN ( select " + currJob.j_mastertable + "." + currJob.j_primarykey + " from " + currJob.j_mastertable;
            for (int i = 0; i < lstTabelle.Count; i++)
            {
                textBox8.Text += "," + lstTabelle[i] + " ";
            }

            textBox8.Text += " where ";

            Boolean bWhere = true;
            for (int i = 0; i < lstCurrCond.Count; i++)
            {
                Condition cc = new Condition();
                cc = lstCurrCond[i];

                if (bWhere == false)
                    textBox8.Text += "and ";

                if (cc.c_field_type.CompareTo("STRING") == 0 && cc.c_operator.CompareTo("RAWSTATEMENT") != 0 && cc.c_operator.CompareTo("LIKE") != 0)
                {
                    textBox8.Text += cc.c_field + " " + cc.c_operator.ToLower() + " '" + cc.c_operands + "' ";
                    bWhere = false;
                }
                else if (cc.c_field_type.CompareTo("NUMBER") == 0)
                {
                    textBox8.Text += cc.c_field + " " + cc.c_operator.ToLower() + " " + cc.c_operands + " ";
                    bWhere = false;
                }
                else if (cc.c_operator.CompareTo("RAWSTATEMENT") == 0)
                {
                    textBox8.Text += cc.c_field + " " + cc.c_operands + " ";
                    bWhere = false;
                }
                else if (cc.c_operator.CompareTo("LIKE") == 0)
                {
                    textBox8.Text += cc.c_field + " " + cc.c_operator.ToLower() + " " + cc.c_operands + " ";
                    bWhere = false;
                }
                else if (cc.c_field_type.CompareTo("TABLE") == 0 || cc.c_field_type.CompareTo("FIELD") == 0)
                {
                    textBox8.Text += cc.c_field + " " + cc.c_operator.ToLower() + " " + cc.c_operands + " ";
                    bWhere = false;
                }
                else if (cc.c_field_type.CompareTo("DATE") == 0)
                {
                    textBox8.Text += cc.c_field + " " + cc.c_operator.ToLower() + " " + cc.c_operands + " ";
                    bWhere = false;
                }

            }
            textBox8.Text += " ) ";
            ColourRrbText();

        }


        /// <summary>
        /// Questo metodo ricarica un Job all'interno di un DropDownTreeView
        /// </summary>
        /// <param name="strJob">String</param>
        private void loadJob(string strJob)
        {


            string strMastertable = "";

            textBox8.Text = "";
            ClearTreeView();


            //List<string> lstCn = new List<string>();
            Condition cCond;
            List<Condition> lstCn = new List<Condition>();
            List<string> lstOp = new List<string>();
            List<string> lstTabelle = new List<string>();
            List<Condition> lstCurrCond = new List<Condition>();
            List<Operation> lstCurrOper = new List<Operation>();
            List<string> lstAccCurrCond = new List<string>();
            currJob = new Job();



            JpubOraDb JpubDb = new JpubOraDb();

            if (JpubDb.Connect() == true)
            {

                Cursor.Current = Cursors.WaitCursor;

                OracleConnection cn = JpubDb.getJpubOraDbConn();
                OracleDataReader reader;
                OracleCommand cmd = new OracleCommand();

                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                OracleParameter ID_JOB = new OracleParameter("ID_JOB", strJob);
                cmd.Parameters.Add(ID_JOB);
                cmd.CommandText = "SELECT DESCRIZIONE, ACTIVE, JOB_TYPE,CODE_TIPO_CONTR, MASTERTABLE FROM WP1_JOB WHERE ID_JOB = :ID_JOB ";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    currJob.id_job = strJob;
                    currJob.j_descrizione = reader[0].ToString();
                    currJob.j_active = reader[1].ToString();
                    currJob.j_job_type = reader[2].ToString();
                    currJob.j_code_tipo_contr = reader[3].ToString();
                    currJob.j_mastertable = reader[4].ToString();
                    strMastertable = currJob.j_mastertable;
                }
                reader.Close();



                cmd.Parameters.Clear();
                OracleParameter MASTERTABLE = new OracleParameter("mastertable", strMastertable);
                cmd.Parameters.Add(MASTERTABLE);
                cmd.CommandText = "select column_name from user_tab_columns where table_name  = :mastertable and column_id = 1";

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    currJob.j_primarykey = reader.GetString(0);
                }
                reader.Close();

                cmd.Parameters.Clear();
                ID_JOB = new OracleParameter("ID_JOB", strJob);
                cmd.Parameters.Add(ID_JOB);
                cmd.CommandText = "SELECT ID_CONDITION, OPTIONAL FROM WP1_JOB_CONDITION WHERE ID_JOB = :ID_JOB ";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cCond = new Condition();
                    cCond.id_condition = reader[0].ToString();
                    cCond.c_cond_optional = reader[1].ToString();
                    lstCn.Add(cCond);
                }
                reader.Close();

                cmd.CommandText = "SELECT ID_OPERATION FROM WP1_JOB_OPERATION WHERE ID_JOB = :ID_JOB ";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lstOp.Add(reader[0].ToString());
                }
                reader.Close();
                JpubDb.Close();


                TreeNode opNode = new TreeNode("Operations(" + lstOp.Count + ")");
                TreeNode condNode = new TreeNode("Conditions(" + lstCn.Count + ")");


                TreeNode jobNode = new TreeNode(strJob);
                jobNode.Nodes.Add(opNode);
                jobNode.Nodes.Add(condNode);
                dropDownTreeView1.Nodes.Add(jobNode);


                //Aggiungo le operations
                for (int i = 0; i < lstOp.Count; i++)
                {
                    DropDownTreeNode cn2 = new DropDownTreeNode(lstOp[i].ToString());
                    for (int j = 0; j < lstOperation.Count; j++)
                    {
                        cn2.ComboBox.Items.Add(lstOperation[j].id_operation);
                    }
                    cn2.ComboBox.Text = lstOp[i].ToString();
                    dropDownTreeView1.Nodes[0].Nodes[0].Nodes.Add(cn2);
                    dropDownTreeView1.HideCB();
                }


                //Aggiungo le conditions
                for (int i = 0; i < lstCn.Count; i++)
                {
                    DropDownTreeNode cn1;
                    if (lstCn[i].c_cond_optional.CompareTo("1") == 0)
                        cn1 = new DropDownTreeNode(lstCn[i].id_condition + "*");
                    else
                        cn1 = new DropDownTreeNode(lstCn[i].id_condition);

                    for (int j = 0; j < lstCondition.Count; j++)
                    {
                        cn1.ComboBox.Items.Add(lstCondition[j].id_condition);
                    }

                    dropDownTreeView1.Nodes[0].Nodes[1].Nodes.Add(cn1);
                    dropDownTreeView1.HideCB();
                }

                Cursor.Current = Cursors.Default;


            }

        }



        /// <summary>
        /// Questo metodo provvede al salvataggio di un job configurato in DropDownTreeView
        /// </summary>
        /// <param name="sender">Oggetto</param>
        /// <param name="e">Oggetto EventArgs</param>
        private void btnSalvaCfgJob_Click(object sender, EventArgs e)
        {
            if (dropDownTreeView1.Nodes.Count <= 0)
            {
                MessageBox.Show("Nessun Job configurato.", "Salva Configurazione Job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (dropDownTreeView1.Nodes[0].Nodes.Count <= 0)
            {
                MessageBox.Show("Nessun Job configurato.", "Salva Configurazione Job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }


            if (MessageBox.Show("Salvare la configurazione del job (" + currJob.id_job + ") nel database?", "Salva Configurazione Job", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }





            string strSql = "";
            int sqlRc = 0;
            Boolean bRC = false;


            List<Condition> lstCurrCond = new List<Condition>();
            List<Operation> lstCurrOper = new List<Operation>();

            //Carico le liste Operations/Conditions del Job Corrente
            lstCurrCond = getListCurrConditions();
            lstCurrOper = getListCurrOperations();






            try
            {
                //Inserimento nuovo ticket 
                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {


                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleCommand cmd = new OracleCommand();

                    OracleParameter id_job = new OracleParameter("id_job", currJob.id_job);

                    strSql = "delete from wp1_job_operation where id_job = :id_job ";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = cn;
                    cmd.Parameters.Add(id_job);
                    cmd.CommandText = strSql;
                    sqlRc = cmd.ExecuteNonQuery();


                    strSql = "delete from wp1_job_condition where id_job = :id_job ";
                    cmd.CommandText = strSql;
                    sqlRc = cmd.ExecuteNonQuery();

                    for (int i = 0; i < lstCurrOper.Count; i++)
                    {
                        OracleParameter id_operation = new OracleParameter("id_operation", lstCurrOper[i].id_operation);
                        strSql = "insert into WP1_JOB_OPERATION (ID_JOB,ID_OPERATION) values ( :id_job, :id_operation)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(id_job);
                        cmd.Parameters.Add(id_operation);
                        cmd.CommandText = strSql;
                        sqlRc = cmd.ExecuteNonQuery();

                    }

                    for (int i = 0; i < lstCurrCond.Count; i++)
                    {
                        OracleParameter id_condition = new OracleParameter("id_condition", lstCurrCond[i].id_condition);
                        OracleParameter c_cond_optional = new OracleParameter("c_cond_optional", lstCurrCond[i].c_cond_optional);
                        strSql = "insert into wp1_job_condition (id_job,id_condition,optional) values(:id_job,:id_condition,:c_cond_optional)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(id_job);
                        cmd.Parameters.Add(id_condition);
                        cmd.Parameters.Add(c_cond_optional);
                        cmd.CommandText = strSql;
                        sqlRc = cmd.ExecuteNonQuery();
                    }



                    MessageBox.Show("Configurazione Job Salvata con Successo!", "Salva Configurazione Job", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    JpubDb.Close();

                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Salva Configurazione Job - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("inserisciTicket Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }










        }//btnSalvaCfgJob_Click



        /// <summary>
        /// Questo metodo esegue una NonQuery sul database con l'utilizzo di Prepared Statament Oracle
        /// </summary>
        /// <param name="strSql">String</param>
        /// <param name="strSql">OracleParameter</param>
        /// <return>Boolean</reuturn>
        private Boolean ExecuteSQL(string strSql, OracleParameter[] parameters)
        {
            Boolean bRc = false;
            int sqlRc = -1;

            try
            {
                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {

                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleCommand cmd = new OracleCommand();

                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = cn;
                        cmd.CommandText = strSql;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddRange(parameters);
                        sqlRc = cmd.ExecuteNonQuery();
                        Console.WriteLine("SQL:" + strSql + " -RC:" + sqlRc);

                        bRc = true;

                    }
                    catch (Exception exsql)
                    {
                        MessageBox.Show(exsql.Message, "Execute SQL - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    JpubDb.Close();

                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Execute SQL - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Execute SQL - Error:" + ex.Message);
            }


            return (bRc);
        }


        /// <summary>
        /// Questo metodo esegue una NonQuery sul database 
        /// </summary>
        /// <param name="strSql">String</param>
        /// <return>Boolean</reuturn>
        private Boolean ExecuteSQL(string strSql)
        {
            Boolean bRc = false;
            int sqlRc = -1;

            try
            {
                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {

                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleCommand cmd = new OracleCommand();

                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = cn;
                        cmd.CommandText = strSql;
                        sqlRc = cmd.ExecuteNonQuery();
                        Console.WriteLine("SQL:" + strSql + " -RC:" + sqlRc);

                        bRc = true;

                    }
                    catch (Exception exsql)
                    {
                        MessageBox.Show(exsql.Message, "Execute SQL - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    JpubDb.Close();

                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Execute SQL - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Execute SQL - Error:" + ex.Message);
            }


            return (bRc);
        }

        private void esciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void comboBox19_SelectedIndexChanged(object sender, EventArgs e)
        {
            //caricaDatiJobSelezionato(comboBox20, comboBox19);
        }

        private void comboBox20_SelectedIndexChanged(object sender, EventArgs e)
        {
            caricaJobDelTicket(comboBox20, comboBox19);
            refreshTicketsJobsList();

        }

        private void btnAggiungiTicketJob_Click(object sender, EventArgs e)
        {
            String strSql = "";
            int sqlRc = 0;
            int nMaxCounter = 0;

            if (comboBox20.Text.CompareTo("") == 0)
            {
                MessageBox.Show("Nessun Ticket selezionato!", "Inserimento Ticket-Job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox20.Focus();
                return;
            }

            if (comboBox19.Text.CompareTo("") == 0)
            {
                MessageBox.Show("Nessun Job selezionato!", "Inserimento Ticket-Job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox19.Focus();
                return;
            }





            if (MessageBox.Show("Inserire l'associazione ticket-job nel database?", "Inserimento Ticket-Job", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }


            try
            {
                //Inserimento nuovo ticket->job
                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {


                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleCommand cmd = new OracleCommand();
                    OracleDataReader reader;

                    OracleParameter id_ticket = new OracleParameter("id_ticket", comboBox20.Text);
                    cmd.Parameters.Add(id_ticket);

                    cmd.CommandText = "SELECT nvl(max(counter),0) as massimo from wp1_instance_exec where id_ticket = :id_ticket";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = cn;

                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        nMaxCounter = Int32.Parse(reader[0].ToString());
                    }
                    nMaxCounter++;

                    reader.Close();

                    OracleParameter id_job = new OracleParameter("id_job", comboBox19.Text);
                    cmd.Parameters.Add(id_job);
                    OracleParameter n_max_counter = new OracleParameter("n_max_counter", nMaxCounter.ToString());
                    cmd.Parameters.Add(n_max_counter);


                    strSql = "insert into wp1_instance_exec (id_ticket,id_job,counter ) values (:id_ticket,:id_job,:n_max_counter)";

                    try
                    {

                        cmd.CommandText = strSql;
                        sqlRc = cmd.ExecuteNonQuery();
                        MessageBox.Show("Inserimento effettuato con successo!", "Inserimento ticket-job", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception exsql)
                    {
                        MessageBox.Show(exsql.Message, "Aggiungi nuovo ticket-job - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }



                    Console.WriteLine("SQL:" + strSql + " - RC:" + sqlRc);

                    JpubDb.Close();

                    refreshTicketsJobsList();
                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Aggiungi nuovo ticke-jobt - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("inserisciTicketJob Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }


        }

        private void listView5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView5.SelectedItems.Count <= 0)
                return;

            comboBox20.Text = listView5.SelectedItems[0].SubItems[0].Text;
            comboBox19.Text = listView5.SelectedItems[0].SubItems[1].Text;
            caricaParametriTicketJob(comboBox20.Text, comboBox19.Text, listView5.SelectedItems[0].SubItems[2].Text, listView6);

        }





        private void btnModificaTicketJob_Click(object sender, EventArgs e)
        {

        }

        private void btnEliminaTicketJob_Click(object sender, EventArgs e)
        {

            String strSql = "";
            int sqlRc = 0;


            if (listView5.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Nessuna riga selezionata in lista!", "Elimina Ticket-Job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox19.Focus();
                return;
            }




            if (MessageBox.Show("Eliminare l'associazione ticket-job nel database?", "Elimina Ticket-Job", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            try
            {
                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {


                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleCommand cmd = new OracleCommand();


                    try
                    {
                        OracleParameter[] parameters = new OracleParameter[]{
                                    new OracleParameter("id_ticket",listView5.SelectedItems[0].SubItems[0].Text),
                                    new OracleParameter("id_job", listView5.SelectedItems[0].SubItems[1].Text),
                                    new OracleParameter("counter",listView5.SelectedItems[0].SubItems[2].Text)
                                };

                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = cn;
                        cmd.Parameters.AddRange(parameters);

                        strSql = "delete from wp1_instance_parameters ";
                        strSql += "where id_ticket = :id_ticket and id_job = :id_job and counter = :counter  ";
                        cmd.CommandText = strSql;
                        sqlRc = cmd.ExecuteNonQuery();

                        strSql = "delete from wp1_instance_exec ";
                        strSql += "where id_ticket = :id_ticket and id_job = :id_job and counter = :counter ";
                        cmd.CommandText = strSql;
                        sqlRc = cmd.ExecuteNonQuery();


                        listView6.Items.Clear();
                        comboBox21.Text = "";
                        comboBox22.Text = "";
                        checkBox1.Checked = false;

                        MessageBox.Show("Associazione eliminata con successo!", "Elimina Ticket->Job", MessageBoxButtons.OK, MessageBoxIcon.Information);


                    }
                    catch (Exception exsql)
                    {
                        MessageBox.Show(exsql.Message, "Elimina Ticket->Job - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    Console.WriteLine("SQL:" + strSql + " - RC:" + sqlRc);

                    JpubDb.Close();

                    refreshTicketsJobsList();
                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Elimina  Ticket->Job - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("inserisciTicket Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }

        }

        private void listView6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView6.SelectedItems.Count <= 0)
                return;

            comboBox21.Text = listView6.SelectedItems[0].SubItems[0].Text;
            comboBox22.Text = listView6.SelectedItems[0].SubItems[3].Text;
            checkBox1.Checked = false;
            if (listView6.SelectedItems[0].SubItems[4].Text.CompareTo("1") == 0)
            {
                checkBox1.Checked = true;
            }

        }

        private void btnAggiungiTicketJobParam_Click(object sender, EventArgs e)
        {
            String strSql = "";
            int sqlRc = 0;

            String strParametro = "";

            if (listView5.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Nessun Ticket->Job Selezionato in lista !", "Inserimento Ticket-Job Parameter", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                listView5.Focus();
                return;
            }

            if (comboBox21.Text.CompareTo("") == 0)
            {
                MessageBox.Show("Nessun Nome Parametro inserito!", "Modifica Ticket-Job Parameter", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox21.Focus();
                return;
            }




            if (checkNameParam(comboBox21.Text) == false)
            {
                MessageBox.Show("Formato del parametro errato!", "Modifica Ticket-Job Parameter", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox21.Focus();
                return;
            }




            if (MessageBox.Show("Inserire i parametri dell'associazione nel database?", "Inserimento Ticket-Job Parameter", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }


            try
            {
                //Inserimento nuovo ticket->job
                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {


                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleCommand cmd = new OracleCommand();

                    int isMacro = 0;
                    if (checkBox1.Checked == true)
                    {
                        isMacro = 1;
                    }

                    strParametro = comboBox22.Text;
                    strParametro = strParametro;

                    OracleParameter[] parameters = new OracleParameter[]{
                                    new OracleParameter("name_param",comboBox21.Text),
                                    new OracleParameter("id_ticket",comboBox20.Text),
                                    new OracleParameter("id_job",comboBox19.Text),
                                    new OracleParameter("val_param",strParametro),
                                    new OracleParameter("is_macro",isMacro.ToString()),
                                    new OracleParameter("counter",listView5.SelectedItems[0].SubItems[2].Text)
                                };


                    strSql = "insert into wp1_instance_parameters (name_param,id_ticket,id_job,val_param,is_macro,counter ) values (:name_param,:id_ticket,:id_job,:val_param,:is_macro,:counter)";

                    try
                    {

                        cmd.CommandText = strSql;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = cn;
                        cmd.Parameters.AddRange(parameters);
                        sqlRc = cmd.ExecuteNonQuery();
                        MessageBox.Show("Inserimento effettuato con successo!", "Inserimento parametri ticket-job", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        caricaParametriTicketJob(listView5.SelectedItems[0].SubItems[0].Text, listView5.SelectedItems[0].SubItems[1].Text, listView5.SelectedItems[0].SubItems[2].Text, listView6);
                        comboBox21.Text = "";
                        comboBox22.Text = "";
                        checkBox1.Checked = false;
                    }
                    catch (Exception exsql)
                    {
                        MessageBox.Show(exsql.Message, "Aggiungi nuovo ticket-job - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }



                    Console.WriteLine("SQL:" + strSql + " - RC:" + sqlRc);

                    JpubDb.Close();

                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Aggiungi nuovo ticke-jobt - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("inserisciTicketJob Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }

        }

        private void btnModificaTicketJobParam_Click(object sender, EventArgs e)
        {
            String strSql = "";
            int sqlRc = 0;
            String strParametro = "";



            if (listView6.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Nessun Ticket->Job Param Selezionato in lista !", "Modifica Ticket-Job Parameter", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                listView6.Focus();
                return;
            }



            if (comboBox21.Text.CompareTo("") == 0)
            {
                MessageBox.Show("Nessun Nome Parametro inserito!", "Modifica Ticket-Job Parameter", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox21.Focus();
                return;
            }



            if (checkNameParam(comboBox21.Text) == false)
            {
                MessageBox.Show("Formato del parametro errato!", "Modifica Ticket-Job Parameter", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comboBox21.Focus();
                return;
            }


            if (MessageBox.Show("Aggiornare i parametri dell'associazione nel database?", "Modifica Ticket-Job Parameter", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }


            try
            {
                //Modifica nuovo ticket->job
                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {


                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleCommand cmd = new OracleCommand();

                    int isMacro = 0;
                    if (checkBox1.Checked == true)
                    {
                        isMacro = 1;
                    }

                    String id_ticket = listView6.SelectedItems[0].SubItems[1].Text;
                    String id_job = listView6.SelectedItems[0].SubItems[2].Text;
                    String counter = listView6.SelectedItems[0].SubItems[5].Text;

                    strSql = "update wp1_instance_parameters set ";
                    strSql += "name_param = :name_param ,";

                    strParametro = comboBox22.Text;
                    strParametro = strParametro;

                    strSql += "val_param = :val_param ,";
                    strSql += "is_macro = :is_macro ";
                    strSql += " where name_param = :name_param and id_ticket = :id_ticket and id_job = :id_job and counter =:counter ";

                    OracleParameter[] parameters = new OracleParameter[]{
                                    new OracleParameter("name_param",listView6.SelectedItems[0].SubItems[0].Text),
                                    new OracleParameter("id_ticket",listView6.SelectedItems[0].SubItems[1].Text),
                                    new OracleParameter("id_job",listView6.SelectedItems[0].SubItems[2].Text ),
                                    new OracleParameter("val_param",strParametro),
                                    new OracleParameter("is_macro",isMacro.ToString()),
                                    new OracleParameter("counter",listView6.SelectedItems[0].SubItems[5].Text)
                                };


                    try
                    {

                        cmd.CommandText = strSql;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = cn;
                        cmd.Parameters.AddRange(parameters);
                        sqlRc = cmd.ExecuteNonQuery();
                        MessageBox.Show("Aggiornamento effettuato con successo!", "Modifica parametri ticket-job", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        caricaParametriTicketJob(id_ticket, id_job, counter, listView6);
                        comboBox21.Text = "";
                        comboBox22.Text = "";
                        checkBox1.Checked = false;


                    }
                    catch (Exception exsql)
                    {
                        MessageBox.Show(exsql.Message, "Modifica ticket-job param - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }



                    Console.WriteLine("SQL:" + strSql + " - RC:" + sqlRc);

                    JpubDb.Close();

                    //refreshTicketsJobsList();
                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Aggiungi nuovo ticke-jobt - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("inserisciTicketJob Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }
        }

        private void btnEliminaTicketJobParam_Click(object sender, EventArgs e)
        {

            String strSql = "";
            int sqlRc = 0;



            if (listView6.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Nessuna riga selezionata in lista parametri!", "Elimina Ticket-Job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                listView6.Focus();
                return;
            }



            if (MessageBox.Show("Eliminare i parametri dell'associazione nel database?", "Elimina Ticket-Job Parameter", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }


            try
            {
                //Modifica nuovo ticket->job
                JpubOraDb JpubDb = new JpubOraDb();

                if (JpubDb.Connect() == true)
                {


                    OracleConnection cn = JpubDb.getJpubOraDbConn();
                    OracleCommand cmd = new OracleCommand();

                    int isMacro = 0;
                    if (checkBox1.Checked == true)
                    {
                        isMacro = 1;
                    }



                    String id_ticket = listView6.SelectedItems[0].SubItems[1].Text;
                    String id_job = listView6.SelectedItems[0].SubItems[2].Text;
                    String counter = listView6.SelectedItems[0].SubItems[5].Text;

                    OracleParameter[] parameters = new OracleParameter[]{
                                    new OracleParameter("name_param",listView6.SelectedItems[0].SubItems[0].Text),
                                    new OracleParameter("id_ticket",listView6.SelectedItems[0].SubItems[1].Text),
                                    new OracleParameter("id_job",listView6.SelectedItems[0].SubItems[2].Text ),
                                    new OracleParameter("counter",listView6.SelectedItems[0].SubItems[5].Text)
                                };

                    strSql = "delete from wp1_instance_parameters  ";
                    strSql += " where name_param = :name_param and id_ticket = :id_ticket and id_job = :id_job and counter = :counter ";


                    try
                    {


                        cmd.CommandText = strSql;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = cn;
                        cmd.Parameters.AddRange(parameters);
                        sqlRc = cmd.ExecuteNonQuery();
                        MessageBox.Show("Eliminazione effettuata con successo!", "Modifica parametri ticket-job", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        caricaParametriTicketJob(id_ticket, id_job, counter, listView6);
                        comboBox21.Text = "";
                        comboBox22.Text = "";
                        checkBox1.Checked = false;

                    }
                    catch (Exception exsql)
                    {
                        MessageBox.Show(exsql.Message, "Elimina ticket-job param - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }



                    Console.WriteLine("SQL:" + strSql + " - RC:" + sqlRc);

                    JpubDb.Close();

                    // refreshTicketsJobsList();
                }
                else
                {
                    MessageBox.Show("Errore durante la connessione al database!", "Aggiungi nuovo ticke-jobt - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("eliminaTicketJob Error:" + ex.Message + "\nSQL:" + strSql + " - RC:" + sqlRc);
            }

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void contextMenuBuilder_Opening(object sender, CancelEventArgs e)
        {

            aggiungiOperation.Visible = false;
            aggiungiCondition.Visible = false;
            eliminaOperation.Visible = false;
            eliminaCondition.Visible = false;
            toolStripSeparator7.Visible = false;
            toolStripSeparator8.Visible = false;
            toolStripSeparator9.Visible = false;


            if (dropDownTreeView1.SelectedNode != null)
            {
                if (dropDownTreeView1.SelectedNode.Text.CompareTo(strSelectJob) == 0)
                {
                    aggiungiJob.Visible = false;
                    proprietàToolStripMenuItem.Visible = false;
                    toolStripSeparator5.Visible = false;

                }
                else
                {
                    aggiungiJob.Visible = true;
                    proprietàToolStripMenuItem.Visible = true;
                    toolStripSeparator5.Visible = true;
                }

            }
            else
            {
                if (dropDownTreeView1.Nodes.Count > 0)
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    proprietàToolStripMenuItem.Visible = false;
                    toolStripSeparator5.Visible = false;
                }
            }


            try
            {
                if (dropDownTreeView1.SelectedNode != null && dropDownTreeView1.SelectedNode.Text.Length > 11 && dropDownTreeView1.SelectedNode.Text.Substring(0, 11).CompareTo("Conditions(") == 0)
                {
                    aggiungiCondition.Visible = true;
                }

                if (dropDownTreeView1.SelectedNode != null && dropDownTreeView1.SelectedNode.Parent.Text.ToLower().Substring(0, 11).CompareTo("conditions(") == 0)
                {
                    contextMenuConditionItem = true;
                    eliminaCondition.Visible = true;
                    toolStripSeparator7.Visible = false;
                }
                else
                    contextMenuConditionItem = false;

            }
            catch (Exception ex)
            {
                contextMenuConditionItem = false;
            }


            try
            {
                if (dropDownTreeView1.SelectedNode != null && dropDownTreeView1.SelectedNode.Text.Length > 11 && dropDownTreeView1.SelectedNode.Text.Substring(0, 11).CompareTo("Operations(") == 0)
                {
                    aggiungiOperation.Visible = true;
                }

                if (dropDownTreeView1.SelectedNode != null && dropDownTreeView1.SelectedNode.Parent.Text.ToLower().Substring(0, 11).CompareTo("operations(") == 0)
                {
                    eliminaOperation.Visible = true;
                }

            }
            catch (Exception ex)
            {
            }




            Console.WriteLine("MENU APERTO!" + setOptionalToolStripMenuItem.Visible);
            if (contextMenuConditionItem == true)
            {
                setOptionalToolStripMenuItem.Visible = true;
                toolStripSeparator7.Visible = true;
            }
            else
            {
                setOptionalToolStripMenuItem.Visible = false;
                toolStripSeparator7.Visible = false;
            }

        }

        private void setOptionalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dropDownTreeView1.SelectedNode == null)
                    return;

                if (dropDownTreeView1.SelectedNode.Text.CompareTo(strSelezionaCondition) == 0)
                    return;

                if (dropDownTreeView1.SelectedNode.Text.CompareTo(strSelezionaOperation) == 0)
                    return;

                ToolStripMenuItem mnuItem = sender as ToolStripMenuItem;
                DropDownTreeNode dd = new DropDownTreeNode();
                String txtNode = "";

                if (sender.ToString().CompareTo("Set/Unset Optional") == 0)
                {


                    dd = (DropDownTreeNode)dropDownTreeView1.SelectedNode;
                    txtNode = dropDownTreeView1.SelectedNode.Text;
                    int index = -1;
                    if (txtNode.Substring(txtNode.Length - 1, 1).CompareTo("*") == 0)
                    {
                        txtNode = txtNode.Substring(0, txtNode.Length - 1);
                        index = dd.ComboBox.FindString(txtNode);
                    }
                    else
                    {
                        index = dd.ComboBox.FindString(txtNode);
                        txtNode += "*";
                    }

                    dropDownTreeView1.SelectedNode.Text = txtNode;
                    dd.ComboBox.Items[index] = txtNode;
                }
            }
            catch (Exception ex)
            {
            }



        }

        private void salvaFileCsvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frmExportCSV = new frmExportCSV();
            frmExportCSV.ShowDialog(this);
        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {


            comboBox2.Enabled = true;
            comboBox8.Width = 472;

            label30.Visible = true;
            comboBox2.Visible = true;
            label22.Visible = true;
            btnCopia.Visible = false;

            if (comboBox11.Text.CompareTo("TABLE") == 0)
                comboBox10.Enabled = false;
            else
                comboBox10.Enabled = true;


            String strSQl = "";

            if (comboBox9.Text.CompareTo("COMPLEX") == 0 || comboBox9.Text.CompareTo("RAWSTATEMENT") == 0)
            {
                comboBox11.Text = "STRING";
                comboBox11.Enabled = false;
                comboBox10.Text = "";


                comboBox8.Items.Clear();
                comboBox2.Items.Clear();
                comboBox2.Text = "";
                comboBox8.Text = "";


                if (comboBox9.Text.CompareTo("COMPLEX") == 0)
                {
                    Application.DoEvents();
                    label30.Visible = false;
                    comboBox2.Visible = false;
                    label22.Visible = false;
                    btnCopia.Visible = false;
                    comboBox8.Width = 814;
                    comboBox10.Enabled = false;
                    comboBox10.Text = "N\\A";

                    comboBox2.Enabled = false;
                    label35.Text = "Packages";
                    Application.DoEvents();
                    this.Refresh();


                    JpubOraDb JpubDb = new JpubOraDb();

                    if (JpubDb.Connect() == true)
                    {

                        Cursor.Current = Cursors.WaitCursor;

                        OracleConnection cn = JpubDb.getJpubOraDbConn();
                        OracleDataReader reader;
                        OracleCommand cmd = new OracleCommand();


                        strSQl = "select  ";
                        strSQl += "    uo.OBJECT_NAME || decode(ap.procedure_name,NULL,'','.'||ap.procedure_name)     ";
                        strSQl += "from ";
                        strSQl += "user_objects uo, ";
                        strSQl += "all_procedures ap ";
                        strSQl += "where  ";
                        strSQl += "uo.object_name = ap.object_name and ";
                        strSQl += "uo.OBJECT_TYPE in ('PROCEDURE','PACKAGE BODY') ";
                        strSQl += "order by 1 ";

                        cmd.CommandText = strSQl;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = cn;

                        reader = cmd.ExecuteReader();

                        cbAdd(comboBox8, "");
                        while (reader.Read())
                        {
                            cbAdd(comboBox8, reader[0].ToString());
                        }

                        reader.Close();
                        JpubDb.Close();
                        comboBox8.SelectedIndex = 0;


                    }
                    else
                    {
                        MessageBox.Show(JpubDb.getLastError(), "Errore Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        JpubDb = null;
                    }
                    Cursor.Current = Cursors.Default;

                }
                else
                {
                    label35.Text = "Table";
                    caricaField(comboBox8);
                }


            }
            else
            {
                if (label35.Text.CompareTo("Table") == 0 && comboBox8.Items.Count > 0)
                {
                    comboBox11.Enabled = true;
                    return;
                }

                label35.Text = "Table";
                caricaField(comboBox8);
                comboBox11.Enabled = true;
                comboBox10.Text = "";

            }

        }

        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox23.Text = "";
            comboBox24.Text = "";
            dateTimePicker4.Visible = false;

            if (comboBox11.Text.CompareTo("TABLE") == 0)
            {
                caricaField(comboBox24);
                comboBox10.Text = "";
                comboBox10.Enabled = false;

                label49.Visible = true;
                label50.Visible = true;
                label51.Visible = true;
                comboBox24.Visible = true;
                comboBox23.Visible = true;
                comboBox10.Visible = false;
                label24.Visible = false;

            }
            else
            {
                comboBox10.Enabled = true;
                label49.Visible = false;
                label50.Visible = false;
                label51.Visible = false;
                comboBox24.Visible = false;
                comboBox23.Visible = false;
                comboBox10.Visible = true;
                label24.Visible = true;


                if (comboBox11.Text.CompareTo("DATE") == 0)
                {
                    comboBox10.Width = 513;
                    dateTimePicker4.Visible = true;
                }
                else
                {
                    comboBox10.Width = 652;
                    dateTimePicker4.Visible = false;
                }
                this.Refresh();

            }

        }

        private void comboBox16_SelectedIndexChanged(object sender, EventArgs e)
        {



            String strSQl = "";
            comboBox13.Text = "";
            //comboBox14.Text = "";
            comboBox15.Text = "";
            //comboBox17.Text = "";
            comboBox15.Enabled = true;


            comboBox13.Enabled = true;
            comboBox14.Enabled = true;
            comboBox15.Enabled = true;
            comboBox17.Enabled = true;


            if (comboBox16.Text.CompareTo("DELETE") == 0)
            {
                comboBox13.Text = "ALL";
                comboBox14.Text = "N\\A";
                comboBox15.Text = "N\\A";
                comboBox17.Text = "";
                comboBox13.Enabled = false;
                comboBox14.Enabled = false;
                comboBox15.Enabled = false;
                comboBox17.Enabled = false;
            }
            else if (comboBox16.Text.CompareTo("SET") == 0)
            {
                caricaField(comboBox17);
            }




            if (comboBox16.Text.CompareTo("COMPLEX") == 0 || comboBox16.Text.CompareTo("RAWSTATEMENT") == 0 || comboBox16.Text.CompareTo("POSTCOMPLEX") == 0)
            {


                if (comboBox16.Text.CompareTo("COMPLEX") == 0 || comboBox16.Text.CompareTo("POSTCOMPLEX") == 0)
                {

                    label37.Visible = false;
                    comboBox14.Visible = false;
                    button1.Visible = false;
                    label36.Visible = false;
                    comboBox17.Width = 724;
                    comboBox17.Items.Clear();
                    comboBox17.Text = "";
                    label33.Text = "Package";

                    comboBox15.Text = "N\\A";
                    comboBox15.Enabled = false;
                    this.Refresh();





                    JpubOraDb JpubDb = new JpubOraDb();

                    if (JpubDb.Connect() == true)
                    {

                        Cursor.Current = Cursors.WaitCursor;

                        OracleConnection cn = JpubDb.getJpubOraDbConn();
                        OracleDataReader reader;
                        OracleCommand cmd = new OracleCommand();


                        strSQl = "select  ";
                        strSQl += "    uo.OBJECT_NAME || decode(ap.procedure_name,NULL,'','.'||ap.procedure_name)     ";
                        strSQl += "from ";
                        strSQl += "user_objects uo, ";
                        strSQl += "all_procedures ap ";
                        strSQl += "where  ";
                        strSQl += "uo.object_name = ap.object_name and ";
                        strSQl += "uo.OBJECT_TYPE in ('PROCEDURE','PACKAGE BODY') ";
                        strSQl += "order by 1 ";

                        cmd.CommandText = strSQl;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = cn;

                        reader = cmd.ExecuteReader();

                        cbAdd(comboBox17, "");
                        while (reader.Read())
                        {
                            cbAdd(comboBox17, reader[0].ToString());
                        }

                        reader.Close();
                        JpubDb.Close();
                        comboBox17.SelectedIndex = 0;


                    }
                    else
                    {
                        MessageBox.Show(JpubDb.getLastError(), "Errore Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        JpubDb = null;
                    }
                    Cursor.Current = Cursors.Default;

                }
                else
                {
                    comboBox17.Items.Clear();
                    comboBox17.Text = "";
                    caricaField(comboBox17);
                    label37.Visible = true;
                    comboBox14.Visible = true;
                    button1.Visible = false;
                    label36.Visible = true;
                    comboBox17.Width = 387;
                    label33.Text = "Table";
                    this.Refresh();
                }


            }
            else
            {
                if (label33.Text.CompareTo("Table") == 0 && comboBox17.Items.Count > 0)
                {

                    if (comboBox16.Text.CompareTo("DELETE") != 0)
                    {
                        comboBox14.Text = "";
                        comboBox15.Enabled = true;
                        caricaField(comboBox17);
                        label37.Visible = true;
                        comboBox14.Visible = true;
                        button1.Visible = false;
                        label36.Visible = true;
                        comboBox17.Width = 387;
                        label33.Text = "Table";
                        this.Refresh();
                    }
                    else
                        comboBox15.Enabled = false;




                    return;
                }

                if (comboBox16.Text.CompareTo("SET") == 0)
                {
                    comboBox14.Text = "";
                    comboBox15.Enabled = true;
                    caricaField(comboBox17);
                    label37.Visible = true;
                    comboBox14.Visible = true;
                    button1.Visible = false;
                    label36.Visible = true;
                    comboBox17.Width = 387;
                    label33.Text = "Table";
                    this.Refresh();
                    return;
                }



                comboBox17.Items.Clear();
                comboBox17.Text = "";



                label37.Visible = true;
                comboBox14.Visible = true;
                button1.Visible = false;
                label36.Visible = true;
                comboBox17.Width = 387;
                label33.Text = "Table";
                this.Refresh();


            }
        }

        private void comboBox24_SelectedIndexChanged(object sender, EventArgs e)
        {
            caricaColumns(comboBox23, comboBox24.Text);
        }

        private void comboBox23_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox10.Text = comboBox24.Text + "." + comboBox23.Text;
        }

        private void dateTimePicker4_ValueChanged(object sender, EventArgs e)
        {
            comboBox10.Text = dateTimePicker4.Text;
        }

        public static bool IsDateTime(string txtDate)
        {
            DateTime tempDate;

            return DateTime.TryParse(txtDate, out tempDate) ? true : false;
        }


        /// <summary>
        /// Questo metodo controlla se un valore è numerico
        /// </summary>
        /// <param name="s">String</param>
        /// <return>Boolean</reuturn>
        private bool IsNumeric(string s)
        {
            float output;
            return float.TryParse(s, out output);
        }



        public static bool checkDateFormat(string txtDate)
        {
            bool rc = false;



            string regExpressionString = @"^(0[1-9]|1[012])/(0[1-9]|[12][0-9]|3[01])/(19|20)\d\d$";


            if (System.Text.RegularExpressions.Regex.IsMatch(txtDate, regExpressionString))
            {
                rc = true;
            }



            return (rc);

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }



        /// <summary>
        /// Questo metodo l'intervallo di date di un ticket
        /// </summary>
        /// <param name="txtDataInizio">String</param>
        /// <param name="txtDataFine">String</param>
        /// <return>Boolean</reuturn>
        public static bool checkDateTicket(string txtDataInizio, string txtDataFine)
        {
            bool rc = true;

            Console.WriteLine("DT1:" + txtDataInizio + " DT2:" + txtDataFine);

            DateTime oDateInizio = DateTime.ParseExact(txtDataInizio, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime oDateFine = DateTime.ParseExact(txtDataFine, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            if (oDateFine < oDateInizio)
            {
                MessageBox.Show("Data Fine minore di Data Inizio!", "Errore Ticket", MessageBoxButtons.OK, MessageBoxIcon.Error);
                rc = false;
            }

            return (rc);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBox5.Text.CompareTo("SAP") == 0)
                return;

            if (comboBox1.Text.CompareTo("") == 0 || comboBox1.Text.CompareTo("ALL") == 0)
            {
                caricaCodeTipoContrTicket(comboBox25, comboBox1.Text, true);
                //comboBox25.Enabled = false;
            }
            else
            {
                caricaCodeTipoContrTicket(comboBox25, comboBox1.Text, false);
                comboBox25.Enabled = true;
            }
        }


        /// <summary>
        /// Questo metodo se il valore di un parametro è corretto
        /// </summary>
        /// <param name="txtParam">String</param>
        /// <return>Boolean</reuturn>
        public static bool checkNameParam(String txtParam)
        {
            bool brc = false;


            if (txtParam.Substring(0, 1).CompareTo("$") != 0)
                return (brc);

            if (txtParam.Substring(txtParam.Length - 1, 1).CompareTo("$") != 0)
                return (brc);

            if (txtParam.CompareTo("$$") == 0)
                return (brc);

            if (txtParam.IndexOf(" ") != -1)
                return (brc);


            int count = txtParam.Split('$').Length - 1;

            if (count > 2)
                return (brc);

            brc = true;

            return (brc);
        }

        private void comboBox25_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                Console.WriteLine("ITEM SELECTED: " + (comboBox25.SelectedItem as ComboboxItem).Value.ToString());
            }
            catch (Exception ex)
            {
            }
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnCancellaTutto_Click(object sender, EventArgs e)
        {
            if (dropDownTreeView1.Nodes.Count <= 0)
                return;

            if (dropDownTreeView1.Nodes.Count > 0)
            {
                if (MessageBox.Show("Cancellare il contenuto dell'alberatura?", "Job Builder", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }


            ClearTreeView();
        }

        private void eliminaOperation_Click(object sender, EventArgs e)
        {
            dropDownTreeView1.Nodes.Remove(dropDownTreeView1.SelectedNode);
            dropDownTreeView1.Nodes[0].Nodes[0].Text = "Operations(" + dropDownTreeView1.Nodes[0].Nodes[0].Nodes.Count + ")";
        }

        private void eliminaCondition_Click(object sender, EventArgs e)
        {
            dropDownTreeView1.Nodes.Remove(dropDownTreeView1.SelectedNode);
            dropDownTreeView1.Nodes[0].Nodes[1].Text = "Conditions(" + dropDownTreeView1.Nodes[0].Nodes[1].Nodes.Count + ")";
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, ".\\help.chm");
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox5.Text.CompareTo("SAP") == 0)
            {
                comboBox25.Items.Clear();
                cbAdd(comboBox25, "null", "null");
                cbAdd(comboBox25, "FASE 1", "1");
                cbAdd(comboBox25, "FASE 2", "2");
                cbAdd(comboBox25, "FASE 3", "3");
            }
            else
                caricaCodeTipoContrTicket(comboBox25, comboBox1.Text, true);

            comboBox1_SelectedIndexChanged(null, null);
        }



        private void copiaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                STR_CLIPBOARD_UTILITY = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue.ToString();
            }
            catch (Exception ex)
            {
            }

        }

        private void contextMenuUtilityAccount_Opening(object sender, CancelEventArgs e)
        {

        }

        private void contextMenuUtilityAccount_Click(object sender, EventArgs e)
        {
            try
            {
                if (STR_CLIPBOARD_UTILITY != null)
                    System.Windows.Forms.Clipboard.SetText(STR_CLIPBOARD_UTILITY);
            }
            catch (Exception ex)
            {
            }

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                STR_CLIPBOARD_UTILITY = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue.ToString();
            }
            catch (Exception ex)
            {
            }


        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                STR_CLIPBOARD_UTILITY = dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue.ToString();
            }
            catch (Exception ex)
            {
            }

        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox6.Text.CompareTo("SAP") == 0)
            {
                comboBox18.Items.Clear();
                cbAdd(comboBox18, "ALL", "null");
                cbAdd(comboBox18, "FASE 1", "1");
                cbAdd(comboBox18, "FASE 2", "2");
                cbAdd(comboBox18, "FASE 3", "3");
            }
            else
                caricaCodeTipoContrTicket(comboBox18, comboBox6.Text, true);

            // comboBox1_SelectedIndexChanged(null, null);
        }







    }
}
