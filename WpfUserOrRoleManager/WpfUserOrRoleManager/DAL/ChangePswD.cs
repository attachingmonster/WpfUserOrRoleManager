using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUserOrRoleManager.Models;

namespace WpfUserOrRoleManager.DAL
{
    public class ChangePswD
    {
        public void Change(string Account,string OldPassword,string Password)
        {
            try
            {
                using (var db = new AccountContext())
                {
                    var u = db.SysUsers.Where(a => a.UserAccount.Equals(Account) && a.UserPassword.Equals(OldPassword)).FirstOrDefault();  //查此账号
                    if (u == null)
                    {
                        throw new Exception("用户名或密码错误！");
                    }
                    else
                    {
                        u.UserAccount = Account;
                        u.UserPassword = Password;
                        db.SaveChanges();        //修改
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
