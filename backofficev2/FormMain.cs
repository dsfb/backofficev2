using backofficev2.DominoBdDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static backofficev2.DominoBdDataSet;

namespace backofficev2
{
    public partial class FormMain : Form
    {
        private static FormMain instance = null;

        private BackOfficeHandler bo = new BackOfficeHandler();
        
        public FormMain()
        {
            InitializeComponent();
            this.numUpDownSetImg.Maximum = decimal.MaxValue;
            instance = this;
        }

        private void btnfindbackground_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fold = new FolderBrowserDialog();
            if(fold.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            MessageBox.Show("Test");
        }

        private void btnfindpeca_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fold = new FolderBrowserDialog();
            if (fold.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                MessageBox.Show("Test");
        }

        private void administradoresBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.administradoresBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.dominoBdDataSet);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dominoBdDataSet.img_fundo_tab' table. You can move, or remove it, as needed.
            this.img_fundo_tabTableAdapter.Fill(this.dominoBdDataSet.img_fundo_tab);
            // TODO: This line of code loads data into the 'dominoBdDataSet.img_peca' table. You can move, or remove it, as needed.
            this.img_pecaTableAdapter.Fill(this.dominoBdDataSet.img_peca);
            // TODO: This line of code loads data into the 'dominoBdDataSet.conj_img_pecas' table. You can move, or remove it, as needed.
            this.conj_img_pecasTableAdapter.Fill(this.dominoBdDataSet.conj_img_pecas);
            // TODO: This line of code loads data into the 'dominoBdDataSet.administradores' table. You can move, or remove it, as needed.
            this.administradoresTableAdapter.Fill(this.dominoBdDataSet.administradores);
        }

        private bool IsInvalidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address != email;
            }
            catch
            {
                return true;
            }
        }

        public static void refresh_admin_table(String sender)
        {
            if (sender.Equals("FormAddUser"))
            {
                instance.administradoresTableAdapter.Fill(instance.dominoBdDataSet.administradores);                
            }
        }

        private void btn_add_admin_Click(object sender, EventArgs e)
        {
            FormAddAdmin formAddUser = new FormAddAdmin();
            formAddUser.Show();
        }

        private DataRow GetSelectedDataRowAdminBd(string id)
        {
            DataRow selected_dr = null;

            var bRows = this.dominoBdDataSet.administradores.Where(r => r.RowState != DataRowState.Deleted);

            var results = from DataRow myRow in bRows
                          where myRow[0].ToString() == id
                          select myRow;

            if (results.Count() == 1)
            {
                selected_dr = results.First();
            }

            return selected_dr;
        }

        private bool IsValidAdminName(string name)
        {
            var bRows = this.dominoBdDataSet.administradores.Where(r => r.RowState != DataRowState.Deleted);

            var results = from DataRow myRow in bRows
                          where myRow[1].ToString() == name
                          select myRow;

            return results.Count() == 0;
        }

        private void GetNomeEmailSenhaDataRow(DataRow selected_dr, out string nome, 
            out string email, out string senha)
        {
            int nomeIndex = 1;
            int senhaIndex = 2;
            int emailIndex = 3;

            nome = selected_dr[nomeIndex].ToString();
            email = selected_dr[emailIndex].ToString();
            senha = selected_dr[senhaIndex].ToString();
        }

        private void btn_change_admin_Click(object sender, EventArgs e)
        {
            string idChange = this.txtIdChange.Text;

            if (idChange.Length == 0)
            {
                MessageBox.Show("Erro ao carregar os dados de edição de administrador! Preencha o ID do registro de administrador que será alterado!");
                return;
            }

            int id = -1;
            string nome = this.txtNome.Text, email = this.txtEmail.Text, 
                senha = this.txtSenha.Text;

            if (Int32.TryParse(idChange, out id))
            {
                DataRow selected_dr = this.GetSelectedDataRowAdminBd(idChange);

                if (selected_dr == null)
                {
                    MessageBox.Show("Erro ao carregar os dados de edição de administrador! Forneça um ID válido de um registro de administrador!");
                    return;
                }
                else if (nome.Length == 0 || email.Length == 0 || senha.Length == 0)
                {
                    this.GetNomeEmailSenhaDataRow(selected_dr, out nome,
                        out email, out senha);
                    this.txtNome.Text = nome;
                    this.txtSenha.Text = senha;
                    this.txtEmail.Text = email;

                    MessageBox.Show("Altere os dados deste registro e depois clique novamente no botão \"Alterar\"!");
                    return;
                }
                else
                {
                    int nomeIndex = 1;
                    int senhaIndex = 2;
                    int emailIndex = 3;

                    selected_dr.BeginEdit();
                    selected_dr[nomeIndex] = this.txtNome.Text;
                    selected_dr[senhaIndex] = this.txtSenha.Text;
                    selected_dr[emailIndex] = this.txtEmail.Text;
                    selected_dr.EndEdit();
                    selected_dr.AcceptChanges();
                    
                    if(this.bo.EditAdmin(id, this.txtNome.Text, this.txtEmail.Text,
                        this.txtSenha.Text))
                    {
                        MessageBox.Show("Mudança(s) feita(s) com sucesso no registro alterado de administrador!");
                        this.txtNome.Text = "";
                        this.txtSenha.Text = "";
                        this.txtEmail.Text = "";
                        this.txtIdChange.Text = "";
                    }
                    else
                    {
                        string erro = this.bo.GetErrorMessage();
                        MessageBox.Show("Erro ao editar o registro de Administrador: " + erro);
                    }
                }
            }
            else
            {
                MessageBox.Show("Erro ao carregar os dados de edição de administrador! Forneça um número inteiro para o ID de um registro de administrador!");
                return;
            }   
        }

        private void btn_remove_admin_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.administradoresDataGridView.SelectedRows.Count > 0)
                {
                    MessageBox.Show("O ID selecionado eh: " + this.administradoresDataGridView.SelectedRows[0].Cells[0].Value.ToString());
                    return;
                }
                else if (this.administradoresDataGridView.SelectedCells.Count > 0)
                {
                    MessageBox.Show("O ID selecionado eh: " + this.administradoresDataGridView.SelectedCells[0].OwningRow.Cells[0].Value.ToString());
                    return;
                }
                else
                {
                    MessageBox.Show("Selecione somente um único ID de admin a ser removido!");
                    return;
                }
            } catch (System.NullReferenceException)
            {
                MessageBox.Show("Selecione somente um único ID válido de admin a ser removido!");
                return;
            }
            
            
            string idRemove = this.txtIdRemove.Text;

            int id = -1;
            if (Int32.TryParse(idRemove, out id))
            {
                DataRow selected_dr = this.GetSelectedDataRowAdminBd(idRemove);

                if (selected_dr == null)
                {
                    MessageBox.Show("Erro ao carregar os dados de remoção de administrador! Forneça um ID válido de um registro de administrador!");
                    return;
                }
                else
                {
                    string nome, email, senha;
                    this.GetNomeEmailSenhaDataRow(selected_dr, out nome,
                        out email, out senha);
                    selected_dr.Delete();
                    if(this.bo.DeleteAdmin(id))
                    {
                        MessageBox.Show("O registro de administrador selecionado foi removido com sucesso!");
                        this.txtIdRemove.Text = "";
                    }
                    else
                    {
                        string erro = this.bo.GetErrorMessage();
                        MessageBox.Show("Erro ao excluir o registro de Administrador: " + erro);
                    }
                }
            }
            else
            {
                MessageBox.Show("Erro ao carregar os dados de remoção de administrador! Forneça um número inteiro para o ID de um registro de administrador!");
                return;
            }
        }

        private bool ValidateIdImgSet(int id)
        {
            var results = from DataRow myRow in this.dominoBdDataSet.conj_img_pecas.Rows
                          where Int32.Parse(myRow[0].ToString()) == id
                          select myRow;
            return results.Count() == 1;
        }

        private bool ValidateIdFundo(int id)
        {
            var results = from DataRow myRow in this.dominoBdDataSet.img_fundo_tab.Rows
                          where Int32.Parse(myRow[0].ToString()) == id
                          select myRow;
            return results.Count() == 1;
        }

        private bool ValidateImg(int idSet, int up, int down, int result)
        {
            var results = from DataRow myRow in this.dominoBdDataSet.img_peca.Rows
                          where Int32.Parse(myRow[1].ToString()) == idSet &&
                                Int32.Parse(myRow[3].ToString()) == up &&
                                Int32.Parse(myRow[4].ToString()) == down
                          select myRow;
            return results.Count() == result;
        }

        private bool ValidateSetName(string name)
        {
            var results = from DataRow myRow in this.dominoBdDataSet.conj_img_pecas.Rows
                          where myRow[1].ToString() == name
                          select myRow;
            return results.Count() == 0;
        }

        private bool ValidateBackgroundName(string name)
        {
            var results = from DataRow myRow in this.dominoBdDataSet.img_fundo_tab.Rows
                          where myRow[1].ToString() == name
                          select myRow;
            return results.Count() == 0;
        }

        private byte[] GetImgData(int idSet, int up, int down)
        {
            var results = from DataRow myRow in this.dominoBdDataSet.img_peca.Rows
                          where Int32.Parse(myRow[1].ToString()) == idSet &&
                                Int32.Parse(myRow[3].ToString()) == up &&
                                Int32.Parse(myRow[4].ToString()) == down
                          select myRow[2];
            return (byte[])results.First();
        }

        private byte[] GetImgFundoData(int id)
        {
            var results = from DataRow myRow in this.dominoBdDataSet.img_fundo_tab.Rows
                          where Int32.Parse(myRow[0].ToString()) == id
                          select myRow[1];
            return (byte[])results.First();
        }

        private string GetImgFilename(int idSet, int up, int down)
        {
            var results = from DataRow myRow in this.dominoBdDataSet.img_peca.Rows
                          where Int32.Parse(myRow[1].ToString()) == idSet &&
                                Int32.Parse(myRow[3].ToString()) == up &&
                                Int32.Parse(myRow[4].ToString()) == down
                          select myRow[5];
            return results.First().ToString();
        }

        private string GetImgFundoFilename(int id)
        {
            var results = from DataRow myRow in this.dominoBdDataSet.img_fundo_tab.Rows
                          where Int32.Parse(myRow[0].ToString()) == id
                          select myRow[2];
            return results.First().ToString();
        }

        private void btnAdminConjChgConj_Click(object sender, EventArgs e)
        {
            string newImgSet = this.txtNomeConj.Text;

            if (newImgSet.Length == 0)
            {
                MessageBox.Show("Forneça um novo nome válido de conjunto de imagens!");
                return;
            }

            int id = (int) this.numUpDownAdminIdSet.Value;
            
            if (!ValidateIdImgSet(id))
            {
                MessageBox.Show("Forneça um ID existente de conjunto de imagens!");
                return;
            }

            if (this.bo.EditImgSet(id, newImgSet))
            {
                MessageBox.Show("O registro de conjunto foi atualizado com sucesso!");
                this.txtNomeConj.Text = "";
                this.conj_img_pecasTableAdapter.Fill(this.dominoBdDataSet.conj_img_pecas);
            }
            else
            {
                string erro = this.bo.GetErrorMessage();
                MessageBox.Show("Erro ao editar o registro de conjunto: " + erro);
            }
        }

        private void btnAdminConjDelConj_Click(object sender, EventArgs e)
        {
            int id = (int)this.numUpDownAdminIdSet.Value;
            
            if (!ValidateIdImgSet(id))
            {
                MessageBox.Show("Forneça um ID existente de conjunto de imagens!");
                return;
            }

            if (this.bo.DeleteImgSet(id))
            {
                MessageBox.Show("O registro de conjunto foi excluído com sucesso!");
                this.conj_img_pecasTableAdapter.Fill(this.dominoBdDataSet.conj_img_pecas);
            }
            else
            {
                string erro = this.bo.GetErrorMessage();
                MessageBox.Show("Erro ao excluir o registro de conjunto: " + erro);
            }
        }

        private void btnUploadRemImg_Click(object sender, EventArgs e)
        {
            int id = (int)this.numUpDownSetImg.Value;

            if (!ValidateIdImgSet(id))
            {
                MessageBox.Show("Forneça um ID existente de conjunto de imagens!");
                return;
            }

            int down = (int)this.nudImgDown.Value;
            int up = (int)this.nudImgUp.Value;

            if (ValidateImg(id, up, down, 0))
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Image img = Image.FromFile(openFileDialog.FileName);


                    if (bo.InsertImg(img, up, down, Path.GetFileName(openFileDialog.FileName), id))
                    {
                        MessageBox.Show("O registro de imagem foi criado com sucesso!");
                        this.img_pecaTableAdapter.Fill(this.dominoBdDataSet.img_peca);
                    }
                    else
                    {
                        string erro = this.bo.GetErrorMessage();
                        MessageBox.Show("Erro ao criar o registro de imagem: " + erro);
                    }
                }
                else
                {
                    MessageBox.Show("Você desistiu de escolher uma imagem para Upload!");
                }
            }
            else
            {
                MessageBox.Show("Erro ao fazer upload de Imagem: peça já cadastrada para este conjunto!");
            }
        }

        private void btnMakeRemConj_Click(object sender, EventArgs e)
        {
            string nome_conj = this.txtNomeConj.Text;

            if (nome_conj.Length == 0)
            {
                MessageBox.Show("Forneça um nome válido de conjunto a ser criado!");
                return;
            }

            if (nome_conj.Length > 64)
            {
                MessageBox.Show("Forneça um nome de conjunto a ser criado, com até 64 caracteres, no máximo!");
                return;
            }

            if (this.ValidateSetName(nome_conj))
            {
                if (this.bo.InsertSet(nome_conj))
                {
                    MessageBox.Show("O registro de conjunto foi criado com sucesso!");
                    this.txtNomeConj.Text = "";
                    this.conj_img_pecasTableAdapter.Fill(this.dominoBdDataSet.conj_img_pecas);
                }
                else
                {
                    string erro = this.bo.GetErrorMessage();
                    MessageBox.Show("Erro ao criar o registro de conjunto: " + erro);
                }
            }
            else
            {
                MessageBox.Show("Erro ao criar o registro de conjunto: nome pré-existente de conjunto de peças!");
            }
        }

        private int GetIdImgSetAdminImg()
        {
            return (int)numUpDownSetImg.Value;
        }

        private int GetNumDownAdminImg()
        {
            return (int)nudImgDown.Value;
        }

        private int GetNumUpAdminImg()
        {
            return (int)nudImgUp.Value;
        }

        private void DisplayImgInPictureBox(byte[] dadosImg, PictureBox pb)
        {
            MemoryStream ms = new MemoryStream(dadosImg);
            pb.Image = Image.FromStream(ms);
            pb.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private bool ValidateSingleImgInDB(int id, int up, int down)
        {
            if (!ValidateIdImgSet(id))
            {
                MessageBox.Show("Forneça um ID existente de conjunto de imagens!");
                return false;
            }

            if (!ValidateImg(id, up, down, 1) && !ValidateImg(id, down, up, 1))
            {
                if (ValidateImg(id, up, down, 0) && ValidateImg(id, down, up, 0))
                {
                    MessageBox.Show("Erro: Imagem inexistente no Banco de Dados!");
                    return false;
                }
                else
                {
                    MessageBox.Show("Erro: Imagem duplicada no Banco de Dados!");
                    return false;
                }
            }

            return true;
        }

        private void btnShowSelImg_Click(object sender, EventArgs e)
        {
            int id = this.GetIdImgSetAdminImg();
            int up = this.GetNumUpAdminImg();
            int down = this.GetNumDownAdminImg();

            if (!ValidateSingleImgInDB(id, up, down))
            {
                return;
            }

            try
            {
                if (ValidateImg(id, up, down, 1))
                {
                    DisplayImgInPictureBox(GetImgData(id, up, down), picBoxSelImg);
                }
                else
                {
                    DisplayImgInPictureBox(GetImgData(id, down, up), picBoxSelImg);
                }

                MessageBox.Show("A imagem foi exibida com sucesso!");
            }
            catch (Exception)
            {
                MessageBox.Show("Ocorreu um erro e a imagem não foi exibida com sucesso!");
            }
        }

        private void btnDownImg_Click(object sender, EventArgs e)
        {
            int id = this.GetIdImgSetAdminImg();
            int up = this.GetNumUpAdminImg();
            int down = this.GetNumDownAdminImg();

            if (!ValidateSingleImgInDB(id, up, down))
            {
                return;
            }

            if (!ValidateImg(id, up, down, 1))
            {
                // Troca de valores entre duas variáveis!
                up = up + down;
                down = up - down;
                up = up - down;
            }

            try
            {
                byte[] dadosImg = GetImgData(id, up, down);
                string imgFileName = GetImgFilename(id, up, down);
                MemoryStream ms = new MemoryStream(dadosImg);
                Image img = Image.FromStream(ms);
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK && 
                    !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    Bitmap bmp = new Bitmap(img);
                    bmp.Save(Path.Combine(fbd.SelectedPath, imgFileName), img.RawFormat);
                    MessageBox.Show("A imagem da peça foi baixada com sucesso!");
                }
                else
                {
                    MessageBox.Show("Você decidiu não baixar a imagem da peça!");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ocorreu um erro e a imagem da peça não foi baixada com sucesso!");
            }
        }

        private void btnDelSelImg_Click(object sender, EventArgs e)
        {
            int id = this.GetIdImgSetAdminImg();
            int up = this.GetNumUpAdminImg();
            int down = this.GetNumDownAdminImg();

            if (!ValidateSingleImgInDB(id, up, down))
            {
                return;
            }

            if (!ValidateImg(id, up, down, 1))
            {
                // Troca de valores entre duas variáveis!
                up = up + down;
                down = up - down;
                up = up - down;
            }

            var results = from DataRow myRow in this.dominoBdDataSet.img_peca.Rows
                          where Int32.Parse(myRow[1].ToString()) == id &&
                                Int32.Parse(myRow[3].ToString()) == up &&
                                Int32.Parse(myRow[4].ToString()) == down
                          select myRow[0];
            int imgId = (int)results.First();

            if (this.bo.DeleteImg(imgId))
            {
                MessageBox.Show("O registro de imagem foi apagado com sucesso!");
                this.img_pecaTableAdapter.Fill(this.dominoBdDataSet.img_peca);                
            }
            else
            {
                string erro = this.bo.GetErrorMessage();
                MessageBox.Show("Erro ao apagar o registro de imagem: " + erro);
            }
        }

        private void btnChangeNomeFundo_Click(object sender, EventArgs e)
        {
            string nomeFundo = this.txtNomeFundo.Text;

            if (nomeFundo.Length == 0)
            {
                MessageBox.Show("Forneça um novo nome válido de fundo de tabuleiro!");
                return;
            }

            int id = (int)this.numUpDownIdFundo.Value;
            
            if (!ValidateIdFundo(id))
            {
                MessageBox.Show("Forneça um ID existente de fundo de tabuleiro!");
                return;
            }

            if (this.bo.EditImgFundo(id, nomeFundo))
            {
                MessageBox.Show("O registro de fundo de tabuleiro foi atualizado com sucesso!");
                this.txtNomeFundo.Text = "";
                this.img_fundo_tabTableAdapter.Fill(this.dominoBdDataSet.img_fundo_tab);                
            }
            else
            {
                string erro = this.bo.GetErrorMessage();
                MessageBox.Show("Erro ao editar o registro de fundo de tabuleiro: " + erro);
            }
        }

        private void btnUploadFundo_Click(object sender, EventArgs e)
        {
            string nomeFundo = this.txtNomeFundo.Text;

            if (nomeFundo.Length == 0)
            {
                MessageBox.Show("Forneça um novo nome válido de fundo de tabuleiro!");
                return;
            }

            if(ValidateBackgroundName(nomeFundo))
            {
                MessageBox.Show("Erro ao inserir novo fundo de tabuleiro: nome pré-existente de fundo de tabuleiro!");
                return;
            }

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Image img = Image.FromFile(openFileDialog.FileName);
                
                if (bo.InsertImgFundo(img, nomeFundo, Path.GetFileName(openFileDialog.FileName)))
                {
                    MessageBox.Show("O registro de imagem de fundo de tabuleiro foi criado com sucesso!");
                    this.img_fundo_tabTableAdapter.Fill(this.dominoBdDataSet.img_fundo_tab);
                }
                else
                {
                    string erro = this.bo.GetErrorMessage();
                    MessageBox.Show("Erro ao criar o registro de imagem de fundo: " + erro);
                }
            }
            else
            {
                MessageBox.Show("Você desistiu de escolher uma imagem de fundo para Upload!");
            }
        }

        private void btnDeleteFundo_Click(object sender, EventArgs e)
        {
            int id = (int)this.numUpDownIdFundo.Value;

            if (!ValidateIdFundo(id))
            {
                MessageBox.Show("Forneça um ID existente de fundo de tabuleiro!");
                return;
            }

            if (this.bo.DeleteImgFundo(id))
            {
                MessageBox.Show("O registro de imagem de fundo foi apagado com sucesso!");
                this.img_fundo_tabTableAdapter.Fill(this.dominoBdDataSet.img_fundo_tab);
            }
            else
            {
                string erro = this.bo.GetErrorMessage();
                MessageBox.Show("Erro ao apagar o registro de imagem de fundo: " + erro);
            }
        }

        private void btnShowFundo_Click(object sender, EventArgs e)
        {
            int id = (int)this.numUpDownIdFundo.Value;

            if (!ValidateIdFundo(id))
            {
                MessageBox.Show("Forneça um ID existente de fundo de tabuleiro!");
                return;
            }

            try
            {
                DisplayImgInPictureBox(GetImgFundoData(id), picBoxSelFundo);

                MessageBox.Show("A imagem do fundo foi exibida com sucesso!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um erro e a imagem do fundo não foi exibida com sucesso!");
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        private void btnDownloadFundo_Click(object sender, EventArgs e)
        {
            int id = (int)this.numUpDownIdFundo.Value;

            if (!ValidateIdFundo(id))
            {
                MessageBox.Show("Forneça um ID existente de fundo de tabuleiro!");
                return;
            }

            try
            {
                byte[] dadosImg = GetImgFundoData(id);
                string imgFileName = GetImgFundoFilename(id);
                MemoryStream ms = new MemoryStream(dadosImg);
                Image img = Image.FromStream(ms);
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK &&
                    !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    Bitmap bmp = new Bitmap(img);
                    bmp.Save(Path.Combine(fbd.SelectedPath, imgFileName), img.RawFormat);
                    MessageBox.Show("A imagem do fundo foi baixada com sucesso!");
                }
                else
                {
                    MessageBox.Show("Você decidiu não baixar a imagem do fundo!");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ocorreu um erro e a imagem do fundo não foi baixada com sucesso!");
            }
        }
    }
}
