using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUserOrRoleManager.DAL;
using WpfUserOrRoleManager.Method;

namespace WpfUserOrRoleManager.BLL
{
    public class Login
    {
        public void login(string Account, string Password)
        {
            try
            {
                var PasswordMD5 = CreateMD5.EncryptWithMD5(Password);  //加密
                var Get = new GetUsers();
                var u = Get.GetUser(Account, PasswordMD5);
                if(u==null)
                {
                    throw new Exception("用户名或密码错误！");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
