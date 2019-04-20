using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUserOrRoleManager.DAL;
using WpfUserOrRoleManager.Method;
using WpfUserOrRoleManager.Models;

namespace WpfUserOrRoleManager.BLL
{
    public class ChangePassword
    {
        public void Change(string Account,string OldPassword,string NewPassword)
        {
            try
            {
                var OldMD5 = CreateMD5.EncryptWithMD5(OldPassword);
                var NewMD5 = CreateMD5.EncryptWithMD5(NewPassword);  //加密
                var ChangeP = new ChangePswD();
                ChangeP.Change(Account, OldMD5, NewMD5);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
