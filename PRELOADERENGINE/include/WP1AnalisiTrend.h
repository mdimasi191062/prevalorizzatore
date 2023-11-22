/*
 * WP1AnalisiTrend.h
 *
 *  Created on: Nov 28, 2014
 *      Author: amonteforte
 */

#ifndef WP1ANALISITREND_H_
#define WP1ANALISITREND_H_

#include "WP1Bean.h"

using namespace std;

class WP1AnalisiTrend : public WP1Bean {

private:
    int idElab;
    string codeAccount;
    int importoFtCorrente;
    int importNcCorrente;
    int mediaFt;
    int mediaNc;
    int percDiffFat;
    int percDiffNc;
    int percDiffTot;
    
    string analisiTrendRow;

public:
    WP1AnalisiTrend(int idElab,
            string codeAccount,
            int importoFtCorrente,
            int importNcCorrente,
            int mediaFt,
            int mediaNc,
            int percDiffFat,
            int percDiffNc,
            int percDiffTot);

    WP1AnalisiTrend();

    virtual ~WP1AnalisiTrend();

    void printWP1AnalisiTrendRow();
};

#endif /* WP1ANALISITREND_H_ */
