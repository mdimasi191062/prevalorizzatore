/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package jribescommander.db.entity;

import oracle.jdbc.OracleTypes;


public class AbstractEJBTypes 
{
  // Costanti che identificano la tipologia del parametro.
  public static final String PARAMETER_RETURN = "1";
  static final int PARAMETER_RETURN_NUMBER = 1;
  public static final String PARAMETER_INPUT = "2";
  static final int PARAMETER_INPUT_NUMBER = 2;
  public static final String PARAMETER_OUTPUT = "3";
  static final int PARAMETER_OUTPUT_NUMBER = 3;

  // Costanti che identificano il tipo del parametro.
  public static final String TYPE_STRING = Integer.toString(OracleTypes.VARCHAR);
  static final int TYPE_STRING_NUMBER = OracleTypes.VARCHAR;
  public static final String TYPE_INTEGER = Integer.toString(OracleTypes.INTEGER);
  static final int TYPE_INTEGER_NUMBER = OracleTypes.INTEGER;
  public static final String TYPE_DOUBLE = Integer.toString(OracleTypes.DOUBLE);
  static final int TYPE_DOUBLE_NUMBER = OracleTypes.DOUBLE;
  public static final String TYPE_RESULTSET = Integer.toString(OracleTypes.CURSOR);
  static final int TYPE_RESULTSET_NUMBER = OracleTypes.CURSOR;
  // UMBP 25032003 Aggiunta definizione LONGVARCHAR
  public static final String TYPE_LONGVARCHAR = Integer.toString(OracleTypes.LONGVARCHAR);
  static final int TYPE_LONGVARCHAR_NUMBER = OracleTypes.LONGVARCHAR;
  // RM 05042006 Aggiunta definizione STRUCT
  public static final String TYPE_STRUCT = Integer.toString(OracleTypes.STRUCT);
  static final int TYPE_STRUCT_NUMBER = OracleTypes.STRUCT;

  // ------------------------------------------------

  
  public static final String TYPE_VECTOR = "9876001";
  static final int TYPE_VECTOR_NUMBER = 9876001;
}