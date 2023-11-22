using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPubGfx
{

    /// <summary>
    /// Classe che rappresenta l'oggetto Condition
    /// </summary>
    public class Condition
    {
        public string id_condition { get; set; }
        public string c_field { get; set; }
        public string c_operator { get; set; }
        public string c_operands { get; set; }
        public string c_field_type { get; set; }
        public string c_cond_type { get; set; }
        public string c_cond_optional { get; set; }
        public Condition()
        {
        }

    }
}
