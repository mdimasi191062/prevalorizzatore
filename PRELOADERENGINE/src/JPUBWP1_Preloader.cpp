//============================================================================
// Name        : JPUBWP1_Preloader.cpp
// Author      : amonteforte
// Version     :
// Copyright   : Your copyright notice
// Description : Hello World in C++, Ansi-style
//============================================================================
#include "JPUBWP1_Preloader.h"
#include "WP1ConfigBean.h"
#include "ServiceBean.h"
#include "WP1JobBean.h"
#include "WP1ConditionBean.h"
#include "WP1OperationBean.h"
#include "WP1JobConditionOperation.h"
#include "WP1TicketBean.h"
#include "WP1AnalisiTrend.h"
#include "WP1Log.h"	
#include "WP1TicketElab.h"
#include "WP1TicketElabDett.h"

#define __TRY try {
#define __CATCH } catch (SQLException &ex) { int errNum = ex.getErrorCode(); onErrorCatalog(ex.getMessage(), errNum, lineCatalog);  lineCatalog = 2;}
#define __CATCH_CONFIG } catch (SQLException &ex) { int errNum = ex.getErrorCode(); onErrorConfig(ex.getMessage(), errNum, lineConfig);  lineConfig = 1;}
#define __CATCH_DEBUG } catch (SQLException &ex) { cout << ex.getMessage() << endl; }

using namespace std;

list<WP1ConfigBean> listConfigBean; //WP1_CONFIG section
list<ServiceBean> listServiceBean; //WP1_SERVICES section
list<WP1JobBean> listJobBean; //WP1_JOB section
list<WP1ConditionBean> listCondBean; //WP1_CONDITIONS section
list<WP1OperationBean> listOperBean; //WP1_OPERATION section
list<WP1JobConditionOperation> jobCondOpList; //WP1_JOB_OPERATION_CONDITION
list<WP1TicketBean> ticketBeanList; //WP1_TICKET section
DB_Manager* dataBaseManager;

int lineCatalog = 2;
int lineConfig = 1;

bool setupDBConnection() {
   dataBaseManager = DB_Manager::Instance();  
   string strUser, strPassword, strConnect;

   if ( getenv("JW1USER") != NULL) 
   strUser = getenv("JW1USER");
   else
   {
      cerr << "Environment variable JW1USER must be defined." << endl;
      exit(-1);  
   } 

   if ( getenv("JW1PASSWD") != NULL)
       strPassword = getenv("JW1PASSWD");
   else 
   {
      cerr << "Environment variable JW1PASSWD must be defined." << endl;
      exit(-1);  
   } 

   if ( getenv("JW1CONNECT") != NULL)
       strConnect = getenv("JW1CONNECT");
   else 
   {
      cerr << "Environment variable JW1CONNECT must be defined." << endl;
      exit(-1);  
   } 
   
   dataBaseManager->setCredenziali(strUser, strPassword, strConnect);   //TO UNCOMMENT
   //dataBaseManager->setCredenziali("parma", "parma", "V3");  //TO COMMENT

   return dataBaseManager->connectDB();
}

void checkSintattico(int totFields, int numFieldsPerLine) {
	if (totFields % numFieldsPerLine != 0) {
		cout << "Input file contains errors. Please modify it before run program again!!!" << endl;
		exit(1);
	}
}

void checkRow(string s, int numFieldsInRow, int line)  {
	const char* sChar = s.c_str();
	int cont=0;
	char delim= '#';
	int i=0;
	while ( sChar[i]!='\0') {
		if (sChar[i]==delim)
			cont++;
		i++;
	}	
	if (cont != numFieldsInRow) {
		cout << "Input file contains errors at line " << line <<". Please modify it before run program again!!!" << endl;
		exit(1);
	}
}

void printUsage(string execName) {
    if (execName.compare("./jpubwp1_trend_analysis") == 0) {
        cout << "Usage: " << execName << " [ID_ELAB]" << endl;
    } else if (execName.compare("./jpubwp1_log") == 0) {
		cout << "Usage: " << execName << " YYYYMMDD" << endl;
	} else if (execName.compare("./jpubwp1_ticket_elab") == 0) {
		cout << "Usage: " << execName << " YYYYMMDD" << endl;
	} else {
		cout << "Usage 1: " << execName << " [ADD|UPD|DEL] inputCsvFile" << endl;
		cout << "Usage 2: " << execName << " VIEW" << endl;
		if ((execName.compare("./jpubwp1_tickets_config") == 0) ||
			(execName.compare("./jpubwp1_job_config") == 0) ||
			(execName.compare("./jpubwp1_job_catalog") == 0) ) {
			cout << "Usage 3: " << execName << " VIEW [PATTERN]" << endl;
		}
    }
}

string  trimString(string input) {
	//DOUBLE SINGLE QUOTE
	size_t quotedIndex = input.find("'");
	while (quotedIndex != string::npos) {
		input = input.replace(quotedIndex, 1, "''");
		quotedIndex = input.find("'", quotedIndex+2);
	}
	return input;
}

bool isnumber(const char*s) {
   char* e = NULL;
   (void) strtol(s, &e, 0);
   return e != NULL && *e == (char)0;
}

string ucase(const string& s)
{
    string str = s;
    for( string::iterator i = str.begin(); i != str.end(); ++i)
        if (toupper(*i) != (int)*i)
            *i = (char)toupper(*i);
    return str;
}

string lcase(const string & s)
{
    string str = s;
    for( string::iterator i = str.begin(); i != str.end(); ++i)
        if (tolower(*i) != (int)*i)
            *i = (char)tolower(*i);
    return str;
}

void split_line(string line, string delim, list<string>& values) {
	if (!line.empty()) {
		size_t pos = 0;
		while ((pos = line.find(delim, (pos + 1))) != string::npos) {
			string p = line.substr(0, pos);
						if(!p.empty()) {
				values.push_back(p);
				}
			line = line.substr(pos + 1);
		}	

		if (!line.empty()) {
			values.push_back(line);
		}	
	}
}

void split(string& s, char c, vector<string>& v) {
   string::size_type i = 0;
   string::size_type j = s.find(c);

   while (j != string::npos) {
      v.push_back(s.substr(i, j-i));
      i = ++j;
      j = s.find(c, j);

      if (j == string::npos)
         v.push_back(s.substr(i, s.length()));
   }
}

void split_line_delimiter(string line, vector<string>& values) {
	if (!line.empty()) {
	   istringstream ss(line);
	   while (ss) {
		string s;
		if (!getline( ss, s, '#' )) break;
		values.push_back(s);
		} 
	}
}

void onErrorCatalog(string messageError, int errorNum, int line) {
	switch(errorNum)
	{
		case 1:
			cout << "Duplicated element at line " <<  line << ". Please check and modify the input file." << endl;
			break;
		case 12899:
			cout << "Value too large for column at line " <<  line << ". Please check and modify the input file." << endl;
			break;
		case 2291:
			cout << "Foreign key not found at line " <<  line << ". Please check and modify the input file." << endl;
			break;
		case 1830:
			cout << "Date non correctly formatted. The correct format is YYYYMMDD." << endl;
			break;
		case 1840:
			cout << "Input value not long enough for the date format. The correct format is YYYYMMDD." << endl;
			break;
		case 1843:
			cout << "Date contains not a valid month." << endl;
			break;
		case 1847:
			cout << "Date contains not a valid day for the selected month." << endl;
			break;
		default:
			cout << "ERROR: " << messageError << "." << endl;
	}
}

