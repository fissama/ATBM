using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATBM_1
{
    public partial class DangNhap : Form
    {
        public DangNhap()
        {
            InitializeComponent();
        }

        private void DeathClick_Click(object sender, EventArgs e)
        {
            if (this.userTB.Text == "admin" && this.PasswordTB.Text == "admin")
            {
                QuanLyBenhVien newAmind = new QuanLyBenhVien();
                newAmind.Show();
                this.Visible = false;
            }
            else
            {
                OracleConnection con = DBConnection.GetConnection(this.userTB.Text, this.PasswordTB.Text);


                
                
            }
        }
    }
}
