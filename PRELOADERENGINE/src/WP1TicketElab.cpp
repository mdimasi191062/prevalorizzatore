/*
 * WP1TicketElab.cpp
 *
 *  Created on: Nov 28, 2014
 *      Author: amonteforte
 */

#include "WP1TicketElab.h"



WP1TicketElab::WP1TicketElab() {
}

WP1TicketElab::WP1TicketElab(long ID_ELABORAZIONE,
							string ID_TICKET,
							string DESCRIZIONE_ERRORE,
							string ESITO,
							string DATA_MODIFICA) : WP1Bean() {
	this->ID_ELABORAZIONE = ID_ELABORAZIONE;
	this->ID_TICKET = ID_TICKET;
	this->DESCRIZIONE_ERRORE = DESCRIZIONE_ERRORE;
	this->ESITO = ESITO;
	this->DATA_MODIFICA = DATA_MODIFICA;
}

WP1TicketElab::~WP1TicketElab() {}

void WP1TicketElab::printWP1TicketElab() {
    char buffer1[40];
    sprintf(buffer1, "%lu", ID_ELABORAZIONE);
	cout << "ID_ELABORAZIONE: " << string(buffer1) << endl;
	cout << "ID_TICKET: " << ID_TICKET << endl;
	cout << "DESCRIZIONE_ERRORE: " << DESCRIZIONE_ERRORE << endl;
	cout << "ESITO: " << ESITO << endl;
	cout << "DATA_MODIFICA: " << DATA_MODIFICA << endl;
	cout << "*************************************** DETTAGLIO *********************************" << endl;
}
