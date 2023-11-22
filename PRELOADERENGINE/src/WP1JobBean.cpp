/*
 * WP1JobBean.cpp
 *
 *  Created on: Oct 28, 2014
 *      Author: amonteforte
 */

#include "WP1JobBean.h"

WP1JobBean::WP1JobBean() {}

WP1JobBean::WP1JobBean(string idJob, string descrizione, string jobType, int active, string codeTipoContr, string masterTable) : WP1Bean() {
	this->idJob = idJob;
	this->descrizione = descrizione;
	this->jobType = jobType;
	this->active = active;
	this->codeTipoContr  = codeTipoContr;
	this->masterTable = masterTable;
	this->jobRow = "";
}

WP1JobBean::~WP1JobBean() {}

string WP1JobBean::prepareSqlInsert() {
	char buffer[10];
	sprintf(buffer, "%d", active);
	string insertFix = "Insert into WP1_JOB (ID_JOB,DESCRIZIONE,ACTIVE,JOB_TYPE,CODE_TIPO_CONTR,MASTERTABLE) values ";
	string insertVar = "(\'" + idJob + "\'," +
			            "\'" + descrizione + "\'," +
			                   string(buffer) +
			            ",\'" + jobType  + "\'," +
			            "\'" + codeTipoContr + "\'," +
			            "\'" + masterTable  + "\')";;
	this->sqlInsert = insertFix+insertVar;
	return sqlInsert;
}

string WP1JobBean::prepareSqlUpdate() {
	char buffer[10];
	sprintf(buffer, "%d", active);
	string updateFix = "UPDATE WP1_JOB SET ";
	string updateVar = "DESCRIZIONE=\'" + descrizione +
			           "\',ACTIVE="+string(buffer) +
			           ",JOB_TYPE=" + "\'" + jobType  +
			           "\',CODE_TIPO_CONTR=" + "\'" + codeTipoContr  +
			           "\',MASTERTABLE=" + "\'" + masterTable  + "\'";
        string where = " WHERE ID_JOB=\'" + idJob + "\'";
	this->sqlUpdate = updateFix+updateVar+where;
	return sqlUpdate;
}

string WP1JobBean::prepareSqlDelete() {
	this->sqlDelete = "DELETE FROM WP1_JOB WHERE ID_JOB=\'"+idJob+"\'";
	return this->sqlDelete;
}

void WP1JobBean::printJobRow() {
	if (this->codeTipoContr.compare("") == 0)
		this->codeTipoContr = "ALL";
	char buffer[10];
	sprintf(buffer, "%d", this->active);
	this->jobRow = "" +
	this->idJob + "#" +
	this->descrizione + "#" +
	string(buffer) + "#" +
	this->jobType + "#" +
	this->codeTipoContr + "#" +
	this->masterTable;
	cout << this->jobRow << endl;

}
void WP1JobBean::logicCheck(int line) {
	//ACTIVE
	if ( (this->active != 0) &&
		 (this->active != 1) ) {
		printLogicErrorAndExitProgram("ACTIVE", line);
	}
}
