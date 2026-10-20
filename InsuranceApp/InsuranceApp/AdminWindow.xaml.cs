using System;
using System.Linq;
using System.Windows;

namespace InsuranceApp
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            Loaded += AdminWindow_Loaded;
        }

        private void AdminWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentUser != null)
            {
                lblUserName.Text = $"{MainWindow.CurrentUser.LastName} {MainWindow.CurrentUser.FirstName}";
                lblRoleName.Text = MainWindow.CurrentUser.ins_Roles?.RoleName ?? "Администратор";
            }
            LoadUsers();
            LoadTypes();
        }

        // ---------- НАВИГАЦИЯ ----------
        private void HideAll()
        {
            pnlMenu.Visibility = Visibility.Collapsed;
            pnlUsers.Visibility = Visibility.Collapsed;
            pnlTypes.Visibility = Visibility.Collapsed;
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            HideAll();
            pnlMenu.Visibility = Visibility.Visible;
        }

        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            HideAll();
            pnlUsers.Visibility = Visibility.Visible;
            LoadUsers();
        }

        private void BtnTypes_Click(object sender, RoutedEventArgs e)
        {
            HideAll();
            pnlTypes.Visibility = Visibility.Visible;
            LoadTypes();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }

        // ---------- ЗАГРУЗКА ----------
        private void LoadUsers()
        {
            using (var db = new user30_dbEntities())
            {
                var users = db.ins_Users.Include("ins_Roles")
                                        .OrderBy(u => u.LastName)
                                        .ToList();
                dgUsers.ItemsSource = users;
                lblUsersSubtitle.Text = $"Всего сотрудников: {users.Count}";
            }
        }

        private void LoadTypes()
        {
            using (var db = new user30_dbEntities())
            {
                var types = db.ins_InsuranceTypes.OrderBy(t => t.TypeName).ToList();
                dgTypes.ItemsSource = types;
                lblTypesSubtitle.Text = $"Всего видов страхования: {types.Count}";
            }
        }

        // ---------- ПОЛЬЗОВАТЕЛИ ----------
        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new EditUserWindow(null) { Owner = this };
            if (wnd.ShowDialog() == true)
            {
                LoadUsers();
                lblStatus.Text = "✅ Сотрудник добавлен.";
            }
        }

        private void BtnEditUser_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is ins_Users user)
            {
                var wnd = new EditUserWindow(user) { Owner = this };
                if (wnd.ShowDialog() == true)
                {
                    LoadUsers();
                    lblStatus.Text = "✅ Данные сотрудника обновлены.";
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника из списка.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is ins_Users user)
            {
                var result = MessageBox.Show(
                    $"Сбросить пароль пользователя «{user.Login}» на значение «hash»?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                try
                {
                    using (var db = new user30_dbEntities())
                    {
                        var dbUser = db.ins_Users.Find(user.UserID);
                        if (dbUser != null)
                        {
                            dbUser.PasswordHash = "hash";
                            db.SaveChanges();
                        }
                    }
                    lblStatus.Text = $"✅ Пароль пользователя {user.Login} сброшен на «hash».";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка:\n" + ex.Message, "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника из списка.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is ins_Users user)
            {
                // Нельзя удалить самого себя
                if (user.UserID == MainWindow.CurrentUser.UserID)
                {
                    MessageBox.Show("Нельзя удалить собственную учётную запись.",
                        "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Нельзя удалить, если у сотрудника есть договоры
                using (var db = new user30_dbEntities())
                {
                    bool hasContracts = db.ins_InsuranceContracts.Any(c => c.AgentID == user.UserID);
                    if (hasContracts)
                    {
                        MessageBox.Show("Нельзя удалить сотрудника, у которого есть оформленные договоры.",
                            "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                var result = MessageBox.Show(
                    $"Удалить пользователя «{user.Login}»?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                try
                {
                    using (var db = new user30_dbEntities())
                    {
                        var dbUser = db.ins_Users.Find(user.UserID);
                        if (dbUser != null)
                        {
                            db.ins_Users.Remove(dbUser);
                            db.SaveChanges();
                        }
                    }
                    LoadUsers();
                    lblStatus.Text = "🗑 Сотрудник удалён.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении:\n" + ex.Message,
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника из списка.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // ---------- ВИДЫ СТРАХОВАНИЯ ----------
        private void BtnAddType_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new AddInsuranceTypeWindow { Owner = this };
            if (wnd.ShowDialog() == true)
            {
                LoadTypes();
                lblStatus.Text = "✅ Вид страхования добавлен.";
            }
        }

        private void BtnDeleteType_Click(object sender, RoutedEventArgs e)
        {
            if (dgTypes.SelectedItem is ins_InsuranceTypes type)
            {
                using (var db = new user30_dbEntities())
                {
                    bool usedInContracts = db.ins_InsuranceContracts.Any(c => c.TypeID == type.TypeID);
                    if (usedInContracts)
                    {
                        MessageBox.Show("Нельзя удалить вид страхования, который используется в договорах.",
                            "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                var result = MessageBox.Show(
                    $"Удалить вид страхования «{type.TypeName}»?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                try
                {
                    using (var db = new user30_dbEntities())
                    {
                        var dbType = db.ins_InsuranceTypes.Find(type.TypeID);
                        if (dbType != null)
                        {
                            db.ins_InsuranceTypes.Remove(dbType);
                            db.SaveChanges();
                        }
                    }
                    LoadTypes();
                    lblStatus.Text = "🗑 Вид страхования удалён.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении:\n" + ex.Message,
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите вид страхования из списка.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}