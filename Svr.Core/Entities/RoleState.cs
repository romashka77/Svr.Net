using System.ComponentModel;

namespace Svr.Core.Entities
{
    public enum RoleState : byte
    {
        [Description("Администратор")]
        Administrator,
        [Description("Администратор ОПФР")]
        AdministratorOPFR,
        [Description("Администратор УПФР")]
        AdministratorUPFR,
        [Description("Пользователь ОПФР")]
        UserOPFR,
        [Description("Пользователь УПФР")]
        UserUPFR,
        [Description("Пользователь")]
        User
    }
    
}
