/*
 * jpubwp1_trend_analysis.cpp
 *
 *  Created on: Nov 28, 2014
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
            viewWP1AnalisiTrend(argv1_string);
        }
    } else if (argc == 1 ) {
        //VIEW STANDARD
        viewWP1AnalisiTrend();
    } else {
        printUsage(string(argv[0]));
    }
}

