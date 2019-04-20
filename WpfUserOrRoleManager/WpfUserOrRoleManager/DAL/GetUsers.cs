using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUserOrRoleManager.Models;

namespace WpfUserOrRoleManager.DAL
{
    public class GetUsers
    {
        public SysUser GetUser(string Account, string Password)
        {
            try
            {
                using (var db = new AccountContext())  //数据库上下文联动
                {
                    var u = db.SysUsers.Where(a => a.UserAccount.Equals(Account) && a.UserPassword.Equals(Password)).FirstOrDefault(); //判断账户密码是否相等，取相等的，否则返回空值
                    return u;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
