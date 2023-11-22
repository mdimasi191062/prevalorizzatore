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
public class AccountElab {

  private String account;
  private String accountCode;
  private String dataIni;
  private String dataFine;
  private String flagErrore;
  private String flagErroreReport;
  private int scarti;
  private String cicloIni;
  private String cicloFine;
  private int numNC;
  private String codeParam;

  public String getCodeParam()
  { return codeParam; }
  
  public void setCodeParam(String stringa)
  { codeParam=stringa; }

  
  public String getAccount()
  { return account; }
  
  public void setAccount(String stringa)
  { account=stringa; }

   public String getCodeAccount()
  { return accountCode; }
  
  public void setCodeAccount(String stringa)
  { accountCode=stringa; }
  
  public String getDataIni()
  {  return dataIni; }
  
  public void setDataIni(String stringa)
  { dataIni=stringa; }
  
  public int getScarti()
  {
  return scarti; }
  
  public void setScarti(int nrg)
  { 
  scarti=nrg; }
 
  public String getDataFine()
  {  return dataFine; }
  
  public void setDataFine(String stringa)
  { dataFine=stringa; }
  
  public String getFlagErrore()
  {  return flagErrore; }
  
  public void setFlagErrore(String stringa)
  { flagErrore=stringa; }

  public String getFlagErroreReport()
  {  return flagErroreReport; }
  
  public void setFlagErroreReport(String stringa)
  { flagErroreReport=stringa; }

    public String getCicloFine()
  {  return cicloFine; }
  
  public void setCicloFine(String stringa)
  { cicloFine=stringa; }
  
  public String getCicloIni()
  {  return cicloIni; }
  
  public void setCicloIni(String stringa)
  { cicloIni=stringa; }

    public int getNumNC()
  {
  return numNC; }
  
  public void setNumNC(int nrg)
  { 
  numNC=nrg; }

}
