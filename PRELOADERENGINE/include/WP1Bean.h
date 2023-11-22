/*
 * WP1Bean.h
 *
 *  Created on: Oct 28, 2014
 *      Author: amonteforte
 */

#ifndef WP1BEAN_H_
#define WP1BEAN_H_

#include <string>
#include <iostream>
#include <cstdio>
#include "DBManager.hpp"
#include "StringUtil.h"

using namespace std;

class WP1Bean {
protected:
	string sqlInsert, sqlUpdate, sqlDelete;
	int line;

public:
	WP1Bean();
	virtual ~WP1Bean();
	
	void printLogicErrorAndExitProgram(string field, int line);

	inline string getSqlInsert() const {
		return sqlInsert;
	}

	inline string getSqlUpdate() const {
		return sqlUpdate;
	}

	inline string getSqlDelete() const {
		return sqlDelete;
	}
	
	inline void setLine(int line) {
		this->line = line;
	}
	
	inline int getLine() const {
		return this->line;
	}
};

#endif /* WP1BEAN_H_ */
