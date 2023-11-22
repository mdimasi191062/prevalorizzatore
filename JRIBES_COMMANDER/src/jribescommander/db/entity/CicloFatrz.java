/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package jribescommander.db.entity;

import java.rmi.RemoteException;
import java.sql.Connection;
import java.sql.SQLException;
import java.sql.Types;
import java.util.Collection;
import java.util.Vector;
import jribescommander.db.JRCContext;
import jribescommander.db.entity.beans.PeriodoRiferimento;
import oracle.jdbc.OracleCallableStatement;
import oracle.jdbc.OracleTypes;
import oracle.sql.ARRAY;
import oracle.sql.ArrayDescriptor;
import oracle.sql.Datum;
import oracle.sql.STRUCT;

/**
 *
 * @author oracle
 */
public class CicloFatrz {

    public PeriodoRiferimento findPerCicloFat(String CodCicloFat, String CodTipoContr) {
        PeriodoRiferimento pk = new PeriodoRiferimento();
        OracleCallableStatement cs = null;
        try {
            Connection conn = JRCContext.getInstance().getCon();
            cs = (OracleCallableStatement) conn.prepareCall("{call " + JRCContext.PACKAGE_SPECIAL + ".PARAM_VALO_DETERMINA_CICLO (?,?,?,?,?,?,?)}");

            // Impostazione types I/O
            cs.setString(1, CodCicloFat);
            cs.setString(2, CodTipoContr);
            cs.registerOutParameter(3, Types.VARCHAR);
            cs.registerOutParameter(4, Types.VARCHAR);
            cs.registerOutParameter(5, Types.VARCHAR);
            cs.registerOutParameter(6, Types.INTEGER);
            cs.registerOutParameter(7, Types.VARCHAR);
            cs.execute();
            if ((cs.getInt(6) != JRCContext.OK_RT) && (cs.getInt(6) != JRCContext.NOT_FOUND_RT)) {
                throw new RuntimeException("Errore nella ricerca del periodo di riferimento! Codice[" + cs.getInt(6) + "]");
            }
            pk.setDataIniCiclo(cs.getString(3));
            pk.setDataFineCiclo(cs.getString(4));
            pk.setPeriodoFat(cs.getString(5));

        } catch (Throwable e) {
            JRCContext.getInstance().err(e);
            throw new RuntimeException(e);
        } finally {
            if (cs != null) {
                try {
                    cs.close();
                } catch (SQLException ex) {

                }
            }
        }
        return pk;
    }

    public PeriodoRiferimento findPeriodoRiferimentoReport(String CodTipoContr, boolean repricing)  {
        PeriodoRiferimento pk = new PeriodoRiferimento();
        OracleCallableStatement cs = null;
        try {
            Connection conn = JRCContext.getInstance().getCon();
            if (repricing) {
                cs = (OracleCallableStatement) conn.prepareCall("{call " + JRCContext.PACKAGE_SPECIAL + ".PARAM_VALO_SP_PER_REPR_XDSL (?,?,?,?)}");
            } else {
                cs = (OracleCallableStatement) conn.prepareCall("{call " + JRCContext.PACKAGE_SPECIAL + ".PARAM_VALO_SP_PER_RIFR_VA_XDSL (?,?,?,?)}");
            }
            // Impostazione types I/O
            cs.setString(1, CodTipoContr);
            cs.registerOutParameter(2, OracleTypes.ARRAY, "ARR_PERIODO_RIFR");
            cs.registerOutParameter(3, Types.INTEGER);
            cs.registerOutParameter(4, Types.VARCHAR);
            cs.execute();
            if ((cs.getInt(3) != JRCContext.OK_RT) && (cs.getInt(3) != JRCContext.NOT_FOUND_RT)) {
                throw new RuntimeException("Errore nella ricerca del periodo di riferimento! Codice[" + cs.getInt(3) + "]");
            }
            if(cs.getInt(3)==JRCContext.NOT_FOUND_RT)
                throw new RuntimeException("Nessun periodo di riferimento trovato per il contratto "+CodTipoContr);
            // Costruisco l'array che conterr� i dati di ritorno della stored
            ArrayDescriptor ad = ArrayDescriptor.createDescriptor("ARR_PERIODO_RIFR", conn);
            ARRAY rs = cs.getARRAY(2);

            Datum dati[] = rs.getOracleArray();

            STRUCT s = (STRUCT) dati[dati.length - 1]; //PRENDO QUELLO CON DATA INIZIO FATTURAZIONE PIU GRANDE
            Datum attr[] = s.getOracleAttributes();
            pk.setCode(attr[1].stringValue());
            pk.setDescOf(attr[0].stringValue());

        } catch (Throwable e) {
            JRCContext.getInstance().err(e);
            throw new RuntimeException(e);
        } finally {
            if (cs != null) {
                try {
                    cs.close();
                } catch (SQLException ex) {

                }
            }
        }
        return pk;
    }

}
