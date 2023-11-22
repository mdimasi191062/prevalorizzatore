/*
 * WP1ConfigBean.h
 *
 *  Created on: Oct 27, 2014
 *      Author: amonteforte
 */

#ifndef WP1CONFIGBEAN_H_
#define WP1CONFIGBEAN_H_

#include "WP1Bean.h"

using namespace std;

class WP1ConfigBean : public WP1Bean {

private:
	string componentName, paramName, paramValue, descrizione, configRow;
	int active, isMacro;

public:
	WP1ConfigBean();

	WP1ConfigBean(string componentName, string paramName, string paramValue, string descrizione, int active, int isMacro);

	virtual ~WP1ConfigBean();

	string prepareSqlInsert();
	string prepareSqlUpdate();
	void prepareSqlDelete();

	void printWP1ConfigRow();
	
	void logicCheck(int line);

};

#endif /* WP1CONFIGBEAN_H_ */
