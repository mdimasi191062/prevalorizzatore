/*
 * Util.hpp
 *
 *  Created on: 27/ott/2014
 *      Author: Marco
 */

#ifndef UTIL_H_
#define UTIL_H_
#include <string>
#include <time.h>
#include <sys/time.h>
#include <sys/timeb.h>
#include <stdexcept>
using namespace std;

class Util
{

public:
	Util();
   ~Util();
	static Util* Instance();
	string sostituisciApici(string value);
        std::string ReplaceString(std::string subject, const std::string& search,
                          const std::string& replace);
	string trim(const string & s);
	string lcase(const string & s);
	string ucase(const string & s);
	string toString(int number);
	bool isnumber(const char * s);
    bool IsDateValid(string dt, string & strDate);
	long getTimeStamp();
	long getTimeStampMillisec();
	void getDateTime(char *dt);
	void getTime(char tL, char *bufferTime);
       

private:

    static Util* m_pInstance;
};

#endif /* UTIL_H_ */

