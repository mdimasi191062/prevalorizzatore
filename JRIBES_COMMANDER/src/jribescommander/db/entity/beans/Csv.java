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
public class Csv {
  
  private String account;
  private int csv;
  private int ps; 
  public Csv()
  {
  }

 public Csv(int nrgCsv, int nrgPs, String acc)
  {
   this.csv=nrgCsv;
   this.ps=nrgPs;
   this.account=acc;
  }

  public String getAccount()
  {
    return account;
  }
  public void setAccount(String acc)
  {
      account=acc;
  }
  public int getCsv()
  {
    return csv;
  }
  public void setCsv(int nrgCsv)
  {
      csv=nrgCsv;
  }
  public int getPs()
  {
    return ps;
  }
  public void setPs(int nrgPs)
  {
      ps=nrgPs;
  }


}
