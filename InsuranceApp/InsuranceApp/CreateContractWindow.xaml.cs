using System;
using System.Linq;
using System.Windows;

namespace InsuranceApp
{
    public partial class CreateContractWindow : Window
    {
        private readonly int _clientId;

        public CreateContractWindow(int clientId)
        {
            InitializeComponent();
            _clientId = clientId;

            dpStartDate.SelectedDate = DateTime.Now;
            dpEndDate.SelectedDate = DateTime.Now.AddYears(1);
            txtContractNumber.Text = $"ДГ-{DateTime.Now:yyyy}-{new Random().Next(100, 999)}";

            LoadClientName();
            LoadTypes();
        }

        private void LoadClientName()
        {
            using (var db = new user30_dbEntities())
            {
                var client = db.ins_Clients.Find(_clientId);
                if (client != null)
                    lblClientName.Text = $"Клиент: {client.LastName} {client.FirstName} {client.MiddleName}";
            }
        }

        private void LoadTypes()
        {
            using (var db = new user30_dbEntities())
            {
                cmbInsuranceType.ItemsSource = db.ins_InsuranceTypes.ToList();
                if (cmbInsuranceType.Items.Count > 0)
                    cmbInsuranceType.SelectedIndex = 0;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtContractNumber.Text) ||
                cmbInsuranceType.SelectedValue == null ||
                !decimal.TryParse(txtInsuredSum.Text, out decimal insuredSum) ||
                !decimal.TryParse(txtPremium.Text, out decimal premium) ||
                dpStartDate.SelectedDate == null ||
                dpEndDate.SelectedDate == null)
            {
                MessageBox.Show("Проверьте корректность заполнения полей.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpEndDate.SelectedDate <= dpStartDate.SelectedDate)
            {
                MessageBox.Show("Дата окончания должна быть позже даты начала.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new user30_dbEntities())
                {
                    var contract = new ins_InsuranceContracts
                    {
                        ContractNumber = txtContractNumber.Text.Trim(),
                        ClientID = _clientId,
                        AgentID = MainWindow.CurrentUser.UserID,
                        TypeID = (int)cmbInsuranceType.SelectedValue,
                        ConclusionDate = DateTime.Now,
                        StartDate = dpStartDate.SelectedDate.Value,
                        EndDate = dpEndDate.SelectedDate.Value,
                        InsuredSum = insuredSum,
                        Premium = premium,
                        IsAccept = false
                    };
                    db.ins_InsuranceContracts.Add(contract);
                    db.SaveChanges();
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании договора:\n" + ex.Message,
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