/*
 * ServiceBean.h
 *
 *  Created on: Oct 28, 2014
 *      Author: amonteforte
 */

#ifndef SERVICEBEAN_H_
#define SERVICEBEAN_H_

#include <string>
#include <iostream>
#include "WP1Bean.h"

using namespace std;

class ServiceBean : public WP1Bean {
private:
	string id_macro_service, id_service, descrizione, serviceRow;
public:
	ServiceBean();
	ServiceBean(string id_macro_service, string service, string descrizione);
	virtual ~ServiceBean();

	string prepareSqlInsert();
	string prepareSqlUpdate();
	void prepareSqlDelete();
	void printServiceRow();
};

#endif /* SERVICEBEAN_H_ */
