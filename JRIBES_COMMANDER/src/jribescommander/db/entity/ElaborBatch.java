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
import jribescommander.db.JRCContext;
import jribescommander.db.entity.beans.BatchElem;
import oracle.jdbc.OracleCallableStatement;

/**
 *
 * @author oracle
 */
public class ElaborBatch {

    public int elabBatchUguali(int flagContratto) {

        int ret = -1;
        OracleCallableStatement cs = null;
        try {
            Connection conn = JRCContext.getInstance().getCon();

            cs = (OracleCallableStatement) conn.prepareCall("{call " + JRCContext.PACKAGE_SPECIAL + ".ACCOUNT_VER_ELAB_IN_CORSO (?,?,?,?)}");

            // Impostazione types I/O
            cs.setInt(1, flagContratto);
            cs.registerOutParameter(2, Types.INTEGER);
            cs.registerOutParameter(3, Types.INTEGER);
            cs.registerOutParameter(4, Types.VARCHAR);
            cs.execute();
            ret = cs.getInt(2);

            cs.close();

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

        return ret;

    }

    public int elabBatchCodeTipoContrUguali(String CodeTipoContr) {
        int ret = -1;
        OracleCallableStatement cs = null;
        try {
            Connection conn = JRCContext.getInstance().getCon();
            cs = (OracleCallableStatement) conn.prepareCall("{call " + JRCContext.PACKAGE_SPECIAL + ".ACC_VER_ELAB_CONTR_IN_CORSO (?,?,?,?)}");

            // Impostazione types I/O
            cs.setString(1, CodeTipoContr);
            cs.registerOutParameter(2, Types.INTEGER);
            cs.registerOutParameter(3, Types.INTEGER);
            cs.registerOutParameter(4, Types.VARCHAR);
            cs.execute();
            ret = cs.getInt(2);

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

        return ret;

    }
    
    
    public BatchElem getCodeFunzFlag(String codeTipoContr,String codeTipoBatch, String codeFunzXRep) 
    {
    BatchElem  elem = null;
     OracleCallableStatement cs = null;
        try {
            Connection conn = JRCContext.getInstance().getCon();
       cs=(OracleCallableStatement)conn.prepareCall("{call " + JRCContext.PACKAGE_SPECIAL + ".BATCH_DA_LANCIARE(?,?,?,?,?,?,?,?)}");
      cs.setString(1,codeTipoContr);
      cs.setString(2,codeTipoBatch);
      cs.setString(3,codeFunzXRep);
      cs.setString(4,"S");
      cs.registerOutParameter(5,Types.VARCHAR);
      cs.registerOutParameter(6,Types.INTEGER);
      cs.registerOutParameter(7,Types.INTEGER);
      cs.registerOutParameter(8,Types.VARCHAR); 
      cs.execute();
     if ((cs.getInt(7)!=JRCContext.OK_RT)&&(cs.getInt(7)!=JRCContext.NOT_FOUND_RT))
          throw new RuntimeException("Errore nella ricerca del codice funzione! Codice["+cs.getInt(7)+"]");

      
      elem=new BatchElem();
      elem.setCodeFunz(cs.getString(5));
      elem.setFlagTipoContr(cs.getInt(6));

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

        return elem;
  }
}
