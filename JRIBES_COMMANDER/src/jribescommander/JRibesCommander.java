/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package jribescommander;

import java.util.TimeZone;
import jribescommander.db.JRCContext;
import jribescommander.helpers.HelperCongelamento;
import jribescommander.helpers.HelperReport;
import jribescommander.helpers.HelperRepricing;
import jribescommander.helpers.HelperResocontoSap;
import jribescommander.helpers.HelperSap;
import jribescommander.helpers.HelperValorizzazione;

/**
 *
 * @author oracle
 */
public class JRibesCommander {

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        //codeFunzBatch+"$"+codUtente+"$"+generaReport(0/1)+"$_$"+soloPopolamento(S/N)+"$"+soloScarti(0/1)+"$"+datiBatch (LIST dataAAAAMMGGcodeParam)
        // 21$UE014334$0$_$S$0$
        TimeZone.setDefault(TimeZone.getTimeZone("Europe/Rome"));
        JRCContext c = JRCContext.getInstance();
        String command=null;
        String params=null;
        try {
            if (args.length < 1) {
                System.err.println("Parametri errati nel lancio.");
                System.err.println("Es. java -jar JRibesCommander.jar TIPOLOGIA_LANCIO PARAMS");
                System.err.println("Consultare il manuale per le tipologie di lancio e per i parametri di ogni singola tipologia");
                System.out.println("Exit code [2]");
                System.exit(2);
            }
            if(args.length>2)
                params = args[1];
            command = args[0];
            c.init(command);
            c.openCon();
        } catch (Throwable ex) {
            System.err.println("Impossibile inizializzare JRibesCommander. Esecuzione Abortita");
            ex.printStackTrace(System.err);
            System.out.println("Exit code [1]");
            System.exit(1);
        }
        try {
            LaunchJRibes jribesLauncher = new LaunchJRibes();
            
            stampaParametri(args);
            

            if (command.compareTo("val") == 0) {
                JRCContext.getInstance().inf("LANCIO DELLA VALORIZZAZIONE ATTIVA...");

                params = new HelperValorizzazione(args, false).esegui();
                command = "jribes";
            }
            if (command.compareTo("val-info") == 0) {
                params = new HelperValorizzazione(args, true).esegui();
                JRCContext.getInstance().inf("STRINGA DA INVIARE A JRIBES = " + params);
                System.exit(0);
            }
            if (command.compareTo("csv") == 0) {
                JRCContext.getInstance().inf("LANCIO DELLA VALORIZZAZIONE ATTIVA...");

                params = new HelperReport(args, false).esegui();
                command = "jribes";
            }
            if (command.compareTo("csv-info") == 0) {
                params = new HelperReport(args, true).esegui();
                JRCContext.getInstance().inf("STRINGA DA INVIARE A JRIBES = " + params);
                System.exit(0);
            }
            if (command.compareTo("sap") == 0) {
                JRCContext.getInstance().inf("LANCIO SAP...");

                params = new HelperSap(args, false).esegui();
                command = "jribes";
            }
            if (command.compareTo("sap-info") == 0) {
                params = new HelperSap(args, true).esegui();
                JRCContext.getInstance().inf("STRINGA DA INVIARE A JRIBES = " + params);
                System.exit(0);
            }
            if (command.compareTo("cong") == 0) {
               JRCContext.getInstance().inf("LANCIO DELLA VALORIZZAZIONE ATTIVA...");

                params = new HelperCongelamento(args, false).esegui();
                command = "jribes";
            }
            if (command.compareTo("cong-info") == 0) {
                params = new HelperCongelamento(args, true).esegui();
                JRCContext.getInstance().inf("STRINGA DA INVIARE A JRIBES = " + params);
                System.exit(0);
            }
            if (command.compareTo("repr") == 0) {
               JRCContext.getInstance().inf("LANCIO DELLA VALORIZZAZIONE ATTIVA...");

                params = new HelperRepricing(args, false).esegui();
                command = "jribes";
            }
            if (command.compareTo("repr-info") == 0) {
                params = new HelperRepricing(args, true).esegui();
                JRCContext.getInstance().inf("STRINGA DA INVIARE A JRIBES = " + params);
                System.exit(0);
            }
             if (command.compareTo("ressap") == 0) {
               JRCContext.getInstance().inf("LANCIO DELLA VALORIZZAZIONE ATTIVA...");

                params = new HelperResocontoSap(args, false).esegui();
                command = "jribes";
            }
            if (command.compareTo("ressap-info") == 0) {
                params = new HelperResocontoSap(args, true).esegui();
                JRCContext.getInstance().inf("STRINGA DA INVIARE A JRIBES = " + params);
                System.exit(0);
            }
            if (command.compareTo("jribes") == 0) {
                jribesLauncher.lancioJRibes(params);
                int ret = jribesLauncher.waitRibes();
                System.out.println("Exit code [" + ret + "]");
                System.exit(ret);
            }

        } catch (Throwable e) {
            JRCContext.getInstance().err("Errore nel lancio!");
            JRCContext.getInstance().err(e);
            System.out.println("Exit code [" + 1 + "]");
            System.exit(1);
        }
        System.out.println("Exit code [0]");
        System.exit(0);

    }

    private static void stampaParametri(String[] args) {
        for (int i = 0; i < args.length; i++) {
            JRCContext.getInstance().inf("PARAMETRO " + i + " = " + args[i]);
        }
    }

}
