/*
 * ServiceBean.cpp
 *
 *  Created on: Oct 28, 2014
 *      Author: amonteforte
 */

#include "ServiceBean.h"

ServiceBean::ServiceBean(string id_macro_service, string service, string descrizione) : WP1Bean () {
	this->id_macro_service = id_macro_service;
	this->id_service = service;
	this->descrizione = descrizione;
	this->serviceRow = "";
}

ServiceBean::ServiceBean() {
}

ServiceBean::~ServiceBean() {
}

string ServiceBean::prepareSqlInsert() {

	string insertFix = "Insert into WP1_SERVICES (ID_MACRO_SERVICE,ID_SERVICE,DESCRIZIONE) values ";
	string insertVar = "(\'" + id_macro_service + "\'," +
					   "\'" + id_service + "\'," +
			           "\'" + descrizione + "\')";
	this->sqlInsert = insertFix+insertVar;
	return sqlInsert;
}

string ServiceBean::prepareSqlUpdate() {
	string updateFix = "UPDATE WP1_SERVICES SET ";
	string updateVar = "ID_MACRO_SERVICE=\'"+id_macro_service+"\', DESCRIZIONE=" + "\'" + descrizione + "\'";
    string where = " WHERE ID_SERVICE=\'" + id_service + "\'";
	this->sqlUpdate = updateFix+updateVar+where;
	return sqlUpdate;
}

void ServiceBean::prepareSqlDelete() {
	this->sqlDelete = "DELETE FROM WP1_SERVICES WHERE ID_SERVICE=\'"+id_service+"\'";
}

void ServiceBean::printServiceRow() {
	this->serviceRow = ""+
	this->id_macro_service + "#" +
	this->descrizione + "#" +
	this->id_service;
	cout << this->serviceRow << endl;
	
}
