/*
 * WP1ConditionBean.cpp
 *
 *  Created on: Oct 28, 2014
 *      Author: amonteforte
 */

#include "WP1ConditionBean.h"

WP1ConditionBean::WP1ConditionBean() {}

WP1ConditionBean::WP1ConditionBean(string idCondition, string field, string operatore, string operands, string fieldType, string condType) : WP1Bean() {
	this->idCondition = idCondition;
	this->field = field;
	this->operatore = operatore;
	this->operands = operands;
	this->fieldType = fieldType;
	this->condType = condType;
	this->condRow = "";
}

WP1ConditionBean::~WP1ConditionBean() {}

string WP1ConditionBean::prepareSqlInsert() {
	string insertFix = "Insert into WP1_CONDITIONS (ID_CONDITION,FIELD,OPERATOR,OPERANDS,FIELD_TYPE,COND_TYPE) values ";
	string insertVar = "(\'" + idCondition + "\'," +
			            "\'" + field + "\'," +
			            "\'" + operatore + "\',"+
			            "\'" + operands  + "\'," +
			            "\'" + fieldType + "\'," +
			            "\'" + condType  + "\')";;
	this->sqlInsert = insertFix+insertVar;
	return sqlInsert;
}

string WP1ConditionBean::prepareSqlUpdate() {
	string updateFix = "UPDATE WP1_CONDITIONS SET ";
	string updateVar = "FIELD=\'" + field +
			           "\',OPERATOR="+"\'"+operatore +
			           "\',OPERANDS=" + "\'" + operands  +
			           "\',FIELD_TYPE=" + "\'" + fieldType  +
			           "\',COND_TYPE=" + "\'" + condType  + "\'";
	string where = " WHERE ID_CONDITION=\'" + idCondition + "\'";
	this->sqlUpdate = updateFix+updateVar+where;
	return sqlUpdate;
}

void WP1ConditionBean::prepareSqlDelete() {
	this->sqlDelete = "DELETE FROM WP1_CONDITIONS WHERE ID_CONDITION=\'"+idCondition+"\'";
}

void WP1ConditionBean::printWP1CondRow() {
	this->condRow = ""+
	this->idCondition + "#" +
	this->field + "#" +
	this->operatore + "#" +
	this->operands + "#" +
	this->fieldType + "#" +
	this->condType;
	cout << this->condRow << endl;
}
