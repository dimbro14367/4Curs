using System;
using System.Linq;
using System.Windows;

namespace InsuranceApp
{
    public partial class UnderwriterWindow : Window
    {
        public UnderwriterWindow()
        {
            InitializeComponent();
            Loaded += UnderwriterWindow_Loaded;

            dpReportFrom.SelectedDate = new DateTime(DateTime.Now.Year, 1, 1);
            dpReportTo.SelectedDate = DateTime.Now;
        }

        private void UnderwriterWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentUser != null)
            {
                lblUserName.Text = $"{MainWindow.CurrentUser.LastName} {MainWindow.CurrentUser.FirstName}";
                lblRoleName.Text = MainWindow.CurrentUser.ins_Roles?.RoleName ?? "Андеррайтер";
            }
            LoadData();
        }

        // ---- Навигация ----
        private void HideAll()
        {
            pnlMenu.Visibility = Visibility.Collapsed;
            pnlContracts.Visibility = Visibility.Collapsed;
            pnlCases.Visibility = Visibility.Collapsed;
            pnlReport.Visibility = Visibility.Collapsed;
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e) { HideAll(); pnlMenu.Visibility = Visibility.Visible; }
        private void BtnContracts_Click(object sender, RoutedEventArgs e) { HideAll(); pnlContracts.Visibility = Visibility.Visible; LoadData(); }
        private void BtnCases_Click(object sender, RoutedEventArgs e) { HideAll(); pnlCases.Visibility = Visibility.Visible; LoadData(); }
        private void BtnReport_Click(object sender, RoutedEventArgs e) { HideAll(); pnlReport.Visibility = Visibility.Visible; }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }

        // ---- Загрузка данных ----
        private void LoadData()
        {
            using (var db = new user30_dbEntities())
            {
                dgPendingContracts.ItemsSource = db.ins_InsuranceContracts
                    .Where(c => !c.IsAccept && c.RejectReason == null)
                    .ToList();

                dgPendingCases.ItemsSource = db.ins_InsuranceCases
                    .Where(c => !c.IsApproved)
                    .ToList();
            }
        }

        // ---- Решения по договорам ----
        private void BtnApproveContract_Click(object sender, RoutedEventArgs e)
        {
            if (dgPendingContracts.SelectedItem is ins_InsuranceContracts contract)
            {
                using (var db = new user30_dbEntities())
                {
                    var c = db.ins_InsuranceContracts.Find(contract.ContractID);
                    if (c != null) { c.IsAccept = true; c.RejectReason = null; db.SaveChanges(); }
                }
                LoadData();
                lblStatus.Text = "✅ Договор одобрен.";
            }
            else MessageBox.Show("Выберите договор из списка.");
        }

        private void BtnRejectContract_Click(object sender, RoutedEventArgs e)
        {
            if (dgPendingContracts.SelectedItem is ins_InsuranceContracts contract)
            {
                using (var db = new user30_dbEntities())
                {
                    var c = db.ins_InsuranceContracts.Find(contract.ContractID);
                    if (c != null) { c.IsAccept = false; c.RejectReason = "Отклонено андеррайтером"; db.SaveChanges(); }
                }
                LoadData();
                lblStatus.Text = "❌ Договор отклонён.";
            }
            else MessageBox.Show("Выберите договор из списка.");
        }

        // ---- Решения по выплатам ----
        private void BtnApproveCase_Click(object sender, RoutedEventArgs e)
        {
            if (dgPendingCases.SelectedItem is ins_InsuranceCases ic)
            {
                using (var db = new user30_dbEntities())
                {
                    var c = db.ins_InsuranceCases.Find(ic.CaseID);
                    if (c != null) { c.IsApproved = true; db.SaveChanges(); }
                }
                LoadData();
                lblStatus.Text = "✅ Выплата одобрена.";
            }
            else MessageBox.Show("Выберите случай из списка.");
        }

        private void BtnRejectCase_Click(object sender, RoutedEventArgs e)
        {
            if (dgPendingCases.SelectedItem is ins_InsuranceCases ic)
            {
                using (var db = new user30_dbEntities())
                {
                    var c = db.ins_InsuranceCases.Find(ic.CaseID);
                    if (c != null) { c.IsApproved = false; db.SaveChanges(); }
                }
                LoadData();
                lblStatus.Text = "❌ Выплата отклонена.";
            }
            else MessageBox.Show("Выберите случай из списка.");
        }

        // ---- Отчёт ----
        private void BtnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            if (dpReportFrom.SelectedDate == null || dpReportTo.SelectedDate == null)
            {
                MessageBox.Show("Выберите период.");
                return;
            }

            DateTime from = dpReportFrom.SelectedDate.Value.Date;
            DateTime to = dpReportTo.SelectedDate.Value.Date.AddDays(1);

            using (var db = new user30_dbEntities())
            {
                var report = db.ins_InsuranceCases
                    .Where(c => c.CaseDate >= from && c.CaseDate < to)
                    .OrderBy(c => c.CaseDate)
                    .ToList();

                dgReport.ItemsSource = report;

                decimal total = report
                    .Where(r => r.PaymentAmount.HasValue && r.IsApproved)
                    .Sum(r => r.PaymentAmount.Value);

                lblReportSum.Text = $"Итого выплат: {total:N2} ₽";
            }
        }
    }
}