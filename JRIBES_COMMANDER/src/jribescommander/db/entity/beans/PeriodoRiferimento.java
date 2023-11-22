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
public class PeriodoRiferimento {

    private String code;
    private String descOf;
    private String dataIniCiclo;
    private String dataFineCiclo;
    private String periodoFat;

    public PeriodoRiferimento() {
    }

    public PeriodoRiferimento(String code, String descrizione) {
        this.descOf = descrizione;
        this.code = code;
    }

    public String getCode() {
        return code;
    }

    public void setCode(String stringa) {
        code = stringa;
    }

    public String getDescOf() {
        return descOf;
    }

    public void setDescOf(String stringa) {
        descOf = stringa;
    }

    public String getDataIniCiclo() {
        return dataIniCiclo;
    }

    public void setDataIniCiclo(String stringa) {
        dataIniCiclo = stringa;
    }

    public String getDataFineCiclo() {
        return dataFineCiclo;
    }

    public void setDataFineCiclo(String stringa) {
        dataFineCiclo = stringa;
    }

    public String getPeriodoFat() {
        return periodoFat;
    }

    public void setPeriodoFat(String stringa) {
        periodoFat = stringa;
    }
}
