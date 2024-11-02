using System;
using System.Data;
using System.Data.SqlClient; // Include SQL client
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;
namespace QLSV
{   
    public partial class Form1 : Form
    {
        // Update the connection string according to your database
        private string connectionString = @"Data Source=LAPTOP-028H0UE8;Initial Catalog=qlsv_2;Integrated Security=True;";

        public Form1()
        {
            InitializeComponent();
            button1.Click += new EventHandler(btFile_Click);

            LoadHeaderImage();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            LoadData(); // Load student data into DataGridView
            dgvEmployee.CellClick += dgvEmployee_CellClick;

            // Enable ID field for adding new students
            tbId.Enabled = true;

            // Initialize ComboBox with majors
            comboBoxMajor.Items.AddRange(new object[]
            {
                "Công nghệ thông tin",
                "Điện",
                "Ngôn ngữ Anh",
                "Cơ khí",
                "Quản trị kinh doanh",
                "Quản lý khách sạn",
                "Kế toán"
            });
            comboBoxMajor.SelectedIndex = 0; // Set default value for ComboBox

            comboBoxClassCode.Items.AddRange(new object[]
            {
                "CCQ2211A",
                "CCQ2211B",
                "CCQ2211C",
                "CCQ2211D",
            });
            comboBoxClassCode.SelectedIndex = 0;
        }

        // Load student data from the database
        private void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(@"Data Source=LAPTOP-028H0UE8;Initial Catalog=qlsv_2;Integrated Security=True"))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT MSSV AS Id, HoTen AS Name, GioiTinh AS Gender, Nganh AS Major, DiaChi AS Address, NgaySinh AS BirthDate, MaLop AS ClassCode FROM Students", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Gán DataTable cho DataGridView
                dgvEmployee.DataSource = dataTable;

