using System;
using System.Windows;

namespace InsuranceApp
{
    public partial class AddClientWindow : Window
    {
        public AddClientWindow()
        {
            InitializeComponent();
            dpBirthDate.SelectedDate = DateTime.Now.AddYears(-30);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                dpBirthDate.SelectedDate == null)
            {
                MessageBox.Show("Заполните обязательные поля: фамилия, имя, дата рождения.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new user30_dbEntities())
                {
                    var client = new ins_Clients
                    {
                        LastName = txtLastName.Text.Trim(),
                        FirstName = txtFirstName.Text.Trim(),
                        MiddleName = txtMiddleName.Text.Trim(),
                        BirthDate = dpBirthDate.SelectedDate.Value,
                        PassportSeries = txtPassportSeries.Text.Trim(),
                        PassportNumber = txtPassportNumber.Text.Trim(),
                        Address = txtAddress.Text.Trim(),
                        Phone = txtPhone.Text.Trim()
                    };
                    db.ins_Clients.Add(client);
                    db.SaveChanges();
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении клиента:\n" + ex.Message,
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