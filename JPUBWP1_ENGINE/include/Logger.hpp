/*
 * Logger.hpp
 *
 *  Created on: 11/11/2014
 *      Author: Marco Onotri
 */

#ifndef LOGGER_H_
#define LOGGER_H_

#include <iostream>
#include <fstream>
#include <string>
#include <sys/stat.h>
#include <stdarg.h>
#include <Config.hpp>
#include <DBManager.hpp>
#include <Util.hpp>

#define ERR  1
#define WRN  2
#define INF  3
#define DBG  4
#define OK  "OK"
#define KO  "KO"
#define RUN "RUNNING"
#define logLevel  1  // 1 ERROR, 2 WARN, 3 INFO

class Logger {
public:
    Logger();
    ~Logger();
    static Logger* Instance();

    int  Init(string OperationMode, long TIME);
    void Log(int type, bool dbLog, const char *content, ...);
    void setLogFileName(long TIME);
    void setTag(const char *filename);
    void setLevel(int level);
    void setTimeStamp();
    long getTimeStamp();
    long getMilliSecond();
    void write_log_elab(string esito, string buffer);
    void write_log_elab_dett(string esito, string buffer);

private:
    static Logger* m_pInstance;

    string LogFileName;
    string TagName;
    int Level;
    long TimeStamp;

    unsigned short enableLog;
    void writeLog(int, bool, const char*);
    void setEnabledLog(string);

    Util *Utl;
    Config *Cfg;
    DB_Manager *DB;
};
#endif /* LOGGER_H_ */
