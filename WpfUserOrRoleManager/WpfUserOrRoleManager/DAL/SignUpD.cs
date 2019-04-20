using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUserOrRoleManager.Models;

namespace WpfUserOrRoleManager.DAL
{
    public class SignUpD
    {
        public void SignUP(string Account, string Password)
        {
            try
            {
                using (var db = new AccountContext())
                {
                    var u = db.SysUsers.Where(a => a.UserAccount.Equals(Account)).FirstOrDefault();  //查账户
                    if (u != null)
                    {
                        throw new Exception("用户名已存在！");
                    }
                    else
                    {
                        db.SysUsers.Add(new SysUser { UserAccount = Account, UserPassword = Password });  //增加新账户
                        db.SaveChanges();
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