void onErrorConfig(string errorMessage, int errorNum, int line) {
	switch(errorNum)
	{
		case 1:
			cout << "Element at line " <<  line << " contains duplicated elements or sub-elements. Please check and modify the input file." << endl;
			break;
		case 12899:
			cout << "Value too large for column at line " <<  line << ". Please check and modify the input file." << endl;
			break;
		case 2291:
			cout << "Foreign key not found at line " <<  line << ". Please check and modify the input file." << endl;
			break;
		case 1830:
			cout << "Date non correctly formatted. The correct format is YYYYMMDD." << endl;
			break;
		case 1840:
			cout << "Input value not long enough for the date format. The correct format is YYYYMMDD." << endl;
			break;
		case 1843:
			cout << "Date contains not a valid month." << endl;
			break;
		case 1847:
			cout << "Date contains not a valid day for the selected month." << endl;
			break;
		default:
			cout << "ERROR: " << errorMessage << "." << endl;
	}
}

unsigned char* clobToChar(Clob clob, int size) throw (SQLException) {
/*
    Stream *instream = clob.getStream (1,0);
    char *buffer = new char[size];
    memset (buffer, 0, size);
    instream->readBuffer (buffer, size);
    clob.closeStream (instream);
	return buffer;
*/
	if (clob.length() == 0)
		return new unsigned char[0];
	unsigned char *buffer= new unsigned char[size]; 
	memset (buffer, 0, size);
	clob.read(size,buffer,size,0);
	//for (int i = 0; i < bytesRead; ++i)
	//cout << buffer;
	//cout << endl;
	return buffer;
}

//------------------------ WP1_CONFIG section BEG ------------------------//
void commit_prepared_insert_WP1_CONFIG() {
	__TRY
	list<WP1ConfigBean>::iterator it = listConfigBean.begin();
	for (it = listConfigBean.begin(); it != listConfigBean.end(); it++) {
		//cout << (*it).getSqlInsert() << endl;
		lineCatalog = (*it).getLine();
		dataBaseManager->executeInsUpdDelStmt((*it).getSqlInsert());
	}
	dataBaseManager->Commit();
	__CATCH
}

void commit_prepared_update_WP1_CONFIG() {
	__TRY
	list<WP1ConfigBean>::iterator it = listConfigBean.begin();
	for (it = listConfigBean.begin(); it != listConfigBean.end(); it++) {
		//cout << (*it).getSqlUpdate() << endl;
		lineCatalog = (*it).getLine();
		dataBaseManager->executeInsUpdDelStmt((*it).getSqlUpdate());
	}
	dataBaseManager->Commit();
	__CATCH
}

void commit_deleted_WP1_CONFIG() {
	__TRY
	list<WP1ConfigBean>::iterator it = listConfigBean.begin();
	for (it = listConfigBean.begin(); it != listConfigBean.end(); it++) {
		lineCatalog = (*it).getLine();
		//cout << (*it).getSqlDelete() << endl;
		dataBaseManager->executeInsUpdDelStmt((*it).getSqlDelete());
	}
	dataBaseManager->Commit(); 
	__CATCH
}

void prepare_query_WP1_CONFIG(const char* fileName) {
	ifstream file(fileName); // declare file stream
	if (!file.fail()) {
	string value;
	list<string> values;
	//Jumping the first line of file
	getline(file,value);
	value.clear();
	while (file.good()) {
		getline(file, value, '#'); // read a string until next comma
		value = trimString(value); //MAN
		if (value.find('\n') != string::npos) {
			split_line(value, "\n", values);
		} else {
			values.push_back(value);
		}
	}
	checkSintattico(values.size(), NUM_FILELDS_WP1_CONFIG);	

	//Reading the ConfigBean
	list<string>::const_iterator it = values.begin();
	int active = 3; //NOT VALID VALUE
	int isMacro = 3; //NOT VALID VALUE
	for (it = values.begin(); it != values.end();) {
		string componentName = *it;
		it++;
		string paramName = *it;
		it++;
		string paramValue = *it;
		it++;
		string descrizione = *it;
		it++;
		if (isnumber((*it).c_str()) ) {
			active = atoi((*it).c_str());
		}
		it++;
		if (isnumber((*it).c_str()) ) {
			isMacro = atoi((*it).c_str());
		}
		it++;
		//Creating the ConfigBean
		WP1ConfigBean wp1ConfBean(componentName, paramName, paramValue,
				descrizione, active, isMacro);
		//Set the line to the ConfigBean
		wp1ConfBean.setLine(lineCatalog);
		//Check logico su bean
		wp1ConfBean.logicCheck(lineCatalog);
		//Preparing SQL INSERT
		wp1ConfBean.prepareSqlInsert();
		//Preparing SQL UPDATE
		wp1ConfBean.prepareSqlUpdate();
		//Preparing SQL DELETE
		wp1ConfBean.prepareSqlDelete();
		//Collecting the WP1ConfigBean
		listConfigBean.push_back(wp1ConfBean);
		lineCatalog++;
	}
	} else {
		cout << "The specified input file does NOT exist. Please select a valid file!!!" << endl;
	}
}

void viewWP1Config() {

	cout << "COMPONENT_NAME#PARAM_NAME#PARAM_VALUE#DESCR#ACTIVE#IS_MACRO" << endl;
	string queryConfig = "SELECT * FROM WP1_CONFIG ORDER BY COMPONENT_NAME, PARAM_NAME";
	__TRY
	//Create ResultSet for TICKET
	ResultSet *resultSetConfig = dataBaseManager->executeSelectStmt(queryConfig);

	while (resultSetConfig->next()) {
		WP1ConfigBean wp1ConfigBean(resultSetConfig->getString(1), resultSetConfig->getString(2), resultSetConfig->getString(3),
					resultSetConfig->getString(5), resultSetConfig->getInt(4), resultSetConfig->getInt(6));

		wp1ConfigBean.printWP1ConfigRow();
	}
    //Closing resultSetConfig ResultSet
	dataBaseManager->closeRecordset(resultSetConfig);
	__CATCH
}
//------------------------ WP1_CONFIG section END ------------------------//

//------------------------ WP1_SERVICES section BEG ------------------------//
void commit_prepared_insert_WP1_SERVICES() {
	__TRY
	list<ServiceBean>::iterator it = listServiceBean.begin();
	for (it = listServiceBean.begin(); it != listServiceBean.end(); it++) {
		lineCatalog = (*it).getLine();
		dataBaseManager->executeInsUpdDelStmt((*it).getSqlInsert());
	}
	dataBaseManager->Commit();
	__CATCH
}

void commit_prepared_update_WP1_SERVICES() {
	__TRY
	list<ServiceBean>::iterator it = listServiceBean.begin();
	for (it = listServiceBean.begin(); it != listServiceBean.end(); it++) {
		//cout << (*it).getSqlUpdate() << endl;
		lineCatalog = (*it).getLine();
		dataBaseManager->executeInsUpdDelStmt((*it).getSqlUpdate());
	}
	dataBaseManager->Commit();
	__CATCH
}

void commit_deleted_WP1_SERVICES() {
	__TRY
	list<ServiceBean>::iterator it = listServiceBean.begin();
	for (it = listServiceBean.begin(); it != listServiceBean.end(); it++) {
		//cout << (*it).getSqlDelete() << endl;
		lineCatalog = (*it).getLine();
        dataBaseManager->executeInsUpdDelStmt((*it).getSqlDelete());
	}
	dataBaseManager->Commit();
	__CATCH
}

