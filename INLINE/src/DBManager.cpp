/*
 * DBManager.cpp
 *
 *  Created on: 03/ott/2014
 *      Author: Marco
 */

#include <stdio.h>
#include <sys/time.h>
#include <string>
#include <string.h>
#include <occi.h>
#include <iostream>
#include <sstream>
#include <DBManager.hpp>


using namespace oracle::occi;
using namespace std;

DB_Manager* DB_Manager::m_pInstance = 0;

DB_Manager::DB_Manager() {
    env = NULL;
    conn = NULL;

    TS = 0;
    Id_Job = "";
    Id_Ticket = "";
    Id_Condition = "";
    Id_Operation = "";
    ContatoreDet = 0;
}

DB_Manager::~DB_Manager() {
}

DB_Manager* DB_Manager::Instance() {
    if (!m_pInstance) {
        m_pInstance = new DB_Manager();
    }
    return m_pInstance;
}

bool DB_Manager::connectDB() {

    try {
        env = Environment::createEnvironment(Environment::DEFAULT);
        conn = env->createConnection(oUser, oPassaword, oConnect);
        if (conn)
            return true;
        else
            return false;
    } catch (SQLException& ex) {
        //int errno = ex.getErrorCode();
        //Logger::Instance()->Log(ERR,false,"Errore nella connessione al DB %s \n\t Error:%d -> %s", ex.what(), errno, ex.getMessage().c_str());
        return false;
    }
}

void DB_Manager::DisconnectDB() {

    try {
        if(conn && conn)
        {
            env->terminateConnection(conn);
            Environment::terminateEnvironment(env);
        }
        //Logger::Instance()->Log(INF, false, "Disconnesso dal db");

    } catch (SQLException& ex) {
         cerr<< "Errore nella disconnessione del db" <<endl;
    } catch(...)
    { 
        cerr<< "Errore nella disconnessione del db" <<endl;
    }
}

int DB_Manager::insertLog(string message, long tempo) {
    string stmSql = "BEGIN :1 := PKG_WP1_UTILITY.write_log(:2, :3, :4, :5, :6); END;";

    int result = 0;
    Statement *stmt = NULL;

    stmt = conn->createStatement(stmSql);
    stmt->registerOutParam(1, OCCINUMBER);

    stmt->setNumber(2, tempo);
    stmt->setNumber(3, TS);
    stmt->setString(4, Id_Ticket);
    stmt->setString(5, Id_Job);
    stmt->setString(6, message);
    stmt->setAutoCommit(false);
    stmt->executeQuery();
    result = stmt->getNumber(1);
    conn->terminateStatement(stmt);
        
    return result;
}

int DB_Manager::executePackageLOG_KEY() {
    string stmSql = "BEGIN :1 := PKG_WP1_UTILITY.LOG_KEY(:2, :3, :4); END;";
    int result = 0;
    Statement *stmt = NULL;
    //Logger::Instance()->Log(DBG, false, "\tEsecuzione del comando \n\t\t  %s", stmSql.c_str());

    stmt = conn->createStatement(stmSql);
    stmt->registerOutParam(1, OCCINUMBER);

    stmt->setString(2, Id_Ticket);
    stmt->setString(3, Id_Job);
    stmt->setNumber(4, ContatoreDet);
    
    stmt->setAutoCommit(false);
    stmt->executeQuery();

    result = stmt->getNumber(1);
    conn->terminateStatement(stmt);


    return result;
}
int DB_Manager::executePackageJOB_LOG(string message, string esito) {
    string stmSql = "BEGIN :1 := PKG_WP1_UTILITY.write_log_elab_dett(:2, :3, :4, :5, :6, :7, :8, :9); END;";
    int result = 0;
    Statement *stmt = NULL;
    //Logger::Instance()->Log(DBG, false, "\tEsecuzione del comando \n\t\t  %s", stmSql.c_str());

    stmt = conn->createStatement(stmSql);
    stmt->registerOutParam(1, OCCINUMBER);

    stmt->setNumber(2, TS);
    stmt->setString(3, Id_Ticket);
    stmt->setString(4, Id_Job);
    stmt->setString(5, Id_Condition);
    stmt->setString(6, Id_Operation);
    stmt->setString(7, message);
    stmt->setString(8, esito);
    stmt->setNumber(9, ContatoreDet);
    
    stmt->setAutoCommit(false);
    stmt->executeQuery();

    result = stmt->getNumber(1);
    conn->terminateStatement(stmt);


    return result;
}

