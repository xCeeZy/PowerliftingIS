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
    public partial class ResultsPage : Page
    {
        public ResultsPage()
        {
            InitializeComponent();

            AthleteFilterCb.SelectedValuePath = "AthleteId";
            AthleteFilterCb.DisplayMemberPath = "FullName";

            List<Athletes> AthletesList = new List<Athletes>();
            AthletesList.Add(new Athletes() { AthleteId = 0, FullName = "Все спортсмены" });
            foreach (Athletes AthleteItem in App.context.Athletes.ToList())
            {
                AthletesList.Add(AthleteItem);
            }
            AthleteFilterCb.ItemsSource = AthletesList;
            AthleteFilterCb.SelectedIndex = 0;

            ExerciseFilterCb.SelectedValuePath = "ExerciseId";
            ExerciseFilterCb.DisplayMemberPath = "ExerciseName";

            List<Exercises> ExercisesList = new List<Exercises>();
            ExercisesList.Add(new Exercises() { ExerciseId = 0, ExerciseName = "Все упражнения" });
            foreach (Exercises ExerciseItem in App.context.Exercises.ToList())
            {
                ExercisesList.Add(ExerciseItem);
            }
            ExerciseFilterCb.ItemsSource = ExercisesList;
            ExerciseFilterCb.SelectedIndex = 0;

            List<string> RecordOptions = new List<string>();
            RecordOptions.Add("Все результаты");
            RecordOptions.Add("Только рекорды");
            RecordFilterCb.ItemsSource = RecordOptions;
            RecordFilterCb.SelectedIndex = 0;

            LoadData();
        }

        private void LoadData()
        {
            int SelectedAthleteId = 0;
            int SelectedExerciseId = 0;
            bool OnlyRecords = RecordFilterCb.SelectedIndex == 1;

            if (AthleteFilterCb.SelectedValue != null && (int)AthleteFilterCb.SelectedValue != 0)
            {
                SelectedAthleteId = (int)AthleteFilterCb.SelectedValue;
            }

            if (ExerciseFilterCb.SelectedValue != null && (int)ExerciseFilterCb.SelectedValue != 0)
            {
                SelectedExerciseId = (int)ExerciseFilterCb.SelectedValue;
            }

            List<Results> FilteredList = new List<Results>();

            foreach (Results ResultItem in App.context.Results.ToList())
            {
                bool MatchesAthlete = SelectedAthleteId == 0 ||
                                      ResultItem.AthleteId == SelectedAthleteId;

                bool MatchesExercise = SelectedExerciseId == 0 ||
                                       ResultItem.ExerciseId == SelectedExerciseId;

                bool MatchesRecord = !OnlyRecords || ResultItem.IsPersonalRecord == true;

                if (MatchesAthlete && MatchesExercise && MatchesRecord)
                {
                    FilteredList.Add(ResultItem);
                }
            }

            ResultsDg.ItemsSource = FilteredList;
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ResultsDg != null)
            {
                LoadData();
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ResultAddPage());
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ResultsDg.SelectedItem == null)
            {
                MessageBox.Show("Выберите результат для удаления");
            }
            else
            {
                Results SelectedResult = ResultsDg.SelectedItem as Results;

                MessageBoxResult Result = MessageBox.Show(
                    "Удалить результат?",
                    "Подтверждение",
                    MessageBoxButton.YesNo);

                if (Result == MessageBoxResult.Yes)
                {
                    App.context.Results.Remove(SelectedResult);
                    App.context.SaveChanges();
                    LoadData();
                }
            }
        }
    }
}