                // Thiết lập các cột cho DataGridView
                dgvEmployee.Columns["Id"].HeaderText = "MSSV";
                dgvEmployee.Columns["Name"].HeaderText = "Họ Tên";
                dgvEmployee.Columns["Gender"].HeaderText = "Giới Tính";
                dgvEmployee.Columns["Major"].HeaderText = "Chuyên Ngành";
                dgvEmployee.Columns["Address"].HeaderText = "Địa Chỉ";
                dgvEmployee.Columns["BirthDate"].HeaderText = "Ngày Sinh";
                dgvEmployee.Columns["ClassCode"].HeaderText = "Mã Lớp";
                // Tùy chỉnh thêm nếu cần
                dgvEmployee.Columns["Id"].Width = 150;
                dgvEmployee.Columns["Name"].Width = 200;
                dgvEmployee.Columns["Gender"].Width = 50;
                dgvEmployee.Columns["Major"].Width = 150;
                dgvEmployee.Columns["Address"].Width = 200;
                dgvEmployee.Columns["BirthDate"].Width = 200;
                dgvEmployee.Columns["ClassCode"].Width = 200;
                dgvEmployee.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Đảm bảo chọn dòng
            }
        }

        private void btAddNew_Click(object sender, EventArgs e)
        {
            // Retrieve data from input fields
            string studentId = tbId.Text;
            string studentName = tbName.Text;
            DateTime birthDate = dateTimePicker1.Value; // Get birth date from DateTimePicker
            bool gender = ckGender.Checked;
            string major = comboBoxMajor.SelectedItem?.ToString(); // Get value from ComboBox
            string address = tbAddress.Text;
            string classcode = comboBoxClassCode.SelectedItem?.ToString();
            // Validate inputs
            if (string.IsNullOrEmpty(studentId) || studentId.Length != 10)
            {
                MessageBox.Show("Mã sinh viên phải có 10 ký tự.");
                return;
            }
            if (string.IsNullOrEmpty(studentName))
            {
                MessageBox.Show("Tên sinh viên không được để trống.");
                return;
            }
            if (birthDate >= DateTime.Now)
            {
                MessageBox.Show("Ngày sinh không hợp lệ.");
                return;
            }

            // Age validation
            int age = DateTime.Now.Year - birthDate.Year;
            if (birthDate > DateTime.Now.AddYears(-age)) age--; // Adjust for birth date

            if (age < 18 || age >= 100)
            {
                MessageBox.Show("Tuổi phải từ 18 đến dưới 100.");
                return;
            }

            if (string.IsNullOrEmpty(major))
            {
                MessageBox.Show("Ngành không được để trống.");
                return;
            }

            if (string.IsNullOrEmpty(classcode))
            {
                MessageBox.Show("Mã lớp không được để trống.");
                return;
            }
            // Check for uniqueness of Student ID
            foreach (DataGridViewRow row in dgvEmployee.Rows)
            {
                if (row.Cells["Id"].Value?.ToString() == studentId) // Update here to use the new column name
                {
                    MessageBox.Show("Mã sinh viên đã tồn tại. Vui lòng nhập mã khác.");
                    return;
                }
            }

            // Add student to the database
            AddStudentToDatabase(studentId, studentName, birthDate, gender, major, address, classcode);

            LoadData();

            // Clear input fields after adding
            ClearInputFields();
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            if (dgvEmployee.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvEmployee.SelectedRows)
                {
                    // Kiểm tra xem dòng có dữ liệu không
                    if (row.Cells["Id"].Value == null || string.IsNullOrEmpty(row.Cells["Id"].Value.ToString()))
                    {
                        MessageBox.Show("Dòng được chọn không có dữ liệu. Vui lòng chọn một dòng có dữ liệu để xóa.");
                        return;
                    }

                    // Lấy MSSV từ dòng đã chọn
                    string studentId = row.Cells["Id"].Value.ToString();

                    // Xóa sinh viên khỏi cơ sở dữ liệu
                    DeleteStudentFromDatabase(studentId);

                    // Xóa dòng khỏi DataGridView
                    dgvEmployee.Rows.Remove(row);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một hàng để xóa.");
            }
        }


        private void btEdit_Click(object sender, EventArgs e)
        {
            if (dgvEmployee.SelectedRows.Count == 1)
            {
                DataGridViewRow selectedRow = dgvEmployee.SelectedRows[0];
                string currentId = selectedRow.Cells["Id"].Value.ToString();

                // Bỏ điều kiện không cho phép sửa MSSV, chỉ cho phép không nhập ID mới
                if (tbId.Enabled == false)
                {
                    MessageBox.Show("MSSV không được phép sửa.");
                    return;
                }

                // Nếu người dùng không sửa ID, cho phép tiếp tục
                if (tbId.Text != currentId)
                {
                    // Có thể xử lý các trường hợp nếu người dùng đã sửa ID
                    MessageBox.Show("Bạn không thể sửa MSSV.");
                    return;
                }

                // Cập nhật các trường khác
                if (!string.IsNullOrEmpty(tbName.Text))
                {
                    selectedRow.Cells["Name"].Value = tbName.Text;
                }
                else
                {
                    MessageBox.Show("Tên sinh viên không được để trống.");
                    return;
                }

                DateTime birthDate = dateTimePicker1.Value;
                int age = DateTime.Now.Year - birthDate.Year;
                if (birthDate > DateTime.Now.AddYears(-age)) age--;

                if (age < 18 || age >= 100)
                {
                    MessageBox.Show("Tuổi phải từ 18 đến dưới 100.");
                    return;
                }

                selectedRow.Cells["Gender"].Value = ckGender.Checked;

                if (comboBoxMajor.SelectedItem != null)
                {
                    selectedRow.Cells["Major"].Value = comboBoxMajor.SelectedItem.ToString();
                }

                selectedRow.Cells["Address"].Value = tbAddress.Text;

                if (comboBoxClassCode.SelectedItem != null)
                {
                    selectedRow.Cells["ClassCode"].Value = comboBoxClassCode.SelectedItem.ToString();
                }

                // Cập nhật sinh viên vào cơ sở dữ liệu
                UpdateStudentInDatabase(currentId, tbName.Text, birthDate, ckGender.Checked, comboBoxMajor.SelectedItem?.ToString(), tbAddress.Text, comboBoxClassCode.SelectedItem?.ToString());

                // Tải lại dữ liệu để hiển thị các thay đổi mới nhất
                LoadData();

                ClearInputFields();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một hàng để chỉnh sửa.");
            }
        }




        private void dgvEmployee_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvEmployee.Rows[e.RowIndex].Cells["Id"].Value != null)
            {
                DataGridViewRow selectedRow = dgvEmployee.Rows[e.RowIndex];

                tbId.Text = selectedRow.Cells["Id"].Value.ToString();
                tbName.Text = selectedRow.Cells["Name"].Value?.ToString() ?? "";
                ckGender.Checked = selectedRow.Cells["Gender"].Value is bool gender && gender;

                string major = selectedRow.Cells["Major"].Value?.ToString() ?? "";
                if (comboBoxMajor.Items.Contains(major))
                {
                    comboBoxMajor.SelectedItem = major;
                }

                string classcode = selectedRow.Cells["ClassCode"].Value?.ToString() ?? "";
                if (comboBoxClassCode.Items.Contains(classcode))
                {
                    comboBoxClassCode.SelectedItem = classcode;
                }

                tbAddress.Text = selectedRow.Cells["Address"].Value?.ToString() ?? "";

                if (DateTime.TryParse(selectedRow.Cells["BirthDate"].Value?.ToString(), out DateTime birthDate))
                {
                    dateTimePicker1.Value = birthDate;
                }
                else
                {
                    dateTimePicker1.Value = DateTime.Now;
                }
            }
            else
            {
                ClearInputFields();
            }
        }

        private void AddStudentToDatabase(string studentId, string studentName, DateTime birthDate, bool gender, string major, string address,string classcode)
        {
            try
            {
                // Open connection to the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open(); // Open the connection

                    // Create SQL query
                    string query = "INSERT INTO Students (MSSV, HoTen, GioiTinh, Nganh, DiaChi, NgaySinh,MaLop) VALUES (@MSSV, @HoTen, @GioiTinh, @Nganh, @DiaChi, @NgaySinh, @MaLop)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@MSSV", studentId);
                        command.Parameters.AddWithValue("@HoTen", studentName);
                        command.Parameters.AddWithValue("@GioiTinh", gender);
                        command.Parameters.AddWithValue("@Nganh", major);
                        command.Parameters.AddWithValue("@DiaChi", address);
                        command.Parameters.AddWithValue("@NgaySinh", birthDate);
                        command.Parameters.AddWithValue("@MaLop", classcode);
                        // Execute the command
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Thêm sinh viên thành công.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm sinh viên: " + ex.Message);
            }
        }

        private void UpdateStudentInDatabase(string studentId, string studentName, DateTime birthDate, bool gender, string major, string address,string classcode)
        {
            try
            {
                // Open connection to the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open(); // Open the connection

                    // Create SQL query
                    string query = "UPDATE Students SET HoTen = @HoTen, GioiTinh = @GioiTinh, Nganh = @Nganh, DiaChi = @DiaChi, NgaySinh = @NgaySinh, MaLop =@MaLop WHERE MSSV = @MSSV";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@MSSV", studentId);
                        command.Parameters.AddWithValue("@HoTen", studentName);
                        command.Parameters.AddWithValue("@GioiTinh", gender);
                        command.Parameters.AddWithValue("@Nganh", major);
                        command.Parameters.AddWithValue("@DiaChi", address);
                        command.Parameters.AddWithValue("@NgaySinh", birthDate);
                        command.Parameters.AddWithValue("@MaLop", classcode);
                        // Execute the command
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Cập nhật thông tin sinh viên thành công.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật thông tin sinh viên: " + ex.Message);
            }
        }

        private void DeleteStudentFromDatabase(string studentId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open(); // Open the connection

                    string query = "DELETE FROM Students WHERE MSSV = @MSSV"; // Using MSSV to delete
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MSSV", studentId);
                        command.ExecuteNonQuery(); // Execute the delete command
                    }
                }

                MessageBox.Show("Xóa sinh viên thành công.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa sinh viên: " + ex.Message);
            }
        }

        private void ClearInputFields()
        {
            tbId.Clear();
            tbName.Clear();
            tbAddress.Clear();
            ckGender.Checked = false;
            comboBoxMajor.SelectedIndex = 0; // Reset to the first item
            dateTimePicker1.Value = DateTime.Now; // Reset to current date
            comboBoxClassCode.SelectedIndex = 0;
        }

        private void btFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"; // Expanded filter for various image formats

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage; // Set size mode
                    pictureBox1.ImageLocation = dlg.FileName; // Set image location
                }
            }
        }
        private void labelName_Click(object sender, EventArgs e)
        {
            // Logic của sự kiện ở đây
        }

        // Định nghĩa phương thức sự kiện cho labelGender_Click
        private void labelGender_Click(object sender, EventArgs e)
        {
            // Logic của sự kiện ở đây
        }

        // Định nghĩa phương thức sự kiện cho dgvEmployee_CellContentClick
        private void dgvEmployee_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Logic của sự kiện ở đây
        }

        // Định nghĩa phương thức sự kiện cho comboBoxMajor_SelectedIndexChanged
        private void comboBoxMajor_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Logic của sự kiện ở đây
        }

        // Định nghĩa phương thức sự kiện cho pictureBox1_Click
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Logic của sự kiện ở đây
        }

        // Định nghĩa phương thức sự kiện cho button1_Click
        private void button1_Click(object sender, EventArgs e)
        {
            // Logic của sự kiện ở đây
        }

        private void comboBoxClassCode_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void LoadHeaderImage()
        {
            // Đặt đường dẫn đến logo (thay đổi đường dẫn theo vị trí của logo trong dự án của bạn)
            string logoPath = @"D:\_A123\QLSV\QLSV\hitu.png"; // Đường dẫn đến tệp logo
            if (File.Exists(logoPath))
            {
                pictureBoxHeader.Image = Image.FromFile(logoPath);
            }
            else
            {
                MessageBox.Show("Không tìm thấy logo tại đường dẫn: " + logoPath);
            }
        }

        



        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
