/*
 * WP1OperationBean.h
 *
 *  Created on: Oct 28, 2014
 *      Author: amonteforte
 */

#ifndef WP1OPERATIONBEAN_H_
#define WP1OPERATIONBEAN_H_

#include "WP1Bean.h"

class WP1OperationBean: public WP1Bean {
private:
	string idOperation, field, operatore, operand, opType, operRow;
public:
	WP1OperationBean();
	WP1OperationBean(string idOperation, string field, string operatore, string operand, string opType);
	virtual ~WP1OperationBean();

	string prepareSqlInsert();
	string prepareSqlUpdate();
	void prepareSqlDelete();
	void printWP1OperationRow();
};

#endif /* WP1OPERATIONBEAN_H_ */