int DB_Manager::executePackageLOG(string message, string esito) {
    string stmSql = "BEGIN :1 := PKG_WP1_UTILITY.write_log_elab(:2, :3, :4, :5); END;";
    Statement *stmt;
    int result = 0;
    message = Id_Job + " -> " + message;

    //Logger::Instance()->Log(DBG, false, "\tEsecuzione del comando \n\t\t  %s", stmSql.c_str());

    stmt = conn->createStatement(stmSql);
    stmt->registerOutParam(1, OCCINUMBER);

    stmt->setNumber(2, TS);
    stmt->setString(3, Id_Ticket);
    stmt->setString(4, message);
    stmt->setString(5, esito);

    stmt->setAutoCommit(false);
    stmt->executeQuery();

    result = stmt->getNumber(1);
    conn->terminateStatement(stmt);

    return result;
}

string DB_Manager::getIdTicket() {
    return this->Id_Ticket;
}

string DB_Manager::getIdJob() {
    return this->Id_Job;
}

void DB_Manager::LogDbError(SQLException ex) {
    int result = 0;

    result = executePackageJOB_LOG(ex.getMessage(), "KO");
}

ResultSet * DB_Manager::executePackage(string stmSql, string dataIn) {
    //stmSql: BEGIN selettore_proc_1(:1, :2); END;
    ResultSet *rs = NULL;
    Statement *stmt = NULL;
    //Logger::Instance()->Log(DBG, false, "\tEsecuzione del comando \n\t\t  %s", stmSql.c_str());

    stmt = conn->createStatement(stmSql);

    stmt->setString(1, dataIn);
    // need to register the ref cursor output parameter
    stmt->registerOutParam(2, OCCICURSOR);
    stmt->setAutoCommit(false);

    rs = stmt->executeQuery();

    rs = stmt->getCursor(2);

    conn->terminateStatement(stmt);

    return rs;
}

string DB_Manager::executePackageConfig(string stmSql) {
    //stmSql: BEGIN selettore_proc_1(); END;
    string result;
    Statement *stmt = NULL;

    //Logger::Instance()->Log(DBG, false, "\tEsecuzione del comando \n\t\t  %s", stmSql.c_str());
    stmt = conn->createStatement(stmSql);

    // set parameters - return value is first
    stmt->registerOutParam(1, OCCISTRING, 40);
    stmt->setAutoCommit(false);

    stmt->executeUpdate();
    result = stmt->getString(1);
    conn->terminateStatement(stmt);

    return result;
}

int DB_Manager::executePackageTableKey(string table, string idSelOp) {
    // PKG_WP1_PREVAL.COND_OL_TO_VAL_OPERATORI_CES1('WS00002', 'JOB_OL_TO_VAL_OPERATORI_CES1', 'COND_OL_TO_VAL_OPERATORI_CES1'
    //"BEGIN PKG_WP1_PREVAL.COND_OL_TO_VAL_OPERATORI_CES1(:1, :2, :3); END;"; 
    Statement *stmt = NULL;
    int result = -1;
    string execPkg = "begin :1 := " + table + "(:2, :3 ,:4); end;";

    //Logger::Instance()->Log(INF, true, "*** ESEGUO L'OPERAZIONE: \n\t %s",execPkg.c_str());
    stmt = conn->createStatement(execPkg);

    // set parameters - return value is first
    stmt->registerOutParam(1, OCCINUMBER);
    stmt->setAutoCommit(false);

    stmt->setString(2, Id_Ticket);
    stmt->setString(3, Id_Job);
    stmt->setString(4, idSelOp);
	
    stmt->executeUpdate();

    result = stmt->getNumber(1);
    conn->terminateStatement(stmt);
    
    return result;
}

ResultSet * DB_Manager::executeSelectStmt(string stmSql, Statement *stmt) {
    ResultSet *rs;
    //Logger::Instance()->Log(DBG, false, "\tEsecuzione del comando \n\t\t  %s", stmSql.c_str());


    stmt = conn->createStatement(stmSql);
    stmt->setAutoCommit(false);
    rs = stmt->executeQuery();

    return rs;
}

