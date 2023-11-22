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
public class HelperSap extends GenericHelper {

    private String codeTipoContr = null;
    private String repricing;
    private String rewrite;
    boolean debug;
    private String dataInizioCiclo = null;
    private String dataFineCiclo = null;

    public HelperSap(String[] args, boolean debug) {

        this.debug = debug;
        if (debug) {
            if (args.length != 1) {
                throw new RuntimeException("Lancio errato. Usage: jribescommander.sh sap-info");
            }

            codeUtente = "";
            rewrite = "N";
            repricing = "N";
        } else {
            init(args);
        }
    }

    public void init(String[] args) {

        if (args.length < 4) {
            throw new RuntimeException("Lancio errato. Usage: jribescommander.sh sap tipo(V/R) rewrite(S/N) utente [CODE_TIPO_CONTR]");
        }

        if (args[1].equals("R")) {
            repricing = "R";
        } else if (args[1].equals("V")) {
            repricing = "V";
        } else {
            throw new RuntimeException("Il parametro tipo può essere V (Valorizzazione) o R(Repricing)!");
        }

        if (args[2].equals("S")) {
            rewrite = "S";
        } else if (args[2].equals("N")) {
            rewrite = "N";
        } else {
            throw new RuntimeException("Il parametro tipo può essere V (Valorizzazione) o R(Repricing)!");
        }

        codeUtente = args[3];
        if (args.length == 5) {
            codeTipoContr = args[4];
        }

    }

    public String esegui() {
        controlloAltreElaborazioni();
        boolean isRewrite = rewrite.equals("S");
        String codeFunz = isRewrite ? "10039" : "10037";
        String listContr = "";
        PeriodoRiferimento periodoRifermento = null;
        
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
                if (!isRewrite && periodoRifermento == null) {
                    periodoRifermento = new CicloFatrz().findPerCicloFat("1", contratto.getCODE_TIPO_CONTR());
                    if (periodoRifermento.getDataFineCiclo() == null || periodoRifermento.getDataIniCiclo() == null) {
                        periodoRifermento = null;
                    }
                }
            }

        } else {
            listContr = codeTipoContr;
            if (!isRewrite) {
                periodoRifermento = new CicloFatrz().findPerCicloFat("1", codeTipoContr);
                if (periodoRifermento.getDataFineCiclo() == null || periodoRifermento.getDataIniCiclo() == null) {
                    periodoRifermento = null;
                }

            }
        }
        
        String datiBatch = null;
        if (!isRewrite) {
            if (periodoRifermento == null) {
                throw new RuntimeException("Nessun periodo di riferimento trovato per il servizio scelto!");
            } else {

                JRCContext.getInstance().inf("CICLO DI RIFERIMENTO = " + periodoRifermento.getDataIniCiclo() + " - " + periodoRifermento.getDataFineCiclo());
                if (debug) {
                    System.out.println("CICLO DI RIFERIMENTO = " + periodoRifermento.getDataIniCiclo() + " - " + periodoRifermento.getDataFineCiclo());
                }
            }
            datiBatch = periodoRifermento.getDataIniCiclo() + "$" + periodoRifermento.getDataFineCiclo() + "$" + repricing + "$" + rewrite + "$" + listContr + "$";
        } else {
            datiBatch = repricing + "$" + rewrite + "$" + listContr + "$";

        }
        return codeFunz + "$" + codeUtente + "$_$" + datiBatch;

    }

}
