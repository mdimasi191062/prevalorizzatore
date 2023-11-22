/*
 * Job.cpp
 *
 *  Created on: 31/ott/2014
 *      Author: Marco
 */

#include <Job.hpp>
#include <iostream>
#include <string>
#include <vector>
#include <Util.hpp>

#ifndef OCCI_ORACLE
#include <occi.h>
#endif

ObjJob::ObjJob() // Constructor
{
    ID_TICKET = "";
    ID_JOB = "";
    CODE_TIPO_CONTR = "";
    MASTERTABLE = "";
    COUNTER = -1;
    tabKeyType.clear();
    tabKeyType.reserve(10);
    logkey = false;
}

ObjJob::ObjJob(const ObjJob &copyin) // Copy constructor to handle pass by value.
{
    ID_TICKET = copyin.ID_TICKET;
    ID_JOB = copyin.ID_JOB;
    CODE_TIPO_CONTR = copyin.CODE_TIPO_CONTR;
    MASTERTABLE = copyin.MASTERTABLE;
    COUNTER = copyin.COUNTER;
    logkey = copyin.logkey;
}

void ObjJob::SetField(ResultSet *rs) {
    ID_TICKET = rs->getString(1);
    ID_JOB = rs->getString(2);
    CODE_TIPO_CONTR = rs->getString(3);
    MASTERTABLE = rs->getString(4);
    COUNTER = rs->getNumber(5);
}

bool ObjJob::isLogKey() {
    return this->logkey;
}

string ObjJob::GetMST() {
    return MASTERTABLE;
}

int ObjJob::getCounter() {
    return COUNTER;
}

string ObjJob::GetStatementFromTableKey() {
    string stmtFrom = "";

    for (unsigned i = 0; i < tabKeyType.size(); i++) {
        if (stmtFrom.compare("") == 0) {
            if ((tabKeyType[i].type).compare("NUMBER") == 0) {
                stmtFrom = "ID_KEY_NUMBER";
            } else {
                stmtFrom = "ID_KEY_VARCHAR";
            }
        } else {
            stmtFrom += ",";
            if ((tabKeyType[i].type).compare("NUMBER") == 0) {
                stmtFrom += "ID_KEY2_NUMBER";
            } else {
                stmtFrom += "ID_KEY2_VARCHAR";
            }
        }
    }
    return ("Select " + stmtFrom);
}

string ObjJob::GetTableWhithKey() {
    string sqlWhere = "";

    std::vector<KeyType>::iterator KTiter = tabKeyType.begin();

    for (KTiter = tabKeyType.begin(); KTiter != tabKeyType.end(); KTiter++) {
        if (sqlWhere.compare("") == 0)
            sqlWhere += MASTERTABLE + "." + (*KTiter).key;
        else
            sqlWhere += ", " + MASTERTABLE + "." + (*KTiter).key;
    }

    sqlWhere = "( " + sqlWhere + ")";

    return sqlWhere;
}

string ObjJob::GetSelectForMasterTable() {
    string stmtFromSql;
    std::vector<KeyType>::iterator KTiter = tabKeyType.begin();

    stmtFromSql = "Select ";
    for (KTiter = tabKeyType.begin(); KTiter != tabKeyType.end(); KTiter++) {
        stmtFromSql += MASTERTABLE + "." + (*KTiter).key + ",";
    }
    stmtFromSql.erase(stmtFromSql.end() - 1);

    stmtFromSql += " FROM " + MASTERTABLE + " ";

    return stmtFromSql;
}

bool ObjJob::SetMasterTable(Config *Cfg) {
    std::string::size_type pos, lastPos = 0;
    string strBufferMST;
    KeyType recordKey;
    bool result = true;
    bool trimEmpty = false;
    string delimiters = ";";
    string delimiter = ".";
    string tokens = "";

    strBufferMST = Cfg->getTempWp1Job(GetMST());
    if (strBufferMST != "") {
        while (true) {
            pos = strBufferMST.find_first_of(delimiters, lastPos);

            if (pos == std::string::npos) {
                pos = strBufferMST.length();

                if (pos != lastPos || !trimEmpty) {
                    tokens = strBufferMST.substr(lastPos, (pos - lastPos));
                    if (tokens.compare("") != 0) {
                        size_t pos2 = 0;
                        if ((pos2 = tokens.find(delimiter)) != std::string::npos) {
                            recordKey.key = tokens.substr(0, pos2);
                            //std::cout << "**** SetMasterTable->recordKey.key: " << recordKey.key << " ****" << std::endl;
                            tokens.erase(0, pos2 + delimiter.length());
                            recordKey.type = tokens;
                            //std::cout << "**** SetMasterTable->recordKey.type: " << recordKey.type << " ****" << std::endl;	
                            tabKeyType.push_back(recordKey);
                        }
                    } else {
                        result = false;
                        break;
                    }
                }
                break;
            } else {
                if (pos != lastPos || !trimEmpty) {
                    tokens = strBufferMST.substr(lastPos, pos);
                    if (tokens.compare("") != 0) {
                        size_t pos2 = 0;
                        if ((pos2 = tokens.find(delimiter)) != std::string::npos) {
                            recordKey.key = tokens.substr(0, pos2);
                            //std::cout << "**** SetMasterTable->recordKey.key: " << recordKey.key << " ****" << std::endl;
                            tokens.erase(0, pos2 + delimiter.length());
                            recordKey.type = tokens;
                            //std::cout << "**** SetMasterTable->recordKey.type: " << recordKey.type << " ****" << std::endl;	
                            tabKeyType.push_back(recordKey);
                        }
                    } else {
                        result = false;
                        break;
                    }
                }
            }
            lastPos = pos + 1;
        }
    } else {
        throw runtime_error("Impossibile reperire le informazioni di chiave per la tabella " + MASTERTABLE);
    }

    return result;
}

