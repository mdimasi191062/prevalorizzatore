/*
 * JobInstanceBean.cpp
 *
 *  Created on: Oct 30, 2014
 *      Author: amonteforte
 */

#include "JobInstanceBean.h"

JobInstanceBean::JobInstanceBean() {}

JobInstanceBean::JobInstanceBean(string idJob) {
	this->idJob = idJob;
	//this->paramList.clear();
        //this->jobInstanceRow = "";
}

JobInstanceBean::~JobInstanceBean() {}

void JobInstanceBean::clear() {
	this->idJob = "";
	this->paramList.clear();
        this->jobInstanceRow = "";
}

void JobInstanceBean::printJobInstanceRow() {
	cout << "JOB_ROW#"+ this->idJob << endl;
	list<ParamBean>::iterator it = paramList.begin();
	for (it = paramList.begin(); it != paramList.end(); it++)  {
                (*it).printParamRow();
	}
}

