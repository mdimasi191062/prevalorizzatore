/*
 * JpubEngine.cpp
 *
 *  Created on: 03/ott/2014
 *      Author: Marco
 */

#include <JpubEngine.hpp>
#include <sys/time.h>
#include <sstream>



using namespace std;
using namespace oracle::occi;
DB_Manager *DB;
Util *Utl;

void onError(string message) {

    Logger *log = Logger::Instance();
    log->Log(ERR, true, message.c_str());
    DB->RollBack();
    log->write_log_elab_dett(KO, message);
}

std::string checkIfTrasform(string type, string value) {
    string strValue = "";
    string typeCol = DB->getTypeFromColumn(type);

    if (typeCol.compare("DATE") == 0) {
        strValue = "TO_DATE('" + value + "', 'DD/MM/YYYY')";
    } else if ((typeCol.compare("CHAR") == 0) || (typeCol.compare("VARCHAR") == 0) || (typeCol.compare("VARCHAR2") == 0)) {
        strValue = "'" + value + "'";
    } else {
        strValue = value;
    }

    return strValue;
}

bool checkInputParameters(int argc, char **argv, string &OperationMode, string &Service, string &strDataFineCiclo) {
    bool result = true;

    if (argc != 4) {
        cerr << "Jpub engine error: numero di parametri errati [" << argc << "]." << endl;
        cerr << "Parametri Attesi: PREVAL/POSTVAL/[...] CODE_TIPO_CONTR YYYYmmdd" << endl;
        result = false;
    } else {
        if (strcmp(argv[1], "") == 0) {
            cerr << "Jpub engine error: 1 parametro ricevuto = NULL ." << endl;
            result = false;
        } else {
            OperationMode = argv[1];
        }

        if (Utl->isnumber(argv[2])) {
            Service = argv[2];
            cout << "CODE_TIPO_CONTR: " << Service << endl;
        } else {
            cerr << "Jpub engine error: 2 parametro ricevuto: [" << argv[2] << "], 2 parametro atteso come number:(0..9)" << endl;
            result = false;
        }
        result = Utl->IsDateValid(argv[3], strDataFineCiclo);

        if (result != false)
            cout << "strDataFineCiclo: " << strDataFineCiclo << endl;
    }

    return result;
}

bool checkEnvironment(string &strUser, string &strPassword, string &strConnect) {
    bool result = true;

    if (getenv("JW1USER") != NULL)
        strUser = getenv("JW1USER");
    else {
        cerr << "Environment variable JW1USER must be defined." << endl;
        result = false;
    }

    if (getenv("JW1PASSWD") != NULL)
        strPassword = getenv("JW1PASSWD");
    else {
        cerr << "Environment variable JW1PASSWD must be defined." << endl;
        result = false;
    }

    if (getenv("JW1CONNECT") != NULL)
        strConnect = getenv("JW1CONNECT");
    else {
        cerr << "Environment variable JW1CONNECT must be defined." << endl;
        result = false;
    }
    return result;
}

