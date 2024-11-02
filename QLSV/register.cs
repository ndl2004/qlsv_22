using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace QLSV
{
    public partial class register : Form
    {
        public register()
        {
            InitializeComponent();
            LoadHeaderImage();
            // Thiết lập sự kiện cho LinkLabel
            this.linkLabel1.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
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

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Lấy giá trị nhập vào từ các TextBox
            string registeredUsername = tbUsername.Text;  // Tên đăng nhập
            string registeredPassword = tbPassword.Text;  // Mật khẩu
            string confirmPassword = tbConfirmPassword.Text;  // Xác nhận mật khẩu

            // Kiểm tra nếu mật khẩu xác nhận khớp
            if (registeredPassword == confirmPassword)
            {
                // Mã hóa mật khẩu
                string hashedPassword = HashPassword(registeredPassword);

                // Thêm tài khoản mới vào cơ sở dữ liệu
                using (SqlConnection connection = new SqlConnection("Data Source=LAPTOP-028H0UE8;Initial Catalog=qlsv_2;Integrated Security=True"))
                {
                    try
                    {
                        connection.Open();
                        string query = "INSERT INTO login (taikhoan, matkhau) VALUES (@username, @password)";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@username", registeredUsername);
                            command.Parameters.AddWithValue("@password", hashedPassword); // Lưu mật khẩu đã mã hóa

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Đăng ký thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Mở form đăng nhập sau khi đăng ký thành công
                                this.Hide();
                                login loginForm = new login();
                                loginForm.Show();
                            }
                            else
                            {
                                MessageBox.Show("Đăng ký không thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Mở form đăng nhập
            this.Hide();
            login loginForm = new login(); // Giả sử tên form đăng nhập là login
            loginForm.Show(); // Hiển thị form đăng nhập
        }

        // Các sự kiện không cần thiết đã được loại bỏ
        private void tbPassword_TextChanged(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void lblTitle_Click(object sender, EventArgs e) { }
        private void tbConfirmPassword_TextChanged(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
    }
}
