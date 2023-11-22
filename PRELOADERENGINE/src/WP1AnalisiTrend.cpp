/*
 * WP1AnalisiTrend.cpp
 *
 *  Created on: Nov 28, 2014
 *      Author: amonteforte
 */

#include "WP1AnalisiTrend.h"



WP1AnalisiTrend::WP1AnalisiTrend() {
}

WP1AnalisiTrend::WP1AnalisiTrend(int idElab,
        string codeAccount,
        int importoFtCorrente,
        int importNcCorrente,
        int mediaFt,
        int mediaNc,
        int percDiffFat,
        int percDiffNc,
        int percDiffTot) : WP1Bean() {
    this->idElab = idElab;
    this->codeAccount = codeAccount;
    this->importoFtCorrente = importoFtCorrente;
    this->importNcCorrente = importNcCorrente;
    this->mediaFt = mediaFt;
    this->mediaNc = mediaNc;
    this->percDiffFat = percDiffFat;
    this->percDiffNc = percDiffNc;
    this->percDiffTot = percDiffTot;
    
    this->analisiTrendRow = "";
}

WP1AnalisiTrend::~WP1AnalisiTrend() {}

void WP1AnalisiTrend::printWP1AnalisiTrendRow() {
    char buffer1[10];
    sprintf(buffer1, "%d", this->idElab);
    char buffer3[10];
    sprintf(buffer3, "%d", this->importoFtCorrente);
    char buffer4[10];
    sprintf(buffer4, "%d", this->importNcCorrente);
    char buffer5[10];
    sprintf(buffer5, "%d", this->mediaFt);
    char buffer6[10];
    sprintf(buffer6, "%d", this->mediaNc);
    char buffer7[10];
    sprintf(buffer7, "%d", this->percDiffFat);
    char buffer8[10];
    sprintf(buffer8, "%d", this->percDiffNc);
    char buffer9[10];
    sprintf(buffer9, "%d", this->percDiffTot);
    
    this->analisiTrendRow = ""+
    string(buffer1) + "," +
    this->codeAccount + "," +
    string(buffer3) + "," +
    string(buffer4) + "," +
    string(buffer5) + "," +
    string(buffer6) + "," +
    string(buffer7) + "," +
    string(buffer8) + "," +
    string(buffer9);

    cout << this->analisiTrendRow << endl;
}