int main(int argc, char **argv) {
    bool status;
    int result = 0;
    int exitResult = 0;
    long TimeStamp;
    string OperationMode; //0 = PRE VAL  , 1 = POST VAL
    string Service;
    string strUser;
    string strPassword;
    string strConnect;
    string strMessage = "";
    string stmSqlUpdate = "";
    string strDataFineCiclo = "";
    string Sistema = "";
    int tipoSel = -1; //0: Semplice, 1:Complex


    Config *Cfg;
    Ticket *tTicket;
    Job *tJob = NULL;
    Logger *Log;

    status = checkInputParameters(argc, argv, OperationMode, Service, strDataFineCiclo);
    if (status == false) {
        return 1;
    }

    status = checkEnvironment(strUser, strPassword, strConnect);
    if (status == false) {
        return 1;
    }

    Utl = Util::Instance();

    DB = DB_Manager::Instance();
    DB->setCredenziali(strUser, strPassword, strConnect);
    status = DB->connectDB();

    cout << "Stato Connessione Database: " << status << endl;

    if (status == 1) {
        TimeStamp = Utl->getTimeStamp();
        DB->setTS(TimeStamp);
        
        Log = Logger::Instance();
        Cfg = Config::Instance();        
        
        result = Log->Init(OperationMode, TimeStamp);
        
        if (result == -1) {
            DB->DisconnectDB();
            exit(1);
        }
            
        try {

            //status = Cfg->insertTempConfig("$TS$", Utl->toString(TimeStamp));
            Log->Log(INF, true, "Inzio Elaborazione per servizio %s e data %s", Service.c_str(), strDataFineCiclo.c_str());
            Sistema = DB->checkService(Service);
            if (Sistema.length() == 0) {
                strMessage = "Errore. Parametro Service [" + Service + "] non presente nella tabella WP1_SERVICES.";
                cerr << strMessage << endl;
                Log->Log(ERR, true, strMessage.c_str());
                DB->DisconnectDB();
                exit(1);
            }

            Log->Log(INF, false, "DB Username  -> %s.", strUser.c_str());
            //Log->Log(INF, false, "DB Password  -> %s.", strPassword.c_str());
            Log->Log(INF, false, "DB Connect   -> %s.", strConnect.c_str());
            Log->Log(INF, "OperationMode-> %s.", OperationMode.c_str());
            
            try {
                Log->Log(INF, false, "Carico le configurazioni globali");
                Cfg->initWp1JobConfig(Service, Sistema, strDataFineCiclo, TimeStamp, argv[1]);
                Cfg->readConfig(OperationMode); //Legge da Config prima ALL e poi PRE/POST/... 
            } catch (SQLException& ex) {
                int errno = ex.getErrorCode();
                Logger::Instance()->Log(ERR, false, "%s \n\t Error: %d -> %s", ex.what(), errno, ex.getMessage().c_str());
                DB->DisconnectDB();
                exit(1);
            }
            DB->Commit(); // AZZERO LE TABELLE TEMPORANEE

            if (result < 0) {
                strMessage = "Errore nel creare la tabella temporanea Wp1Job_TEMP con i parametri globali. Esecuzione abortita.";
                cerr << strMessage << endl;
                Log->Log(ERR, true, strMessage.c_str());
                exit (1);
            }
            tTicket = new Ticket();
            int numTicket = tTicket->Load(Cfg, Service, Sistema, OperationMode);
            Log->Log(INF, true, "Inizio l'elaborazione. Trovati %d Ticket ", numTicket);
        } catch (SQLException& ex) {
            int errno = ex.getErrorCode();
            Logger::Instance()->Log(ERR, false, "%s \n\t Error: %d -> %s", ex.what(), errno, ex.getMessage().c_str());
            DB->DisconnectDB();
            exit(1);
        } catch (...) {
            Logger::Instance()->Log(ERR, false, "%s \n\t Errore non previsto nella configurazione del motore");
            DB->DisconnectDB();
            exit(1);
        }

        while (tTicket->getCount() > 0) {

            ObjTicket rTicket = tTicket->getTicket();
            DB->setTicket(rTicket.ID_TICKET);
            Log->Log(INF, true, "IN ELABORAZIONE IL TICKET %s", rTicket.ID_TICKET.c_str());
            tJob = new Job(); //Contenitore di oggetti JOB

            int numJob = tJob->Load(Cfg, rTicket.ID_TICKET, rTicket.TICKET_TYPE, Service);

            Log->Log(INF, false, "TROVATI %d JOB", numJob);

            int cntJob = 0;
            Log->write_log_elab(RUN, "Inizio Elaborazione Ticket.");

            bool flagJobStatus = false;
            bool flagTiketStatus = true;

            while (tJob->getCount() > 0) {
                flagJobStatus = true;
                ObjJob rJob = tJob->getJob();
                Selettore Sel;
                try {
                    cntJob++;

                    DB->setJob(rJob.GetIdJob(), rJob.getCounter());
                    Log->Log(INF, true, "IN ELABORAZIONE IL JOB %s", rJob.GetIdJob().c_str());
                    Log->write_log_elab_dett(RUN, "IN ELABORAZIONE IL JOB " + rJob.GetIdJob());
                    Log->Log(INF, false, "ESEGUO LE OPERAZIONI PRELIMINARI");
                    Cfg->initWp1JobConfig(Service, Sistema, strDataFineCiclo, TimeStamp, argv[1]);
                    Cfg->CaricaWp1Config();
                    Cfg->insertTempWp1Job("$ID_JOB$", rJob.GetIdJob());
                    Cfg->insertTempWp1Job("$MASTERTABLE$", rJob.GetMST());
                    Cfg->insertTempWp1Job("$ID_TICKET$", rTicket.ID_TICKET);

                    rJob.GetAndSetParametersTo_WP1_JOB(DB, Cfg);
                    status = rJob.SetMasterTable(Cfg); //Setta i campi: nome | tipo della master table                     
                    result = Sel.SetKeyForMasterTable(rJob.GetMST(), rJob.GetSelectForMasterTable());

                    if (result == 1) {
                        Log->Log(INF, true, "CERCO I SELETTORI");
                        tipoSel = Sel.SetSelectWhereCondition(rJob.GetIdJob()); //0: Semplice, 1:Complex
                        Statement *stmt = NULL;
                        ResultSet *rs = NULL;
                        if(rJob.isLogKey())
                        {
                            Log->Log(INF,true,"Richiesto il log delle chiavi sulla tabella WP1_LOG_KEY");
                             if (tipoSel == 0) {
                                    Sel.InsertIntoTableKey(rJob.tabKeyType);
                             }
                            DB->executePackageLOG_KEY();
                        }
                        
                        Log->Log(INF, true, "CERCO GLI OPERATORI");
                        string stmtSql = "select WP1_OPERATIONS.ID_OPERATION, WP1_OPERATIONS.FIELD, WP1_OPERATIONS.OPERATOR, WP1_OPERATIONS.OPERAND ";
                        stmtSql += "from WP1_JOB INNER JOIN WP1_JOB_OPERATION ON (WP1_JOB.ID_JOB = WP1_JOB_OPERATION.ID_JOB) ";
                        stmtSql += "INNER JOIN WP1_OPERATIONS  ON (WP1_OPERATIONS.ID_OPERATION = WP1_JOB_OPERATION.ID_OPERATION) ";
                        stmtSql += "where  WP1_OPERATIONS.OPERATOR!='POSTCOMPLEX' and WP1_JOB.ID_JOB = '" + rJob.GetIdJob() + "' order by WP1_OPERATIONS.ID_OPERATION";

                        rs = DB->executeSelectStmt(stmtSql, stmt);

                        bool createSqlUpdate = false;
                        stmSqlUpdate = "";
                        string strValueForOperand = "";

                        if (rs) {

                            bool fResultsetFull = false;

                            while (rs->next() == true) {

                                fResultsetFull = true;
                                DB->setOperation(rs->getString(1));
                                Log->Log(INF, true, "ELABORO L'OPERATORE %s ", rs->getString(1).c_str());
                                Log->Log(DBG, false, "Operatore di tipo %s", rs->getString(2).c_str());

                                Cfg->insertTempWp1Job("$ID_OPERATION$", rs->getString(1));

                                //GESTIONE SULL'OPERATORE ..............							 
                                if (rs->getString(3).compare("COMPLEX") == 0) {

                                    //ESEGUO SOLO IL PACKAGE
                                    Log->Log(INF, true, "TROVATO OPERATORE COMPLEX ");

                                    if (tipoSel == 0) {
                                        Log->Log(INF, false, "INSERISCO LE CHIAVI NELLA TABELLA TEMPORANEA...");
                                        Sel.InsertIntoTableKey(rJob.tabKeyType);
                                    }
                                    Log->Log(INF, false, "ESEGUO L'OPERATORE...");
                                    DB->executePackageTableKey(rs->getString(2), rs->getString(1));

                                } else if (rs->getString(3).compare("DELETE") == 0) {

                                    Log->Log(INF, true, "TROVATO OPERATORE DELETE");
                                    stmSqlUpdate = "DELETE FROM " + rJob.GetMST() + " ";
                                    createSqlUpdate = true;
                                } else {
                                    Log->Log(INF, true, "TROVATO OPERATORE SEMPLICE");
                                    createSqlUpdate = true;
                                    strValueForOperand = rs->getString(4);
                                    if (strValueForOperand.find("$", 0) != std::string::npos) {
                                        strValueForOperand = Cfg->convertOperand(strValueForOperand);
                                        if (strValueForOperand.compare("nul") == 0)
                                            throw runtime_error("Impossibile risolvere operando " + rs->getString(4));
                                    }
                                    if (rs->getString(3).compare("RAWSTATEMENT") != 0) {
                                        strValueForOperand = checkIfTrasform(rs->getString(2), strValueForOperand);
                                    }

                                    Log->Log(DBG, false, "OPERAND %s", strValueForOperand.c_str());
                                    if (stmSqlUpdate.compare("") == 0) { //Se 1 STEP non necessita della virgola finale
                                        stmSqlUpdate = " update " + rJob.GetMST() + " set " + rs->getString(2) + " = " + strValueForOperand;
                                    } else {
                                        stmSqlUpdate += ", " + rs->getString(2) + "=" + strValueForOperand;
                                    }
                                }
                            } // END WHILE RECORDSET 	

                            if (fResultsetFull == true) {
                                DB->closeRecordset(stmt, rs);

                                if (createSqlUpdate) {
                                    stmSqlUpdate += " where " + rJob.GetTableWhithKey();
                                    if (tipoSel == 1) { // COMPLEX
                                        Sel.SetStmtFrom(rJob.GetStatementFromTableKey());
                                    }
                                    Sel.MakeSQL(stmSqlUpdate);
                                    result = Sel.ExecuteSQL();
                                    Log->Log(INF, true, "Effettuato l'update di %d righe", result);
                                }
                                string tmpLog = "";


                                string stmtSql = "select WP1_OPERATIONS.ID_OPERATION, WP1_OPERATIONS.FIELD, WP1_OPERATIONS.OPERATOR, WP1_OPERATIONS.OPERAND ";
                                stmtSql += "from WP1_JOB INNER JOIN WP1_JOB_OPERATION ON (WP1_JOB.ID_JOB = WP1_JOB_OPERATION.ID_JOB) ";
                                stmtSql += "INNER JOIN WP1_OPERATIONS  ON (WP1_OPERATIONS.ID_OPERATION = WP1_JOB_OPERATION.ID_OPERATION) ";
                                stmtSql += "where  WP1_OPERATIONS.OPERATOR='POSTCOMPLEX' and WP1_JOB.ID_JOB = '" + rJob.GetIdJob() + "' order by WP1_OPERATIONS.ID_OPERATION";

                                ResultSet *rsPost = DB->executeSelectStmt(stmtSql, stmt);
                                if (rsPost) {
                                    while (rsPost->next() == true) {
                                        //ESEGUO SOLO IL PACKAGE
                                        Log->Log(INF, true, "TROVATO OPERATORE POSTCOMPLEX %s", rsPost->getString(1).c_str());

                                        if (tipoSel == 0) {
                                            Log->Log(INF, false, "INSERISCO LE CHIAVI NELLA TABELLA TEMPORANEA...");
                                            Sel.InsertIntoTableKey(rJob.tabKeyType);
                                        }
                                        Log->Log(INF, false, "ESEGUO L'OPERATORE...");
                                        DB->executePackageTableKey(rsPost->getString(2), rsPost->getString(1));
                                    }
                                }

                                Log->Log(INF, true, "JOB ESEGUITO CORRETTAMENTE");
                                tmpLog = "Elaborazione eseguita correttamente.";

                                Log->write_log_elab_dett(OK, tmpLog);

                                if ((Cfg->getTempWp1Job("COMMIT").empty() == false) && (Cfg->getTempWp1Job("COMMIT").compare("1") == 0)) {
                                    Log->Log(INF, true, "ESEGUO IL COMMIT");
                                    DB->Commit();
                                } else if (Cfg->getTempWp1Job("COMMIT").empty() == true) {
                                    Log->Log(INF, true, "ESEGUO IL COMMIT");
                                    DB->Commit();
                                } else {
                                    DB->RollBack();
                                    Log->Log(INF, true, "MODALITA SIMULAZIONE! NON ESEGUO IL COMMIT");
                                }

                            } else {
                                throw runtime_error("Errore: Nessuna operazione associata al selettore.");
                            }
                        } else {
                            throw runtime_error("Errore: Nessuna operazione associata al selettore.");
                        }
                    } else {
                        throw runtime_error("Errore: La chiave primaria del selettore è errata o non esistente.");
                    }
                } catch (SQLException &ex) {
                    int errno = ex.getErrorCode();
                    flagTiketStatus = false;
                    onError(string(ex.what()) + "\n\t Error: " + Utl->toString(errno) + " -> " + ex.getMessage());

                } catch (runtime_error ex) {
                    flagTiketStatus = false;
                    onError(ex.what());
                } catch (...) {
                    flagTiketStatus = false;
                    onError("Errore non gestito nell'esecuzione del job.");
                }

                DB->setJob("", 0);
            } // FINE WHILE SU JOB


            if (flagTiketStatus) {
                if (flagJobStatus == true) {
                    Log->Log(INF, true, "TICKET ESEGUITO CORRETTAMENTE");
                    Log->write_log_elab(OK, "Elaborazione del ticket eseguita correttamente.");
                    //exitResult=0;
                } else {
                    Log->Log(INF, true, "NESSUN JOB ESEGUITO PER IL TICKET");
                    Log->write_log_elab(KO, "Errore: nessun job eseguito per il ticket.");
                    exitResult = 1;
                }
            } else {
                Log->Log(INF, true, "ERRORE NELL'ESECUZIONE DI UNO O PIU JOB DEL TICKET");
                Log->write_log_elab(KO, "Errore nell'esecuzione di uno o piu' job");
                    exitResult = 1;
            }

            flagTiketStatus = true;

            DB->setTicket("");
        } // FINE WHILE SU TICKET

    } else {
        cerr << "Errore Connessione Database Fallita !!! [" << strConnect << "]" << endl;
        //Log->Log(ERR, false, "Errore Connessione Database: [%s] Fallita.", strConnect.c_str());
        return 1;
    }
    Log->Log(INF, true, "FINE ELABORAZIONE");

    DB->DisconnectDB();
    delete DB;

    delete tJob;
    delete tTicket;
    delete Cfg;
    delete Log;
    delete Utl;
    cout << "FINE ELABORAZIONE. RETURN CODE [" << exitResult << "]" << endl;
    return exitResult;
}
