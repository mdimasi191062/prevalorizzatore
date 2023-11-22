/*
 * WP1Log.cpp
 *
 *  Created on: Nov 28, 2014
 *      Author: amonteforte
 */

#include "WP1Log.h"



WP1Log::WP1Log() {
}

WP1Log::WP1Log(long ts,
			long idElab,
			string idTicket,
			string idJob,
			Clob messaggio,
			string dataIns) : WP1Bean() {
	this->ts = ts;
	this->idElab = idElab;
	this->idTicket = idTicket;
	this->messaggio = messaggio;
	this->dataIns = dataIns;
}

WP1Log::~WP1Log() {}

void WP1Log::stampaClob() {
	if (this->messaggio.isNull()){
		return;
	}
	unsigned int cloblen = this->messaggio.length();
	if (cloblen == 0) {
		return;
	}
	unsigned char *buffer= new unsigned char[MAX_BUF_SIZE]; 
	memset (buffer, 0, MAX_BUF_SIZE);
	int bytesRead=this->messaggio.read(MAX_BUF_SIZE,buffer,MAX_BUF_SIZE,1);
	cout << "MESSAGGIO: ";
	for (int i = 0; i < bytesRead; ++i)
		cout << buffer[i];
	cout << endl;
	delete buffer;
}

void WP1Log::printWP1Log() {
    char buffer1[40];
    sprintf(buffer1, "%lu", this->ts);
    char buffer2[10];
    sprintf(buffer2, "%lu", this->idElab);
	cout << "TS: " << string(buffer1) << endl;
	cout << "ID_ELAB: " << string(buffer2) << endl;
	cout << "ID_TICKET: " << this->idTicket << endl;
	cout << "ID_JOB: " << this->idJob << endl;
	cout << "DATA_INS: " << this->dataIns << endl;
	stampaClob();
	cout << "----------------------------------------------------------------------------------" << endl;
}
