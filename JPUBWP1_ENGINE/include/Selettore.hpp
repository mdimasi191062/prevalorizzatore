/*
 * Selettore.hpp
 *
 *  Created on: 31/ott/2014
 *      Author: Marco
 */

#ifndef SELETTORE_H_
#define SELETTORE_H_

#ifndef OCCI_ORACLE
# include <occi.h>
#endif
#include <DBManager.hpp>
#include <Config.hpp>
#include <Logger.hpp>
#include <Util.hpp>
#include <vector>
#include <string>

using namespace std;

class Selettore
{ 
   public:        	 
      Selettore();
      Selettore(const Selettore &);
      ~Selettore(){};
	  int SetKeyForMasterTable(string mst, string stmtSQL);
	  int SetSelectWhereCondition(string IdJob); //, string &nomePakage, string &strCond);
	  void SetStmtFrom(string sFrom);
	  void MakeSQL(string stmSqlUpdate);
	  int ExecuteSQL();
	  ResultSet * GetOperationForJob(string IdJob, Statement *stmt);
	  void PrintSQL();	
	  void InsertIntoTableKey(vector<KeyType>&);
   private:
	  string MASTERTABLE;
	  string stmtSelect;
	  string stmtFrom;
	  string stmtWhere;
	  string stmtSQL;
	  
	  Util *Utl;
	  Config *Cfg;
	  DB_Manager *DB;
	  Logger *Log;
	  bool keyInsertedIntoTemporaryTable;
	  vector<std::string> TabKeyForMasterTable;
};

#endif /* SELETTORE_H_ */
