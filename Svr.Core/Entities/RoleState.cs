using System.ComponentModel;

namespace Svr.Core.Entities
{
    public enum RoleState : byte
    {
        [Description("Администратор")]
        Administrator,
        [Description("Администратор ОПФР")]
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once InconsistentNaming
        AdministratorOPFR,
        [Description("Администратор УПФР")]
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once InconsistentNaming
        AdministratorUPFR,
        [Description("Пользователь ОПФР")]
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once InconsistentNaming
        UserOPFR,
        [Description("Пользователь УПФР")]
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once InconsistentNaming
        UserUPFR,
        [Description("Пользователь")]
        User
    }
    
}
