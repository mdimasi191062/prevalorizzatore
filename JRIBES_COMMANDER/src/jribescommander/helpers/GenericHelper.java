/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package jribescommander.helpers;

import java.text.SimpleDateFormat;
import jribescommander.db.entity.ElaborBatch;

/**
 *
 * @author oracle
 */
public abstract class GenericHelper {
    protected String codeUtente;
    protected String codeTipoContr;
    protected SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy");
    protected SimpleDateFormat sdfEng = new SimpleDateFormat("yyyyMMdd");
    public void controlloAltreElaborazioni() {

        int numElabInCorsoAcc = new ElaborBatch().elabBatchCodeTipoContrUguali(codeTipoContr);
        if (numElabInCorsoAcc != 0) //CASO 0
        {
            throw new RuntimeException("Errore. Ci sono elaborazioni corso. Annullo il lancio");
        }

    }
    public abstract String esegui();
}
