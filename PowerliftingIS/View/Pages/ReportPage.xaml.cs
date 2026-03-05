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
    public partial class ReportPage : Page
    {
        public ReportPage()
        {
            InitializeComponent();

            RankFilterCb.SelectedValuePath = "RankId";
            RankFilterCb.DisplayMemberPath = "RankName";

            List<Ranks> RanksList = new List<Ranks>();
            RanksList.Add(new Ranks() { RankId = 0, RankName = "Все разряды" });
            foreach (Ranks RankItem in App.context.Ranks.ToList())
            {
                RanksList.Add(RankItem);
            }
            RankFilterCb.ItemsSource = RanksList;
            RankFilterCb.SelectedIndex = 0;

            DateFromDp.SelectedDate = new DateTime(2024, 1, 1);
            DateToDp.SelectedDate = DateTime.Today;

            BuildReport();
        }

        private void BuildReport()
        {
            int SelectedRankId = 0;
            if (RankFilterCb.SelectedValue != null && (int)RankFilterCb.SelectedValue != 0)
            {
                SelectedRankId = (int)RankFilterCb.SelectedValue;
            }

            DateTime DateFrom = DateFromDp.SelectedDate ?? new DateTime(DateTime.Today.Year, 1, 1);
            DateTime DateTo = DateToDp.SelectedDate ?? DateTime.Today;

            List<Athletes> AthletesList = new List<Athletes>();
            foreach (Athletes AthleteItem in App.context.Athletes.ToList())
            {
                if (SelectedRankId == 0 || AthleteItem.RankId == SelectedRankId)
                {
                    AthletesList.Add(AthleteItem);
                }
            }

            List<Results> AllResults = new List<Results>();
            foreach (Results ResultItem in App.context.Results.ToList())
            {
                if (ResultItem.ResultDate >= DateFrom && ResultItem.ResultDate <= DateTo)
                {
                    AllResults.Add(ResultItem);
                }
            }

            List<TrainingAthletes> AllAttendance = App.context.TrainingAthletes.ToList();

            int SquatId = 0;
            int BenchId = 0;
            int DeadliftId = 0;

            foreach (Exercises ExItem in App.context.Exercises.ToList())
            {
                if (ExItem.ExerciseName.Contains("Присед"))
                {
                    SquatId = ExItem.ExerciseId;
                }
                else if (ExItem.ExerciseName.Contains("Жим"))
                {
                    BenchId = ExItem.ExerciseId;
                }
                else if (ExItem.ExerciseName.Contains("тяга") || ExItem.ExerciseName.Contains("Тяга"))
                {
                    DeadliftId = ExItem.ExerciseId;
                }
            }

            var ReportData = AthletesList
                .Select(a => new
                {
                    AthleteName = a.FullName,
                    RankName = a.Ranks.RankName,
                    TrainingCount = AllAttendance.Count(ta => ta.AthleteId == a.AthleteId),
                    BestSquat = AllResults
                                    .Where(r => r.AthleteId == a.AthleteId && r.ExerciseId == SquatId)
                                    .Select(r => (decimal?)r.ResultWeight)
                                    .Max() ?? 0,
                    BestBench = AllResults
                                    .Where(r => r.AthleteId == a.AthleteId && r.ExerciseId == BenchId)
                                    .Select(r => (decimal?)r.ResultWeight)
                                    .Max() ?? 0,
                    BestDeadlift = AllResults
                                    .Where(r => r.AthleteId == a.AthleteId && r.ExerciseId == DeadliftId)
                                    .Select(r => (decimal?)r.ResultWeight)
                                    .Max() ?? 0,
                    RecordsCount = AllResults
                                    .Count(r => r.AthleteId == a.AthleteId && r.IsPersonalRecord == true)
                })
                .OrderByDescending(x => x.BestSquat + x.BestBench + x.BestDeadlift)
                .ToList();

            ReportDg.ItemsSource = ReportData;

            TotalAthletesTb.Text = AthletesList.Count.ToString();

            int TotalTrainings = 0;
            foreach (Athletes AthleteItem in AthletesList)
            {
                foreach (TrainingAthletes TaItem in AllAttendance)
                {
                    if (TaItem.AthleteId == AthleteItem.AthleteId)
                    {
                        TotalTrainings++;
                    }
                }
            }
            TotalTrainingsTb.Text = TotalTrainings.ToString();

            int TotalRecords = 0;
            foreach (Results ResultItem in AllResults)
            {
                bool AthleteInList = false;
                foreach (Athletes AthleteItem in AthletesList)
                {
                    if (AthleteItem.AthleteId == ResultItem.AthleteId)
                    {
                        AthleteInList = true;
                        break;
                    }
                }
                if (AthleteInList && ResultItem.IsPersonalRecord == true)
                {
                    TotalRecords++;
                }
            }
            TotalRecordsTb.Text = TotalRecords.ToString();

            decimal BestWeight = 0;
            foreach (Results ResultItem in AllResults)
            {
                if (ResultItem.ResultWeight > BestWeight)
                {
                    BestWeight = ResultItem.ResultWeight;
                }
            }
            BestResultTb.Text = BestWeight > 0 ? BestWeight.ToString() : "—";
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReportDg != null)
            {
                BuildReport();
            }
        }

        private void BuildBtn_Click(object sender, RoutedEventArgs e)
        {
            BuildReport();
        }
    }
}

