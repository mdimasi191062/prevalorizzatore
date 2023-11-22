/*
 * WP1JobConditionOperation.cpp
 *
 *  Created on: Oct 29, 2014
 *      Author: amonteforte
 */

#include "WP1JobConditionOperation.h"

WP1JobConditionOperation::WP1JobConditionOperation() {}

WP1JobConditionOperation::WP1JobConditionOperation(string jobType,string idJob) {
	this->jobType = jobType;
	this->idJob = idJob;
	this->sqlDeleteJobCondition = "";
	this->sqlDeleteJobOperation = "";
	this->jobRow = "";
	this->line = 0;
}

WP1JobConditionOperation::~WP1JobConditionOperation() {}

void WP1JobConditionOperation::clear() {
	this->jobType = "";
	this->idJob = "";
	this->idConditionList.clear();
	this->idOperationList.clear();
	this->sqlInsertList.clear();
	this->sqlUpdateList.clear();
	this->sqlDeleteJobCondition = "";
	this->sqlDeleteJobOperation = "";
	this->jobRow = "";
	this->line = 0;
}

void WP1JobConditionOperation::calculateSqlInsert() { //MOD.02 20-11-2014
	//JOB_CONDITION
	list<string>::const_iterator it = idConditionList.begin();
	list<int>::const_iterator itOpt = optionalList.begin();
	string insertFix = "Insert into WP1_JOB_CONDITION (ID_JOB,ID_CONDITION, OPTIONAL) values ";
	string insertVar = "";
	string sqlInsert = "";
	for (it = idConditionList.begin(); it != idConditionList.end() || itOpt != optionalList.end(); it++) {
		insertVar = "";
		sqlInsert = "";
		char buffer[10];
		sprintf(buffer, "%d", *itOpt);
		insertVar = "(\'" + idJob + "\'," +
				    "\'" + *it + "\'," +
					       string(buffer) + ")";
		sqlInsert = insertFix+insertVar;
		this->sqlInsertList.push_back(sqlInsert);
		itOpt++;
	}
	//JOB_OPERATION
	it = idOperationList.begin();
	insertFix = "Insert into WP1_JOB_OPERATION (ID_JOB,ID_OPERATION) values ";
	insertVar = "";
	sqlInsert = "";
	for (it = idOperationList.begin(); it != idOperationList.end(); it++) {
		insertVar = "";
		sqlInsert = "";
		insertVar = "(\'" + idJob + "\'," +
				    "\'" + *it + "\')";
		sqlInsert = insertFix+insertVar;
		this->sqlInsertList.push_back(sqlInsert);
	}
}

void WP1JobConditionOperation::calculateSqlDelete() {
	//JOB_CONDITION
	this->sqlDeleteJobCondition = "DELETE FROM WP1_JOB_CONDITION WHERE ID_JOB=\'"+this->idJob+"\'";
	//JOB_OPERATION
	this->sqlDeleteJobOperation = "DELETE FROM WP1_JOB_OPERATION WHERE ID_JOB=\'"+this->idJob+"\'"; 
}

void WP1JobConditionOperation::calculateSqlUpdate() {
	//JOB_CONDITION
	list<string>::const_iterator it = idConditionList.begin();
	list<int>::const_iterator itOpt = optionalList.begin();
	string updateFix = "UPDATE WP1_JOB_CONDITION SET ";
	string updateVar = "";
	string sqlUpdate = "";
	for (it = idConditionList.begin(); it != idConditionList.end() || itOpt != optionalList.end(); it++) {
		updateVar = "";
		sqlUpdate = "";
		char buffer[10];
		sprintf(buffer, "%d", *itOpt);
		updateVar = "ID_JOB=\'" + idJob +
				     "\',ID_CONDITION=" + "\'" + *it  +
					 "\',OPTIONAL=" + string(buffer);
		sqlUpdate = updateFix+updateVar;
		this->sqlUpdateList.push_back(sqlUpdate);
		itOpt++;
	}
	//JOB_OPERATION
	it = idOperationList.begin();
	updateFix = "UPDATE WP1_JOB_OPERATION SET ";
	updateVar = "";
	sqlUpdate = "";
	for (it = idOperationList.begin(); it != idOperationList.end(); it++) {
		sqlUpdate = "";
		updateVar = "ID_JOB=\'" + idJob +
				     "\',ID_OPERATION=" + "\'" + *it  + "\'";
		sqlUpdate = updateFix+updateVar;
		this->sqlUpdateList.push_back(sqlUpdate);
	}
}

void WP1JobConditionOperation::printJobConfigRow() {
        this->jobRow = "JOB_ROW#"+
        this->jobType + "#" +
        this->idJob;
        //Print JOB_ROW
        cout <<  this->jobRow << endl;
        //Print CONDITION_ROW
        list<string>::iterator it;
		list<int>::iterator itOpt = optionalList.begin();
        for (it = idConditionList.begin(); it != idConditionList.end() || itOpt != optionalList.end(); it++) {
				char buffer[10];
				sprintf(buffer, "%d", *itOpt);
                cout << "CONDITION_ROW#" + (*it) + "#" + string(buffer) << endl;
				itOpt++;
        }
        //Print OPERATION_ROW
        list<string>::iterator it1;
        for (it1 = idOperationList.begin(); it1 != idOperationList.end(); it1++) {
                cout << "OPERATION_ROW#" + (*it1) << endl;
        }
}

void WP1JobConditionOperation::printLogicErrorAndExitProgram(string field, int line) {
	cout << "Bad field " << field << " at line " << line << ". Please check and modify input file before run the program again!!!" << endl;
	exit(1);
}

void WP1JobConditionOperation::logicCheck(int line) {
	//OPTIONAL
	line++;
	list<string>::iterator it;
	list<int>::iterator itOpt = optionalList.begin();
	for (it = idConditionList.begin(); it != idConditionList.end() || itOpt != optionalList.end(); it++) {
		if ( ((*itOpt) != 0 ) &&
			 ((*itOpt) != 1 ) ) {
				printLogicErrorAndExitProgram("OPTIONAL", line);
		}
		itOpt++;
		line++;
	}
}
