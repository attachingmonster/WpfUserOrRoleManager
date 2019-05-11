using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUserOrRoleManager.Models
{
    public class SysRole
    {
        public int ID { get; set; }
        /// <summary>
        /// 角色名字
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 角色功能
        /// </summary>
        public string RoleDec { get; set; }
       
    }
}
