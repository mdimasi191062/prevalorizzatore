/*
 * WP1TicketElab.h
 *
 *  Created on: Dec 05, 2014
 *      Author: amonteforte
 */

#ifndef WP1TICKETELAB_H_
#define WP1TICKETELAB_H_

#include "WP1Bean.h"

#define MAX_BUF_SIZE 8096

using namespace std;

class WP1TicketElab : public WP1Bean {

private:
	long ID_ELABORAZIONE;
	string ID_TICKET;
	string DESCRIZIONE_ERRORE;
	string ESITO;
	string DATA_MODIFICA;

public:
	 WP1TicketElab(	long ID_ELABORAZIONE,
					string ID_TICKET,
					string DESCRIZIONE_ERRORE,
					string ESITO,
					string DATA_MODIFICA);

    WP1TicketElab();

    virtual ~WP1TicketElab();

    void printWP1TicketElab();
};

#endif /* WP1TICKETELAB_H_ */
