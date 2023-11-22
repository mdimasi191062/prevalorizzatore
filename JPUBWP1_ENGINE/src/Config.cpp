/*
 * Config.cpp
 *
 *  Created on: 27/ott/2014
 *      Author: Marco
 */

#include <Config.hpp>
#include <iostream>
#include <string>
#include <vector>

#include "Logger.hpp"

#ifndef OCCI_ORACLE
#include <occi.h>
#endif


using namespace std;
using namespace oracle::occi;

ConfigRecord::ConfigRecord() // Constructor
{
    Active = 0;
    IsMacro = 0;

    ParamName = "";
    ParamValue = "";
    ComponentName = "";
}

ConfigRecord::ConfigRecord(const ConfigRecord &copyin) // Copy constructor to handle pass by value.
{
    IsMacro = copyin.IsMacro;
    Active = copyin.Active;
    ComponentName = copyin.ComponentName;
    ParamName = copyin.ParamName;
    ParamValue = copyin.ParamValue;
}

void ConfigRecord::SetField(ResultSet *rs, string pValue) {
    IsMacro = rs->getInt(6);
    Active = rs->getInt(4);
    ComponentName = rs->getString(1);
    ParamName = rs->getString(2);
    ParamValue = pValue;
}

void ConfigRecord::Print() {
    cout << "IsMacro: " << IsMacro;
    cout << " - Active: " << Active;
    cout << " - ComponentName: " << ComponentName;
    cout << " - ParamName: " << ParamName;
    cout << " - ParamValue: " << ParamValue << endl;
}

string ConfigRecord::GetField(int Pos) {

    switch (Pos) {
        case 1:
            return ComponentName;
            break;
        case 2:
            return ParamName;
            break;
        case 3:
            return ParamValue;
            break;
        default:
            return "";
            break;
    }
    return "";
}

Config* Config::m_pInstance = 0;

Config::Config() {
    tab_config.clear();
    tab_config.reserve(10);
    Utl = Util::Instance();
    DB = DB_Manager::Instance();
}

Config::~Config() {
}

Config* Config::Instance() {
    if (!m_pInstance) {
        m_pInstance = new Config();
    }
    return m_pInstance;
}

void Config::viewTempWp1Job() {
    //    int cnt = 0;
    Statement *stmt = NULL;
    ResultSet *rs = NULL;

    rs = DB->executeSelectStmt("Select * from Wp1Job_temp", stmt);

    Logger::Instance()->Log(INF, false, "\t PARAMETRI CONFIGURATI");
    while (rs->next() == true) {
        Logger::Instance()->Log(INF, false, "\t PARAMETRI CONFIGURATI %s=%s", rs->getString(1).c_str(), rs->getString(2).c_str());
        cout << "PARAMETRI CONFIGURATI: [" << rs->getString(1) << "] [" << rs->getString(2) << "]" << endl;
    }
    DB->closeRecordset(stmt, rs);
}

void Config::initWp1JobConfig(string Service, string Sistema, string strDataFineCiclo, long TS, char *TicketType) {
   
    insertTempWp1Job("$CODE_TIPO_CONTR$", Service);
    insertTempWp1Job("$SISTEMA$", Sistema);
    insertTempWp1Job("$TICKET_TYPE$", TicketType);
    insertTempWp1Job("$DATA_FINE_PERIODO$", strDataFineCiclo);
    insertTempWp1Job("$TS$", Utl->toString(TS));
    
}

void Config::insertTempWp1Job(string pName, string pValue) {

    string stmSql;
    string result = "";
    Logger::Instance()->Log(DBG,false,"insertTempWp1Job");
         
    if(pValue.size()>0 && pValue.at(0)=='$' && pValue.at(pValue.size()-1)=='$')
    {
         string paramRivalutato="";
         Logger::Instance()->Log(DBG,false,"il parametro %s presenta un valore da ricalcolare: %s",pName.c_str(),pValue.c_str());
         paramRivalutato = DB->getStringFromSelect("SELECT PARAM_VALUE FROM Wp1Job_temp WHERE PARAM_NAME = '" + pValue + "'");
         if(paramRivalutato=="")
         {
             Logger::Instance()->Log(ERR,true,"il parametro %s referenziato dal parametro %s non esiste",pValue.c_str(),pName.c_str());
             throw runtime_error( "il parametro "+pValue+" referenziato dal parametro "+pName+" non esiste");
         }
         else
         {
             pValue=paramRivalutato;
             Logger::Instance()->Log(DBG,false,"il parametro %s è stato rivalutato in %s",pName.c_str(),pValue.c_str());
         }
         
    }
    
    result = DB->getStringFromSelect("SELECT PARAM_NAME FROM Wp1Job_temp WHERE PARAM_NAME = '" + pName + "'");
    string nvalue=Utl->sostituisciApici(pValue);
    if (!result.empty()) {
        stmSql = "UPDATE Wp1Job_temp SET PARAM_NAME='" + pName + "', PARAM_VALUE='" + nvalue + "' WHERE PARAM_NAME='" + pName + "'";
    } else {
        stmSql = "INSERT INTO Wp1Job_temp (PARAM_NAME, PARAM_VALUE) VALUES (";
        stmSql += "'" + pName + "', '" + nvalue + "')";
    }

    DB->executeInsUpdDelStmt(stmSql);

}

