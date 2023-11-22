/*
 * WP1JobBean.h
 *
 *  Created on: Oct 28, 2014
 *      Author: amonteforte
 */

#ifndef WP1JOBBEAN_H_
#define WP1JOBBEAN_H_

#include "WP1Bean.h"

class WP1JobBean: public WP1Bean {
private:
	string idJob, descrizione, jobType, codeTipoContr, masterTable, jobRow;
	int active;
public:
	WP1JobBean();
	WP1JobBean(string idJob, string descrizione, string jobType, int active, string codeTipoContr, string masterTable);
	virtual ~WP1JobBean();

	string prepareSqlInsert();
	string prepareSqlUpdate();
	string prepareSqlDelete();
	void printJobRow();
	void logicCheck(int line);
};

#endif /* WP1JOBBEAN_H_ */
