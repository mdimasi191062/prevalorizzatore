/*
 * WP1TicketBean.cpp
 *
 *  Created on: Oct 30, 2014
 *      Author: amonteforte
 */

#include "WP1TicketBean.h"

WP1TicketBean::WP1TicketBean() {}

WP1TicketBean::WP1TicketBean(string idTicket, string dataEmissione,
		string dataInizioVal, string dataFineVal, string userNws,
		string userGA, string system, string descrizione,
		string ordinamento, string active,
		string ticketType, string codeTipoContr) {
	this->idTicket = idTicket;
	this->dataEmissione = dataEmissione;
	this->dataInizioVal = dataInizioVal;
	this->dataFineVal = dataFineVal;
	this->userNws = userNws;
	this->userGA = userGA;
	this->system = system;
	this->descrizione = descrizione;
	this->ordinamento = ordinamento;
	this->active = active;
	this->ticketType = ticketType;
	this->codeTipoContr = codeTipoContr;

	this->jobInstanceBeanList.clear();
	this->sqlInsertTicket = "";
	this->sqlUpdateTicket = "";
	this->sqlInsertInstanceExecList.clear();
	this->sqlInsertInstanceParamList.clear();
	this->sqlUpdateInstanceExecList.clear();
	this->sqlUpdateInstanceParamList.clear();
	this->sqlDeleteTicket = "";
	this->sqlDeleteInstanceExec = "";
	this->sqlDeleteInstanceParam = "";
    this->ticketRow = "";
	this->line = 0;
}

WP1TicketBean::~WP1TicketBean() {}

void WP1TicketBean::calculateSqlInsert() {
	//Iterator
	list<JobInstanceBean>::const_iterator it = jobInstanceBeanList.begin();
	//TICKETGA
	string insertFix = "Insert into WP1_TICKETGA (ID_TICKET,DATA_EMISSIONE,DATA_INIZIO_VAL,DATA_FINE_VAL,USER_NWS,USER_GA,SYSTEM,DESCRIZIONE,ORDINAMENTO,ACTIVE,TICKET_TYPE,CODE_TIPO_CONTR) values ";
	string insertVar = "(\'" + idTicket + "\'," +
			            "TO_DATE(\'" + dataEmissione + "\',\'YYYYMMDD\')," +
			            "TO_DATE(\'" + dataInizioVal + "\',\'YYYYMMDD\')," +
			            "TO_DATE(\'" + dataFineVal  + "\',\'YYYYMMDD\')," +
			            "\'" + userNws + "\'," +
			            "\'" + userGA + "\',"+
			            "\'" + system  + "\'," +
			            "\'" + descrizione + "\'," +
			                   ordinamento + "," +
			                   active  + "," +
			            "\'" + ticketType  + "\'," +
						"\'" + codeTipoContr + "\')";
	this->sqlInsertTicket = insertFix+insertVar;
	//INSTANCE_EXEC
	insertFix = "Insert into WP1_INSTANCE_EXEC (ID_TICKET,ID_JOB,COUNTER) values ";
	insertVar = "";
	int currCycle = 1;
	for (it = jobInstanceBeanList.begin(); it != jobInstanceBeanList.end(); it++)
	{
        char buffer[10];
        sprintf(buffer, "%d", currCycle);
		insertVar = "(\'" + idTicket + "\'," +
					"\'" + (*it).getIdJob() + "\'," +
						   string(buffer) + ")";
		sqlInsertInstanceExecList.push_back(insertFix+insertVar);
		currCycle++;
	}

    //INSTANCE_PARAMETERS
    insertFix = "Insert into WP1_INSTANCE_PARAMETERS (NAME_PARAM,ID_TICKET,ID_JOB,VAL_PARAM,IS_MACRO,COUNTER) values ";
    insertVar = "";
    list<ParamBean>::const_iterator iterParamBean;
	currCycle = 1;
    for(it = jobInstanceBeanList.begin(); it != jobInstanceBeanList.end(); it++)
    {
        char counterBuff[10];
        sprintf(counterBuff, "%d", currCycle);
		
    	list<ParamBean> listParameter=(*it).getParamList();
    	for (iterParamBean = listParameter.begin(); iterParamBean != listParameter.end(); iterParamBean++) {
    		char buffer[10];
    		sprintf(buffer, "%d", (*iterParamBean).getIsMacro());
			insertVar = "(\'" + (*iterParamBean).getNameParam() +  "\'," +
							"\'" + idTicket + "\'," +
							"\'" + (*it).getIdJob() + "\',"+
							"\'" + (*iterParamBean).getValParam()  + "\',"+
							       string(buffer) + "," +
								   string(counterBuff) + ")";
    		sqlInsertInstanceParamList.push_back(insertFix+insertVar);
    	}
		currCycle++;
    }
}

