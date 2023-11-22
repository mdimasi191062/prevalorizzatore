using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Oracle.DataAccess.Client;
using System.Data.OracleClient;
using System.Xml;

namespace JPubGfx
{

    /// <summary>
    /// Classe per la gestione delle operazioni di database
    /// </summary>
    class JpubOraDb
    {
        OracleConnection con = null;
        string sIpaddress = "";
        string sPort = "";
        string sUsername = "";
        string sPassword = "";
        string sService = "";
        string sTipoServizio = "";

        string strLastError = "";


        /// <summary>
        /// Costruttore della classe JpubOraDb
        /// </summary>
        public JpubOraDb()
        {

        }



        /// <summary>
        /// Questa funzione ritorna l'oggetto connessione al database
        /// </summary>
        /// <returns>OracleConnection</returns>
        public OracleConnection getJpubOraDbConn()
        {
            return (con);
        }




        /// <summary>
        /// Metodo che apre la connessione al database senza passaggio di parametri
        /// i parametri vengono letti runtime dal file di configurazione
        /// </summary>
        /// <returns>Boolean</returns>
        public Boolean Connect()
        {
            Boolean bRc = false;

            try
            {
                if (checkDb() == true)
                {
                    bRc = Connect(sService,sIpaddress,sPort,sUsername,sPassword,sTipoServizio);
                }
            }
            catch (Exception e)
            {
                strLastError = e.Message;
                Console.WriteLine("Connected to Oracle Error" + e.Message);
            }

            return (bRc);
        }




        /// <summary>
        /// Metodo che apre la connessione al database con passaggio di parametri
        /// </summary>
        /// <param name="servizio">Nome del servizio Oracle</param>
        /// <param name="ipaddress">Ip Address</param>
        /// <param name="port">Port</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Boolean</returns>
        public Boolean Connect(string str_servizio, string str_ipaddress, string str_porta,string str_username, string str_password, string str_tiposervizio)
        {
            Boolean bRc = false;

            try
            {
                con = new OracleConnection();

                //string CONNECTION_STRING = "User Id=" + str_username + ";Password=" + str_password + ";Data Source=(DESCRIPTION=" +  "(ADDRESS=(PROTOCOL=TCP)(HOST=" + str_ipaddress + ")(PORT=" + str_porta + "))" + "(CONNECT_DATA=(SID=" + str_servizio + ")));";
                string CONNECTION_STRING = "User Id=" + str_username + ";Password=" + str_password + ";Data Source=(DESCRIPTION=" + "(ADDRESS=(PROTOCOL=TCP)(HOST=" + str_ipaddress + ")(PORT=" + str_porta + "))" + "(CONNECT_DATA=( " + str_tiposervizio + " =" + str_servizio + ")));";
                con.ConnectionString = CONNECTION_STRING;
                con.Open();
                Console.WriteLine("Connected to Oracle" + con.ServerVersion);
                bRc = true;
            }
            catch (Exception e)
            {
                strLastError = e.Message;
                Console.WriteLine("Connected to Oracle Error" + e.Message);
            }

            return (bRc);
        }




        /// <summary>
        /// Metodo che chiude la connessione al database
        /// </summary>
        public void Close()
        {
            con.Close();
            con.Dispose();
        }





        /// <summary>
        /// Questo metodo legge la configurazione dal file jpubconfig.xml
        /// </summary>
        /// <returns>Nothing</returns>
        public void ReadConfig()
        {

            try
            {
                string str_apppath = AppDomain.CurrentDomain.BaseDirectory;
                XmlTextReader textReader = new XmlTextReader(@str_apppath + "jpconfig.xml");
                textReader.Read();
                while (textReader.Read())
                {
                    if (textReader.Name == "servizio")
                    {
                        sService = textReader.ReadString();
                    }
                    else if (textReader.Name == "ipaddress")
                    {
                        sIpaddress = textReader.ReadString();
                    }
                    else if (textReader.Name == "tipo")
                    {
                        sTipoServizio = textReader.ReadString();
                    }
                    else if (textReader.Name == "porta")
                    {
                        sPort = textReader.ReadString();
                    }
                    else if (textReader.Name == "username")
                    {
                        sUsername = textReader.ReadString();
                    }
                    else if (textReader.Name == "password")
                    {
                        sPassword = textReader.ReadString();
                    }
                }
                textReader.Close();
            }
            catch (Exception e)
            {
                strLastError = e.Message;
                Console.WriteLine("ReadConfig Error: " + e.Message);
            }

        }//ReadConfig



        /// <summary>
        /// Questo metodo ritorna l'ultimo errore verificato negli altri metodi
        /// </summary>
        /// <returns>String</returns>
        public string getLastError()
        {
            return (strLastError);
        }


        /// <summary>
        /// Questo metodo ritorna il nome del servizio Oracle
        /// </summary>
        /// <returns>String</returns>
        public string getService()
        {
            return (sService);
        }

        /// <summary>
        /// Questo metodo ritorna la username del servizio Oracle
        /// </summary>
        /// <returns>string</returns>
        public string getUsername()
        {
            return (sUsername);
        }

        /// <summary>
        /// Questo metodo ritorna la password del servizio Oracle
        /// </summary>
        /// <returns>String</returns>
        public string getPassword()
        {
            return (sPassword);
        }

        /// <summary>
        /// Questo metodo ritorna l'indirizzo ip del servizio Oracle
        /// </summary>
        /// <returns>String</returns>
        public string getIpAddress()
        {
            return (sIpaddress);
        }
        /// <summary>
        /// Questo metodo ritorna il numero di porta del servizio Oracle
        /// </summary>
        /// <returns>String</returns>
        public string getPort()
        {
            return (sPort);
        }

        /// <summary>
        /// Questo metodo ritorna il tipo di servizio Oacle (SID,SERVICE)
        /// </summary>
        /// <returns>String</returns>
        public string getTipoService()
        {
            return (sTipoServizio);
        }

        /// <summary>
        /// Metodo che scrive i dati di connessione al servizio Oracle nel file jpconfig.xml
        /// </summary>
        public void WriteDbConfig(string str_servizio, string str_ipaddress, string str_porta, string str_username, string str_pw, string str_servicetype)
        {

            try
            {
                string str_apppath = AppDomain.CurrentDomain.BaseDirectory;
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@str_apppath + "jpconfig.xml"))
                {
                    file.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    file.WriteLine("<configurazione>");
                    file.WriteLine("    <database>");
                    file.WriteLine("        <servizio>" + str_servizio + "</servizio>");
                    file.WriteLine("        <tipo>" + str_servicetype + "</tipo>");
                    file.WriteLine("        <ipaddress>" + str_ipaddress + "</ipaddress>");
                    file.WriteLine("        <porta>" + str_porta + "</porta>");
                    file.WriteLine("        <username>" + str_username + "</username>");
                    file.WriteLine("        <password>" + str_pw + "</password>");
                    file.WriteLine("    </database>");
                    file.WriteLine("</configurazione>");
                }
            }
            catch (Exception e)
            {
                strLastError = e.Message;
                Console.WriteLine("WriteDbConfig Error: " + e.Message);
            }
        }





        /// <summary>
        /// Metodo che controlla se esistono i parametri del database
        /// </summary>
        /// <returns>Boolean</returns>
        public Boolean checkDb()
        {
            Boolean bRc = false;

            ReadConfig();

            if (sService != "" && sUsername != "" && sPassword != "" && sIpaddress != "" && sPort != "" && sTipoServizio != "")
            {
                bRc = true;
            }

            return (bRc);
        }




    }
}
