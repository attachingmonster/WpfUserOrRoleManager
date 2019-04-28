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
using WpfUserOrRoleManager.Model;
using WpfUserOrRoleManager.Models;

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

        private void Login_Click(object sender, RoutedEventArgs e)//登陆事件
        {
            if (tbxUserName.Text == "" || pbxPassword.Password == "")
            {
                MessageBox.Show("用户名或者密码不能为空！");
            }
            else
            {
                var users = unitOfWork.SysUserRepository.Get();
                var user = users.Where(s => s.Account.Equals(tbxUserName.Text) && s.Password.Equals(CreateMD5.EncryptWithMD5(pbxPassword.Password))).FirstOrDefault();//判断数据库中是否存在账号密码
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

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)//点击鼠标实现窗口拖动
        {
            this.DragMove();
        }

        private void Min_Click(object sender, RoutedEventArgs e)//缩小登录窗口
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void Close_Click(object sender, RoutedEventArgs e)//关闭登录窗口
        {
            SystemCommands.CloseWindow(this);
        }

        private void Change_password(object sender, RoutedEventArgs e)//在登录界面点击修改密码按钮时发生的事件
        {
            login.Visibility = Visibility.Collapsed;
            changepassword.Visibility = Visibility.Visible;
            loginsystem.Height = 447.739;
            loginsystem.Width = 424.623;
        }

        private void changepwdMin_Click(object sender, RoutedEventArgs e)//缩小修改密码界面的窗口
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void changepwdClose_Click(object sender, RoutedEventArgs e)//关闭修改密码界面的窗口
        {
            SystemCommands.CloseWindow(this);
        }

        private void changepwdreturn(object sender, RoutedEventArgs e)//修改密码界面的返回事件
        {
            changepassword.Visibility = Visibility.Collapsed;
            login.Visibility = Visibility.Visible;         
            loginsystem.Height = 514.089;
            loginsystem.Width = 848.5;
        }

        private void Confirmchange(object sender, RoutedEventArgs e)//实现修改密码功能
        {
            string OldPassword = Model.CreateMD5.EncryptWithMD5(oldpbxPassword.Password);     //原密码
            string NewPassword = Model.CreateMD5.EncryptWithMD5(newpbxPassword.Password);     //新密码
            var user = unitOfWork.SysUserRepository.Get()
                .Where(s => s.Account.Equals(tbxUserName2.Text.Trim()) && s.Password.ToLower().Equals(OldPassword)).FirstOrDefault();//判断账号是否存在与密码是否相等
            if (user != null)
            {
                if(newpbxPassword.Password!="")
                {
                    if (newpbxPassword.Password == oldpbxPassword.Password)
                    {
                        MessageBox.Show("您的原密码和新密码输入一致，请重新修改密码！");//重点，不可忽略
                    }
                    else if (newpbxPassword.Password == confirmpbxPassword.Password)//判断新密码与确认密码是否相等
                    {
                        user.Password = NewPassword;
                        unitOfWork.SysUserRepository.Update(user);
                        unitOfWork.Save();
                        MessageBox.Show("修改成功，请登录");
                        changepassword.Visibility = Visibility.Collapsed;
                        login.Visibility = Visibility.Visible;
                        loginsystem.Height = 514.089;
                        loginsystem.Width = 848.5;
                    }
                    else
                    {
                        MessageBox.Show("您的新密码与确认密码填写不一致！");
                    }

                }
                else
                {
                    MessageBox.Show("新密码不能为空！");
                }
            }
            else
            {
                MessageBox.Show("修改密码失败！错误原因：用户名不存在或原密码输入错误！");
            }
        }

        private void loginRegister_Click(object sender, RoutedEventArgs e)//点击登录界面的注册账号按钮时发生的事件
        {
            login.Visibility = Visibility.Collapsed;
            RegisterWindow.Visibility = Visibility.Visible;
            loginsystem.Height = 447.739;
            loginsystem.Width = 424.623;
        }

        private void RegisterMin_Click(object sender, RoutedEventArgs e)//缩小注册账号界面
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void RegisterClose_Click(object sender, RoutedEventArgs e)//关闭注册账号界面
        {
            SystemCommands.CloseWindow(this);
        }

        private void RegisterReturn_Click(object sender, RoutedEventArgs e)//注册界面的返回事件
        {
            RegisterWindow.Visibility = Visibility.Collapsed;
            login.Visibility = Visibility.Visible;
            loginsystem.Height = 514.089;
            loginsystem.Width = 848.5;
        }

        private void Registering_Click(object sender, RoutedEventArgs e)//实现注册账号功能
        {
            try
            {
                if (Account_register.Text != "")
                {
                    var u = unitOfWork.SysUserRepository.Get().Where(s => s.Account.Equals(Account_register.Text)).FirstOrDefault();  //查找是否存在账号
                    if (u == null)
                    {
                        if (Password_register.Password != "")
                        {
                            if (SurePassword_register.Password.Equals(Password_register.Password))//判断密码与确认密码是否相等
                            {
                                if (QuestionAnswer_register.Text != "")
                                {
                                    var CurrentUser = new SysUser();
                                    CurrentUser.Account = Account_register.Text;
                                    CurrentUser.Password = CreateMD5.EncryptWithMD5(Password_register.Password);
                                    CurrentUser.QuestionAnswer = CreateMD5.EncryptWithMD5(QuestionAnswer_register.Text);
                                    unitOfWork.SysUserRepository.Insert(CurrentUser);    //增加新User
                                    unitOfWork.Save();
                                    MessageBox.Show("注册成功");
                                }
                                else
                                {
                                    throw new Exception("密码拾回问题答案不能为空！");
                                }
                            }
                            else
                            {
                                throw new Exception("两次输入的密码不一致！");
                            }
                        }
                        else
                        {
                            throw new Exception("密码不能为空！");
                        }
                    }
                    else
                    {
                        throw new Exception("用户名已存在！");
                    }
                }
                else
                {
                    throw new Exception("账号不能为空！");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("注册失败！错误信息：\n" + ex.Message);
            }
        }

        private void loginetrievePwd_Click(object sender, RoutedEventArgs e)//登录界面上点击找回密码按钮发生的事件
        {
            login.Visibility = Visibility.Collapsed;
            RetrievePasswordWindow.Visibility = Visibility.Visible;
            loginsystem.Height = 447.739;
            loginsystem.Width = 424.623;
        }

        private void loginetrieveMin_Click(object sender, RoutedEventArgs e)//缩小找回密码界面
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void loginetrieveClose_Click(object sender, RoutedEventArgs e)//关闭找回密码界面
        {
            SystemCommands.CloseWindow(this);
        }

        private void loginetrieveBack_Click(object sender, RoutedEventArgs e)//找回密码界面的返回事件
        {
            RetrievePasswordWindow.Visibility = Visibility.Collapsed;
            login.Visibility = Visibility.Visible;
            loginsystem.Height = 514.089;
            loginsystem.Width = 848.5;
        }

        private void EtrievePwd_Click(object sender, RoutedEventArgs e)//实现找回密码功能
        {
            try
            {
                var u = unitOfWork.SysUserRepository.Get().Where(s => s.Account.Equals(Account_etrievePwd.Text)).FirstOrDefault();  //查找是否存在账号
                if (u != null)
                {
                    if (QuestionAnswer_etrievePwd.Text != "")
                    {
                        if (u.QuestionAnswer.Equals(CreateMD5.EncryptWithMD5(QuestionAnswer_etrievePwd.Text)))
                        {

                            if (NewPassword_etrievePwd.Password != "")
                            {
                                if (SurePassword_etrievePwd.Password.Equals(NewPassword_etrievePwd.Password))//判断密码与确认密码是否相等
                                {
                                    u.Password = CreateMD5.EncryptWithMD5(NewPassword_etrievePwd.Password);
                                    unitOfWork.SysUserRepository.Update(u);
                                    unitOfWork.Save();
                                    MessageBox.Show("密码拾回成功!");
                                }
                                else
                                {
                                    throw new Exception("两次输入的密码不一致！");
                                }
                            }
                            else
                            {
                                throw new Exception("新密码不能为空！");
                            }
                        }
                        else
                        {
                            throw new Exception("密码拾回问题答案错误");
                        }
                    }
                    else
                    {
                        throw new Exception("请您输入密码拾回问题答案");
                    }
                }
                else
                {
                    throw new Exception("账号不存在！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("找回密码失败！错误信息：\n" + ex.Message);
            }
        }
    }
 }

