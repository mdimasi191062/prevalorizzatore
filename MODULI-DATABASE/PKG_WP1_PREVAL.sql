spool 'PKG_WP1_PREVAL.log';

CREATE OR REPLACE PACKAGE PKG_WP1_PREVAL AS 

  function CAMBIO_CICLO(ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER;
    
  function COND_OL_TO_VAL_OPERATORI_CES1 (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER;
    
  function COND_OL_TO_VAL_OPERATORI_CES2 (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER;  
    
  
  function COND_OL_TO_VAL_OPERATORI_CES3 (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER;
 
 function COND_OL_TO_VAL_OPERATORI_CES4 (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER;
 
 function COND_OL_TO_VAL_OPERATORI_CES5 (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER;
  
 function COND_OL_TO_VAL_OPERATORI_CES6 (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER;

 
 function COND_OL_DOPPI_SERV_REGOL_NWLR (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER;
    
 function COND_OL_DOPPI_SERV_REGOL_OWLR (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER;   
    
 function COND_OL_DOPPI_NP_CPS (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER;      
 
 function COND_OL_DOPPI_NP_XDSL (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER;         
  
 function COND_OL_CESS_SERV_NP_WLR (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER;        

 function COND_CODPROGCESS_WIN1106WLR (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER;
  
 function COND_ALLIN_DATA_ACQ_REG (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER;   
     function OP_ALLIN_DATA_ACQ_REG (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER;      
 
 function COND_ALLIN_DATA_ACQ_XDSL (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER;      
 function OP_ALLIN_DATA_ACQ_XDSL (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER;      
function COND_EASYIP_SP_OP_NON_MIGR (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER;      


  function STORICIZZA_XDSL (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_OPERATION IN varchar2) 
     return NUMBER;

  function STORICIZZA_REG (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_OPERATION IN varchar2) 
     return NUMBER;

  function STORICIZZA_NPCPS (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_OPERATION IN varchar2) 
     return NUMBER;

  function STORICIZZA_SCARTI (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_OPERATION IN varchar2) 
     return NUMBER;

END PKG_WP1_PREVAL;
/


CREATE OR REPLACE package body PKG_WP1_PREVAL AS 

-- ========== 
-- ========== INIZIO BLOCCO Accantonamento degli OL da non valorizzare relativi agli operatori cessati.
-- ========== 
function CAMBIO_CICLO (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER is
    
    lstr_codeTipoContr varchar2(10) ;
    ldat_dataInizioCicloFatrz date ;
    ldat_dataFineCicloFatrz date ;

    Cursor curListaAccount Is 
                Select a.code_account
                        , a.data_inizio_valid
                    From i5_1account a
                            , i5_1account_x_contr b
                            , i5_1contr c
                    Where c.code_tipo_contr = lstr_codeTipoContr
                            And c.flag_sys = 'S'
                            And b.code_contr = c.code_contr
                            And b.flag_sys = c.flag_sys
                            And a.code_account = b.code_account
                            And a.flag_sys = b.flag_sys
                            And sysdate >= a.data_inizio_valid;
    begin
    
    begin
      select to_date(PARAM_VALUE,'dd/mm/yyyy') into ldat_dataInizioCicloFatrz from WP1JOB_TEMP where PARAM_NAME = '$DATA_INIZIO_CICLO$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $DATA_INIZIO_CICLO$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $DATA_INIZIO_CICLO$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
   begin
      select PARAM_VALUE into lstr_codeTipoContr from WP1JOB_TEMP where PARAM_NAME = '$CODE_TIPO_CONTR$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CODE_TIPO_CONTR$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CODE_TIPO_CONTR$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
   ldat_dataFineCicloFatrz:=ADD_MONTHS(ldat_dataInizioCicloFatrz,1)-1;
    
    For recAccount In curListaAccount
    Loop
        Declare
            lint_nrParamValoriz number := 0;
        Begin
            --dbms_output.put_line('Controllo l''account ' || recAccount.code_account);

            -- Per ogni account trovato si verifica l'esistenza di una riga nella tabella i5_2param_valoriz_sp
            Select Count(*)
                Into lint_nrParamValoriz
                From i5_2param_valoriz_sp
                Where code_account = recAccount.code_account;
            If (lint_nrParamValoriz = 0) Then
                Begin
                    -- Inserimento riga nella tabella i5_2param_valoriz_sp
                    Insert Into i5_2param_valoriz_sp (code_param, code_account, code_elab
                                                        , data_inizio_ciclo_fatrz, data_fine_ciclo_fatrz, data_inizio_periodo
                                                        , data_fine_periodo, data_cong, valo_nr_scarti_nb
                                                        , tipo_flag_err_blocc, tipo_flag_stato_cong, flag_sys)
                                                    Values (i5q21_param_valoriz_sp.nextval, recAccount.code_account, Null
                                                                , ldat_dataInizioCicloFatrz, ldat_dataFineCicloFatrz, recAccount.data_inizio_valid
                                                                , Null, Null, Null
                                                                , ' ', 'N', 'S');
                     INSERT INTO WP1_TABLE_KEY (WP1_TABLE_KEY.ID_KEY_VARCHAR) 
                     values ( i5q21_param_valoriz_sp.currval);
                    --dbms_output.put_line('Inserisco param per l''account ' || recAccount.code_account);
                End;
            Else
                Declare
                    lrec_paramValoriz i5_2param_valoriz_sp%ROWTYPE;
                Begin
                    Select *
                        Into lrec_paramValoriz
                        From i5_2param_valoriz_sp a
                        Where a.code_account = recAccount.code_account
                                And a.data_cong Is Null
                                And a.data_inizio_ciclo_fatrz < ldat_dataInizioCicloFatrz;

                    Update i5_2param_valoriz_sp
                        Set data_inizio_ciclo_fatrz = ldat_dataInizioCicloFatrz
                                , data_fine_ciclo_fatrz = ldat_dataFineCicloFatrz
                        Where code_param = lrec_paramValoriz.code_param;
                    INSERT INTO WP1_TABLE_KEY (WP1_TABLE_KEY.ID_KEY_VARCHAR) 
                     values ( lrec_paramValoriz.code_param);
                    --dbms_output.put_line('Modifico param per l''account ' || recAccount.code_account);
                Exception
                    When no_data_found Then
                        Null;
                End;
            End If;
        End;
    End Loop;
    
    return 0;
    
end CAMBIO_CICLO;
    
function COND_OL_TO_VAL_OPERATORI_CES1 (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER is

-- Parametri dalla temp table 
v_code_contr i5_1contr.code_tipo_contr%type;
ris_0 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_P I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_Z I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
v_scarto i5_5scarti_valorizzazione.stato_scarto%type:= 'A';
v_flag_sys i5_1contr.flag_sys%type := 'S';

begin

-- Prelevo i parametri dalla tabella temporanea WP1JOB_TEMP

  -- $CODE_TIPO_CONTR$
  begin
      select PARAM_VALUE into v_code_contr from WP1JOB_TEMP where PARAM_NAME = '$CODE_TIPO_CONTR$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CODE_TIPO_CONTR$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CODE_TIPO_CONTR$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
  -- $RIS_NON_ACQ$
  begin
      select PARAM_VALUE into ris_0 from WP1JOB_TEMP where PARAM_NAME = '$RIS_NON_ACQ$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_NON_ACQ$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_NON_ACQ$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
   -- $RIS_BLOCK$
  begin
      select PARAM_VALUE into ris_P from WP1JOB_TEMP where PARAM_NAME = '$RIS_BLOCK$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_BLOCK$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_BLOCK$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;

 -- $RIS_Z$
  begin
      select PARAM_VALUE into ris_Z from WP1JOB_TEMP where PARAM_NAME = '$RIS_Z$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_Z$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_Z$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
  INSERT INTO WP1_TABLE_KEY (WP1_TABLE_KEY.ID_KEY_NUMBER) 
    select 
       i5_5scarti_valorizzazione.SEQ_SCARTO 
    from i5_5scarti_valorizzazione
    where 
        stato_scarto = v_scarto
    and code_contr in (Select code_contr From i5_1contr 
                        Where code_tipo_contr = v_code_contr 
                        and flag_sys = v_flag_sys)
    and (code_itrf_fat_xdsl_jpub, flag_prov, desc_id_risorsa, code_contr) in
    (select 
               b.code_itrf_fat, b.flag_prov, b.desc_id_risorsa, b.code_contr
     from 
        i5_2account_cessati_sp a, 
        i5_5itrf_fat b
     where b.code_contr = a.code_contr and b.tipo_flag_acq_rich in (ris_0, ris_P, ris_Z)
  );
    
  return 0;
end COND_OL_TO_VAL_OPERATORI_CES1;


function COND_OL_TO_VAL_OPERATORI_CES2 (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER is

-- Parametri dalla temp table 
v_code_contr i5_1contr.code_tipo_contr%type;
ris_0 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_P I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_Z I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
--v_scarto i5_5scarti_valorizzazione.stato_scarto%type:= 'A';
v_flag_sys i5_1contr.flag_sys%type := 'S';

begin

-- Prelevo i parametri dalla tabella temporanea WP1JOB_TEMP

  -- $CODE_TIPO_CONTR$
  begin
      select PARAM_VALUE into v_code_contr from WP1JOB_TEMP where PARAM_NAME = '$CODE_TIPO_CONTR$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CODE_TIPO_CONTR$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CODE_TIPO_CONTR$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
  -- $RIS_NON_ACQ$
  begin
      select PARAM_VALUE into ris_0 from WP1JOB_TEMP where PARAM_NAME = '$RIS_NON_ACQ$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_NON_ACQ$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_NON_ACQ$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
   -- $RIS_BLOCK$
  begin
      select PARAM_VALUE into ris_P from WP1JOB_TEMP where PARAM_NAME = '$RIS_BLOCK$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_BLOCK$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_BLOCK$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;

 -- $RIS_Z$
  begin
      select PARAM_VALUE into ris_Z from WP1JOB_TEMP where PARAM_NAME = '$RIS_Z$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_Z$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_Z$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;

  INSERT INTO WP1_TABLE_KEY (WP1_TABLE_KEY.ID_KEY_VARCHAR,WP1_TABLE_KEY.ID_KEY2_VARCHAR) 
  select 
      i5_5itrf_fat.CODE_ITRF_FAT, i5_5itrf_fat.FLAG_PROV 
   from i5_5itrf_fat    
   where code_contr in 
            (Select code_contr From i5_1contr Where code_tipo_contr = v_code_contr and flag_sys = v_flag_sys)
        and (code_itrf_fat, flag_prov) in 
            (select b.code_itrf_fat, b.flag_prov from i5_2account_cessati_sp a, i5_5itrf_fat b
             where b.code_contr = a.code_contr and b.tipo_flag_acq_rich in (ris_0, ris_P, ris_Z));
             
   
return 0;
end COND_OL_TO_VAL_OPERATORI_CES2;


function COND_OL_TO_VAL_OPERATORI_CES3 (ID_TICKET IN varchar2, ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
   return NUMBER is



-- Parametri dalla temp table 
v_code_contr i5_1contr.code_tipo_contr%type;
ris_0 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_P I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_Z I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
v_scarto i5_5scarti_valorizzazione.stato_scarto%type:= 'A';
v_flag_sys i5_1contr.flag_sys%type := 'S';

begin

-- Prelevo i parametri dalla tabella temporanea WP1JOB_TEMP

  -- $CODE_TIPO_CONTR$
  begin
      select PARAM_VALUE into v_code_contr from WP1JOB_TEMP where PARAM_NAME = '$CODE_TIPO_CONTR$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CODE_TIPO_CONTR$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CODE_TIPO_CONTR$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
  -- $RIS_NON_ACQ$
  begin
      select PARAM_VALUE into ris_0 from WP1JOB_TEMP where PARAM_NAME = '$RIS_NON_ACQ$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_NON_ACQ$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_NON_ACQ$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
   -- $RIS_BLOCK$
  begin
      select PARAM_VALUE into ris_P from WP1JOB_TEMP where PARAM_NAME = '$RIS_BLOCK$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_BLOCK$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_BLOCK$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;

 -- $RIS_Z$
  begin
      select PARAM_VALUE into ris_Z from WP1JOB_TEMP where PARAM_NAME = '$RIS_Z$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_Z$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_Z$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;

  INSERT INTO WP1_TABLE_KEY (WP1_TABLE_KEY.ID_KEY_NUMBER) 
    select i5_5scarti_valorizzazione.SEQ_SCARTO from i5_5scarti_valorizzazione
     where 
          stato_scarto = v_scarto
      and code_contr in (Select code_contr From i5_1contr Where code_tipo_contr = v_code_contr and flag_sys = v_flag_sys)
      and (code_itrf_fat_xdsl_jpub, flag_prov, desc_id_risorsa, code_contr) in
          (select b.code_itrf_fat_xdsl, b.flag_prov, b.desc_id_risorsa, b.code_contr
           from i5_2account_cessati_sp a, 
                i5_5itrf_fat_xdsl b
           where 
                 b.code_contr = a.code_contr and b.tipo_flag_acq_rich in (ris_0, ris_P, ris_Z));
    
return 0;
end COND_OL_TO_VAL_OPERATORI_CES3;

function COND_OL_TO_VAL_OPERATORI_CES4 (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
  return NUMBER is


-- Parametri dalla temp table 
v_code_contr i5_1contr.code_tipo_contr%type;
ris_0 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_P I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_Z I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
v_flag_sys i5_1contr.flag_sys%type := 'S';

begin

-- Prelevo i parametri dalla tabella temporanea WP1JOB_TEMP

  -- $CODE_TIPO_CONTR$
  begin
      select PARAM_VALUE into v_code_contr from WP1JOB_TEMP where PARAM_NAME = '$CODE_TIPO_CONTR$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CODE_TIPO_CONTR$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CODE_TIPO_CONTR$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
  -- $RIS_NON_ACQ$
  begin
      select PARAM_VALUE into ris_0 from WP1JOB_TEMP where PARAM_NAME = '$RIS_NON_ACQ$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_NON_ACQ$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_NON_ACQ$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
   -- $RIS_BLOCK$
  begin
      select PARAM_VALUE into ris_P from WP1JOB_TEMP where PARAM_NAME = '$RIS_BLOCK$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_BLOCK$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_BLOCK$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;

 -- $RIS_Z$
  begin
      select PARAM_VALUE into ris_Z from WP1JOB_TEMP where PARAM_NAME = '$RIS_Z$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_Z$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_Z$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
   
      INSERT INTO WP1_TABLE_KEY (WP1_TABLE_KEY.ID_KEY_VARCHAR) 
      select i5_5itrf_fat_xdsl.code_itrf_fat_xdsl 
      from i5_5itrf_fat_xdsl
     where 
          code_contr in (Select code_contr From i5_1contr Where code_tipo_contr = v_code_contr and flag_sys = v_flag_sys)
      and (code_itrf_fat_xdsl, flag_prov) in ( select 
                                          b.code_itrf_fat_xdsl, 
                                          b.flag_prov 
                                          from 
                                          i5_2account_cessati_sp a, 
                                          i5_5itrf_fat_xdsl b
                                          where 
                                          b.code_contr = a.code_contr and 
                                          b.tipo_flag_acq_rich in (ris_0, ris_P, ris_Z));
   
   
return 0;
end COND_OL_TO_VAL_OPERATORI_CES4;


function COND_OL_TO_VAL_OPERATORI_CES5 (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
  return  NUMBER is


-- Parametri dalla temp table 
v_code_contr i5_1contr.code_tipo_contr%type;
ris_0 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_P I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_Z I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
v_scarto i5_5scarti_valorizzazione.stato_scarto%type:= 'A';
v_flag_sys i5_1contr.flag_sys%type := 'S';

begin

-- Prelevo i parametri dalla tabella temporanea WP1JOB_TEMP

  -- $CODE_TIPO_CONTR$
  begin
      select PARAM_VALUE into v_code_contr from WP1JOB_TEMP where PARAM_NAME = '$CODE_TIPO_CONTR$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CODE_TIPO_CONTR$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CODE_TIPO_CONTR$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
  -- $RIS_NON_ACQ$
  begin
      select PARAM_VALUE into ris_0 from WP1JOB_TEMP where PARAM_NAME = '$RIS_NON_ACQ$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_NON_ACQ$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_NON_ACQ$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
   -- $RIS_BLOCK$
  begin
      select PARAM_VALUE into ris_P from WP1JOB_TEMP where PARAM_NAME = '$RIS_BLOCK$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_BLOCK$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_BLOCK$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;

 -- $RIS_Z$
  begin
      select PARAM_VALUE into ris_Z from WP1JOB_TEMP where PARAM_NAME = '$RIS_Z$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_Z$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_Z$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;

  INSERT INTO WP1_TABLE_KEY (WP1_TABLE_KEY.ID_KEY_NUMBER) 
   select 
       i5_5scarti_valorizzazione.SEQ_SCARTO from i5_5scarti_valorizzazione
   where 
           stato_scarto = v_scarto
       and code_contr in (Select code_contr From i5_1contr Where code_tipo_contr = v_code_contr and flag_sys = v_flag_sys)
       and (code_itrf_fat_xdsl_jpub, flag_prov, desc_id_risorsa, code_contr) in
           (select 
            b.code_itrf_fat, 
            b.flag_prov, 
            b.desc_id_risorsa, 
            b.code_contr
            from i5_2account_cessati_sp a, 
            i5_4itrf_fat_np_cps b
            where b.code_contr = a.code_contr and b.tipo_flag_acq_rich in (ris_0, ris_P, ris_Z));
   
   
return 0;
end COND_OL_TO_VAL_OPERATORI_CES5;


function COND_OL_TO_VAL_OPERATORI_CES6 (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
  return NUMBER is


-- Parametri dalla temp table 
v_code_contr i5_1contr.code_tipo_contr%type;
ris_0 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_P I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_Z I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
v_flag_sys i5_1contr.flag_sys%type := 'S';

begin

-- Prelevo i parametri dalla tabella temporanea WP1JOB_TEMP

  -- $CODE_TIPO_CONTR$
  begin
      select PARAM_VALUE into v_code_contr from WP1JOB_TEMP where PARAM_NAME = '$CODE_TIPO_CONTR$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CODE_TIPO_CONTR$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CODE_TIPO_CONTR$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
  -- $RIS_NON_ACQ$
  begin
      select PARAM_VALUE into ris_0 from WP1JOB_TEMP where PARAM_NAME = '$RIS_NON_ACQ$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_NON_ACQ$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_NON_ACQ$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
   -- $RIS_BLOCK$
  begin
      select PARAM_VALUE into ris_P from WP1JOB_TEMP where PARAM_NAME = '$RIS_BLOCK$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_BLOCK$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_BLOCK$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;

 -- $RIS_Z$
  begin
      select PARAM_VALUE into ris_Z from WP1JOB_TEMP where PARAM_NAME = '$RIS_Z$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_Z$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_Z$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;

  INSERT INTO WP1_TABLE_KEY (WP1_TABLE_KEY.ID_KEY_VARCHAR) 
  select  i5_4itrf_fat_np_cps.CODE_ITRF_FAT 
  from
      i5_4itrf_fat_np_cps    
  where code_contr in (Select code_contr From i5_1contr Where code_tipo_contr = v_code_contr and flag_sys = v_flag_sys)
       and (code_itrf_fat, flag_prov) in (select b.code_itrf_fat_xdsl, 
                                                 b.flag_prov
                                          from i5_2account_cessati_sp a, i5_5itrf_fat_xdsl b
                                          where b.code_contr = a.code_contr and 
                                          b.tipo_flag_acq_rich in (ris_0, ris_P, ris_Z));
 
return 0;
end COND_OL_TO_VAL_OPERATORI_CES6;
-- ========== 
-- ========== FINE BLOCCO Accantonamento degli OL da non valorizzare relativi agli operatori cessati.
-- ========== 

-- ========== 
-- ========== INIZIO BLOCCO Accantonamento degli OL doppi per i servizi regolamentati.
-- ========== 
function COND_OL_DOPPI_SERV_REGOL_NWLR (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
   return NUMBER is


-- Parametri dalla temp table 
v_code_contr i5_1contr.code_tipo_contr%type;
ris_0 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_P I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_Z I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
v_scarto i5_5scarti_valorizzazione.stato_scarto%type:= 'A';
v_flag_sys i5_1contr.flag_sys%type := 'S';
ldat_startDataAcqChius date;
cursor istanze_da_analizzare is
                Select q.code_contr, q.desc_id_risorsa, count(*)
                    From i5_5itrf_fat q
                    Where q.tipo_flag_acq_rich in (ris_0, ris_P)
                            and q.data_acq_chius >= ldat_startDataAcqChius
          and code_contr in (Select code_contr From i5_1contr Where code_tipo_contr = v_code_contr and flag_sys = 'S')
                            and ((q.code_tipo_contr = '41' and q.code_tipo_caus not in ('W1','W2'))
                                    or (q.code_tipo_contr != '41'))
                    Group By q.code_contr, q.desc_id_risorsa
                    Having count(*) > 1;

begin

-- Prelevo i parametri dalla tabella temporanea WP1JOB_TEMP

  -- $CODE_TIPO_CONTR$
  begin
      select PARAM_VALUE into v_code_contr from WP1JOB_TEMP where PARAM_NAME = '$CODE_TIPO_CONTR$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CODE_TIPO_CONTR$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CODE_TIPO_CONTR$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
  -- $RIS_NON_ACQ$
  begin
      select PARAM_VALUE into ris_0 from WP1JOB_TEMP where PARAM_NAME = '$RIS_NON_ACQ$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_NON_ACQ$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_NON_ACQ$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
   -- $RIS_BLOCK$
  begin
      select PARAM_VALUE into ris_P from WP1JOB_TEMP where PARAM_NAME = '$RIS_BLOCK$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_BLOCK$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_BLOCK$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;

 -- $DATA_INIZIO_PERIODO$
  begin
      select TO_DATE(PARAM_VALUE,'DD/MM/YYYY') into ldat_startDataAcqChius from WP1JOB_TEMP where PARAM_NAME = '$DATA_INIZIO_PERIODO$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $DATA_INIZIO_PERIODO$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $DATA_INIZIO_PERIODO$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  


  For recordIstDaAna In istanze_da_analizzare
    Loop
        Declare
            Cursor eventi_da_controllare is
                        Select *
                            From i5_5itrf_fat q
                            Where q.tipo_flag_acq_rich in (ris_0, ris_p)
                                    and q.code_contr = recordIstDaAna.code_contr
                                    and q.desc_id_risorsa = recordIstDaAna.desc_id_risorsa
                                    and q.code_tipo_caus not in ('W1','W2')
                            Order By q.code_tipo_contr, q.code_tipo_prest, q.code_contr, 
                                     q.desc_id_risorsa, q.code_caus_ull, 
                                     q.code_ps_fatt, q.code_tipo_caus, q.data_dest, q.flag_prov;
            lbln_isPrimoRecord boolean;
            lstr_chiaveDaControllarePrec varchar2(1500);
            lstr_chiaveDaControllare varchar2(1500);
        Begin
            lstr_chiaveDaControllarePrec := ' ';
            lbln_isPrimoRecord := true;
            For recordEveDaCon In eventi_da_controllare
            Loop
                lstr_chiaveDaControllare := recordEveDaCon.code_tipo_contr
                                                || recordEveDaCon.code_tipo_prest
                                                || recordEveDaCon.code_contr
                                                || recordEveDaCon.desc_id_risorsa
                                                || recordEveDaCon.code_caus_ull
                                                || recordEveDaCon.code_ps_fatt
                                                || recordEveDaCon.code_tipo_caus
                                                || to_char(recordEveDaCon.data_dest, 'yyyymmdd')
                                                || recordEveDaCon.flag_prov;

                --dbms_output.put_line('lstr_chiaveDaControllare: ' || lstr_chiaveDaControllare);

                If (lbln_isPrimoRecord) Then
                    lbln_isPrimoRecord := false;
                    lstr_chiaveDaControllarePrec := lstr_chiaveDaControllare;
                Else 
                    If (lstr_chiaveDaControllarePrec = lstr_chiaveDaControllare) Then
                        --dbms_output.put_line('lstr_chiaveDaControllare: ' || lstr_chiaveDaControllare);
                        --dbms_output.put_line('Record doppio metto la D.');
                         
                         --out_rec.ID_KEY_VARCHAR := recordEveDaCon.code_itrf_fat;
                         --out_rec.ID_KEY2_VARCHAR := recordEveDaCon.flag_prov;
                         INSERT INTO WP1_TABLE_KEY (ID_KEY_VARCHAR,ID_KEY2_VARCHAR) values (recordEveDaCon.code_itrf_fat,recordEveDaCon.flag_prov); 
                       
                    Else
                        lstr_chiaveDaControllarePrec := lstr_chiaveDaControllare;
                    End If;
                End If;
            End Loop;

            --dbms_output.put_line(' ');

        End;
    End Loop;

    return 0;
end COND_OL_DOPPI_SERV_REGOL_NWLR;

function COND_OL_DOPPI_SERV_REGOL_OWLR (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
  return  NUMBER is


-- Parametri dalla temp table 
v_code_contr i5_1contr.code_tipo_contr%type;
ris_0 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_P I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_Z I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
v_scarto i5_5scarti_valorizzazione.stato_scarto%type:= 'A';
v_flag_sys i5_1contr.flag_sys%type := 'S';
ldat_startDataAcqChius date;
cursor istanze_da_analizzare is
          select q.code_contr, q.desc_id_risorsa, count(*)
          From i5_5itrf_fat q
          Where q.tipo_flag_acq_rich in (ris_0, ris_P)
                and code_contr in (Select code_contr From i5_1contr Where code_tipo_contr = v_code_contr and flag_sys = 'S')
                and q.data_acq_chius >= ldat_startDataAcqChius
                and (q.code_tipo_contr = '41' and q.code_tipo_caus in ('W1','W2'))
          Group By q.code_contr, q.desc_id_risorsa
          Having count(*) > 1;

begin

-- Prelevo i parametri dalla tabella temporanea WP1JOB_TEMP

  -- $CODE_TIPO_CONTR$
  begin
      select PARAM_VALUE into v_code_contr from WP1JOB_TEMP where PARAM_NAME = '$CODE_TIPO_CONTR$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CODE_TIPO_CONTR$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CODE_TIPO_CONTR$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
  -- $RIS_NON_ACQ$
  begin
      select PARAM_VALUE into ris_0 from WP1JOB_TEMP where PARAM_NAME = '$RIS_NON_ACQ$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_NON_ACQ$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_NON_ACQ$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
   -- $RIS_BLOCK$
  begin
      select PARAM_VALUE into ris_P from WP1JOB_TEMP where PARAM_NAME = '$RIS_BLOCK$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_BLOCK$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_BLOCK$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;

 -- $DATA_INIZIO_PERIODO$
  begin
      select TO_DATE(PARAM_VALUE,'DD/MM/YYYY') into ldat_startDataAcqChius from WP1JOB_TEMP where PARAM_NAME = '$DATA_INIZIO_PERIODO$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $DATA_INIZIO_PERIODO$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $DATA_INIZIO_PERIODO$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
  For recordIstDaAna In istanze_da_analizzare
    Loop
        Declare
            Cursor eventi_da_controllare is
                        Select *
                            From i5_5itrf_fat q
                            Where q.tipo_flag_acq_rich in (ris_0, ris_P)
                                    and q.code_contr = recordIstDaAna.code_contr
                                    and q.desc_id_risorsa = recordIstDaAna.desc_id_risorsa
                                    and q.code_tipo_caus in ('W1','W2')
                            Order By q.code_tipo_contr, q.code_tipo_prest, q.code_contr, q.desc_id_risorsa, 
                            q.code_caus_ull, q.code_ps_fatt, q.code_tipo_caus, q.code_ps_sts, q.data_dest, q.flag_prov;
            lbln_isPrimoRecord boolean;
            lstr_chiaveDaControllarePrec varchar2(1500);
            lstr_chiaveDaControllare varchar2(1500);
        Begin
            lstr_chiaveDaControllarePrec := ' ';
            lbln_isPrimoRecord := true;
            For recordEveDaCon In eventi_da_controllare
            Loop
                lstr_chiaveDaControllare := recordEveDaCon.code_tipo_contr
                                                || recordEveDaCon.code_tipo_prest
                                                || recordEveDaCon.code_contr
                                                || recordEveDaCon.desc_id_risorsa
                                                || recordEveDaCon.code_caus_ull
                                                || recordEveDaCon.code_ps_fatt
                                                || recordEveDaCon.code_tipo_caus
                                                || recordEveDaCon.code_ps_sts
                                                || to_char(recordEveDaCon.data_dest, 'yyyymmdd')
                                                || recordEveDaCon.flag_prov;

                --dbms_output.put_line('lstr_chiaveDaControllare: ' || lstr_chiaveDaControllare);

                If (lbln_isPrimoRecord) Then
                    lbln_isPrimoRecord := false;
                    lstr_chiaveDaControllarePrec := lstr_chiaveDaControllare;
                Else
                    If (lstr_chiaveDaControllarePrec = lstr_chiaveDaControllare) Then
                        --dbms_output.put_line('Record doppio metto la D.');
                         --out_rec.ID_KEY_VARCHAR := recordEveDaCon.code_itrf_fat;
                         --out_rec.ID_KEY2_VARCHAR := recordEveDaCon.flag_prov;
                         
                         INSERT INTO WP1_TABLE_KEY (ID_KEY_VARCHAR,ID_KEY2_VARCHAR) values (recordEveDaCon.code_itrf_fat,recordEveDaCon.flag_prov); 
                         
                    Else
                        lstr_chiaveDaControllarePrec := lstr_chiaveDaControllare;
                    End If;
                End If;
            End Loop;

            --dbms_output.put_line(' ');

        End;
    End Loop;

    return 0;
end COND_OL_DOPPI_SERV_REGOL_OWLR;



-- ========== 
-- ========== FINE BLOCCO Accantonamento degli OL doppi per i servizi regolamentati.
-- ========== 

-- ========== 
-- ========== INIZIO BLOCCO Accantonamento degli OL doppi per i servizi Np e Cps.
-- ========== 
function COND_OL_DOPPI_NP_CPS (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
  return  NUMBER is


-- Parametri dalla temp table 
v_code_contr i5_1contr.code_tipo_contr%type;
ris_0 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_P I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_Z I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
v_flag_sys i5_1contr.flag_sys%type := 'S';
ldat_startDataAcqChius date;

cursor istanze_da_analizzare is
                Select q.code_contr, q.desc_id_risorsa, count(*)
                    From i5_4itrf_fat_np_cps q
                    Where q.tipo_flag_acq_rich in (ris_0, ris_P)
          and code_contr in (Select code_contr From i5_1contr Where code_tipo_contr = v_code_contr and flag_sys = 'S')
                            and q.data_acq_chius >= ldat_startDataAcqChius
                    Group By q.code_contr, q.desc_id_risorsa
                    Having count(*) > 1;


begin

-- Prelevo i parametri dalla tabella temporanea WP1JOB_TEMP

  -- $CODE_TIPO_CONTR$
  begin
      select PARAM_VALUE into v_code_contr from WP1JOB_TEMP where PARAM_NAME = '$CODE_TIPO_CONTR$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CODE_TIPO_CONTR$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CODE_TIPO_CONTR$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
  -- $RIS_NON_ACQ$
  begin
      select PARAM_VALUE into ris_0 from WP1JOB_TEMP where PARAM_NAME = '$RIS_NON_ACQ$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_NON_ACQ$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_NON_ACQ$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
   -- $RIS_BLOCK$
  begin
      select PARAM_VALUE into ris_P from WP1JOB_TEMP where PARAM_NAME = '$RIS_BLOCK$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_BLOCK$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_BLOCK$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;

 -- $DATA_INIZIO_PERIODO$
  begin
      select TO_DATE(PARAM_VALUE,'DD/MM/YYYY') into ldat_startDataAcqChius from WP1JOB_TEMP where PARAM_NAME = '$DATA_INIZIO_PERIODO$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $DATA_INIZIO_PERIODO$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $DATA_INIZIO_PERIODO$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
   
                         
  For recordIstDaAna In istanze_da_analizzare
    Loop
        Declare
            Cursor eventi_da_controllare is
                        Select *
                            From i5_4itrf_fat_np_cps q
                            Where q.tipo_flag_acq_rich in (ris_0, ris_P)
                                    and q.code_contr = recordIstDaAna.code_contr
                                    and q.desc_id_risorsa = recordIstDaAna.desc_id_risorsa
                            Order By q.code_contr, q.desc_id_risorsa, q.code_caus, q.code_ps_fatt, q.data_dest;
            lbln_isPrimoRecord boolean;
            lstr_chiaveDaControllarePrec varchar2(1500);
            lstr_chiaveDaControllare varchar2(1500);
        Begin
            lstr_chiaveDaControllarePrec := ' ';
            lbln_isPrimoRecord := true;
            For recordEveDaCon In eventi_da_controllare
            Loop
                lstr_chiaveDaControllare := recordEveDaCon.code_contr
                                                || recordEveDaCon.desc_id_risorsa
                                                || recordEveDaCon.code_caus
                                                || recordEveDaCon.code_ps_fatt
                                                || to_char(recordEveDaCon.data_dest, 'yyyymmdd');

                --dbms_output.put_line('lstr_chiaveDaControllare: ' || lstr_chiaveDaControllare);

                If (lbln_isPrimoRecord) Then
                    lbln_isPrimoRecord := false;
                    lstr_chiaveDaControllarePrec := lstr_chiaveDaControllare;
                Else
                    If (lstr_chiaveDaControllarePrec = lstr_chiaveDaControllare) Then
                        --dbms_output.put_line('lstr_chiaveDaControllare: ' || lstr_chiaveDaControllare);
                        --dbms_output.put_line('Record doppio metto la D.');
                        
                        
                        INSERT INTO WP1_TABLE_KEY (ID_KEY_VARCHAR) values (recordEveDaCon.code_itrf_fat);
                        
                    Else
                        lstr_chiaveDaControllarePrec := lstr_chiaveDaControllare;
                    End If;
                End If;
            End Loop;

            --dbms_output.put_line(' ');

        End;
    End Loop;                       
  
    return 0;
end COND_OL_DOPPI_NP_CPS;

-- ========== 
-- ========== FINE BLOCCO Accantonamento degli OL doppi per i servizi Np e Cps.
-- ========== 

-- ========== 
-- ========== INIZIIO BLOCCO Accantonamento degli OL doppi per i servizi Xdsl.
-- ========== 
--  ==========  FUNZIONALITA' IN SOSPESO DA MODIFICARE UPDATE PARTICOLARE
  function COND_OL_DOPPI_NP_XDSL (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
     return NUMBER is
  
  
  -- Parametri dalla temp table 
  v_code_contr i5_1contr.code_tipo_contr%type;
  ris_0 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
  ris_P I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
  ris_Z I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
  v_flag_sys i5_1contr.flag_sys%type := 'S';
  ldat_startDataAcqChius date;
  
  cursor istanze_da_analizzare is
              Select q.code_contr, q.desc_id_risorsa, count(*)
                      From i5_5itrf_fat_xdsl q
                      Where q.tipo_flag_acq_rich in (ris_0, ris_P)
                      and code_contr in (Select code_contr From i5_1contr Where code_tipo_contr = v_code_contr and flag_sys ='S')
                              and q.data_acq_chius >= ldat_startDataAcqChius
                      Group By q.code_contr, q.desc_id_risorsa
                      Having count(*) > 1;                    
  begin
  
  -- Prelevo i parametri dalla tabella temporanea WP1JOB_TEMP
  
    -- $CODE_TIPO_CONTR$
    begin
        select PARAM_VALUE into v_code_contr from WP1JOB_TEMP where PARAM_NAME = '$CODE_TIPO_CONTR$';
    EXCEPTION 
      WHEN NO_DATA_FOUND THEN
          RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CODE_TIPO_CONTR$ non presente nella tabella di lavoro WP1JOB_TEMP');         
      WHEN OTHERS THEN    
          RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CODE_TIPO_CONTR$ dalla tabella di lavoro WP1JOB_TEMP'); 
    end;
    
    -- $RIS_NON_ACQ$
    begin
        select PARAM_VALUE into ris_0 from WP1JOB_TEMP where PARAM_NAME = '$RIS_NON_ACQ$';
    EXCEPTION 
      WHEN NO_DATA_FOUND THEN
          RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_NON_ACQ$ non presente nella tabella di lavoro WP1JOB_TEMP');         
      WHEN OTHERS THEN    
          RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_NON_ACQ$ dalla tabella di lavoro WP1JOB_TEMP'); 
    end;
    
     -- $RIS_BLOCK$
    begin
        select PARAM_VALUE into ris_P from WP1JOB_TEMP where PARAM_NAME = '$RIS_BLOCK$';
    EXCEPTION 
      WHEN NO_DATA_FOUND THEN
          RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_BLOCK$ non presente nella tabella di lavoro WP1JOB_TEMP');         
      WHEN OTHERS THEN    
          RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_BLOCK$ dalla tabella di lavoro WP1JOB_TEMP'); 
    end;
  
   -- $DATA_INIZIO_PERIODO$
    begin
        select TO_DATE(PARAM_VALUE,'DD/MM/YYYY') into ldat_startDataAcqChius from WP1JOB_TEMP where PARAM_NAME = '$DATA_INIZIO_PERIODO$';
    EXCEPTION 
      WHEN NO_DATA_FOUND THEN
          RAISE_APPLICATION_ERROR(-20000, 'Paramtro $DATA_INIZIO_PERIODO$ non presente nella tabella di lavoro WP1JOB_TEMP');         
      WHEN OTHERS THEN    
          RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $DATA_INIZIO_PERIODO$ dalla tabella di lavoro WP1JOB_TEMP'); 
    end;
      
    For recordIstDaAna In istanze_da_analizzare
      Loop
          Declare
              Cursor eventi_da_controllare is
                          Select *
                              From i5_5itrf_fat_xdsl q
                              Where q.tipo_flag_acq_rich in (ris_0, ris_P)
                                      and q.code_contr = recordIstDaAna.code_contr
                                      and q.desc_id_risorsa = recordIstDaAna.desc_id_risorsa
                              Order By q.code_tipo_contr, q.code_tipo_prest, q.code_contr, q.code_rich_xdsl_ti, 
                              q.desc_id_risorsa, q.code_caus_xdsl, q.code_ps_fatt, q.code_tipo_caus_variaz, q.data_dro, q.flag_prov;
              lbln_isPrimoRecord boolean;
              lstr_chiaveDaControllarePrec varchar2(1500);
              lstr_chiaveDaControllare varchar2(1500);
          Begin
              lstr_chiaveDaControllarePrec := ' ';
              lbln_isPrimoRecord := true;
              For recordEveDaCon In eventi_da_controllare
              Loop
                  lstr_chiaveDaControllare := recordEveDaCon.code_tipo_contr
                                                  || recordEveDaCon.code_tipo_prest
                                                  || recordEveDaCon.code_contr
                                                  || recordEveDaCon.code_rich_xdsl_ti
                                                  || '-' || recordEveDaCon.desc_id_risorsa || '-'
                                                  || recordEveDaCon.code_caus_xdsl
                                                  || recordEveDaCon.code_ps_fatt
                                                  || recordEveDaCon.code_tipo_caus_variaz
                                                  || to_char(recordEveDaCon.data_dro, 'yyyymmdd')
                                                  || recordEveDaCon.flag_prov;
  
                  --dbms_output.put_line('lstr_chiaveDaControllare: ' || lstr_chiaveDaControllare);
  
                  If (lbln_isPrimoRecord) Then
                      lbln_isPrimoRecord := false;
                      lstr_chiaveDaControllarePrec := lstr_chiaveDaControllare;
                  Else
                      If (lstr_chiaveDaControllarePrec = lstr_chiaveDaControllare) Then
                          --dbms_output.put_line('lstr_chiaveDaControllare: ' || lstr_chiaveDaControllare);
                          --dbms_output.put_line('Record doppio metto la D.');
      
                          --out_rec.ID_KEY_VARCHAR := recordEveDaCon.code_itrf_fat_xdsl;
                          --out_rec.ID_KEY2_VARCHAR := recordEveDaCon.flag_prov;
                          
                          /*Update i5_5itrf_fat_xdsl
                            Set tipo_flag_acq_rich = 'D'
                            Where code_itrf_fat_xdsl = recordEveDaCon.code_itrf_fat_xdsl
                                    and flag_prov = recordEveDaCon.flag_prov;*/
                          
                          INSERT INTO WP1_TABLE_KEY (ID_KEY_VARCHAR) values (recordEveDaCon.code_itrf_fat_xdsl);
                          
                          /*INSERT INTO WP1_TABLE_KEY (ID_KEY_VARCHAR)  select 
                                                                        i5_5itrf_fat_xdsl.code_itrf_fat_xdsl 
                                                                      from i5_5itrf_fat_xdsl
                                                                      Where 
                                                                            code_itrf_fat_xdsl = recordEveDaCon.code_itrf_fat_xdsl
                                                                       and flag_prov = recordEveDaCon.flag_prov;
                          */
                      Else
                          lstr_chiaveDaControllarePrec := lstr_chiaveDaControllare;
                      End If;
                  End If;
              End Loop;
  
              --dbms_output.put_line(' ');
  
          End;
          
      End Loop;
    
  
      return 0;
  end COND_OL_DOPPI_NP_XDSL;
-- ========== 
-- ========== FINE BLOCCO Accantonamento degli OL doppi per i servizi Xdsl.
-- ========== 

-- ========== 
-- ========== INIZIO BLOCCO Accantonamento degli OL del servizio Np che sono da valorizzare anche per il Wlr.
-- ========== 

function COND_OL_CESS_SERV_NP_WLR (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
     return NUMBER is
  
  
 -- Parametri dalla temp table 
v_code_contr i5_1contr.code_tipo_contr%type;
ris_0 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_P I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_Z I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
v_scarto i5_5scarti_valorizzazione.stato_scarto%type:= 'A';
v_flag_sys i5_1contr.flag_sys%type := 'S';
v_data_inizio_periodo  date;

begin

-- Prelevo i parametri dalla tabella temporanea WP1JOB_TEMP

  -- $CODE_TIPO_CONTR$
  begin
      select PARAM_VALUE into v_code_contr from WP1JOB_TEMP where PARAM_NAME = '$CODE_TIPO_CONTR$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CODE_TIPO_CONTR$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CODE_TIPO_CONTR$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
  -- $RIS_NON_ACQ$
  begin
      select PARAM_VALUE into ris_0 from WP1JOB_TEMP where PARAM_NAME = '$RIS_NON_ACQ$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_NON_ACQ$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_NON_ACQ$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
   -- $RIS_BLOCK$
  begin
      select PARAM_VALUE into ris_P from WP1JOB_TEMP where PARAM_NAME = '$RIS_BLOCK$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_BLOCK$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_BLOCK$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
  -- $DATA_INIZIO_PERIODO$
  begin
      select TO_DATE(PARAM_VALUE,'DD/MM/YYYY') into v_data_inizio_periodo from WP1JOB_TEMP where PARAM_NAME = '$DATA_INIZIO_PERIODO$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $DATA_INIZIO_PERIODO$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $DATA_INIZIO_PERIODO$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;

  
  INSERT INTO WP1_TABLE_KEY (WP1_TABLE_KEY.ID_KEY_VARCHAR) 
  select 
    i5_4itrf_fat_np_cps.CODE_ITRF_FAT
  from
    i5_4itrf_fat_np_cps
  where 
      i5_4itrf_fat_np_cps.code_ps_fatt != '250' -- CPS
  and i5_4itrf_fat_np_cps.DATA_ACQ_CHIUS >= v_data_inizio_periodo
  and i5_4itrf_fat_np_cps.code_contr in (Select code_contr From i5_1contr Where code_tipo_contr = v_code_contr and flag_sys = 'S')
  and i5_4itrf_fat_np_cps.tipo_flag_acq_rich in (ris_0,ris_P)
  and i5_4itrf_fat_np_cps.desc_id_risorsa in (select distinct (a.desc_id_risorsa)
                          from i5_4itrf_fat_np_cps  a,i5_5itrf_fat b
                          where a.desc_id_risorsa = b.desc_id_risorsa
                          and  b.code_tipo_contr = '41'
                          and  a.code_ps_fatt != '250' -- CPS
                          and a.tipo_flag_acq_rich in (ris_0,ris_P)
                          and b.code_caus_ull = '1'
                          and b.tipo_flag_acq_rich in (ris_0,ris_P)
                          and a.DATA_ACQ_CHIUS >= v_data_inizio_periodo
                          and b.DATA_ACQ_CHIUS >= v_data_inizio_periodo);
    
  return 0;
  end COND_OL_CESS_SERV_NP_WLR;
-- ========== 
-- ========== FINE BLOCCO Accantonamento degli OL del servizio Np che sono da valorizzare anche per il Wlr.
-- ==========

-- ========== 
-- ========== INIZIO BLOCCO Aggiornamento del campo codice_progetto sugli OL di cessazione del servizio Wlr su risorse attivate con il campo codice_progetto uguale a WIN1106WLR
-- ==========

function COND_CODPROGCESS_WIN1106WLR (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
     return NUMBER is
  
  
 -- Parametri dalla temp table 
v_code_contr i5_1contr.code_tipo_contr%type;
ris_0 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_P I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_Z I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_1 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
v_scarto i5_5scarti_valorizzazione.stato_scarto%type:= 'A';
v_flag_sys i5_1contr.flag_sys%type := 'S';
v_data_inizio_periodo  date;

begin

-- Prelevo i parametri dalla tabella temporanea WP1JOB_TEMP

  -- $CODE_TIPO_CONTR$
  /*begin NON SERVE
      select PARAM_VALUE into v_code_contr from WP1JOB_TEMP where PARAM_NAME = '$CODE_TIPO_CONTR$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CODE_TIPO_CONTR$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CODE_TIPO_CONTR$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  */
  
  -- $RIS_NON_ACQ$
  begin
      select PARAM_VALUE into ris_0 from WP1JOB_TEMP where PARAM_NAME = '$RIS_NON_ACQ$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_NON_ACQ$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_NON_ACQ$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
   -- $RIS_BLOCK$
  begin
      select PARAM_VALUE into ris_P from WP1JOB_TEMP where PARAM_NAME = '$RIS_BLOCK$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_BLOCK$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_BLOCK$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
  -- $RIS_ACQ$
  begin
      select PARAM_VALUE into ris_1 from WP1JOB_TEMP where PARAM_NAME = '$RIS_ACQ$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_NON_ACQ$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_NON_ACQ$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
  -- $DATA_INIZIO_PERIODO$
  begin
      select TO_DATE(PARAM_VALUE,'DD/MM/YYYY') into v_data_inizio_periodo from WP1JOB_TEMP where PARAM_NAME = '$DATA_INIZIO_PERIODO$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $DATA_INIZIO_PERIODO$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $DATA_INIZIO_PERIODO$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;

  
  INSERT INTO WP1_TABLE_KEY (WP1_TABLE_KEY.ID_KEY_VARCHAR,WP1_TABLE_KEY.ID_KEY2_VARCHAR) 
    select i5_5itrf_fat.CODE_ITRF_FAT,i5_5itrf_fat.FLAG_PROV
    from i5_5itrf_fat
    where tipo_flag_acq_rich in (ris_0,ris_P)
    and code_contr = '3111'
    and data_acq_chius >= v_data_inizio_periodo
    and (code_itrf_fat, flag_prov) in (select b.code_itrf_fat,b.flag_prov
    from i5_5itrf_fat a
    , i5_5itrf_fat b
    where a.desc_id_risorsa = b.desc_id_risorsa
    and a.code_contr = b.code_contr
    and a.code_ps_fatt = b.code_ps_fatt
    and a.tipo_flag_acq_rich = ris_1
    and b.tipo_flag_acq_rich in (ris_0,ris_P)
    and a.code_caus_ull = '1'
    and b.code_caus_ull = '2'
    and a.data_dest <= b.data_dest
    and a.code_contr = '3111'
    and a.codice_progetto = 'WIN1106WLR'
    and a.code_tipo_contr = b.code_tipo_contr);


    
  return 0;
  end COND_CODPROGCESS_WIN1106WLR;
-- ========== 
-- ========== FINE BLOCCO Aggiornamento del campo codice_progetto sugli OL di cessazione del servizio Wlr su risorse attivate con il campo codice_progetto uguale a WIN1106WLR
-- ==========

-- ========== 
-- ========== INIZIO BLOCCO Allineamento della data_acq_chius degli OL aventi piu ordini da valorizzare per i servizi regolamentati.
-- ==========
-- ========== DA COMPLETARE!!! PROBLEMI NELLA GESTIONE DELLE CHIAVI E DELLE OPERAZIONI
-- ========== IN SOSPESO
function COND_ALLIN_DATA_ACQ_REG (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
     return NUMBER is
  
  
 -- Parametri dalla temp table 
v_code_contr i5_1contr.code_tipo_contr%type;
ris_0 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_P I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_Z I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_1 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
v_scarto i5_5scarti_valorizzazione.stato_scarto%type:= 'A';
v_flag_sys i5_1contr.flag_sys%type := 'S';
ldat_startDataAcqChius date;

Cursor istanze_da_analizzare is
                Select q.code_contr, q.desc_id_risorsa, count(*)
                    From i5_5itrf_fat q
                    Where 
                        q.tipo_flag_acq_rich in (ris_0, ris_P)
                    and code_contr in (Select code_contr From i5_1contr Where code_tipo_contr = v_code_contr and flag_sys = 'S')
                    and q.data_acq_chius >= ldat_startDataAcqChius
                    Group By q.code_contr, q.desc_id_risorsa
                    Having count(*) > 1;

begin

-- Prelevo i parametri dalla tabella temporanea WP1JOB_TEMP

  -- $CODE_TIPO_CONTR$
  begin 
      select PARAM_VALUE into v_code_contr from WP1JOB_TEMP where PARAM_NAME = '$CODE_TIPO_CONTR$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CODE_TIPO_CONTR$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CODE_TIPO_CONTR$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
  
  -- $RIS_NON_ACQ$
  begin
      select PARAM_VALUE into ris_0 from WP1JOB_TEMP where PARAM_NAME = '$RIS_NON_ACQ$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_NON_ACQ$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_NON_ACQ$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
   -- $RIS_BLOCK$
  begin
      select PARAM_VALUE into ris_P from WP1JOB_TEMP where PARAM_NAME = '$RIS_BLOCK$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_BLOCK$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_BLOCK$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
  
  -- $DATA_INIZIO_PERIODO$
  begin
      select TO_DATE(PARAM_VALUE,'DD/MM/YYYY') into ldat_startDataAcqChius from WP1JOB_TEMP where PARAM_NAME = '$DATA_INIZIO_PERIODO$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $DATA_INIZIO_PERIODO$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $DATA_INIZIO_PERIODO$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
    For recordIstDaAna In istanze_da_analizzare
    Loop
        Declare
            Cursor eventi_da_allineare is
                        Select *
                            From i5_5itrf_fat q
                            Where q.tipo_flag_acq_rich in (ris_0, ris_P)
                                    and q.code_contr = recordIstDaAna.code_contr
                                    and q.desc_id_risorsa = recordIstDaAna.desc_id_risorsa
                            Order By q.code_tipo_contr, q.code_contr, q.desc_id_risorsa, q.data_dro, q.data_dest, to_number(q.code_itrf_fat);
            lbln_isPrimoRecord boolean;
            ldat_newDataAcqChius date;
        Begin
            lbln_isPrimoRecord := true;
            For recordEveDaAll In eventi_da_allineare
            Loop
                
                   
                 INSERT INTO WP1_TABLE_KEY (ID_KEY_VARCHAR,ID_KEY2_VARCHAR) 
                 values (recordEveDaAll.code_itrf_fat,recordEveDaAll.flag_prov);
                  
                
            End Loop;

            /*
            dbms_output.put_line(' ');
            */
        End;
    End Loop;


    
  return 0;
  end COND_ALLIN_DATA_ACQ_REG;



function OP_ALLIN_DATA_ACQ_REG (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
     return NUMBER is
  
  
 -- Parametri dalla temp table 
v_code_contr i5_1contr.code_tipo_contr%type;
ris_0 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_P I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_Z I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_1 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
v_scarto i5_5scarti_valorizzazione.stato_scarto%type:= 'A';
v_flag_sys i5_1contr.flag_sys%type := 'S';
ldat_startDataAcqChius date;

Cursor istanze_da_analizzare is
                Select q.code_contr, q.desc_id_risorsa
                    From i5_5itrf_fat q
                    Where 
                        (code_itrf_fat,flag_prov) in (select  ID_KEY_VARCHAR,ID_KEY2_VARCHAR from  WP1_TABLE_KEY )
                    Group By q.code_contr, q.desc_id_risorsa;

begin

-- Prelevo i parametri dalla tabella temporanea WP1JOB_TEMP

  -- $CODE_TIPO_CONTR$
  begin 
      select PARAM_VALUE into v_code_contr from WP1JOB_TEMP where PARAM_NAME = '$CODE_TIPO_CONTR$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CODE_TIPO_CONTR$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CODE_TIPO_CONTR$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
  
  -- $RIS_NON_ACQ$
  begin
      select PARAM_VALUE into ris_0 from WP1JOB_TEMP where PARAM_NAME = '$RIS_NON_ACQ$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_NON_ACQ$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_NON_ACQ$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
   -- $RIS_BLOCK$
  begin
      select PARAM_VALUE into ris_P from WP1JOB_TEMP where PARAM_NAME = '$RIS_BLOCK$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_BLOCK$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_BLOCK$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
  
  -- $DATA_INIZIO_PERIODO$
  begin
      select TO_DATE(PARAM_VALUE,'DD/MM/YYYY') into ldat_startDataAcqChius from WP1JOB_TEMP where PARAM_NAME = '$DATA_INIZIO_PERIODO$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $DATA_INIZIO_PERIODO$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $DATA_INIZIO_PERIODO$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
    For recordIstDaAna In istanze_da_analizzare
    Loop
        Declare
            Cursor eventi_da_allineare is
                        Select *
                            From i5_5itrf_fat q
                            Where q.tipo_flag_acq_rich in (ris_0, ris_P)
                                    and q.code_contr = recordIstDaAna.code_contr
                                    and q.desc_id_risorsa = recordIstDaAna.desc_id_risorsa
                            Order By q.code_tipo_contr, q.code_contr, q.desc_id_risorsa, q.data_dro, q.data_dest, to_number(q.code_itrf_fat);
            lbln_isPrimoRecord boolean;
            ldat_newDataAcqChius date;
        Begin
            lbln_isPrimoRecord := true;
            For recordEveDaAll In eventi_da_allineare
            Loop
                If (lbln_isPrimoRecord) Then
                    lbln_isPrimoRecord := false;
                    If (recordEveDaAll.data_dest > ldat_startDataAcqChius) Then
                        ldat_newDataAcqChius := recordEveDaAll.data_dest;
                    Else
                        ldat_newDataAcqChius := ldat_startDataAcqChius;
                    End If;
                Else
                    If (recordEveDaAll.data_dest > ldat_newDataAcqChius) Then
                        ldat_newDataAcqChius := recordEveDaAll.data_dest;
                    Else
                        ldat_newDataAcqChius := ldat_newDataAcqChius +1;
                    End If;
                End If;
                    
                       Update i5_5itrf_fat
                    Set data_acq_chius = ldat_newDataAcqChius
                    Where code_itrf_fat = recordEveDaAll.code_itrf_fat
                            and flag_prov = recordEveDaAll.flag_prov;
                  
                
            End Loop;

            /*
            dbms_output.put_line(' ');
            */
        End;
    End Loop;


    
  return 0;
  end OP_ALLIN_DATA_ACQ_REG;
  -- ========== 
  -- ========== FINE BLOCCO Allineamento della data_acq_chius degli OL aventi piu ordini da valorizzare per i servizi regolamentati.
  -- ==========
  
-- ========== 
-- ========== INIZIO BLOCCO Posticipazione degli OL del servizio BS EasyIp Special Profile appartenenti ad operatori non migrati o da verificare.
-- ==========  

function COND_EASYIP_SP_OP_NON_MIGR (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
     return NUMBER is
  
  
 -- Parametri dalla temp table 
v_code_contr i5_1contr.code_tipo_contr%type;
ris_0 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_P I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_Z I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_1 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
v_scarto i5_5scarti_valorizzazione.stato_scarto%type:= 'A';
v_flag_sys i5_1contr.flag_sys%type := 'S';
v_data_inizio_periodo  date;

begin

-- Prelevo i parametri dalla tabella temporanea WP1JOB_TEMP

 
  
  -- $RIS_NON_ACQ$
  begin
      select PARAM_VALUE into ris_0 from WP1JOB_TEMP where PARAM_NAME = '$RIS_NON_ACQ$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_NON_ACQ$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_NON_ACQ$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
 
  -- $DATA_INIZIO_PERIODO$
  begin
      select TO_DATE(PARAM_VALUE,'DD/MM/YYYY') into v_data_inizio_periodo from WP1JOB_TEMP where PARAM_NAME = '$DATA_INIZIO_PERIODO$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $DATA_INIZIO_PERIODO$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $DATA_INIZIO_PERIODO$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;

  
  INSERT INTO WP1_TABLE_KEY (WP1_TABLE_KEY.ID_KEY_VARCHAR) 
    select 
          i5_5itrf_fat_xdsl.CODE_ITRF_FAT_XDSL
    from i5_5itrf_fat_xdsl
    Where 
        code_tipo_contr = '48'
    and tipo_flag_acq_rich = ris_0
    and (code_itrf_fat_xdsl, flag_prov) in (
            SELECT a.code_itrf_fat_xdsl, a.flag_prov
            FROM 
              I5_5ITRF_FAT_XDSL a
              , i5_1contr b
           WHERE  b.code_contr = a.code_contr
              and b.flag_sys = 'S'
              and b.code_tipo_contr = '48'
              and tipo_flag_acq_rich = ris_0 
    and data_acq_chius >= v_data_inizio_periodo
    and b.CODE_CONTR not IN (SELECT CODE_CONTR
    FROM OLO_VAL_EASYIPBS_SP_NOV_2013)
    GROUP BY a.code_itrf_fat_xdsl, a.flag_prov
    );


    
  return 0;
  end COND_EASYIP_SP_OP_NON_MIGR;
-- ========== 
-- ========== FINE BLOCCO Posticipazione degli OL del servizio BS EasyIp Special Profile appartenenti ad operatori non migrati o da verificare.
-- ==========  

-- ========== 
-- ========== INIZIO BLOCCO  Allineamento della data_acq_chius degli OL aventi piu ordini da valorizzare per i servizi xdsl. (IN SOSPESP PER UPFATE PARTICOLARE)
-- ==========  

function OP_ALLIN_DATA_ACQ_XDSL (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
     return NUMBER is
  
  
 -- Parametri dalla temp table 
v_code_contr i5_1contr.code_tipo_contr%type;
ris_0 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_P I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_Z I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_1 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
v_scarto i5_5scarti_valorizzazione.stato_scarto%type:= 'A';
v_flag_sys i5_1contr.flag_sys%type := 'S';
ldat_startDataAcqChius date;

Cursor istanze_da_analizzare is
                Select q.code_contr, q.desc_id_risorsa, count(*)
                    From i5_5itrf_fat_xdsl q
                    Where q.code_itrf_fat_xdsl in (select ID_KEY_VARCHAR from  WP1_TABLE_KEY)
                    Group By q.code_contr, q.desc_id_risorsa;

begin

-- Prelevo i parametri dalla tabella temporanea WP1JOB_TEMP

 -- $CODE_TIPO_CONTR$
  begin 
      select PARAM_VALUE into v_code_contr from WP1JOB_TEMP where PARAM_NAME = '$CODE_TIPO_CONTR$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CODE_TIPO_CONTR$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CODE_TIPO_CONTR$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
  -- $RIS_NON_ACQ$
  begin
      select PARAM_VALUE into ris_0 from WP1JOB_TEMP where PARAM_NAME = '$RIS_NON_ACQ$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_NON_ACQ$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_NON_ACQ$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
 
  -- $DATA_INIZIO_PERIODO$
  begin
      select TO_DATE(PARAM_VALUE,'DD/MM/YYYY') into ldat_startDataAcqChius from WP1JOB_TEMP where PARAM_NAME = '$DATA_INIZIO_PERIODO$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $DATA_INIZIO_PERIODO$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $DATA_INIZIO_PERIODO$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;

  
  For recordIstDaAna In istanze_da_analizzare
    Loop
        Declare
            Cursor eventi_da_allineare is
                        Select *
                            From i5_5itrf_fat_xdsl q
                            Where q.tipo_flag_acq_rich in (ris_0, ris_P)
                                    and q.code_contr = recordIstDaAna.code_contr
                                    and q.desc_id_risorsa = recordIstDaAna.desc_id_risorsa
                            Order By q.code_tipo_contr, q.code_contr, q.desc_id_risorsa, q.data_dro, q.data_dest, to_number(q.code_itrf_fat_xdsl);
            lbln_isPrimoRecord boolean;
            ldat_newDataAcqChius date;
        Begin
            lbln_isPrimoRecord := true;
            For recordEveDaAll In eventi_da_allineare
            Loop
                If (lbln_isPrimoRecord) Then
                    lbln_isPrimoRecord := false;
                    If (recordEveDaAll.data_dest > ldat_startDataAcqChius) Then
                        ldat_newDataAcqChius := recordEveDaAll.data_dest;
                    Else
                        ldat_newDataAcqChius := ldat_startDataAcqChius;
                    End If;
                Else
                    If (recordEveDaAll.data_dest > ldat_newDataAcqChius) Then
                        ldat_newDataAcqChius := recordEveDaAll.data_dest;
                    Else
                        ldat_newDataAcqChius := ldat_newDataAcqChius +1;
                    End If;
                End If;

                /*
                dbms_output.put_line('CodeContr: ' || recordEveDaAll.code_contr
                                            || ' - DescIdRisorsa: ' || recordEveDaAll.desc_id_risorsa
                                            || ' - CodeCausXdsl: ' || recordEveDaAll.code_caus_xdsl
                                            || ' - DataDro: ' || recordEveDaAll.data_dro
                                            || ' - DataDest: ' || recordEveDaAll.data_dest
                                            || ' - DataAcqChius: ' || recordEveDaAll.data_acq_chius
                                            || ' - NewDataAcqChius: ' || ldat_newDataAcqChius);
                */

                
                Update i5_5itrf_fat_xdsl
                    Set data_acq_chius = ldat_newDataAcqChius
                    Where code_itrf_fat_xdsl = recordEveDaAll.code_itrf_fat_xdsl
                            and flag_prov = recordEveDaAll.flag_prov;  
                            
                          
            End Loop;

            /*
            dbms_output.put_line(' ');
            */
        End;
    End Loop;


    
  return 0;
  end OP_ALLIN_DATA_ACQ_XDSL;

function COND_ALLIN_DATA_ACQ_XDSL (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
     return NUMBER is
  
  
 -- Parametri dalla temp table 
v_code_contr i5_1contr.code_tipo_contr%type;
ris_0 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_P I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_Z I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
ris_1 I5_5ITRF_FAT.TIPO_FLAG_ACQ_RICH%type;
v_scarto i5_5scarti_valorizzazione.stato_scarto%type:= 'A';
v_flag_sys i5_1contr.flag_sys%type := 'S';
ldat_startDataAcqChius date;

Cursor istanze_da_analizzare is
                Select q.code_contr, q.desc_id_risorsa, count(*)
                    From i5_5itrf_fat_xdsl q
                    Where q.tipo_flag_acq_rich in (ris_0,ris_P)
                    and code_contr in (Select code_contr From i5_1contr Where code_tipo_contr = v_code_contr and flag_sys = 'S')
                    and q.data_acq_chius >= ldat_startDataAcqChius
                    Group By q.code_contr, q.desc_id_risorsa
                    Having count(*) > 1;

begin

-- Prelevo i parametri dalla tabella temporanea WP1JOB_TEMP

 -- $CODE_TIPO_CONTR$
  begin 
      select PARAM_VALUE into v_code_contr from WP1JOB_TEMP where PARAM_NAME = '$CODE_TIPO_CONTR$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CODE_TIPO_CONTR$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CODE_TIPO_CONTR$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
  -- $RIS_NON_ACQ$
  begin
      select PARAM_VALUE into ris_0 from WP1JOB_TEMP where PARAM_NAME = '$RIS_NON_ACQ$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $RIS_NON_ACQ$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $RIS_NON_ACQ$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;
  
 
  -- $DATA_INIZIO_PERIODO$
  begin
      select TO_DATE(PARAM_VALUE,'DD/MM/YYYY') into ldat_startDataAcqChius from WP1JOB_TEMP where PARAM_NAME = '$DATA_INIZIO_PERIODO$';
  EXCEPTION 
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $DATA_INIZIO_PERIODO$ non presente nella tabella di lavoro WP1JOB_TEMP');         
    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $DATA_INIZIO_PERIODO$ dalla tabella di lavoro WP1JOB_TEMP'); 
  end;

  
  For recordIstDaAna In istanze_da_analizzare
    Loop
        Declare
            Cursor eventi_da_allineare is
                        Select *
                            From i5_5itrf_fat_xdsl q
                            Where q.tipo_flag_acq_rich in (ris_0, ris_P)
                                    and q.code_contr = recordIstDaAna.code_contr
                                    and q.desc_id_risorsa = recordIstDaAna.desc_id_risorsa
                            Order By q.code_tipo_contr, q.code_contr, q.desc_id_risorsa, q.data_dro, q.data_dest, to_number(q.code_itrf_fat_xdsl);
            lbln_isPrimoRecord boolean;
            ldat_newDataAcqChius date;
        Begin
            lbln_isPrimoRecord := true;
            For recordEveDaAll In eventi_da_allineare
            Loop
                If (lbln_isPrimoRecord) Then
                    lbln_isPrimoRecord := false;
                    If (recordEveDaAll.data_dest > ldat_startDataAcqChius) Then
                        ldat_newDataAcqChius := recordEveDaAll.data_dest;
                    Else
                        ldat_newDataAcqChius := ldat_startDataAcqChius;
                    End If;
                Else
                    If (recordEveDaAll.data_dest > ldat_newDataAcqChius) Then
                        ldat_newDataAcqChius := recordEveDaAll.data_dest;
                    Else
                        ldat_newDataAcqChius := ldat_newDataAcqChius +1;
                    End If;
                End If;

                /*
                dbms_output.put_line('CodeContr: ' || recordEveDaAll.code_contr
                                            || ' - DescIdRisorsa: ' || recordEveDaAll.desc_id_risorsa
                                            || ' - CodeCausXdsl: ' || recordEveDaAll.code_caus_xdsl
                                            || ' - DataDro: ' || recordEveDaAll.data_dro
                                            || ' - DataDest: ' || recordEveDaAll.data_dest
                                            || ' - DataAcqChius: ' || recordEveDaAll.data_acq_chius
                                            || ' - NewDataAcqChius: ' || ldat_newDataAcqChius);
                */

                
             
                INSERT INTO WP1_TABLE_KEY (ID_KEY_VARCHAR) values (recordEveDaAll.code_itrf_fat_xdsl);   
                            
                          
            End Loop;

            /*
            dbms_output.put_line(' ');
            */
        End;
    End Loop;


    
  return 0;
  end COND_ALLIN_DATA_ACQ_XDSL;
  
  function STORICIZZA_XDSL (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_OPERATION IN varchar2) 
     return NUMBER is
     ts int;
  begin
  begin       
	    select PARAM_VALUE into ts from WP1JOB_TEMP where PARAM_NAME = '$TS$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $TS$ nei parametri'); 
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $TS$ nei parametri'); 
      end;
    INSERT
        INTO WP1_I5_5TCKT_ITRF_FAT_XDSL
          (
            ID_TICKET,
            ID_JOB,
            ID_ELABORAZIONE,
            CODE_ITRF_FAT_XDSL,
            CODE_RICH_XDSL_TI,
            DESC_ID_RISORSA,
            CODE_TIPO_PREST,
            CODE_PREST_AGG,
            CODE_CAUS_XDSL,
            CODE_TIPO_CAUS_VARIAZ,
            CODE_COMUNE,
            DATA_DRO,
            DATA_DVTC,
            DATA_DEST,
            CODE_STATO_PS_XDSL,
            CODE_PS_FATT,
            DATA_ACQ_CHIUS,
            CODE_CONTR,
            TIPO_FLAG_ACQ_RICH,
            CODE_AREA_RACCOLTA,
            CODE_NUM_TD,
            CODE_LOTTO,
            CODE_PS_FATT_PRINC,
            VALO_MCR_LOTTO,
            VALO_PCR_LOTTO,
            DESC_CODE_VPI,
            FLAG_TRASPORTO,
            NUM_CLIENTI_LOTTO,
            MOD_FATTURAZIONE,
            TIPO_FLAG_SPLITTER,
            VALO_PCR_UP_VC,
            VALO_MCR_UP_VC,
            VALO_PCR_DOWN_VC,
            VALO_MCR_DOWN_VC,
            OPZ_OFF_CALC,
            TIPO_MODEM,
            CODE_TIPO_CONTR,
            MOD_FATTURAZIONE_TRASP,
            DESC_CODE_VPI_TRASP,
            TIPO_DOMINIO,
            MOD_VENDITA,
            TIPO_FAMIGLIA,
            OPZ_COMM,
            TIPO_PROT,
            TIPO_LINEA,
            SUBNET_EASYIP,
            TIPO_MODULAZ,
            MOD_ACCESSO,
            TIPO_VALID,
            FLAG_MODEM,
            CLASSE_SERV,
            SCR_UP_VC,
            SCR_DOWN_VC,
            TIPO_MOD_FAT,
            MCR_TRASP,
            BANDA_CVP,
            MOD_INDIR,
            MOD_CONS,
            CODE_ITRF_FAT_XDSL_PROV,
            FLAG_PROV,
            ID_ACCESSO,
            DATA_ACQUISIZIONE,
            OPZ_CONTRATTUALE,
            TIPO_PROVENIENZA,
            UTENZA_RIF,
            CODE_OL,
            DATA_INIZIO_NOL,
            TIPO_SLA_PLUS,
            OFFERTA_SLA_PLUS,
            COPERTURA_ORARIA,
            FLAG_DISP,
            CODICE_PROGETTO,
            VALO_PCR_UP_VC_OLD,
            VALO_PCR_DOWN_VC_OLD,
            FLAG_ESITO_PREQ,
            CODE_CLLI,
            ID_RIS_OLD,
            PROFILO_ACCESSO,
            CODE_PROFILO_ESTESO,
            CODE_MACRO_AREA,
            FLAG_PRIMA_ATTIV,
            COUNT_VLAN,
            CODE_IDBRE_DSLAM,
            FLAG_NUOVO_FEEDER,
            COUNT_FEEDER,
            ID_ORD_CRMWS,
            CODICE_QUALITA,
            CODICE_DELIVERY,
            CAMPO_SERV_IT,
            FLAG_MONITORAGGIO,
            TECNOLOGIA,
            VLAN_TIPO_BANDA,
            VLAN_ACCE_TIPO,
            VLAN_QUALITA,
            VLAN_CAR,
            ACCE_MODALITA,
            ID_CVLAN
          )
          SELECT 
         
          ID_TICKET,
          ID_JOB,
          ts,
          CODE_ITRF_FAT_XDSL,
          CODE_RICH_XDSL_TI,
          DESC_ID_RISORSA,
          CODE_TIPO_PREST,
          CODE_PREST_AGG,
          CODE_CAUS_XDSL,
          CODE_TIPO_CAUS_VARIAZ,
          CODE_COMUNE,
          DATA_DRO,
          DATA_DVTC,
          DATA_DEST,
          CODE_STATO_PS_XDSL,
          CODE_PS_FATT,
          DATA_ACQ_CHIUS,
          CODE_CONTR,
          TIPO_FLAG_ACQ_RICH,
          CODE_AREA_RACCOLTA,
          CODE_NUM_TD,
          CODE_LOTTO,
          CODE_PS_FATT_PRINC,
          VALO_MCR_LOTTO,
          VALO_PCR_LOTTO,
          DESC_CODE_VPI,
          FLAG_TRASPORTO,
          NUM_CLIENTI_LOTTO,
          MOD_FATTURAZIONE,
          TIPO_FLAG_SPLITTER,
          VALO_PCR_UP_VC,
          VALO_MCR_UP_VC,
          VALO_PCR_DOWN_VC,
          VALO_MCR_DOWN_VC,
          OPZ_OFF_CALC,
          TIPO_MODEM,
          CODE_TIPO_CONTR,
          MOD_FATTURAZIONE_TRASP,
          DESC_CODE_VPI_TRASP,
          TIPO_DOMINIO,
          MOD_VENDITA,
          TIPO_FAMIGLIA,
          OPZ_COMM,
          TIPO_PROT,
          TIPO_LINEA,
          SUBNET_EASYIP,
          TIPO_MODULAZ,
          MOD_ACCESSO,
          TIPO_VALID,
          FLAG_MODEM,
          CLASSE_SERV,
          SCR_UP_VC,
          SCR_DOWN_VC,
          TIPO_MOD_FAT,
          MCR_TRASP,
          BANDA_CVP,
          MOD_INDIR,
          MOD_CONS,
          CODE_ITRF_FAT_XDSL_PROV,
          FLAG_PROV,
          ID_ACCESSO,
          DATA_ACQUISIZIONE,
          OPZ_CONTRATTUALE,
          TIPO_PROVENIENZA,
          UTENZA_RIF,
          CODE_OL,
          DATA_INIZIO_NOL,
          TIPO_SLA_PLUS,
          OFFERTA_SLA_PLUS,
          COPERTURA_ORARIA,
          FLAG_DISP,
          CODICE_PROGETTO,
          VALO_PCR_UP_VC_OLD,
          VALO_PCR_DOWN_VC_OLD,
          FLAG_ESITO_PREQ,
          CODE_CLLI,
          ID_RIS_OLD,
          PROFILO_ACCESSO,
          CODE_PROFILO_ESTESO,
          CODE_MACRO_AREA,
          FLAG_PRIMA_ATTIV,
          COUNT_VLAN,
          CODE_IDBRE_DSLAM,
          FLAG_NUOVO_FEEDER,
          COUNT_FEEDER,
          ID_ORD_CRMWS,
          CODICE_QUALITA,
          CODICE_DELIVERY,
          CAMPO_SERV_IT,
          FLAG_MONITORAGGIO,
          TECNOLOGIA,
          VLAN_TIPO_BANDA,
          VLAN_ACCE_TIPO,
          VLAN_QUALITA,
          VLAN_CAR,
          ACCE_MODALITA,
          ID_CVLAN
          FROM I5_5ITRF_FAT_XDSL 
          where CODE_ITRF_FAT_XDSL in ( SELECT ID_KEY_VARCHAR from WP1_TABLE_KEY);
     
  return 0;
  end STORICIZZA_XDSL;


function STORICIZZA_REG (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_OPERATION IN varchar2) 
     return NUMBER is
     ts int;
 begin    
  begin       
	    select PARAM_VALUE into ts from WP1JOB_TEMP where PARAM_NAME = '$TS$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $TS$ nei parametri'); 
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $TS$ nei parametri'); 
      end;   
     
     INSERT
INTO WP1_I5_5TCKT_ITRF_FAT
  (
    ID_TICKET,
    ID_JOB,
    ID_ELABORAZIONE,
    CODE_ITRF_FAT,
    CODE_RICH_ULL_TI,
    DESC_ID_RISORSA,
    CODE_TIPO_PREST,
    CODE_PREST_AGG,
    CODE_CAUS_ULL,
    CODE_COMUNE,
    DATA_DRO,
    DATA_DVTC,
    DATA_DEST,
    CODE_STATO_PS_ULL,
    CODE_PS_FATT,
    DATA_ACQ_CHIUS,
    CODE_ID_ULLCO,
    CODE_CONTR,
    TIPO_FLAG_ACQ_RICH,
    CODE_TIPO_CONTR,
    QNTA_VALO,
    CODE_DISTR,
    CODE_TIPO_CAUS,
    DESC_ID_RISORSA_OLD,
    CODE_PS_FATT_OLD,
    CODE_GEST_REC,
    CODICE_PROGETTO,
    DATA_INIZIO_NOL,
    CODE_PS_STS,
    DESC_ID_RISORSA_PRIM,
    FLAG_LINEA_NUM_AGG,
    CODICE_QUALITA,
    CODICE_DELIVERY,
    CAMPO_SERV_IT,
    ID_ORD_CRMWS,
    FLAG_SPLITTER,
    FLAG_MIGRAZIONE,
    FLAG_QUALIFICAZIONE,
    OPZ_COMM,
    TIPO_LINEA,
    TIPO_FAMIGLIA,
    FLAG_PROV,
    SUB_TIPO_LINEA_RESIDENZIALE,
    SUB_TIPO_LINEA_DIREZIONE,
    CODE_STS,
    SUB_TIPO_LINEA,
    MOD_VENDITA,
    FLAG_PERMUTA,
    FLAG_NP,
    ID_RIS_AGGIUNTIVA,
    CODICE_CRM_PROV,
    CODE_CLLI,
    CODE_ID_SLUCO,
    FLAG_MANTENIMENTO_STS
  )
  
  SELECT 
  ID_TICKET,
  ID_JOB,
  ts,
  CODE_ITRF_FAT,
  CODE_RICH_ULL_TI,
  DESC_ID_RISORSA,
  CODE_TIPO_PREST,
  CODE_PREST_AGG,
  CODE_CAUS_ULL,
  CODE_COMUNE,
  DATA_DRO,
  DATA_DVTC,
  DATA_DEST,
  CODE_STATO_PS_ULL,
  CODE_PS_FATT,
  DATA_ACQ_CHIUS,
  CODE_ID_ULLCO,
  CODE_CONTR,
  TIPO_FLAG_ACQ_RICH,
  CODE_TIPO_CONTR,
  QNTA_VALO,
  CODE_DISTR,
  CODE_TIPO_CAUS,
  DESC_ID_RISORSA_OLD,
  CODE_PS_FATT_OLD,
  CODE_GEST_REC,
  CODICE_PROGETTO,
  DATA_INIZIO_NOL,
  CODE_PS_STS,
  DESC_ID_RISORSA_PRIM,
  FLAG_LINEA_NUM_AGG,
  CODICE_QUALITA,
  CODICE_DELIVERY,
  CAMPO_SERV_IT,
  ID_ORD_CRMWS,
  FLAG_SPLITTER,
  FLAG_MIGRAZIONE,
  FLAG_QUALIFICAZIONE,
  OPZ_COMM,
  TIPO_LINEA,
  TIPO_FAMIGLIA,
  FLAG_PROV,
  SUB_TIPO_LINEA_RESIDENZIALE,
  SUB_TIPO_LINEA_DIREZIONE,
  CODE_STS,
  SUB_TIPO_LINEA,
  MOD_VENDITA,
  FLAG_PERMUTA,
  FLAG_NP,
  ID_RIS_AGGIUNTIVA,
  CODICE_CRM_PROV,
  CODE_CLLI,
  CODE_ID_SLUCO,
  FLAG_MANTENIMENTO_STS
FROM I5_5ITRF_FAT  
where (CODE_ITRF_FAT,FLAG_PROV) in ( SELECT ID_KEY_VARCHAR,ID_KEY2_VARCHAR from WP1_TABLE_KEY);
     
  return 0;
  end STORICIZZA_REG;
  
  function STORICIZZA_NPCPS (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_OPERATION IN varchar2) 
     return NUMBER is
     ts int;
     begin
    begin       
	    select PARAM_VALUE into ts from WP1JOB_TEMP where PARAM_NAME = '$TS$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $TS$ nei parametri'); 
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $TS$ nei parametri'); 
      end;   
   INSERT
INTO WP1_I5_4TCKT_ITRF_FAT_NP_CPS
  (
    ID_TICKET,
    ID_JOB,
    ID_ELABORAZIONE,
    CODE_ITRF_FAT,
    DESC_ID_RISORSA,
    CODE_CAUS,
    DATA_DRO,
    DATA_DVTC,
    DATA_DEST,
    CODE_STATO_PS,
    CODE_PS_FATT,
    CODE_CONTR,
    CODE_DISTR,
    QNTA_VALO,
    TIPO_FLAG_ACQ_RICH,
    DESC_ID_RISORSA_PRINC,
    DATA_ACQ_CHIUS,
    CODE_CLLI,
    SUB_TIPO_LINEA,
    CODICE_ORDINE_ISP,
    ID_ORD_CRMWS,
    FLAG_PROV,
    CODICE_CRM_PROV,
    CODE_DONOR,
    CODE_DONATING
  )
 SELECT 
 ID_TICKET,
 ID_JOB,
 ts,
 CODE_ITRF_FAT,
  DESC_ID_RISORSA,
  CODE_CAUS,
  DATA_DRO,
  DATA_DVTC,
  DATA_DEST,
  CODE_STATO_PS,
  CODE_PS_FATT,
  CODE_CONTR,
  CODE_DISTR,
  QNTA_VALO,
  TIPO_FLAG_ACQ_RICH,
  DESC_ID_RISORSA_PRINC,
  DATA_ACQ_CHIUS,
  CODE_CLLI,
  SUB_TIPO_LINEA,
  CODICE_ORDINE_ISP,
  ID_ORD_CRMWS,
  FLAG_PROV,
  CODICE_CRM_PROV,
  CODE_DONOR,
  CODE_DONATING
FROM I5_4ITRF_FAT_NP_CPS 
where (CODE_ITRF_FAT) in ( SELECT ID_KEY_VARCHAR from WP1_TABLE_KEY);
     
  return 0;
  end STORICIZZA_NPCPS;
  
  
  function STORICIZZA_SCARTI (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_OPERATION IN varchar2) 
     return NUMBER is
     ts int;
     begin
     begin       
	    select PARAM_VALUE into ts from WP1JOB_TEMP where PARAM_NAME = '$TS$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $TS$ nei parametri'); 
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $TS$ nei parametri'); 
      end;   
     
   INSERT
INTO WP1_I5_5TCKT_SCARTI_VALORIZ
  (
    ID_TICKET,
    ID_JOB,
    ID_ELABORAZIONE,
    CODE_ITRF_FAT_XDSL_XML_RIF,
    CODE_SCARTO,
    DESC_SCARTO,
    DATA_SCARTO,
    DATA_CHIUSURA_SCARTO,
    STATO_SCARTO,
    DESC_ID_RISORSA,
    CODE_TIPO_CONTR,
    CODE_CONTR,
    CODE_TRACCIATO,
    CODE_DETT_SCARTO,
    CHIAVE_PITA_JPUB,
    FLAG_PROV,
    CODE_ITRF_FAT_XDSL_JPUB,
    SEQ_SCARTO,
    DATA_DRO
  )
  SELECT 
  ID_TICKET,
  ID_JOB,
  ts,
  CODE_ITRF_FAT_XDSL_XML_RIF,
  CODE_SCARTO,
  DESC_SCARTO,
  DATA_SCARTO,
  DATA_CHIUSURA_SCARTO,
  STATO_SCARTO,
  DESC_ID_RISORSA,
  CODE_TIPO_CONTR,
  CODE_CONTR,
  CODE_TRACCIATO,
  CODE_DETT_SCARTO,
  CHIAVE_PITA_JPUB,
  FLAG_PROV,
  CODE_ITRF_FAT_XDSL_JPUB,
  SEQ_SCARTO,
  DATA_DRO
FROM I5_5SCARTI_VALORIZZAZIONE  where (SEQ_SCARTO) in ( SELECT ID_KEY_NUMBER from WP1_TABLE_KEY);
  return 0;
  end STORICIZZA_SCARTI;
-- ========== 
-- ========== FINE BLOCCO  Allineamento della data_acq_chius degli OL aventi piu ordini da valorizzare per i servizi xdsl. (IN SOSPESP PER UPFATE PARTICOLARE)
-- ==========  

END PKG_WP1_PREVAL;
/

spool off;
