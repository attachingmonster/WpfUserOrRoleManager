using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUserOrRoleManager.Models;
using WpfUserOrRoleManager.Repositories;
using WpfUserOrRoleManager.ViewModels;

namespace WpfUserOrRoleManager.DAL
{
    public class UnitOfWork : IDisposable
    {
        private AccountContext context = new AccountContext();

        private GenericRepository<SysUser> sysUserRepository;
        private GenericRepository<SysRole> sysRoleRepository;
        private GenericRepository<SysUserRole> sysUserRoleRepository;
        

        public GenericRepository<SysUser> SysUserRepository
        {
            get
            {
                if (this.sysUserRepository == null)
                {
                    this.sysUserRepository = new GenericRepository<SysUser>(context);
                }
                return sysUserRepository;
            }
        }
        public GenericRepository<SysRole> SysRoleRepository
        {
            get
            {
                if (this.sysRoleRepository == null)
                {
                    this.sysRoleRepository = new GenericRepository<SysRole>(context);
                }
                return sysRoleRepository;
            }
        }
        public GenericRepository<SysUserRole> SysUserRoleRepository
        {
            get
            {
                if (this.sysUserRoleRepository == null)
                {
                    this.sysUserRoleRepository = new GenericRepository<SysUserRole>(context);
                }
                return sysUserRoleRepository;
            }
        }
        



        #region Save & Dispose
        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}