void viewWP1Services() {

	cout << "ID_MACRO_SERVICE#DESCRIZIONE#ID_SERVICE" << endl;
	string queryServ = "SELECT * FROM WP1_SERVICES ORDER BY ID_SERVICE";

	__TRY
	//Create ResultSet for TICKET
	ResultSet *resultSetConfig = dataBaseManager->executeSelectStmt(queryServ);

	while (resultSetConfig->next()) {

		ServiceBean wp1Service(resultSetConfig->getString(3), resultSetConfig->getString(1), resultSetConfig->getString(2));

		wp1Service.printServiceRow();
    }
	//Closing resultSetConfig ResultSet
	dataBaseManager->closeRecordset(resultSetConfig);
	__CATCH
}

void prepare_query_WP1_SERVICES(const char* fileName) {
	ifstream file(fileName); // declare file stream
	if (!file.fail()) {
	string value;
	list<string> values;
	//Jumping the first line of file
	getline(file,value);
	value.clear();
	while (file.good()) {
		getline(file, value, '#'); // read a string until next comma
		value = trimString(value); //MAN
		if (value.find('\n') != string::npos) {
			split_line(value, "\n", values);
		} else {
			values.push_back(value);
		}
	}
	checkSintattico(values.size(), NUM_FILELDS_WP1_SERVICES);	

	//Reading the ServiceBean
	list<string>::const_iterator it = values.begin();
	for (it = values.begin(); it != values.end();) {
		string id_macro_service = *it;
		it++;
		string descrizione = *it;
		it++;
		string id_service = *it;
		it++;
		//Creating the ServiceBean
		ServiceBean wp1ServBean(id_macro_service, id_service, descrizione);
		//Preparing SQL INSERT
		wp1ServBean.prepareSqlInsert();
		//Preparing SQL UPDATE
		wp1ServBean.prepareSqlUpdate();
		//Set line of file to the bean
		//Preparing SQL DELETE
		wp1ServBean.prepareSqlDelete();
		//Set lineCatalog variable
		wp1ServBean.setLine(lineCatalog);
		//Collecting the WP1ConfigBean
		listServiceBean.push_back(wp1ServBean);
		//Increment lineCatalog
		lineCatalog++;
	}
	} else {
		cout << "The specified input file does NOT exist. Please select a valid file!!!" << endl;
	}
}
//------------------------ WP1_SERVICES section END ------------------------//

//------------------------ WP1_JOB section BEG ------------------------//
void commit_prepared_insert_WP1_JOB() {
	__TRY
	list<WP1JobBean>::iterator it = listJobBean.begin();
	for (it = listJobBean.begin(); it != listJobBean.end(); it++) {
		//cout << (*it).getSqlInsert() << endl;
		lineCatalog = (*it).getLine();
		dataBaseManager->executeInsUpdDelStmt((*it).getSqlInsert());
	}
	dataBaseManager->Commit(); 
	__CATCH
}

void commit_prepared_update_WP1_JOB() {
	__TRY
	list<WP1JobBean>::iterator it = listJobBean.begin();
	for (it = listJobBean.begin(); it != listJobBean.end(); it++) {
		//cout << (*it).getSqlUpdate() << endl;
		lineCatalog = (*it).getLine();
		dataBaseManager->executeInsUpdDelStmt((*it).getSqlUpdate());
	}
	dataBaseManager->Commit(); 
	__CATCH
}

void commit_deleted_WP1_JOB() {
	__TRY
	list<WP1JobBean>::iterator it = listJobBean.begin();
	for (it = listJobBean.begin(); it != listJobBean.end(); it++) {
		//cout << (*it).getSqlDelete() << endl;
		lineCatalog = (*it).getLine();
		dataBaseManager->executeInsUpdDelStmt((*it).getSqlDelete());
	}
	dataBaseManager->Commit(); 
	__CATCH
}

void prepare_query_WP1_JOB(const char* fileName) {
	ifstream file(fileName); // declare file stream
	if (!file.fail()) {
	string value;
	list<string> values;
	//Jumping the first line of file
	getline(file,value);
	value.clear();
	while (file.good()) {
		getline(file, value, '#'); // read a string until next comma
		value = trimString(value); //MAN
		if (value.find('\n') != string::npos) {
			split_line(value, "\n", values);
		} else {
			values.push_back(value);
		}
	}
	checkSintattico(values.size(), NUM_FILELDS_WP1_JOB);	
	//Reading the JobBean
	int active = 3; //NOT VALID VALUE
	list<string>::const_iterator it = values.begin();
	for (it = values.begin(); it != values.end();) {
		string idJob = *it;
		it++;
		string descrizione = *it;
		it++;
		if (isnumber((*it).c_str()) ) {
			active = atoi((*it).c_str());
		}
		it++;
		string jobType = *it;
		it++;
		string codeTipoContr = *it;
		if (codeTipoContr.compare("ALL") == 0) { //MOD.06
			codeTipoContr = "";
		}
		it++;
		string masterTable = *it;
		it++;
		//Creating the ServiceBean
		WP1JobBean wp1JobBean(idJob,descrizione, jobType, active, codeTipoContr, masterTable);
		//Check logico su bean
		wp1JobBean.logicCheck(lineCatalog);
		//Preparing SQL INSERT
		wp1JobBean.prepareSqlInsert();
		//Preparing SQL UPDATE
		wp1JobBean.prepareSqlUpdate();
		//Preparing SQL DELETE
		wp1JobBean.prepareSqlDelete();
		//Set lineCatalog variable
		wp1JobBean.setLine(lineCatalog);
		//Collecting the WP1ConfigBean
		listJobBean.push_back(wp1JobBean);
		//Increment lineCatalog variable
		lineCatalog++;
	}
	} else {
		cout << "The specified input file does NOT exist. Please select a valid file!!!" << endl;
	}
}

void viewJobs(string pattern) {
	cout << "ID_JOB#DESCRIZIONE#ACTIVE#JOB_TYPE#CODE_TIPO_CONTR#MASTERTABLE" << endl;
	string whereQueryJob = "";
	if (pattern.compare("") != 0) {
		pattern = trimString(pattern);
		whereQueryJob = " WHERE ID_JOB||DESCRIZIONE||JOB_TYPE||CODE_TIPO_CONTR||MASTERTABLE LIKE \'%"+pattern+"%\'";
	}
	string queryConfig = "SELECT * FROM WP1_JOB "+ whereQueryJob +" ORDER BY ID_JOB";

	__TRY
	//Create ResultSet for JOB
	ResultSet *resultSetConfig = dataBaseManager->executeSelectStmt(queryConfig);

	while (resultSetConfig->next()) {

		WP1JobBean wp1JobBean(resultSetConfig->getString(1), resultSetConfig->getString(2), resultSetConfig->getString(4),
					resultSetConfig->getInt(3), resultSetConfig->getString(5), resultSetConfig->getString(6));

		wp1JobBean.printJobRow();
	}
	//Closing resultSetConfig ResultSet
	dataBaseManager->closeRecordset(resultSetConfig);
	__CATCH

}
//------------------------ WP1_JOB section END ------------------------//

//------------------------ WP1_CONDITION section BEG ------------------------//
void commit_prepared_insert_WP1_CONDITION() {
	__TRY
	list<WP1ConditionBean>::iterator it = listCondBean.begin();
	for (it = listCondBean.begin(); it != listCondBean.end(); it++) {
		//cout << (*it).getSqlInsert() << endl;
		lineCatalog = (*it).getLine();
        dataBaseManager->executeInsUpdDelStmt((*it).getSqlInsert());
	}
	dataBaseManager->Commit(); 
	__CATCH
}

