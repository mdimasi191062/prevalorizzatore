/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package jribescommander.db.entity.beans;

/**
 *
 * @author oracle
 */
public class Account {
    private String account;
  private String desc;
  private String codeParam;
  private String codeDocFatt;
  private String codElabBatch;
  private Integer NumFattLisUn;
  private String dataIniPerFatt;
  private String dataFinePerFatt;
  private String maxDataFine;
 
  public Account()
  {
  }

  public Account(String account, String descrizione)
  {
   this.desc=descrizione;
   this.account=account;
  }


  public String getCodeDocFatt()
  {
    return codeDocFatt;
  }
  public void setCodeDocFatt(String stringa)
  {
     codeDocFatt=stringa;
  }

  
  public String getAccount()
  {
    return account;
  }
  public void setAccount(String stringa)
  {
      account=stringa;
  }
  public String getDesc()
  {
    return desc;
  }
  public void setDesc(String stringa)
  {
    desc=stringa;
  }
  public String getCodeParam()
  {
    return codeParam;
  }
  public void setCodeParam(String stringa)
  {
      codeParam=stringa;
  }
  
  public void setCodElabBatch(String stringa)
  {
    codElabBatch=stringa;
  }
  public String getCodElabBatch()
  {
    return codElabBatch;
  }
  public void setNumFattLisUn(Integer stringa)
  {
    NumFattLisUn=stringa;
  }
  public Integer getNumFattLisUn()
  {
    return NumFattLisUn;
  }
  public String getDataIniPerFatt()
  {
    return dataIniPerFatt;
  }
  public void setDataIniPerFatt(String stringa)
  {
    dataIniPerFatt=stringa;
  }
  public String getDataFinePerFatt()
  {
    return dataFinePerFatt;
  }
  public void setDataFinePerFatt(String stringa)
  {
    dataFinePerFatt=stringa;
  }

  public String getMaxDataFine()
  {
    return maxDataFine;
  }
  public void setMaxDataFine(String stringa)
  {
    maxDataFine=stringa;
  }
 
}
