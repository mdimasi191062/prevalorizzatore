spool 'PKG_WP1_POSTVAL.log';

CREATE OR REPLACE PACKAGE "PKG_WP1_POSTVAL" IS

  function MODIFICA_NOTA_CRED (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER;
	  function MODIFICA_FATTURA (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER;
	function AGGIORNA_TESTATA_FATTURA (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_OPERATION IN varchar2) 
    return NUMBER;
	function AGGIORNA_TESTATA_NC (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_OPERATION IN varchar2) 
    return NUMBER;
	function STORICIZZA_FATTURA(ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_OPERATION IN varchar2) 
    return NUMBER;
	function STORICIZZA_NC(ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_OPERATION IN varchar2) 
    return NUMBER;
	FUNCTION GET_ACONET(ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2)  return number;
  FUNCTION MODIFICA_FATTURA_X_TIPO_CONTR(ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2)  return number;
   FUNCTION MODIFICA_NC_X_TIPO_CONTR(ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2)  return number;
END "PKG_WP1_POSTVAL";
/


CREATE OR REPLACE PACKAGE BODY "PKG_WP1_POSTVAL" IS


  
  function MODIFICA_NOTA_CRED (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER is
	/* PARAMETRI */
	 pstr_codeAccount varchar2(32767);
     pdat_dataInizioValid date;
     pbln_checkDataDro int;
     pdat_dataDaMin  date;
     pdat_dataDaMax   date;
 --    pstr_codiceProgetto  varchar2(32767);
	-- pstr_causali varchar2(32767);
	 --pstr_ogg_fatrz varchar2(32767);
  -- pstr_code_ps varchar2(32767);
 	/* FINE_PARAMETRI*/
  
        lstr_codeNotaCred i5_2test_docum_nota_cred_xdsl.code_nota_cred%TYPE;

        Cursor istAttDaAzzerare is
            Select x.*
                From i5_2dett_docum_nota_cred_xdsl x
                Where x.code_nota_cred = lstr_codeNotaCred
                        and ((pbln_checkDataDro = 0 and (x.data_da between pdat_dataDaMin and pdat_dataDaMax))
                                or (pbln_checkDataDro = 1));

        lrec_inventario i5_1invent_ps_xdsl%ROWTYPE;
       
        lint_quantiCodeOggFatrz number;
        lint_quanteCausali number;
        lint_quantiCodePs number;
        lint_quantiCodiciProgetto number;
        
        checkCodeOggFatrz int;
        checkCausali int;
        checkCodePs int;
        checkCodiciProgetto int;
    Begin
       
	   -- RICAVO I PARAMETRI
      begin 
      select PARAM_VALUE into pstr_codeAccount from WP1JOB_TEMP where PARAM_NAME = '$CODE_ACCOUNT$';
      EXCEPTION    
       WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CODE_ACCOUNT$ non presente nei parametri');  
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CODE_ACCOUNT$ nei parametri'); 
      end;
      
      begin 
      select to_date(PARAM_VALUE,'dd/mm/yyyy') into pdat_dataInizioValid from WP1JOB_TEMP where PARAM_NAME = '$DATA_INIZIO_VALID$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $DATA_INIZIO_VALID$ non presente nei parametri');
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $DATA_INIZIO_VALID$ nei parametri'); 
      end;
	   
     begin 
     select decode(PARAM_VALUE,'1',1,0) into pbln_checkDataDro from WP1JOB_TEMP where PARAM_NAME = '$CHECK_DATA_DRO$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CHECK_DATA_DRO$ non presente nei parametri');
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CHECK_DATA_DRO$ nei parametri'); 
      end;
     
	   begin 
      select to_date(PARAM_VALUE,'dd/mm/yyyy') into pdat_dataDaMin from WP1JOB_TEMP where PARAM_NAME = '$DATA_DA_MIN$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $DATA_DA_MIN$ non presente nei parametri');
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $DATA_DA_MIN$ nei parametri'); 
      end;
      
      begin 
      
	   select to_date(PARAM_VALUE,'dd/mm/yyyy') into pdat_dataDaMax from WP1JOB_TEMP where PARAM_NAME = '$DATA_DA_MAX$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $DATA_DA_MAX$ non presente nei parametri');
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $DATA_DA_MAX$ nei parametri'); 
      end;
	   
      
     begin 
      
	    select count(*) into checkCodiciProgetto from WP1JOB_TEMP where PARAM_NAME like '$LIST_CODICE_PROGETTO[%]$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        checkCodiciProgetto:=0;
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $LIST_CODICE_PROGETTO$ nei parametri'); 
      end;

      
       
      begin 
      
	  select count(*) into checkCausali from WP1JOB_TEMP where PARAM_NAME like '$LIST_CAUSALI[%]$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        checkCausali:=0;
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $LIST_CAUSALI$ dalla tabella di lavoro WP1JOB_TEMP'); 
      end;
	 
     begin 
      
	   select count(*) into checkCodeOggFatrz from WP1JOB_TEMP where PARAM_NAME like '$LIST_OGG_DI_FATRZ[%]$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        checkCodeOggFatrz:=0;
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $LIST_OGG_DI_FATRZ$ nei parametri'); 
      end;
	   
	  begin 
      
	   select count(*) into checkCodePs from WP1JOB_TEMP where PARAM_NAME like '$LIST_CODE_PS[%]$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        checkCodePs:=0;
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $LIST_CODE_PS$ nei parametri'); 
      end;

	    -- Seleziono il codeNotaCred
      begin
        Select code_nota_cred Into lstr_codeNotaCred From i5_2test_docum_nota_cred_xdsl w Where w.code_account = pstr_codeAccount;
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
       RAISE_APPLICATION_ERROR(-20000, 'Codice Account non presente nella testata nota di credito'); 
	    WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Impossibile ricavare il CODE_NOTA_CRED!'); 
       end;
       
       
        -- Imposto se vanno controllate le causali con quelle presenti nella tmpcausali
        -- Select Count(*) Into lint_checkCausali From tmpcausali q Where q.code_tipo_caus_variaz = 'TUTTE';
       

        For recIstDaAzzerare In istAttDaAzzerare
        Loop
            -- Check sugli oggetti di fatturazione
				if(checkCodeOggFatrz>0) THEN
					Select Count(*) Into lint_quantiCodeOggFatrz from WP1JOB_TEMP where PARAM_NAME like '$LIST_OGG_DI_FATRZ[%]$'
          and param_value = recIstDaAzzerare.code_ogg_fatrz;
					If (lint_quantiCodeOggFatrz = 0) Then
						
            GOTO end_loop;
					End if;
				End if;
						
				if(checkCausali>0) then
            Select Count(*) Into lint_quanteCausali from WP1JOB_TEMP where PARAM_NAME like '$LIST_CAUSALI[%]$'
            and param_value = recIstDaAzzerare.code_tipo_caus_variaz;
            If (lint_quanteCausali = 0) Then
          
              GOTO end_loop;
            End If;
        End if;  
				
        BEGIN
            Select *
                       Into lrec_inventario
                        From i5_1invent_ps_xdsl q
                        Where q.code_invent = recIstDaAzzerare.code_invent;
      EXCEPTION 
        WHEN OTHERS THEN    
          RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del dell''inventario!'); 
      end;

                    -- Check sulla data_dro
                    -- Se è stato richiesto di controllare la data_dro verifico che questa sia compresa
                    --      tra la dataMin e la dataMax passate in input; se non è compresa passo al
                    --      record successivo
					If (pbln_checkDataDro=1) Then
						If (not (lrec_inventario.data_dro >= pdat_dataDaMin
									and lrec_inventario.data_dro <= pdat_dataDaMax)) Then
							GOTO end_loop;
						End If;
					End If;


							
								-- Check sulla dataInizioValid
								-- Se è stata passata una dataInizioValid verifico che questa sia minore di
								--      quella presente sul record d'inventario; se non è cosi passo al
								--      record successivo
					If (pdat_dataInizioValid Is Not Null) Then
						If (not (pdat_dataInizioValid < lrec_inventario.data_inizio_valid)) Then
							GOTO end_loop;
						End If;
					End If;


                   
                        -- Check sul codiceProgetto
                        -- Se è stato passato un codiceProgetto verifico che questo è presente
                        --      sul record d'inventario; se non è cosi passo al
                        --      record successivo

         if(checkCodiciProgetto>0) then
            Select Count(*) Into lint_quantiCodiciProgetto from WP1JOB_TEMP where PARAM_NAME like '$LIST_CODICE_PROGETTO[%]$'
                    and lrec_inventario.codice_progetto is not null 
                    and param_value = lrec_inventario.codice_progetto;
            If (lint_quantiCodiciProgetto = 0) Then
    
              GOTO end_loop;
            End If;
          End  if;
                    
                        -- Check sui codePs
                        -- Se il codePs presente sul record d'inventario non è tra quelli ammessi
                        --      passo al record successivo
          if(checkCodePs>0) then
          
            Select Count(*) Into lint_quantiCodePs  from WP1JOB_TEMP where PARAM_NAME like '$LIST_CODE_PS[%]$'
            and param_value = lrec_inventario.code_ps;
            If (lint_quantiCodePs = 0) Then
              GOTO end_loop;
            End If;
					End if;
			
            INSERT INTO WP1_TABLE_KEY (WP1_TABLE_KEY.ID_KEY_VARCHAR,WP1_TABLE_KEY.ID_KEY2_VARCHAR) 
            values ( recIstDaAzzerare.code_riga_nota_cred,
                      recIstDaAzzerare.code_nota_cred);
						
					<<end_loop>>
					NULL;
        End Loop;

		Return 0;
    End MODIFICA_NOTA_CRED;




	function MODIFICA_FATTURA (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
    return NUMBER is
	/* PARAMETRI */
	 pstr_codeAccount varchar2(32767);
     pdat_dataInizioValid date;
     pbln_checkDataDro int;
     pdat_dataDaMin  date;
     pdat_dataDaMax   date;
  
         lstr_codeDocFattura i5_2test_docum_fatt_xdsl.code_doc_fattura%TYPE;

       Cursor istAttDaAzzerare is
            Select x.*
                From i5_2dett_docum_fatt_xdsl x
                Where x.code_doc_fattura = lstr_codeDocFattura
                       and ((pbln_checkDataDro = 0 and (x.data_da between pdat_dataDaMin and pdat_dataDaMax))
                              or (pbln_checkDataDro = 1));


        lrec_inventario i5_1invent_ps_xdsl%ROWTYPE;
       
        lint_quantiCodeOggFatrz number;
        lint_quanteCausali number;
        lint_quantiCodePs number;
        lint_quantiCodiciProgetto number;
        
        checkCodeOggFatrz int;
        checkCausali int;
        checkCodePs int;
        checkCodiciProgetto int;

    Begin
       
	   -- RICAVO I PARAMETRI
      begin 
      
      select PARAM_VALUE into pstr_codeAccount from WP1JOB_TEMP where PARAM_NAME = '$CODE_ACCOUNT$';
      EXCEPTION    
       WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CODE_ACCOUNT$ non presente nei parametri');  
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CODE_ACCOUNT$ nei parametri'); 
      end;
      
      begin 
      select to_date(PARAM_VALUE,'dd/mm/yyyy') into pdat_dataInizioValid from WP1JOB_TEMP where PARAM_NAME = '$DATA_INIZIO_VALID$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $DATA_INIZIO_VALID$ non presente nei parametri');
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $DATA_INIZIO_VALID$ nei parametri'); 
      end;
	   
     begin 
     select decode(PARAM_VALUE,'1',1,0) into pbln_checkDataDro from WP1JOB_TEMP where PARAM_NAME = '$CHECK_DATA_DRO$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CHECK_DATA_DRO$ non presente nei parametri');
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CHECK_DATA_DRO$ nei parametri'); 
      end;
      
	   begin 
      select to_date(PARAM_VALUE,'dd/mm/yyyy') into pdat_dataDaMin from WP1JOB_TEMP where PARAM_NAME = '$DATA_DA_MIN$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $DATA_DA_MIN$ non presente nei parametri');
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $DATA_DA_MIN$ nei parametri'); 
      end;
     
      begin 
      
	   select to_date(PARAM_VALUE,'dd/mm/yyyy') into pdat_dataDaMax from WP1JOB_TEMP where PARAM_NAME = '$DATA_DA_MAX$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $DATA_DA_MAX$ non presente nei parametri');
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $DATA_DA_MAX$ nei parametri'); 
      end;
	 
      
      begin 
      
	    select count(*) into checkCodiciProgetto from WP1JOB_TEMP where PARAM_NAME like '$LIST_CODICE_PROGETTO[%]$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        checkCodiciProgetto:=0;
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $LIST_CODICE_PROGETTO$ nei parametri'); 
      end;

      
       
      begin 
      
	  select count(*) into checkCausali from WP1JOB_TEMP where PARAM_NAME like '$LIST_CAUSALI[%]$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        checkCausali:=0;
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $LIST_CAUSALI$ dalla tabella di lavoro WP1JOB_TEMP'); 
      end;
	 
     begin 
      
	   select count(*) into checkCodeOggFatrz from WP1JOB_TEMP where PARAM_NAME like '$LIST_OGG_DI_FATRZ[%]$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        checkCodeOggFatrz:=0;
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $LIST_OGG_DI_FATRZ$ nei parametri'); 
      end;
	   
	  begin 
      
	   select count(*) into checkCodePs from WP1JOB_TEMP where PARAM_NAME like '$LIST_CODE_PS[%]$';
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
        checkCodePs:=0;
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $LIST_CODE_PS$ nei parametri'); 
      end;
 

	    -- Seleziono il code_doc_fattura
      begin
         Select code_doc_fattura Into lstr_codeDocFattura From i5_2test_docum_fatt_xdsl w Where w.code_account = pstr_codeAccount;
      EXCEPTION  
       WHEN NO_DATA_FOUND THEN
       RAISE_APPLICATION_ERROR(-20000, 'Codice Account non presente nella testata fattura'); 
	   WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Impossibile ricavare il CODE_DOC_FATTURA!'); 
       end;
       

        -- Imposto se vanno controllate le causali con quelle presenti nella tmpcausali
        -- Select Count(*) Into lint_checkCausali From tmpcausali q Where q.code_tipo_caus_variaz = 'TUTTE';
       
        begin
        
        For recIstDaAzzerare In istAttDaAzzerare
        Loop
     
            -- Check sugli oggetti di fatturazione
			if(checkCodeOggFatrz>0) THEN
					Select Count(*) Into lint_quantiCodeOggFatrz from WP1JOB_TEMP where PARAM_NAME like '$LIST_OGG_DI_FATRZ[%]$'
          and param_value = recIstDaAzzerare.code_ogg_fatrz;
					If (lint_quantiCodeOggFatrz = 0) Then
						
            GOTO end_loop;
					End if;
				End if;
						
				if(checkCausali>0) then
            Select Count(*) Into lint_quanteCausali from WP1JOB_TEMP where PARAM_NAME like '$LIST_CAUSALI[%]$'
            and param_value = recIstDaAzzerare.code_tipo_caus_variaz;
            If (lint_quanteCausali = 0) Then
          
              GOTO end_loop;
            End If;
        End if;       
        
        BEGIN
				Select *
                       Into lrec_inventario
                        From i5_1invent_ps_xdsl q
                        Where q.code_invent = recIstDaAzzerare.code_invent;
         EXCEPTION 
        WHEN OTHERS THEN    
          RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del dell''inventario!'); 
        end;     
                    -- Check sulla data_dro
                    -- Se è stato richiesto di controllare la data_dro verifico che questa sia compresa
                    --      tra la dataMin e la dataMax passate in input; se non è compresa passo al
                    --      record successivo
                    
					If (pbln_checkDataDro=1) Then
						If (not (lrec_inventario.data_dro >= pdat_dataDaMin
									and lrec_inventario.data_dro <= pdat_dataDaMax)) Then
							GOTO end_loop;
						End If;
					End If;


							
								-- Check sulla dataInizioValid
								-- Se è stata passata una dataInizioValid verifico che questa sia minore di
								--      quella presente sul record d'inventario; se non è cosi passo al
								--      record successivo
					If (pdat_dataInizioValid Is Not Null) Then
						If (not (pdat_dataInizioValid < lrec_inventario.data_inizio_valid)) Then
				
              GOTO end_loop;
						End If;
					End If;


                   
                        -- Check sul codiceProgetto
                        -- Se è stato passato un codiceProgetto verifico che questo è presente
                        --      sul record d'inventario; se non è cosi passo al
                        --      record successivo

          if(checkCodiciProgetto>0) then
            Select Count(*) Into lint_quantiCodiciProgetto from WP1JOB_TEMP where PARAM_NAME like '$LIST_CODICE_PROGETTO[%]$'
                    and lrec_inventario.codice_progetto is not null 
                    and param_value = lrec_inventario.codice_progetto;
            If (lint_quantiCodiciProgetto = 0) Then
    
              GOTO end_loop;
            End If;
          End  if;
                    
                        -- Check sui codePs
                        -- Se il codePs presente sul record d'inventario non è tra quelli ammessi
                        --      passo al record successivo
          if(checkCodePs>0) then
          
            Select Count(*) Into lint_quantiCodePs  from WP1JOB_TEMP where PARAM_NAME like '$LIST_CODE_PS[%]$'
            and param_value = lrec_inventario.code_ps;
            If (lint_quantiCodePs = 0) Then
              GOTO end_loop;
            End If;
					End if;
					/*ldbl_importoModificato := ldbl_importoModificato + (pdbl_importo-recIstDaAzzerare.impt_riga_nota_cred);*/
						/*
                        Update i5_2dett_docum_nota_cred_xdsl
                            Set impt_riga_nota_cred = pdbl_importo
                                    ,desc_riga_nota_cred = desc_riga_nota_cred || ' - ' || pstr_tracciamento
                                    ,code_tipo_caus_variaz = pstr_codeTipoCausVariaz
                            Where code_account = recIstDaAzzerare.code_account
                                    and code_riga_nota_cred= recIstDaAzzerare.code_riga_nota_cred;*/
	
            INSERT INTO WP1_TABLE_KEY (WP1_TABLE_KEY.ID_KEY_VARCHAR,WP1_TABLE_KEY.ID_KEY2_VARCHAR) 
						values ( recIstDaAzzerare.code_riga_fattura,
								  recIstDaAzzerare.code_doc_fattura);
						
					<<end_loop>>
					NULL;
        End Loop;
    EXCEPTION
    WHEN OTHERS THEN    
          RAISE_APPLICATION_ERROR(-20000, 'Errore nel reperire le fatture!!');
        end;   
		Return 0;
    
    End MODIFICA_FATTURA;


	function AGGIORNA_TESTATA_FATTURA (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_OPERATION IN varchar2) 
    return NUMBER is
    lsum NUMBER(18,6);
    lcodedoc NVARCHAR2(255);
	begin
      SELECT SUM(IMPT_RIGA_FATTURA),max(CODE_DOC_FATTURA) into lsum,lcodedoc  from i5_2dett_docum_fatt_xdsl p
      where (p.CODE_RIGA_FATTURA,p.CODE_DOC_FATTURA) in 
     ( select ID_KEY_VARCHAR,ID_KEY2_VARCHAR from WP1_TABLE_KEY );
  
      UPDATE I5_2TEST_DOCUM_FATT_XDSL SET IMPT_TOT_FATTURA= lsum where CODE_DOC_FATTURA=lcodedoc;
      
	return 0;
	end AGGIORNA_TESTATA_FATTURA;


		function AGGIORNA_TESTATA_NC (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_OPERATION IN varchar2) 
    return NUMBER is
    lsum NUMBER(18,6);
    
    lcodedoc NVARCHAR2(255);
	begin
      SELECT SUM(IMPT_RIGA_NOTA_CRED),max(CODE_NOTA_CRED) into lsum,lcodedoc from i5_2dett_docum_nota_cred_xdsl where 
        (CODE_RIGA_NOTA_CRED,CODE_NOTA_CRED) in 
        ( select ID_KEY_VARCHAR,ID_KEY2_VARCHAR from WP1_TABLE_KEY );
  
      UPDATE I5_2TEST_DOCUM_NOTA_CRED_XDSL SET IMPT_TOT_NOTA_CRED= lsum where CODE_NOTA_CRED=lcodedoc;
      
	return 0;
	end AGGIORNA_TESTATA_NC;

	function STORICIZZA_FATTURA (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_OPERATION IN varchar2) 
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
      
      
		 INSERT INTO WP1_I5_2SP_TCKT_DETT_FAT_XDSL
      (
        ID_TICKET ,
        ID_ELABORAZIONE,
        CODE_DOC_FATTURA ,
        CODE_ACCOUNT ,
        TIPO_FLAG_PRIMA_ATVZ ,
        ALIQUOTA ,
        CODE_MOV_NON_RIC ,
        DATA_INIZIO_VALID_OF ,
        TIPO_FLAG_TARIFFA ,
        DATA_TRANSAZ ,
        CODE_PR_TARIFFA ,
        CODE_RIGA_FATTURA ,
        CODE_TIPO_CAUS_VARIAZ ,
        QNTA_GG_OGG_FATRZ ,
        CODE_PROMOZIONE ,
        DESC_RIGA_FATTURA ,
        CODE_OGG_FATRZ ,
        DATA_A ,
        IMPT_RIGA_FATTURA ,
        CODE_TARIFFA ,
        CODE_INVENT ,
        DATA_DA ,
        TIPO_FLAG_IMPT_RATEO ,
        CODE_SCONTO
      )
      select 
        ID_TICKET,
        ts,
        p.CODE_DOC_FATTURA ,
        p.CODE_ACCOUNT ,
        p.TIPO_FLAG_PRIMA_ATVZ ,
        p.ALIQUOTA ,
        p.CODE_MOV_NON_RIC ,
        p.DATA_INIZIO_VALID_OF ,
        p.TIPO_FLAG_TARIFFA ,
        p.DATA_TRANSAZ ,
        p.CODE_PR_TARIFFA ,
        p.CODE_RIGA_FATTURA ,
        p.CODE_TIPO_CAUS_VARIAZ ,
        p.QNTA_GG_OGG_FATRZ ,
        p.CODE_PROMOZIONE ,
        p.DESC_RIGA_FATTURA ,
        p.CODE_OGG_FATRZ ,
        p.DATA_A ,
        p.IMPT_RIGA_FATTURA ,
        p.CODE_TARIFFA ,
        p.CODE_INVENT ,
        p.DATA_DA ,
        p.TIPO_FLAG_IMPT_RATEO ,
        p.CODE_SCONTO
      from i5_2dett_docum_fatt_xdsl p
      where (p.CODE_RIGA_FATTURA,p.CODE_DOC_FATTURA) in 
     ( select ID_KEY_VARCHAR,ID_KEY2_VARCHAR from WP1_TABLE_KEY );
    return 0;
	end STORICIZZA_FATTURA;

	function STORICIZZA_NC (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_OPERATION IN varchar2) 
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
      
  INSERT INTO WP1_I5_2SP_TCKT_NOTA_CRED_XDSL
      (
        ID_TICKET ,
        ID_ELABORAZIONE,
        CODE_MOV_NON_RIC ,
        CODE_PR_FASCIA_SCONTO_AREA ,
        DATA_TRANSAZ ,
        CODE_TIPO_CAUS_VARIAZ ,
        CODE_PROMOZIONE ,
        CODE_PR_PENALE ,
        CODE_SCONTO_AGGR ,
        CODE_OGG_FATRZ ,
        CODE_RIGA_NOTA_CRED ,
        CODE_FASCIA_SCONTO_AREA ,
        DATA_VARIAZ ,
        TIPO_FLAG_IMPT_RATEO ,
        IMPT_RIGA_NOTA_CRED ,
        CODE_SCONTO ,
        DATA_AA_RIF_FATTURA ,
        CODE_ACCOUNT ,
        CODE_NOTA_CRED ,
        ALIQUOTA ,
        CODE_PR_SCONTO ,
        DATA_INIZIO_VALID_OF ,
        TIPO_FLAG_TARIFFA ,
        CODE_PR_TARIFFA ,
        QNTA_GG_OGG_FATRZ ,
        CODE_PENALE ,
        DATA_A ,
        DATA_MM_RIF_FATTURA ,
        CODE_TARIFFA ,
        CODE_INVENT ,
        CODE_OFF_SCONTO ,
        DATA_DA ,
        DESC_RIGA_NOTA_CRED
      )
