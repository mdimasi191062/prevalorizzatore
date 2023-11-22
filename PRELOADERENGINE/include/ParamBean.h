/*
 * ParamBeam.h
 *
 *  Created on: Oct 31, 2014
 *      Author: amonteforte
 */

#ifndef PARAMBEAN_H_
#define PARAMBEAN_H_

#include <string>
#include <cstdio>
#include <iostream>

using namespace std;

class ParamBean {
private:
	string valParam;
	string nameParam;
	int isMacro;
	string paramRow;
public:
	ParamBean();
	ParamBean(string nameParam, string valParam, int isMacro);
	~ParamBean();

	void clear();
	
	void printLogicErrorAndExitProgram(string field, int line);
	void logicCheck(int line);

    void printParamRow();

	int getIsMacro() const {
		return isMacro;
	}

	void setIsMacro(int isMacro) {
		this->isMacro = isMacro;
	}

	string getNameParam() const {
		return nameParam;
	}

	void setNameParam(string nameParam) {
		this->nameParam = nameParam;
	}

	string getValParam() const {
		return valParam;
	}

	void setValParam(string valParam) {
		this->valParam = valParam;
	}
};

#endif /* PARAMBEAM_H_ */
