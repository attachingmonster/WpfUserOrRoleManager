using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
using WpfUserOrRoleManager.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Security.Policy;

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
        HttpClient client = new HttpClient();
        
        public MainWindow()
        {
            InitializeComponent();
            var user = unitOfWork.SysUserRepository.Get();
           
        
            
           
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = (from us in unitOfWork.SysUserRepository.Get()
                             join ur in unitOfWork.SysUserRoleRepository.Get() on us.ID equals ur.SysUserID
                             join r in unitOfWork.SysRoleRepository.Get() on ur.SysRoleID equals r.ID

                             select new ViewModel { ViewUserAccount = us.UserAccount, ViewRoleName = r.RoleName, ViewRoleDec = r.RoleDec }).ToList();
            ListView.ItemsSource = viewModel;
            ListView.SelectedIndex = 0;
            var user = unitOfWork.SysUserRepository.Get();
            cbxUserAccountLogin.ItemsSource = user.ToList();       //combobox数据源连接数据库
            cbxUserAccountLogin.DisplayMemberPath = "UserAccount";  //combobox下拉显示的值
            cbxUserAccountLogin.SelectedValuePath = "UserAccount";  //combobox选中项显示的值
            cbxUserAccountLogin.SelectedIndex = 0;               //登录界面 combobox初始显示第一项
            var u = user.Where(s => s.UserAccount.Equals(cbxUserAccountLogin.Text)).FirstOrDefault();//通过在数据库中搜寻combobox第一项的值返回一个以combobox第一项的值为UserAccount的对象
            if (u != null)
            {
                if (u.RememberPassword == "isChecked")              //判断该对象的 记住密码 是否为 已选
                {
                    pbxUserPasswordLogin.Password = u.UserPassword;//给passwordbox一串固定密码
                    cheRememberPwdLogin.IsChecked = true;     //让记住密码选择框显示选中
                }
            }
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
        #region 登录界面
        private void Login_Click(object sender, RoutedEventArgs e)    //登录事件
        {
            try
            {
                //判断数据库中的(是否存在账号and(输入密码正确or(已记住密码and给定密码正确))逻辑是否存在 ，如果存在则返回对象，否则返回null
                var sysUser = unitOfWork.SysUserRepository.Get().Where(s => s.UserAccount.Equals(cbxUserAccountLogin.Text) && (s.UserPassword.Equals(CreateMD5.EncryptWithMD5(pbxUserPasswordLogin.Password)) || (s.RememberPassword.Equals("1") && s.UserPassword.Equals(pbxUserPasswordLogin.Password)))).FirstOrDefault();    
                if (sysUser != null)
                {
                    

                    //判断登录成功时是否记住密码
                    if (cheRememberPwdLogin.IsChecked == true)   
                    {
                        sysUser.RememberPassword = "1";
                        unitOfWork.SysUserRepository.Update(sysUser);
                        unitOfWork.Save();
                    }
                    else
                    {
                        sysUser.RememberPassword = "0";
                        unitOfWork.SysUserRepository.Update(sysUser);
                        unitOfWork.Save();
                    }
    

                    //linq 多表查询得到选择的用户的角色，暂时一个账号只有一个角色
                    var UserRole = (from u in unitOfWork.SysUserRepository.Get()
                                   join ur in unitOfWork.SysUserRoleRepository.Get() on u.ID equals ur.SysUserID
                                   join r in unitOfWork.SysRoleRepository.Get() on ur.SysRoleID equals r.ID
                                   where u.UserAccount.Equals(cbxUserAccountLogin.Text)
                                   select new { UserAccount=u.UserAccount , RoleName=r.RoleName  })
                                   .FirstOrDefault();
                   
                    
                    # region 不同角色进入不同窗口
                    if (UserRole.RoleName.Equals("admin"))    //判断用户的角色
                    {
                        LabTextListView.Content = sysUser.UserAccount + "用户为管理员";
                        LoginWindow.Visibility = Visibility.Collapsed;
                        ListViewWindow.Visibility = Visibility.Visible;
                        Height = 421;
                        Width = 656;
                                           
                        
                    }
                    else if (UserRole.RoleName.Equals("教师"))
                    {
                        ListViewWindow.Visibility = Visibility.Collapsed;
                        TeacherWindow.Visibility = Visibility.Visible;
                        Left = 0;
                        Top = 0;
                        Width = 1920;
                        Height = 1080;
                    }
                    else
                    {
                        StudentWindow.Visibility = Visibility.Visible;
                        LoginWindow.Visibility = Visibility.Collapsed;
                        Left = 0;
                        Top = 0;
                        Width = 1920;
                        Height = 1080;
                    }
                    #endregion
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
        private void UserAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)    //combobox选择改变发生事件
        {
       
                var users = unitOfWork.SysUserRepository.Get();
                if (cbxUserAccountLogin.SelectedValue != null)//获取combobox选择的值
                {
                    //通过在数据库中搜寻combobox选择的值返回一个以combobox选择的值为UserAccount的对象
                    var sysUser = users.Where(s => s.UserAccount.Equals(cbxUserAccountLogin.SelectedValue.ToString())).FirstOrDefault(); 
                    if (sysUser != null)
                    {
                        //comboboxde选中的账号是否记忆密码
                        if (sysUser.RememberPassword == "1")
                        {
                            pbxUserPasswordLogin.Password = sysUser.UserPassword;
                            cheRememberPwdLogin.IsChecked = true;
                        }
                        else
                        {
                            pbxUserPasswordLogin.Password = "";
                            cheRememberPwdLogin.IsChecked = false;
                        }
                    }
                   
                }
          
        }




        #endregion
        #region 用户管理界面

        

        private void ListViewDelete_Click(object sender, RoutedEventArgs e) //Delete
        {
            var viewModel = (ViewModel)ListView.SelectedItem;
            //找到显示用户ID在SysUser表里的实例
            var sysUser = unitOfWork.SysUserRepository.Get().Where(s => s.UserAccount.Equals(viewModel.ViewUserAccount)).FirstOrDefault();
            //找到显示用户ID在SysUserRole表里的实例
            var sysUserRole = unitOfWork.SysUserRoleRepository.Get().Where(s => s.SysUserID==sysUser.ID).FirstOrDefault();
            //删除数据库中SysUser表相应的值
            unitOfWork.SysUserRepository.Delete(sysUser);   
            unitOfWork.Save();
            //删除数据库中SysUserRole表相应的值
            unitOfWork.SysUserRoleRepository.Delete(sysUserRole);
            unitOfWork.Save();
           

            //刷新列表
            var viewModels = (from us in unitOfWork.SysUserRepository.Get()
                             join ur in unitOfWork.SysUserRoleRepository.Get() on us.ID equals ur.SysUserID
                             join r in unitOfWork.SysRoleRepository.Get() on ur.SysRoleID equals r.ID

                             select new ViewModel { ViewUserAccount = us.UserAccount, ViewRoleName = r.RoleName, ViewRoleDec = r.RoleDec }).ToList();
            ListView.ItemsSource = viewModels;
            ListView.SelectedIndex = 0;
            ListView.Items.Refresh();
            //combobox的刷新
            var user = unitOfWork.SysUserRepository.Get();
            cbxUserAccountLogin.ItemsSource = user.ToList();       //combobox数据源连接数据库
            cbxUserAccountLogin.DisplayMemberPath = "UserAccount";  //combobox下拉显示的值
            cbxUserAccountLogin.SelectedValuePath = "UserAccount";  //combobox选中项显示的值
            cbxUserAccountLogin.SelectedIndex = 0;               //登陆界面 combobox初始显示第一项

        }
        private void ListViewAdd_Click(object sender, RoutedEventArgs e) //Add
        {
            btnBack2_register.Visibility = Visibility.Visible;
            btnBack1_register.Visibility = Visibility.Collapsed;
            ListViewWindow.Visibility = Visibility.Collapsed;
            RegisterWindow.Visibility = Visibility.Visible;
            Height = 441;
            unitOfWork.Save();
            //刷新列表
            var viewModels = (from us in unitOfWork.SysUserRepository.Get()
                              join ur in unitOfWork.SysUserRoleRepository.Get() on us.ID equals ur.SysUserID
                              join r in unitOfWork.SysRoleRepository.Get() on ur.SysRoleID equals r.ID

                              select new ViewModel { ViewUserAccount = us.UserAccount, ViewRoleName = r.RoleName, ViewRoleDec = r.RoleDec }).ToList();
            ListView.ItemsSource = viewModels;
            ListView.SelectedIndex = 0;
            ListView.Items.Refresh();
            //combobox的刷新
            var user = unitOfWork.SysUserRepository.Get();
            cbxUserAccountLogin.ItemsSource = user.ToList();       //combobox数据源连接数据库
            cbxUserAccountLogin.DisplayMemberPath = "UserAccount";  //combobox下拉显示的值
            cbxUserAccountLogin.SelectedValuePath = "UserAccount";  //combobox选中项显示的值
            cbxUserAccountLogin.SelectedIndex = 0;               //登陆界面 combobox初始显示第一项
        }
        private void ListViewChangeRole_Click(object sender, RoutedEventArgs e) //ChangeRole
        {
            ChangeRoleWindow.Visibility = Visibility.Visible;
            ListViewWindow.Visibility = Visibility.Collapsed;
            Left = 750;
            Top = 430;
            Height = 310;
            Width = 307;

            var viewModel = (ViewModel)ListView.SelectedItem;
            LabTextChangeRole.Content = "更改" + viewModel.ViewUserAccount + "用户的角色";
      
            if (viewModel.ViewRoleName.Equals(cheAdminChangeRole.Content))
            {
                cheAdminChangeRole.IsChecked = true;
            }
            else if (viewModel.ViewRoleName.Equals(cheStudentChangeRole.Content))
            {
                cheStudentChangeRole.IsChecked = true;
            }
            else
            {
                cheTeacherChangeRole.IsChecked = true;
            }
        }
        private void ListViewBack_Click(object sender, RoutedEventArgs e)    //切换ListViewWindow与LoginWindow界面
        {
            
            //如果登陆时选中未记忆密码则清空密码
            var sysUser = unitOfWork.SysUserRepository.Get().Where(s => s.UserAccount.Equals(cbxUserAccountLogin.Text)).FirstOrDefault();
            if (sysUser.RememberPassword == "0")
            {
                pbxUserPasswordLogin.Password = "";
            }
            ListViewWindow.Visibility = Visibility.Collapsed;
            LoginWindow.Visibility = Visibility.Visible;
            Height = 311;
            Width = 536;

        }

        #endregion
        #region 修改用户角色界面




        private void ChangeRole_Click(object sender, RoutedEventArgs e)
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
                    var UserRole = unitOfWork.SysRoleRepository.Get().Where(s => s.RoleName.Equals(viewModel.ViewRoleName)).FirstOrDefault();
                    var sysUserRole = unitOfWork.SysUserRoleRepository.Get().Where(s => s.ID == UserRole.ID).FirstOrDefault();            
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
                    var viewModels = (from us in unitOfWork.SysUserRepository.Get()
                                      join ur in unitOfWork.SysUserRoleRepository.Get() on us.ID equals ur.SysUserID
                                      join r in unitOfWork.SysRoleRepository.Get() on ur.SysRoleID equals r.ID

                                      select new ViewModel { ViewUserAccount = us.UserAccount, ViewRoleName = r.RoleName, ViewRoleDec = r.RoleDec }).ToList();
                    ListView.ItemsSource = viewModels;
                    ListView.SelectedIndex = 0;
                    ListView.Items.Refresh();
                    //自动返回ListView界面
                    LabTextListView.Content = cbxUserAccountLogin.Text + "用户为管理员";
                    Top = 385;
                    Left = 692;
                    ChangeRoleWindow.Visibility = Visibility.Collapsed;
                    ListViewWindow.Visibility = Visibility.Visible;
                    Height = 421;
                    Width = 656;
                    //combobox的刷新
                    var user = unitOfWork.SysUserRepository.Get();
                    cbxUserAccountLogin.ItemsSource = user.ToList();       //combobox数据源连接数据库
                    cbxUserAccountLogin.DisplayMemberPath = "UserAccount";  //combobox下拉显示的值
                    cbxUserAccountLogin.SelectedValuePath = "UserAccount";  //combobox选中项显示的值
                    cbxUserAccountLogin.SelectedIndex = 0;               //登陆界面 combobox初始显示第一项

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
         //切换ListViewWindow与ChangeRoleWindow界面
        private void ChangeRoleBack_Click(object sender, RoutedEventArgs e) 
        {
            LabTextListView.Content = cbxUserAccountLogin.Text + "用户为管理员";
            ChangeRoleWindow.Visibility = Visibility.Collapsed;
            ListViewWindow.Visibility = Visibility.Visible;
            Top = 385;
            Left = 692;
            Height = 421;
            Width = 656;
        }



        #endregion
        #region 注册账号界面



        private void loginRegister_Click(object sender, RoutedEventArgs e)    //切换界面事件
        {
            LoginWindow.Visibility = Visibility.Collapsed;
            RegisterWindow.Visibility = Visibility.Visible;
            Height = 441;
        }
        private void Registering_Click(object sender, RoutedEventArgs e)
        {
            if (tbxUserPasswordRegister.Visibility != Visibility.Collapsed)    //如果显示密码按钮事件开始，则让显示密码值赋值给隐藏密码的值
            {
                pbxUserPasswordRegister.Password = tbxUserPasswordRegister.Text;
                pbxSurePasswordRegister.Password = tbxSurePasswordRegister.Text;
            }
            String UserAnswer = "1" + tbxUserAnswer1Register.Text + "2" + tbxUserAnswer2Register.Text + "3" + tbxUserAnswer3Register.Text + "4" + tbxUserAnswer4Register.Text + "5" + tbxUserAnswer5Register.Text;//拾回密码的各个答案与问题号的连接
            ViewModelRegister viewModelRegister = new ViewModelRegister();
            viewModelRegister.Account = tbxUserAccountRegister.Text;
            viewModelRegister.Password = pbxUserPasswordRegister.Password;
            viewModelRegister.RememberPasswerd = pbxSurePasswordRegister.Password;
            viewModelRegister.RememberPasswerd = "0";
            viewModelRegister.RoleName = cbxUserRoleRegister.Text;
            viewModelRegister.Answer = UserAnswer;
            viewModelRegister = ViewRegister(viewModelRegister);
            #region 注释
            /*try
            {
               
                if (tbxUserPasswordRegister.Visibility != Visibility.Collapsed)    //如果显示密码按钮事件开始，则让显示密码值赋值给隐藏密码的值
                {
                    pbxUserPasswordRegister.Password = tbxUserPasswordRegister.Text;
                    pbxSurePasswordRegister.Password = tbxSurePasswordRegister.Text;
                }
                if (tbxUserAccountRegister.Text != "")    //判断用户账号是否为空
                {
                    var u = unitOfWork.SysUserRepository.Get().Where(s => s.UserAccount.Equals(tbxUserAccountRegister.Text)).FirstOrDefault();   //查找是否存在账号，存在返回账号所在对象，否则返回null
                    if (u == null)    //判断账号是否存在
                    {
                        #region 账号规范
                        foreach (char c in tbxUserAccountRegister.Text)   //规范账号必须由字母和数字构成
                        {
                            if (!(('0' <= c && c <= '9') || ('A' <= c && c <= 'Z') || ('a' <= c && c <= 'z')))
                            {
                                throw new Exception("账号必须只由字母和数字构成！");
                            }
                        }
                        #endregion
                        if (pbxUserPasswordRegister.Password != "")    //判断密码是否为空
                        {
                            #region 密码规范
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
                            #endregion
                            if (pbxSurePasswordRegister.Password.Equals(pbxUserPasswordRegister.Password))    //判断密码与确认密码是否相等
                            {
                                String UserAnswer = "1" + tbxUserAnswer1Register.Text + "2" + tbxUserAnswer2Register.Text + "3" + tbxUserAnswer3Register.Text + "4" + tbxUserAnswer4Register.Text + "5" + tbxUserAnswer5Register.Text;//拾回密码的各个答案与问题号的连接
                                if (UserAnswer != "1" + "2" + "3" + "4" + "5")    //判断拾回密码是否为空
                                {
                                    var sysRole = unitOfWork.SysRoleRepository.Get().Where(s => s.RoleName.Equals(cbxUserRoleRegister.Text)).FirstOrDefault();    //寻找用户所选择角色在UserRole里的实例，返回对象
                                    if (sysRole != null)
                                    {
                                        var CurrentUser = new SysUser();
                                        CurrentUser.UserAccount = tbxUserAccountRegister.Text;
                                        CurrentUser.UserPassword = CreateMD5.EncryptWithMD5(pbxUserPasswordRegister.Password);
                                        CurrentUser.UserAnswer = CreateMD5.EncryptWithMD5(UserAnswer);
                                        CurrentUser.RememberPassword = "0";
                                        unitOfWork.SysUserRepository.Insert(CurrentUser);    //增加新SysUser
                                        unitOfWork.Save();

                                        var CurrentUserRole = new SysUserRole();
                                        CurrentUserRole.SysUserID = CurrentUser.ID;
                                        CurrentUserRole.SysRoleID = sysRole.ID;
                                        unitOfWork.SysUserRoleRepository.Insert(CurrentUserRole);    //增加新SysUserRole
                                        unitOfWork.Save();    //对更改进行保存
                                        MessageBox.Show("注册成功");
                                        var user = unitOfWork.SysUserRepository.Get();         //combobox的更新
                                        cbxUserAccountLogin.ItemsSource = user.ToList();       //combobox数据源连接数据库
                                        cbxUserAccountLogin.DisplayMemberPath = "UserAccount";  //combobox下拉显示的值
                                        cbxUserAccountLogin.SelectedValuePath = "UserAccount";  //combobox选中项显示的值
                                        cbxUserAccountLogin.SelectedIndex = 0;               //登陆界面 combobox初始显示第一项
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
            }*/
            #endregion
        }

        private void registerBack1_Click(object sender, RoutedEventArgs e)   //返回事件
        {
            RegisterWindow.Visibility = Visibility.Collapsed;
            LoginWindow.Visibility = Visibility.Visible;
            Height = 311;
            tbxUserAccountRegister.Text ="";
            pbxUserPasswordRegister.Password = "";
            pbxSurePasswordRegister.Password = "";
            tbxUserAnswer1Register.Text = "";
            tbxUserAnswer2Register.Text = "";
            tbxUserAnswer3Register.Text = "";
            tbxUserAnswer4Register.Text = "";
            tbxUserAnswer5Register.Text = "";
            tbxUserPasswordRegister.Text = "";
            tbxSurePasswordRegister.Text = "";

        }
        private void registerBack2_Click(object sender, RoutedEventArgs e)
        {
            var viewModels = (from us in unitOfWork.SysUserRepository.Get()
                              join ur in unitOfWork.SysUserRoleRepository.Get() on us.ID equals ur.SysUserID
                              join r in unitOfWork.SysRoleRepository.Get() on ur.SysRoleID equals r.ID

                              select new ViewModel { ViewUserAccount = us.UserAccount, ViewRoleName = r.RoleName, ViewRoleDec = r.RoleDec }).ToList();
            ListView.ItemsSource = viewModels;
            ListView.SelectedIndex = 0;
            ListView.Items.Refresh();
            LabTextListView.Content = cbxUserAccountLogin.Text + "用户为管理员";
            RegisterWindow.Visibility = Visibility.Collapsed;
            ListViewWindow.Visibility = Visibility.Visible;
            Height = 421;
            Width = 656;
            tbxUserAccountRegister.Text = "";
            pbxUserPasswordRegister.Password = "";
            pbxSurePasswordRegister.Password = "";
            tbxUserAnswer1Register.Text = "";
            tbxUserAnswer2Register.Text = "";
            tbxUserAnswer3Register.Text = "";
            tbxUserAnswer4Register.Text = "";
            tbxUserAnswer5Register.Text = "";
            tbxUserPasswordRegister.Text = "";
            tbxSurePasswordRegister.Text = "";
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
            tbxUserAccountEtrievePwd.Text = "";
            tbxUserAnswer1EtrievePwd.Text = "";
            tbxUserAnswer2EtrievePwd.Text = "";
            tbxUserAnswer3EtrievePwd.Text = "";
            tbxUserAnswer4EtrievePwd.Text = "";
            tbxUserAnswer5EtrievePwd.Text = "";

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
                    var u = unitOfWork.SysUserRepository.Get().Where(s => s.UserAccount.Equals(tbxUserAccountEtrievePwd.Text)).FirstOrDefault();//获取与需要拾回密码的SysUser
                    if (!u.UserPassword.Equals(CreateMD5.EncryptWithMD5(pbxUserPasswordResetPwd.Password)))
                    {
                        if (pbxSurePasswordResetPwd.Password.Equals(pbxUserPasswordResetPwd.Password))//判断密码与确认密码是否相等
                        {

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
                        throw new Exception("新密码不能与原密码一致！");
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
            pbxUserPasswordResetPwd.Password = "";
            pbxSurePasswordResetPwd.Password = "";
            tbxUserAccountEtrievePwd.Text = "";
            tbxUserAnswer1EtrievePwd.Text = "";
            tbxUserAnswer2EtrievePwd.Text = "";
            tbxUserAnswer3EtrievePwd.Text = "";
            tbxUserAnswer4EtrievePwd.Text = "";
            tbxUserAnswer5EtrievePwd.Text = "";
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
            tbxUserAccountChangePwd.Text= "";
            pbxOldPasswordChangePwd.Password = "";
            pbxUserPasswordChangePwd.Password = "";
            pbxSurePasswordChangePwd.Password = "";
        }









        #endregion
        private  void btn_(object sender, RoutedEventArgs e)
        {

            /*String Url = client.BaseAddress.ToString();
            String postDataStr = "123";
            bool isSuccess = false;
            string f = HttpPost(Url, postDataStr, ref isSuccess);
            var   viewModel = (from us in unitOfWork.SysUserRepository.Get()
                             join ur in unitOfWork.SysUserRoleRepository.Get() on us.ID equals ur.SysUserID
                             join r in unitOfWork.SysRoleRepository.Get() on ur.SysRoleID equals r.ID

                             select new ViewModel { ViewUserAccount = us.UserAccount, ViewRoleName = r.RoleName, ViewRoleDec = r.RoleDec }).ToList();

            Task<bool> flag =  Login(viewModel);*/
            
        }
       
        //异步登陆方法
        private async Task<ViewModelRegister> ViewRegister(ViewModelRegister viewModelRegister )
        {
            //将用户发送给api服务器，判断是否登录成功
            try
            {
                
                //Post提交数据时直接把要提交的数据格式要和Webapi的接收格式一致。不需要再转换了
                var response = await client.PostAsJsonAsync("http://localhost:60033/api/Register/PostRegister", viewModelRegister);
                response.EnsureSuccessStatusCode();
                ViewModelRegister msgdto = await response.Content.ReadAsAsync<ViewModelRegister>();//MsgDTO需和WebApi的MSGHelper这个类保持一致
                if (msgdto == null)
                {
                    //this.msglbl.Text = "网络错误";
                    return viewModelRegister;
                }

                /*if (msgdto.ViewUserAccount.ToLower() == "error")
                {
                    //this.msglbl.Text = msgdto.message;
                    return false;
                }
                if (msgdto.ViewUserAccount.ToLower() == "noright")
                {
                    //this.msglbl.Text = msgdto.message;
                    return false;
                }*/
                else
                {
                    

                    return viewModelRegister ;
                }
            }
            catch (HttpRequestException ex)
            {
                return viewModelRegister;
            }
            catch (System.FormatException)
            {
                return viewModelRegister;
            }
        }
        public static string HttpPost(string Url, string postDataStr, ref bool isSuccess)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
                //request.CookieContainer = cookie;
                Stream myRequestStream = request.GetRequestStream();
                StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
                myStreamWriter.Write(postDataStr);
                myStreamWriter.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //response.Cookies = cookie.GetCookies(response.ResponseUri);
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                return retString;
            }
            catch (Exception e)
            {
                isSuccess = false;
                Console.Write(e.Message);
                return e.Message;
            }
        }
        /*private async void GetProducts(object sender, RoutedEventArgs e)
        {
            try
            {
                btnGetProducts.IsEnabled = false;

                var response = await client.GetAsync("api/products");
                response.EnsureSuccessStatusCode(); // Throw on error code（有错误码时报出异常）.

                var products = await response.Content.ReadAsAsync<IEnumerable<Product>>();
                _products.CopyFrom(products);

            }
            catch (Newtonsoft.Json.JsonException jEx)
            {
                // This exception indicates a problem deserializing the request body.
                // 这个异常指明了一个解序列化请求体的问题。
                MessageBox.Show(jEx.Message);
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btnGetProducts.IsEnabled = true;
            }
        }*/
    }
}


