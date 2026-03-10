using PowerliftingIS.AppData;
using PowerliftingIS.Model;
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

namespace PowerliftingIS.View.Pages
{
    public partial class ResultAddPage : Page
    {
        public ResultAddPage()
        {
            InitializeComponent();

            AthleteCb.SelectedValuePath = "AthleteId";
            AthleteCb.DisplayMemberPath = "FullName";

            List<Athletes> AthletesList = new List<Athletes>();
            foreach (Athletes AthleteItem in App.context.Athletes.ToList())
            {
                bool MatchesRole = SessionManager.IsAdmin ||
                                   AthleteItem.CoachId == SessionManager.CurrentCoach.CoachId;
                if (MatchesRole)
                {
                    AthletesList.Add(AthleteItem);
                }
            }
            AthleteCb.ItemsSource = AthletesList;

            ExerciseCb.SelectedValuePath = "ExerciseId";
            ExerciseCb.DisplayMemberPath = "ExerciseName";
            ExerciseCb.ItemsSource = App.context.Exercises.ToList();

            CompetitionCb.SelectedValuePath = "CompetitionId";
            CompetitionCb.DisplayMemberPath = "CompetitionName";

            List<object> CompetitionsList = new List<object>();
            CompetitionsList.Add(new { CompetitionId = (int?)null, CompetitionName = "Без соревнования" });
            foreach (Competitions CompetitionItem in App.context.Competitions.ToList())
            {
                CompetitionsList.Add(CompetitionItem);
            }
            CompetitionCb.ItemsSource = CompetitionsList;
            CompetitionCb.SelectedIndex = 0;

            ResultDateDp.SelectedDate = System.DateTime.Today;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AthleteCb.SelectedItem == null ||
                ExerciseCb.SelectedItem == null ||
                string.IsNullOrEmpty(WeightTb.Text) ||
                ResultDateDp.SelectedDate == null)
            {
                MessageBox.Show("Заполните все обязательные поля");
            }
            else
            {
                decimal ResultWeight = decimal.Parse(WeightTb.Text.Replace('.', ','));

                int? SelectedCompetitionId = null;
                if (CompetitionCb.SelectedItem is Competitions)
                {
                    SelectedCompetitionId = (int?)CompetitionCb.SelectedValue;
                }

                Results NewResult = new Results()
                {
                    AthleteId = (int)AthleteCb.SelectedValue,
                    ExerciseId = (int)ExerciseCb.SelectedValue,
                    CompetitionId = SelectedCompetitionId,
                    ResultWeight = ResultWeight,
                    ResultDate = ResultDateDp.SelectedDate.Value,
                    IsPersonalRecord = RecordYesRb.IsChecked == true,
                    Note = string.IsNullOrEmpty(NoteTb.Text) ? null : NoteTb.Text
                };

                App.context.Results.Add(NewResult);
                App.context.SaveChanges();

                MessageBox.Show("Результат добавлен");
                NavigationService.Navigate(new ResultsPage());
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ResultsPage());
        }
    }
}