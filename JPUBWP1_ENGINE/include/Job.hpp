/*
 * Job.hpp
 *
 *  Created on: 31/ott/2014
 *      Author: Marco
 */

#ifndef JOB_H_
#define JOB_H_

#ifndef OCCI_ORACLE
# include <occi.h>
#endif
#include <Config.hpp>
#include <DBManager.hpp>
#include <iostream>
#include <vector>
#include <string>
#include <Logger.hpp>
using namespace std;

class ObjJob
{ 
   public:        	 
      ObjJob();
      ObjJob(const ObjJob &);
      ~ObjJob(){};
	  string GetField(int Pos);
	  void SetField(ResultSet *rs);
	  void Print();	
	  string GetMST();
	  string GetIdJob();
          int getCounter();
	  void GetAndSetParametersTo_WP1_JOB(DB_Manager *db, Config *Cfg);
	  bool SetMasterTable(Config *Cfg);
	  string GetSelectForMasterTable();
          string GetStatementFromTableKey();
	  string GetTableWhithKey();
          bool isLogKey();
	  vector<KeyType> tabKeyType;

private:
    string ID_TICKET;
    string ID_JOB;
    string CODE_TIPO_CONTR;
    string MASTERTABLE;
    int COUNTER;
    bool logkey;
};

class Job
{   
   public:
      Job();
      Job(const Job &);
      ~Job(){};	  
	  void Print();	
      int Load(Config *Cfg, string id_ticket,string ticket_type,string code_tipo_contr);
      int getCount();
      void putJob(ObjJob *); 
      ObjJob getJob();		  
   private:
	  vector<ObjJob> tab_job;
      DB_Manager *DB;   
};

#endif /* JOB_H_ */
