using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPubGfx
{

    /// <summary>
    /// Classe che rappresenta l'oggetto Operation
    /// </summary>
    public class Operation
    {
        public string id_operation { get; set; }
        public string o_field { get; set; }
        public string o_operator { get; set; }
        public string o_operand { get; set; }
        public string o_op_type { get; set; }

        public Operation()
        {
        }
    }
}
