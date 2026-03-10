using PowerliftingIS;
using PowerliftingIS.AppData;
using PowerliftingIS.Model;
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

namespace PowerliftingIS.View.Pages
{
    public partial class AthletesPage : Page
{
    public AthletesPage()
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

        LoadData();
    }

    private void LoadData()
    {
        string SearchText = SearchTb.Text.ToLower().Trim();
        int SelectedRankId = 0;

        if (RankFilterCb.SelectedValue != null && (int)RankFilterCb.SelectedValue != 0)
        {
            SelectedRankId = (int)RankFilterCb.SelectedValue;
        }

        List<Athletes> FilteredList = new List<Athletes>();

        foreach (Athletes AthleteItem in App.context.Athletes.ToList())
        {
            bool MatchesRole = SessionManager.IsAdmin ||
                               AthleteItem.CoachId == SessionManager.CurrentCoach.CoachId;

            bool MatchesSearch = string.IsNullOrEmpty(SearchText) ||
                                 AthleteItem.FullName.ToLower().Contains(SearchText);

            bool MatchesRank = SelectedRankId == 0 ||
                               AthleteItem.RankId == SelectedRankId;

            if (MatchesRole && MatchesSearch && MatchesRank)
            {
                FilteredList.Add(AthleteItem);
            }
        }

        AthletesDg.ItemsSource = FilteredList;
    }

    private void SearchTb_TextChanged(object sender, TextChangedEventArgs e)
    {
        LoadData();
    }

    private void RankFilterCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (AthletesDg != null)
        {
            LoadData();
        }
    }

    private void AddBtn_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new AthleteAddPage());
    }

    private void DeleteBtn_Click(object sender, RoutedEventArgs e)
    {
        if (AthletesDg.SelectedItem == null)
        {
            MessageBox.Show("Выберите спортсмена для удаления");
        }
        else
        {
            Athletes SelectedAthlete = AthletesDg.SelectedItem as Athletes;

            if (!SessionManager.IsAdmin && SelectedAthlete.CoachId != SessionManager.CurrentCoach.CoachId)
            {
                MessageBox.Show("Вы можете удалять только своих спортсменов");
            }
            else
            {
                MessageBoxResult Result = MessageBox.Show(
                    "Удалить спортсмена " + SelectedAthlete.FullName + "?",
                    "Подтверждение",
                    MessageBoxButton.YesNo);

                if (Result == MessageBoxResult.Yes)
                {
                    App.context.Athletes.Remove(SelectedAthlete);
                    App.context.SaveChanges();
                    LoadData();
                }
            }
        }
    }
}
}