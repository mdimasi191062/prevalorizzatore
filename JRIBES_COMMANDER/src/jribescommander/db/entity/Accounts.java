/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package jribescommander.db.entity;

import jribescommander.db.entity.beans.AccountElab;
import java.rmi.RemoteException;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Types;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Vector;
import jribescommander.db.JRCContext;
import jribescommander.db.entity.beans.Account;
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
public class Accounts {

    public void setCodeParam(Account acc) {
        OracleCallableStatement cs = null;
        try {
            Connection conn = JRCContext.getInstance().getCon();

            cs = (OracleCallableStatement) conn.prepareCall("{call " + JRCContext.PACKAGE_SPECIAL + ".PARAM_VALO_LEGGI_COD_X_ACC(?,?,?,?)}");
            // Impostazione types I/O
            cs.setString(1, acc.getAccount());
            cs.registerOutParameter(2, Types.VARCHAR);
            cs.registerOutParameter(3, Types.INTEGER);
            cs.registerOutParameter(4, Types.VARCHAR);
            cs.execute();
            acc.setCodeParam(cs.getString(2));

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

    }

    public ArrayList getAccountXValorizzazione(String CicloFatt, String DataIniCiclo, String CodTipoContr, boolean cong) {
        ArrayList recs = new ArrayList();
        OracleCallableStatement cs = null;
        try {
            Connection conn = JRCContext.getInstance().getCon();
            if (!cong) {
                cs = (OracleCallableStatement) conn.prepareCall("{call " + JRCContext.PACKAGE_SPECIAL + ".ACCOUNT_LST_X_VA(?,?,?,?,?,?)}");
            } else {
                cs = (OracleCallableStatement) conn.prepareCall("{call " + JRCContext.PACKAGE_SPECIAL + ".ACCOUNT_VA_ABORT_X_CICLO(?,?,?,?,?,?)}");
            }
            cs.setString(1, CicloFatt);
            cs.setString(2, DataIniCiclo);
            cs.setString(3, CodTipoContr);
            cs.registerOutParameter(4, OracleTypes.ARRAY, "ARR_ACCOUNT_ABORT");
            cs.registerOutParameter(5, Types.INTEGER);
            cs.registerOutParameter(6, Types.VARCHAR);
            cs.execute();
            if ((cs.getInt(6) != JRCContext.OK_RT) && (cs.getInt(6) != JRCContext.NOT_FOUND_RT)) {
                throw new RuntimeException("Errore nella ricerca degli account per il ciclo! Codice[" + cs.getInt(6) + "]");
            }
            ArrayDescriptor ad = ArrayDescriptor.createDescriptor("ARR_ACCOUNT_ABORT", conn);

            // Ottengo i dati
            ARRAY rs = cs.getARRAY(4);
            Datum dati[] = rs.getOracleArray();
            // Estrazione dei dati
            for (int i = 0; i < dati.length; i++) {
                Account pk = new Account();
                STRUCT s = (STRUCT) dati[i];
                Datum attr[] = s.getOracleAttributes();
                pk.setAccount(attr[0].stringValue());
                pk.setDesc(attr[1].stringValue());
                pk.setDataIniPerFatt(attr[4].stringValue());
                if (attr[5] != null) {
                    pk.setDataFinePerFatt(attr[5].stringValue());
                }
                recs.add(pk);
            }

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
        return recs;
    }

    public Collection getAccountPerReport(String DataIni, String DataFine, String CodTipoContr, String CodeFunz, boolean repricing) {
        ArrayList recs = new ArrayList();
        OracleCallableStatement cs = null;
        try {
            Connection conn = JRCContext.getInstance().getCon();
            if (!repricing) {
                cs = (OracleCallableStatement) conn.prepareCall("{call " + JRCContext.PACKAGE_SPECIAL_ONLINE + ".ACCOUNT_LST_REPORT_VA_XDSL (?,?,?,?,?,?,?,?)}");
            } else {
                cs = (OracleCallableStatement) conn.prepareCall("{call " + JRCContext.PACKAGE_SPECIAL + ".ACCOUNT_LST_REPORT_REPR_SP (?,?,?,?,?,?,?,?)}");
            }
            // Impostazione types I/O
            cs.setString(1, DataIni);
            cs.setString(2, DataFine);
            cs.setString(3, CodTipoContr);
            cs.setString(4, CodeFunz);
            cs.setString(5, "S");
            cs.registerOutParameter(6, OracleTypes.ARRAY, "ARR_ACCOUNT_CONG");
            cs.registerOutParameter(7, Types.INTEGER);
            cs.registerOutParameter(8, Types.VARCHAR);

            cs.execute();

            // Costruisco l'array che conterr� i dati di ritorno della stored
            ArrayDescriptor ad = ArrayDescriptor.createDescriptor("ARR_ACCOUNT_CONG", conn);
            ARRAY rs = cs.getARRAY(6);

            Datum dati[] = rs.getOracleArray();

            // Estrazione dei dati
            for (int i = 0; i < dati.length; i++) {
                Account pk = new Account();

                STRUCT s = (STRUCT) dati[i];
                Datum attr[] = s.getOracleAttributes();

                pk.setAccount(attr[1].stringValue());
                pk.setDesc(attr[2].stringValue());
                recs.add(pk);
            }

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
        return recs;
    }

    public Collection getAccountXCong(String code_tipo_contr) {
        ArrayList recs = new ArrayList();
        OracleCallableStatement cs = null;
        try {
            Connection conn = JRCContext.getInstance().getCon();
            cs = (OracleCallableStatement) conn.prepareCall("{call " + JRCContext.PACKAGE_SPECIAL_ONLINE + ".ACCOUNT_LST_X_LAN_CONG_SPE(?,?,?,?)}");

            // Impostazione types I/O    
            cs.setString(1, code_tipo_contr);
            cs.registerOutParameter(2, OracleTypes.ARRAY, "ARR_ACC_X_VER_VA_2");
            cs.registerOutParameter(3, Types.INTEGER);
            cs.registerOutParameter(4, Types.VARCHAR);

            cs.execute();

            // Costruisco l'array che conterr� i dati di ritorno della stored
            ArrayDescriptor ad = ArrayDescriptor.createDescriptor("ARR_ACC_X_VER_VA_2", conn);
            ARRAY rs = cs.getARRAY(2);
            Datum dati[] = rs.getOracleArray();

            // Estrazione dei dati
            for (int i = 0; i < dati.length; i++) {
                AccountElab elem = new AccountElab();

                STRUCT s = (STRUCT) dati[i];

                Datum attr[] = s.getOracleAttributes();
                elem.setCodeParam(JRCContext.nh(attr[0].stringValue()));
                elem.setCodeAccount(JRCContext.nh(attr[1].stringValue()));
                elem.setDataIni(JRCContext.nh(attr[3].stringValue()));

                recs.add(elem);

            }
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
        return recs;
    }

    public Collection getAccountXRepricing(String code_tipo_contr) {
        ArrayList recs = new ArrayList();
        OracleCallableStatement cs = null;
        try {
            Connection conn = JRCContext.getInstance().getCon();
            cs = (OracleCallableStatement) conn.prepareCall("{call " + JRCContext.PACKAGE_SPECIAL + ".ACCOUNT_LST_X_VER_RPC_SPE(?,?,?,?)}");

            // Impostazione types I/O    
            cs.setString(1, code_tipo_contr);
            cs.registerOutParameter(2, OracleTypes.ARRAY, "ARR_ACC_X_VER_NC");
            cs.registerOutParameter(3, Types.INTEGER);
            cs.registerOutParameter(4, Types.VARCHAR);

            cs.execute();

            // Costruisco l'array che conterr� i dati di ritorno della stored
            ArrayDescriptor ad = ArrayDescriptor.createDescriptor("ARR_ACC_X_VER_NC", conn);
            ARRAY rs = cs.getARRAY(2);
            Datum dati[] = rs.getOracleArray();

            // Estrazione dei dati
            for (int i = 0; i < dati.length; i++) {
                AccountElab elem = new AccountElab();

                STRUCT s = (STRUCT) dati[i];

                Datum attr[] = s.getOracleAttributes();
                
                
                elem.setCodeParam(attr[0].stringValue());
              elem.setCodeAccount(attr[1].stringValue());
              elem.setAccount(attr[2].stringValue());

                recs.add(elem);

            }
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
        return recs;
    }

    public boolean controllaAccountRepricing(String CodeTipoContr)  {

        ArrayList recs = new ArrayList();
        OracleCallableStatement cs = null;
        try {
            Connection conn = JRCContext.getInstance().getCon();
            cs = (OracleCallableStatement) conn.prepareCall("{call " + JRCContext.PACKAGE_SPECIAL + ".CONTROLLO_ACC_REPR_SPE(?,?,?,?)}");

            cs.setString(1, CodeTipoContr);
            cs.registerOutParameter(2, Types.INTEGER);
            cs.registerOutParameter(3, Types.INTEGER);
            cs.registerOutParameter(4, Types.VARCHAR);
            cs.execute();

            if ((cs.getInt(3) != JRCContext.OK_RT) && (cs.getInt(3) != JRCContext.NOT_FOUND_RT)) {
                throw new RuntimeException("DB:" + cs.getInt(3) + ":" + cs.getString(4));
            }
            int nTestate = cs.getInt(2);
            return nTestate == 0;

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

    }
}
