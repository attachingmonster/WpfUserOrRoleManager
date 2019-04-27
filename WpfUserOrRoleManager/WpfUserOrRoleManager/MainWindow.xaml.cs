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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (tbxUserName.Text == "" || pbxPassword.Password == "")
            {
                MessageBox.Show("用户名或者密码不能为空！");
            }
            else
            {
                var users = unitOfWork.SysUserRepository.Get();
                var user = users.Where(s => s.Account == this.tbxUserName.Text && s.Password == this.pbxPassword.Password).FirstOrDefault();
                if (user == null)
                {
                    MessageBox.Show("用户或密码出错！");
                }
                else
                {
                    MessageBox.Show("登入成功！");
                }

            }
            

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Min_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void Change_password(object sender, RoutedEventArgs e)
        {
            login.Visibility = Visibility.Collapsed;
            changepassword.Visibility = Visibility.Visible;
            loginsystem.Height = 447.739;
            loginsystem.Width = 424.623;
        }

        private void changepwdMin_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void changepwdClose_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void changepwdreturn(object sender, RoutedEventArgs e)
        {
            changepassword.Visibility = Visibility.Collapsed;
            login.Visibility = Visibility.Visible;         
            loginsystem.Height = 514.089;
            loginsystem.Width = 848.5;
        }

        private void Confirmchange(object sender, RoutedEventArgs e)
        {
            string UseAccount = tbxUserName2.Text;//需要修改的账号的控件名为ggg
            string UserPassword = oldpbxPassword.Password;//输入的原密码的控件名为www
            string Newpassword = newpbxPassword.Password;//输入的新密码的控件名为hhh
            var user = unitOfWork.SysUserRepository.Get()
                .Where(s => s.Account.Equals(UseAccount.Trim()) && s.Password.ToLower().Equals(UserPassword)).FirstOrDefault();
            if (user != null)
            {
                if (newpbxPassword.Password== confirmpbxPassword.Password)//新密码的控件名为hhh，确认密码的控件名为kkk
                {
                    user.Account = UseAccount;
                    user.Password = Newpassword;
                    unitOfWork.SysUserRepository.Update(user);
                    unitOfWork.Save();
                    MessageBox.Show("修改成功，请登录");
                }
                else
                {
                    MessageBox.Show("您的两次新密码填写不一致！");
                }

            }
            else
            {
                MessageBox.Show("修改失败");
            }
        }
    }
}
