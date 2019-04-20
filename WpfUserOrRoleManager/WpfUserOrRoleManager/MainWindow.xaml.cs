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
using WpfUserOrRoleManager.BLL;
using WpfUserOrRoleManager.DAL;

namespace WpfUserOrRoleManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var user = unitOfWork.SysUserRepository.Get();
        }

        AccountContext db = new AccountContext();
        UnitOfWork unitOfWork = new UnitOfWork();

        private void Min_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void Move_window(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var Log = new Login();
                Log.login(tbxUserAccount.Text, pbxPassword.Password);  //登录
                MessageBox.Show("登入成功");
            }
            catch(Exception ex)
            {
                MessageBox.Show("登入失败！错误信息：\n" + ex.Message);
            }
        }

        private void Sign_Click(object sender, RoutedEventArgs e) //注册
        {
            try
            {
                if(SurepbxPassword1.Password.Equals(pbxPassword1.Password))
                {
                    var Sign = new SignUp();
                    Sign.Sign(tbxUserAccount1.Text, pbxPassword1.Password);
                }
                else
                {
                    MessageBox.Show("密码与确认密码不一致！");
                }
                MessageBox.Show("注册成功！");
                Signup.Visibility = Visibility.Collapsed;
                LoginForm.Visibility = Visibility.Visible;
            }
            catch(Exception ex)
            {
                MessageBox.Show("注册失败！错误信息：\n" + ex.Message);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Signup.Visibility = Visibility.Collapsed;
            LoginForm.Visibility = Visibility.Visible;
        }

        private void ChangePsw_Click(object sender, RoutedEventArgs e)  //修改密码
        {
            try
            {
                if (SurepbxPassword2.Password.Equals(pbxPassword2.Password))
                {
                    var c = new ChangePassword();
                    c.Change(tbxUserAccount2.Text, OldPassword.Password, pbxPassword2.Password);
                }
                else
                {
                    MessageBox.Show("密码与确认密码不一致！");
                }
                MessageBox.Show("修改成功！");
                Change.Visibility = Visibility.Collapsed;
                LoginForm.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("修改失败！错误信息：\n" + ex.Message);
            }
        }

        private void Back1_Click(object sender, RoutedEventArgs e)
        {
            Change.Visibility = Visibility.Collapsed;
            LoginForm.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Signup.Visibility = Visibility.Visible;
            LoginForm.Visibility = Visibility.Collapsed;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Change.Visibility = Visibility.Visible;
            LoginForm.Visibility = Visibility.Collapsed;
        }
    }
}
