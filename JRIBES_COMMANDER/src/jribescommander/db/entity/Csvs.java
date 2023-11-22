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
import jribescommander.db.entity.beans.Csv;
import oracle.jdbc.OracleCallableStatement;

/**
 *
 * @author oracle
 */
public class Csvs {

    public Csv findCsvXDSL(String account, String cicloIni) {

        Csv pk = new Csv();

        OracleCallableStatement cs = null;
        try {
            Connection conn = JRCContext.getInstance().getCon();
            cs = (OracleCallableStatement) conn.prepareCall("{call " + JRCContext.PACKAGE_SPECIAL + ".CSV_FATTURA_XDSL_VER_ES (?,?,?,?,?)}");

            // Impostazione types I/O
            cs.setString(1, account);
            cs.setString(2, cicloIni);
            cs.registerOutParameter(3, Types.INTEGER);
            cs.registerOutParameter(4, Types.INTEGER);
            cs.registerOutParameter(5, Types.VARCHAR);
            cs.execute();
            pk.setCsv(cs.getInt(3));

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
