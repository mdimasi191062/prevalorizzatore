/*
 * WP1Log.h
 *
 *  Created on: Dec 05, 2014
 *      Author: amonteforte
 */

#ifndef WP1LOG_H_
#define WP1LOG_H_

#include "WP1Bean.h"

#define MAX_BUF_SIZE 8096

using namespace std;

class WP1Log : public WP1Bean {

private:
	long ts;
	long idElab;
	string idTicket;
	string idJob;
	Clob messaggio;
	string dataIns;
	
	void stampaClob();

public:
	 WP1Log(long ts,
			long idElab,
			string idTicket,
			string idJob,
			Clob messaggio,
			string dataIns);

    WP1Log();

    virtual ~WP1Log();

    void printWP1Log();
};

#endif /* WP1LOG_H_ */
