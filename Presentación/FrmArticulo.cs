using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dominio;
using Negocio;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Presentación
{


    public partial class FrmArticulo : Form
    {
        private List<Articulo> listaArticulo;
        Articulo seleccionado;


        public FrmArticulo()
        {
            InitializeComponent();
        }


        private void FrmArticulo_Load(object sender, EventArgs e)
        {
            cargar();
            cbxCampo.Items.Add("Nombre");
            cbxCampo.Items.Add("Tipo");
            cbxCampo.Items.Add("Marca");
            cbxCampo.Items.Add("Precio");

        }

        private void cargar()
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                listaArticulo = negocio.listar();
                DgvCatalogo.DataSource = listaArticulo;
                PbxArticulo.Load(listaArticulo[0].ImagenUrl);
                ocultarColumnas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void ocultarColumnas()
        {
            DgvCatalogo.Columns["ImagenUrl"].Visible = false;
            DgvCatalogo.Columns["Id"].Visible = false;

        }

        private void DgvCatalogo_SelectionChanged(object sender, EventArgs e)
        {
            if (DgvCatalogo.CurrentRow != null)
            {
                Articulo seleccionado = (Articulo)DgvCatalogo.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.ImagenUrl);
            }

        }

        private void cargarImagen(string imagen)
        {
            try
            {
                PbxArticulo.Load(imagen);
            }
            catch (Exception ex)
            {
                PbxArticulo.Load("https://upload.wikimedia.org/wikipedia/commons/thumb/6/65/No-Image-Placeholder.svg/1665px-No-Image-Placeholder.svg.png");

            }

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            FrmAltaArticulo alta = new FrmAltaArticulo();
            alta.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (DgvCatalogo.CurrentRow != null)
            {
                try
                {
                    seleccionado = (Articulo)DgvCatalogo.CurrentRow.DataBoundItem;
                    FrmAltaArticulo modificar = new FrmAltaArticulo(seleccionado);
                    modificar.ShowDialog();
                    cargar();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }
            else
            {
                MessageBox.Show("Seleccione un artículo");
            }


        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();

            try
            {
                DialogResult respuesta = MessageBox.Show("Eliminará definitivamente el artículo", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (respuesta == DialogResult.Yes)
                {
                    seleccionado = (Articulo)DgvCatalogo.CurrentRow.DataBoundItem;
                    negocio.eliminar(seleccionado.Id);
                    cargar();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private bool validarFiltro()
        {
            if (cbxCampo.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione el campo para filtrar");
                return true;
            }
            if (cbxCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione el criterio para filtrar");
                return true;
            }
            if (cbxCampo.SelectedItem.ToString() == "Precio")
            {
                if (string.IsNullOrEmpty(txtFiltroAvanzado.Text))
                {
                    MessageBox.Show("Por favor, complete con números");
                    return true;
                }
                if (!(soloNumeros(txtFiltroAvanzado.Text)))
                {
                    MessageBox.Show("Por favor, cargue números");
                    return true;
                }

            }
            return false;
        }

        private bool soloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                {
                    return false;
                }
            }

            return true;
        }


        private void btnFiltro_Click_1(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();

            try
            {
                if (validarFiltro())
                {
                    return;
                }
                string campo = cbxCampo.SelectedItem.ToString();
                string criterio = cbxCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;
                DgvCatalogo.DataSource = negocio.filtrar(campo, criterio, filtro);
            }
            catch (Exception ex)
            {

                MessageBox.Show("Se ha producido el siguiente error: " + ex.ToString());
            }



        }

        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            List<Articulo> listaFiltrada;
            string filtro = txtFiltro.Text;
            if (filtro.Length >= 2)
            {
                listaFiltrada = listaArticulo.FindAll(x => x.Nombre.ToUpper().Contains(filtro.ToUpper()) || x.Marca.Descripcion.ToUpper().Contains(filtro.ToUpper()) || x.Tipo.Descripcion.ToUpper().Contains(filtro.ToUpper()));
            }
            else
            {
                listaFiltrada = listaArticulo;
            }


            DgvCatalogo.DataSource = null;
            DgvCatalogo.DataSource = listaFiltrada;
            ocultarColumnas();
        }

        private void cbxCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cbxCampo.SelectedItem.ToString();

            try
            {
                if (opcion == "Precio")
                {
                    cbxCriterio.Items.Clear();
                    cbxCriterio.Items.Add("Mayor a");
                    cbxCriterio.Items.Add("Menor a");
                    cbxCriterio.Items.Add("Igual a");
                }
                else
                {
                    cbxCriterio.Items.Clear();
                    cbxCriterio.Items.Add("Comienza con");
                    cbxCriterio.Items.Add("Termina con");
                    cbxCriterio.Items.Add("Contiene");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

     
        }

        private void DgvCatalogo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    DataGridViewCell celda = DgvCatalogo.Rows[e.RowIndex].Cells[e.ColumnIndex];


                    if (celda.OwningColumn.Name == "Descripcion")
                    {
                        string descripcion = celda.Value.ToString();

                        if (!string.IsNullOrEmpty(descripcion))
                        {
                            MessageBox.Show(descripcion, "Descripción", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
    }

}
