/*
 * WP1Bean.cpp
 *
 *  Created on: Oct 28, 2014
 *      Author: amonteforte
 */

#include "WP1Bean.h"

WP1Bean::WP1Bean() {
	this->sqlInsert = "";
	this->sqlUpdate = "";
	this->sqlInsert = "";
	this->line = 0;
}

WP1Bean::~WP1Bean() {
}

void WP1Bean::printLogicErrorAndExitProgram(string field, int line) {
	cout << "Bad field " << field << " at line " << line << ". Please check and modify input file before run the program again!!!" << endl;
	exit(1);
}

