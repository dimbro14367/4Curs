using System.Linq;
using System.Windows;

namespace InsuranceApp
{
    public partial class MainWindow : Window
    {
        public static ins_Users CurrentUser { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            lblError.Text = string.Empty;

            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Введите логин и пароль.";
                return;
            }

            try
            {
                using (var db = new user30_dbEntities())
                {
                    var user = db.ins_Users
                        .Include("ins_Roles")
                        .FirstOrDefault(u => u.Login == login && u.PasswordHash == password);

                    if (user == null)
                    {
                        lblError.Text = "Неверный логин или пароль.";
                        return;
                    }

                    CurrentUser = user;
                    string roleName = user.ins_Roles != null ? user.ins_Roles.RoleName : "";

                    if (roleName == "Агент")
                    {
                        new AgentWindow().Show();
                    }
                    else if (roleName == "Андеррайтер")
                    {
                        new UnderwriterWindow().Show();
                    }
                    else if (roleName == "Администратор")
                    {
                        new AdminWindow().Show();
                    }
                    else
                    {
                        MessageBox.Show("Неизвестная роль пользователя: " + roleName);
                        return;
                    }

                    this.Close();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка подключения к БД:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}