string Config::getTempWp1Job(string paramName) {

    string result = "";
    result = DB->getStringFromSelect("Select PARAM_VALUE from Wp1Job_temp WHERE PARAM_NAME='" + Utl->ucase(paramName) + "'");

    return result;
}

string Config::convertOperand(string paramName) {
    string result = "";
    std::size_t indx=0;
    string allResult=paramName;
    
    while((indx=paramName.find("$",0))!=std::string::npos)
    {
        int end= paramName.find("$",indx+1);
        string param= paramName.substr(indx,end-indx+1);
        result= DB->getStringFromSelect("Select PARAM_VALUE from Wp1Job_temp WHERE PARAM_NAME='" + Utl->ucase(param) + "'");
        if(result.compare("")==0)
        {
            return "nul";
        }
        Logger::Instance()->Log(DBG,false,"Sostituisco il parametro %s con %s",param.c_str(),result.c_str());
        paramName.replace(indx,param.length(),result.c_str());
        indx=end+1;
    }
    return paramName;
}

void Config::TrasformField(ConfigRecord *CF, ResultSet *rs) {
    string stmSql;
    string paramValue;

    if (rs->getInt(6) == 1) { //IsMacro

        if (rs->getString(3).compare("") != 0) {
            // Trasformo il paramValue tramite il paramName (Store Procedure)   	  
            stmSql = "BEGIN :1 := " + rs->getString(3) + "; END;";
            paramValue = DB->executePackageConfig(stmSql);

        } else {
            Logger::Instance()->Log(ERR,true,"Impossibile risolvere il parametro %s",rs->getString(2).c_str());
            paramValue = "nul";
        }
    } else {
        paramValue = rs->getString(3);
    }

    //cout << "TrasformField: [" << paramValue << "]" << endl; 
    CF->SetField(rs, paramValue);
}

void Config::readConfig(string operMode) {
    ResultSet *rs, *rs2;
    ConfigRecord *rConfig;
    Statement *stmt = NULL;

    rs = DB->executeSelectStmt("Select * from WP1_CONFIG WHERE COMPONENT_NAME = 'ALL'", stmt);
    if (rs) {
        while (rs->next() == true) {
            rConfig = new ConfigRecord();

            TrasformField(rConfig, rs);

            tab_config.push_back(*rConfig);
        }
        
    }
    DB->closeRecordset(stmt, rs);
    stmt = NULL;

    bool recordFound = false;
        rs2 = DB->executeSelectStmt("Select * from WP1_CONFIG WHERE COMPONENT_NAME = '" + operMode + "_ENGINE'", stmt);
        if (rs2) {
            while (rs2->next() == true) {
                recordFound = true;
                if ((rs2->getString(2)).compare(0, 3, "LOG") != 0) {
                    rConfig = new ConfigRecord();

                    TrasformField(rConfig, rs2);

                    tab_config.push_back(*rConfig);
                    delete rConfig;
                } 
            }
            DB->closeRecordset(stmt, rs2);

            if (recordFound == false) {
                throw runtime_error( "COMPONENT_NAME = '" + operMode + "_ENGINE', non presente nella WP1_CONFIG.");
            } 
        }
     Logger::Instance()->Log(INF,false,"LETTO PARAMETRI GLOBALI:");   
     for(std::vector<ConfigRecord>::iterator it = tab_config.begin(); it != tab_config.end(); ++it) {
        Logger::Instance()->Log(INF,true,"PARAMETRO GLOBALE %s=%s",it->GetField(2).c_str(),it->GetField(3).c_str());
     }
}

void Config::CaricaWp1Config() {


    std::vector <ConfigRecord>::iterator myIterConfig = tab_config.begin();

    for (myIterConfig = tab_config.begin(); myIterConfig != tab_config.end(); myIterConfig++) {
        if (myIterConfig->GetField(3).empty() == false) {
            insertTempWp1Job(myIterConfig->GetField(2), myIterConfig->GetField(3));
            Logger::Instance()->Log(DBG,false,"COPIA PARAMETRO GLOBALE %s=%s",myIterConfig->GetField(2).c_str(),myIterConfig->GetField(3).c_str());
        } else {
            Logger::Instance()->Log(DBG,false,"PARAMETRO %s vuoto. Impostato a NULL",myIterConfig->GetField(2).c_str());
            insertTempWp1Job(myIterConfig->GetField(2), "nul");
        }
    }
}

string Config::getLogFileName(string strMod) {
    string strResult;

    strResult = DB->getStringFromSelect("SELECT PARAM_VALUE FROM WP1_CONFIG WHERE PARAM_NAME = 'LOG_FILE_NAME' AND COMPONENT_NAME = '" + strMod + "_ENGINE'");

    return strResult;
}