select 
ID_TICKET,
ts,
CODE_MOV_NON_RIC ,
        CODE_PR_FASCIA_SCONTO_AREA ,
        DATA_TRANSAZ ,
        CODE_TIPO_CAUS_VARIAZ ,
        CODE_PROMOZIONE ,
        CODE_PR_PENALE ,
        CODE_SCONTO_AGGR ,
        CODE_OGG_FATRZ ,
        CODE_RIGA_NOTA_CRED ,
        CODE_FASCIA_SCONTO_AREA ,
        DATA_VARIAZ ,
        TIPO_FLAG_IMPT_RATEO ,
        IMPT_RIGA_NOTA_CRED ,
        CODE_SCONTO ,
        DATA_AA_RIF_FATTURA ,
        CODE_ACCOUNT ,
        CODE_NOTA_CRED ,
        ALIQUOTA ,
        CODE_PR_SCONTO ,
        DATA_INIZIO_VALID_OF ,
        TIPO_FLAG_TARIFFA ,
        CODE_PR_TARIFFA ,
        QNTA_GG_OGG_FATRZ ,
        CODE_PENALE ,
        DATA_A ,
        DATA_MM_RIF_FATTURA ,
        CODE_TARIFFA ,
        CODE_INVENT ,
        CODE_OFF_SCONTO ,
        DATA_DA ,
        DESC_RIGA_NOTA_CRED
        from i5_2dett_docum_nota_cred_xdsl where 
        (CODE_RIGA_NOTA_CRED,CODE_NOTA_CRED) in 
        ( select ID_KEY_VARCHAR,ID_KEY2_VARCHAR from WP1_TABLE_KEY );
	return 0;
	end STORICIZZA_NC;

