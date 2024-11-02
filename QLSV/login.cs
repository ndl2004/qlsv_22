using System;
using System.Data.SqlClient;
using System.Drawing; // Thêm using này
using System.IO; // Thêm using này
using System.Reflection; // Thêm using này
using System.Security.Cryptography; // Thêm using này
using System.Text; // Thêm using này
using System.Windows.Forms;

namespace QLSV
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
            LoadHeaderImage(); // Gọi hàm để tải ảnh header
        }

        private void LoadHeaderImage()
        {
            try
            {
                // Tên tài nguyên của ảnh nhúng (thay đổi nếu cần thiết)
                var resourceName = "QLSV.sv_header_login.png"; // Đảm bảo đường dẫn chính xác

                // Lấy assembly hiện tại
                var assembly = Assembly.GetExecutingAssembly();

                // Tìm tài nguyên
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        // Tạo một Image từ stream
                        Image headerImage = Image.FromStream(stream);

                        // Thiết lập ảnh làm background cho một label
                        Label headerLabel = new Label
                        {
                            Image = headerImage,
                            Size = new Size(headerImage.Width, headerImage.Height),
                            Location = new Point((this.ClientSize.Width - headerImage.Width) / 2, 10), // Vị trí
                            AutoSize = false
                        };

                        // Thêm label vào form
                        this.Controls.Add(headerLabel);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy tài nguyên ảnh.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải ảnh: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Kiểm tra dữ liệu người dùng nhập
            if (string.IsNullOrWhiteSpace(tbUsername.Text))
            {
                MessageBox.Show("Bạn hãy nhập tên đăng nhập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbUsername.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(tbPassword.Text))
            {
                MessageBox.Show("Bạn hãy nhập mật khẩu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbPassword.Focus();
                return;
            }

            // Mã hóa mật khẩu nhập vào
            string hashedPassword = HashPassword(tbPassword.Text.Trim());

            DBConnection.Connect(); // Mở kết nối tới cơ sở dữ liệu
            string sql = "SELECT COUNT(1) FROM login WHERE taikhoan = @taikhoan AND matkhau = @matkhau";

            using (SqlCommand cmd = new SqlCommand(sql, DBConnection.conn))
            {
                cmd.Parameters.AddWithValue("@taikhoan", tbUsername.Text.Trim());
                cmd.Parameters.AddWithValue("@matkhau", hashedPassword); // Sử dụng mật khẩu đã mã hóa

                try
                {
                    int result = Convert.ToInt32(cmd.ExecuteScalar());
                    if (result == 1)
                    {
                        // Đăng nhập thành công
                        Form1 main = new Form1();
                        main.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Tài khoản hoặc mật khẩu không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    DBConnection.Disconnect(); // Ngắt kết nối sau khi hoàn tất
                }
            }
        }

        // Hàm mã hóa mật khẩu
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Chuyển đổi chuỗi mật khẩu thành mảng byte
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Chuyển đổi mảng byte thành chuỗi hex
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // Chuyển đổi từng byte thành dạng hex
                }
                return builder.ToString(); // Trả về chuỗi mật khẩu đã mã hóa
            }
        }
    }
}
