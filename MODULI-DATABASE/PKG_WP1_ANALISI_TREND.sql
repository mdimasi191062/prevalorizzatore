spool 'PKG_WP1_ANALISI_TREND.log';

CREATE OR REPLACE PACKAGE PKG_WP1_ANALISI_TREND AS 

  /* TODO enter package declarations (types, exceptions, methods etc) here */ 
  function GET_ACCOUNT_CON_FATTURA (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER;


  function ANALISI_TREND (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_OPERATION IN varchar2) 
    return NUMBER;

END PKG_WP1_ANALISI_TREND;
/


CREATE OR REPLACE PACKAGE BODY PKG_WP1_ANALISI_TREND AS
  function GET_ACCOUNT_CON_FATTURA (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER is
  BEGIN
  INSERT INTO WP1_TABLE_KEY (WP1_TABLE_KEY.ID_KEY_VARCHAR) 
						select I5_1ACCOUNT.CODE_ACCOUNT from 
                    I5_1ACCOUNT,
                    I5_1ACCOUNT_X_CONTR ,
                    I5_1CONTR
                    where
                     I5_1ACCOUNT_X_CONTR.CODE_ACCOUNT=I5_1ACCOUNT.CODE_ACCOUNT
                     and I5_1ACCOUNT_X_CONTR.CODE_CONTR=I5_1CONTR.CODE_CONTR
                     and I5_1ACCOUNT.FLAG_SYS='S'
                     and I5_1CONTR.FLAG_SYS='S'
                     and  exists (
                     SELECT CODE_ACCOUNT from I5_2TEST_DOCUM_FATT_XDSL where tipo_flag_funzione_creaz_impt='V' union all
                     SELECT CODE_ACCOUNT from I5_2TEST_DOCUM_NOTA_CRED_XDSL where tipo_flag_funzione_creaz_impt='V' 
                     );
						
  
  
  RETURN 0;
  END GET_ACCOUNT_CON_FATTURA;
  
  function ANALISI_TREND (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_OPERATION IN varchar2) 
    return NUMBER is
    ts int;
    trend_ana_depth int;
    
    trend_ana_threshold number(18,6);
    
    trend_ana_depth_account int;
    trend_ana_threshold_account number(18,6);
    
    
    media_precedente_fattura number(18,6);
    media_precedente_notacred number(18,6);
     importo_fattura number(18,6);
      importo_notacred number(18,6);
      dis_perc_fattura number(18,6);
       dis_perc_notacred number(18,6);
        dis_perc_tot number(18,6);
      
      alarm number;
     Cursor chiavi is
            Select *
                From WP1_TABLE_KEY ;

  BEGIN
    begin       
	    select PARAM_VALUE into ts from WP1JOB_TEMP where PARAM_NAME = '$TS$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $TS$ nei parametri'); 
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $TS$ nei parametri'); 
      end;
      
      
      begin       
	    select PARAM_VALUE into trend_ana_depth from WP1JOB_TEMP where PARAM_NAME = '$TREND_ANA_DEPTH$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $TREND_ANA_DEPTH$ nei parametri'); 
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $TREND_ANA_DEPTH$ nei parametri'); 
      end;
  
   begin       
	    select PARAM_VALUE into trend_ana_threshold from WP1JOB_TEMP where PARAM_NAME = '$TREND_ANA_THRESHOLD$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $TREND_ANA_THRESHOLD$ nei parametri'); 
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $TREND_ANA_THRESHOLD$ nei parametri'); 
      end;
      
       For chiave In chiavi
        Loop
        alarm:=0;
        
         begin       
	    select PARAM_VALUE into trend_ana_threshold_account from WP1JOB_TEMP where PARAM_NAME = '$TREND_ANA_THRESHOLD_'||chiave.ID_KEY_VARCHAR||'$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        trend_ana_threshold_account:=null;
      end;
      
         begin       
	    select PARAM_VALUE into trend_ana_depth_account from WP1JOB_TEMP where PARAM_NAME = '$TREND_ANA_DEPTH_'||chiave.ID_KEY_VARCHAR||'$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        trend_ana_depth_account:=null;
      end;
      
      if(trend_ana_threshold_account=null) then
        trend_ana_threshold_account:=trend_ana_threshold;
      end if;
       if(trend_ana_depth_account=null) then
        trend_ana_depth_account:=trend_ana_depth;
      end if;
      
      begin
       select (importo/numberfatture) as media into media_precedente_fattura
            from (
                select sum(IMPT_TOT) as importo,count(*) as numberfatture from (
                                        select IMPT_TOT from I5_2ST_TEST_FT
                                        where data_creaz >= add_months(sysdate,trend_ana_depth_account*-1)
                                        and code_account=chiave.ID_KEY_VARCHAR
                                        ) y
                ) x;
      EXCEPTION
      WHEN NO_DATA_FOUND THEN
        media_precedente_fattura:=0;
      end;
      begin
       select (importo/numberfatture) as media into media_precedente_notacred
            from (
                select sum(IMPT_TOT) as importo,count(*) as numberfatture from (
                                        select IMPT_TOT from I5_2ST_TEST_NC
                                        where data_creaz > add_months(sysdate,trend_ana_depth_account*-1)
                                         and code_account=chiave.ID_KEY_VARCHAR
                                        ) y
                ) x;
      EXCEPTION
       WHEN NO_DATA_FOUND THEN
        media_precedente_notacred:=0;
      end;
      
        begin
        select IMPT_TOT_FATTURA into importo_fattura from I5_2TEST_DOCUM_FATT_XDSL
                                                      where code_account=chiave.ID_KEY_VARCHAR
                                                      and tipo_flag_funzione_creaz_impt='V';
        EXCEPTION
      WHEN NO_DATA_FOUND THEN
        importo_fattura:=0;
      end;
 begin
        select IMPT_TOT_NOTA_CRED into importo_notacred from I5_2TEST_DOCUM_NOTA_CRED_XDSL
                                                      where code_account=chiave.ID_KEY_VARCHAR
                                                      and tipo_flag_funzione_creaz_impt='V';
                                                      
   EXCEPTION
   WHEN NO_DATA_FOUND THEN
        importo_notacred:=0;
      end;
        
        
         dis_perc_fattura:=abs(importo_fattura-media_precedente_fattura) *100/media_precedente_fattura;
         dis_perc_notacred:= abs(importo_notacred-media_precedente_notacred) *100/media_precedente_notacred;
         dis_perc_tot:= abs(abs(importo_fattura-importo_notacred)-abs(media_precedente_fattura-media_precedente_notacred)) *100/(abs(media_precedente_fattura-media_precedente_notacred));
         
        if (dis_perc_fattura> trend_ana_threshold_account or 
            dis_perc_notacred > trend_ana_threshold_account or
            dis_perc_tot > trend_ana_threshold_account) then
             
             insert into WP1_ANALISI_TREND
             (ID_ELAB,CODE_ACCOUNT,IMPORTO_FT_CORRENTE,
             IMPORTO_NC_CORRENTE,MEDIA_FT,MEDIA_NC,PERC_DIFF_FAT,
             PERC_DIFF_NC,PERC_DIFF_TOT)
             values(
                    ts,
                    chiave.ID_KEY_VARCHAR,
                    importo_fattura,
                    importo_notacred,
                    media_precedente_fattura,
                    media_precedente_notacred,
                    dis_perc_fattura,
                    dis_perc_notacred,
                    dis_perc_tot);
         end if;
         
         
         
      End Loop;          
  RETURN 0;
  END ANALISI_TREND;
  
END PKG_WP1_ANALISI_TREND;
/

spool off;
