/*
 * DBManager.hpp
 *
 *  Created on: 03/ott/2014
 *      Author: Marco
 */

#ifndef DBMANAGER_H_
#define DBMANAGER_H_

#ifndef OCCI_ORACLE
# include <occi.h>
#endif

using namespace std;
using namespace oracle::occi;
using oracle::occi::Environment;
using oracle::occi::Connection;


class DB_Manager
{
public:
    DB_Manager ();
   ~DB_Manager ();
	static DB_Manager* Instance();

    bool connectDB();
    bool DisconnectDB();
	string checkService(string);
	void setCredenziali(string, string, string);
	
    ResultSet * executeSelectStmt(string);
	int executeInsUpdDelStmt(string);
    ResultSet * executePackage(string, string);
	string executePackageConfig(string);
	string getStringFromSelect(string stmSql);
    void Commit();
    void RollBack();
	
    void closeRecordset(ResultSet *);
	Connection * GetConnection();
	
private:
    static DB_Manager* m_pInstance;
	
    DB_Manager(DB_Manager const&);
    DB_Manager& operator=(DB_Manager const &);
	
    Environment *env;
    Connection *conn;
    Statement *stmt;

    string oUser;
    string oPassaword;
    string oConnect;

    void getDateTime(char *);

};

#endif /* DBMANAGER_H_ */
