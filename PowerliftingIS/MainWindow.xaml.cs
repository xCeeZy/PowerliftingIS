using PowerliftingIS.AppData;
using PowerliftingIS.View.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PowerliftingIS
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CoachNameTb.Text = SessionManager.CurrentCoach.FullName;
            CoachRoleTb.Text = SessionManager.IsAdmin ? "Администратор" : "Тренер";

            MainFrame.Navigate(new AthletesPage());
            SetActiveButton(AthleteBtn);
        }

        private void SetActiveButton(Button ActiveBtn)
        {
            AthleteBtn.Style = (Style)FindResource("MenuButton");
            TrainingBtn.Style = (Style)FindResource("MenuButton");
            ResultBtn.Style = (Style)FindResource("MenuButton");
            ReportBtn.Style = (Style)FindResource("MenuButton");

            ActiveBtn.Style = (Style)FindResource("MenuButtonActive");
        }

        private void AthleteBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AthletesPage());
            SetActiveButton(AthleteBtn);
        }

        private void TrainingBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TrainingsPage());
            SetActiveButton(TrainingBtn);
        }

        private void ResultBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ResultsPage());
            SetActiveButton(ResultBtn);
        }

        private void ReportBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ReportPage());
            SetActiveButton(ReportBtn);
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Result = MessageBox.Show(
                "Вы уверены, что хотите выйти?",
                "Выход",
                MessageBoxButton.YesNo);

            if (Result == MessageBoxResult.Yes)
            {
                SessionManager.CurrentCoach = null;
                SessionManager.IsAdmin = false;

                View.Windows.LoginWindow Login = new View.Windows.LoginWindow();
                Login.Show();
                this.Close();
            }
        }
    }
}