FUNCTION GET_ACONET(ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) return number Is
        Cursor risorse_da_bonificare is
                        Select b.*
                                ,a.valo_mcr as valo_mcr_vc
                                ,a.valo_mcr_up as valo_mcr_up_vc
                            From 
                                    (Select *
                                        From i5_1invent_ps_xdsl z
                                        Where z.code_account = 'CS3471'
                                                and z.data_fine_valid is null
                                                and z.id_ord_crmws is not null
                                                and z.code_istanza_ps = z.id_vc) a

                                    ,(Select * 
                                        From i5_1invent_ps_xdsl q
                                        Where q.code_account = 'CS3471'
                                                and q.data_fine_valid is null
                                                and q.id_ord_crmws is not null
                                                and q.code_istanza_ps = q.id_accesso) b

                            Where b.code_account = a.code_account
                                    and b.code_istanza_ps = a.id_accesso
                                    and b.valo_mcr = 0
                    ;

       

    Begin
       
        For recDaBonificare In risorse_da_bonificare
        Loop
            Update i5_1invent_ps_xdsl
                Set valo_mcr = recDaBonificare.valo_mcr_vc
                        ,valo_mcr_up = recDaBonificare.valo_mcr_up_vc
                Where code_invent = recDaBonificare.code_invent;
			  INSERT INTO WP1_TABLE_KEY (WP1_TABLE_KEY.ID_KEY_VARCHAR) 
						values ( recDaBonificare.code_invent);
        End Loop;
	return 0;