string DB_Manager::getTypeFromColumn(string tabcol) {
    string sQL = "";
    string result = "";
    string tab;
    string col;
    size_t pos = 0;

    if ((pos = tabcol.find(".")) != std::string::npos) {
        tab = tabcol.substr(0, pos);
        col = tabcol.substr(pos + 1, tabcol.length() - pos);


        //SELECT C.TABLE_NAME, C.COLUMN_NAME, C.DATA_TYPE
        sQL = "SELECT UPPER(C.DATA_TYPE) FROM ALL_TAB_COLUMNS C WHERE C.TABLE_NAME = '";
        sQL += tab + "' AND C.OWNER = UPPER('" + oUser + "') AND C.COLUMN_NAME = '" + col + "' ORDER BY C.TABLE_NAME";

        Statement *stmt = NULL;
        ResultSet *rs = NULL;
        //Logger::Instance()->Log(DBG, false, "\tEsecuzione del comando \n\t\t  %s", sQL.c_str());

        rs = executeSelectStmt(sQL, stmt);

        if (rs) {
            if (rs->next() == true) {
                result = rs->getString(1);
            }
        }
        closeRecordset(stmt, rs);
    }


    return result;
}

string DB_Manager::getStringFromSelect(string stmSql) {
    ResultSet *rs = NULL;
    string result = "";
    Statement *stmt = NULL;
    //Logger::Instance()->Log(DBG, false, "\tEsecuzione del comando \n\t\t  %s", stmSql.c_str());
    stmt = conn->createStatement(stmSql);

    stmt->setAutoCommit(false);

    rs = stmt->executeQuery();
    ResultSet::Status status = rs->status();

    if (status == 1) {
        if (rs->next() == true) {
            result = rs->getString(1);
        }
    }
    closeRecordset(stmt, rs);

    return result;
}

int DB_Manager::executeInsUpdDelStmt(string stmSql) {
    int status = -1;
    Statement *stmt = NULL;
    //Logger::Instance()->Log(DBG, false, "\tEsecuzione del comando \n\t\t  %s", stmSql.c_str());
    
    stmt = conn->createStatement(stmSql);
    stmt->setAutoCommit(false);
    status = stmt->executeUpdate(stmSql);

    conn->terminateStatement(stmt);


    return status;
}

void DB_Manager::closeRecordset(Statement *stmt, ResultSet *rs) {

    if (stmt != NULL) {
        if (rs != NULL) {
            stmt->closeResultSet(rs);
        }
        conn->terminateStatement(stmt);
    }

}

string DB_Manager::checkService(string idService) {
    string result = "";

    ResultSet *rs = NULL;
    Statement *stmt = NULL;
    string stmSql = "Select ID_MACRO_SERVICE from WP1_SERVICES WHERE ID_SERVICE='" + idService + "'";
    //Logger::Instance()->Log(DBG, false, "\tEsecuzione del comando \n\t\t  %s", stmSql.c_str());

    rs = executeSelectStmt(stmSql, stmt);
    if (rs) {
        if (rs->next() == true) {
            result = rs->getString(1);
        }
    }
    closeRecordset(stmt, rs);
    return result;
}

Connection * DB_Manager::GetConnection() {
    return (conn);
}

void DB_Manager::Commit() {
    conn->commit();
}

void DB_Manager::RollBack() {
    conn->rollback();
}

void DB_Manager::setCredenziali(string strUser, string strPassword, string strConnect) {
    oUser = strUser;
    oPassaword = strPassword;
    oConnect = strConnect;
}



void DB_Manager::setTicket(string idTicket) {
    Id_Ticket = idTicket;
}

void DB_Manager::setTS(long ts) {
    TS = ts;
}

void DB_Manager::setJob(string IdJob,int contatore) {
    Id_Job = IdJob;
    ContatoreDet=contatore;
}

void DB_Manager::setCondition(string idCondition) {

    Id_Condition = idCondition;
}

void DB_Manager::setOperation(string idOperation) {

    Id_Operation = idOperation;
}
