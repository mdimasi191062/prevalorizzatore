/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package jribescommander;

public class JRibesReturn implements java.io.Serializable
{
  private String jr_code_elab;
  private String jr_flag_sys;
  private String jr_name;
  private String jr_username;
  private String jr_is_available;
  private String jr_have_concurrency;
  private int jr_error_code;
  private String jr_error_desc;
  
  public String getJr_code_elab()
  {
    return jr_code_elab;
  }
  public void setJr_code_elab(String stringa)
  {
      jr_code_elab=stringa;
  }
  
  public String getJr_flag_sys()
  {
    return jr_flag_sys;
  }
  public void setJr_flag_sys(String stringa)
  {
      jr_flag_sys=stringa;
  }
  
  public String getJr_name()
  {
    return jr_name;
  }
  public void setJr_name(String stringa)
  {
      jr_name=stringa;
  }
  
  public String getJr_username()
  {
    return jr_username;
  }
  public void setJr_username(String stringa)
  {
      jr_username=stringa;
  }
  
  public String getJr_is_available()
  {
    return jr_is_available;
  }
  public void setJr_is_available(String stringa)
  {
      jr_is_available=stringa;
  }
  
  public String getJr_have_concurrency()
  {
    return jr_have_concurrency;
  }
  public void setJr_have_concurrency(String stringa)
  {
      jr_have_concurrency=stringa;
  }
  
  public int getJr_error_code()
  {
    return jr_error_code;
  }
  public void setJr_error_code(int intero)
  {
      jr_error_code=intero;
  }
  
  public String getJr_error_desc()
  {
    return jr_error_desc;
  }
  public void setJr_error_desc(String stringa)
  {
      jr_error_desc=stringa;
  }
}