/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package jribescommander.helpers;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import jribescommander.db.JRCContext;
import jribescommander.db.entity.Accounts;
import jribescommander.db.entity.Csvs;
import jribescommander.db.entity.beans.AccountElab;

/**
 *
 * @author oracle
 */
public class HelperCongelamento extends GenericHelper {

    private String tipo;

    boolean debug;

    public HelperCongelamento(String[] args, boolean debug) {

        this.debug = debug;
        if (debug) {
            if (args.length != 3) {
                throw new RuntimeException("Lancio errato. Usage: jribescommander.sh cong-info code_tipo_contr tipo(V/R)");
            }
            codeTipoContr = args[1];
            if (args[2].equals("R")) {
                tipo = "R";
            } else if (args[2].equals("V")) {
                tipo = "V";
            } else {
                throw new RuntimeException("Il parametro tipo può essere V (Valorizzazione) o R(Repricing)!");
            }
            codeUtente = "";

        } else {
            init(args);
        }
    }

    public void init(String[] args) {

        if (args.length < 4) {
            throw new RuntimeException("Lancio errato. Usage: jribescommander.sh cong code_tipo_contr tipo(V/R) utente ");
        }

        codeTipoContr = args[1];

        if (args[2].equals("R")) {
            tipo = "R";
        } else if (args[2].equals("V")) {
            tipo = "V";
        } else {
            throw new RuntimeException("Il parametro tipo può essere V (Valorizzazione) o R(Repricing)!");
        }

        codeUtente = args[3];

    }

    public String esegui() {
        controlloAltreElaborazioni();
        boolean repricing = tipo.equals("R");
        String codeFunz = repricing ? "27" : "1006";

        ArrayList cli = new ArrayList();
        cli.addAll(new Accounts().getAccountXCong(codeTipoContr));

        if (cli.isEmpty()) {
            throw new RuntimeException("Nessun account per cui lanciare il congelamento!");
        }

        String account_list = "";
        for (int i = 0; i < cli.size(); i++) {
            AccountElab account = (AccountElab) (cli.get(i));
            JRCContext.getInstance().inf("ACCOUNT DA LANCIARE = " + account.getCodeAccount()+" con DATA INIZIO ="+account.getDataIni());
            if (debug) {
                System.out.println("ACCOUNT DA LANCIARE = " + account.getCodeAccount()+" con DATA INIZIO ="+account.getDataIni());
            }
            int nCsv = new Csvs().findCsvXDSL(account.getCodeAccount(), account.getDataIni()).getCsv();
            if (nCsv == 0) {
                throw new RuntimeException("Impossibile lanciare il congelamento. L'account " + account.getCodeAccount() + " non ha valorizzato i csv!");
            }

           

            account_list += account.getCodeParam();
            if (i != cli.size() - 1) {
                account_list += "$";
            }

        }

        return codeFunz + "$" + codeUtente + "$" + account_list;

    }

}
