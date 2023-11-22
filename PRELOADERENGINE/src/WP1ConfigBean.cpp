/*
 * WP1ConfigBean.cpp
 *
 *  Created on: Oct 27, 2014
 *      Author: amonteforte
 */

#include "WP1ConfigBean.h"



WP1ConfigBean::WP1ConfigBean() {}

WP1ConfigBean::WP1ConfigBean(string componentName, string paramName, string paramValue, string descrizione, int active, int isMacro) : WP1Bean() {
		this->componentName = componentName;
		this->paramName = paramName;
		this->paramValue = paramValue;
		this->descrizione = descrizione;
		this->active = active;
		this->isMacro = isMacro;
		this->configRow = configRow;
		this->sqlDelete = "";
}

WP1ConfigBean::~WP1ConfigBean() { }

string WP1ConfigBean::prepareSqlInsert() {
	char buffer[10];
	sprintf(buffer, "%d", active);
	char buffer1[10];
	sprintf(buffer1, "%d", isMacro);
	string insertFix = "Insert into WP1_CONFIG (COMPONENT_NAME,PARAM_NAME,PARAM_VALUE,ACTIVE,DESCR,IS_MACRO) values ";
	string insertVar = "(\'" + componentName + "\'," +
                           "\'" + paramName + "\'," +
                           "\'" + paramValue + "\',"+
                                  string(buffer) + "," +
                           "\'" + descrizione  + "\'," + 
                                  string(buffer1) + ")";
	this->sqlInsert = insertFix+insertVar;
	return sqlInsert;
}

string WP1ConfigBean::prepareSqlUpdate() {
	char buffer[10];
	sprintf(buffer, "%d", active);
	char buffer1[10];
	sprintf(buffer1, "%d", isMacro);
	string updateFix = "UPDATE WP1_CONFIG SET ";
	string updateVar = "PARAM_VALUE=\'"+paramValue +
			           "\',ACTIVE=" + string(buffer) +
			           ",DESCR=\'" + descrizione  +
			           "\',IS_MACRO=" + string(buffer1);
        string where = " WHERE COMPONENT_NAME=\'" + componentName + "\'" +
        	       " AND " +
                       "PARAM_NAME=\'" + paramName + "\'";
	this->sqlUpdate = updateFix+updateVar+where;
	return sqlUpdate;
}

void WP1ConfigBean::prepareSqlDelete() {
	this->sqlDelete = "DELETE FROM WP1_CONFIG WHERE COMPONENT_NAME=\'"+componentName+"\' AND PARAM_NAME=\'"+paramName+"\'";
}

void WP1ConfigBean::printWP1ConfigRow() {
	char buffer[10];
	sprintf(buffer, "%d", this->active);
	char buffer1[10];
	sprintf(buffer1, "%d", this->isMacro);
	this->configRow = ""+
	this->componentName + "#" +
	this->paramName + "#" +
	this->paramValue + "#" +
	this->descrizione + "#" +
	string(buffer) + "#" +
	string(buffer1);
	cout << this->configRow << endl;
}

void WP1ConfigBean::logicCheck(int line) {
	//COMPONENT_NAME
	if ( this->componentName.compare("") == 0 ) {
		printLogicErrorAndExitProgram("COMPONENT_NAME", line);
	}
	//ACTIVE
	if ( (this->active != 0) &&
		 (this->active != 1) ) {
		printLogicErrorAndExitProgram("ACTIVE", line);
	}
	//IS_MACRO
	if ( (this->isMacro != 0) &&
		 (this->isMacro != 1) ) {
		printLogicErrorAndExitProgram("IS_MACRO", line);
	}
}

