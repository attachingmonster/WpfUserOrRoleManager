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
            var users = unitOfWork.SysUserRepository.Get();
            var user = users.Where(s => s.Account == this.tbxUserName.Text && s.Password == this.pbxPassword.Password).FirstOrDefault();
            if (user == null)
            {
                MessageBox.Show("用户和密码出错");
            }
            else
            {
                MessageBox.Show("登入成功");
            }

        }
    }
}
