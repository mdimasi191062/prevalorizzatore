/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package jribescommander.helpers;

import java.util.ArrayList;
import jribescommander.db.JRCContext;
import jribescommander.db.entity.Accounts;
import jribescommander.db.entity.beans.AccountElab;

/**
 *
 * @author oracle
 */
public class HelperRepricing extends GenericHelper {
private ArrayList codeAccount = new ArrayList();
    private String generaReport;
    boolean debug;

    public HelperRepricing(String[] args, boolean debug) {

        this.debug = debug;
        if (debug) {
            if (args.length != 2) {
                throw new RuntimeException("Lancio errato. Usage: jribescommander.sh repr-info code_tipo_contr");
            }
            codeTipoContr = args[1];
            generaReport="S";
            codeUtente = "";

        } else {
            init(args);
        }
    }

    public void init(String[] args) {

        if (args.length < 4) {
            throw new RuntimeException("Lancio errato. Usage: jribescommander.sh repr code_tipo_contr generaReport(S/N) utente [CODE_ACCOUNT_1] [CODE_ACCOUNT_2] ....");
        }

        codeTipoContr = args[1];

        if (args[2].equals("S")) {
            generaReport = "1";
        } else if (args[2].equals("N")) {
            generaReport = "0";
        } else {
            throw new RuntimeException("Il parametro genera report può essere S o N!");
        }

        codeUtente = args[3];
        for (int i = 4; i < args.length; i++) {
            codeAccount.add(args[i]);
        }

    }

    public String esegui() {
        controlloAltreElaborazioni();
        
        String codeFunz= "26";
        boolean isOk=new Accounts().controllaAccountRepricing(codeTipoContr);
        if(!isOk)
            throw new RuntimeException("Impossibile lanciare il repricing ! Ci sono testate non congelate per il servizio "+codeTipoContr);
        
        ArrayList cli = new ArrayList();
        cli.addAll(new Accounts().getAccountXRepricing(codeTipoContr));
        
        if (!codeAccount.isEmpty()) {
            ArrayList subList = new ArrayList();
            for (int j = 0; j < codeAccount.size(); j++) {
                boolean trovato = false;
                for (int i = 0; i < cli.size(); i++) {
                    AccountElab a = (AccountElab) cli.get(i);

                    if (codeAccount.get(j).equals(a.getCodeAccount())) {
                        subList.add(a);
                        trovato = true;
                        break; // se lo trovo passo al prossimo
                    }

                }
                // se non trovo un account segnalato tra i valorizzabili esco.
                if (!trovato) {
                    throw new RuntimeException("Impossibile trovare l'account " + codeAccount.get(j) + " tra gli account da lanciare!");
                }

            }
            cli = subList;
        }
        
        if (cli.isEmpty()) {
               throw new RuntimeException("Nessun account per cui lanciare il repricing!");
        }
         
        String account_list="";
        for (int i = 0; i < cli.size(); i++) {
            AccountElab account = (AccountElab) (cli.get(i));

            JRCContext.getInstance().inf("ACCOUNT DA LANCIARE = " + account.getCodeAccount()+ " " + account.getAccount());
            if (debug) {
                System.out.println("ACCOUNT DA LANCIARE = " + account.getCodeAccount() + " " + account.getAccount() );
            }

            account_list += account.getAccount();
            if (i != cli.size() - 1) {
                account_list += "$";
            }

        }
       
        return codeFunz + "$" + codeUtente + "$" + generaReport+ "$" + account_list;

    }

}
