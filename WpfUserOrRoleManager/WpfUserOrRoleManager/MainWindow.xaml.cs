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
        #region 成员定义
        public MainWindow()
        {
            InitializeComponent();
            var user = unitOfWork.SysUserRepository.Get();
        }
        AccountContext db = new AccountContext();
        UnitOfWork unitOfWork = new UnitOfWork();
        #endregion

        #region 公用界面事件
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

        #endregion

        #region 登陆界面
        private void Login_Click(object sender, RoutedEventArgs e)//登陆事件
        {
            try
            {
                //判断数据库中的(是否存在账号and(输入密码正确or(已记住密码and给定密码正确))逻辑是否存在 ，如果存在则返回对象，否则返回null
                var sysUser = unitOfWork.SysUserRepository.Get().Where(s => s.Account.Equals(tbxUserName.Text) && (s.Password.Equals(CreateMD5.EncryptWithMD5(pbxPassword.Password)) || (s.LoginOrnot.Equals("1") && s.Password.Equals(pbxPassword.Password)))).FirstOrDefault();
                if (sysUser != null)
                {
                    //判断登录成功时是否记住密码
                    if (rememberpwd.IsChecked == true)
                    {
                        sysUser.LoginOrnot = "1";
                        unitOfWork.SysUserRepository.Update(sysUser);
                        unitOfWork.Save();
                    }
                    else
                    {
                        sysUser.LoginOrnot = "0";
                        unitOfWork.SysUserRepository.Update(sysUser);
                        unitOfWork.Save();
                    }

                    MessageBox.Show("登录成功！");
                    //linq 多表查询得到选择的用户的角色，暂时一个账号只有一个角色
                    var UserRole = (from u in unitOfWork.SysUserRepository.Get()
                                    join ur in unitOfWork.SysUserRoleRepository.Get() on u.ID equals ur.SysUserID
                                    join r in unitOfWork.SysRoleRepository.Get() on ur.SysRoleID equals r.ID
                                    where u.Account.Equals(tbxUserName.Text)
                                    select new { UserAccount = u.Account, RoleName = r.RoleName })
                                     .FirstOrDefault();

                    if (UserRole.RoleName.Equals("admin"))    //判断用户的角色
                    {
                        login.Visibility = Visibility.Collapsed;
                        UserManagementWindow.Visibility = Visibility.Visible;
                        Height = 427.406;
                        Width = 641.841;
                        var views = unitOfWork.ViewModelRepository.Get();    //listview的数据源绑定数据库                       
                        ListView.ItemsSource = views.ToList();
                        ListView.SelectedIndex = 0;
                    }
                    else if (UserRole.RoleName.Equals("教师"))
                    {
                        login.Visibility = Visibility.Collapsed;
                        TeacherWindow.Visibility = Visibility.Visible;
                        Height = 354.131;
                        Width = 350.841;
                    }
                    else
                    {
                        StudentWindow.Visibility = Visibility.Visible;
                        login.Visibility = Visibility.Collapsed;
                        Height = 354.131;
                        Width = 350.841;
                    }
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


            /*
            var user = unitOfWork.SysUserRepository.Get().Where(s => s.Account.Equals(tbxUserName.Text)).FirstOrDefault();//判断数据库中是否存在账号 ，如果纯在则返回对象不存在返回null
            if (user != null)
            {
                if (user.LoginOrnot != "1")  //如果该对象的“记住密码”没打勾
                {
                    if (user.Password.Equals(CreateMD5.EncryptWithMD5(pbxPassword.Password))) //判断密码框里填的密码是否正确
                    {
                        MessageBox.Show("登陆成功！");
                        if (rememberpwd.IsChecked == true)  //登录成功后再次判断是否选择了“记住密码”
                        {
                            user.LoginOrnot = "1";
                            unitOfWork.SysUserRepository.Update(user);
                            unitOfWork.Save();
                        }

                    }
                    else
                    {
                        throw new Exception("密码错误！");
                    }
                }
                else  //如果该对象的“记住密码”打勾了则直接登录
                {
                    if (pbxPassword.Password == user.Password)
                    {
                        MessageBox.Show("登陆成功！");


                    }
                    else if (user.Password.Equals(CreateMD5.EncryptWithMD5(pbxPassword.Password))) //如果该对象的“记住密码”为“没选”则判断密码登陆
                    {
                        MessageBox.Show("登陆成功！");
                    }
                    else
                    {
                        pbxPassword.Password = "";
                        throw new Exception("密码错误！");
                    }
                    if (rememberpwd.IsChecked == false)//选择记住密码的登陆，判断是否未选择记住密码
                    {
                        user.LoginOrnot = "0";
                        unitOfWork.SysUserRepository.Update(user);
                        unitOfWork.Save();
                        pbxPassword.Password = "";
                    }                     
                }
            }
            else
            {
                throw new Exception("账号不存在！");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("登陆失败！错误信息：\n" + ex.Message);
        }
        */
            /*
            if (tbxUserName.Text == "" || pbxPassword.Password == "")
            {
                MessageBox.Show("用户名或者密码不能为空！");
            }
            else
            {
                var user = unitOfWork.SysUserRepository.Get().Where(s => s.Account.Equals(tbxUserName.Text) && s.Password.Equals(CreateMD5.EncryptWithMD5(pbxPassword.Password))).FirstOrDefault();//判断数据库中是否存在账号密码
                var users = unitOfWork.SysUserRepository.Get().Where(s => s.Account.Equals(tbxUserName.Text) && s.LoginOrnot.Equals("Yes")).FirstOrDefault();
                if (users!=null)//如果用户名存在并且数据库标记为记住密码（“Yes”）
                {
                    MessageBox.Show("登入成功！");
                    if (rememberpwd.IsChecked == true)//如果打勾了记住密码，就把对应登陆成功的账号标记为“Yes”
                    {
                        users.LoginOrnot = "1";
                        unitOfWork.SysUserRepository.Update(users);
                        unitOfWork.Save();
                    }
                    else
                    {
                        users.LoginOrnot = "0";
                        unitOfWork.SysUserRepository.Update(users);
                        unitOfWork.Save();
                    }

                }
                else if (user == null)
                {               
                    MessageBox.Show("用户名不存在或原密码输入错误！");                                
                }
                else
                {
                    MessageBox.Show("登入成功！");
                    if (rememberpwd.IsChecked == true)//如果打勾了记住密码，就把对应登陆成功的账号标记为“Yes”
                    {
                        user.LoginOrnot = "1";
                        unitOfWork.SysUserRepository.Update(user);
                        unitOfWork.Save();
                    }
                    else
                    {
                        user.LoginOrnot = "0";
                        unitOfWork.SysUserRepository.Update(user);
                        unitOfWork.Save();
                    }
                }               
            }
            */

        }

        private void Seepwd_Checked(object sender, RoutedEventArgs e)//登陆界面中显示密码事件
        {
            tbxPassword.Visibility = Visibility.Visible;
            pbxPassword.Visibility = Visibility.Collapsed;
            tbxPassword.Text = pbxPassword.Password;
        }

        private void Seepwd_Unchecked(object sender, RoutedEventArgs e)//登陆界面中隐藏密码事件
        {
            pbxPassword.Visibility = Visibility.Visible;
            tbxPassword.Visibility = Visibility.Collapsed;
            pbxPassword.Password = tbxPassword.Text;
        }
        #endregion

        #region 修改密码界面
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

        private void ChangeSeepwd_Checked(object sender, RoutedEventArgs e)//修改密码界面中显示密码事件
        {
            tbxoldpbxPassword.Visibility = Visibility.Visible;
            oldpbxPassword.Visibility = Visibility.Collapsed;
            tbxoldpbxPassword.Text = oldpbxPassword.Password;

            tbxnewpbxPassword.Visibility = Visibility.Visible;
            newpbxPassword.Visibility = Visibility.Collapsed;
            tbxnewpbxPassword.Text = newpbxPassword.Password;

            tbxconfirmpbxPassword.Visibility = Visibility.Visible;
            confirmpbxPassword.Visibility = Visibility.Collapsed;
            tbxconfirmpbxPassword.Text = confirmpbxPassword.Password;
        }

        private void ChangeSeepwd_Unchecked(object sender, RoutedEventArgs e)//修改密码界面中隐藏密码事件
        {
            tbxoldpbxPassword.Visibility = Visibility.Collapsed;
            oldpbxPassword.Visibility = Visibility.Visible;
            oldpbxPassword.Password = tbxoldpbxPassword.Text;

            tbxnewpbxPassword.Visibility = Visibility.Collapsed;
            newpbxPassword.Visibility = Visibility.Visible;
            newpbxPassword.Password = tbxnewpbxPassword.Text;

            tbxconfirmpbxPassword.Visibility = Visibility.Collapsed;
            confirmpbxPassword.Visibility = Visibility.Visible;
            confirmpbxPassword.Password = tbxconfirmpbxPassword.Text;
        }
        #endregion

        #region 注册账号界面
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
            Account_register.Text = "";
            Password_register.Password = "";
            textPassword_register.Text = "";
            SurePassword_register.Password = "";
            textSurePassword_register.Text = "";
            QuestionAnswer_register.Text = "";
        }

        private void RegisterReturn2_Click(object sender, RoutedEventArgs e)//在用户管理界面点击“增加”按钮时出现的注册账号界面的“返回”按钮事件
        {
            var view = unitOfWork.ViewModelRepository.Get().ToList();
            ListView.ItemsSource = view;
            //var view = unitOfWork.ViewModelRepository.Get();    //listview的数据源绑定数据库                       
            // ListView.ItemsSource = view.ToList();
            ListView.SelectedIndex = 0;
            ListView.Items.Refresh();
            RegisterWindow.Visibility = Visibility.Collapsed;
            ListView.Visibility = Visibility.Visible;
            Height = 427.406;
            Width = 641.841;
            Account_register.Text = "";
            Password_register.Password = "";
            textPassword_register.Text = "";
            SurePassword_register.Password = "";
            textSurePassword_register.Text = "";
            QuestionAnswer_register.Text = "";
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
                        if (Account_register.Text.Trim() != "")
                        {
                            if(Password_register.Password != "")
                            {
                                if(Password_register.Password.Length > 8)
                                 {

                                    if (SurePassword_register.Password.Equals(Password_register.Password))//判断密码与确认密码是否相等
                                    {
                                        if (QuestionAnswer_register.Text != "")
                                        {
                                            var sysRole = unitOfWork.SysRoleRepository.Get().Where(s => s.RoleName.Equals(cbxUserRoleRegister.Text)).FirstOrDefault();    //寻找用户所选择角色在UserRole里的实例，返回对象
                                            if (sysRole != null)
                                            {
                                                var CurrentUser = new SysUser();
                                                CurrentUser.Account = Account_register.Text;
                                                CurrentUser.Password = CreateMD5.EncryptWithMD5(Password_register.Password);
                                                CurrentUser.QuestionAnswer = CreateMD5.EncryptWithMD5(QuestionAnswer_register.Text);
                                                CurrentUser.LoginOrnot = "0";
                                                unitOfWork.SysUserRepository.Insert(CurrentUser);    //增加新SysUser
                                                unitOfWork.Save();


                                                var CurrentUserRole = new SysUserRole();
                                                CurrentUserRole.SysUserID = CurrentUser.ID;
                                                CurrentUserRole.SysRoleID = sysRole.ID;
                                                unitOfWork.SysUserRoleRepository.Insert(CurrentUserRole);    //增加新SysUserRole
                                                unitOfWork.Save();


                                                var CurrentViewModel = new ViewModel();
                                                CurrentViewModel.ViewUserAccount = CurrentUser.Account;
                                                CurrentViewModel.ViewUserID = CurrentUser.ID;
                                                CurrentViewModel.ViewRoleName = sysRole.RoleName;
                                                CurrentViewModel.ViewRoleDec = sysRole.RoleDec;
                                                unitOfWork.ViewModelRepository.Insert(CurrentViewModel);    //增加新ViewModel
                                                unitOfWork.Save(); 



                                                MessageBox.Show("注册成功");
                                                var user = unitOfWork.SysUserRepository.Get();         //combobox的更新
                                                tbxUserName.ItemsSource = user.ToList();       //combobox数据源连接数据库
                                                tbxUserName.DisplayMemberPath = "Account";  //combobox下拉显示的值
                                                tbxUserName.SelectedValuePath = "Account";  //combobox选中项显示的值
                                                tbxUserName.SelectedIndex = 0;              //登陆界面 combobox初始显示第一项



                                                RegisterWindow.Visibility = Visibility.Collapsed;
                                                login.Visibility = Visibility.Visible;
                                                loginsystem.Height = 514.089;
                                                loginsystem.Width = 848.5;

                                                Account_register.Text = "";
                                                Password_register.Password = "";
                                                textPassword_register.Text = "";
                                                SurePassword_register.Password = "";
                                                textSurePassword_register.Text = "";
                                                QuestionAnswer_register.Text = "";
                                            }
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
                                    MessageBox.Show("您的密码安全性较低，请重新设置大于8个字符的密码！");
                                }
                            }
                            else
                            {
                                MessageBox.Show("密码不能为空！");
                            }
                        }
                        else
                        {
                            throw new Exception("用户名不能全是空格！");
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
        private void SeepwdRegister_Checked(object sender, RoutedEventArgs e)//注册界面中显示密码事件
        {
            textPassword_register.Visibility = Visibility.Visible;
            Password_register.Visibility = Visibility.Collapsed;
            textPassword_register.Text = Password_register.Password;

            textSurePassword_register.Visibility = Visibility.Visible;
            SurePassword_register.Visibility = Visibility.Collapsed;
            textSurePassword_register.Text = SurePassword_register.Password;
        }

        private void SeepwdRegister_Unchecked(object sender, RoutedEventArgs e)//注册界面中隐藏密码事件
        {
            textPassword_register.Visibility = Visibility.Collapsed;
            Password_register.Visibility = Visibility.Visible;
            Password_register.Password = textPassword_register.Text;

            textSurePassword_register.Visibility = Visibility.Collapsed;
            SurePassword_register.Visibility = Visibility.Visible;
            SurePassword_register.Password = textSurePassword_register.Text;
        }
        #endregion

        #region 找回密码界面
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
                                    MessageBox.Show("密码拾回成功，请重新登录！");
                                    RetrievePasswordWindow.Visibility = Visibility.Collapsed;
                                    login.Visibility = Visibility.Visible;
                                    loginsystem.Height = 514.089;
                                    loginsystem.Width = 848.5;

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

        private void RetrieveSeepwd_Checked(object sender, RoutedEventArgs e)//找回密码界面中显示密码事件
        {
            tbxNewPassword_etrievePwd.Visibility = Visibility.Visible;
            NewPassword_etrievePwd.Visibility = Visibility.Collapsed;
            tbxNewPassword_etrievePwd.Text = NewPassword_etrievePwd.Password;

            tbxSurePassword_etrievePwd.Visibility = Visibility.Visible;
            SurePassword_etrievePwd.Visibility = Visibility.Collapsed;
            tbxSurePassword_etrievePwd.Text = SurePassword_etrievePwd.Password;
        }

        private void RetrieveSeepwd_Unchecked(object sender, RoutedEventArgs e)//找回密码界面中隐藏密码事件
        {
            tbxNewPassword_etrievePwd.Visibility = Visibility.Collapsed;
            NewPassword_etrievePwd.Visibility = Visibility.Visible;
            NewPassword_etrievePwd.Password = tbxNewPassword_etrievePwd.Text;

            tbxSurePassword_etrievePwd.Visibility = Visibility.Collapsed;
            SurePassword_etrievePwd.Visibility = Visibility.Visible;
            SurePassword_etrievePwd.Password = tbxSurePassword_etrievePwd.Text;
        }
        #endregion

        #region 初始化数据
        private void Loginsystem_Loaded(object sender, RoutedEventArgs e)//运行界面的初始化数据
        {
            var users = unitOfWork.SysUserRepository.Get();
            tbxUserName.ItemsSource = users.ToList();
            tbxUserName.DisplayMemberPath = "Account";
            tbxUserName.SelectedValuePath = "Account";
            //this.cbxUser.Items.Add(user.UserName);
            tbxUserName.SelectedIndex = 0;//combobox初始选择第一项
            var u = users.Where(s => s.Account.Equals(tbxUserName.Text)).FirstOrDefault();//通过在数据库中搜寻combobox第一项的值返回一个以combobox第一项的值为UserAccount的对象
            if (u != null)
            {
                if (u.LoginOrnot == "1")              //如果该对象的“记住密码”已打勾
                {
                    pbxPassword.Password = u.Password;//随便给passwordbox一串密码
                    rememberpwd.IsChecked = true;     //让记住密码选择框显示选中
                }
            }
        }
        #endregion
      
        #region 用户管理界面

        private void UserManagementWindowMin_Click(object sender, RoutedEventArgs e)//缩小用户管理界面
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void UserManagementWindowClose_Click(object sender, RoutedEventArgs e)//关闭用户管理界面
        {
            SystemCommands.CloseWindow(this);
        }

        private void UserManagementWindowReturn_Click(object sender, RoutedEventArgs e)//用户管理界面的返回事件
        {
            UserManagementWindow.Visibility = Visibility.Collapsed;
            login.Visibility = Visibility.Visible;
            loginsystem.Height = 514.089;
            loginsystem.Width = 848.5;
        }

        private void UserManagementWindowAdd_Click(object sender, RoutedEventArgs e)//用户管理界面的“增加”按钮事件
        {
            RegisterReturn2.Visibility = Visibility.Visible;
            RegisterReturn.Visibility = Visibility.Collapsed;
            ListView.Visibility = Visibility.Collapsed;
            RegisterWindow.Visibility = Visibility.Visible;
            loginsystem.Height = 447.739;
            loginsystem.Width = 424.623;
            unitOfWork.Save();
            //刷新列表
            var view = unitOfWork.ViewModelRepository.Get().ToList();
            ListView.ItemsSource = view;
            ListView.SelectedIndex = 0;
            ListView.Items.Refresh();
            //combobox的刷新
            var user = unitOfWork.SysUserRepository.Get();
            tbxUserName.ItemsSource = user.ToList();       //combobox数据源连接数据库
            tbxUserName.DisplayMemberPath = "Account";  //combobox下拉显示的值
            tbxUserName.SelectedValuePath = "Account";  //combobox选中项显示的值
            tbxUserName.SelectedIndex = 0;               //登陆界面 combobox初始显示第一项
        }

        private void UserManagementWindowDelete_Click(object sender, RoutedEventArgs e)//用户管理界面的“删除”按钮事件
        {
            var viewModel = (ViewModel)ListView.SelectedItem;
            //找到选中用户ID在SysUser表里的实例
            var sysUser = unitOfWork.SysUserRepository.Get().Where(s => s.ID == viewModel.ViewUserID).FirstOrDefault();
            //找到选中用户ID在SysUserRole表里的实例
            var sysUserRole = unitOfWork.SysUserRoleRepository.Get().Where(s => s.SysUserID == viewModel.ViewUserID).FirstOrDefault();
            //删除数据库中SysUser表相应的值
            unitOfWork.SysUserRepository.Delete(sysUser);
            unitOfWork.Save();
            //删除数据库中SysUserRole表相应的值
            unitOfWork.SysUserRoleRepository.Delete(sysUserRole);
            unitOfWork.Save();
            //删除数据库中ViewModel表相应的值
            unitOfWork.ViewModelRepository.Delete(viewModel);
            unitOfWork.Save();

            //刷新列表
            var view = unitOfWork.ViewModelRepository.Get().ToList();
            ListView.ItemsSource = view;
            ListView.SelectedIndex = 0;
            ListView.Items.Refresh();
            //combobox的刷新
            var user = unitOfWork.SysUserRepository.Get();
            tbxUserName.ItemsSource = user.ToList();       //combobox数据源连接数据库
            tbxUserName.DisplayMemberPath = "Account";  //combobox下拉显示的值
            tbxUserName.SelectedValuePath = "Account";  //combobox选中项显示的值
            tbxUserName.SelectedIndex = 0;               //登陆界面 combobox初始显示第一项
        }

        private void UserManagementWindowChangeRole_Click(object sender, RoutedEventArgs e)//用户管理界面的“修改角色”按钮事件
        {
            ChangeRoleWindow.Visibility = Visibility.Visible;
            UserManagementWindow.Visibility = Visibility.Collapsed;
            loginsystem.Height = 354.131;
            loginsystem.Width = 350.841;
            var viewModel = (ViewModel)ListView.SelectedItem;
            LabTextChangeRole.Content = "更改" + viewModel.ViewUserAccount + "用户的角色";

        }



        #endregion

        #region 修改用户角色界面

        private void ManagementWindowChangeRole_Click(object sender, RoutedEventArgs e)//修改用户角色界面的“修改”按钮事件
        {
            try
            {
                if (cheStudentChangeRole.IsChecked == false && cheTeacherChangeRole.IsChecked == false && cheAdminChangeRole.IsChecked == false)
                {
                    throw new Exception("请选择角色！");
                }
                else if (cheTeacherChangeRole.IsChecked == false && cheAdminChangeRole.IsChecked == true && cheStudentChangeRole.IsChecked == false || cheTeacherChangeRole.IsChecked == true && cheAdminChangeRole.IsChecked == false && cheStudentChangeRole.IsChecked == false || cheTeacherChangeRole.IsChecked == false && cheAdminChangeRole.IsChecked == false && cheStudentChangeRole.IsChecked == true)
                {
                    MessageBox.Show("修改成功！");
                    //更改角色
                    var viewModel = (ViewModel)ListView.SelectedItem;
                    var sysUserRole = unitOfWork.SysUserRoleRepository.Get().Where(s => s.SysUserID == viewModel.ViewUserID).FirstOrDefault();    //寻找用户在表里的实例，返回对象                 

                    if (cheTeacherChangeRole.IsChecked == true)
                    {
                        viewModel.ViewRoleName = "教师";//更改viewModel表里的角色名
                        unitOfWork.Save();
                        var sysRole = unitOfWork.SysRoleRepository.Get().Where(s => s.RoleName.Equals("教师")).FirstOrDefault();//更改sysUserRole表的对应关系
                        sysUserRole.SysRoleID = sysRole.ID;
                        unitOfWork.Save();
                    }
                    else if (cheStudentChangeRole.IsChecked == true)
                    {
                        viewModel.ViewRoleName = "学生";//更改viewModel表里的角色名
                        unitOfWork.Save();
                        var sysRole = unitOfWork.SysRoleRepository.Get().Where(s => s.RoleName.Equals("学生")).FirstOrDefault();//更改sysUserRole表的对应关系
                        sysUserRole.SysRoleID = sysRole.ID;
                        unitOfWork.Save();
                    }
                    else
                    {
                        viewModel.ViewRoleName = "admin";//更改viewModel表里的角色名
                        unitOfWork.Save();
                        var sysRole = unitOfWork.SysRoleRepository.Get().Where(s => s.RoleName.Equals("admin")).FirstOrDefault();//更改sysUserRole表的对应关系
                        sysUserRole.SysRoleID = sysRole.ID;
                        unitOfWork.Save();
                    }
                    //刷新列表
                    var view = unitOfWork.ViewModelRepository.Get().ToList();
                    ListView.ItemsSource = view;
                    ListView.SelectedIndex = 0;
                    ListView.Items.Refresh();
                    //自动返回ListView界面
                    ChangeRoleWindow.Visibility = Visibility.Collapsed;
                    UserManagementWindow.Visibility = Visibility.Visible;
                    loginsystem.Height = 427.406;
                    loginsystem.Width = 641.841;
                    //combobox的刷新
                    var user = unitOfWork.SysUserRepository.Get();
                    tbxUserName.ItemsSource = user.ToList();       //combobox数据源连接数据库
                    tbxUserName.DisplayMemberPath = "Account";  //combobox下拉显示的值
                    tbxUserName.SelectedValuePath = "Account";  //combobox选中项显示的值
                    tbxUserName.SelectedIndex = 0;               //登陆界面 combobox初始显示第一项

                }
                else
                {
                    throw new Exception("只能选择一位角色，请重新选择！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("修改失败！错误信息：\n" + ex.Message);
            }
        }

        private void ChangeRoleWindowReturn_Click(object sender, RoutedEventArgs e)//修改用户角色界面的“返回”按钮事件
        {
            ChangeRoleWindow.Visibility = Visibility.Collapsed;
            UserManagementWindow.Visibility = Visibility.Visible;
            loginsystem.Height = 427.406;
            loginsystem.Width = 641.841;
        }

        #endregion

        private void TbxUserName_SelectionChanged(object sender, SelectionChangedEventArgs e)//对于改变combobox的菜单事件，如果改变后的账号仍是有记住密码的，则passwordbox里继续有乱密码，否则里面为空
        {
            var users = unitOfWork.SysUserRepository.Get();
            String str = tbxUserName.SelectedValue.ToString();  //获取combobox选择的值
            if (str != null)
            {
                var u = users.Where(s => s.Account.Equals(str)).FirstOrDefault(); //通过在数据库中搜寻combobox选择的值返回一个以combobox选择的值为UserAccount的对象
                if (u.LoginOrnot == "1")
                {
                    pbxPassword.Password = u.Password;
                    rememberpwd.IsChecked = true;
                }
                else
                {
                    pbxPassword.Password = "";
                    rememberpwd.IsChecked = false;
                }
            }

            /* if(e.RemovedItems.Count > 0)
             {
                 var users = unitOfWork.SysUserRepository.Get();
                 var u = users.FirstOrDefault();
                 if (u.LoginOrnot == "Yes" && tbxUserName.Text == u.Account)
                 {
                     pbxPassword.Password = CreateMD5.EncryptWithMD5(u.Password);//随便给passwordbox一串密码
                 }
                 else
                 {
                     pbxPassword.Password = "";
                 }

             }
             */

        }

        private void StudentWindowReturn_Click(object sender, RoutedEventArgs e)
        {
            StudentWindow.Visibility = Visibility.Collapsed;
            login.Visibility = Visibility.Visible;
            Height = 514.089;
            Width = 848.5;
        }//学生窗口返回事件

        private void TeacherWindowReturn_Click(object sender, RoutedEventArgs e)
        {
            TeacherWindow.Visibility = Visibility.Collapsed;
            login.Visibility = Visibility.Visible;
            Height = 514.089;
            Width = 848.5;
        }//教师窗口返回事件
    }
}

