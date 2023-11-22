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

DB_Manager::DB_Manager()
{
  env = NULL;
  conn = NULL;
  stmt = NULL;
}

DB_Manager::~DB_Manager()
{

}

DB_Manager* DB_Manager::Instance()
{
	if(!m_pInstance) {
		m_pInstance = new DB_Manager();
	}
	return m_pInstance;
}

bool DB_Manager::connectDB() {

	try
	{
		env = Environment::createEnvironment(Environment::DEFAULT);		
		conn = env->createConnection(oUser, oPassaword, oConnect);
		if (conn) 
			return true;
		
		return false;
	}
	catch (SQLException& e)
	{
		std::cout<<e.what();
		cerr << e.getErrorCode() << e.getMessage() << endl;
		return false;
	}
}

bool DB_Manager::DisconnectDB() {

    try{
		conn->terminateStatement (stmt);
		env->terminateConnection(conn);
		Environment::terminateEnvironment(env);
		cout << "DB->DisconnectDB()" << endl;
	} catch(SQLException ex) {
	    int errno = ex.getErrorCode();
        cout << ex.what() << endl;
		cout << "Error: " << errno << " -> " << ex.getMessage() << endl;
    }
    return true;
}

ResultSet * DB_Manager::executePackage(string stmSql, string dataIn) {
    //stmSql: BEGIN selettore_proc_1(:1, :2); END;
	ResultSet *rs = NULL;
	
	try{
	   cout << "executePackage: [" << stmSql << "] - parametro: [" << dataIn << "]" << endl; 
       stmt = conn->createStatement(stmSql);
	
       stmt->setAutoCommit(false);
       
	   stmt->setString(1, dataIn);		   
	   // need to register the ref cursor output parameter
       stmt->registerOutParam(2, OCCICURSOR);
  
       rs = stmt->executeQuery();
	
       // get the ref cursor as an occi resultset
       rs = stmt->getCursor(2);
  
    } catch(SQLException ex) {
	    int errno = ex.getErrorCode();
        cout << ex.what() << endl;
		cout << "Error: " << errno << " -> " << ex.getMessage() << endl;
    }
    return rs;
}

string DB_Manager::executePackageConfig(string stmSql) {
    //stmSql: BEGIN selettore_proc_1(); END;
	string result;
	
	try{
	   cout << "executePackageConfig: [" << stmSql <<  endl; 
       stmt = conn->createStatement(stmSql);
	
	   // set parameters - return value is first
       stmt->registerOutParam(1, OCCISTRING, 40);
       stmt->setAutoCommit(false);
  
	   stmt->executeUpdate();	
       result = stmt->getString(1);
  
    } catch(SQLException ex) {
	    int errno = ex.getErrorCode();
        cout << ex.what() << endl;
		cout << "Error: " << errno << " -> " << ex.getMessage() << endl;
    }
    return result;
}

ResultSet * DB_Manager::executeSelectStmt(string stmSql) {
    ResultSet *rs = NULL;	
	stmt = conn->createStatement(stmSql);
	rs = stmt->executeQuery();
	rs->status();
    return rs;
}

string DB_Manager::getStringFromSelect(string stmSql) {		
    ResultSet *rs = NULL;	
	string result="";
	
	try{
		stmt = conn->createStatement(stmSql);
		cout << "getStringFromSelect: " << stmSql << endl;	
	
		rs = stmt->executeQuery();	
		ResultSet::Status status = rs->status();
		cout << "getStringFromSelect: status->" << status << endl;
		
		if (status == 1) {
		   if (rs->next() == true) {
	          result = rs->getString(1);
			  cout << "getStringFromSelect: [" << result << "]" << endl;
           }
           closeRecordset(rs);
		}
		
	} catch(SQLException ex) {
	    int errno = ex.getErrorCode();
        cout << ex.what() << endl;
		cout << "getStringFromSelect Error: " << errno << " -> " << ex.getMessage() << endl;
    }	
    return result;
}

int DB_Manager::executeInsUpdDelStmt(string stmSql) {
	int status = -1;
	
	//try{ //MAN DEBUG
	stmt = conn->createStatement(stmSql);
	stmt->setAutoCommit(false);	
	//cout << "executeInsUpdDelStmt: " << stmSql << endl;	
	stmt->executeUpdate (stmSql);
	status = 0;
	
	//MAN DEBUG BEG
	//} catch(SQLException ex) {
    //    cout << ex.what() << endl;
	//	  cout << ex.getMessage() << endl;
    //}
	//MAN DEBUG END
    return status;
}

void DB_Manager::closeRecordset(ResultSet *rs) {
// try{ //MAN DEBUG
	stmt->closeResultSet(rs);		
//MAN DEBUG BEG
// 	} catch(SQLException ex) {
//        cout << ex.what() << endl;
//		cout << ex.getMessage() << endl;
//    }
//MAN DEBUG END
}

string DB_Manager::checkService(string idService) {
   string result="";
   
   ResultSet *rs = executeSelectStmt("Select ID_MACRO_SERVICE from WP1_SERVICES WHERE ID_SERVICE=" + idService);
   if (rs->next() == true) {
	   result = rs->getString(1);
   }
   closeRecordset(rs);
   
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

void DB_Manager::getDateTime(char *dt) {
    char buffer[50];
    time_t t = time(NULL);
    struct tm tm = *localtime(&t);

    memset(buffer, '\0', sizeof(buffer));
    sprintf(buffer, "%d%d%d%d%d%d",  tm.tm_mday, tm.tm_mon + 1, tm.tm_year + 1900, tm.tm_hour, tm.tm_min, tm.tm_sec);

    strcpy(dt, buffer);
}

