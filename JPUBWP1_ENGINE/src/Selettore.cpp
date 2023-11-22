/*
 * Selettore.cpp
 *
 *  Created on: 31/ott/2014
 *      Author: Marco
 */


#include <string>
#include <iostream>
#include <Selettore.hpp>


#ifndef OCCI_ORACLE
#include <occi.h>
#endif

Selettore::Selettore() // Constructor
{
    stmtSelect = "";
    stmtFrom = "";
    stmtWhere = "";
    stmtSQL = "";
    MASTERTABLE = "";
    keyInsertedIntoTemporaryTable = false;
    Utl = Util::Instance();
    Cfg = Config::Instance();
    DB = DB_Manager::Instance();
    Log = Logger::Instance();
}

Selettore::Selettore(const Selettore &copyin) // Copy constructor to handle pass by value.
{
    stmtSelect = copyin.stmtSelect;
    stmtFrom = copyin.stmtFrom;
    stmtWhere = copyin.stmtWhere;
    stmtSQL = copyin.stmtSQL;
    MASTERTABLE = copyin.MASTERTABLE;
}

void Selettore::SetStmtFrom(string sFrom) {
    
    stmtFrom += sFrom;
}

int Selettore::SetSelectWhereCondition(string IdJob) {
    int cnt = 0;
    int Optional = -1;
    int SelettoreComplex = -1;
    string stmtSql = "";
    string strFieldType = "";
    string strOperator = "";
    string strDataOrTableOrOther = "";

    stmtSql = "select WP1_CONDITIONS.FIELD, WP1_CONDITIONS.OPERATOR, WP1_CONDITIONS.OPERANDS,  WP1_CONDITIONS.FIELD_TYPE, WP1_CONDITIONS.ID_CONDITION, WP1_JOB_CONDITION.OPTIONAL ";
    stmtSql += "from WP1_JOB INNER JOIN WP1_JOB_CONDITION ON (WP1_JOB.ID_JOB = WP1_JOB_CONDITION.ID_JOB) ";
    stmtSql += "INNER JOIN WP1_CONDITIONS ON (WP1_CONDITIONS.ID_CONDITION = WP1_JOB_CONDITION.ID_CONDITION) ";
    stmtSql += "where WP1_JOB.ID_JOB = '" + IdJob + "'";

    ResultSet *rs = NULL;
    Statement *stmt = NULL;
    rs = DB->executeSelectStmt(stmtSql, stmt);
    string tabelleDaAggiungere=" "+MASTERTABLE+" ";
    stmtWhere = " where ";
    while (rs->next() == true) {
        
        strFieldType = rs->getString(4);
        DB->setCondition(rs->getString(5));
        Log->Log(INF, true, "ELABORO IL SELETTORE %s", rs->getString(5).c_str());
        Optional = rs->getInt(6);
        std::transform(strFieldType.begin(), strFieldType.end(), strFieldType.begin(), ::toupper);

        strOperator = rs->getString(2);
        strDataOrTableOrOther = rs->getString(3);

        if (strOperator.compare("COMPLEX") == 0) {
            SelettoreComplex = 1;
            Log->Log(INF, true, "ELABORO IL SELETTORE COMPLEX...");
            DB->executePackageTableKey(rs->getString(1), rs->getString(5)); //RIEMPIO LA TABELLA TEMPORANEA...
            stmtFrom = "";
            stmtWhere = " FROM WP1_TABLE_KEY";
            break;
        } else {
            SelettoreComplex = 0;
            if (strDataOrTableOrOther.find("$", 0) != std::string::npos) {
                // Eventuale sostituzione dalla temporary table ....
                //strDataOrTableOrOther = Cfg->getTempWp1Job(strDataOrTableOrOther);
                strDataOrTableOrOther = Cfg->convertOperand(strDataOrTableOrOther);
                if (strDataOrTableOrOther.compare("nul") == 0) {
                    if (Optional == 1) {
                        Log->Log(DBG, false, "Paramentro non impostato per il campo [%s] e condition [%s] ... SKIP !!", rs->getString(4).c_str(), rs->getString(5).c_str());
                        continue;
                    } else {

                        throw runtime_error("Errore nel selettore. Parametro " + rs->getString(3) + " è null");
                    }
                }
            }
            if (strOperator.compare("RAWSTATEMENT") == 0) {
                strOperator = "";
            } else if (strDataOrTableOrOther.compare("NULL") != 0) {
                if (strFieldType.compare("TABLE") == 0) {
                    string Buffer = strDataOrTableOrOther;

                    std::size_t found = strDataOrTableOrOther.find_first_of(".");

                    if (found != std::string::npos && found!=0)
                    {    
                        Buffer = strDataOrTableOrOther.substr(0, found);
                        if(tabelleDaAggiungere.find(" "+Buffer+" ")==std::string::npos)
                        {
                            SetStmtFrom(" , " + Buffer); //PER LA JOIN
                            tabelleDaAggiungere=tabelleDaAggiungere+" "+Buffer+" ";
                        }   	
                    }
                    else
                    {
                        throw new runtime_error("ERRORE! Un selettore di tipo table deve essere nel formato TABELLA.CHIAVE ");
                    }

                } else if (strFieldType.compare("DATE") == 0) {

                    strDataOrTableOrOther = "TO_DATE('" + strDataOrTableOrOther + "', 'DD/MM/YYYY')";

                } else if (strFieldType.compare("STRING") == 0) {

                    strDataOrTableOrOther = "'" + strDataOrTableOrOther + "'";

                } else {

                    strDataOrTableOrOther = strDataOrTableOrOther;
                }
            }

            if (cnt > 0) {
                stmtWhere += " and ";
            }
            if (Utl->ucase(strDataOrTableOrOther).compare("NULL") == 0 && strOperator.compare("=") == 0) {
                strOperator = "IS";
            }
            if (Utl->ucase(strDataOrTableOrOther).compare("NULL") == 0 && strOperator.compare("!=") == 0) {
                strOperator = "IS NOT";
            }

            stmtWhere += rs->getString(1) + " " + strOperator + " " + strDataOrTableOrOther;



        }
        cnt++;

    }

    DB->closeRecordset(stmt, rs);

    return SelettoreComplex;
}

