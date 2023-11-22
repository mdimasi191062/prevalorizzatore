/*
 * WP1OperationBean.cpp
 *
 *  Created on: Oct 28, 2014
 *      Author: amonteforte
 */

#include "WP1OperationBean.h"

WP1OperationBean::WP1OperationBean() {}

WP1OperationBean::WP1OperationBean(string idOperation, string field, string operatore, string operand, string opType) : WP1Bean() {
	this->idOperation = idOperation;
	this->field = field;
	this->operatore = operatore;
	this->operand = operand;
	this->opType = opType;
	this->operRow = "";
}

WP1OperationBean::~WP1OperationBean() {}

string WP1OperationBean::prepareSqlInsert() {
	string insertFix = "Insert into WP1_OPERATIONS (ID_OPERATION,FIELD,OPERATOR,OPERAND,OP_TYPE) values ";
	string insertVar = "(\'" + idOperation + "\'," +
			            "\'" + field + "\'," +
			            "\'" + operatore + "\',"+
			            "\'" + operand  + "\'," +
			            "\'" + opType  + "\')";;
	this->sqlInsert = insertFix+insertVar;
	return sqlInsert;
}

string WP1OperationBean::prepareSqlUpdate() {
	string updateFix = "UPDATE WP1_OPERATIONS SET ";
	string updateVar = "FIELD=\'" + field +
			           "\',OPERATOR="+"\'"+operatore +
			           "\',OPERAND=" + "\'" + operand  +
			           "\',OP_TYPE=" + "\'" + opType  + "\'";
	string where = " WHERE ID_OPERATION=\'" + idOperation + "\'";
	this->sqlUpdate = updateFix+updateVar+where;
	return sqlUpdate;
}

void WP1OperationBean::prepareSqlDelete() {
	this->sqlDelete = "DELETE FROM WP1_OPERATIONS WHERE ID_OPERATION=\'"+idOperation+"\'";
}

void WP1OperationBean::printWP1OperationRow() {
	this->operRow = ""+
	this->idOperation + "#" +
	this->field + "#" +
	this->operatore + "#" +
	this->operand + "#" +
	this->opType;
	cout << this->operRow << endl;
}

