/*
 * StringUtil.cpp
 *
 *  Created on: Oct 27, 2014
 *      Author: amonteforte
 */
 
#include "StringUtil.h"

string StringUtil::lcase(const string & s)
{
    string str = s;
    for( string::iterator i = str.begin(); i != str.end(); ++i)
        if (tolower(*i) != (int)*i)
            *i = (char)tolower(*i);
    return str;
}

string StringUtil::ucase(const string & s)
{
    string str = s;
    for( string::iterator i = str.begin(); i != str.end(); ++i)
        if (toupper(*i) != (int)*i)
            *i = (char)toupper(*i);
    return str;
}

string StringUtil::toString(int number)
{
   char str[12];

   sprintf(str, "%d", number);
   
   return str;
}

bool StringUtil::isnumber(const char*s) {
   char* e = NULL;
   (void) strtol(s, &e, 0);
   return e != NULL && *e == (char)0;
}

bool StringUtil::IsDateValid(string dt) {
   struct tm tm;
   string strDate;
   bool isValid = true;   
   
   if (dt.length() == 8) {
		strDate = dt.substr (0,4) + "/" + dt.substr (4,2) + "/" + dt.substr (6,2);

		char* result_strptime = strptime(strDate.c_str(), "%Y/%m/%d", &tm);

		if (result_strptime != NULL) {
			if ( !strcmp(result_strptime, "") == 0) {
				isValid = false;
			}
		} else {
			isValid = false;
		}
	} else {
		isValid = false;
	}
	return isValid;
}
