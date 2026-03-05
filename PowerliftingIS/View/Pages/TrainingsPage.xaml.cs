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
    public partial class TrainingsPage : Page
    {
        public TrainingsPage()
        {
            InitializeComponent();

            CoachFilterCb.SelectedValuePath = "CoachId";
            CoachFilterCb.DisplayMemberPath = "FullName";

            List<Coaches> CoachesList = new List<Coaches>();
            CoachesList.Add(new Coaches() { CoachId = 0, FullName = "Все тренеры" });
            foreach (Coaches CoachItem in App.context.Coaches.ToList())
            {
                CoachesList.Add(CoachItem);
            }
            CoachFilterCb.ItemsSource = CoachesList;
            CoachFilterCb.SelectedIndex = 0;

            LoadData();
        }

        private void LoadData()
        {
            string SearchText = SearchTb.Text.ToLower().Trim();
            int SelectedCoachId = 0;

            if (CoachFilterCb.SelectedValue != null && (int)CoachFilterCb.SelectedValue != 0)
            {
                SelectedCoachId = (int)CoachFilterCb.SelectedValue;
            }

            List<Trainings> FilteredList = new List<Trainings>();

            foreach (Trainings TrainingItem in App.context.Trainings.ToList())
            {
                bool MatchesSearch = string.IsNullOrEmpty(SearchText) ||
                                     TrainingItem.Description != null &&
                                     TrainingItem.Description.ToLower().Contains(SearchText);

                bool MatchesCoach = SelectedCoachId == 0 ||
                                    TrainingItem.CoachId == SelectedCoachId;

                if (MatchesSearch && MatchesCoach)
                {
                    FilteredList.Add(TrainingItem);
                }
            }

            TrainingsDg.ItemsSource = FilteredList;
            AttendanceDg.ItemsSource = null;
        }

        private void SearchTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TrainingsDg != null)
            {
                LoadData();
            }
        }

        private void CoachFilterCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TrainingsDg != null)
            {
                LoadData();
            }
        }

        private void TrainingsDg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TrainingsDg.SelectedItem == null)
            {
                AttendanceDg.ItemsSource = null;
            }
            else
            {
                Trainings SelectedTraining = TrainingsDg.SelectedItem as Trainings;

                List<TrainingAthletes> AttendanceList = new List<TrainingAthletes>();
                foreach (TrainingAthletes Item in App.context.TrainingAthletes.ToList())
                {
                    if (Item.TrainingId == SelectedTraining.TrainingId)
                    {
                        AttendanceList.Add(Item);
                    }
                }

                AttendanceDg.ItemsSource = AttendanceList;
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TrainingAddPage());
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TrainingsDg.SelectedItem == null)
            {
                MessageBox.Show("Выберите тренировку для удаления");
            }
            else
            {
                Trainings SelectedTraining = TrainingsDg.SelectedItem as Trainings;

                MessageBoxResult Result = MessageBox.Show(
                    "Удалить тренировку от " + SelectedTraining.TrainingDate.ToString("dd.MM.yyyy") + "?",
                    "Подтверждение",
                    MessageBoxButton.YesNo);

                if (Result == MessageBoxResult.Yes)
                {
                    List<TrainingAthletes> AttendanceList = new List<TrainingAthletes>();
                    foreach (TrainingAthletes Item in App.context.TrainingAthletes.ToList())
                    {
                        if (Item.TrainingId == SelectedTraining.TrainingId)
                        {
                            AttendanceList.Add(Item);
                        }
                    }

                    foreach (TrainingAthletes Item in AttendanceList)
                    {
                        App.context.TrainingAthletes.Remove(Item);
                    }

                    App.context.Trainings.Remove(SelectedTraining);
                    App.context.SaveChanges();
                    LoadData();
                }
            }
        }
    }
}
