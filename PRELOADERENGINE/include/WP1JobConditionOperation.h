/*
 * WP1JobConditionOperation.h
 *
 *  Created on: Oct 29, 2014
 *      Author: amonteforte
 */

#ifndef WP1JOBCONDITIONOPERATION_H_
#define WP1JOBCONDITIONOPERATION_H_

#include <vector>
#include <string>
#include <iostream>
#include <cstdio>
#include <list>

using namespace std;

class WP1JobConditionOperation {
private:
	string jobType;
	string idJob;
	list<string> idConditionList;
	list<int> optionalList;
	list<string> idOperationList;
	list<string> sqlInsertList;
	list<string> sqlUpdateList;
	string sqlDeleteJobCondition;
	string sqlDeleteJobOperation;
	string jobRow;
	int line;

public:
	WP1JobConditionOperation(string jobType,string idJob);
	WP1JobConditionOperation();
	virtual ~WP1JobConditionOperation();

	void calculateSqlInsert();
	void calculateSqlUpdate();
	void clear();
	void printJobConfigRow();
	void calculateSqlDelete();
	
	void printLogicErrorAndExitProgram(string field, int line);
	void logicCheck(int line);

	string getSqlDeleteJobCondition() const {
		return sqlDeleteJobCondition;
	}

	string getSqlDeleteJobOperation() const {
		return sqlDeleteJobOperation;
	}

	list<int> getOptionalList() const {
		return optionalList;
	}

	void setOptionalList(list<int> optionalList) {
		this->optionalList = optionalList;
	}
	
	void setIdConditionList(list<string> idConditionList) {
		this->idConditionList = idConditionList;
	}

	void setIdOperationList(list<string> idOperationList) {
		this->idOperationList = idOperationList;
	}

	string getIdJob() const {
		return idJob;
	}

	string getJobType() const {
		return jobType;
	}

	void setIdJob(string idJob) {
		this->idJob = idJob;
	}

	void setJobType(string jobType) {
		this->jobType = jobType;
	}

	list<string> getIdConditionList() {
		return idConditionList;
	}

	list<string> getIdOperationList() {
		return idOperationList;
	}

	list<string> getSqlInsertList() {
		return sqlInsertList;
	}

	list<string> getSqlUpdateList() {
		return sqlUpdateList;
	}
	
	int getLine() {
		return this->line;
	}

	void setLine(int line) {
		this->line = line;
	}
	
	
};

#endif /* WP1JOBCONDITIONOPERATION_H_ */