int Selettore::SetKeyForMasterTable(string mst, string stmtSQL) {
    string stmtSql = "";
    int cntVerifica = 0;


    MASTERTABLE = mst;

    if (stmtSQL.compare("") == 0) {
        //std::transform(mst.begin(), mst.end(),mst.begin(), ::toupper);     
        stmtSql = "SELECT column_name FROM all_cons_columns WHERE constraint_name = (";
        stmtSql += "SELECT constraint_name FROM user_constraints ";
        stmtSql += "WHERE UPPER(TABLE_NAME) = UPPER('" + mst + "') AND CONSTRAINT_TYPE = 'P')";

        ResultSet *rs = NULL;
        Statement *stmt = NULL;
        rs = DB->executeSelectStmt(stmtSql, stmt);

        while (rs->next() == true) {
            if (cntVerifica == 0) {
                TabKeyForMasterTable.push_back(rs->getString(1));
                stmtFrom = "SELECT " + mst + "." + rs->getString(1) + " FROM " + mst;
            } else
                break;
            cntVerifica++;
        }
        //cout << "SetKeyForMasterTable: " << rs->getString(1) << endl;
        DB->closeRecordset(stmt, rs);
    } else {
        stmtFrom = stmtSQL;
        cntVerifica = 1;
    }
    //cout << "SetKeyForMasterTable: [" << stmtFrom << "]" << endl;
    //Log->Log(INF, "Selettore SetKeyForMasterTable: [%s]", stmtFrom.c_str());

    return cntVerifica;
}

void Selettore::MakeSQL(string stmSqlUpdate) {

    stmtSQL = stmSqlUpdate + " IN (" + stmtFrom + stmtWhere + ")";

    Log->Log(DBG, false, "MakeSQL -> stmtFrom:  [%s]", stmtFrom.c_str());
    Log->Log(DBG, false, "MakeSQL -> stmtWhere: [%s]", stmtWhere.c_str());
    Log->Log(DBG, false, "MakeSQL -> stmtSQL:   [%s]", stmtSQL.c_str());
}

int Selettore::ExecuteSQL() {

    Log->Log(INF, true, "*** ESEGUO L'OPERAZIONE: \n\t %s", stmtSQL.c_str());
    int status = DB->executeInsUpdDelStmt(stmtSQL);
    return status;
}

ResultSet * Selettore::GetOperationForJob(string IdJob, Statement * stmt) {
    string stmtSql;

    stmtSql += "select WP1_OPERATIONS.ID_OPERATION, WP1_OPERATIONS.FIELD, WP1_OPERATIONS.OPERATOR, WP1_OPERATIONS.OPERAND ";
    stmtSql += "from WP1_JOB INNER JOIN WP1_JOB_OPERATION ON (WP1_JOB.ID_JOB = WP1_JOB_OPERATION.ID_JOB) ";
    stmtSql += "INNER JOIN WP1_OPERATIONS  ON (WP1_OPERATIONS.ID_OPERATION = WP1_JOB_OPERATION.ID_OPERATION) ";
    stmtSql += "where  WP1_JOB.ID_JOB = '" + IdJob + "'";

    ResultSet *rs = NULL;
    rs = DB->executeSelectStmt(stmtSql, stmt);
    Log->Log(DBG, false, "Selettore::GetOperationForJob -> [%s]", stmtSql.c_str());

    return rs;
}

void Selettore::PrintSQL() {
    cout << "___________________________________________________________________" << endl;
    cout << stmtSQL << endl;
    cout << "___________________________________________________________________" << endl;
}

void Selettore::InsertIntoTableKey(vector<KeyType>& tabKey) {
    if (!keyInsertedIntoTemporaryTable) {
        string chiavi = "";

        for (unsigned i = 0; i < tabKey.size(); i++) {
            if (i == 0) {
                if ((tabKey[i].type).compare("NUMBER") == 0) {
                    chiavi = "ID_KEY_NUMBER";
                } else {
                    chiavi = "ID_KEY_VARCHAR";
                }
            } else {
                chiavi += ",";
                if ((tabKey[i].type).compare("NUMBER") == 0) {
                    chiavi += "ID_KEY2_NUMBER";
                } else {
                    chiavi += "ID_KEY2_VARCHAR";
                }
            }

        }
        string myStatment = "insert into WP1_TABLE_KEY (" + chiavi + ")  " + stmtFrom + stmtWhere;
        Log->Log(DBG, true, "*** ESEGUO L'OPERAZIONE: \n\t %s", myStatment.c_str());
        DB->executeInsUpdDelStmt(myStatment);
    }
    keyInsertedIntoTemporaryTable = true;
}
