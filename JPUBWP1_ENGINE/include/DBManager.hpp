/*
 * DBManager.hpp
 *
 *  Created on: 03/ott/2014
 *      Author: Marco
 */

#ifndef DBMANAGER_H_
#define DBMANAGER_H_

#ifndef OCCI_ORACLE
#include <occi.h>
#endif


using namespace std;
using namespace oracle::occi;
using oracle::occi::Environment;
using oracle::occi::Connection;

class DB_Manager {
public:
    DB_Manager();
    ~DB_Manager();
    static DB_Manager* Instance();

    bool connectDB();
    void DisconnectDB();
    string checkService(string);
    void setCredenziali(string, string, string);

    void setTS(long TimeStamp);
    void setJob(string IdJob,int idContatore);
    void setTicket(string idTicket);
    void setCondition(string idCondition);
    void setOperation(string idOperation);

    ResultSet * executeSelectStmt(string, Statement *);
    int executeInsUpdDelStmt(string);
    ResultSet * executePackage(string, string);
    string executePackageConfig(string);
    int executePackageLOG(string,string);
    int executePackageLOG_KEY(); 
    int executePackageJOB_LOG(string, string);
    string getStringFromSelect(string stmSql);
    string getTypeFromColumn(string tabcol);
    int executePackageTableKey(string table,  string cond);

    void Commit();
    void RollBack();
    void closeRecordset(Statement *, ResultSet *);
    Connection * GetConnection();
    int insertLog(string message, long tempo);
    string getIdTicket();
    string getIdJob();
private:
    static DB_Manager* m_pInstance;

    DB_Manager(DB_Manager const&);
    DB_Manager& operator=(DB_Manager const &);


    void LogDbError(SQLException ex);
    long GetContatoreDet();

    Environment *env;
    Connection *conn;

    string oUser;
    string oPassaword;
    string oConnect;

    long TS;
    long ContatoreDet;

    string Id_Ticket;
    string Id_Job;
    string Id_Condition;
    string Id_Operation;

};

#endif /* DBMANAGER_H_ */
