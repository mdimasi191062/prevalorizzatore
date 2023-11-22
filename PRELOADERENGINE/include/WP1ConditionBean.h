/*
 * WP1ConditionBean.h
 *
 *  Created on: Oct 28, 2014
 *      Author: amonteforte
 */

#ifndef WP1CONDITIONBEAN_H_
#define WP1CONDITIONBEAN_H_

#include "WP1Bean.h"

class WP1ConditionBean: public WP1Bean {
private:
	string idCondition, field, operatore, operands, fieldType, condType, condRow;
public:
	WP1ConditionBean();
	WP1ConditionBean(string idCondition, string field, string operatore, string operands, string fieldType, string condType);
	virtual ~WP1ConditionBean();

	string prepareSqlInsert();
	string prepareSqlUpdate();
	void prepareSqlDelete();
	void printWP1CondRow();
};

#endif /* WP1CONDITIONBEAN_H_ */
