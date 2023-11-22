spool 'PKG_WP1_UTILITY.log'

CREATE OR REPLACE package  pkg_wp1_utility as  
     function LOG_KEY (iID_TICKET IN varchar2 , iID_JOB IN varchar2,iCOUNTER IN number)
    return number;
   function write_log_elab (i_id_elab in number, i_id_ticket in varchar2,i_descrizione in varchar2,i_esito in varchar2)
     return number;
   function write_log_elab_dett (i_id_elab in number, i_id_ticket in varchar2,i_id_job in varchar2, 
   i_id_condition in varchar2,i_id_operation in varchar2, i_descrizione_errore in varchar2,
   i_esito in varchar2,i_contatore in number)
     return number;      
      function write_log (ts in number,idelab in number , idTicket in varchar2,idJob in varchar2,
      messaggio in clob) return number ;
   function get_data_inizio_periodo  return varchar2;
  
   function FAKE_OPERATION_COMPLEX (ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_OPERATION IN varchar2) 
    return NUMBER;      
 
  function get_data_inizio_ciclo_5_anni return varchar2;
  function get_data_inizio_ciclo return varchar2;
 
end pkg_wp1_utility;
/


CREATE OR REPLACE package body pkg_wp1_utility as  

    function LOG_KEY (iID_TICKET IN varchar2 , iID_JOB IN varchar2,iCOUNTER IN number)
       return number is
      -- pragma autonomous_transaction;
       BEGIN
     --   begin
       delete from WP1_LOG_KEY where ID_JOB=iID_JOB and ID_TICKET=iID_TICKET and CONTATORE=iCOUNTER;
       
        insert into WP1_LOG_KEY ( ID_JOB,ID_TICKET,CONTATORE,ID_KEY_VARCHAR,ID_KEY_NUMBER,ID_KEY2_VARCHAR,ID_KEY2_NUMBER)
        select iID_JOB,iID_TICKET,iCOUNTER,ID_KEY_VARCHAR,ID_KEY_NUMBER,ID_KEY2_VARCHAR,ID_KEY2_NUMBER
          from WP1_TABLE_KEY;
    --     commit; 
    --    exception  when others then  
    --        rollback;
     --       raise;
     --     END;
         
        return sql%rowcount;
       END LOG_KEY;
       
     -- funzione di log
     function write_log_elab (i_id_elab in number, i_id_ticket in varchar2,i_descrizione in varchar2,i_esito in varchar2)
       return number is
       pragma autonomous_transaction;
       vCount number:=0;
     begin
       
         select nvl(count(*),0) into vCount from wp1_ticketga_elaboration where id_elaborazione = i_id_elab and id_ticket  =  i_id_ticket;
       
         if (vCount = 0 ) then 
           insert into wp1_ticketga_elaboration (
                id_elaborazione,
                id_ticket,
                descrizione_errore,
                esito,
                data_modifica)
            values (
                  i_id_elab,
                  i_id_ticket,
                  i_descrizione,
                  i_esito,
                  sysdate
                  );
        else
            update wp1_ticketga_elaboration 
            set
              descrizione_errore = i_descrizione,
              esito = i_esito,
              data_modifica=sysdate
            where id_elaborazione = i_id_elab and id_ticket  =  i_id_ticket;
        end if;
              
       commit;       
       return 0;
     exception  when others then  
      rollback;
      raise;
      return 1;
     end write_log_elab;
     
  
     
      function write_log_elab_dett (i_id_elab in number, i_id_ticket in varchar2,i_id_job in varchar2,
      i_id_condition in varchar2,i_id_operation in varchar2, 
      i_descrizione_errore in varchar2,i_esito in varchar2, i_contatore in number)
       return number is
       pragma autonomous_transaction;
       vCount number:=0;
     begin
  
       select nvl(count(*),0) into vCount from wp1_ticketga_elaboration_det where id_elaborazione = i_id_elab and id_ticket  =  i_id_ticket and id_job = i_id_job
                                                                          and i_contatore=contatore;
       
        if (vCount = 0 ) then 
         insert into wp1_ticketga_elaboration_det (
              id_elaborazione,
              id_ticket,
              id_job,
              id_condition,
              id_operation,
              descrizione,
              esito,
              contatore,
              data_modifica
              )
          values (
                i_id_elab,
                i_id_ticket,
                i_id_job,
                i_id_condition,
                i_id_operation,
                i_descrizione_errore,
                i_esito,
                i_contatore,
                sysdate
                );   
        else
              update wp1_ticketga_elaboration_det set
                id_condition = i_id_condition,
                id_operation = i_id_operation,
                descrizione  = i_descrizione_errore,
                esito = i_esito,
                data_modifica=sysdate
              where id_elaborazione  = i_id_elab     and
                    id_ticket = i_id_ticket   and
                    id_job    = i_id_job and
                    contatore =i_contatore;
        end if;      
       commit;       
       return 0;
     exception  when others then  
       rollback;
       raise;
      return 1;
     end write_log_elab_dett;
     
     
     
     function write_log (ts in number,idelab in number ,idTicket in varchar2,idJob in varchar2,
      messaggio in clob)
       return number is
       pragma autonomous_transaction;
     
     begin
  
     
       insert into wp1_log (
            ts,
            id_elab,
            id_ticket,
            id_job,
            messaggio,
            data_ins
            )
        values (
              ts,
              idelab,
              idTicket,
              idJob,
              messaggio,
              sysdate
              );     
       commit;       
       return 0;
     exception  when others then  
       rollback;
       raise;
      return 1;
     end write_log;
     
     function get_data_inizio_ciclo_5_anni return varchar2
     is
       svdate varchar2(10); 
      begin
      
          select to_char(add_months(max(q.data_inizio_ciclo_fatrz),-60),'DD/MM/YYYY')
          into svdate
                                                      From 
                                                           i5_2param_valoriz_sp q
                                                            ,i5_1account w
                                                      Where q.data_cong is null
                                                              and w.code_account = q.code_account
                                                              and w.flag_sys = 'S'
                                                              and q.data_inizio_ciclo_fatrz < (sysdate-10);
        return svdate;
      
      end get_data_inizio_ciclo_5_anni;
      
      function get_data_inizio_ciclo return varchar2
     is
       svdate varchar2(10); 
      begin
      
          select TO_CHAR(max(q.data_inizio_ciclo_fatrz),'DD/MM/YYYY')
          into svdate
                                                      From 
                                                           i5_2param_valoriz_sp q
                                                            ,i5_1account w
                                                      Where q.data_cong is null
                                                              and w.code_account = q.code_account
                                                              and w.flag_sys = 'S'
                                                              and q.data_inizio_ciclo_fatrz < (sysdate-10);
        return svdate;
      
      end get_data_inizio_ciclo;
    
     function get_data_inizio_periodo return varchar2
     is
       svdate varchar2(10); 
       begin
       
       
       
        select TO_CHAR(max(distinct z.data_fine_periodo),'DD/MM/YYYY')
              into svdate
        from 
          i5_2param_valoriz_sp z
          ,i5_1account x
        where 
        z.data_inizio_ciclo_fatrz = (Select add_months((max(q.data_inizio_ciclo_fatrz)), -1)
                                                      From 
                                                           i5_2param_valoriz_sp q
                                                            ,i5_1account w
                                                      Where q.data_cong is null
                                                              and w.code_account = q.code_account
                                                              and w.flag_sys = 'S'
                                                              and q.data_inizio_ciclo_fatrz < (sysdate-10))
                      and x.code_account = z.code_account
                      and x.flag_sys = 'S';
                      
        -- PEZZA DA ELIMINARE!!! ATTENZEIONE SOLO PER TEST IN LABORATORIO
        --svdate := '10/11/2014';
                      
        return svdate;                
       end get_data_inizio_periodo;
     
    /*FAKE */ 
    function FAKE_OPERATION_COMPLEX(ID_TICKET IN varchar2 , ID_JOB IN varchar2,ID_OPERATION IN varchar2) return number
     is
     begin
      return 0;
    end FAKE_OPERATION_COMPLEX;     
  
  
  
  end pkg_wp1_utility;
/

spool off;