void commit_prepared_update_WP1_CONDITION() {
	__TRY
	list<WP1ConditionBean>::iterator it = listCondBean.begin();
	for (it = listCondBean.begin(); it != listCondBean.end(); it++) {
		//cout << (*it).getSqlUpdate() << endl;
		lineCatalog = (*it).getLine();
        dataBaseManager->executeInsUpdDelStmt((*it).getSqlUpdate());
	}
	dataBaseManager->Commit(); 
	__CATCH
}

void commit_deleted_WP1_CONDITION() {
	__TRY
	list<WP1ConditionBean>::iterator it = listCondBean.begin();
	for (it = listCondBean.begin(); it != listCondBean.end(); it++) {
		//cout << (*it).getSqlDelete() << endl;
		lineCatalog = (*it).getLine();
        dataBaseManager->executeInsUpdDelStmt((*it).getSqlDelete());
	}
    dataBaseManager->Commit(); 
	__CATCH_DEBUG
}

void viewCondition() {

	cout << "ID_CONDITION#FIELD#OPERATOR#OPERANDS#FIELD_TYPE#COND_TYPE" << endl;
	string queryCond = "SELECT * FROM WP1_CONDITIONS ORDER BY ID_CONDITION";

	__TRY
	//Create ResultSet for CONDITION
	ResultSet *resultSetConfig = dataBaseManager->executeSelectStmt(queryCond);

	while (resultSetConfig->next()) {
		WP1ConditionBean wp1ConditionBean(resultSetConfig->getString(1), resultSetConfig->getString(2), resultSetConfig->getString(3),
						  resultSetConfig->getString(4), resultSetConfig->getString(5), resultSetConfig->getString(6));

		wp1ConditionBean.printWP1CondRow();
	}
	//Closing resultSetConfig ResultSet
	dataBaseManager->closeRecordset(resultSetConfig);
	__CATCH
}

void prepare_query_WP1_CONDITION(const char* fileName) {
	ifstream file(fileName); // declare file stream
	if(!file.fail()) {
	string value;
	list<string> values;
	//Jumping the first line of file
	getline(file,value);
	value.clear();
	while (file.good()) {
		getline(file, value, '#'); // read a string until next comma
		value = trimString(value); //MAN
		if (value.find('\n') != string::npos) {
			split_line(value, "\n", values);
		} else {
			values.push_back(value);
		}
	}
	checkSintattico(values.size(), NUM_FILELDS_WP1_CONDITION);	
	//Reading the ConditionBean
	list<string>::const_iterator it = values.begin();
	for (it = values.begin(); it != values.end();) {
		string idCondition = *it;
		it++;
		string field = *it;
		it++;
		string operatore = *it;
		it++;
		string operands = *it;
		it++;
		string fieldType = *it;
		it++;
		string condType = *it;
		it++;
		//Creating the ServiceBean
		WP1ConditionBean wp1CondBean(idCondition,field,operatore,operands,fieldType,condType);
		//Preparing SQL INSERT
		wp1CondBean.prepareSqlInsert();
		//Preparing SQL UPDATE
		wp1CondBean.prepareSqlUpdate();
		//Preparing SQL DELETE
		wp1CondBean.prepareSqlDelete();
		//Set lineCatalog to the bean
		wp1CondBean.setLine(lineCatalog);
		//Collecting the WP1ConfigBean
		listCondBean.push_back(wp1CondBean);
		//Increment lineCatalog variable
		lineCatalog++;
	}
	} else {
		cout << "The specified input file does NOT exist. Please select a valid file!!!" << endl;
	}
}
//------------------------ WP1_CONDITION section END ------------------------//

//------------------------ WP1_OPERATION section BEG ------------------------//
void commit_prepared_insert_WP1_OPERATION() {
	__TRY
	list<WP1OperationBean>::iterator it = listOperBean.begin();
	for (it = listOperBean.begin(); it != listOperBean.end(); it++) {
		//cout << (*it).getSqlInsert() << endl;
		lineCatalog = (*it).getLine();
         dataBaseManager->executeInsUpdDelStmt((*it).getSqlInsert());
	}
	dataBaseManager->Commit(); 
	__CATCH
}

void commit_prepared_update_WP1_OPERATION() {
	__TRY
	list<WP1OperationBean>::iterator it = listOperBean.begin();
	for (it = listOperBean.begin(); it != listOperBean.end(); it++) {
		//cout << (*it).getSqlUpdate() << endl;
		lineCatalog = (*it).getLine();
        dataBaseManager->executeInsUpdDelStmt((*it).getSqlUpdate());
	}
	dataBaseManager->Commit(); 
	__CATCH
}

void commit_deleted_WP1_OPERATION() {
	__TRY
	list<WP1OperationBean>::iterator it = listOperBean.begin();
	for (it = listOperBean.begin(); it != listOperBean.end(); it++) {
		//cout << (*it).getSqlDelete() << endl;
		lineCatalog = (*it).getLine();
        dataBaseManager->executeInsUpdDelStmt((*it).getSqlDelete());
	}
	dataBaseManager->Commit(); 
	__CATCH
}

void viewWP1Operation() {

	cout << "ID_OPERATION#FIELD#OPERATOR#OPERAND#OP_TYPE" << endl;
	string queryOper = "SELECT * FROM WP1_OPERATIONS ORDER BY ID_OPERATION";

	__TRY
	//Create ResultSet for OPERATION
	ResultSet *resultSetConfig = dataBaseManager->executeSelectStmt(queryOper);

	while (resultSetConfig->next()) {
		WP1OperationBean wp1OperationBean(resultSetConfig->getString(1), resultSetConfig->getString(2), resultSetConfig->getString(3),
						  resultSetConfig->getString(4), resultSetConfig->getString(5));
		wp1OperationBean.printWP1OperationRow();
	}
	//Closing resultSetConfig ResultSet
	dataBaseManager->closeRecordset(resultSetConfig);
	__CATCH
}

void prepare_query_WP1_OPERATION(const char* fileName) {
	ifstream file(fileName); // declare file stream
	if (!file.fail()) {
	string value;
	list<string> values;
	//Jumping the first line of file
	getline(file,value);
	value.clear();
	while (file.good()) {
		getline(file, value, '#'); // read a string until next comma
		value = trimString(value); //MAN
		if (value.find('\n') != string::npos) {
			split_line(value, "\n", values);
		} else {
			values.push_back(value);
		}
	}
	checkSintattico(values.size(), NUM_FILELDS_WP1_OPERATION);	
	//Reading the OperationBean
	list<string>::const_iterator it = values.begin();
	for (it = values.begin(); it != values.end();) {
		string idOperation = *it;
		it++;
		string field = *it;
		it++;
		string operatore = *it;
		it++;
		string operand = *it;
		it++;
		string opType = *it;
		it++;
		//Creating the OperationBean
		WP1OperationBean wp1OperBean(idOperation,field,operatore,operand,opType);
		//Preparing SQL INSERT
		wp1OperBean.prepareSqlInsert();
		//Preparing SQL UPDATE
		wp1OperBean.prepareSqlUpdate();
		//Preparing SQL DELETE
		//Set lineCatalog to the bean
		wp1OperBean.setLine(lineCatalog);
		wp1OperBean.prepareSqlDelete();
		//Collecting the WP1ConfigBean
		listOperBean.push_back(wp1OperBean);
		//Increment lineCatalog variable
		lineCatalog++;
	}
	} else {
		cout << "The specified input file does NOT exist. Please select a valid file!!!" << endl;
	}
}
//------------------------ WP1_OPERATION section END ------------------------//

