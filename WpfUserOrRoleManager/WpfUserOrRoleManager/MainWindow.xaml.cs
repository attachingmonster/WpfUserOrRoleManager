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
        #region 成员定义
        AccountContext db = new AccountContext();//数据库上下文实例
        UnitOfWork unitOfWork = new UnitOfWork();//单元工厂实例
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            var user = unitOfWork.SysUserRepository.Get();
        }

        #region 公用界面事件

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
        #endregion
        #region 登陆界面
        private void Login_Click(object sender, RoutedEventArgs e)   //登陆事件
        {
            try
            {
                if (tbxUserPasswordLogin.Visibility != Visibility.Collapsed)//如果显示密码事件开始，则让显示密码值赋值给隐藏密码的值
                {
                    pbxUserPasswordLogin.Password = tbxUserPasswordLogin.Text;
                }
                var sysUser = unitOfWork.SysUserRepository.Get().Where(s => s.UserAccount.Equals(tbxUserAccountLogin.Text) && s.UserPassword.Equals(CreateMD5.EncryptWithMD5(pbxUserPasswordLogin.Password))).FirstOrDefault();//判断数据库中是否存在账号密码
                if (sysUser != null)
                {
                    MessageBox.Show("登陆成功！");
                }
                else
                {
                    throw new Exception("账号不存在或密码错误！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("登陆失败！错误信息：\n" + ex.Message);
            }
        }

        private void loginShowPassword_Check(object sender, RoutedEventArgs e)//登陆界面中显示密码事件
        {
            tbxUserPasswordLogin.Visibility = Visibility.Visible;
            pbxUserPasswordLogin.Visibility = Visibility.Collapsed;
            tbxUserPasswordLogin.Text = pbxUserPasswordLogin.Password;
        }

        private void loginHiddenPassword_Check(object sender, RoutedEventArgs e)//登陆界面中隐藏密码事件
        {
            pbxUserPasswordLogin.Visibility = Visibility.Visible;
            tbxUserPasswordLogin.Visibility = Visibility.Collapsed;
            pbxUserPasswordLogin.Password = tbxUserPasswordLogin.Text;
        }
        #endregion
        #region 注册账号界面

        /// <summary>
        /// 注册账号界面事件
        /// </summary>

        private void loginRegister_Click(object sender, RoutedEventArgs e) //切换界面事件
        {
            LoginWindow.Visibility = Visibility.Collapsed;
            RegisterWindow.Visibility = Visibility.Visible;
            Height = 441;
        }
        private void Registering_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tbxUserPasswordRegister.Visibility != Visibility.Collapsed)//如果显示密码事件开始，则让显示密码值赋值给隐藏密码的值
                {
                    pbxUserPasswordRegister.Password = tbxUserPasswordRegister.Text;
                    pbxSurePasswordRegister.Password = tbxSurePasswordRegister.Text;
                }
                if (tbxUserAccountRegister.Text != "")
                {
                    var u = unitOfWork.SysUserRepository.Get().Where(s => s.UserAccount.Equals(tbxUserAccountRegister.Text)).FirstOrDefault();  //查找是否存在账号
                    if (u == null)
                    {
                        foreach (char c in tbxUserAccountRegister.Text)   //规范账号必须由字母和数字构成
                        {
                            if (  !(  ('0' <= c && c <= '9') || ('A' <= c && c <= 'Z') || ('a' <= c && c <= 'z')  )  )
                            {
                                throw new Exception("账号必须只由字母和数字构成！");
                            }
                        }
                        if (pbxUserPasswordRegister.Password != "")
                        {
                            int number = 0, character = 0;
                            foreach (char c in pbxUserPasswordRegister.Password)   //规范密码必须由ASCII码33~126之间的字符构成
                            {
                                if (!(33 <= c && c <= 126))
                                {
                                    throw new Exception("符号错误，请重新输入！");
                                }
                                if ('0' <= c && c <= '9') //number记录数字个数
                                {
                                    number++;
                                }
                                else                      //character记录字符个数
                                {
                                    character++;
                                }
                            }
                            if (number < 5 || character < 2)  //密码的安全系数
                            {
                                throw new Exception("密码安全系数太低！");
                            }
                            if (pbxSurePasswordRegister.Password.Equals(pbxUserPasswordRegister.Password))//判断密码与确认密码是否相等
                            {
                                String UserAnswer = "1" + tbxUserAnswer1Register.Text + "2" + tbxUserAnswer2Register.Text + "3" + tbxUserAnswer3Register.Text + "4" + tbxUserAnswer4Register.Text + "5" + tbxUserAnswer5Register.Text;//拾回密码的答案连接
                                if (UserAnswer != "1"+"2"+"3"+"4"+"5")
                                {
                                    var CurrentUser = new SysUser();
                                    CurrentUser.UserAccount = tbxUserAccountRegister.Text;
                                    CurrentUser.UserPassword = CreateMD5.EncryptWithMD5(pbxUserPasswordRegister.Password);
                                    CurrentUser.UserAnswer = CreateMD5.EncryptWithMD5(UserAnswer);
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
            RegisterWindow.Visibility = Visibility.Collapsed;
            LoginWindow.Visibility = Visibility.Visible;
            Height = 311;
        }
        private void registerShowPassword_Check(object sender, RoutedEventArgs e) //注册界面中显示密码事件
        {
            tbxUserPasswordRegister.Visibility = Visibility.Visible;
            pbxUserPasswordRegister.Visibility = Visibility.Collapsed;
            tbxUserPasswordRegister.Text = pbxUserPasswordRegister.Password;

            tbxSurePasswordRegister.Visibility = Visibility.Visible;
            pbxSurePasswordRegister.Visibility = Visibility.Collapsed;
            tbxSurePasswordRegister.Text = pbxSurePasswordRegister.Password;
        }
        private void registerHiddenPassword_Check(object sender, RoutedEventArgs e) //注册界面中隐藏密码事件
        {
            tbxUserPasswordRegister.Visibility = Visibility.Collapsed;
            pbxUserPasswordRegister.Visibility = Visibility.Visible;
            pbxUserPasswordRegister.Password = tbxUserPasswordRegister.Text;

            tbxSurePasswordRegister.Visibility = Visibility.Collapsed;
            pbxSurePasswordRegister.Visibility = Visibility.Visible;
            pbxSurePasswordRegister.Password = tbxSurePasswordRegister.Text;
        }

        #endregion
        #region 密码拾回界面
        private void loginetrievePwd_Click(object sender, RoutedEventArgs e)  //切换界面事件
        {
            LoginWindow.Visibility = Visibility.Collapsed;
            RetrievePasswordWindow.Visibility = Visibility.Visible;
            Height = 411;
        }
        private void EtrievePwd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var u = unitOfWork.SysUserRepository.Get().Where(s => s.UserAccount.Equals(tbxUserAccountEtrievePwd.Text)).FirstOrDefault();  //查找是否存在账号
                if (u != null)
                {
                    String UserAnswer = "1" + tbxUserAnswer1EtrievePwd.Text + "2" + tbxUserAnswer2EtrievePwd.Text + "3" + tbxUserAnswer3EtrievePwd.Text + "4" + tbxUserAnswer4EtrievePwd.Text + "5" + tbxUserAnswer5EtrievePwd.Text;//拾回密码答案
                    if (UserAnswer != "1" + "2" + "3" + "4" + "5")
                    {
                        if (u.UserAnswer.Equals(CreateMD5.EncryptWithMD5(UserAnswer)))
                        {
                            RetrievePasswordWindow.Visibility = Visibility.Collapsed;
                            ResetPasswordWindow.Visibility = Visibility.Visible;
                            Height = 311;
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
            RetrievePasswordWindow.Visibility = Visibility.Collapsed;
            LoginWindow.Visibility = Visibility.Visible;
            Height = 311;
        }

        #endregion
        #region 密码重置界面
        private void resetPwd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (pbxUserPasswordResetPwd.Password != "")
                {
                    int number = 0, character = 0;
                    foreach (char c in pbxUserPasswordResetPwd.Password)   //规范密码必须由ASCII码33~126之间的字符构成
                    {
                        if (!(33 <= c && c <= 126))
                        {
                            throw new Exception("符号错误，请重新输入！");
                        }
                        if ('0' <= c && c <= '9') //number记录数字个数
                        {
                            number++;
                        }
                        else                      //character记录字符个数
                        {
                            character++;
                        }
                    }
                    if (number < 5 || character < 2)  //密码的安全系数
                    {
                        throw new Exception("新密码安全系数太低！");
                    }
                    if (pbxSurePasswordResetPwd.Password.Equals(pbxUserPasswordResetPwd.Password))//判断密码与确认密码是否相等
                    {
                        var u = unitOfWork.SysUserRepository.Get().Where(s => s.UserAccount.Equals(tbxUserAccountEtrievePwd.Text)).FirstOrDefault();  //获取与需要拾回密码的SysUser
                        u.UserPassword = CreateMD5.EncryptWithMD5(pbxUserPasswordResetPwd.Password);
                        unitOfWork.SysUserRepository.Update(u);//重置密码
                        unitOfWork.Save();
                        MessageBox.Show("密码重置成功!");
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
            catch (Exception ex)
            {
                MessageBox.Show("重置密码失败！错误信息：\n" + ex.Message);
            }
        }

        private void resetPwdBack_Click(object sender, RoutedEventArgs e)//返回登陆界面事件
        {
            ResetPasswordWindow.Visibility = Visibility.Collapsed;
            LoginWindow.Visibility = Visibility.Visible;
        }
        #endregion
        #region 修改密码界面

        private void loginChangePwd_Click(object sender, RoutedEventArgs e)   //切换界面事件
        {
            LoginWindow.Visibility = Visibility.Collapsed;
            ChangePasswordWindow.Visibility = Visibility.Visible;
        }
        private void ChangePsw_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string OldPassword = CreateMD5.EncryptWithMD5(pbxOldPasswordChangePwd.Password);     //原密码
                string NewPassword = CreateMD5.EncryptWithMD5(pbxUserPasswordChangePwd.Password);     //新密码

                var u = unitOfWork.SysUserRepository.Get().Where(s => s.UserAccount.Equals(tbxUserAccountChangePwd.Text.Trim()) && s.UserPassword.ToLower().Equals(OldPassword)).FirstOrDefault();//账号是否存在与密码是否相等
                if (u != null)
                {
                    if (pbxUserPasswordChangePwd.Password != "")
                    {
                        if (!pbxOldPasswordChangePwd.Password.Equals(pbxUserPasswordChangePwd.Password))
                        {
                            int number = 0, character = 0;
                            foreach (char c in pbxUserPasswordChangePwd.Password)   //规范密码必须由ASCII码33~126之间的字符构成
                            {
                                if (!(33 <= c && c <= 126))
                                {
                                    throw new Exception("符号错误，请重新输入！");
                                }
                                if ('0' <= c && c <= '9') //number记录数字个数
                                {
                                    number++;
                                }
                                else                      //character记录字符个数
                                {
                                    character++;
                                }
                            }
                            if (number < 5 || character < 2)  //密码的安全系数
                            {
                                throw new Exception("新密码安全系数太低！");
                            }
                            if (pbxSurePasswordChangePwd.Password.Equals(pbxUserPasswordChangePwd.Password)) //新密码与确认密码是否相等
                            {
                                u.UserPassword = NewPassword;
                                unitOfWork.SysUserRepository.Update(u);//更改密码
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
                            throw new Exception("新密码与原密码不能相同！");
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
        private void changePwdBack_click(object sender, RoutedEventArgs e)    //返回事件
        {
            LoginWindow.Visibility = Visibility.Visible;
            ChangePasswordWindow.Visibility = Visibility.Collapsed;
        }
        #endregion
    }
}

