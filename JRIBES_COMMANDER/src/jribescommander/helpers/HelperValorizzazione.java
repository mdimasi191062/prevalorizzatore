/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package jribescommander.helpers;

import java.text.ParseException;
import java.util.Date;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.List;
import java.util.TimeZone;
import jribescommander.db.JRCContext;
import jribescommander.db.entity.Accounts;
import jribescommander.db.entity.CicloFatrz;
import jribescommander.db.entity.ElaborBatch;
import jribescommander.db.entity.beans.Account;
import jribescommander.db.entity.beans.PeriodoRiferimento;

/**
 *
 * @author oracle
 */
public class HelperValorizzazione extends GenericHelper{

    private String soloPopolamento;
    private String soloScarti;
    private String generaReport;
    private Date dataFinePeriodo; // 

    private ArrayList codeAccount = new ArrayList();
    private boolean debug;

    public HelperValorizzazione(String[] args, boolean debug) {
        
        this.debug = debug;
        if (debug) {
            if (args.length != 2) {
                throw new RuntimeException("Lancio errato. Usage: jribescommander.sh val-info code_tipo_contr");
            }
            codeTipoContr = args[1];
            soloPopolamento = "S";
            soloScarti = "0";
            generaReport = "0";
            dataFinePeriodo = null;
            codeUtente = "";

        } else {
            init(args);
        }
    }

    public void init(String[] args) {

        if (args.length < 7) {
            throw new RuntimeException("Lancio errato. Usage: jribescommander.sh val code_tipo_contr soloPopolamento(S/N) soloScarti(S/N) generaReport(S/N)  dataFinePeriodo(yyyymmdd) utente [CODE_ACCOUNT_1] [CODE_ACCOUNT_2] ....");
        }

        codeTipoContr = args[1];

        if (args[2].equals("S")) {
            soloPopolamento = "S";
        } else if (args[2].equals("N")) {
            soloPopolamento = "N";
        } else {
            throw new RuntimeException("Il parametro solo popolamento può essere S o N!");
        }

        if (args[3].equals("S")) {
            soloScarti = "1";
        } else if (args[3].equals("N")) {
            soloScarti = "0";
        } else {
            throw new RuntimeException("Il parametro solo scarti può essere S o N!");
        }

        if (args[4].equals("S")) {
            generaReport = "1";
        } else if (args[4].equals("N")) {
            generaReport = "0";
        } else {
            throw new RuntimeException("Il parametro genera report può essere S o N!");
        }

        
        try {
            dataFinePeriodo = sdfEng.parse(args[5]);
        } catch (ParseException ex) {
            throw new RuntimeException("Errore nel parsing della data fine periodo di fatturazione!");
        }
        if (dataFinePeriodo.after(new Date())) {
            throw new RuntimeException("La data fine periodo deve essere minore o uguale alla data di sistema");
        }
        codeUtente = args[6];
        for (int i = 7; i < args.length; i++) {
            codeAccount.add(args[i]);
        }

    }

    public String esegui() {

        controlloAltreElaborazioni();
        PeriodoRiferimento periodoRifermento = new CicloFatrz().findPerCicloFat("1", codeTipoContr);

        JRCContext.getInstance().inf("CICLO DI RIFERIMENTO = " + periodoRifermento.getDataIniCiclo() + " - " + periodoRifermento.getDataFineCiclo());
        if (debug) {
            System.out.println("CICLO DI RIFERIMENTO = " + periodoRifermento.getDataIniCiclo() + " - " + periodoRifermento.getDataFineCiclo());
        }
        ArrayList cli = new ArrayList();
        cli.addAll(new Accounts().getAccountXValorizzazione("1", periodoRifermento.getDataIniCiclo(), codeTipoContr, false));
        cli.addAll(new Accounts().getAccountXValorizzazione("1", periodoRifermento.getDataIniCiclo(), codeTipoContr, true));

        Date dataFineCicloFatrz = null;
        try {
            dataFineCicloFatrz = sdf.parse(periodoRifermento.getDataFineCiclo());
        } catch (ParseException ex) {
            throw new RuntimeException("Errore nel parsing della data fine ciclo di fatturazione!");
        }
        if (!debug) {
            if (dataFinePeriodo.after(dataFineCicloFatrz)) {
                throw new RuntimeException("La data fine periodo deve essere minore o uguale alla data fine ciclo di fatturazione!");
            }

            if (cli.isEmpty()) {
                throw new RuntimeException("Impossibile lanciare la valorizzazione. Nessun account valorizzabile per il code tipo contr impostato!");
            }
        }

        if (!codeAccount.isEmpty()) {
            ArrayList subList = new ArrayList();
            for (int j = 0; j < codeAccount.size(); j++) {
                boolean trovato=false;
                for (int i = 0; i < cli.size(); i++) {
                    Account a = (Account) cli.get(i);

                    if (codeAccount.get(j).equals(a.getAccount())) {
                        subList.add(a);
                        trovato=true;
                        break; // se lo trovo passo al prossimo
                    }
                    
                }
                // se non trovo un account segnalato tra i valorizzabili esco.
                if(!trovato)
                {
                    throw new RuntimeException("Impossibile trovare l'account "+codeAccount.get(j)+" tra gli account da valorizzare!");
                }
                
            }
            cli=subList;

        }

        String datiBatch = "";
        for (int i = 0; i < cli.size(); i++) {
            Account account = (Account) (cli.get(i));
            String data = account.getDataIniPerFatt();
            Date d;
            try {
                d = sdf.parse(data);
            } catch (ParseException ex) {
                throw new RuntimeException("Errore nel parsing della data fine dell'account " + account.getAccount());
            }

            JRCContext.getInstance().inf("ACCOUNT DA VALORIZZARE = " + account.getAccount() + " " + account.getDesc() + " - DATA INIZIO: " + account.getDataIniPerFatt());
            if (debug) {
                System.out.println("ACCOUNT DA VALORIZZARE = " + account.getAccount() + " " + account.getDesc() + " - DATA INIZIO: " + account.getDataIniPerFatt());
            }
            if (!debug && dataFinePeriodo.before(d)) {
                throw new RuntimeException("la data fine periodo deve essere strettamente maggiore della data di inizio periodo maggiore tra quelle degli account da valorizzare");
            }
            new Accounts().setCodeParam(account);
            String dataAAAAMMGG = "";
         

            if (account.getCodeParam() != null) {
                JRCContext.getInstance().inf("\t Code Param= " + account.getCodeParam());
                if (debug) {
                    System.out.println("\t Code Param= " + account.getCodeParam());
                }
                if (soloScarti.equals("1") || debug) {
                    dataAAAAMMGG = "00000000";
                } else {
                    dataAAAAMMGG = sdfEng.format(dataFinePeriodo);
                }
                datiBatch += dataAAAAMMGG + account.getCodeParam();
                if (i != (cli.size() - 1)) {
                    datiBatch += "$";
                }

            } else {
                throw new RuntimeException("Impossibile recuperare il code param per l'account!");
            }

        }

        return "21$" + codeUtente + "$" + generaReport + "$_$" + soloPopolamento + "$" + soloScarti + "$" + datiBatch;

    }

    

    public void getPeriodoRiferimento() {

    }
}
