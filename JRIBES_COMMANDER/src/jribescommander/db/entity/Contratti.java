/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package jribescommander.db.entity;

import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import jribescommander.db.JRCContext;
import jribescommander.db.entity.beans.Contratto;
import oracle.jdbc.OracleCallableStatement;
import oracle.jdbc.OracleTypes;

/**
 *
 * @author oracle
 */
public class Contratti {

    public ArrayList getTuttiContratti() {
        ArrayList lvct_TipiContratto = new ArrayList();
        OracleCallableStatement cs = null;
        try {
            Connection conn = JRCContext.getInstance().getCon();

            cs = (OracleCallableStatement) conn.prepareCall("{? = call PKG_BILL_COM.getTipiContrattoPS}");
            cs.registerOutParameter(1, OracleTypes.CURSOR);
            cs.execute();

            ResultSet lrs_Rset = (ResultSet) cs.getObject(1);
            while (lrs_Rset.next()) {
                Contratto lobj_TipoContratto = new Contratto();
                lobj_TipoContratto.setCODE_TIPO_CONTR(lrs_Rset.getString("CODE_TIPO_CONTR"));
                lobj_TipoContratto.setDESC_TIPO_CONTR(lrs_Rset.getString("DESC_TIPO_CONTR"));
                lobj_TipoContratto.setFLAG_SYS(lrs_Rset.getString("FLAG_SYS"));
                lobj_TipoContratto.setTIPO_SPECIAL(lrs_Rset.getString("TIPO_SPECIAL"));
                lvct_TipiContratto.add(lobj_TipoContratto);
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
        return lvct_TipiContratto;
    }
}
