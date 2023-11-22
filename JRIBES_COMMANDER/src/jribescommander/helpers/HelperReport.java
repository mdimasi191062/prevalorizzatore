/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package jribescommander.helpers;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import jribescommander.db.JRCContext;
import jribescommander.db.entity.Accounts;
import jribescommander.db.entity.CicloFatrz;
import jribescommander.db.entity.ElaborBatch;
import jribescommander.db.entity.beans.Account;
import jribescommander.db.entity.beans.BatchElem;
import jribescommander.db.entity.beans.PeriodoRiferimento;

/**
 *
 * @author oracle
 */
public class HelperReport extends GenericHelper {

    private ArrayList codeAccount = new ArrayList();
    private String tipo;
    boolean debug;

    public HelperReport(String[] args, boolean debug) {

        this.debug = debug;
        if (debug) {
            if (args.length != 3) {
                throw new RuntimeException("Lancio errato. Usage: jribescommander.sh csv-info code_tipo_contr tipo(V/R)");
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
            throw new RuntimeException("Lancio errato. Usage: jribescommander.sh csv code_tipo_contr tipo(V/R) utente [CODE_ACCOUNT_1] [CODE_ACCOUNT_2] ....");
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
        for (int i = 4; i < args.length; i++) {
            codeAccount.add(args[i]);
        }

    }

    public String esegui() {
        controlloAltreElaborazioni();
        boolean repricing = tipo.equals("R");
        String comboFunzSelez = repricing ? "26" : "21";
        BatchElem elem = new ElaborBatch().getCodeFunzFlag(codeTipoContr, "RP", comboFunzSelez);
        String codeFunz = elem.getCodeFunz();

        PeriodoRiferimento periodoRifermento = new CicloFatrz().findPeriodoRiferimentoReport(codeTipoContr, repricing);

        JRCContext.getInstance().inf("CICLO DI RIFERIMENTO = " + periodoRifermento.getDescOf());
        if (debug) {
            System.out.println("CICLO DI RIFERIMENTO = " + periodoRifermento.getDescOf());
        }

        ArrayList cli = new ArrayList();
        cli.addAll(new Accounts().getAccountPerReport(periodoRifermento.getDescOf().substring(0, 10), periodoRifermento.getDescOf().substring(13), codeTipoContr, comboFunzSelez, repricing));
        if (!codeAccount.isEmpty()) {
            ArrayList subList = new ArrayList();
            for (int j = 0; j < codeAccount.size(); j++) {
                boolean trovato = false;
                for (int i = 0; i < cli.size(); i++) {
                    Account a = (Account) cli.get(i);

                    if (codeAccount.get(j).equals(a.getAccount())) {
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
        String datiBatch = periodoRifermento.getDescOf().substring(6, 10) + periodoRifermento.getDescOf().substring(3, 5) + periodoRifermento.getDescOf().substring(0, 2);
        String account_list = "";
         if (cli.isEmpty()) {
               throw new RuntimeException("Nessun account per cui lanciare i report!");
         }
         
    
        for (int i = 0; i < cli.size(); i++) {
            Account account = (Account) (cli.get(i));

            JRCContext.getInstance().inf("ACCOUNT DA LANCIARE = " + account.getAccount() + " " + account.getDesc() );
            if (debug) {
                System.out.println("ACCOUNT DA LANCIARE = " + account.getAccount() + " " + account.getDesc() );
            }

            account_list += account.getAccount();
            if (i != cli.size() - 1) {
                account_list += "$";
            }

        }
       
        return codeFunz + "$" + codeUtente + "$" + datiBatch + "$" + tipo + "$" + account_list;

    }


}
