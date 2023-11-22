/*
 * JPUBWP1_Preloader.h
 *
 *  Created on: Nov 3, 2014
 *      Author: amonteforte
 */

#ifndef JPUBWP1_PRELOADER_H_
#define JPUBWP1_PRELOADER_H_

#define NUM_FILELDS_WP1_CONFIG 6
#define NUM_FILELDS_WP1_SERVICES 3	
#define NUM_FILELDS_WP1_JOB 6
#define NUM_FILELDS_WP1_CONDITION 6
#define NUM_FILELDS_WP1_OPERATION 5
#define NUM_FIELDS_TICKET_ROW 13
#define NUM_FIELDS_JOB_ROW 2
#define NUM_FIELDS_JOB_PARAM_ROW 4
#define NUM_FIELDS_JOB_CONFIG_ROW 3
#define NUM_FIELDS_JOB_CONDITION_ROW 3
#define NUM_FIELDS_OPERATION_ROW 2

#include <list>
#include <iostream>
#include <sstream>
#include <fstream>
#include <string>
#include <cstdlib>
#include <iterator>
#include "DBManager.hpp"


using namespace std;

bool setupDBConnection();

void split_line(string line, string delim, list<string>& values) ;

void split_line_delimiter(string line, vector<string>& values);

void split(string& s, char c, vector<string>& v);

void onErrorCatalog(int errorNum, int line);

void onErrorConfig(int errorNum, int line);

void checkSintattico(int totFields, int numFieldsPerLine);

void checkRow(string s, int numFieldsInRow, int line);

string trimString(string input);

bool isnumber(const char*s);

string ucase(const string& s);


bool IsDateValid(string dt);

void printUsage(string execName);

unsigned char* clobToChar(Clob clob, int size) throw (SQLException);

//------------------------ WP1_CONFIG section BEG ------------------------//
void commit_prepared_insert_WP1_CONFIG();

void commit_prepared_update_WP1_CONFIG() ;

void prepare_query_WP1_CONFIG(const char* fileName);

void commit_deleted_WP1_CONFIG();

void viewWP1Config();
//------------------------ WP1_CONFIG section END ------------------------//

//------------------------ WP1_SERVICES section BEG ------------------------//
void commit_prepared_insert_WP1_SERVICES();

void commit_prepared_update_WP1_SERVICES();

void commit_deleted_WP1_SERVICES();

void viewWP1Services();

string lcase(const string & s);

void prepare_query_WP1_SERVICES(const char* fileName);
//------------------------ WP1_SERVICES section END ------------------------//

//------------------------ WP1_JOB section BEG ------------------------//
void commit_prepared_insert_WP1_JOB();

void commit_prepared_update_WP1_JOB();

void commit_deleted_WP1_JOB();

void prepare_query_WP1_JOB(const char* fileName);

void viewJobs(string pattern="");
//------------------------ WP1_JOB section END ------------------------//

//------------------------ WP1_CONDITION section BEG ------------------------//
void commit_prepared_insert_WP1_CONDITION();

void commit_prepared_update_WP1_CONDITION();

void prepare_query_WP1_CONDITION(const char* fileName) ;

void commit_deleted_WP1_CONDITION();

void viewCondition();
//------------------------ WP1_CONDITION section END ------------------------//

//------------------------ WP1_OPERATION section BEG ------------------------//
void commit_prepared_insert_WP1_OPERATION();

void commit_prepared_update_WP1_OPERATION();

void commit_deleted_WP1_OPERATION();

void prepare_query_WP1_OPERATION(const char* fileName) ;

void viewWP1Operation();

//------------------------ WP1_OPERATION section END ------------------------//

//------------------------ WP1_JOB_CONDITION_OPERATION section BEG ------------------------//
void commit_prepared_insert_WP1_JOB_CONDITION_OPERATION();

void commit_prepared_update_WP1_JOB_CONDITION_OPERATION();

void commit_prepared_delete_WP1_JOB_CONDITION_OPERATION();

void prepare_query_WP1_JOB_CONDITION_OPERATION(const char* fileName);

void viewJobConfig(string pattern="");
//------------------------ WP1_JOB_CONDITION_OPERATION section END ------------------------//

//------------------------ WP1_TICKET section BEG ------------------------//
void commit_prepared_insert_WP1_TICKET();

void commit_prepared_delete_WP1_TICKET();

void viewTickets(string pattern="");

void prepare_query_WP1_TICKET(const char* fileName);
//------------------------ WP1_TICKET section END ------------------------//

//------------------------ WP1_ANALISI_TREND section BEG ------------------------//
void viewWP1AnalisiTrend(string pattern="");
//------------------------ WP1_ANALISI_TREND section END ------------------------//

//------------------------ WP1_LOG section BEG ------------------------//
void viewWP1Log(string pattern="");
//------------------------ WP1_LOG section END ------------------------//

//------------------------ WP1_TICKET_ELAB section BEG ------------------------//
void viewWP1TicketElab(string pattern="");
//------------------------ WP1_TICKET_ELAB section END ------------------------//
#endif /* JPUBWP1_PRELOADER_H_ */