int Config::getLogLevel(string strMod) {
    string strResult;

    strResult = DB->getStringFromSelect("SELECT PARAM_VALUE FROM WP1_CONFIG WHERE PARAM_NAME = 'LOG_FILE_LEVEL' AND COMPONENT_NAME = '" + strMod + "_ENGINE'");

    return (atoi(strResult.c_str()));
}

string Config::getLogTagName(string strMod) {
    string strResult;

    strResult = DB->getStringFromSelect("SELECT PARAM_VALUE FROM WP1_CONFIG WHERE PARAM_NAME = 'LOG_TAG_NAME' AND COMPONENT_NAME = '" + strMod + "_ENGINE'");

    return strResult;
}

/*
string Config::getTempConfig(string paramName) {

    string result = "";
    result = DB->getStringFromSelect("Select PARAM_VALUE from config_temp WHERE PARAM_NAME='" + Utl->ucase(paramName) + "'");

    return result;
}

string Config::CaricaTempConfig() {
    bool status = false;
    string ResultString = "";

    std::vector <ConfigRecord>::iterator myIterConfig = tab_config.begin();

    for (myIterConfig = tab_config.begin(); myIterConfig != tab_config.end(); myIterConfig++) {
        if (myIterConfig->GetField(3).empty() == false) {
            status = insertTempConfig(myIterConfig->GetField(2), myIterConfig->GetField(3));
        } else {
            cout << "CaricaTempConfig-> Errore: Parametro " << myIterConfig->GetField(2) << " = NULL." << endl;
            ResultString = "Parametro: " + myIterConfig->GetField(2) + " = NULL.";
            status = insertTempConfig(myIterConfig->GetField(2), "nul");
        }
    }
    return ResultString;
}

bool Config::createTempConfig(string Service, string Sistema, string strDataFineCiclo,long TS, char *TicketType) {
    string stmSql;
    bool status;


    status = DB->executeInsUpdDelStmt("TRUNCATE table config_temp");
    status = DB->executeInsUpdDelStmt("DROP table config_temp");

    stmSql = "CREATE GLOBAL TEMPORARY TABLE config_temp ";
    stmSql += "(PARAM_NAME varchar2(255) not null, ";
    stmSql += "PARAM_VALUE varchar2(512) not null) ON COMMIT PRESERVE ROWS";
    
    status = DB->executeInsUpdDelStmt(stmSql);
    
    
    if (status == 0) {
        status = insertTempConfig("$CODE_TIPO_CONTR$", Service);
        status = insertTempConfig("$SISTEMA$", Sistema);
        status = insertTempConfig("$TICKET_TYPE$", TicketType);
        status = insertTempConfig("$DATA_FINE_PERIODO$", strDataFineCiclo);
        status = insertTempConfig("$TS$", Utl->toString(TS));
    }

    return status;
}

bool Config::insertTempConfig(string pName, string pValue) {
    try {
        bool status;
        string stmSql;
        string result = "";


        result = DB->getStringFromSelect("SELECT PARAM_NAME FROM config_temp WHERE PARAM_NAME = '" + pName + "'");

        if (!result.empty()) {
            stmSql = "UPDATE config_temp SET PARAM_NAME='" + pName + "', PARAM_VALUE='" + pValue + "' WHERE PARAM_NAME='" + pName + "'";
        } else {
            stmSql = "INSERT INTO config_temp (PARAM_NAME, PARAM_VALUE) VALUES (";
            stmSql += "'" + pName + "', '" + pValue + "')";
        }

        status = DB->executeInsUpdDelStmt(stmSql);

        return status;
    }    catch (SQLException& ex) {
        int errno = ex.getErrorCode();
        Logger::Instance()->Log(ERR, false,"%s \n\t Error: %d -> %s",ex.what(),errno,ex.getMessage().c_str());
        return -1;
    }
}

void Config::viewTempConfig() {
    int cnt = 0;
    Statement *stmt = NULL;
    ResultSet *rs = NULL;

    rs = DB->executeSelectStmt("Select * from config_temp", stmt);
    while (rs->next() == true) {
        cout << ++cnt << ": [" << rs->getString(1) << "] 2: [" << rs->getString(2) << "]" << endl;
    }
    DB->closeRecordset(stmt, rs);
}
 
void Config::createTempWp1Job() {
    bool status;
    string stmSql;

    status = DB->executeInsUpdDelStmt("Truncate table Wp1Job_temp");

    //stmSql = "CREATE GLOBAL TEMPORARY TABLE Wp1Job_temp ON COMMIT PRESERVE ROWS AS SELECT * from config_temp";    
    stmSql = "INSERT INTO Wp1Job_temp (PARAM_NAME, PARAM_VALUE) SELECT PARAM_NAME, PARAM_VALUE FROM config_temp";
    status = DB->executeInsUpdDelStmt(stmSql);
}
*/