END GET_ACONET;


function MODIFICA_FATTURA_X_TIPO_CONTR (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
return NUMBER is
pstr_code_tipo_contr varchar2(32767);
p_ret int;
Cursor accountdaanalizzare is
            Select distinct code_account
                From i5_2test_docum_fatt_xdsl x
                Where x.code_account in ( 
                select code_account from I5_1ACCOUNT_X_CONTR x 
                join I5_1CONTR y on x.CODE_CONTR=y.CODE_CONTR where y.CODE_TIPO_CONTR=pstr_code_tipo_contr);
begin
      begin 
      
      select PARAM_VALUE into pstr_code_tipo_contr from WP1JOB_TEMP where PARAM_NAME = '$CODE_TIPO_CONTR$';
      EXCEPTION    
       WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CODE_ACCOUNT$ non presente nei parametri');  
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CODE_ACCOUNT$ nei parametri'); 
      end;
  For recAccDaAnalizzare In accountdaanalizzare
        Loop
        delete from WP1JOB_TEMP where PARAM_NAME='$CODE_ACCOUNT$';
        insert into WP1JOB_TEMP (PARAM_NAME,PARAM_VALUE) values
           ('$CODE_ACCOUNT$',recAccDaAnalizzare.code_account);
        begin
          p_ret:=PKG_WP1_POSTVAL.MODIFICA_FATTURA(ID_TICKET,ID_JOB,ID_CONDITION);
        exception
         WHEN OTHERS THEN
          raise;
        END;
    end loop;
    return 0;
END MODIFICA_FATTURA_X_TIPO_CONTR;


function MODIFICA_NC_X_TIPO_CONTR (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) 
return NUMBER is
pstr_code_tipo_contr varchar2(32767);
p_ret int;
Cursor accountdaanalizzare is
            Select distinct code_account
                From i5_2test_docum_nota_cred_xdsl x
                Where x.code_account in ( 
                select code_account from I5_1ACCOUNT_X_CONTR x 
                join I5_1CONTR y on x.CODE_CONTR=y.CODE_CONTR where y.CODE_TIPO_CONTR=pstr_code_tipo_contr);
begin
      begin 
      
      select PARAM_VALUE into pstr_code_tipo_contr from WP1JOB_TEMP where PARAM_NAME = '$CODE_TIPO_CONTR$';
      EXCEPTION    
       WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20000, 'Paramtro $CODE_ACCOUNT$ non presente nei parametri');  
      WHEN OTHERS THEN    
        RAISE_APPLICATION_ERROR(-20000, 'Errore nella lettura del parametro $CODE_ACCOUNT$ nei parametri'); 
      end;
  For recAccDaAnalizzare In accountdaanalizzare
        Loop
        delete from WP1JOB_TEMP where PARAM_NAME='$CODE_ACCOUNT$';
        insert into WP1JOB_TEMP (PARAM_NAME,PARAM_VALUE) values
           ('$CODE_ACCOUNT$',recAccDaAnalizzare.code_account);
        begin
          p_ret:=PKG_WP1_POSTVAL.MODIFICA_NOTA_CRED(ID_TICKET,ID_JOB,ID_CONDITION);
        exception
         WHEN OTHERS THEN
          raise;
        END;
    end loop;
    return 0;
END MODIFICA_NC_X_TIPO_CONTR;

END "PKG_WP1_POSTVAL";
/

spool off;
