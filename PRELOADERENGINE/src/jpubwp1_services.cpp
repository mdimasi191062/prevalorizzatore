/*
 * jpubwp1_services.cpp
 *
 *  Created on: Nov 3, 2014
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

	if (argc == 3) {
		//ADD, UPD, DEL
		string argv1_string = ucase(string(argv[1]));
		if (argv1_string == "ADD") {
			prepare_query_WP1_SERVICES(argv[2]);
			commit_prepared_insert_WP1_SERVICES();
		} else if (argv1_string == "UPD") {
			prepare_query_WP1_SERVICES(argv[2]);
			commit_prepared_update_WP1_SERVICES();
		} else if (argv1_string == "DEL") {
			prepare_query_WP1_SERVICES(argv[2]);
			commit_deleted_WP1_SERVICES();
		} else {
			printUsage(string(argv[0]));
		}
	} else if (argc == 2) {
		//VIEW
		string argv1_string = ucase(string(argv[1]));
		if (argv1_string == "VIEW") {
            viewWP1Services();
		} else {
			printUsage(string(argv[0]));
		}
	} else {
		printUsage(string(argv[0]));
	}
}

