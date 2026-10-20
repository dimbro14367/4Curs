using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InsuranceApp
{
    public partial class AgentWindow : Window
    {
        public AgentWindow()
        {
            InitializeComponent();
            Loaded += AgentWindow_Loaded;
        }

        private void AgentWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Профиль
            if (MainWindow.CurrentUser != null)
            {
                lblUserName.Text = $"{MainWindow.CurrentUser.LastName} {MainWindow.CurrentUser.FirstName}";
                lblRoleName.Text = MainWindow.CurrentUser.ins_Roles?.RoleName ?? "Агент";
            }
            lblClientsSubtitle.Text = $"Всего клиентов: {CountClients()}";
        }

        private int CountClients()
        {
            using (var db = new user30_dbEntities())
                return db.ins_Clients.Count();
        }

        // ---- Навигация ----
        private void ShowMenu()
        {
            pnlMenu.Visibility = Visibility.Visible;
            pnlClients.Visibility = Visibility.Collapsed;
        }

        private void ShowClients()
        {
            pnlMenu.Visibility = Visibility.Collapsed;
            pnlClients.Visibility = Visibility.Visible;
            LoadClients();
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e) => ShowMenu();

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }

        // ---- Действия ----
        private void LoadClients()
        {
            using (var db = new user30_dbEntities())
            {
                dgClients.ItemsSource = db.ins_Clients.OrderBy(c => c.LastName).ToList();
            }
            lblClientsSubtitle.Text = $"Всего клиентов: {CountClients()}";
        }

        private void BtnAddClient_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new AddClientWindow { Owner = this };
            if (wnd.ShowDialog() == true)
            {
                lblStatus.Text = "✅ Клиент успешно добавлен.";
                ShowClients();
            }
        }

        private void BtnCreateContract_Click(object sender, RoutedEventArgs e)
        {
            ShowClients();
            lblStatus.Text = "Выберите клиента из списка и нажмите «Оформить договор» справа.";

            // Показываем диалог выбора клиента
            var picker = new ContractPickerWindow { Owner = this };
            if (picker.ShowDialog() == true)
            {
                var wnd = new CreateContractWindow(picker.SelectedClientId) { Owner = this };
                if (wnd.ShowDialog() == true)
                    lblStatus.Text = "✅ Договор создан и отправлен на проверку.";
            }
        }

        private void BtnCreateCase_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new CreateCaseWindow { Owner = this };
            if (wnd.ShowDialog() == true)
                lblStatus.Text = "✅ Запрос на выплату отправлен.";
        }
    }
}