using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization;


namespace JPubGfx
{
    /// <summary>
    /// Estensione del TreeNode che visualizza un ComboBox sul nodo
    /// </summary>
    public class DropDownTreeNode : TreeNode
    {
        #region Constructors
        /// <summary>
        /// Inizializza l'instanza della classe
        /// </summary>
        public DropDownTreeNode()
            : base()
        {
        }

        /// <summary>
        /// Inizializza l'instanza della classe con un testo specifico
        /// </summary>
        /// <param name="text">Testo</param>
        public DropDownTreeNode(string text)
            : base(text)
        {
        }

        
        /// <summary>
        /// Inizializza l'instanza della classe con un testo e un array di figli
        /// </summary>
        /// <param name="text">Testo</param>
        /// <param name="children">Array</param>
        public DropDownTreeNode(string text, TreeNode[] children)
            : base(text, children)
        {
        }

        /// <summary>
        /// Inizializza l'instanza della classe con informazioni Serializzate
        /// </summary>
        /// <param name="serializationInfo">A <see cref="T:System.Runtime.Serialization.SerializationInfo"></see>Dati Serializzati</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see>Destinazione dello stream serializzato</param>
        public DropDownTreeNode(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }

        /// <summary>
        /// Inizializza l'instanza della classe <see cref="T:DropDownTreeNode"/> .
        /// </summary>
        /// <param name="text">Testo</param>
        /// <param name="imageIndex">Indice dell'immagine</param>
        /// <param name="selectedImageIndex">Indice dell'immagine selezionata</param>
        public DropDownTreeNode(string text, int imageIndex, int selectedImageIndex)
            : base(text, imageIndex, selectedImageIndex)
        {
        }

        /// <summary>
        /// Inizializza una nuova instanza della classe  <see cref="T:DropDownTreeNode"/>
        /// </summary>
        /// <param name="text">Testo</param>
        /// <param name="imageIndex">Indice dell'immagine </param>
        /// <param name="selectedImageIndex">Indice dell'immagine selezionata</param>
        /// <param name="children">Nodo Figlio</param>
        public DropDownTreeNode(string text, int imageIndex, int selectedImageIndex, TreeNode[] children)
            : base(text, imageIndex, selectedImageIndex, children)
        {
        }
        #endregion


        #region Property - ComboBox
        private ComboBox m_ComboBox = new ComboBox();
        /// <summary>
        /// Ottiene o imposta il ComboBox. Consente di accedere a tutte le proprietà della ComboBox interna.
        /// </summary>
        /// <example>
        /// Esempio
        /// <code>
        /// DropDownTreeNode node1 = new DropDownTreeNode("Some text");
        /// node1.ComboBox.Items.Add("Testo1");
        /// node1.ComboBox.Items.Add("Testo2");
        /// node1.IsDropDown = true; 
        /// </code>
        /// </example>
        /// <value>ComboBox</value>
        public ComboBox ComboBox
        {
            get
            {
                this.m_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                return this.m_ComboBox;
            }
            set
            {
                this.m_ComboBox = value;
                this.m_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }
        #endregion
    }
}