//------------------------ WP1_JOB_CONDITION_OPERATION section BEG ------------------------//
void commit_prepared_insert_WP1_JOB_CONDITION_OPERATION() {
	__TRY
	list<WP1JobConditionOperation>::iterator it = jobCondOpList.begin();
	for (it = jobCondOpList.begin(); it != jobCondOpList.end(); it++) {
		lineConfig = (*it).getLine();
		list<string> sqlInsertList =  (*it).getSqlInsertList();
		list<string>::iterator it = sqlInsertList.begin();
		for (it = sqlInsertList.begin(); it != sqlInsertList.end(); it++) {
			//cout << *it << endl;
            dataBaseManager->executeInsUpdDelStmt(*it);
		}
	}
	dataBaseManager->Commit();
	__CATCH_CONFIG
}

void commit_prepared_update_WP1_JOB_CONDITION_OPERATION() {
	__TRY
	list<WP1JobConditionOperation>::iterator it = jobCondOpList.begin();
	for (it = jobCondOpList.begin(); it != jobCondOpList.end(); it++) {
		lineConfig = (*it).getLine();
		list<string> sqlUpdateList =  (*it).getSqlUpdateList();
		list<string>::iterator it = sqlUpdateList.begin();
		for (it = sqlUpdateList.begin(); it != sqlUpdateList.end(); it++) {
			//cout << *it << endl;
            dataBaseManager->executeInsUpdDelStmt(*it);
		}
	}
	dataBaseManager->Commit(); 
	__CATCH_CONFIG
}

void commit_prepared_delete_WP1_JOB_CONDITION_OPERATION() {
	__TRY
	list<WP1JobConditionOperation>::iterator it = jobCondOpList.begin();
	for (it = jobCondOpList.begin(); it != jobCondOpList.end(); it++) {
		lineConfig = (*it).getLine();
		(*it).calculateSqlDelete();
		dataBaseManager->executeInsUpdDelStmt((*it).getSqlDeleteJobCondition()); //INSTANCE_EXEC
		dataBaseManager->executeInsUpdDelStmt((*it).getSqlDeleteJobOperation()); //INSTANCE_PARAMETERS
	}
    dataBaseManager->Commit(); 
	__CATCH_CONFIG
}


void prepare_query_WP1_JOB_CONDITION_OPERATION(const char* fileName) {
	ifstream file(fileName); // declare file stream
	if (!file.fail()) {
	string value;
	int line = 0;
	//DEBUG	list<string> values;
    //values.clear();
	list<string> operationList;
	list<string> conditionList;
	list<int> optionalList;
	WP1JobConditionOperation jobCondOp;
	int jobCompleted = 0; //Initially jobCompleted=0
	while (!file.eof()) {
		getline(file, value); // read a string until next comma
		line++;
			if (value.find("JOB_ROW") == 0) {
				checkRow(value, NUM_FIELDS_JOB_CONFIG_ROW - 1, line);
				if (jobCompleted != 0) {
					//Preparing INSERT
					jobCondOp.calculateSqlInsert();
					//Preparing UPDATE
					jobCondOp.calculateSqlUpdate();
					//Set the line of the file into the bean
					jobCondOp.setLine(lineConfig);
					//Logic check on the bean
					jobCondOp.logicCheck(lineConfig);
					jobCondOpList.push_back(jobCondOp);
					//clear object
					jobCondOp.clear();
					conditionList.clear();
					optionalList.clear();
					operationList.clear();
					jobCompleted = 0; //reset jobCompleted
				}
				//Saving the lineConfig of the JOB_ROW
				lineConfig = line;
				jobCompleted++; //increment jobCompleted
				vector<string> values; //DEBUG
				//split_line_delimiter(value, values);
				value = trimString(value); //MAN
				split(value, '#', values);
				vector<string>::const_iterator it; //MAN
				for (it = values.begin(); it != values.end();) {
					it++; //In order to jump the string "JOB_ROW"
					string jobType = *it;
					it++;
					string idJob = *it;
					it++;
					//Save the couple into the bean
					jobCondOp.setJobType(jobType);
					jobCondOp.setIdJob(idJob);
				}
				values.clear();
			} else if (value.find("CONDITION_ROW") == 0) {
				checkRow(value, NUM_FIELDS_JOB_CONDITION_ROW - 1, line);
	            vector<string> values;
				vector<string>::const_iterator it;
				//split_line_delimiter(value, values);
				value = trimString(value);
				split(value, '#', values);
				it = values.begin(); it++; //In order to jump the string "CONDITION_ROW"
				int optional = 3; //NOT ACTIVE VALUE
				for (; it != values.end();) {
					string idCondition = *it;
					it++;
					conditionList.push_back(idCondition);
					if (isnumber((*it).c_str())) {
						optional = atoi((*it).c_str());
					}
					it++;
					optionalList.push_back(optional);
				}
				//Save condId
				jobCondOp.setIdConditionList(conditionList);
				//Save optionalList
				jobCondOp.setOptionalList(optionalList);
				//Clearing the vector values
				values.clear();
			} else if (value.find("OPERATION_ROW") == 0) {
				checkRow(value, NUM_FIELDS_OPERATION_ROW - 1, line);
				vector<string> values; 
				vector<string>::const_iterator it;
				//split_line_delimiter(value, values);
				value = trimString(value);
                                split(value, '#', values);
				it = values.begin(); it++; //In order to jump the string "OPERATION_ROW"
				for (; it != values.end();) {
					string idOperation = *it;
					it++;
					operationList.push_back(idOperation);
				}
				//Save condId
				jobCondOp.setIdOperationList(operationList);
				//Clearing the vector values
				values.clear();
			}
	}
	if (jobCompleted != 0) {
		//Preparing INSERT
		jobCondOp.calculateSqlInsert();
		//Preparing UPDATE
		jobCondOp.calculateSqlUpdate();
		//Set the line of the file into the bean
		jobCondOp.setLine(lineConfig);
		//Logic check on the bean
		jobCondOp.logicCheck(lineConfig);
		jobCondOpList.push_back(jobCondOp);
		//clear object
		jobCondOp.clear();
		conditionList.clear();
		optionalList.clear();
		operationList.clear();
		jobCompleted = 0; //reset jobCompleted
	}
	} else {
		cout << "The specified input file does NOT exist. Please select a valid file!!!" << endl;
	}
}


