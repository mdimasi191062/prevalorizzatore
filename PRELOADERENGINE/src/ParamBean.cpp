/*
 * ParamBean.cpp
 *
 *  Created on: Oct 31, 2014
 *      Author: amonteforte
 */

#include "ParamBean.h"

ParamBean::ParamBean() {}

ParamBean::ParamBean(string nameParam, string valParam, int isMacro) {
	this->nameParam = nameParam;
	this->valParam = valParam;
	this->isMacro = isMacro;
        //this->paramRow = "";
}

ParamBean::~ParamBean() {}

void ParamBean::clear() {
	this->nameParam = "";
	this->valParam = "";
	this->isMacro = 0;
    this->paramRow = "";
}

void ParamBean::printParamRow() {
	char buffer[10];
	sprintf(buffer, "%d", this->isMacro);
	cout << "JOB_PARAM_ROW#" +
	this->nameParam + "#" +
	this->valParam + "#" +
	string(buffer) << endl;
}

void ParamBean::printLogicErrorAndExitProgram(string field, int line) {
	cout << "Bad field " << field << " at line " << line << ". Please check and modify input file before run the program again!!!" << endl;
	exit(1);
}

void ParamBean::logicCheck(int line) {
	//IS_MACRO
	if ( (this->isMacro != 0) &&
		 (this->isMacro != 1) ) {
		 printLogicErrorAndExitProgram("IS_MACRO", line);
	}
}