void WP1TicketBean::calculateSqlUpdate() {
	//Iterator
	list<JobInstanceBean>::const_iterator it = jobInstanceBeanList.begin();
	//TICKETGA
	string updateFix = "UPDATE WP1_TICKETGA SET ";
	string updateVar = "ID_TICKET=\'" + idTicket +
			            "\',DATA_EMISSIONE=\'" + dataEmissione +
			            "\',DATA_INIZIO_VAL=\'" + dataInizioVal +
			            "\',DATA_FINE_VAL=\'" + dataFineVal  +
			            "\',USER_NWS=\'" + userNws +
			            "\',USER_GA=\'" + userGA +
			            "\',SYSTEM=\'" + system  +
			            "\',DESCRIZIONE=\'" + descrizione +
			            "\',ORDINAMENTO=\'" + ordinamento +
			            "\',ACTIVE=\'" + active  +
			            "\',TICKET_TYPE=\'" + ticketType  +
						"\',CODE_TIPO_CONTR=\'" + codeTipoContr + "\'";;
	this->sqlUpdateTicket = updateFix+updateVar;
	//INSTANCE_EXEC
	updateFix = "UPDATE WP1_INSTANCE_EXEC SET ";
	updateVar = "";
	int currCycle = 1;
	for (it = jobInstanceBeanList.begin(); it != jobInstanceBeanList.end(); it++)
	{
		char counterBuff[10];
		sprintf(counterBuff, "%d", currCycle);
		
		updateVar = "ID_TICKET=\'" + idTicket +
					"\',ID_JOB=\'" + (*it).getIdJob() + 
					"\',COUNTER=" + string(counterBuff);
		sqlUpdateInstanceExecList.push_back(updateFix+updateVar);
		currCycle++;
	}

	//INSTANCE_PARAMETERS
	updateFix = "UPDATE WP1_INSTANCE_PARAMETERS SET ";
    updateVar = "";
    list<ParamBean>::const_iterator iterParamBean;
	currCycle = 1;
    for(it = jobInstanceBeanList.begin(); it != jobInstanceBeanList.end(); it++)
    {
		char counterBuff[10];
		sprintf(counterBuff, "%d", currCycle);
		
    	list<ParamBean> listParameter=(*it).getParamList();
    	for (iterParamBean = listParameter.begin(); iterParamBean != listParameter.end(); iterParamBean++) {
			char buffer[10];
			sprintf(buffer, "%d", (*iterParamBean).getIsMacro());
			updateVar = "NAME_PARAM=\'" + (*iterParamBean).getNameParam() +
						"\',ID_TICKET=\'" + idTicket +
						"\',ID_JOB=\'" + (*it).getIdJob() +
						"\',VAL_PARAM=\'" + (*iterParamBean).getValParam()  +
						"\',IS_MACRO=\'" + string(buffer) +
						"\',COUNTER=" + string(counterBuff);
    		sqlUpdateInstanceParamList.push_back(updateFix+updateVar);
    	}
		currCycle++;
    }
}

