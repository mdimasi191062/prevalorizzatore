/*
 * jpubwp1_ticket_elab.cpp
 *
 *  Created on: Dec 05, 2014
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
    if (argc == 2) {
        //VIEW SELECTIVE
        string argv1_string = ucase(string(argv[1]));
        if (argv1_string.compare("") != 0) {
            viewWP1TicketElab(argv1_string);
        }
    } else {
        printUsage(string(argv[0]));
    }
}

