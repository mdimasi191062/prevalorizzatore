/*
 * Ticket.hpp
 *
 *  Created on: 31/ott/2014
 *      Author: Marco
 */

#ifndef TICKET_H_
#define TICKET_H_

#ifndef OCCI_ORACLE
# include <occi.h>
#endif
#include <Config.hpp>
#include <DBManager.hpp>
#include <iostream>
#include <vector>
#include <string>

using namespace std;

class ObjTicket
{ 
   public:        	 
      ObjTicket();
      ObjTicket(const ObjTicket &);
      ~ObjTicket(){};
	  string GetField(int Pos);
	  void SetField(ResultSet *rs);
	  void Print();	
	  
	  string ID_TICKET;
          string TICKET_TYPE; 
          string CODE_TIPO_CONTR;
   private:
      
      string DATA_EMISSIONE;
	  string DATA_INIZIO_VAL;
	  string DATA_FINE_VAL;
	  string USER_NWS;
	  string USER_GA;
	  string SYSTEM;
	  string DESCRIZIONE;
	  int ORDINAMENTO;
	  int ACTIVE;
	  
};

class Ticket
{   
   public:
      Ticket();
      Ticket(const Ticket &);
      ~Ticket(){};	  
	  void Print();	
      int Load(Config *,string ,string,string);
      int getCount(); 
      void putTicket(ObjTicket *); 
      ObjTicket getTicket();	  
   private:
	  vector<ObjTicket> tab_ticket;
      DB_Manager *DB;
};

#endif /* TICKET_H_ */

