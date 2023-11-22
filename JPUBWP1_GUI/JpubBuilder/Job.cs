using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPubGfx
{
    /// <summary>
    /// Classe che rappresenta l'oggetto Job
    /// </summary>
    public class Job
    {
        public string id_job { get; set; }
        public string j_descrizione { get; set; }
        public string j_active { get; set; }
        public string j_job_type { get; set; }
        public string j_code_tipo_contr { get; set; }
        public string j_mastertable { get; set; }
        public string j_primarykey { get; set; }

        public Job()
        {
        }

    }
}