void ObjJob::GetAndSetParametersTo_WP1_JOB(DB_Manager *db, Config *Cfg) {
    string stmtSql;

    stmtSql = "select WP1_INSTANCE_PARAMETERS.NAME_PARAM, WP1_INSTANCE_PARAMETERS.VAL_PARAM, WP1_INSTANCE_PARAMETERS.IS_MACRO, WP1_INSTANCE_PARAMETERS.IS_MACRO ";
    stmtSql += "from WP1_INSTANCE_PARAMETERS where ID_TICKET = '" + ID_TICKET + "' and ID_JOB = '" + ID_JOB + "' AND COUNTER = " + Util::Instance()->toString(COUNTER);

    ResultSet *rs = NULL;
    Statement *stmt = NULL;
    rs = db->executeSelectStmt(stmtSql, stmt);
    this->logkey=false;
    while (rs->next() == true) {
        
        if (rs->getString(1).compare("LOG-KEY") == 0) {
            if (!rs->getString(2).empty()
                    && rs->getString(2).compare("1") == 0) {
                this->logkey=true;
            }
            else
            {
                this->logkey=false;
            }
        }        
        Cfg->insertTempWp1Job(rs->getString(1), rs->getString(2));        
    }
    db->closeRecordset(stmt, rs);
}

string ObjJob::GetIdJob() {
    return ID_JOB;
}

void ObjJob::Print() {
    cout << "ID_TICKET: " << ID_TICKET << endl;
    cout << "ID_JOB: " << ID_JOB << endl;
    cout << "CODE_TIPO_CONTR: " << CODE_TIPO_CONTR << endl;
    cout << "MASTERTABLE: " << MASTERTABLE << endl;
    cout << "COUNTER: " << COUNTER << endl;
}

Job::Job(const Job &copyin) // Copy Constructor
{
    tab_job = copyin.tab_job;
}

Job::Job() { // Constructor

    tab_job.clear();
    tab_job.reserve(999);

    DB = DB_Manager::Instance();
}

int Job::Load(Config *Cfg, string id_ticket, string ticket_type, string code_tipo_contr) {
    try {
        int cnt = 0;
        string stmtSql;

        stmtSql = "select WP1_INSTANCE_EXEC.ID_TICKET, WP1_JOB.ID_JOB, WP1_JOB.CODE_TIPO_CONTR, WP1_JOB.MASTERTABLE, WP1_INSTANCE_EXEC.COUNTER from WP1_TICKETGA ";
        stmtSql += "INNER JOIN  WP1_INSTANCE_EXEC ON (WP1_TICKETGA.ID_TICKET = WP1_INSTANCE_EXEC.ID_TICKET) ";
        stmtSql += "INNER JOIN  WP1_JOB ON (WP1_JOB.ID_JOB = WP1_INSTANCE_EXEC.ID_JOB) ";
        stmtSql += "where WP1_JOB.ACTIVE = 1 ";
        stmtSql += "and WP1_JOB.JOB_TYPE = '" + ticket_type + "' ";
        stmtSql += "and (WP1_JOB.CODE_TIPO_CONTR = '" + code_tipo_contr + "' ";
        stmtSql += "OR WP1_JOB.CODE_TIPO_CONTR is null) ";
        stmtSql += "and WP1_INSTANCE_EXEC.ID_TICKET = '" + id_ticket + "' order by COUNTER desc";

        ResultSet *rs = NULL;
        Statement *stmt = NULL;
        rs = DB->executeSelectStmt(stmtSql, stmt);

        while (rs->next() == true) {
            ObjJob *rJob = new ObjJob();

            rJob->SetField(rs);

            tab_job.push_back(*rJob);

            cnt++;
        }
        DB->closeRecordset(stmt, rs);

        return cnt;
    } catch (SQLException& ex) {
        int errno = ex.getErrorCode();
        Logger::Instance()->Log(ERR, false, "Errore nella ricerca dei job %s \n\t Error:%d -> %s", ex.what(), errno, ex.getMessage().c_str());
        return -1;
    } catch (...) {
        Logger::Instance()->Log(ERR, false, "Errore nella ricerca dei job");
        return -1;
    }
}

void Job::Print() {
    int cnt = 0;
    std::vector <ObjJob>::iterator myIterJob = tab_job.begin();
    for (myIterJob = tab_job.begin(); myIterJob != tab_job.end(); myIterJob++) {
        cnt++;
        cout << "------------------ JOB: " << cnt << " ------------------" << endl;
        myIterJob->Print();
        cout << "--------------------------------------------" << endl;
    }
}

int Job::getCount() {
    return tab_job.size();
}

void Job::putJob(ObjJob *rJob) {
    tab_job.push_back(*rJob);
}

ObjJob Job::getJob() {
    ObjJob rJob;

    rJob = tab_job.back();
    tab_job.pop_back();

    DB->setCondition("");
    DB->setOperation("");
    /*
        rJob = tab_job.front();
    if (!tab_job.empty())
        tab_job.erase(tab_job.begin());
     */
    return (rJob);
}
