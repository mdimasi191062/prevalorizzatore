/*
 * JobInstanceBean.h
 *
 *  Created on: Oct 30, 2014
 *      Author: amonteforte
 */

#ifndef JOBINSTANCEBEAN_H_
#define JOBINSTANCEBEAN_H_

#include "ParamBean.h"
#include <list>

class JobInstanceBean {
private:
	string idJob;
	list<ParamBean> paramList;
        string jobInstanceRow;
public:
	JobInstanceBean();
	JobInstanceBean(string idJob);
	~JobInstanceBean();

	void clear();

        void printJobInstanceRow();

	string getIdJob() const {
		return idJob;
	}

	void setIdJob(string idJob) {
		this->idJob = idJob;
	}

	list<ParamBean> getParamList() const {
		return paramList;
	}

	void setParamList(list<ParamBean> paramList) {
		this->paramList = paramList;
	}

};

#endif
