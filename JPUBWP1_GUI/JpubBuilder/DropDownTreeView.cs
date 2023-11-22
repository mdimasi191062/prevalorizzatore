using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace JPubGfx
{
    public class DropDownTreeView : TreeView
    {

        
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:DropDownTreeView"/> class.
        /// </summary>
        public DropDownTreeView()
            : base()
        {

        }
        #endregion


        private DropDownTreeNode m_CurrentNode = null;
        public string currText = "";



        /// <summary>
        /// Evento Click sul nodo dell'albero
        /// </summary>
        /// <param name="e">Contiene i dati dell'evento</param>
        protected override void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e)
        {

            Console.WriteLine("OnNodeMouseDoubleClick   " + e.Node.Text );


            if (e.Node is DropDownTreeNode)
            {
                this.m_CurrentNode = (DropDownTreeNode)e.Node;

                this.Controls.Add(this.m_CurrentNode.ComboBox);

                this.m_CurrentNode.ComboBox.SetBounds(
                    this.m_CurrentNode.Bounds.X - 1,
                    this.m_CurrentNode.Bounds.Y+2,
                    this.m_CurrentNode.Bounds.Width + 95,
                    this.m_CurrentNode.Bounds.Height +10);

                this.m_CurrentNode.ComboBox.SelectedValueChanged += new EventHandler(ComboBox_SelectedValueChanged);
                this.m_CurrentNode.ComboBox.DropDownClosed += new EventHandler(ComboBox_DropDownClosed);
                this.m_CurrentNode.ComboBox.MouseUp += new MouseEventHandler(ComboBox_DropDownMouseUp);
                this.m_CurrentNode.ComboBox.MouseDown += new MouseEventHandler(ComboBox_DropDownMouseDown);


                
                this.m_CurrentNode.ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                this.m_CurrentNode.ComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;


                this.m_CurrentNode.ComboBox.Show();
                if (e.Button == MouseButtons.Left)
                {
                    
                    var hitTest = this.HitTest(e.Location);
                    if (hitTest.Location == TreeViewHitTestLocations.PlusMinus)
                    {
                        HideComboBox();
                    }
                    else
                    {
                        this.m_CurrentNode.ComboBox.DroppedDown = true;
                    }
                }

            }
            base.OnNodeMouseDoubleClick(e);

        }






        /// <summary>
        /// Evento 
        /// Nasconde la ComboBox se è selezionato un elemento
        /// </summary>
        /// <param name="sender">Origine dell'evento.</param>
        /// <param name="e">Instanza contenente i dati dell'evento</param>
        void ComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            currText = (sender as ComboBox).Text;
            HideComboBox();
        }


        /// <summary>
        /// Evento di chiusura tendina ComboBox
        /// </summary>
        /// <param name="sender">Origine dell'evento.</param>
        /// <param name="e"> <see cref="T:System.EventArgs"/>Instanza contenente i dati dell'evento</param>
        void ComboBox_DropDownClosed(object sender, EventArgs e)
        {

            currText = (sender as ComboBox).Text;
            HideComboBox();
        }


        /// <summary>
        /// Evento di MouseUp su Combobox
        /// </summary>
        /// <param name="sender">Origine dell'evento.</param>
        /// <param name="e"><see cref="T:System.EventArgs"/>Instanza contenente i dati dell'evento</param>
        void ComboBox_DropDownMouseUp(object sender, MouseEventArgs e)
        {
            HideComboBox();
        }


        void ComboBox_DropDownMouseDown(object sender, MouseEventArgs e)
        {
        }



        /// <summary>
        /// Nasconde la combobox se l'utente esegue scroll del mouse
        /// </summary>
        /// <param name="e"><see cref="T:System.EventArgs"/>Instanza contenente i dati dell'evento</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            HideComboBox();
            base.OnMouseWheel(e);
        }

        /// <summary>
        /// Metodo che nasconde la combobox
        /// </summary>
        public void HideCB()
        {
            HideComboBox();
        }

        
        /// <summary>
        /// Metodo che nasconde il nodo corrente
        /// </summary>
        private void HideComboBox()
        {
            if (this.m_CurrentNode != null)
            {
                this.m_CurrentNode.ComboBox.SelectedValueChanged -= ComboBox_SelectedValueChanged;
                this.m_CurrentNode.ComboBox.DropDownClosed -= ComboBox_DropDownClosed;
                this.m_CurrentNode.ComboBox.MouseUp -= ComboBox_DropDownMouseUp;
                this.m_CurrentNode.ComboBox.MouseDown -= ComboBox_DropDownMouseDown;

                this.m_CurrentNode.Text = this.m_CurrentNode.ComboBox.Text;
                this.m_CurrentNode.ForeColor = this.m_CurrentNode.ComboBox.ForeColor;


                this.m_CurrentNode.ComboBox.Hide();
                this.m_CurrentNode.ComboBox.DroppedDown = false;

                this.Controls.Remove(this.m_CurrentNode.ComboBox);

                this.m_CurrentNode = null;

                this.OnAfterSelect(null);

            }

        }
    }
}