void viewJobConfig(string pattern) {
	string whereQueryJob = "";
	if (pattern.compare("") != 0) {
		pattern = trimString(pattern);
		whereQueryJob = " WHERE ID_JOB||DESCRIZIONE||JOB_TYPE||CODE_TIPO_CONTR||MASTERTABLE LIKE \'%"+pattern+"%\'";
	}
	string queryJob = "SELECT * FROM WP1_JOB "+ whereQueryJob +" ORDER BY ID_JOB";
	string queryJobCondition = "SELECT * FROM WP1_JOB_CONDITION WHERE ID_JOB=\'";
	string queryJobOperation = "SELECT * FROM WP1_JOB_OPERATION WHERE ID_JOB=\'";
	string idCurrentJob = "";
	ResultSet *resultSetJobCondition = NULL;
	ResultSet *resultSetJobOperation = NULL;
	list<string> jobConditionIdList;
	list<int> optionalList;
	list<string> jobOperationIdList;

	__TRY
	//Create ResultSet for JOB
	ResultSet *resultSetJob = dataBaseManager->executeSelectStmt(queryJob);

	while (resultSetJob->next()) {

		idCurrentJob = resultSetJob->getString(1);
		WP1JobConditionOperation wp1JobConfigBean(idCurrentJob, resultSetJob->getString(1));
		//JOB_CONDITION
		resultSetJobCondition = dataBaseManager->executeSelectStmt(queryJobCondition+idCurrentJob+"\'");
		while (resultSetJobCondition->next()) {
			jobConditionIdList.push_back(resultSetJobCondition->getString(2));
			optionalList.push_back(resultSetJobCondition->getInt(3));

		}
		//Closing resultSetJobCondition ResultSet
		dataBaseManager->closeRecordset(resultSetJobCondition);
		//Saving the jobConditionIdList & optionalList
		wp1JobConfigBean.setIdConditionList(jobConditionIdList); 
		wp1JobConfigBean.setOptionalList(optionalList); 
		//Cleaning the jobConditionIdList & optionalList
		jobConditionIdList.clear();
		optionalList.clear();

		//JOB_OPERATION
		resultSetJobOperation = dataBaseManager->executeSelectStmt(queryJobOperation+idCurrentJob+"\'");
		while (resultSetJobOperation->next()) {
		jobOperationIdList.push_back(resultSetJobOperation->getString(2));

		}
		//Closing resultSetJobOperation ResultSet
		dataBaseManager->closeRecordset(resultSetJobOperation);
		//Saving the jobOperationIdList
		wp1JobConfigBean.setIdOperationList(jobOperationIdList); 
		//Cleaning the jobOperationIdList
		jobOperationIdList.clear();

		wp1JobConfigBean.printJobConfigRow();
	
	}
	//Closing resultSetJob ResultSet
	dataBaseManager->closeRecordset(resultSetJob);
	__CATCH_CONFIG
}
//------------------------ WP1_JOB_CONDITION_OPERATION section END ------------------------//

//------------------------ WP1_TICKET section BEG ------------------------//
void commit_prepared_insert_WP1_TICKET() {
	__TRY
	list<WP1TicketBean>::iterator it = ticketBeanList.begin();
	for (it = ticketBeanList.begin(); it != ticketBeanList.end(); it++) {
		//cout << (*it).getSqlInsertTicket() << endl;
		lineConfig = (*it).getLine();
		dataBaseManager->executeInsUpdDelStmt((*it).getSqlInsertTicket());
		//INSTANCE_EXEC
		list<string> sqlInsertInstanceExecList =  (*it).getSqlInsertInstanceExecList();
		list<string>::const_iterator itExec = sqlInsertInstanceExecList.begin();
		for (itExec = sqlInsertInstanceExecList.begin(); itExec != sqlInsertInstanceExecList.end(); itExec++) {
			lineConfig++; //Increment lineConfig
			//cout << (*itExec) << endl;
			dataBaseManager->executeInsUpdDelStmt(*itExec);
		}
		//INSTANCE_PARAMETERS
		list<string> sqlInsertInstanceParamList =  (*it).getSqlInsertInstanceParamList();
		list<string>::const_iterator itParam = sqlInsertInstanceExecList.begin();
		for (itParam = sqlInsertInstanceParamList.begin(); itParam != sqlInsertInstanceParamList.end(); itParam++) {
			lineConfig++; //Increment lineConfig
			//cout << (*itParam) << endl;
			dataBaseManager->executeInsUpdDelStmt(*itParam);
		}
	}
    dataBaseManager->Commit(); 
	__CATCH_CONFIG
}

void commit_prepared_delete_WP1_TICKET() {
	__TRY
	list<WP1TicketBean>::iterator it = ticketBeanList.begin();
	for (it = ticketBeanList.begin(); it != ticketBeanList.end(); it++) {
		(*it).calculateSqlDelete();
		lineConfig = (*it).getLine();
		dataBaseManager->executeInsUpdDelStmt((*it).getSqlDeleteInstanceExec()); //INSTANCE_EXEC
		dataBaseManager->executeInsUpdDelStmt((*it).getSqlDeleteInstanceParam()); //INSTANCE_PARAMETERS
		dataBaseManager->executeInsUpdDelStmt((*it).getSqlDeleteTicket()); //TICKET
	}
    dataBaseManager->Commit(); 
	__CATCH_CONFIG
}

void viewTickets(string pattern) {
		
		string whereQueryTicket = "";
		if (pattern.compare("") != 0) {
			pattern = trimString(pattern);
			whereQueryTicket = " WHERE ID_TICKET||TO_CHAR(DATA_EMISSIONE, 'YYYYMMDD')||TO_CHAR(DATA_INIZIO_VAL, 'YYYYMMDD')||TO_CHAR(DATA_FINE_VAL, 'YYYYMMDD')||USER_NWS||USER_GA||SYSTEM||DESCRIZIONE||TICKET_TYPE||CODE_TIPO_CONTR LIKE \'%"+pattern+"%\'";
		}

	   string queryTicket = "SELECT ID_TICKET,TO_CHAR(DATA_EMISSIONE, 'YYYYMMDD'), TO_CHAR(DATA_INIZIO_VAL, 'YYYYMMDD'),TO_CHAR(DATA_FINE_VAL, 'YYYYMMDD'),USER_NWS,USER_GA,SYSTEM,DESCRIZIONE,ORDINAMENTO,ACTIVE,TICKET_TYPE,CODE_TIPO_CONTR FROM WP1_TICKETGA "+ whereQueryTicket +" ORDER BY ID_TICKET";
	   string queryInstanceExec = "SELECT * FROM WP1_INSTANCE_EXEC WHERE ID_TICKET=\'";
	   string queryInstanceParam = "SELECT * FROM WP1_INSTANCE_PARAMETERS WHERE ";
	   string idCurrentTicket = "";
	   string idCurrentJob = "";
	   int instanceExecCounter = 0; //MAN DEBUG
	   
	   __TRY
       ResultSet *resultSetInstanceExec = NULL;
       ResultSet *resultSetInstanceParam = NULL;
       list<JobInstanceBean> jobInstanceBeanList;
       list<ParamBean> paramList;

       //Create ResultSet for TICKET
       ResultSet *resultSetTicket = dataBaseManager->executeSelectStmt(queryTicket);

       while (resultSetTicket->next()) {
           WP1TicketBean wp1TicketBean(resultSetTicket->getString(1),resultSetTicket->getString(2),resultSetTicket->getString(3),
					resultSetTicket->getString(4),resultSetTicket->getString(5),resultSetTicket->getString(6),resultSetTicket->getString(7),
					resultSetTicket->getString(8),resultSetTicket->getString(9),
					resultSetTicket->getString(10), resultSetTicket->getString(11), resultSetTicket->getString(12));
			if (wp1TicketBean.getCodeTipoContr().compare("") == 0) {
				wp1TicketBean.setCodeTipoContr("null");
			}
           //INSTANCE_EXEC
           idCurrentTicket = resultSetTicket->getString(1);
           resultSetInstanceExec = dataBaseManager->executeSelectStmt(queryInstanceExec+idCurrentTicket+"\'");
           while (resultSetInstanceExec->next()) {
              idCurrentJob = resultSetInstanceExec->getString(2);
			  instanceExecCounter = resultSetInstanceExec->getInt(3); //MAN DEBUG
              JobInstanceBean jobInstance( idCurrentJob );
              //INSTANCE_PARAMETERS
    		  char buffer[10];
			  sprintf(buffer, "%d", instanceExecCounter);
              resultSetInstanceParam = dataBaseManager->executeSelectStmt(queryInstanceParam+"ID_TICKET=\'"+idCurrentTicket+"\' AND ID_JOB =\'"+idCurrentJob+"\' AND COUNTER="+string(buffer)); //MAN DEBUG
              while (resultSetInstanceParam->next()) {
                 ParamBean param(resultSetInstanceParam->getString(1), resultSetInstanceParam->getString(4), resultSetInstanceParam->getInt(5));
                 paramList.push_back(param);
              }
              //Closing resultSetInstanceParam ResultSet
              dataBaseManager->closeRecordset(resultSetInstanceParam);
              //Saving the paramList              
              jobInstance.setParamList(paramList);
              //Cleaning the paramList
              paramList.clear();
              //Saving the JobInstanceBean into the list jobInstanceBeanList
              jobInstanceBeanList.push_back(jobInstance);
           }
           //Closing resultSetInstanceExec ResultSet
           dataBaseManager->closeRecordset(resultSetInstanceExec);
           //Saving the jobInstanceBeanList
           wp1TicketBean.setJobInstanceBeanList(jobInstanceBeanList);
           //Cleaning the jobInstanceBeanList
           jobInstanceBeanList.clear();

           wp1TicketBean.printTicketRow();
      }
      //Closing resultSetTicket ResultSet
      dataBaseManager->closeRecordset(resultSetTicket);
	  __CATCH_CONFIG
}

