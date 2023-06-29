using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dominio;
using Negocio;
using static System.Net.Mime.MediaTypeNames;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;

namespace Presentación
{
    public partial class FrmAltaArticulo : Form
    {
        private Articulo articulo = null;
        private OpenFileDialog archivo = null;
        public FrmAltaArticulo()
        {
            InitializeComponent();
        }

        public FrmAltaArticulo(Articulo articulo)
        {
            InitializeComponent();
            this.articulo = articulo;
            Text = "Modificar artículo";
        }


        private void btnCancelar_Click(object sender, EventArgs e)
        {
           this.Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            
            try
            {
                if(!(validarCampos()))
                {
                    lblErrorCampo.Text = "Por favor, complete el campo marcado";

                    return;
                }

                if (articulo == null)
                    articulo = new Articulo();

                articulo.Codigo = txtCodigo.Text;
                articulo.Nombre = txtNombre.Text;
                articulo.Descripcion = txtDescripcion.Text;
                articulo.ImagenUrl = txtImagen.Text;
                articulo.Precio = decimal.Parse(txtPrecio.Text);
                articulo.Tipo = (Categoria)cboTipo.SelectedItem;
                articulo.Marca = (Marca)cboMarca.SelectedItem;


                if (articulo.Id != 0)
                {

                    negocio.modificar(articulo);
                    MessageBox.Show("Modificado exitosamente");
                    Close();

                } else
                {
                   
                    negocio.agregar(articulo);
                    MessageBox.Show("Agregado exitosamente");
                    Close();
                }

                if ((archivo != null && !(txtImagen.Text.ToUpper().Contains("HTTP"))))
                {
                    File.Copy(archivo.FileName, ConfigurationManager.AppSettings["Tpfinalapp"] + archivo.SafeFileName);
                
                }

                           
                Close();
            }
            catch (Exception ex )
            {

                MessageBox.Show("Se ha producido el siguiente error: " + ex.ToString());
            }
        }

        private void FrmAltaArticulo_Load(object sender, EventArgs e)
        {
           CategoriaNegocio categoriaNegocio = new CategoriaNegocio();
            MarcaNegocio marcaNegocio = new MarcaNegocio();

            try
            {
                cboTipo.DataSource = categoriaNegocio.listar();
                cboTipo.ValueMember = "Id";
                cboTipo.DisplayMember = "Descripcion";
                cboMarca.DataSource = marcaNegocio.listar();
                cboMarca.ValueMember = "Id";
                cboMarca.DisplayMember = "Descripcion";

             

                if (articulo != null)
                {
                    txtCodigo.Text = articulo.Codigo;
                    txtNombre.Text= articulo.Nombre;
                    txtDescripcion.Text= articulo.Descripcion;
                    txtImagen.Text = articulo.ImagenUrl;
                    cargarImagen(articulo.ImagenUrl);
                    txtPrecio.Text = articulo.ToString();     
                    cboTipo.SelectedValue= articulo.Tipo.Id;
                    cboMarca.SelectedValue = articulo.Marca.Id;                    
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Se ha producido el siguiente error: " + ex.ToString());
            }
        }

        private bool validarCampos()
        {
         

            if (string.IsNullOrEmpty(txtCodigo.Text))
            {
                txtCodigo.BackColor = Color.Red;
                return false;
            }

            if (string.IsNullOrEmpty(txtNombre.Text))
            {
                txtNombre.BackColor = Color.Red;
                return false;
            }

            if (string.IsNullOrEmpty(txtDescripcion.Text))
            {
                txtDescripcion.BackColor = Color.Red;
                return false;
            }

            if (string.IsNullOrEmpty(txtImagen.Text))
            {
                txtImagen.BackColor = Color.Red;
                return false;
            }
            decimal precio;
            if (!(decimal.TryParse(txtPrecio.Text, out precio)))
            {
                txtPrecio.BackColor = Color.Red;
                return false;
            }
                

            return true;
        }

        private void txtImagen_Leave(object sender, EventArgs e)
        {
            cargarImagen(txtImagen.Text);
        }

        private void cargarImagen(string imagen)
        {
            try
            {
                pbxImagen.Load(imagen);
            }
            catch (Exception ex)
            {
                pbxImagen.Load("https://upload.wikimedia.org/wikipedia/commons/thumb/6/65/No-Image-Placeholder.svg/1665px-No-Image-Placeholder.svg.png");

            }

        }

        private void btnAgregarImagen_Click(object sender, EventArgs e)
        {
            try
            {
                archivo = new OpenFileDialog();
                archivo.Filter = "jpg |*.jpg;|png|*.png|webp|*.webp";
                if (archivo.ShowDialog() == DialogResult.OK)
                {
                    txtImagen.Text = archivo.FileName;
                    cargarImagen(archivo.FileName);
                }
            }

            catch (Exception ex)
            {

                MessageBox.Show("Se ha producido el siguiente error: " + ex.ToString());
            }

        }
    }
}
