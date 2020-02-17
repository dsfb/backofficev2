using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace backofficev2
{
    public partial class FormAddAdmin : Form
    {
        private BackOfficeHandler bo = new BackOfficeHandler();

        public FormAddAdmin()
        {
            InitializeComponent();
        }

        private void btn_add_admin_Click(object sender, EventArgs e)
        {
            String name = this.txtNome.Text;
            String password = this.txtSenha.Text;
            String email = this.txtEmail.Text;

            if (String.IsNullOrEmpty(name))
            {
                MessageBox.Show("Erro ao inserir novo registro de Administrador: Nome de Administrador não fornecido!");
                return;
            }

            if (String.IsNullOrEmpty(password))
            {
                MessageBox.Show("Erro ao inserir novo registro de Administrador: Senha de Administrador não fornecida!");
                return;
            }

            if (password.Length > tamanho_maximo_senha)
            {
                MessageBox.Show("Erro ao inserir novo registro de Administrador: Senha inválida (com mais de 25 caracteres!)");
                return;
            }

            if (String.IsNullOrEmpty(email))
            {
                MessageBox.Show("Erro ao inserir novo registro de Administrador: E-mail de Administrador não fornecido!");
                return;
            }

            try
            {
                if (!this.bo.InsertAdmin(name, email, password))
                {
                    string erro = this.bo.GetErrorMessage();
                    MessageBox.Show("Erro ao inserir novo registro de Administrador: " + erro);
                }
                else
                {
                    FormMain.refresh_admin_table("FormAddUser");
                    this.Close();
                }
            }
            catch (Exception ex) when (ex is FormatException || ex is OverflowException)
            {
                MessageBox.Show("Erro ao inserir novo registro de Jogador: insira um valor válido de idade!");
            }
        }

        private static int tamanho_maximo_senha = 25;

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }        
    }
}
