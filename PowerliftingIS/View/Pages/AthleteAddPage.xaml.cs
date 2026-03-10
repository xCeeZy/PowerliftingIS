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
    public partial class AthleteAddPage : Page
    {
        public AthleteAddPage()
        {
            InitializeComponent();

            RankCb.SelectedValuePath = "RankId";
            RankCb.DisplayMemberPath = "RankName";
            RankCb.ItemsSource = App.context.Ranks.ToList();

            WeightCategoryCb.SelectedValuePath = "WeightCategoryId";
            WeightCategoryCb.DisplayMemberPath = "CategoryName";
            WeightCategoryCb.ItemsSource = App.context.WeightCategories.ToList();

            CoachCb.SelectedValuePath = "CoachId";
            CoachCb.DisplayMemberPath = "FullName";

            if (SessionManager.IsAdmin)
            {
                CoachCb.ItemsSource = App.context.Coaches.ToList();
                CoachCb.IsEnabled = true;
            }
            else
            {
                CoachCb.ItemsSource = new System.Collections.Generic.List<Coaches>
                {
                    SessionManager.CurrentCoach
                };
                CoachCb.SelectedIndex = 0;
                CoachCb.IsEnabled = false;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FullNameTb.Text) ||
                BirthDateDp.SelectedDate == null ||
                RankCb.SelectedItem == null ||
                WeightCategoryCb.SelectedItem == null ||
                CoachCb.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля");
            }
            else
            {
                Athletes NewAthlete = new Athletes()
                {
                    FullName = FullNameTb.Text,
                    BirthDate = BirthDateDp.SelectedDate.Value,
                    RankId = (int)RankCb.SelectedValue,
                    WeightCategoryId = (int)WeightCategoryCb.SelectedValue,
                    CoachId = (int)CoachCb.SelectedValue
                };

                App.context.Athletes.Add(NewAthlete);
                App.context.SaveChanges();

                MessageBox.Show("Спортсмен добавлен");
                NavigationService.Navigate(new AthletesPage());
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AthletesPage());
        }
    }
}