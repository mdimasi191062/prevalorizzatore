/*
 * Ticket.cpp
 *
 *  Created on: 31/ott/2014
 *      Author: Marco
 */

#include <Ticket.hpp>
#include <iostream>
#include <string>
#include <vector>

#ifndef OCCI_ORACLE
# include <occi.h>
#endif

ObjTicket::ObjTicket()   // Constructor
{
   ACTIVE = 0; 
   ORDINAMENTO = 0;
   ID_TICKET = "";
   DATA_EMISSIONE = "";
   DATA_INIZIO_VAL = "";
   DATA_FINE_VAL = "";
   USER_NWS = "";
   USER_GA = "";
   SYSTEM = "";
   DESCRIZIONE = "";
   CODE_TIPO_CONTR="";
}
 
ObjTicket::ObjTicket(const ObjTicket &copyin)   // Copy constructor to handle pass by value.
{                            
   ID_TICKET = copyin.ID_TICKET;
   DATA_EMISSIONE = copyin.DATA_EMISSIONE;
   DATA_INIZIO_VAL = copyin.DATA_INIZIO_VAL;
   DATA_FINE_VAL = copyin.DATA_FINE_VAL;
   USER_NWS = copyin.USER_NWS;
   USER_GA = copyin.USER_GA;
   SYSTEM = copyin.SYSTEM;
   DESCRIZIONE = copyin.DESCRIZIONE;
   ORDINAMENTO = copyin.ORDINAMENTO;
   CODE_TIPO_CONTR = copyin.CODE_TIPO_CONTR;
   ACTIVE = copyin.ACTIVE;
   TICKET_TYPE = copyin.TICKET_TYPE;
}

void ObjTicket::SetField(ResultSet *rs) 
{   
   ID_TICKET = rs->getString(1); 
   DATA_EMISSIONE = rs->getString(2);
   DATA_INIZIO_VAL = rs->getString(3);
   DATA_FINE_VAL = rs->getString(4);
   USER_NWS = rs->getString(5);
   USER_GA = rs->getString(6);
   SYSTEM = rs->getString(7);
   DESCRIZIONE = rs->getString(8);   
   ORDINAMENTO = rs->getInt(9);
   
   ACTIVE = rs->getInt(10);
   TICKET_TYPE = rs->getString(11);
    CODE_TIPO_CONTR = rs->getString(12);
}

void ObjTicket::Print() 
{   
   cout << "ID_TICKET: " << ID_TICKET << endl;
   cout << "DATA_EMISSIONE: " << DATA_EMISSIONE << endl;
   cout << "DATA_INIZIO_VAL: " << DATA_INIZIO_VAL << endl;
   cout << "DATA_FINE_VAL: " << DATA_FINE_VAL << endl;
   cout << "USER_NWS: " << USER_NWS << endl;
   cout << "USER_GA: " << USER_GA << endl;
   cout << "SYSTEM: " << SYSTEM << endl;
   cout << "DESCRIZIONE: " << DESCRIZIONE << endl;
   cout << "ORDINAMENTO: " << ORDINAMENTO << endl;
   cout << "CODE_TIPO_CONTR: " << CODE_TIPO_CONTR << endl;
   cout << "ACTIVE: " << ACTIVE << endl;
   cout << "TICKET_TYPE: " << TICKET_TYPE << endl;
}

Ticket::Ticket(const Ticket &copyin)   // Copy Constructor
{
   tab_ticket = copyin.tab_ticket;
}

Ticket::Ticket()   // Constructor
{
   DB = DB_Manager::Instance();
   tab_ticket.clear();
   tab_ticket.reserve(999);
}

int Ticket::Load(Config *Cfg,string service,string sistema,string operationMode) {
   int cnt = 0;
   string stmtSql;

   
   stmtSql = "Select ID_TICKET,DATA_EMISSIONE,DATA_INIZIO_VAL,DATA_FINE_VAL,USER_NWS,USER_GA,SYSTEM,DESCRIZIONE, \
             ORDINAMENTO,ACTIVE,TICKET_TYPE,CODE_TIPO_CONTR  from WP1_TICKETGA where ( SYSTEM IS NULL OR SYSTEM = '" + sistema+ 
             "') and (CODE_TIPO_CONTR is null or CODE_TIPO_CONTR='"+service+
             "') and TICKET_TYPE = '" + operationMode + "' AND ACTIVE = 1" +
             " and SYSDATE between DATA_INIZIO_VAL and DATA_FINE_VAL ORDER BY ORDINAMENTO DESC";
  
   ResultSet *rs = NULL;
   Statement *stmt = NULL;
   rs = DB->executeSelectStmt(stmtSql, stmt);
   
   while (rs->next() == true) {
	   ObjTicket *rTicket = new ObjTicket();
       
	   Cfg->insertTempWp1Job ("$ID_TICKET$", rs->getString(1));
	   
	   rTicket->SetField(rs);	   
	   tab_ticket.push_back(*rTicket);	
	   cnt++;	
   }
   DB->closeRecordset(stmt, rs);
   
   return cnt;
}
 
int Ticket::getCount() {
    return tab_ticket.size();
}

void Ticket::putTicket(ObjTicket *rTicket) {
    tab_ticket.push_back(*rTicket);
}

ObjTicket Ticket::getTicket() {
    ObjTicket rTicket;
   
    //rTicket = tab_ticket.back();
    //tab_ticket.pop_back();
		
	rTicket = tab_ticket.front();
	if (!tab_ticket.empty())
		tab_ticket.erase(tab_ticket.begin());
	   
    return (rTicket);
}

void Ticket::Print()
{
    int cnt=0;
    std::vector <ObjTicket>::iterator myIterTicket = tab_ticket.begin();
    for (myIterTicket=tab_ticket.begin(); myIterTicket!=tab_ticket.end(); myIterTicket++) {
        cnt++;
        cout << "---------------- TICKET: " << cnt << " -----------------" << endl;
	    myIterTicket->Print();
	    cout << "-------------------------------------------" << endl;
	   
    }
}
