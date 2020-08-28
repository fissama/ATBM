using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ATBM_1
{
    public partial class QuanLyBenhVien : Form
    {
        public QuanLyBenhVien()
        {
            InitializeComponent();
            PHIEUKHAMgrt.Enabled = false;
            PHIEUKHAMrvk.Enabled = false;

        }

        private void DanhSachUser()
        {
            OracleConnection con_ds = new OracleConnection();
            con_ds.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";
            DataSet dataSet_ds = new DataSet();
            OracleCommand cmd_ds;
            if (timkiemuserroletb.Text == "")
                cmd_ds = new OracleCommand("Select * from all_users", con_ds);
            else
                cmd_ds = new OracleCommand("Select * from all_users where username = '" + timkiemuserroletb.Text.ToUpper() + "'", con_ds);
            cmd_ds.CommandType = CommandType.Text;
            con_ds.Open();
            using (OracleDataReader reader = cmd_ds.ExecuteReader())
            {
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                //danhsachuserdg = null;
                danhsachuserdg.DataSource = dataTable;
            }

            //danh sách các role 
            OracleCommand cmd_role;
            if (timkiemuserroletb.Text == "")
                cmd_role = new OracleCommand("Select * from DBA_ROLES", con_ds);
            else
                cmd_role = new OracleCommand("Select * from DBA_ROLES where role = '" + timkiemuserroletb.Text.ToUpper() + "'", con_ds);

            using (OracleDataReader reader = cmd_role.ExecuteReader())
            {
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                danhsachroledg.DataSource = dataTable;
            }

            //danh sách role đã cấp cho user
            OracleCommand cmd_userrole;
            if (timkiemuserroletb.Text == "")
                cmd_userrole = new OracleCommand("Select * from dba_ROLE_PRIVS", con_ds);
            else
            {
                if (CheckUser(timkiemuserroletb.Text.ToUpper()) != 0) //có tồn tại user
                    cmd_userrole = new OracleCommand("Select * from dba_ROLE_PRIVS where granted_role<>'CONNECT' and grantee = '" + timkiemuserroletb.Text.ToUpper() + "'", con_ds);
                else //có tồn tại role, nếu không thì không có giá trị trả về
                    cmd_userrole = new OracleCommand("Select * from dba_ROLE_PRIVS where granted_role<>'CONNECT' and granted_role = '" + timkiemuserroletb.Text.ToUpper() + "'", con_ds);

            }


            using (OracleDataReader reader = cmd_userrole.ExecuteReader())
            {
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                danhsachuserroledg.DataSource = dataTable;
            }
            con_ds.Close();
        }

        private void ThongTinQuyen()
        {
            OracleConnection con_ttq = new OracleConnection();
            con_ttq.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";

            DataSet dataSet_ttq = new DataSet();
            OracleCommand cmd_ttq;
            if (timkiemusertb.Text == "")
                cmd_ttq = new OracleCommand("Select * from user_tab_privs ", con_ttq);
            else
                cmd_ttq = new OracleCommand("Select * from user_tab_privs  where grantee = '" + timkiemusertb.Text.ToUpper() + "'", con_ttq);
            //dba_sys_privs 
            //user_tab_privs
            cmd_ttq.CommandType = CommandType.Text;
            con_ttq.Open();
            using (OracleDataReader reader = cmd_ttq.ExecuteReader())
            {
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                thongtinquyendg.DataSource = dataTable;
            }

            //bảng lấy ra các cộ có quyền update
            OracleCommand cmd_update;
            if (timkiemusertb.Text == "")
                cmd_update = new OracleCommand("Select * from USER_COL_PRIVS_MADE", con_ttq);
            else
                cmd_update = new OracleCommand("Select * from USER_COL_PRIVS_MADE where grantee = '" + timkiemusertb.Text.ToUpper() + "'", con_ttq);

            using (OracleDataReader reader = cmd_update.ExecuteReader())
            {
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                thongtinupdatedg.DataSource = dataTable;
            }
            con_ttq.Close();
        }
        private void Admin_Load(object sender, EventArgs e)
        {
            DanhSachUser();
            ThongTinQuyen();

        }


        private void taorolebtn_Click(object sender, EventArgs e)
        {
            if (RoleNameTB.Text == "")
            {
                MessageBox.Show("Lỗi!! Chưa nhập tên role.");
                return;
            }
            OracleConnection con = new OracleConnection();
            con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";
            con.Open();
            /*OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "alter session set \"_ORACLE_SCRIPT\"=true;";
            OracleDataReader dr = cmd.ExecuteReader();*/

            if (CheckRole(RoleNameTB.Text.ToUpper()) == 0 && CheckUser(RoleNameTB.Text.ToUpper()) == 0)
            {
                OracleCommand cmd_themrole = new OracleCommand();
                cmd_themrole.Connection = con;
                cmd_themrole.CommandText = "alter session set \"_ORACLE_SCRIPT\"=true";
                cmd_themrole.CommandType = CommandType.Text;
                cmd_themrole.ExecuteNonQuery();
                cmd_themrole.CommandText = "create role " + RoleNameTB.Text.ToUpper();
                cmd_themrole.CommandType = CommandType.Text;
                cmd_themrole.ExecuteNonQuery();
                MessageBox.Show("Thêm thành công.");
                //load lại danh sách role
                DanhSachUser();
            }
            else
            {
                MessageBox.Show("Đã có tên role hoặc user tồn tại trong hệ thống.");
            }
            con.Close();
        }

        private void xoarolebtn_Click(object sender, EventArgs e)
        {
            if (RoleNameTB.Text == "")
            {
                MessageBox.Show("Lỗi!! Chưa nhập tên role.");
                return;
            }
            OracleConnection con = new OracleConnection();
            con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";
            con.Open();
            if (CheckRole(RoleNameTB.Text.ToUpper()) == 0)
            {
                MessageBox.Show("Role không tồn tại.");
            }
            else
            {
                OracleCommand cmd_xoarole = new OracleCommand();
                cmd_xoarole.Connection = con;
                cmd_xoarole.CommandText = "alter session set \"_ORACLE_SCRIPT\"=true";
                cmd_xoarole.CommandType = CommandType.Text;
                cmd_xoarole.ExecuteNonQuery();
                cmd_xoarole.CommandText = "drop role " + RoleNameTB.Text.ToUpper();
                cmd_xoarole.CommandType = CommandType.Text;
                cmd_xoarole.ExecuteNonQuery();
                MessageBox.Show("Xóa thành công.");
                //load lai Thong thin quye62n
                ThongTinQuyen();
                //load lại danh sách role
                DanhSachUser();
            }
            con.Close();

        }

        private void taouserbtn_Click(object sender, EventArgs e)
        {
            if (tendangnhaptb.Text == "" || matkhautb.Text == "")
            {
                MessageBox.Show("Lỗi! Chưa nhập đủ thông tin");
                return;
            }
            OracleConnection con = new OracleConnection();
            con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";
            con.Open();
            if (CheckRole(tendangnhaptb.Text.ToUpper()) == 0 && CheckUser(tendangnhaptb.Text.ToUpper()) == 0)
            {
                //tạo user
                OracleCommand cmd_themrole = new OracleCommand();
                cmd_themrole.Connection = con;
                cmd_themrole.CommandText = "alter session set \"_ORACLE_SCRIPT\"=true";
                cmd_themrole.CommandType = CommandType.Text;
                cmd_themrole.ExecuteNonQuery();
                cmd_themrole.CommandText = "create user " + tendangnhaptb.Text.ToUpper() + " identified by " + matkhautb.Text;
                cmd_themrole.CommandType = CommandType.Text;
                cmd_themrole.ExecuteNonQuery();

                //cấp quyền connect cho user 
                OracleCommand cmd_connect = new OracleCommand();
                cmd_connect.Connection = con;
                cmd_connect.CommandText = "grant connect to " + tendangnhaptb.Text.ToUpper();
                cmd_connect.CommandType = CommandType.Text;
                cmd_connect.ExecuteNonQuery();
                //load lại danh sách user
                MessageBox.Show("Tạo thành công.");
                DanhSachUser();

            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc role đã tồn tại trong hệ thống.");
            }
            con.Close();
        }

        private void xoauserbtn_Click(object sender, EventArgs e)
        {
            if (tendangnhaptb.Text == "")
            {
                MessageBox.Show("Lỗi! Chưa nhập đủ thông tin");
                return;
            }
            OracleConnection con = new OracleConnection();
            con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";
            con.Open();
            if (CheckUser(tendangnhaptb.Text.ToUpper()) == 0)
            {
                MessageBox.Show("Tên đăng nhập không tồn tại.");
            }
            else
            {
                OracleCommand cmd_xoauser = new OracleCommand();
                cmd_xoauser.Connection = con;
                cmd_xoauser.CommandText = "alter session set \"_ORACLE_SCRIPT\"=true";
                cmd_xoauser.CommandType = CommandType.Text;
                cmd_xoauser.ExecuteNonQuery();
                cmd_xoauser.CommandText = "drop user " + tendangnhaptb.Text.ToUpper() + " CASCADE";
                cmd_xoauser.CommandType = CommandType.Text;
                cmd_xoauser.ExecuteNonQuery();
                MessageBox.Show("Xóa thành công.");
                //load lại danh sách user
                DanhSachUser();
                //load lai Thong thin quye62n
                ThongTinQuyen();
            }
            con.Close();

        }

        private void doimatkhauntn_Click(object sender, EventArgs e)
        {
            if (tendangnhaptb.Text == "" || matkhautb.Text == "")
            {
                MessageBox.Show("Lỗi! Chưa nhập đủ thông tin");
                return;
            }
            OracleConnection con = new OracleConnection();
            con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";
            con.Open();
            if (CheckUser(tendangnhaptb.Text.ToUpper()) == 0)
            {
                MessageBox.Show("Tên đăng nhập không tồn tại.");
            }
            else
            {
                OracleCommand cmd_chinhuser = new OracleCommand();
                cmd_chinhuser.Connection = con;
                cmd_chinhuser.CommandText = "alter session set \"_ORACLE_SCRIPT\"=true";
                cmd_chinhuser.CommandType = CommandType.Text;
                cmd_chinhuser.ExecuteNonQuery();
                cmd_chinhuser.CommandText = "alter user " + tendangnhaptb.Text.ToUpper() + " identified by " + matkhautb.Text;
                cmd_chinhuser.CommandType = CommandType.Text;
                cmd_chinhuser.ExecuteNonQuery();
                MessageBox.Show("Đổi mật khẩu thành công.");
            }
            con.Close();
        }

        private void capbtb_Click(object sender, EventArgs e)
        {
            if (caproletb.Text == "" || chousertb.Text == "")
            {
                MessageBox.Show("Chưa nhập đủ thông tin!");
                return;
            }
            OracleConnection con = new OracleConnection();
            con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";
            con.Open();
            if (CheckRole(caproletb.Text.ToUpper()) == 0 && CheckUser(chousertb.Text.ToUpper()) == 0)
            {
                MessageBox.Show("Tên role hoặc tên user không tồn tại.");
            }
            else
            {
                OracleCommand cmd_caprolechouser = new OracleCommand();
                cmd_caprolechouser.Connection = con;
                cmd_caprolechouser.CommandText = "grant " + caproletb.Text.ToUpper() + " to " + chousertb.Text;
                cmd_caprolechouser.CommandType = CommandType.Text;
                cmd_caprolechouser.ExecuteNonQuery();
                MessageBox.Show(String.Format("Cấp role: {0} cho user:{1} thành công", caproletb.Text.ToUpper(), chousertb.Text.ToUpper()));
            }
            con.Close();
        }

        //kiểm tra view có tồn tại hay chưa
        private int CheckView(string viewname)
        {
            OracleConnection con = new OracleConnection();
            con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";
            con.Open();
            OracleCommand cmd_checkview = new OracleCommand();
            cmd_checkview.Connection = con;
            cmd_checkview.CommandText = "SELECT COUNT(*) FROM dba_views WHERE view_name = '" + viewname + "'";
            cmd_checkview.CommandType = CommandType.Text;
            object count = cmd_checkview.ExecuteScalar();
            if (count.ToString() == "0")
                return 0; //chưa tồn tại
            return 1; //đã tồn tại 
        }
        //kiểm tra role có tồn tại
        private int CheckRole(string rolename)
        {
            OracleConnection con = new OracleConnection();
            con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";
            con.Open();
            OracleCommand cmd_checkrole = new OracleCommand();
            cmd_checkrole.Connection = con;
            cmd_checkrole.CommandText = "select count(*) from DBA_ROLES where role = '" + rolename + "'";
            cmd_checkrole.CommandType = CommandType.Text;
            object count = cmd_checkrole.ExecuteScalar();
            if (count.ToString() == "0")
                return 0; //chưa tồn tại
            return 1; //đã tồn tại 
        }

        //kiểm tra user có tồn tại
        private int CheckUser(string username)
        {
            username = username.ToUpper();
            OracleConnection con = new OracleConnection();
            con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";
            con.Open();
            OracleCommand cmd_checkuser = new OracleCommand();
            cmd_checkuser.Connection = con;
            cmd_checkuser.CommandText = "select count(*) from all_users where username = '" + username + "'";
            cmd_checkuser.CommandType = CommandType.Text;
            object count = cmd_checkuser.ExecuteScalar();
            if (count.ToString() == "0")
                return 0; //chưa tồn tại
            return 1; //đã tồn tại 
        }

        private void btnINSERTgrt_Click(object sender, EventArgs e)
        {
            OracleConnection con = new OracleConnection();
            con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";
            if (capquyenrolerb.Checked)
            {
                if (CheckRole(tenuserroletb.Text.ToUpper()) == 0)
                {
                    MessageBox.Show("Role không tồn tại.");
                    con.Close();
                    return;
                }
            }
            else
            {
                if (CheckUser(tenuserroletb.Text.ToUpper()) == 0)
                {
                    MessageBox.Show("User không tồn tại.");
                    con.Close();
                    return;
                }
            }
            con.Open();
            if (BENHNHANgrtRB.Checked) //insert trên bảng BENHNHAN
            {
                OracleCommand cmd_insert = new OracleCommand();
                cmd_insert.Connection = con;
                cmd_insert.CommandText = "grant insert on BV.BENH_NHAN to " + tenuserroletb.Text.ToUpper();
                if (wgoBNcb.Checked)//có with grant option
                {
                    cmd_insert.CommandText += " WITH GRANT OPTION";
                }
                cmd_insert.CommandType = CommandType.Text;
                cmd_insert.ExecuteNonQuery();
                MessageBox.Show("Cấp quyền insert thành công");
                //load lại thong tin quyền
                ThongTinQuyen();
            }
            else //insert trên bảng PHIEUKHAM
            {
                OracleCommand cmd_insert = new OracleCommand();
                cmd_insert.Connection = con;
                cmd_insert.CommandText = "grant insert on BV.PHIEU_KHAM to " + tenuserroletb.Text.ToUpper();
                if (wgoPKcb.Checked)//có with grant option
                {
                    cmd_insert.CommandText += " WITH GRANT OPTION";
                }
                cmd_insert.CommandType = CommandType.Text;
                cmd_insert.ExecuteNonQuery();
                MessageBox.Show("Cấp quyền insert thành công");
                //load lại thong tin quyền
                ThongTinQuyen();
            }
            con.Close();
        }

        private void BENHNHANgrtRB_CheckedChanged(object sender, EventArgs e)
        {
            PHIEUKHAMgrt.Enabled = false;
            BENHNHANgrt.Enabled = true;
        }

        private void PHIEUKHAMgrtRB_CheckedChanged(object sender, EventArgs e)
        {
            BENHNHANgrt.Enabled = false;
            PHIEUKHAMgrt.Enabled = true;
        }

        private void btnDELETEgrt_Click(object sender, EventArgs e)
        {
            OracleConnection con = new OracleConnection();
            con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";

            if (capquyenrolerb.Checked)
            {
                if (CheckRole(tenuserroletb.Text.ToUpper()) == 0)
                {
                    MessageBox.Show("Role không tồn tại.");
                    con.Close();
                    return;
                }
            }
            else
            {
                if (CheckUser(tenuserroletb.Text.ToUpper()) == 0)
                {
                    MessageBox.Show("User không tồn tại.");
                    con.Close();
                    return;
                }
            }
            con.Open();
            if (BENHNHANgrtRB.Checked) //delete trên bảng BENHNHAN
            {
                OracleCommand cmd_delete = new OracleCommand();
                cmd_delete.Connection = con;
                cmd_delete.CommandText = "grant delete on BV.BENH_NHAN to " + tenuserroletb.Text.ToUpper();
                if (wgoBNcb.Checked)//có with grant option
                {
                    cmd_delete.CommandText += " WITH GRANT OPTION";
                }
                cmd_delete.CommandType = CommandType.Text;
                cmd_delete.ExecuteNonQuery();
                MessageBox.Show("Cấp quyền delete thành công");

                //load lại thong tin quyền
                ThongTinQuyen();
            }
            else //delete trên bảng PHIEUKHAM
            {
                OracleCommand cmd_delete = new OracleCommand();
                cmd_delete.Connection = con;
                cmd_delete.CommandText = "grant delete on BV.PHIEU_KHAM to " + tenuserroletb.Text.ToUpper();
                if (wgoPKcb.Checked)//có with grant option
                {
                    cmd_delete.CommandText += " WITH GRANT OPTION";
                }
                cmd_delete.CommandType = CommandType.Text;
                cmd_delete.ExecuteNonQuery();
                MessageBox.Show("Cấp quyền delete thành công");
                //load lại thong tin quyền
                ThongTinQuyen();
            }
            con.Close();
        }

        private void btnSELECTgrt_Click(object sender, EventArgs e)
        {
            OracleConnection con = new OracleConnection();
            con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";

            if (capquyenrolerb.Checked)
            {
                if (CheckRole(tenuserroletb.Text.ToUpper()) == 0)
                {
                    MessageBox.Show("Role không tồn tại.");
                    con.Close();
                    return;
                }
            }
            else
            {
                if (CheckUser(tenuserroletb.Text.ToUpper()) == 0)
                {
                    MessageBox.Show("User không tồn tại.");
                    con.Close();
                    return;
                }
            }
            con.Open();
            if (BENHNHANgrtRB.Checked) //select trên bảng BENHNHAN
            {
                OracleCommand cmd_select = new OracleCommand();
                cmd_select.Connection = con;
                if (BENHNHANgrt.CheckedItems.Count == 0 || BENHNHANgrt.CheckedItems.Count == 7) //cấp quyền select trên cả bảng
                {
                    cmd_select.CommandText = "grant select on BV.BENH_NHAN to " + tenuserroletb.Text.ToUpper();
                }
                else
                {
                    //những cột viết tắt để tạo view không bị lỗi view quá dài
                    string[] short_name = { "MBN", "TBN", "DC", "DT", "P", "NS", "C" };
                    //kiểm tra cấp quyền select trên những cột nào
                    string column = "";
                    //chọn ra những cột để select
                    string select_column = "";
                    for (int i = 0; i < BENHNHANgrt.Items.Count; i++)
                    {
                        if (BENHNHANgrt.GetItemCheckState(i) == CheckState.Checked)
                        {
                            column += short_name[i] + "_";
                            select_column += BENHNHANgrt.Items[i].ToString() + ",";
                        }
                    }
                    //xóa dấu , ở cuối (để bỏ váo câu lệnh select)
                    select_column = select_column.TrimEnd(',');

                    //them HS vào cuối để biết là view từ bàng BN
                    column += "BN";

                    //kiểm tra view có tồn tại hay chưa
                    if (CheckView(column) == 0)//chưa tồn tại
                    {
                        //tạo view mới
                        OracleCommand cmd_taoview = new OracleCommand();
                        cmd_taoview.Connection = con;
                        cmd_taoview.CommandText = "create view " + column + " as select " + select_column + " from BV.BENH_NHAN";
                        cmd_taoview.CommandType = CommandType.Text;
                        cmd_taoview.ExecuteNonQuery();
                    }
                    //cấp quyền đọc trên view này cho user
                    cmd_select.CommandText = "grant select on " + column + " to " + tenuserroletb.Text.ToUpper();
                }

                if (wgoBNcb.Checked)//có with grant option
                {
                    cmd_select.CommandText += " WITH GRANT OPTION";
                }
                cmd_select.CommandType = CommandType.Text;
                cmd_select.ExecuteNonQuery();
                MessageBox.Show("Cấp quyền select thành công");

                //load lại thong tin quyền
                ThongTinQuyen();
            }
            else //select trên bảng PHIEUKHAM
            {
                OracleCommand cmd_select = new OracleCommand();
                cmd_select.Connection = con;

                if (PHIEUKHAMgrt.CheckedItems.Count == 0 || PHIEUKHAMgrt.CheckedItems.Count == 4) //cấp quyền select trên cả bảng
                {
                    cmd_select.CommandText = "grant select on BV.PHIEU_KHAM to " + tenuserroletb.Text.ToUpper();
                }
                else
                {
                    //kiểm tra cấp quyền select trên những cột nào
                    //những cột viết tắt để tạo view không bị lỗi view quá dài
                    string[] short_name = { "NK", "TC", "TT", "MBN" };
                    //kiểm tra cấp quyền select trên những cột nào
                    string column = "";
                    //chọn ra những cột để select
                    string select_column = "";
                    for (int i = 0; i < PHIEUKHAMgrt.Items.Count; i++)
                    {
                        if (PHIEUKHAMgrt.GetItemCheckState(i) == CheckState.Checked)
                        {
                            column += short_name[i] + "_";
                            select_column += PHIEUKHAMgrt.Items[i].ToString() + ",";
                        }
                    }
                    //xóa dấu , ở cuối (để bỏ váo câu lệnh select)
                    select_column = select_column.TrimEnd(',');

                    //them  vào cuối để biết là view từ bàng PHIEUKHAM
                    column += "PK";

                    //kiểm tra view có tồn tại?
                    if (CheckView(column) == 0)//chưa tồn tại
                    {
                        //tạo view mới
                        OracleCommand cmd_taoview = new OracleCommand();
                        cmd_taoview.Connection = con;
                        cmd_taoview.CommandText = "create view " + column + " as select " + select_column + " from BV.PHIEU_KHAM";
                        cmd_taoview.CommandType = CommandType.Text;
                        cmd_taoview.ExecuteNonQuery();
                    }
                    cmd_select.CommandText = "grant select on " + column + " to " + tenuserroletb.Text.ToUpper();
                }
                if (wgoPKcb.Checked)//có with grant option
                {
                    cmd_select.CommandText += " WITH GRANT OPTION";
                }
                cmd_select.CommandType = CommandType.Text;
                cmd_select.ExecuteNonQuery();
                MessageBox.Show("Cấp quyền select thành công");
                //load lại thong tin quyền
                ThongTinQuyen();
            }
            con.Close();
        }

        private void btnUPDATEgrt_Click(object sender, EventArgs e)
        {
            OracleConnection con = new OracleConnection();
            con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";

            if (capquyenrolerb.Checked)
            {
                if (CheckRole(tenuserroletb.Text.ToUpper()) == 0)
                {
                    MessageBox.Show("Role không tồn tại.");
                    con.Close();
                    return;
                }
            }
            else
            {
                if (CheckUser(tenuserroletb.Text.ToUpper()) == 0)
                {
                    MessageBox.Show("User không tồn tại.");
                    con.Close();
                    return;
                }
            }
            con.Open();
            if (BENHNHANgrtRB.Checked) //update trên bảng BENHNHAN
            {
                OracleCommand cmd_update = new OracleCommand();
                cmd_update.Connection = con;
                if (BENHNHANgrt.CheckedItems.Count == 0 || BENHNHANgrt.CheckedItems.Count == 7) //cấp quyền upate trên cả bảng
                {
                    cmd_update.CommandText = "grant update on BV.BENH_NHAN to " + tenuserroletb.Text.ToUpper();
                }
                else
                {
                    //kiểm tra cấp quyền update trên những cột nào
                    string column = "";
                    for (int i = 0; i < BENHNHANgrt.Items.Count; i++)
                    {
                        if (BENHNHANgrt.GetItemCheckState(i) == CheckState.Checked)
                        {
                            column += BENHNHANgrt.Items[i].ToString() + ",";
                        }
                    }
                    //xóa dấu , ở cuối
                    column = column.TrimEnd(',');
                    cmd_update.CommandText = "grant update(" + column + ") on BV.BENH_NHAN to " + tenuserroletb.Text.ToUpper();
                }

                if (wgoBNcb.Checked)//có with grant option
                {
                    cmd_update.CommandText += " WITH GRANT OPTION";
                }
                cmd_update.CommandType = CommandType.Text;
                cmd_update.ExecuteNonQuery();
                MessageBox.Show("Cấp quyền update thành công");

                //load lại thong tin quyền
                ThongTinQuyen();
            }
            else //update trên bảng PHIEUKHAM
            {
                OracleCommand cmd_update = new OracleCommand();
                cmd_update.Connection = con;

                if (PHIEUKHAMgrt.CheckedItems.Count == 0 || PHIEUKHAMgrt.CheckedItems.Count == 4) //cấp quyền upate trên cả bảng
                {
                    cmd_update.CommandText = "grant update on BV.PHIEU_KHAM to " + tenuserroletb.Text.ToUpper();
                }
                else
                {
                    //kiểm tra cấp quyền update trên những cột nào
                    string column = "";
                    for (int i = 0; i < PHIEUKHAMgrt.Items.Count; i++)
                    {
                        if (PHIEUKHAMgrt.GetItemCheckState(i) == CheckState.Checked)
                        {
                            column += PHIEUKHAMgrt.Items[i].ToString() + ",";
                        }
                    }
                    //xóa dấu , ở cuối
                    column = column.TrimEnd(',');
                    cmd_update.CommandText = "grant update(" + column + ") on BV.PHIEU_KHAM to " + tenuserroletb.Text.ToUpper();
                }
                if (wgoPKcb.Checked)//có with grant option
                {
                    cmd_update.CommandText += " WITH GRANT OPTION";
                }
                cmd_update.CommandType = CommandType.Text;
                cmd_update.ExecuteNonQuery();
                MessageBox.Show("Cấp quyền update thành công");
                //load lại thong tin quyền
                ThongTinQuyen();
            }
            con.Close();
        }

        private void BENHNHANrvkRB_CheckedChanged(object sender, EventArgs e)
        {
            PHIEUKHAMrvk.Enabled = false;
            BENHNHANrvk.Enabled = true;
        }

        private void PHIEUKHAMrvkRB_CheckedChanged(object sender, EventArgs e)
        {
            PHIEUKHAMrvk.Enabled = true;
            BENHNHANrvk.Enabled = false;
        }

        //kiểm tra quyền có tồn tại
        private int CheckPrivilege(string tablename, string privilege, string grantee)
        {
            OracleConnection con = new OracleConnection();
            con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";
            con.Open();

            if (privilege != "UPDATE")
            {
                OracleCommand cmd_checkprivilege = new OracleCommand();
                cmd_checkprivilege.Connection = con;
                cmd_checkprivilege.CommandText = "select count(*) from user_tab_privs where table_name = '" + tablename + "' and privilege = '" + privilege + "' and grantee='" + grantee + "'";
                cmd_checkprivilege.CommandType = CommandType.Text;
                object count = cmd_checkprivilege.ExecuteScalar();
                if (count.ToString() == "0")
                    return 0; //chưa tồn tại
                return 1; //đã tồn tại 
            }
            else
            {
                //kiểm tra có cấp quyền UPDATE trên toàn bảng
                OracleCommand cmd_checkprivilege1 = new OracleCommand();
                cmd_checkprivilege1.Connection = con;
                cmd_checkprivilege1.CommandText = "select count(*) from user_tab_privs where table_name = '" + tablename + "' and privilege = '" + privilege + "' and grantee='" + grantee + "'";
                cmd_checkprivilege1.CommandType = CommandType.Text;
                object count1 = cmd_checkprivilege1.ExecuteScalar();

                //kiểm tra có cấp quyền UPDATE trên mức cột
                OracleCommand cmd_checkprivilege2 = new OracleCommand();
                cmd_checkprivilege2.Connection = con;
                cmd_checkprivilege2.CommandText = "select count(*) FROM USER_COL_PRIVS_MADE where table_name = '" + tablename + "' and privilege = '" + privilege + "' and grantee='" + grantee + "'";
                cmd_checkprivilege2.CommandType = CommandType.Text;
                object count2 = cmd_checkprivilege2.ExecuteScalar();

                if (count1.ToString() == "0" && count2.ToString() == "0") //hoàn toàn chưa được cấp quyền UPDATE
                    return 0; //chưa tồn tại
                return 1; //đã tồn tại 
            }
        }

        private void Revoke(string tablename, string privilege, string grantee)
        {
            OracleConnection con = new OracleConnection();
            con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";
            con.Open();
            OracleCommand cmd_revoke = new OracleCommand();
            cmd_revoke.Connection = con;
            cmd_revoke.CommandText = "revoke " + privilege + " on " + tablename + " from " + grantee;
            cmd_revoke.CommandType = CommandType.Text;
            cmd_revoke.ExecuteNonQuery();
            con.Close();
        }
        private void btnDELETErvk_Click(object sender, EventArgs e)
        {
            //OracleConnection con = new OracleConnection();
            //con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";
            if (thuquyenrolerb.Checked)
            {
                if (CheckRole(tenuserrolethutb.Text.ToUpper()) == 0)
                {
                    MessageBox.Show("Role không tồn tại.");
                    //con.Close();
                    return;
                }
            }
            else
            {
                if (CheckUser(tenuserrolethutb.Text.ToUpper()) == 0)
                {
                    MessageBox.Show("User không tồn tại.");
                    //con.Close();
                    return;
                }
            }
            if (BENHNHANrvkRB.Checked)//thu quyền delete trên bảng BENHNHAN
            {
                //kiểm tra có cấp quyền delete trên bảng BENHNHAN??
                if (CheckPrivilege("BV.BENH_NHAN", "DELETE", tenuserrolethutb.Text.ToUpper()) == 0)//chưa được cấp quyền
                {
                    MessageBox.Show("Chưa được cấp quyền nên không thể thu quyền.");
                    //con.Close();
                    return;
                }
                else
                {
                    //thực hiện thu quyền
                    Revoke("BV.BENH_NHAN", "DELETE", tenuserrolethutb.Text.ToUpper());
                    MessageBox.Show("Thu quyền thành công.");
                }

            }
            else //thu quyền delete trên bảng PHIEUKHAM
            {
                //kiểm tra có cấp quyền delete trên bảng PHIEUKHAM??
                if (CheckPrivilege("BV.PHIEU_KHAM", "DELETE", tenuserrolethutb.Text.ToUpper()) == 0)//chưa được cấp quyền
                {
                    MessageBox.Show("Chưa được cấp quyền nên không thể thu quyền.");
                    //con.Close();
                    return;
                }
                else
                {
                    //thực hiện thu quyền
                    Revoke("BV.PHIEU_KHAM", "DELETE", tenuserrolethutb.Text.ToUpper());
                    MessageBox.Show("Thu quyền thành công.");
                }
            }
            //load lại thong tin quyền
            ThongTinQuyen();
        }

        private void btnINSERTrvk_Click(object sender, EventArgs e)
        {
            if (thuquyenrolerb.Checked)
            {
                if (CheckRole(tenuserrolethutb.Text.ToUpper()) == 0)
                {
                    MessageBox.Show("Role không tồn tại.");
                    //con.Close();
                    return;
                }
            }
            else
            {
                if (CheckUser(tenuserrolethutb.Text.ToUpper()) == 0)
                {
                    MessageBox.Show("User không tồn tại.");
                    //con.Close();
                    return;
                }
            }
            if (BENHNHANrvkRB.Checked)//thu quyền INSERT trên bảng BENHNHAN
            {
                //kiểm tra có cấp quyền INSERT trên bảng BENHNHAN??
                if (CheckPrivilege("BV.BENH_NHAN", "INSERT", tenuserrolethutb.Text.ToUpper()) == 0)//chưa được cấp quyền
                {
                    MessageBox.Show("Chưa được cấp quyền nên không thể thu quyền.");
                    //con.Close();
                    return;
                }
                else
                {
                    //thực hiện thu quyền
                    Revoke("BV.BENH_NHAN", "INSERT", tenuserrolethutb.Text.ToUpper());
                    MessageBox.Show("Thu quyền thành công.");
                }

            }
            else //thu quyền INSERT trên bảng PHIEUKHAM
            {
                //kiểm tra có cấp quyền delete trên bảng PHIEUKHAM??
                if (CheckPrivilege("BV.PHIEU_KHAM", "INSERT", tenuserrolethutb.Text.ToUpper()) == 0)//chưa được cấp quyền
                {
                    MessageBox.Show("Chưa được cấp quyền nên không thể thu quyền.");
                    //con.Close();
                    return;
                }
                else
                {
                    //thực hiện thu quyền
                    Revoke("BV.PHIEU_KHAM", "INSERT", tenuserrolethutb.Text.ToUpper());
                    MessageBox.Show("Thu quyền thành công.");
                }
            }
            //load lại thong tin quyền
            ThongTinQuyen();
        }

        private void btnUPDATErvk_Click(object sender, EventArgs e)
        {
            if (thuquyenrolerb.Checked)
            {
                if (CheckRole(tenuserrolethutb.Text.ToUpper()) == 0)
                {
                    MessageBox.Show("Role không tồn tại.");
                    //con.Close();
                    return;
                }
            }
            else
            {
                if (CheckUser(tenuserrolethutb.Text.ToUpper()) == 0)
                {
                    MessageBox.Show("User không tồn tại.");
                    //con.Close();
                    return;
                }
            }
            if (BENHNHANrvkRB.Checked)//thu quyền UPDATE trên bảng BENHNHAN
            {
                //kiểm tra có cấp quyền delete trên bảng BENHNHAN??
                if (CheckPrivilege("BV.BENH_NHAN", "UPDATE", tenuserrolethutb.Text.ToUpper()) == 0)//chưa được cấp quyền
                {
                    MessageBox.Show("Chưa được cấp quyền nên không thể thu quyền.");
                    //con.Close();
                    return;
                }
                else
                {
                    //thực hiện thu quyền
                    Revoke("BV.BENH_NHAN", "UPDATE", tenuserrolethutb.Text.ToUpper());
                    MessageBox.Show("Thu quyền thành công.");
                }

            }
            else //thu quyền UPDATE trên bảng PHIEUKHAM
            {
                //kiểm tra có cấp quyền UPDATE trên bảng PHIEUKHAM?
                if (CheckPrivilege("BV.PHIEU_KHAM", "UPDATE", tenuserrolethutb.Text.ToUpper()) == 0)//chưa được cấp quyền
                {
                    MessageBox.Show("Chưa được cấp quyền nên không thể thu quyền.");
                    //con.Close();
                    return;
                }
                else
                {
                    //thực hiện thu quyền
                    Revoke("BV.PHIEU_KHAM", "UPDATE", tenuserrolethutb.Text.ToUpper());
                    MessageBox.Show("Thu quyền thành công.");
                }
            }
            //load lại thong tin quyền
            ThongTinQuyen();
        }

        private void btnSELECTrvk_Click(object sender, EventArgs e)
        {
            if (thuquyenrolerb.Checked)
            {
                if (CheckRole(tenuserrolethutb.Text.ToUpper()) == 0)
                {
                    MessageBox.Show("Role không tồn tại.");
                    //con.Close();
                    return;
                }
            }
            else
            {
                if (CheckUser(tenuserrolethutb.Text.ToUpper()) == 0)
                {
                    MessageBox.Show("User không tồn tại.");
                    //con.Close();
                    return;
                }
            }
            if (BENHNHANrvkRB.Checked)//thu quyền SELECT trên bảng BENHNHAN
            {
                //những cột viết tắt để tạo view không bị lỗi view quá dài
                string[] short_name = { "MBN", "TBN", "DC", "DT", "P", "NS", "C" };
                //kiểm tra cấp quyền select trên những cột nào
                string column = "";
                //nếu cấp quyền đọc trên toàn bảng
                if (BENHNHANrvk.CheckedItems.Count != 0 && BENHNHANrvk.CheckedItems.Count != 7)
                {
                    for (int i = 0; i < BENHNHANgrt.Items.Count; i++)
                    {
                        if (BENHNHANgrt.GetItemCheckState(i) == CheckState.Checked)
                        {
                            column += short_name[i] + "_";
                        }
                    }
                    //them HS vào cuối để biết là view từ bàng BN
                    column += "BN";
                }
                else
                    column = "BV.BENH_NHAN";

                //kiểm tra có cấp quyền select trên bảng column??
                if (CheckPrivilege(column, "SELECT", tenuserrolethutb.Text.ToUpper()) == 0)//chưa được cấp quyền
                {
                    MessageBox.Show("Chưa được cấp quyền nên không thể thu quyền.");
                    //con.Close();
                    return;
                }
                else
                {
                    //thực hiện thu quyền
                    Revoke(column, "SELECT", tenuserrolethutb.Text.ToUpper());
                    MessageBox.Show("Thu quyền thành công.");
                }

            }
            else //thu quyền SELECT trên bảng PHIEUKHAM
            {

                //kiểm tra cấp quyền select trên những cột nào
                //những cột viết tắt để tạo view không bị lỗi view quá dài
                string[] short_name = { "NK", "TC", "TT", "MBN" };
                //kiểm tra cấp quyền select trên những cột nào
                string column = "";
                if (PHIEUKHAMrvk.CheckedItems.Count != 0 && PHIEUKHAMrvk.CheckedItems.Count != 4)
                {
                    for (int i = 0; i < PHIEUKHAMrvk.Items.Count; i++)
                    {
                        if (PHIEUKHAMrvk.GetItemCheckState(i) == CheckState.Checked)
                        {
                            column += short_name[i] + "_";
                        }
                    }
                    //them  vào cuối để biết là view từ bàng PHIEUKHAM
                    column += "PK";
                }
                else
                    column = "BV.PHIEU_KHAM";

                //kiểm tra có cấp quyền select trên view column?
                if (CheckPrivilege(column, "SELECT", tenuserrolethutb.Text.ToUpper()) == 0)//chưa được cấp quyền
                {
                    MessageBox.Show("Chưa được cấp quyền nên không thể thu quyền.");
                    //con.Close();
                    return;
                }
                else
                {
                    //thực hiện thu quyền
                    Revoke(column, "SELECT", tenuserrolethutb.Text.ToUpper());
                    MessageBox.Show("Thu quyền thành công.");
                }
            }
            //load lại thong tin quyền
            ThongTinQuyen();
        }

        private void timkiemuserbtn_Click(object sender, EventArgs e)
        {
            ThongTinQuyen();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DanhSachUser();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OracleConnection con = new OracleConnection();
            con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";
            con.Open();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = con;
            cmd.CommandText = "alter system set audit_trail = db, extended scope = spfile";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            cmd.CommandText = "audit all";
            cmd.ExecuteNonQuery();
            con.Close();
            MessageBox.Show("Bật ghi nhật ký thành công!");
        }

        private void timkiemusertb_TextChanged(object sender, EventArgs e)
        {

        }

        private void viewAudit_Click(object sender, EventArgs e)
        {
            OracleConnection con = new OracleConnection();
            con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";

            con.Open();
            OracleCommand cmd = new OracleCommand();
            cmd = new OracleCommand("Select Timestamp,Username,owner,Action_Name,obj_name,Sql_Text,Returncode from DBA_Audit_Trail", con);

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                AuditGD.DataSource = dataTable;
            }
            con.Close();
        }

        private void turnOffAudit_Click(object sender, EventArgs e)
        {
            OracleConnection con = new OracleConnection();
            con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";

            con.Open();
            OracleCommand cmd = new OracleCommand();
            cmd = new OracleCommand("noaudit all", con);
            OracleDataReader reader = cmd.ExecuteReader();
            reader = cmd.ExecuteReader();
            MessageBox.Show("Tắt ghi nhật ký thành công!");
            con.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (viewAuditUserTB.Text == "")
                MessageBox.Show("Vui lòng không để trống tên User!");
            else if (CheckUser(viewAuditUserTB.Text) == 0)
                MessageBox.Show("Tên User không tồn tại!");
            else
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";

                con.Open();
                OracleCommand cmd = new OracleCommand();
                cmd = new OracleCommand("Select Timestamp,Username,owner,Action_Name,obj_name,Sql_Text,Returncode from DBA_Audit_Trail where Username = '" + viewAuditUserTB.Text.ToUpper() + "'", con);

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    AuditGD.DataSource = dataTable;
                }
                con.Close();
            }
        }

        private void TurnOnAuditUserB_Click(object sender, EventArgs e)
        {
            if (UserAudit.Text == "")
                MessageBox.Show("Vui lòng nhập tên User cần Audit!");
            else if (CheckUser(UserAudit.Text) == 0)
                MessageBox.Show("Tên User cần Audit không tồn tại!");
            else
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";
                con.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "alter system set audit_trail = db, extended scope = spfile";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.CommandText = "audit all by " + UserAudit.Text;
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Bật ghi nhật ký thành công!");
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {

            if (BenhNhanAudit.CheckedItems.Count == 0 && PhieuKhamAudit.CheckedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn lệnh cần Audit!");
            }
            else
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";

                con.Open();
                OracleCommand cmd = new OracleCommand();

                String audit = "audit ";
                String[] item = { "SELECT", "INSERT", "UPDATE", "DELETE" };
                if (BenhNhanAudit.CheckedItems.Count != 0)
                {
                    for (int i = 0; i < BenhNhanAudit.Items.Count; i++)
                    {
                        if (BenhNhanAudit.GetItemCheckState(i) == CheckState.Checked)
                        {
                            if (i != 0)
                                audit += ",";
                            audit = audit + item[i];
                        }
                    }
                    audit += " ON BV.benh_nhan";
                    if (AuditMode.Items.Count == 1)
                    {
                        for (int i = 0; i < AuditMode.Items.Count; i++)
                        {
                            if (BenhNhanAudit.GetItemCheckState(i) == CheckState.Checked)
                            {
                                if (i == 0)
                                    audit += " WHENEVER SUCCESSFUL";
                                else audit += " WHENEVER UNSUCCESSFUL";
                            }
                        }
                    }
                    cmd = new OracleCommand(audit, con);
                    OracleDataReader reader = cmd.ExecuteReader();
                    reader = cmd.ExecuteReader();
                }
                if (PhieuKhamAudit.CheckedItems.Count != 0)
                {
                    for (int i = 0; i < PhieuKhamAudit.Items.Count; i++)
                    {
                        if (PhieuKhamAudit.GetItemCheckState(i) == CheckState.Checked)
                        {
                            if (i != 0)
                                audit += ",";
                            audit = audit + item[i];
                        }
                    }
                    audit += " ON BV.PHIEU_KHAM";
                    if (AuditMode.Items.Count == 1)
                    {
                        for (int i = 0; i < AuditMode.Items.Count; i++)
                        {
                            if (BenhNhanAudit.GetItemCheckState(i) == CheckState.Checked)
                            {
                                if (i == 0)
                                    audit += " WHENEVER SUCCESSFUL";
                                else audit += " WHENEVER UNSUCCESSFUL";
                            }
                        }
                    }
                    cmd = new OracleCommand(audit, con);
                    OracleDataReader reader = cmd.ExecuteReader();
                    reader = cmd.ExecuteReader();
                }
                MessageBox.Show("Bật ghi nhật ký thành công!");
                con.Close();
            }
        }

        private void TurnOffAuditUserB_Click(object sender, EventArgs e)
        {
            if (UserAudit.Text == "")
                MessageBox.Show("Vui lòng nhập tên User cần tắt Audit!");
            else if (CheckUser(UserAudit.Text) == 0)
                MessageBox.Show("Tên User cần tắt Audit không tồn tại!");
            else
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";
                con.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "alter system set audit_trail = db, extended scope = spfile";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.CommandText = "noaudit all by " + UserAudit.Text;
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Tắt ghi nhật ký thành công!");
            }
        }

        private void TurnOffAuditTableB_Click(object sender, EventArgs e)
        {
            if (BenhNhanAudit.CheckedItems.Count == 0 && PhieuKhamAudit.CheckedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn lệnh cần tắt Audit!");
            }
            else
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";

                con.Open();
                OracleCommand cmd = new OracleCommand();

                String audit = "noaudit ";
                String[] item = { "SELECT", "INSERT", "UPDATE", "DELETE" };
                if (BenhNhanAudit.CheckedItems.Count != 0)
                {
                    for (int i = 0; i < BenhNhanAudit.Items.Count; i++)
                    {
                        if (BenhNhanAudit.GetItemCheckState(i) == CheckState.Checked)
                        {
                            if (i != 0)
                                audit += ",";
                            audit = audit + item[i];
                        }
                    }
                    audit += " ON BV.benh_nhan";
                    if (AuditMode.Items.Count == 1)
                    {
                        for (int i = 0; i < AuditMode.Items.Count; i++)
                        {
                            if (BenhNhanAudit.GetItemCheckState(i) == CheckState.Checked)
                            {
                                if (i == 0)
                                    audit += " WHENEVER SUCCESSFUL";
                                else audit += " WHENEVER UNSUCCESSFUL";
                            }
                        }
                    }
                    cmd = new OracleCommand(audit, con);
                    OracleDataReader reader = cmd.ExecuteReader();
                    reader = cmd.ExecuteReader();
                }
                if (PhieuKhamAudit.CheckedItems.Count != 0)
                {
                    for (int i = 0; i < PhieuKhamAudit.Items.Count; i++)
                    {
                        if (PhieuKhamAudit.GetItemCheckState(i) == CheckState.Checked)
                        {
                            if (i != 0)
                                audit += ",";
                            audit = audit + item[i];
                        }
                    }
                    audit += " ON BV.PHIEU_KHAM";
                    if (AuditMode.Items.Count == 1)
                    {
                        for (int i = 0; i < AuditMode.Items.Count; i++)
                        {
                            if (BenhNhanAudit.GetItemCheckState(i) == CheckState.Checked)
                            {
                                if (i == 0)
                                    audit += " WHENEVER SUCCESSFUL";
                                else audit += " WHENEVER UNSUCCESSFUL";
                            }
                        }
                    }
                    cmd = new OracleCommand(audit, con);
                    OracleDataReader reader = cmd.ExecuteReader();
                    reader = cmd.ExecuteReader();
                }
                MessageBox.Show("Tắt ghi nhật ký thành công!");
                con.Close();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(viewBNrb.Checked ==true)
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";

                con.Open();
                OracleCommand cmd = new OracleCommand();
                cmd = new OracleCommand("Select Timestamp,Username,owner,Action_Name,obj_name,Sql_Text,Returncode from DBA_Audit_Trail Where obj_name = 'BENH_NHAN'", con);

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    AuditGD.DataSource = dataTable;
                }
                con.Close();
            }
            else
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = @"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = DESKTOP-E7O6VVM)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)));User Id=system;Password=161299";

                con.Open();
                OracleCommand cmd = new OracleCommand();
                cmd = new OracleCommand("Select Timestamp,Username,owner,Action_Name,obj_name,Sql_Text,Returncode from DBA_Audit_Trail Where obj_name = 'PHIEU_KHAM'", con);

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    AuditGD.DataSource = dataTable;
                }
                con.Close();
            }
        }

        private void viewBNrb_CheckedChanged(object sender, EventArgs e)
        {
            button4.Visible = true;
        }

        private void viewPKrb_CheckedChanged(object sender, EventArgs e)
        {
            button4.Visible = true;
        }
    }
}

