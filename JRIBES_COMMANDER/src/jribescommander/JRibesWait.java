/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package jribescommander;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.logging.Level;
import java.util.logging.Logger;
import jribescommander.db.JRCContext;

/**
 *
 * @author oracle
 */
public class JRibesWait {
    
  public final static String S_SUCC = "SUCC";
  public final static String S_SUC2 = "SUC2";
  public final static String S_SUC3 = "SUC3";
  
  public final static String S_DUMP = "DUMP";
  public final static String S_KILL = "KILL";
  public final static String S_SEGV = "SEGV";
  public final static String S_TERM = "TERM";
  public final static String S_CLOS = "CLOS";
    
    private String code_elab;
    private int RET_CODE=-1;
    public void setCodeElab(String codeElab)
    {
        this.code_elab=codeElab;
    }
    public int getRetCode()
    {
        return RET_CODE;
    }
    
    public void checkStatus(boolean jpub2) {
        
        String selectStatement = "select CODE_STATO_BATCH from i5_2elab_batch where CODE_ELAB= ?";
        if(jpub2)
             selectStatement = "select CODE_STATO_BATCH from elab_batch where CODE_ELAB= ?";
        
        JRCContext.getInstance().inf("Cerco stato dell'elaborazione CODE_ELAB "+code_elab);
        Connection con = null;
        ResultSet rs =null;
        while (true) {
            try {
                
                con = JRCContext.getInstance().openCon();
                PreparedStatement prepStmt = con.prepareStatement(selectStatement);
                prepStmt.setString(1, code_elab);
                rs = prepStmt.executeQuery();
                boolean firstPresent=rs.next();
                if(!firstPresent)
                {
                   JRCContext.getInstance().err("Code elab "+code_elab+" non presente!! Esecuzione interrotta");
                   RET_CODE=-1;
                   return;
                }
                String code_stato_batch=rs.getString(1);
                JRCContext.getInstance().inf("Trovato stato "+code_stato_batch);
                /*if(code_stato_batch.equals(S_TERM) ||
                        code_stato_batch.equals(S_CLOS) ||
                        code_stato_batch.equals(S_SEGV) ||
                        code_stato_batch.equals(S_KILL) ||
                        code_stato_batch.equals(S_DUMP))*/
                if(JRCContext.getInstance().checkStatoBatch(code_stato_batch)==-1)
                {
                        RET_CODE=-1;
                        return;
                }
               /* if(code_stato_batch.equals(S_SUCC) ||
                        code_stato_batch.equals(S_SUC2) ||
                        code_stato_batch.equals(S_SUC3))*/
                
                if(JRCContext.getInstance().checkStatoBatch(code_stato_batch)==1)   
                {
                        RET_CODE=0;
                        return;
                }
                
                      
                
              prepStmt.close();
               
                
            } catch (Throwable e) {
                 JRCContext.getInstance().err("Errore nel ricevere lo stato del code elab "+code_elab);
                   JRCContext.getInstance().err(e);
            } finally {
                if(con!=null)
                    try {
                        try{
                        if(rs!=null)
                            rs.close();
                        }
                        catch(Throwable e)
                        {}
                        con.close();
                } catch (Throwable ex) {
                   
                }
                 try{
                Thread.sleep(60000);
                }catch(InterruptedException e)
                {}
            }
        }

    }
}
