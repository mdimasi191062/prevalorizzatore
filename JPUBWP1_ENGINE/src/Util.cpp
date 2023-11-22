/*
 * Util.cpp
 *
 *  Created on: 31/ott/2014
 *      Author: Marco
 */

#include <Util.hpp>
#include <iostream>
#include <string>

using namespace std;

Util* Util::m_pInstance = 0;

Util::Util()
{
   
}

Util::~Util()
{
}

Util* Util::Instance()
{
	if(!m_pInstance) {
		m_pInstance = new Util();
	}
	return m_pInstance;
}

string Util::trim(const string & s)
{
    size_t l = s.length();
    if (l == 0)
        return s;
    size_t b = s.find_first_not_of(" \t\r\n\0");
    if (b == std::string::npos)
        b = 0;
    size_t e = s.find_last_not_of(" \t\r\n\0");
    if (e == std::string::npos)
        return s.substr(b);
    return s.substr(b, e-b+1);
}
string Util::sostituisciApici(string value)
{
    return this->ReplaceString(value,"'","''");    
}
std::string Util::ReplaceString(std::string subject, const std::string& search,
                          const std::string& replace) {
    size_t pos = 0;
    while ((pos = subject.find(search, pos)) != std::string::npos) {
         subject.replace(pos, search.length(), replace);
         pos += replace.length();
    }
    return subject;
}
string Util::lcase(const string & s)
{
    string str = s;
    for( string::iterator i = str.begin(); i != str.end(); ++i)
        if (tolower(*i) != (int)*i)
            *i = (char)tolower(*i);
    return str;
}

string Util::ucase(const string & s)
{
    string str = s;
    for( string::iterator i = str.begin(); i != str.end(); ++i)
        if (toupper(*i) != (int)*i)
            *i = (char)toupper(*i);
    return str;
}

string Util::toString(int number)
{
   char str[12];

   sprintf(str, "%d", number);
   
   return str;
}

bool Util::isnumber(const char*s) {
   char* e = NULL;
   (void) strtol(s, &e, 0);
   return e != NULL && *e == (char)0;
}

bool Util::IsDateValid(std::string dt, std::string &strDate) {
    struct tm tm;
    bool isValid = true;

    //cout << "IsDateValid: [" << dt << "]" << endl;

    if (dt.length() >= 8) {
        strDate = dt.substr(0, 4) + "/" + dt.substr(4, 2) + "/" + dt.substr(6, 2);
        //cout << "DateValid: [" << strDate << "]" << endl;

        char *testDate = strptime(strDate.c_str(), "%Y/%m/%d", &tm);
        if (testDate != NULL) {
            if (!strcmp(testDate, "") == 0) {
                cout << "testDate: [" << testDate << "]" << endl;
                std::cout << "IsDateValid -> data: " << dt << " invalida !!" << endl;
                std::cout << "IsDateValid -> Formato atteso: YYYYmmdd" << endl;
                isValid = false;
            } else {
                strDate = dt.substr(6, 2) + "/" + dt.substr(4, 2) + "/" + dt.substr(0, 4);
            }
        } else {
            std::cout << "IsDateValid -> data: " << dt << " invalida !!" << endl;
            std::cout << "IsDateValid -> Formato atteso: YYYYmmdd" << endl;
            isValid = false;
        }
    } else {
        std::cout << "IsDateValid -> data: " << dt << " invalida !!" << endl;
        std::cout << "IsDateValid -> Formato atteso: YYYYmmdd" << endl;
        isValid = false;
    }
    return isValid;
}

long Util::getTimeStamp() {
    struct timeval tv;
    gettimeofday(&tv, NULL);
	
	return tv.tv_sec;
}

long Util::getTimeStampMillisec() {
    struct timeval tp;
    gettimeofday(&tp, NULL);
    long int ms = tp.tv_sec * 1000 + tp.tv_usec / 1000;
    
	return ms;
}

void Util::getDateTime(char *dt) {
    char buffer[50];
    time_t t = time(NULL);
    struct tm tm = *localtime(&t);

    memset(buffer, '\0', sizeof(buffer));
    sprintf(buffer, "%d%d%d%d%d%d",  tm.tm_mday, tm.tm_mon + 1, tm.tm_year + 1900, tm.tm_hour, tm.tm_min, tm.tm_sec);

    strcpy(dt, buffer);
}

void Util::getTime(char tL, char *bufferTime)
{
	struct timeb buf;
	struct tm *timeinfo;
	time_t t;

	ftime(&buf);
	time(&t);
	timeinfo = localtime(&t);

	if (tL == 'T')
		sprintf(bufferTime,"%24s",ctime(&t));
	else if (tL == 'L')
		strftime(bufferTime,20,"%d%m%Y%H%M%S",timeinfo);
	else if (tL == 'P')
		strftime(bufferTime,12,"_%d%m%Y",timeinfo);
	else if (tL == 'X')
		strftime(bufferTime,10,"_%m%Y",timeinfo);
	else if (tL == 'M')
	   sprintf(bufferTime,"%ld.%d",buf.time,buf.millitm);
        else if (tL == 'p')
		strftime(bufferTime,12,"%Y%m%d",timeinfo);
}

