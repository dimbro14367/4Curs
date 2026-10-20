using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace InsuranceApp
{
    public partial class ContractPickerWindow : Window
    {
        public int SelectedClientId { get; private set; }
        public string SelectedClientName { get; private set; }

        private List<ins_Clients> _allClients;

        public ContractPickerWindow()
        {
            InitializeComponent();
            LoadClients();
        }

        private void LoadClients()
        {
            using (var db = new user30_dbEntities())
            {
                _allClients = db.ins_Clients.OrderBy(c => c.LastName).ToList();
            }
            dgClients.ItemsSource = _allClients;
        }

        private void TxtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_allClients == null) return;

            string query = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(query))
            {
                dgClients.ItemsSource = _allClients;
                return;
            }

            dgClients.ItemsSource = _allClients
                .Where(c =>
                    (c.LastName ?? "").ToLower().Contains(query) ||
                    (c.FirstName ?? "").ToLower().Contains(query) ||
                    (c.MiddleName ?? "").ToLower().Contains(query))
                .ToList();
        }

        private void DgClients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgClients.SelectedItem != null)
                Confirm();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (dgClients.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента из списка.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Confirm();
        }

        private void Confirm()
        {
            if (dgClients.SelectedItem is ins_Clients client)
            {
                SelectedClientId = client.ClientID;
                SelectedClientName = $"{client.LastName} {client.FirstName}";
                DialogResult = true;
                Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}