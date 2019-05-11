using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUserOrRoleManager.Models
{
    public class ViewModel
    {
        public int ID { get; set; }
        /// <summary>
        /// 显示用户账号
        /// </summary>
        public string ViewUserAccount { get; set; }
        /// <summary>
        /// 显示用户ID
        /// </summary>
        public int ViewUserID { get; set; }
        /// <summary>
        /// 显示用户角色
        /// </summary>
        public string ViewRoleName { get; set; }
        /// <summary>
        /// 显示用户角色功能
        /// </summary>
        public string ViewRoleDec { get; set; }
    }
}
