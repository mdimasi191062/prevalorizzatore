/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package jribescommander;

import java.sql.CallableStatement;
import java.sql.Connection;
import java.sql.SQLException;
import java.util.StringTokenizer;
import javax.management.RuntimeErrorException;
import jribescommander.db.JRCContext;
import oracle.jdbc.OracleCallableStatement;
import oracle.jdbc.OracleTypes;
import oracle.sql.Datum;
import oracle.sql.STRUCT;
import oracle.sql.StructDescriptor;

/**
 *
 * @author oracle
 */
public class LaunchJRibes {
    
    private static final String LancioRibes ="{? = call  EXTERNAL_LIBRARY.SEND_REQUEST(?,?,?,?,?,?) }";;
  private CallableStatement cs = null;
  final int primo_parametro=0;
   JRibesReturn jribesClass;
   
   
   
  public void lancioJRibes(String strComRibes){
    int ritorno;
    String send = "";
    String dataSched = "";
    String codeTipoContr = "";
     send = "SEND";
    
    String codeFunz = "";
    String codeUtente = "";
    String generaReport = "";
    
    try{
    StringTokenizer st = new StringTokenizer(strComRibes,"$");

    codeFunz = st.nextToken();
    codeUtente = st.nextToken();
    if(!codeFunz.equals("2001"))
      generaReport = st.nextToken();
    }
    catch(Throwable e)
    {
        throw new RuntimeException("Errore nei parametri jribes ("+strComRibes+")!");
    }
    
    JRCContext.getInstance().inf("Parametri jribes calcolati: ");
    JRCContext.getInstance().inf(" "+strComRibes);
    JRCContext.getInstance().inf(" "+codeUtente);
    JRCContext.getInstance().inf(" "+codeTipoContr);
    
    Connection conn=JRCContext.getInstance().getCon();
    try
    {
      
      OracleCallableStatement cs = (OracleCallableStatement)conn.prepareCall(LancioRibes);
      cs.registerOutParameter(1,OracleTypes.NUMBER);
      cs.setString(2,send); //SEND OR NULL
      cs.setString(3,strComRibes); //MESSAGGIO RIBES
      cs.setString(4,dataSched); //DATA SCHEDULAZIONE
      cs.setString(5,codeUtente); //CODICE UTENTE
      cs.setString(6,codeTipoContr); //CODE_TIPO_CONTR
      cs.registerOutParameter(7,OracleTypes.STRUCT, "EXTERNAL_LIBRARY_OBJ");
      cs.execute();
      ritorno = cs.getInt(1);
     
      if(ritorno!=0)
          throw new RuntimeException("Impossibile lanciare JRIBES. Errore nel lancio");
      

       StructDescriptor sd = StructDescriptor.createDescriptor("EXTERNAL_LIBRARY_OBJ",conn);
        STRUCT rs;
      
        // Ottengo i dati
        rs=cs.getSTRUCT(7);
        Datum dati[]=rs.getOracleAttributes();      
        

        jribesClass = new JRibesReturn();
        
        if (dati[0]!=null) 
            jribesClass.setJr_code_elab(dati[0].stringValue());
        else 
            throw new RuntimeException("Impossibile recuperare il code elab. Errore in JRIBES");
          
        if (dati[1]!=null) jribesClass.setJr_flag_sys(dati[1].stringValue());
        else jribesClass.setJr_flag_sys("");

        if (dati[2]!=null) jribesClass.setJr_name(dati[2].stringValue());
        else jribesClass.setJr_name("");

        if (dati[3]!=null) jribesClass.setJr_username(dati[3].stringValue());
        else jribesClass.setJr_username("");

        if (dati[4]!=null) jribesClass.setJr_is_available(dati[4].stringValue());
        else jribesClass.setJr_is_available("");

        if (dati[5]!=null) jribesClass.setJr_have_concurrency(dati[5].stringValue());
        else jribesClass.setJr_have_concurrency("");

        if (dati[6]!=null) jribesClass.setJr_error_code(dati[6].intValue());
        else jribesClass.setJr_error_code(0);

        if (dati[7]!=null) jribesClass.setJr_error_desc(dati[7].stringValue());
        else jribesClass.setJr_error_desc("");
      
        if(jribesClass.getJr_error_code()!=0)
        {
            throw new RuntimeException("Errore nel lancio dell'eseguibile. Ret Code ["+jribesClass.getJr_error_code()+"] "+jribesClass.getJr_error_desc());
        }
        else
        {
            JRCContext.getInstance().inf("JRIBES ritornato con codice "+jribesClass.getJr_error_code());
        }
        if(jribesClass.getJr_is_available().equals("0"))
        {
            if(jribesClass.getJr_have_concurrency().equals("1"))
                throw new RuntimeException("Errore nel lancio dell'eseguibile. Processo non lanciato a causa di concorrenza con un altro processo con code_elab="+jribesClass.getJr_code_elab() );
            else
                 throw new RuntimeException("Errore nel lancio dell'eseguibile. Errore sul db!" );
                
        }
        JRCContext.getInstance().inf("CODE ELAB=  "+jribesClass.getJr_code_elab());
             
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

  }

    int waitRibes(boolean jpub2) {
        JRibesWait waitRibes=new JRibesWait();
        waitRibes.setCodeElab(jribesClass.getJr_code_elab());
        waitRibes.checkStatus(jpub2);
        return waitRibes.getRetCode();
    }

}
