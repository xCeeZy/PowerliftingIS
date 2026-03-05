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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PowerliftingIS.View.Windows
{
    public partial class LoginWindow : Window
    {
        private int FailedAttempts = 0;
        private string CurrentCaptcha = "";
        private bool IsLocked = false;
        private DispatcherTimer LockTimer = new DispatcherTimer();
        private int LockSecondsLeft = 0;
        private Random Rnd = new Random();

        public LoginWindow()
        {
            InitializeComponent();

            LockTimer.Interval = TimeSpan.FromSeconds(1);
            LockTimer.Tick += LockTimer_Tick;
        }

        #region CapsLock и TextChanged

        private void LoginTb_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            HideError();
        }

        private void PasswordPb_PasswordChanged(object sender, RoutedEventArgs e)
        {
            HideError();
            CheckCapsLock();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            CheckCapsLock();
        }

        private void CheckCapsLock()
        {
            if (Console.CapsLock)
            {
                CapsLockTb.Visibility = Visibility.Visible;
            }
            else
            {
                CapsLockTb.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Капча

        private string GenerateCaptcha()
        {
            string Chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            string Result = "";
            for (int I = 0; I < 5; I++)
            {
                Result += Chars[Rnd.Next(Chars.Length)];
            }
            return Result;
        }

        private void ShowCaptcha()
        {
            CurrentCaptcha = GenerateCaptcha();
            CaptchaCodeTb.Text = CurrentCaptcha;
            CaptchaInputTb.Text = "";
            CaptchaPanel.Visibility = Visibility.Visible;
        }

        private void RefreshCaptchaBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrentCaptcha = GenerateCaptcha();
            CaptchaCodeTb.Text = CurrentCaptcha;
            CaptchaInputTb.Text = "";
        }

        #endregion

        #region Блокировка по таймеру

        private void StartLock(int Seconds)
        {
            IsLocked = true;
            LockSecondsLeft = Seconds;
            LoginBtn.IsEnabled = false;
            LockPanel.Visibility = Visibility.Visible;
            LockTimerTb.Text = "Повторите попытку через " + LockSecondsLeft + " сек.";
            LockTimer.Start();
        }

        private void LockTimer_Tick(object sender, EventArgs e)
        {
            LockSecondsLeft--;
            LockTimerTb.Text = "Повторите попытку через " + LockSecondsLeft + " сек.";

            if (LockSecondsLeft <= 0)
            {
                LockTimer.Stop();
                IsLocked = false;
                LoginBtn.IsEnabled = true;
                LockPanel.Visibility = Visibility.Collapsed;
                FailedAttempts = 0;
                AttemptsHintTb.Visibility = Visibility.Collapsed;
                ShowCaptcha();
            }
        }

        #endregion

        #region Вход

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IsLocked)
            {
                return;
            }

            string Login = LoginTb.Text.Trim();
            string Password = PasswordPb.Password;

            if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Password))
            {
                ShowError("Заполните логин и пароль");
                return;
            }

            if (FailedAttempts >= 3 && CaptchaPanel.Visibility == Visibility.Visible)
            {
                if (CaptchaInputTb.Text.Trim().ToUpper() != CurrentCaptcha)
                {
                    ShowError("Неверный код с картинки");
                    CurrentCaptcha = GenerateCaptcha();
                    CaptchaCodeTb.Text = CurrentCaptcha;
                    CaptchaInputTb.Text = "";
                    return;
                }
            }

            Coaches FoundCoach = null;

            foreach (Coaches CoachItem in App.context.Coaches.ToList())
            {
                if (CoachItem.Login == Login && CoachItem.Password == Password)
                {
                    FoundCoach = CoachItem;
                    break;
                }
            }

            if (FoundCoach == null)
            {
                FailedAttempts++;

                if (FailedAttempts == 1)
                {
                    ShowError("Неверный логин или пароль");
                    AttemptsHintTb.Text = "Попытка 1 из 3";
                    AttemptsHintTb.Visibility = Visibility.Visible;
                }
                else if (FailedAttempts == 2)
                {
                    ShowError("Неверный логин или пароль");
                    AttemptsHintTb.Text = "Попытка 2 из 3";
                }
                else if (FailedAttempts == 3)
                {
                    ShowError("Неверный логин или пароль. Введите капчу.");
                    AttemptsHintTb.Text = "Попытка 3 из 3 — требуется капча";
                    ShowCaptcha();
                }
                else
                {
                    ShowError("Неверный логин или пароль");
                    StartLock(30);
                }
            }
            else
            {
                SessionManager.CurrentCoach = FoundCoach;
                SessionManager.IsAdmin = FoundCoach.IsAdmin;

                MainWindow Main = new MainWindow();
                Main.Show();
                this.Close();
            }
        }

        #endregion

        #region Вспомогательные

        private void ShowError(string Message)
        {
            ErrorTb.Text = Message;
            ErrorPanel.Visibility = Visibility.Visible;
        }

        private void HideError()
        {
            ErrorPanel.Visibility = Visibility.Collapsed;
        }

        #endregion
    }
}
