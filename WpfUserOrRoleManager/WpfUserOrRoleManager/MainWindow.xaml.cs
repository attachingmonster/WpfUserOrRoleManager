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
using WpfUserOrRoleManager.Method;
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


        /// <summary>
        /// 公用界面事件
        /// </summary>

        private void Min_Click(object sender, RoutedEventArgs e)  //缩小窗口
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void Close_Click(object sender, RoutedEventArgs e)  //退出系统
        {
            SystemCommands.CloseWindow(this);
        }

        private void Move_window(object sender, MouseButtonEventArgs e)  //移动窗口
        {
            this.DragMove();
        }


        /// <summary>
        /// 登陆界面事件
        /// </summary>

        private void Login_Click(object sender, RoutedEventArgs e)   //登陆事件
        {
            try
            {
                var u = unitOfWork.SysUserRepository.Get().Where(s => s.UserAccount.Equals(tbxUserAccount_login.Text)&& s.UserPassword.Equals(CreateMD5.EncryptWithMD5(pbxPassword_login.Password))).FirstOrDefault();//判断数据库中是否存在账号密码
                if (u == null)
                {
                    throw new Exception("账号不存在或密码错误！");
                }
                else
                {   
                        MessageBox.Show("登陆成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("登陆失败！错误信息：\n" + ex.Message);
            }
        }



        /// <summary>
        /// 注册账号界面事件
        /// </summary>

        private void loginRegister_Click(object sender, RoutedEventArgs e) //切换界面事件
        {
            LoginWindow.Visibility = Visibility.Collapsed;
            RegisterWindow.Visibility = Visibility.Visible;          
        }
        private void Registering_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tbxUserAccountRegister.Text != "")
                {
                    var u = unitOfWork.SysUserRepository.Get().Where(s => s.UserAccount.Equals(tbxUserAccountRegister.Text)).FirstOrDefault();  //查找是否存在账号
                    if (u == null)
                    {
                        if (pbxUserPasswordRegister.Password != "")
                        {
                            if (pbxSurePassword_register.Password.Equals(pbxUserPasswordRegister.Password))//判断密码与确认密码是否相等
                            {
                                if (tbxUserAnswerRegister.Text != "")
                                {
                                    var CurrentUser = new SysUser();
                                    CurrentUser.UserAccount = tbxUserAccountRegister.Text;
                                    CurrentUser.UserPassword = CreateMD5.EncryptWithMD5(pbxUserPasswordRegister.Password);
                                    CurrentUser.UserAnswer = CreateMD5.EncryptWithMD5(tbxUserAnswerRegister.Text);
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
        private void registerBack_Click(object sender, RoutedEventArgs e)   //返回事件
        {
            LoginWindow.Visibility = Visibility.Visible;
            RegisterWindow.Visibility = Visibility.Collapsed;
        }


        /// <summary>
        /// 密码拾回界面事件
        /// </summary>

        private void loginetrievePwd_Click(object sender, RoutedEventArgs e)  //切换界面事件
        {
            LoginWindow.Visibility = Visibility.Collapsed;
            RetrievePasswordWindow.Visibility = Visibility.Visible;
        }
        private void EtrievePwd_Click(object sender, RoutedEventArgs e)
        {
            //1、把 Python 命名格式改为现在的  驼峰格式  2、拾回密码采用问答方式，注册时问题与答案应该合并加密后放入一个字段里，因为这个是隐私，需要加密为密文。3、密码拾回后如何重置密码考虑下业务逻辑
            try
            {          
                var u = unitOfWork.SysUserRepository.Get().Where(s => s.UserAccount.Equals(tbxUserAccount_etrievePwd.Text)).FirstOrDefault();  //查找是否存在账号
                if (u != null)
                {
                    if (tbxUserAnswer_etrievePwd.Text != "")
                    {
                        if (u.UserAnswer.Equals(CreateMD5.EncryptWithMD5(tbxUserAnswer_etrievePwd.Text)))
                        {

                            if (pbxUserPassword_etrievePwd.Password != "")
                            {
                                if (pbxSurePassword_etrievePwd.Password.Equals(pbxUserPassword_etrievePwd.Password))//判断密码与确认密码是否相等
                                {
                                    u.UserPassword = CreateMD5.EncryptWithMD5(pbxUserPassword_etrievePwd.Password);
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
        private void etrievePwdBack_Click(object sender, RoutedEventArgs e) //返回事件
        {
            LoginWindow.Visibility = Visibility.Visible;
            RetrievePasswordWindow.Visibility = Visibility.Collapsed;
        }


        /// <summary>
        /// 修改密码界面事件
        /// </summary>

        private void loginChangePwd_Click(object sender, RoutedEventArgs e)   //切换界面事件
        {
            LoginWindow.Visibility = Visibility.Collapsed;
            ChangePwdWindow.Visibility = Visibility.Visible;
        }
        private void ChangePsw_Click(object sender, RoutedEventArgs e)
        {
            try
            {                                   
                string OldPassword = CreateMD5.EncryptWithMD5(pbxOldPassword_changePwd.Password);     //原密码
                string NewPassword = CreateMD5.EncryptWithMD5(pbxPassword_changePwd.Password);     //新密码

                var u = unitOfWork.SysUserRepository.Get().Where(s => s.UserAccount.Equals(tbxUserAccount_changePwd.Text.Trim()) && s.UserPassword.ToLower().Equals(OldPassword)).FirstOrDefault();//账号是否存在与密码是否相等
                if (u != null)
                {
                    if (pbxPassword_changePwd.Password != "")
                    {
                        if (pbxSurePassword_changePwd.Password.Equals(pbxPassword_changePwd.Password)) //新密码与确认密码是否相等
                        {
                            u.UserPassword = NewPassword;
                            unitOfWork.SysUserRepository.Update(u);
                            unitOfWork.Save();
                            MessageBox.Show("修改成功!");
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
                    throw new Exception("用户名不存在或原密码输入错误！");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("修改密码失败！错误信息：\n" + ex.Message);
            }
        }
        private void changePwdBack_Click(object sender, RoutedEventArgs e)    //返回事件
        {
            LoginWindow.Visibility = Visibility.Visible;
            ChangePwdWindow.Visibility = Visibility.Collapsed;
        }
    }
}
