/*
 * jpubwp1_job_config.cpp
 *
 *  Created on: Nov 4, 2014
 *      Author: amonteforte
 */
#include <iostream>
#include <sstream>
#include <fstream>
#include <string>
#include <cstdlib>
#include <list>
#include "JPUBWP1_Preloader.h"

using namespace std;


int main(int argc, char** argv) {

        setupDBConnection();

	if (argc >= 3) {
	//ADD, UPD, DEL
		string argv1_string = ucase(string(argv[1]));
		if (argv1_string == "ADD") {
			prepare_query_WP1_JOB_CONDITION_OPERATION(argv[2]);
			commit_prepared_insert_WP1_JOB_CONDITION_OPERATION();
		} else if (argv1_string == "UPD") {
			prepare_query_WP1_JOB_CONDITION_OPERATION(argv[2]);
			commit_prepared_delete_WP1_JOB_CONDITION_OPERATION();
			commit_prepared_insert_WP1_JOB_CONDITION_OPERATION();
		} else if (argv1_string == "DEL") {
			prepare_query_WP1_JOB_CONDITION_OPERATION(argv[2]);
			commit_prepared_delete_WP1_JOB_CONDITION_OPERATION();
		} else if (argv1_string == "VIEW") {
			if (argc == 3) {
				viewJobConfig(string(argv[2]));
			} else {
				printUsage(string(argv[0]));
			}
		}else {
			printUsage(string(argv[0]));
		}
	} else if (argc == 2) {
	//VIEW
		string argv1_string = ucase(string(argv[1]));
		if (argv1_string == "VIEW") {
            viewJobConfig();
		} else {
			printUsage(string(argv[0]));
		}
	} else {
		printUsage(string(argv[0]));
	}
}

