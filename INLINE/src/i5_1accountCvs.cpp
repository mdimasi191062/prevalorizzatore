//=============================================================================
// Name        : i5_1accountCvs.cpp
// Author      : Marco Onotri
// Version     :
// Copyright   : 
// Description : Estrazione dati dalla tabella I5_1ACCOUNT e scrittura file CSV
//=============================================================================

#ifndef OCCI_ORACLE
# include <occi.h>
#endif

#include "DBManager.hpp"
#include <sys/time.h>
#include <sys/timeb.h>
#include <stdlib.h>
#include <iostream>

using namespace std;
using namespace oracle::occi;
DB_Manager *DB;

void getTime(char tL, char *bufferTime)
{
	struct timeb buf;
	struct tm *timeinfo;
	time_t t;

	ftime(&buf);
	time(&t);
	timeinfo = localtime(&t);

	if (tL == 'T')
		sprintf(bufferTime,"%24s",ctime(&t));
	else if (tL == 'L')
		strftime(bufferTime,20,"%d%m%Y%H%M%S",timeinfo);
	else if (tL == 'P')
		strftime(bufferTime,12,"_%d%m%Y",timeinfo);
	else if (tL == 'X')
		strftime(bufferTime,10,"_%m%Y",timeinfo);
	else if (tL == 'M')
	   sprintf(bufferTime,"%ld.%d",buf.time,buf.millitm);
        else if (tL == 'p')
		strftime(bufferTime,12,"%Y%m%d",timeinfo);
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
    string strUser;
    string strPassword;
    string strConnect;
    string stmtSql;
    string nomeFileCsv;    
    char tempo[30] = "";    
    ResultSet *rs = NULL;
    Statement *stmt = NULL;
    FILE *Fp_csv;

    status = checkEnvironment(strUser, strPassword, strConnect);
    if (status == false) {
        return 1;
    }

    DB = DB_Manager::Instance();
    DB->setCredenziali(strUser, strPassword, strConnect);
    status = DB->connectDB();

    cout << "Stato Connessione Database: " << status << endl;

    if (status == 1) {
        memset(tempo, '\0', sizeof (tempo));
        getTime('p', tempo);
        nomeFileCsv = "I5_1ACCOUNT_" + (string)tempo + ".csv";
        
	stmtSql = "select * from I5_1ACCOUNT order by CODE_ACCOUNT";

        rs = DB->executeSelectStmt(stmtSql, stmt);
        Fp_csv = fopen(nomeFileCsv.c_str(), "w");
        fprintf(Fp_csv, "CODE_ACCOUNT; CODE_CICLO_FATRZ; CODE_SCONTO; CODE_GEST; DESC_ACCOUNT; DATA_ULTIMA_FATRZ; DATA_INIZIO_VALID; DATA_CREAZ; CODE_UTENTE_MODIF; DATA_MODIF; FLAG_SYS\n"); //intestazione

        while (rs->next() == true) {

           fprintf(Fp_csv, "%s; %s; %s; %s; %s; %s; %s; %s; %s; %s; %s; %s\n", 
           rs->getString(1).c_str(), rs->getString(2).c_str(), rs->getString(3).c_str(), rs->getString(4).c_str(), 
           rs->getString(5).c_str(), rs->getString(6).c_str(), rs->getString(7).c_str(), rs->getString(8).c_str(),
           rs->getString(9).c_str(), rs->getString(10).c_str(), rs->getString(11).c_str(), rs->getString(12).c_str());

        }
        DB->closeRecordset(stmt, rs);
        fflush(Fp_csv);
        fclose(Fp_csv);
    }
}


