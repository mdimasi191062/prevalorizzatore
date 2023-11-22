/*
 * WP1TicketBean.h
 *
 *  Created on: Oct 30, 2014
 *      Author: amonteforte
 */

#ifndef WP1TICKETBEAN_H_
#define WP1TICKETBEAN_H_

#include <vector>
#include <string>
#include <iostream>
#include <cstdio>
#include <list>
#include "JobInstanceBean.h"
#include "StringUtil.h"

using namespace std;

class WP1TicketBean {
private:
	string idTicket;
	string dataEmissione;
	string dataInizioVal;
	string dataFineVal;
	string userNws;
	string userGA;
	string system;
	string descrizione;
	string ordinamento;
	string active;
	string ticketType;
	string codeTipoContr;
	list<JobInstanceBean> jobInstanceBeanList;

	string sqlInsertTicket;
	string sqlUpdateTicket;
	list<string> sqlInsertInstanceExecList;
	list<string> sqlInsertInstanceParamList;
	list<string> sqlUpdateInstanceExecList;
	list<string> sqlUpdateInstanceParamList;
	string sqlDeleteTicket;
	string sqlDeleteInstanceExec;
	string sqlDeleteInstanceParam;
    string ticketRow;
	int line;
public:
	WP1TicketBean();
	WP1TicketBean(string idTicket, string dataEmissione, string dataInizioVal,
			string dataFineVal, string userNws, string userGA, string system,
			string descrizione, string ordinamento,
			string active, string ticketType, string codeTipoContr);
	~WP1TicketBean();

	void calculateSqlInsert();
	void calculateSqlUpdate();
	void clear();
	void printTicketRow();
	void calculateSqlDelete();
	
	void logicCheck(int line);
	void printLogicErrorAndExitProgram(string field, int line);

	string getSqlDeleteInstanceExec() const {
		return sqlDeleteInstanceExec;
	}

	string getSqlDeleteInstanceParam() const {
		return sqlDeleteInstanceParam;
	}

	string getSqlDeleteTicket() const {
		return sqlDeleteTicket;
	}

	list<string> getSqlUpdateInstanceExecList() const {
		return sqlUpdateInstanceExecList;
	}

	list<string> getSqlUpdateInstanceParamList() const {
		return sqlUpdateInstanceParamList;
	}

	list<string> getSqlInsertInstanceExecList() const {
		return sqlInsertInstanceExecList;
	}

	void setSqlInsertInstanceExecList(list<string> sqlInsertInstanceExecList) {
		this->sqlInsertInstanceExecList = sqlInsertInstanceExecList;
	}

	list<string> getSqlInsertInstanceParamList() const {
		return sqlInsertInstanceParamList;
	}

	void setSqlInsertInstanceParamList(
			list<string> sqlInsertInstanceParamList) {
		this->sqlInsertInstanceParamList = sqlInsertInstanceParamList;
	}

	string getSqlInsertTicket() const {
		return sqlInsertTicket;
	}

	void setSqlInsertTicket(string sqlInsertTicket) {
		this->sqlInsertTicket = sqlInsertTicket;
	}

	string getSqlUpdateTicket() const {
		return sqlUpdateTicket;
	}

	void setSqlUpdateTicket(string sqlUpdateTicket) {
		this->sqlUpdateTicket = sqlUpdateTicket;
	}

	list<JobInstanceBean> getJobInstanceBeanList() const {
		return jobInstanceBeanList;
	}

	void setJobInstanceBeanList(list<JobInstanceBean> jobInstanceBeanList) {
		this->jobInstanceBeanList = jobInstanceBeanList;
	}

	string getActive() const {
		return active;
	}

	void setActive(string active) {
		this->active = active;
	}

	string getDataEmissione() const {
		return dataEmissione;
	}

	void setDataEmissione(string dataEmissione) {
		this->dataEmissione = dataEmissione;
	}

	string getDataFineVal() const {
		return dataFineVal;
	}

	void setDataFineVal(string dataFineVal) {
		this->dataFineVal = dataFineVal;
	}

	string getDataInizioVal() const {
		return dataInizioVal;
	}

	void setDataInizioVal(string dataInizioVal) {
		this->dataInizioVal = dataInizioVal;
	}

	string getDescrizione() const {
		return descrizione;
	}

	void setDescrizione(string descrizione) {
		this->descrizione = descrizione;
	}

	string getCodeTipoContr() const {
		return codeTipoContr;
	}

	void setCodeTipoContr(string codeTipoContr) {
		this->codeTipoContr = codeTipoContr;
	}

	string getIdTicket() const {
		return idTicket;
	}

	void setIdTicket(string idTicket) {
		this->idTicket = idTicket;
	}

	string getOrdinamento() const {
		return ordinamento;
	}

	void setOrdinamento(string ordinamento) {
		this->ordinamento = ordinamento;
	}

	string getSystem() const {
		return system;
	}

	void setSystem(string system) {
		this->system = system;
	}

	string getTicketType() const {
		return ticketType;
	}

	void setTicketType(string ticketType) {
		this->ticketType = ticketType;
	}

	string getUserGa() const {
		return userGA;
	}

	void setUserGa(string userGa) {
		userGA = userGa;
	}

	string getUserNws() const {
		return userNws;
	}

	void setUserNws(string userNws) {
		this->userNws = userNws;
	}
	
	int getLine() const {
		return line;
	}

	void setLine(int line) {
		this->line = line;
	}
};

#endif /* WP1TICKETBEAN_H_ */
