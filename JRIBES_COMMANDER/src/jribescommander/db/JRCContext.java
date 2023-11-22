/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package jribescommander.db;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintStream;
import java.io.PrintWriter;
import java.io.StringWriter;
import java.sql.Connection;
import java.sql.SQLException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Calendar;
import java.util.Date;
import java.util.Properties;
import java.util.TimeZone;

/**
 *
 * @author oracle
 */
public class JRCContext {
    //LOG
      public final static String ERRCODE = "CRIT";
     public final static String INFCODE = "INFO";
     
    //PACKAGES
    public static final String PACKAGE_SPECIAL            = "PKG_BILL_SPE";
    public static final String PACKAGE_SPECIAL_ONLINE     = "PKG_BILL_SPE_ONLINE";    
    //DB MESSAGES
    public static final int OK_RT=0;
    public static final int NOT_FOUND_RT=-1403;

    public  int checkStatoBatch(String code_stato_batch) {
        if(OKSTATUS.contains(code_stato_batch))
            return 1;
        if(NOKSTATUS.contains(code_stato_batch))
            return -1;
        return 0;
    }
    
    
    private ArrayList OKSTATUS=new ArrayList();
    private ArrayList NOKSTATUS=new ArrayList();
    
    private Connection con;
    private Properties pr ;
    String LogFile;
    private JRCContext()
    {
          
    
    }
    
    private static JRCContext _instance=null;
    public static JRCContext getInstance()
    {
        if(_instance==null)
            _instance=new JRCContext();
        return _instance;
    }
//funzione che gestisce i null delle stringhe
    //restituisce "".
    //nh = null handler
    public static String nh(String lstr_Input)
    throws Exception
    {
        if(lstr_Input==null){
            return "";
        }else{
            return lstr_Input;
        }
    }
    public Connection openCon() throws ClassNotFoundException, SQLException{
        setCon(null);
        String Username=null;
        String Password=null;
       
          Class.forName(pr.getProperty("DBDRIVER"));
          StringBuffer dburl = new StringBuffer();
          dburl.append("jdbc:oracle:thin:@");
          dburl.append(pr.getProperty("DBIP"));
          dburl.append(":");
          dburl.append(pr.getProperty("DBPORT"));
          dburl.append(":");
          dburl.append(pr.getProperty("ORACLE_SID"));
          Username=pr.getProperty("USERNAME");
          Password=pr.getProperty("USERPWD");
          setCon(java.sql.DriverManager.getConnection(dburl.toString(),Username,Password));
        
            return getCon();
    }
 
    public static Properties getEnvVars() {
    Properties envVars = new Properties();
    try {
      Process p = null;
      Runtime r = Runtime.getRuntime();
      String OS = System.getProperty("os.name").toLowerCase();
      if (OS.indexOf("windows 9") > -1) {
        p = r.exec( "command.com /c set" );
      } else if ( (OS.indexOf("nt") > -1) || (OS.indexOf("windows 2000") > -1 ) || (OS.indexOf("windows xp") > -1) ) {
        p = r.exec( "cmd.exe /c set" );
      } else {	
        p = r.exec( "env" );
      }
      BufferedReader br = new BufferedReader(new InputStreamReader(p.getInputStream()));
      String line;
      while((line = br.readLine())!= null) {
        int idx = line.indexOf( '=' );
        String key = line.substring( 0, idx );
        String value = line.substring( idx+1 );
        envVars.setProperty(key,value);
      }
    }catch(Exception ex){
      throw new RuntimeException(ex);
    }
    return envVars;
  }
    
    public void init(String type) throws IOException, Exception {
        pr=getEnvVars();
        pr.load(new FileInputStream(new File("./jribescommander.properties")));
        
       
        
        if(!pr.containsKey("USERPWD"))
        {
            throw new Exception("Impossibile trovare la variabile d'ambiente USERPWD");
        }
        if(!pr.containsKey("PATHLOGFILE"))
        {
            throw new Exception("Impossibile trovare la variabile d'ambiente PATHLOGFILE");
        }
        if(!pr.containsKey("USERNAME"))
        {
            throw new Exception("Impossibile trovare la variabile d'ambiente USERNAME");
        }
        
        if(!pr.containsKey("ORACLE_SID"))
        {
            throw new Exception("Impossibile trovare la variabile d'ambiente ORACLE_SID");
        }
        
       if(!pr.containsKey("DBIP"))
        {
            throw new Exception("Impossibile trovare la variabile d'ambiente DBIP");
        }
        
        if(!pr.containsKey("DBPORT"))
        {
            throw new Exception("Impossibile trovare la variabile d'ambiente DBPORT");
        }
        if(!pr.containsKey("OK_STATUS"))
        {
            throw new Exception("Impossibile trovare la variabile d'ambiente OK_STATUS");
        }
        else
        {
           OKSTATUS.addAll(Arrays.asList(pr.getProperty("OK_STATUS").split(";")));
        }
        if(!pr.containsKey("NOK_STATUS"))
        {
            throw new Exception("Impossibile trovare la variabile d'ambiente NOK_STATUS");
        }
        else
        {
           NOKSTATUS.addAll(Arrays.asList(pr.getProperty("NOK_STATUS").split(";")));
        }
              
        if (!pr.getProperty("PATHLOGFILE").endsWith("/")){
            getPr().setProperty("PATHLOGFILE", getPr().getProperty("PATHLOGFILE")+"/");
        }
        SimpleDateFormat sdf=new SimpleDateFormat("yyyyMMdd");
        Date now=new Date();
        LogFile=getPr().getProperty("PATHLOGFILE")+sdf.format(now)+"_"+"JRibesCommander_"+type+"_"+now.getTime()+".log";
      
  }
    private synchronized void log(String m, String type, String source){
    PrintStream out=null;
        try {
      //INFO 14/05/2004 10:00:18 10.11.95.69 ACCESSO ALLA PAGINA DI LOGIN
      // String source = "NULL";
      out = new PrintStream(new FileOutputStream(LogFile,true));
      StringBuffer sb = new StringBuffer();
      
      SimpleDateFormat sdf=new SimpleDateFormat("dd/MM/yyyy hh:mm:ss");
       
      sb.append(type);
      sb.append(" ");
      sb.append(sdf.format(new Date()));
      sb.append(" ");
      sb.append(source);
      sb.append(" ");
      sb.append(m);
      out.println(sb.toString());
      out.flush();
     
    }catch(Exception ex){
      throw new RuntimeException(ex);
    }  
    finally
    {
     if (out != null)
             out.close();
    }
  }


  public void err(String m)
  {
    log(m, ERRCODE, "JRIBESCOMMANDER");
      System.err.println(m);
  }
 public void err(Throwable e){
    StringWriter errors = new StringWriter();
    e.printStackTrace(new PrintWriter(errors));
    log(ERRCODE, errors.toString(), "JRIBESCOMMANDER");
    e.printStackTrace(System.err);
  }
  public void inf(String m){
    log(m, INFCODE,  "JRIBESCOMMANDER");
    
  }

    /**
     * @return the con
     */
    public Connection getCon() {
        return con;
    }

    /**
     * @param con the con to set
     */
    public void setCon(Connection con) {
        this.con = con;
    }

    /**
     * @return the pr
     */
    public Properties getPr() {
        return pr;
    }
  
    
}
