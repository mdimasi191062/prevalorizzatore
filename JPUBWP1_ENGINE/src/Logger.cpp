/*
 * Logger.cpp
 *
 *  Created on: 11/11/2014
 *      Author: Marco Onotri
 */


#include <iostream>
#include <string>
#include <sstream>
#include <fstream>
#include <stdio.h>
#include <sys/time.h>
#include <Logger.hpp>

#ifndef OCCI_ORACLE
#include <occi.h>
#endif


Logger* Logger::m_pInstance = 0;

using namespace std;

Logger::Logger() {
    LogFileName = "";
    TagName = "";
    enableLog = 0;

    Utl = Util::Instance();
    Cfg = Config::Instance();
    DB = DB_Manager::Instance();
}

Logger::~Logger() {
}

Logger* Logger::Instance() {
    if (!m_pInstance) {
        m_pInstance = new Logger();
    }
    return m_pInstance;
}

int Logger::Init(string OperationMode, long ID_ELAB) {

    TagName = Cfg->getLogTagName(OperationMode);
    if (TagName.length() == 0) {
        cerr << "Errore nella wp1_config, controllare campo LOG_TAG_NAME." << endl;
        return -1;
    }
    Level = Cfg->getLogLevel(OperationMode);
    if (Level <= 0) {
        cerr << "Errore nella wp1_config, controllare campo LOG_FILE_LEVEL." << endl;
        return -1;
    }
    LogFileName = Cfg->getLogFileName(OperationMode);
    if (LogFileName.length() == 0) {
        cerr << "Errore nella wp1_config, controllare campo LOG_FILE_NAME." << endl;
        return -1;
    }
    
    setLogFileName(ID_ELAB);
    setTimeStamp();
    
    return 0;
}

void Logger::setLogFileName(long ID_ELAB) {
    char tempo[30] = "";

    memset(tempo, '\0', sizeof (tempo));
    Utl->getTime('p', tempo);

    std::size_t found = LogFileName.find_last_of(".log");
    std::size_t found2 = LogFileName.find_last_of("/");
    string Buffer;
    string path;
    if (found != std::string::npos) {
        Buffer = LogFileName.substr(0, found);
    } else {
        Buffer = LogFileName;
    }
    if (found2 != std::string::npos) {
        path = Buffer.substr(0, found2 + 1);
        Buffer = Buffer.substr(found2 + 1, Buffer.length() - 1);
    } else {
        path = "./";
    }
    LogFileName = path + tempo + "_" + Buffer + "_" + Utl->toString(ID_ELAB) + ".log";

    std::cout << "FILE LOG: " << LogFileName << std::endl;
}

void Logger::setTag(const char *tag) {
    TagName = std::string(tag);
    std::cout << "TAG FILE LOG: " << TagName << std::endl;
}

void Logger::setEnabledLog(std::string en) {
    enableLog = atoi(en.c_str());
}

void Logger::setTimeStamp() {

    TimeStamp = atol(Cfg->getTempWp1Job("$TS$").c_str()); //calcTimeStamp();
}

long Logger::getTimeStamp() {

    return TimeStamp;
}

void Logger::Log(int type, bool dbLog, const char *content, ...) {
    va_list arglist;
    char buffer[8192] = "";

    va_start(arglist, content);
    //vsprintf(buffer, content, arglist);
    int n = vsnprintf(buffer, sizeof (buffer), content, arglist);
    va_end(arglist);

    if ((n > -1) && (n < (int)sizeof(buffer)))
        writeLog(type, dbLog, buffer);
    else
        writeLog(type, dbLog, "Il messaggio supera il buffer istanziato!");
      
    /*
     * type=1 ERROR
     * type=2 WARN
     * type=3 INFO
     */
}

void Logger::writeLog(int level, bool dbLog, const char *buffer) {
    char tempo[30] = "";
    ofstream fout;

    memset(tempo, '\0', sizeof (tempo));
    Utl->getTime('L', tempo);
    string message = "[" + string(tempo) + "] - " + TagName + " - " + DB->getIdTicket() + " - " + DB->getIdJob() + " - " + string(buffer);
    try {
        fout.open(LogFileName.c_str(), ios::app);

        if (level <= Level) {
            fout << message << std::endl;
            if (dbLog)
                DB->insertLog(TagName + " - " + string(buffer), Utl->getTimeStampMillisec());
        }
    } catch (ofstream::failure e) {
        cout << "Exception opening/reading file [" << LogFileName << "]." << endl;
        cerr << "Exception opening/reading file [" << LogFileName << "]." << endl;
    }
    fout.close();
}

void Logger::write_log_elab(string esito, string buffer) {
    int returnValue;
    returnValue = DB->executePackageLOG(buffer, esito);
}

void Logger::write_log_elab_dett(string esito, string buffer) {
    int returnValue;
    returnValue = DB->executePackageJOB_LOG(buffer, esito);
}

void Logger::setLevel(int level) {
    Level = level;
}
