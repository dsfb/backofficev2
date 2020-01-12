using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace backofficev2
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            string nome = this.txtNome.Text, senha = this.txtSenha.Text;

            if (nome.Length == 0)
            {
                MessageBox.Show("Erro ao fazer login: informe um nome válido de usuário!");
                return;
            }

            if (senha.Length == 0)
            {
                MessageBox.Show("Erro ao fazer login: informe uma senha válida de usuário!");
                return;
            }

            var adminBD = this.dominoBdDataSet.administradores;
            var rows = adminBD.Rows;
            int nomeIndex = 1;
            int senhaIndex = 2;
            DataRow selected_dr = null;

            foreach (DataRow dr in rows)
            {
                if (dr[nomeIndex].ToString() == nome &&
                    dr[senhaIndex].ToString() == senha)
                {
                    selected_dr = dr;
                }
            }

            if (selected_dr == null)
            {
                MessageBox.Show("Erro ao fazer login: Dado(s) de Usuário e/ou senha inválido(s)!");
                return;
            }
            else
            {
                this.Hide();
                FormMain f = new FormMain();
                f.FormClosed += (s, args) => this.Close();
                f.Show();
            }
        }

        private void administradoresBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.administradoresBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.dominoBdDataSet);

        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dominoBdDataSet.administradores' table. You can move, or remove it, as needed.
            this.administradoresTableAdapter.Fill(this.dominoBdDataSet.administradores);

        }
    }
}
