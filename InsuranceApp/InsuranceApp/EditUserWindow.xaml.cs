using System;
using System.Linq;
using System.Windows;

namespace InsuranceApp
{
    public partial class EditUserWindow : Window
    {
        private readonly ins_Users _editingUser;
        private readonly bool _isEditMode;

        public EditUserWindow(ins_Users user)
        {
            InitializeComponent();
            _editingUser = user;
            _isEditMode = user != null;

            lblTitle.Text = _isEditMode ? "Редактирование сотрудника" : "Новый сотрудник";
            if (_isEditMode)
            {
                lblPasswordHint.Text = "Пароль (оставьте пустым, чтобы не менять)";
                txtLogin.Text = user.Login;
                txtLastName.Text = user.LastName;
                txtFirstName.Text = user.FirstName;
                txtMiddleName.Text = user.MiddleName;
                txtPhone.Text = user.Phone;
            }

            LoadRoles();
        }

        private void LoadRoles()
        {
            using (var db = new user30_dbEntities())
            {
                var roles = db.ins_Roles.OrderBy(r => r.RoleName).ToList();
                cmbRole.ItemsSource = roles;

                if (_isEditMode && _editingUser.RoleID > 0)
                    cmbRole.SelectedValue = _editingUser.RoleID;
                else if (roles.Count > 0)
                    cmbRole.SelectedIndex = 0;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLogin.Text) ||
                cmbRole.SelectedValue == null ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Заполните обязательные поля: логин, роль, фамилия, имя.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!_isEditMode && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Введите пароль для нового сотрудника.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new user30_dbEntities())
                {
                    // Проверка на уникальность логина
                    bool loginTaken = db.ins_Users.Any(u =>
                        u.Login == txtLogin.Text.Trim() &&
                        (!_isEditMode || u.UserID != _editingUser.UserID));

                    if (loginTaken)
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует.",
                            "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (_isEditMode)
                    {
                        var dbUser = db.ins_Users.Find(_editingUser.UserID);
                        if (dbUser == null)
                        {
                            MessageBox.Show("Сотрудник не найден в базе данных.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        dbUser.Login = txtLogin.Text.Trim();
                        dbUser.RoleID = (int)cmbRole.SelectedValue;
                        dbUser.LastName = txtLastName.Text.Trim();
                        dbUser.FirstName = txtFirstName.Text.Trim();
                        dbUser.MiddleName = txtMiddleName.Text.Trim();
                        dbUser.Phone = txtPhone.Text.Trim();

                        if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                            dbUser.PasswordHash = txtPassword.Text.Trim();
                    }
                    else
                    {
                        var newUser = new ins_Users
                        {
                            Login = txtLogin.Text.Trim(),
                            PasswordHash = txtPassword.Text.Trim(),
                            RoleID = (int)cmbRole.SelectedValue,
                            LastName = txtLastName.Text.Trim(),
                            FirstName = txtFirstName.Text.Trim(),
                            MiddleName = txtMiddleName.Text.Trim(),
                            Phone = txtPhone.Text.Trim()
                        };
                        db.ins_Users.Add(newUser);
                    }

                    db.SaveChanges();
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}