void prepare_query_WP1_TICKET(const char* fileName) {
	ifstream file(fileName); // declare file stream
	if (!file.fail()) {
	string value;
	list<string> values;
	int line = 0;

	WP1TicketBean wp1TicketBean;
	JobInstanceBean jobInstanceBean;
	ParamBean paramBean;
	list<JobInstanceBean> jobInstanceBeanList;
	list<ParamBean> paramBeanList;
	//WP1JobConditionOperation jobCondOp;
	int ticketCompleted = 0; //Initially ticketCompleted=0
	int jobCompleted = 0; //Initially ticketCompleted=0
	list<string>::const_iterator it; //MAN
	while (!file.eof()) {
		getline(file, value);
		line++;
			if (value.find("TICKET_ROW") == 0) {
				checkRow(value, NUM_FIELDS_TICKET_ROW - 1, line);
				if (ticketCompleted != 0) {
					//Aggiungo la lista dei parametri letta all'ultimo job letto
					jobInstanceBean.setParamList(paramBeanList);
					//Clear th paramBeanList
					paramBeanList.clear();
					//Aggiungo alla lista il jobInstanceBean corrente
					jobInstanceBeanList.push_back(jobInstanceBean);
					//Aggiungo al ticket la lista di JobInstanceBean letta
					wp1TicketBean.setJobInstanceBeanList(jobInstanceBeanList);
					//Preparing INSERT
					wp1TicketBean.calculateSqlInsert();
					//Preparing UPDATE
					wp1TicketBean.calculateSqlUpdate();
					//Set the line of the file into the bean
					wp1TicketBean.setLine(lineConfig);
					//Logic check of the bean
					wp1TicketBean.logicCheck(lineConfig);
					ticketBeanList.push_back(wp1TicketBean);
					//clear object
					wp1TicketBean.clear();
					jobInstanceBean.clear();
					paramBean.clear();
					//Clear the used list of bean
					jobInstanceBeanList.clear();

					ticketCompleted = 0; // reset ticketCompleted
					jobCompleted = 0; // reset ticketCompleted
				}
				//Saving the lineConfig of the JOB_ROW
				lineConfig = line;
				ticketCompleted++; //increment jobCompleted
	                        vector<string> values; //DEBUG
				//split_line_delimiter(value, values);
				value = trimString(value); //MAN
				split(value, '#', values);
				vector<string>::const_iterator it = values.begin();
				for (it = values.begin(); it != values.end();) {
					it++; //In order to jump the string "TICKET_ROW"
					string idTicket = *it;
					it++;
					string dataEmissione = *it;
					it++;
					string dataInizioVal = *it;
					it++;
					string dataFineVal = *it;
					it++;
					string userNws = *it;
					it++;
					string userGA = *it;
					it++;
					string system = *it;
					it++;
					string descrizione = *it;
					it++;
					string ordinamento = *it;
					it++;
					string active = *it;
					it++;
					string ticketType = *it;
					it++;
					string codeTipoContr = *it;
					it++;
					//Save the couple into the bean
					wp1TicketBean.setIdTicket(idTicket);
					wp1TicketBean.setDataEmissione(dataEmissione);
					wp1TicketBean.setDataInizioVal(dataInizioVal);
					wp1TicketBean.setDataFineVal(dataFineVal);
					wp1TicketBean.setUserNws(userNws);
					wp1TicketBean.setUserGa(userGA);
					wp1TicketBean.setSystem(system);
					wp1TicketBean.setDescrizione(descrizione);
					wp1TicketBean.setOrdinamento(ordinamento);
					wp1TicketBean.setActive(active);
					wp1TicketBean.setTicketType(ticketType);
					if ( codeTipoContr.compare("null") == 0) {
						wp1TicketBean.setCodeTipoContr("");
					} else {
						wp1TicketBean.setCodeTipoContr(codeTipoContr);
					}
				}
				values.clear();
			} else if (value.find("JOB_ROW") == 0) {
				checkRow(value, NUM_FIELDS_JOB_ROW - 1, line);
				if (jobCompleted != 0) {
					//Salvo la lista dei parametri del job nel JobInstanceBean
					jobInstanceBean.setParamList(paramBeanList);
					//Clear th paramBeanList
					paramBeanList.clear();
					//Aggiungo il jobInstanceBean alla lista che poi verra' aggunta al ticket
					jobInstanceBeanList.push_back(jobInstanceBean);
					//Resetto il JobInstanceBean
					jobInstanceBean.clear();
					jobCompleted = 0;  //Reset jobCompleted
				}
				jobCompleted++;
				vector<string> values; //DEBUG
				vector<string>::const_iterator it; //MAN
				//split_line_delimiter(value, values);
				value = trimString(value); //MAN
				split(value, '#', values);
				it = values.begin(); it++; //In order to jump the string "JOB_ROW"
				string idJob = *it;
				it++;
				jobInstanceBean.setIdJob(idJob);

				//Clearing the vector values
				values.clear();
			} else if (value.find("JOB_PARAM_ROW") == 0) {
				checkRow(value, NUM_FIELDS_JOB_PARAM_ROW - 1, line);
	                        vector<string> values; //DEBUG
				vector<string>::const_iterator it; //MAN
				//split_line_delimiter(value, values);
				value = trimString(value); //MAN
                                split(value, '#', values);
				it = values.begin(); it++; //In order to jump the string "JOB_PARAM_ROW"
				string nameParam = *it;
				it++;
				string valParam = *it;
				it++;
				int isMacro = 33; //NOT VALID VALUE
				if (isnumber((*it).c_str())) {
					isMacro = atoi((*it).c_str());
				}
				it++;
				paramBean.setNameParam(nameParam);
				paramBean.setValParam(valParam);
				paramBean.setIsMacro(isMacro);
				//Logic check of the bean
				paramBean.logicCheck(line);
				//ParamBeanList
				paramBeanList.push_back(paramBean);
				//Reset the paramBean object
				paramBean.clear();
				//Clearing the vector values
				values.clear();
			}
	}
	//Check last ticket
	if (ticketCompleted != 0) {
		//Salvo la lista dei parametri del job nel JobInstanceBean
		jobInstanceBean.setParamList(paramBeanList);
		//Clear th paramBeanList
		paramBeanList.clear();
		//Aggiungo alla lista il jobInstanceBean corrente
		jobInstanceBeanList.push_back(jobInstanceBean);
		//Aggiungo al ticket la lista di JobInstanceBean letta
		wp1TicketBean.setJobInstanceBeanList(jobInstanceBeanList);
		//Preparing INSERT
		wp1TicketBean.calculateSqlInsert();
		//Preparing UPDATE
		wp1TicketBean.calculateSqlUpdate();
		//Set the line of the file into the bean
		wp1TicketBean.setLine(lineConfig);
		//Logic check of the bean
		wp1TicketBean.logicCheck(lineConfig);
		ticketBeanList.push_back(wp1TicketBean);
		//clear object
		wp1TicketBean.clear();
		jobInstanceBean.clear();
		paramBean.clear();
		//Clear the used list of bean
		jobInstanceBeanList.clear();
		ticketCompleted = 0; // reset ticketCompleted
		jobCompleted = 0;  //reset jobCompleted
	}
	} else {
		cout << "The specified input file does NOT exist. Please select a valid file!!!" << endl;
	}
}
//------------------------ WP1_TICKET section END ------------------------//

