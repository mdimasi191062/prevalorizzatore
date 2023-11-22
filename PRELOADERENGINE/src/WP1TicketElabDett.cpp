/*
 * WP1TicketElabDett.cpp
 *
 *  Created on: Nov 28, 2014
 *      Author: amonteforte
 */

#include "WP1TicketElabDett.h"



WP1TicketElabDett::WP1TicketElabDett() {
}

WP1TicketElabDett::WP1TicketElabDett(long ID_ELABORAZIONE,
									string ID_TICKET,
									string ID_JOB,
									string ID_CONDITION,
									string ID_OPERATION,
									string DESCRIZIONE,
									string ESITO,
									int CONTATORE,
									string DATA_MODIFICA) : WP1Bean() {
	this->ID_ELABORAZIONE = ID_ELABORAZIONE;
	this->ID_TICKET = ID_TICKET;
	this->ID_JOB = ID_JOB;
	this->ID_CONDITION = ID_CONDITION;
	this->ID_OPERATION = ID_OPERATION;
	this->DESCRIZIONE = DESCRIZIONE;
	this->ESITO = ESITO;
	this->CONTATORE = CONTATORE;
	this->DATA_MODIFICA = DATA_MODIFICA;
}

WP1TicketElabDett::~WP1TicketElabDett() {}

void WP1TicketElabDett::printWP1TicketElabDett() {
    char buffer1[40];
    sprintf(buffer1, "%lu", ID_ELABORAZIONE);
    char buffer2[40];
    sprintf(buffer2, "%d", CONTATORE);
	cout << "ID_ELABORAZIONE: " << string(buffer1) << endl;
	cout << "ID_TICKET: " << ID_TICKET << endl;
	cout << "ID_JOB: " << ID_JOB << endl;
	cout << "ID_CONDITION: " << ID_CONDITION << endl;
	cout << "ID_OPERATION: " << ID_OPERATION << endl;
	cout << "DESCRIZIONE: " << DESCRIZIONE << endl;
	cout << "ESITO: " << ESITO << endl;
	cout << "CONTATORE: " << string(buffer2) << endl;
	cout << "DATA_MODIFICA: " << DATA_MODIFICA << endl;
	cout << "**********************************************************************************" << endl;
}
