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

        /// <summary>
        /// 登陆按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var users = unitOfWork.SysUserRepository.Get();//获得用户表数据
            var user = users.Where(s => s.Account == this.tbxUserName.Text && s.Password == this.pbxPassword.Password).FirstOrDefault();//与账号密码一致的用户
            if (user != null)
            {
                MessageBox.Show("登入成功!");
            }
            else
            {
                MessageBox.Show("用户和密码出错!");
            }

        }
    }
}
