/*
 * StringUtil.h
 *
 *  Created on: Oct 27, 2014
 *      Author: amonteforte
 */

#ifndef STRINGUTIL_H_
#define STRINGUTIL_H_

#include <string>

using namespace std;

class StringUtil {

public:
	string lcase(const string & s);
	string ucase(const string & s);
	string toString(int number);
	bool isnumber(const char*s);
	bool IsDateValid(string dt);
};

#endif /* STRINGUTIL_H_ */
