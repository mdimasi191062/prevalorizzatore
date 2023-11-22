/*
 * Config.hpp
 *
 *  Created on: 27/ott/2014
 *      Author: Marco
 */

#ifndef CONFIG_H_
#define CONFIG_H_

#ifndef OCCI_ORACLE
#include <occi.h>
#endif

#include <iostream>
#include <vector>
#include <string>

#include <DBManager.hpp>
#include <Util.hpp>

using namespace std;
using namespace oracle::occi;
using oracle::occi::Environment;
using oracle::occi::Connection;

struct KeyType {
    string key;
    string type;
};

class ConfigRecord {
public:

    void Print();
    string GetField(int Pos);
    ConfigRecord();
    ConfigRecord(const ConfigRecord &);

    ~ConfigRecord() {
    };
    void SetField(ResultSet *rs, string pValue);
private:
    int IsMacro;
    int Active;

    string ParamName;
    string ParamValue;
    string ComponentName;
};

class Config {
public:
    Config();
    ~Config();
    static Config* Instance();
    string convertOperand(string paramName);
    string getLogFileName(string strMod);
    string getLogTagName(string strMod);
    int getLogLevel(string strMod);
    void readConfig(string operMode);

    //bool createTempConfig(string Service, string Sistema, string strDataFineCiclo,long TS, char *TicketType);    
    //string CaricaTempConfig();    
    //void viewTempConfig();    
    //string getTempConfig(string paramName);
    //bool insertTempConfig(string, string);
    //void createTempWp1Job();
    
    void CaricaWp1Config();
    void viewTempWp1Job();
    void initWp1JobConfig(string Service, string Sistema, string strDataFineCiclo, long TS, char *TicketType);
    string getTempWp1Job(string paramName);
    void insertTempWp1Job(string, string);
    string sostituisciApici(string value);
    vector<ConfigRecord> tab_config;
    void TrasformField(ConfigRecord *, ResultSet *rs);
private:

    Util *Utl;
    DB_Manager *DB;

    static Config* m_pInstance;
};

#endif /* CONFIG_H_ */

