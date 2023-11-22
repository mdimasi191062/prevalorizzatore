/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package jribescommander.helpers;

import java.util.ArrayList;
import jribescommander.db.JRCContext;
import jribescommander.db.entity.CicloFatrz;
import jribescommander.db.entity.Contratti;
import jribescommander.db.entity.beans.Contratto;
import jribescommander.db.entity.beans.PeriodoRiferimento;

/**
 *
 * @author oracle
 */
public class HelperResocontoSap extends GenericHelper {

    private String codeTipoContr = null;
    boolean debug;

    public HelperResocontoSap(String[] args, boolean debug) {

        this.debug = debug;
        if (debug) {
            if (args.length != 1) {
                throw new RuntimeException("Lancio errato. Usage: jribescommander.sh ressap-info");
            }

            codeUtente = "";
            
        } else {
            init(args);
        }
    }

    public void init(String[] args) {

        if (args.length < 2) {
            throw new RuntimeException("Lancio errato. Usage: jribescommander.sh ressap utente [CODE_TIPO_CONTR]");
        }

        codeUtente = args[1];
        if (args.length == 3) {
            codeTipoContr = args[2];
        }

    }

    public String esegui() {
        //RESOCONTO_SAP$TL001633$*$-l  -x 1 7 8 9 10 11 12 13 17 18 19 20 21 23 35 36 37 38 39 41 42 43 44 
        String codeFunz = "RESOCONTO_SAP";
        String listContr = "";
        
        
        if (codeTipoContr == null) {
            ArrayList contratti = new Contratti().getTuttiContratti();
            if (contratti.isEmpty()) {
                throw new RuntimeException("Nessun servizio trovato a sistema!!");

            }

            for (int i = 0; i < contratti.size(); i++) {
                Contratto contratto = (Contratto) contratti.get(i);
                listContr += contratto.getCODE_TIPO_CONTR();
                if (i != contratti.size() - 1) {
                    listContr += " ";
                }
                JRCContext.getInstance().inf("SERVIZIO = " + contratto.getCODE_TIPO_CONTR() + " - " + contratto.getDESC_TIPO_CONTR());
                if (debug) {
                    System.out.println("SERVIZIO = " + contratto.getCODE_TIPO_CONTR() + " - " + contratto.getDESC_TIPO_CONTR());
                }
                
            }

        } else {
            listContr = codeTipoContr;
        }
       
        return codeFunz + "$" + codeUtente + "$*$-l  -x " + listContr;

    }

}