void WP1TicketBean::calculateSqlDelete() {
	//TICKETGA
	this->sqlDeleteTicket = "DELETE FROM WP1_TICKETGA WHERE ID_TICKET=\'"+this->idTicket+"\'";
	//INSTANCE_EXEC
	this->sqlDeleteInstanceExec = "DELETE FROM WP1_INSTANCE_EXEC WHERE ID_TICKET=\'"+this->idTicket+"\'";
	//INSTANCE_PARAMETERS
	this->sqlDeleteInstanceParam = "DELETE FROM WP1_INSTANCE_PARAMETERS WHERE ID_TICKET=\'"+this->idTicket+"\'";
}

void WP1TicketBean::clear() {
	this->idTicket = "";
	this->dataEmissione = "";
	this->dataInizioVal = "";
	this->dataFineVal = "";
	this->userNws = "";
	this->userGA = "";
	this->system = "";
	this->descrizione = "";
	this->ordinamento = "";
	this->active = "";
	this->ticketType = "";
	this->codeTipoContr = "";
	this->jobInstanceBeanList.clear();

	this->sqlInsertTicket = "";
	this->sqlUpdateTicket = "";
	this->sqlInsertInstanceExecList.clear();
	this->sqlInsertInstanceParamList.clear();
	this->sqlUpdateInstanceExecList.clear();
	this->sqlUpdateInstanceParamList.clear();
	this->sqlDeleteTicket = "";
	this->sqlDeleteInstanceExec = "";
	this->sqlDeleteInstanceParam = "";
	this->ticketRow = "";
	this->line = 0;
}

void WP1TicketBean::printTicketRow() {
	this->ticketRow = "TICKET_ROW#"+
	this->idTicket + "#" +
	this->dataEmissione + "#" +
	this->dataInizioVal + "#" +
	this->dataFineVal + "#" +
	this->userNws + "#" +
	this->userGA + "#" +
	this->system + "#" +
	this->descrizione + "#" +
	this->ordinamento + "#" +
	this->active + "#" +
	this->ticketType + "#" +
	this->codeTipoContr;
	//Print TICKET_ROW
	cout <<  this->ticketRow << endl;
	//JOB INSTANCE
	list<JobInstanceBean>::iterator it = jobInstanceBeanList.begin();
	for (; it != jobInstanceBeanList.end(); it++) {
		(*it).printJobInstanceRow();
	}
}

void WP1TicketBean::printLogicErrorAndExitProgram(string field, int line) {
	cout << "Bad field " << field << " at line " << line << ". Please check and modify input file before run the program again!!!" << endl;
	exit(1);
}

void WP1TicketBean::logicCheck(int line) {
	StringUtil stringUtil;
	//ORDINAMENTO
	if (!stringUtil.isnumber(this->ordinamento.c_str())) {
		printLogicErrorAndExitProgram("ORDINAMENTO", line);
	}
	//ACTIVE
	if (!stringUtil.isnumber(this->active.c_str())) {
		printLogicErrorAndExitProgram("ACTIVE", line);
	} else if ( (this->active.compare("0") != 0) &&
				(this->active.compare("1") != 0) ) {
				printLogicErrorAndExitProgram("ACTIVE", line);
	}
	
	//DATA_EMISSIONE
	if (!stringUtil.IsDateValid(this->dataEmissione)) {
		printLogicErrorAndExitProgram("DATA_EMISSIONE", line);
	}
	//DATA_INIZIO_VAL
	if (!stringUtil.IsDateValid(this->dataInizioVal)) {
		printLogicErrorAndExitProgram("DATA_INIZIO_VAL", line);
	}
	//DATA_FINE_VAL
	if (!stringUtil.IsDateValid(this->dataFineVal)) {
		printLogicErrorAndExitProgram("DATA_FINE_VAL", line);
	}
}
