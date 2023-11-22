spool 'PKG_WP1_SAP.log';

CREATE OR REPLACE PACKAGE        "PKG_WP1_SAP" IS

  function BACKUP_SAP (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2)
    return NUMBER;
  function SELECT_TESTATE_VUOTE_SAP (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2)
    return NUMBER;

  function EXEC_ACCORPA_GESTORI (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2)
    return NUMBER;


function DELETE_TESTATE(ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2) return number;


END "PKG_WP1_SAP";
/


CREATE OR REPLACE PACKAGE BODY PKG_WP1_SAP AS

function DELETE_TESTATE(ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2)
return NUMBER AS
BEGIN
   
   
   DELETE FROM I5_2DETT_CSV_SAP_XDSL WHERE CODE_TEST_SAP IN ( select ID_KEY_VARCHAR from  WP1_TABLE_KEY );
   DELETE FROM I5_2TEST_CSV_SAP_XDSL  where ( I5_2TEST_CSV_SAP_XDSL.CODE_TEST_SAP) 
   IN (
        select ID_KEY_VARCHAR from  WP1_TABLE_KEY
        );
        
   return 0;
END DELETE_TESTATE; 

  function BACKUP_SAP (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2)
    return NUMBER AS
  BEGIN
				delete from WP1_CVS_SAP_TEST_BACKUP;
				delete from WP1_CVS_SAP_DETT_BACKUP;

				insert into WP1_CVS_SAP_TEST_BACKUP(

              CODE_TEST_SAP ,
              CODE_GEST_SAP ,
              CODE_FISC_GEST  ,
              CODE_PARTITA_IVA  ,
              CODE_ORG_COMM   ,
              CODE_CANALE_DISTRIB ,
              CODE_SETT_MERC    ,
              CODE_TIPO_ORD ,
              CODE_PROG_ORD ,
              DATA_ESTRAZ_DATI ,
              CODE_TIPO_PAGAM ,
              DATA_INIZIO_CICLO_FATRZ   ,
              TEXT_NOTA_TEST ,
              FLAG_REPRICING )
				select
            CODE_TEST_SAP ,
            CODE_GEST_SAP ,
            CODE_FISC_GEST  ,
            CODE_PARTITA_IVA  ,
            CODE_ORG_COMM   ,
            CODE_CANALE_DISTRIB ,
            CODE_SETT_MERC    ,
            CODE_TIPO_ORD ,
            CODE_PROG_ORD ,
            DATA_ESTRAZ_DATI ,
            CODE_TIPO_PAGAM ,
            DATA_INIZIO_CICLO_FATRZ   ,
            TEXT_NOTA_TEST ,
            FLAG_REPRICING  from i5_2test_csv_sap_xdsl;

				insert into WP1_CVS_SAP_DETT_BACKUP (CODE_DETT_SAP,
            CODE_TEST_SAP ,
            CODE_MATERIALE ,
            VALO_IMPT_TOT ,
            VALO_QNTA_PS ,
            CODE_PROFIT_CENTER     ,
            TEXT_NOTA_DETT  ,
            ALIQUOTA   )
                    select
                    CODE_DETT_SAP,
            CODE_TEST_SAP ,
            CODE_MATERIALE ,
            VALO_IMPT_TOT ,
            VALO_QNTA_PS ,
            CODE_PROFIT_CENTER     ,
            TEXT_NOTA_DETT  ,
            ALIQUOTA
        from i5_2dett_csv_sap_xdsl;

    return sql%rowcount;
  END BACKUP_SAP;

    function SELECT_TESTATE_VUOTE_SAP (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2)
    return NUMBER AS
  BEGIN
       INSERT INTO WP1_TABLE_KEY (WP1_TABLE_KEY.ID_KEY_VARCHAR)
				select i5_2test_csv_sap_xdsl.CODE_TEST_SAP from
          i5_2test_csv_sap_xdsl where not exists ( select code_test_sap from i5_2dett_csv_sap_xdsl where i5_2dett_csv_sap_xdsl.code_test_sap=i5_2test_csv_sap_xdsl.code_test_sap );

    return sql%rowcount;
  END SELECT_TESTATE_VUOTE_SAP;

 function EXEC_ACCORPA_GESTORI (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_CONDITION IN varchar2)
    return NUMBER AS
  BEGIN
      accorpa_gestori_xdsl();
    return 0;
  END EXEC_ACCORPA_GESTORI;

END PKG_WP1_SAP;
/

spool off;