using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace backofficev2
{
    class BackOfficeHandler
    {
        private string ConString = "Data Source=den1.mssql1.gear.host;Persist Security Info=True;User ID=dominoeng3;Password=Sg68Vox_O0a?";
        private SqlConnection conn { get; set; }
        private string errorMessage { get; set; }

        public string GetErrorMessage()
        {
            return this.errorMessage;
        }

        private void OpenConnection()
        {
            try
            {
                if (this.conn != null)
                {
                    this.conn.Close();
                    this.conn.Dispose();
                }

                this.conn = new SqlConnection(this.ConString);
                this.conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Opa! Ocorreu um erro na operação 'OpenConnection'");
            }

        }

        private void CloseConnection()
        {
            this.conn.Close();
            this.conn.Dispose();
        }

        public bool InsertAdmin(string adminName, string adminEmail, string adminPassword)
        {
            try
            {
                this.OpenConnection();

                string sql = "INSERT INTO dominoeng3.dbo.administradores(Nome, Senha, Email) VALUES (@val_nome, @val_senha, @val_email);";

                SqlCommand cmd = new SqlCommand(sql, this.conn);
                cmd.Parameters.Add("@val_nome", SqlDbType.VarChar, 100);
                cmd.Parameters.Add("@val_senha", SqlDbType.VarChar, 25);
                cmd.Parameters.Add("@val_email", SqlDbType.VarChar, 50);
                cmd.Parameters["@val_nome"].Value = adminName;
                cmd.Parameters["@val_senha"].Value = adminPassword;
                cmd.Parameters["@val_email"].Value = adminEmail;             
                
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                this.CloseConnection();

                return true;
            }
            catch (Exception ex)
            {
                this.errorMessage = ex.Message;
                MessageBox.Show("Ocorreu um erro ao inserir um administrador no BD: " + ex.Message);

                this.CloseConnection();

                return false;
            }
        }

        public bool EditAdmin(int id, string adminName, string adminEmail, string adminPassword)
        {
            try
            {
                this.OpenConnection();

                string sql = "UPDATE dominoeng3.dbo.administradores SET Nome = @val_nome, Senha = @val_senha, Email = @val_email WHERE ID = @val_id;";

                SqlCommand cmd = new SqlCommand(sql, this.conn);
                cmd.Parameters.Add("@val_nome", SqlDbType.VarChar, 100);
                cmd.Parameters.Add("@val_senha", SqlDbType.VarChar, 25);
                cmd.Parameters.Add("@val_email", SqlDbType.VarChar, 50);
                cmd.Parameters.Add("@val_id", SqlDbType.Int);
                cmd.Parameters["@val_nome"].Value = adminName;
                cmd.Parameters["@val_senha"].Value = adminPassword;
                cmd.Parameters["@val_email"].Value = adminEmail;
                cmd.Parameters["@val_id"].Value = id;

                cmd.Prepare();

                cmd.ExecuteNonQuery();

                this.CloseConnection();

                return true;
            }
            catch (Exception ex)
            {
                this.errorMessage = ex.Message;
                MessageBox.Show("Ocorreu um erro ao atualizar um administrador no BD: " + ex.Message);

                this.CloseConnection();

                return false;
            }
        }

        public bool DeleteAdmin(int id)
        {
            try
            {
                this.OpenConnection();

                string sql = "DELETE FROM dominoeng3.dbo.administradores WHERE ID = @val_id;";

                SqlCommand cmd = new SqlCommand(sql, this.conn);
                cmd.Parameters.Add("@val_id", SqlDbType.Int);
                cmd.Parameters["@val_id"].Value = id;

                cmd.Prepare();

                cmd.ExecuteNonQuery();

                this.CloseConnection();

                return true;
            }
            catch (Exception ex)
            {
                this.errorMessage = ex.Message;
                MessageBox.Show("Ocorreu um erro ao excluir um administrador no BD: " + ex.Message);

                this.CloseConnection();

                return false;
            }
        }

        public bool InsertSet(string setName)
        {
            try
            {
                this.OpenConnection();
                
                string sql = "INSERT INTO dominoeng3.dbo.conj_img_pecas(nome_conjunto) VALUES (@val_nome);";

                SqlCommand cmd = new SqlCommand(sql, this.conn);
                cmd.Parameters.Add("@val_nome", SqlDbType.VarChar, 64);
                cmd.Parameters["@val_nome"].Value = setName;

                cmd.Prepare();

                cmd.ExecuteNonQuery();

                this.CloseConnection();

                return true;
            }
            catch (Exception ex)
            {
                this.errorMessage = ex.Message;
                MessageBox.Show("Ocorreu um erro ao inserir um conjunto no BD: " + ex.Message);

                this.CloseConnection();

                return false;
            }
        }

        public bool EditImgSet(int id, string setName)
        {
            try
            {
                this.OpenConnection();

                string sql = "UPDATE dominoeng3.dbo.conj_img_pecas SET nome_conjunto = @val_nome WHERE ID = @val_id;";

                SqlCommand cmd = new SqlCommand(sql, this.conn);
                cmd.Parameters.Add("@val_nome", SqlDbType.VarChar, 64);
                cmd.Parameters.Add("@val_id", SqlDbType.Int);
                cmd.Parameters["@val_nome"].Value = setName;
                cmd.Parameters["@val_id"].Value = id;

                cmd.Prepare();

                cmd.ExecuteNonQuery();

                this.CloseConnection();

                return true;
            }
            catch (Exception ex)
            {
                this.errorMessage = ex.Message;
                MessageBox.Show("Ocorreu um erro ao atualizar um conjunto de imagens no BD: " + ex.Message);

                this.CloseConnection();

                return false;
            }
        }

        public bool DeleteImgSet(int id)
        {
            try
            {
                this.OpenConnection();

                string sql = "DELETE FROM dominoeng3.dbo.conj_img_pecas WHERE ID = @val_id;";

                SqlCommand cmd = new SqlCommand(sql, this.conn);
                cmd.Parameters.Add("@val_id", SqlDbType.Int);
                cmd.Parameters["@val_id"].Value = id;

                cmd.Prepare();

                cmd.ExecuteNonQuery();

                this.CloseConnection();

                return true;
            }
            catch (Exception ex)
            {
                this.errorMessage = ex.Message;
                MessageBox.Show("Ocorreu um erro ao excluir um conjunto de imagens no BD: " + ex.Message);

                this.CloseConnection();

                return false;
            }
        }

        public bool InsertImg(Image img, int up, int down, string nome, int idImgSet)
        {
            try
            {
                byte[] arr;
                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, img.RawFormat);
                    arr = ms.ToArray();
                }

                this.OpenConnection();

                string sql = "INSERT INTO dominoeng3.dbo.img_peca(id_conjunto_pecas, conteudo_arquivo, peca_num_cima, peca_num_abaixo, nome_arquivo) VALUES(@val_id_img_set, @val_conteudo, @val_up, @val_down, @val_nome);";

                SqlCommand cmd = new SqlCommand(sql, this.conn);
                cmd.Parameters.Add("@val_id_img_set", SqlDbType.Int);
                cmd.Parameters.Add("@val_conteudo", SqlDbType.VarBinary, Int32.MaxValue);
                cmd.Parameters.Add("@val_up", SqlDbType.Int);
                cmd.Parameters.Add("@val_down", SqlDbType.Int);
                cmd.Parameters.Add("@val_nome", SqlDbType.VarChar, Int32.MaxValue);
                cmd.Parameters["@val_id_img_set"].Value = idImgSet;
                cmd.Parameters["@val_conteudo"].Value = arr;
                cmd.Parameters["@val_up"].Value = up;
                cmd.Parameters["@val_down"].Value = down;
                cmd.Parameters["@val_nome"].Value = nome;

                cmd.Prepare();

                cmd.ExecuteNonQuery();

                this.CloseConnection();

                return true;
            }
            catch (Exception ex)
            {
                this.errorMessage = ex.Message;
                MessageBox.Show("Ocorreu um erro ao inserir um conjunto no BD: " + ex.Message);

                this.CloseConnection();

                return false;
            }
        }

        public bool DeleteImg(int id)
        {
            try
            {
                this.OpenConnection();

                string sql = "DELETE FROM dominoeng3.dbo.img_peca WHERE ID = @val_id;";

                SqlCommand cmd = new SqlCommand(sql, this.conn);
                cmd.Parameters.Add("@val_id", SqlDbType.Int);
                cmd.Parameters["@val_id"].Value = id;

                cmd.Prepare();

                cmd.ExecuteNonQuery();

                this.CloseConnection();

                return true;
            }
            catch (Exception ex)
            {
                this.errorMessage = ex.Message;
                MessageBox.Show("Ocorreu um erro ao excluir uma imagem no BD: " + ex.Message);

                this.CloseConnection();

                return false;
            }
        }

        public bool InsertImgFundo(Image img, string nome_fundo, string nome_arquivo)
        {
            try
            {
                byte[] arr;
                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, img.RawFormat);
                    arr = ms.ToArray();
                }

                this.OpenConnection();

                string sql = "INSERT INTO dominoeng3.dbo.img_fundo_tab(nome_fundo, conteudo_arquivo, nome_arquivo) VALUES(@val_nome_fundo, @val_conteudo, @val_nome_file);";

                SqlCommand cmd = new SqlCommand(sql, this.conn);
                cmd.Parameters.Add("@val_nome_fundo", SqlDbType.VarChar, Int32.MaxValue);
                cmd.Parameters.Add("@val_conteudo", SqlDbType.VarBinary, Int32.MaxValue);
                cmd.Parameters.Add("@val_nome_file", SqlDbType.VarChar, Int32.MaxValue);
                cmd.Parameters["@val_nome_fundo"].Value = nome_fundo;
                cmd.Parameters["@val_conteudo"].Value = arr;
                cmd.Parameters["@val_nome_file"].Value = nome_arquivo;

                cmd.Prepare();

                cmd.ExecuteNonQuery();

                this.CloseConnection();

                return true;
            }
            catch (Exception ex)
            {
                this.errorMessage = ex.Message;
                MessageBox.Show("Ocorreu um erro ao inserir uma imagem de fundo no BD: " + ex.Message);

                this.CloseConnection();

                return false;
            }
        }

        public bool EditImgFundo(int id, string nomeFundo)
        {
            try
            {
                this.OpenConnection();

                string sql = "UPDATE dominoeng3.dbo.img_fundo_tab SET nome_fundo = @val_nome WHERE ID = @val_id;";

                SqlCommand cmd = new SqlCommand(sql, this.conn);
                cmd.Parameters.Add("@val_nome", SqlDbType.VarChar, Int32.MaxValue);
                cmd.Parameters.Add("@val_id", SqlDbType.Int);
                cmd.Parameters["@val_nome"].Value = nomeFundo;
                cmd.Parameters["@val_id"].Value = id;

                cmd.Prepare();

                cmd.ExecuteNonQuery();

                this.CloseConnection();

                return true;
            }
            catch (Exception ex)
            {
                this.errorMessage = ex.Message;
                MessageBox.Show("Ocorreu um erro ao atualizar um fundo de tabuleiro no BD: " + ex.Message);

                this.CloseConnection();

                return false;
            }
        }

        public bool DeleteImgFundo(int id)
        {
            try
            {
                this.OpenConnection();

                string sql = "DELETE FROM dominoeng3.dbo.img_fundo_tab WHERE ID = @val_id;";

                SqlCommand cmd = new SqlCommand(sql, this.conn);
                cmd.Parameters.Add("@val_id", SqlDbType.Int);
                cmd.Parameters["@val_id"].Value = id;

                cmd.Prepare();

                cmd.ExecuteNonQuery();

                this.CloseConnection();

                return true;
            }
            catch (Exception ex)
            {
                this.errorMessage = ex.Message;
                MessageBox.Show("Ocorreu um erro ao excluir uma imagem de fundo de tabuleiro no BD: " + ex.Message);

                this.CloseConnection();

                return false;
            }
        }
    }
}