//------------------------ WP1_ANALISI_TREND section BEG ------------------------//
void viewWP1AnalisiTrend(string pattern) {

	string wherePattern = "";
	if (pattern.compare("") != 0) {
		wherePattern = "WHERE ID_ELAB="+pattern;
	}

	cout << "ID_ELAB,CODE_ACCOUNT,IMPORTO_FT_CORRENTE,IMPORTO_NC_CORRENTE,MEDIA_FT,MEDIA_NC,PERC_DIFF_FAT,PERC_DIFF_NC,PERC_DIFF_TOT" << endl;
	string queryConfig = "SELECT * FROM WP1_ANALISI_TREND "+ wherePattern +" ORDER BY ID_ELAB";
	__TRY
	//Create ResultSet for TICKET
	ResultSet *resultSetConfig = dataBaseManager->executeSelectStmt(queryConfig);

	while (resultSetConfig->next()) {
	
		WP1AnalisiTrend wp1AnalisiTrend(resultSetConfig->getInt(1), resultSetConfig->getString(2),resultSetConfig->getInt(3),
										resultSetConfig->getInt(4), resultSetConfig->getInt(5),resultSetConfig->getInt(6),
										resultSetConfig->getInt(7),resultSetConfig->getInt(8),resultSetConfig->getInt(9));

		wp1AnalisiTrend.printWP1AnalisiTrendRow();
	}
    //Closing resultSetConfig ResultSet
	dataBaseManager->closeRecordset(resultSetConfig);
	__CATCH
}

//------------------------ WP1_ANALISI_TREND section END ------------------------//

//------------------------ WP1_LOG section BEG ------------------------//
void viewWP1Log(string pattern) {
	string wherePattern = "";
	if (pattern.compare("") != 0) {
		pattern = trimString(pattern);
		wherePattern = "WHERE TO_DATE(TO_CHAR(DATA_INS,'YYYYMMDD'), 'YYYYMMDD')=TO_DATE(\'"+pattern+"\',\'YYYYMMDD\')";
	}

	cout << "----------------------------------------------------------------------------------" << endl;
	string queryConfig = "SELECT * FROM WP1_LOG "+ wherePattern +" ORDER BY TS";
	__TRY
	//Create ResultSet for TICKET
	ResultSet *resultSetConfig = dataBaseManager->executeSelectStmt(queryConfig);

	while (resultSetConfig->next()) {
		string messageString = "";
		Clob messageClob = resultSetConfig->getClob(5);
		WP1Log wp1Log(resultSetConfig->getNumber(1), resultSetConfig->getNumber(2),resultSetConfig->getString(3),
					  resultSetConfig->getString(4),messageClob,resultSetConfig->getString(6));

		wp1Log.printWP1Log();
	}
    //Closing resultSetConfig ResultSet
	dataBaseManager->closeRecordset(resultSetConfig);
	__CATCH
}
//------------------------ WP1_LOG section BEG ------------------------//
//------------------------ WP1_TICKET_ELAB section BEG ------------------------//
void viewWP1TicketElab(string pattern) {
	string wherePattern = "";
	if (pattern.compare("") != 0) {
		pattern = trimString(pattern);
		wherePattern = "WHERE TO_DATE(TO_CHAR(DATA_MODIFICA,'YYYYMMDD'), 'YYYYMMDD')=TO_DATE(\'"+pattern+"\',\'YYYYMMDD\')";
	}
	string queryTicketElab = "SELECT * FROM WP1_TICKETGA_ELABORATION "+ wherePattern +" ORDER BY ID_TICKET";
	string queryTicketDett = "SELECT ID_ELABORAZIONE,ID_TICKET,ID_JOB,ID_CONDITION,ID_OPERATION,DESCRIZIONE,ESITO,CONTATORE,TO_CHAR(DATA_MODIFICA, 'YYYYMMDD') FROM WP1_TICKETGA_ELABORATION_DET WHERE ID_ELABORAZIONE = :1 AND ID_TICKET= :2 ORDER BY ID_ELABORAZIONE, ID_TICKET, ID_JOB, CONTATORE";
	long currentIdElaboration;
	string currIdTicket = "";
	__TRY
	//Create ResultSet for TICKET_ELAB
	ResultSet *resultSetTicketElab = dataBaseManager->executeSelectStmt(queryTicketElab);
	ResultSet *resultSetTicketElabDett = NULL;
	
	while (resultSetTicketElab->next()) {
		currentIdElaboration = resultSetTicketElab->getNumber(1);
		currIdTicket =  resultSetTicketElab->getString(2);
		cout << "**************************************** TESTATA **********************************" << endl;
		
		WP1TicketElab wp1TicketElab(currentIdElaboration,
									currIdTicket,
									resultSetTicketElab->getString(3),
									resultSetTicketElab->getString(4),
									resultSetTicketElab->getString(5));
							
		wp1TicketElab.printWP1TicketElab();
		
		Connection* conn =DB_Manager::Instance()->GetConnection();
		Statement* stm = conn->createStatement(queryTicketDett);
		stm->setNumber(1, currentIdElaboration);
		stm->setString(2, currIdTicket);
		resultSetTicketElabDett = stm->executeQuery();
		
		while (resultSetTicketElabDett->next()) {
			WP1TicketElabDett wp1TicketElabDett(resultSetTicketElabDett->getNumber(1),
									resultSetTicketElabDett->getString(2),
									resultSetTicketElabDett->getString(3),
									resultSetTicketElabDett->getString(4),
									resultSetTicketElabDett->getString(5),
									resultSetTicketElabDett->getString(6),
									resultSetTicketElabDett->getString(7),
									resultSetTicketElabDett->getInt(8),
									resultSetTicketElabDett->getString(9));
			wp1TicketElabDett.printWP1TicketElabDett();
		}
		conn->terminateStatement(stm);
	}
	__CATCH
}
//------------------------ WP1_TICKET_ELAB section END ------------------------//

