/*
 * WP1TicketElabDett.h
 *
 *  Created on: Dec 05, 2014
 *      Author: amonteforte
 */

#ifndef WP1TICKETELABDETT_H_
#define WP1TICKETELABDETT_H_

#include "WP1Bean.h"

using namespace std;

class WP1TicketElabDett : public WP1Bean {

private:
	long ID_ELABORAZIONE;
	string ID_TICKET;
	string ID_JOB;
	string ID_CONDITION;
	string ID_OPERATION;
	string DESCRIZIONE;
	string ESITO;
	int CONTATORE;
	string DATA_MODIFICA;

public:
	 WP1TicketElabDett(	long ID_ELABORAZIONE,
						string ID_TICKET,
						string ID_JOB,
						string ID_CONDITION,
						string ID_OPERATION,
						string DESCRIZIONE,
						string ESITO,
						int CONTATORE,
						string DATA_MODIFICA);

    WP1TicketElabDett();

    virtual ~WP1TicketElabDett();

    void printWP1TicketElabDett();
};

#endif /* WP1TICKETELABDETT_H_ */
