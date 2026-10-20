using System;
using System.Linq;
using System.Windows;

namespace InsuranceApp
{
    public partial class CreateCaseWindow : Window
    {
        public CreateCaseWindow()
        {
            InitializeComponent();
            dpCaseDate.SelectedDate = DateTime.Now;
            LoadContracts();
        }

        private void LoadContracts()
        {
            using (var db = new user30_dbEntities())
            {
                var contracts = db.ins_InsuranceContracts
                    .Where(c => c.IsAccept)
                    .ToList();

                cmbContract.ItemsSource = contracts;
                if (contracts.Count > 0)
                    cmbContract.SelectedIndex = 0;
            }

            if (cmbContract.Items.Count == 0)
            {
                MessageBox.Show("Нет одобренных договоров. Сначала оформите и дождитесь одобрения договора.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbContract.SelectedValue == null ||
                string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Выберите договор и введите описание случая.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal? payment = null;
            if (!string.IsNullOrWhiteSpace(txtPaymentAmount.Text))
            {
                if (decimal.TryParse(txtPaymentAmount.Text, out decimal amount))
                    payment = amount;
                else
                {
                    MessageBox.Show("Сумма выплаты введена некорректно.",
                        "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            try
            {
                using (var db = new user30_dbEntities())
                {
                    var insuranceCase = new ins_InsuranceCases
                    {
                        ContractID = (int)cmbContract.SelectedValue,
                        CaseDate = dpCaseDate.SelectedDate ?? DateTime.Now,
                        Description = txtDescription.Text.Trim(),
                        PaymentAmount = payment,
                        IsApproved = false
                    };
                    db.ins_InsuranceCases.Add(insuranceCase);
                    db.SaveChanges();
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании запроса:\n" + ex.Message,
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