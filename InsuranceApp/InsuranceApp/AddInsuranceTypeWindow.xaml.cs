using System;
using System.Linq;
using System.Windows;

namespace InsuranceApp
{
    public partial class AddInsuranceTypeWindow : Window
    {
        public AddInsuranceTypeWindow()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string name = txtTypeName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите название вида страхования.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new user30_dbEntities())
                {
                    if (db.ins_InsuranceTypes.Any(t => t.TypeName == name))
                    {
                        MessageBox.Show("Такой вид страхования уже существует.",
                            "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    db.ins_InsuranceTypes.Add(new ins_InsuranceTypes { TypeName = name });
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