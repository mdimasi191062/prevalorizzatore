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
public class BatchElem  implements java.io.Serializable
{
  private String codeFunz;
  private int flagTipoContr;

  public String getCodeFunz()
  { 
    return codeFunz; 
  }

  public void setCodeFunz(String stringa)
  {
    codeFunz=stringa; 
  }

  public int getFlagTipoContr()
  {  
     return flagTipoContr; 
  }

  public void setFlagTipoContr(int i)
  {  
    flagTipoContr=i; 
  }
}