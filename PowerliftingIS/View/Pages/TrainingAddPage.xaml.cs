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
    public partial class TrainingAddPage : Page
    {
        public TrainingAddPage()
        {
            InitializeComponent();

            CoachCb.SelectedValuePath = "CoachId";
            CoachCb.DisplayMemberPath = "FullName";
            CoachCb.ItemsSource = App.context.Coaches.ToList();

            AthletesLb.DisplayMemberPath = "FullName";
            AllAthletesLb.DisplayMemberPath = "FullName";
            AllAthletesLb.ItemsSource = App.context.Athletes.ToList();
        }

        private void CoachCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CoachCb.SelectedValue != null)
            {
                int CoachId = (int)CoachCb.SelectedValue;

                List<Athletes> CoachAthletes = new List<Athletes>();
                foreach (Athletes AthleteItem in App.context.Athletes.ToList())
                {
                    if (AthleteItem.CoachId == CoachId)
                    {
                        CoachAthletes.Add(AthleteItem);
                    }
                }

                AthletesLb.ItemsSource = CoachAthletes;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TrainingDateDp.SelectedDate == null || CoachCb.SelectedItem == null)
            {
                MessageBox.Show("Заполните дату и тренера");
            }
            else
            {
                Trainings NewTraining = new Trainings()
                {
                    TrainingDate = TrainingDateDp.SelectedDate.Value,
                    CoachId = (int)CoachCb.SelectedValue,
                    Description = DescriptionTb.Text
                };

                App.context.Trainings.Add(NewTraining);
                App.context.SaveChanges();

                foreach (Athletes AthleteItem in AthletesLb.SelectedItems)
                {
                    TrainingAthletes NewRecord = new TrainingAthletes()
                    {
                        TrainingId = NewTraining.TrainingId,
                        AthleteId = AthleteItem.AthleteId
                    };
                    App.context.TrainingAthletes.Add(NewRecord);
                }

                foreach (Athletes AthleteItem in AllAthletesLb.SelectedItems)
                {
                    bool AlreadyAdded = false;
                    foreach (Athletes SelectedItem in AthletesLb.SelectedItems)
                    {
                        if (SelectedItem.AthleteId == AthleteItem.AthleteId)
                        {
                            AlreadyAdded = true;
                            break;
                        }
                    }

                    if (!AlreadyAdded)
                    {
                        TrainingAthletes NewRecord = new TrainingAthletes()
                        {
                            TrainingId = NewTraining.TrainingId,
                            AthleteId = AthleteItem.AthleteId
                        };
                        App.context.TrainingAthletes.Add(NewRecord);
                    }
                }

                App.context.SaveChanges();
                MessageBox.Show("Тренировка добавлена");
                NavigationService.Navigate(new TrainingsPage());
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TrainingsPage());
        }
    }
}
