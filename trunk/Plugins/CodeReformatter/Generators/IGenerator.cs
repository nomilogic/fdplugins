using System;
using System.Collections.Generic;
using System.Text;

namespace CodeReformatter.Generators
{
    interface IGenerator
    {
        #region Properties
        Settings SettingObject { set; get; }
        String TabString { set; get; }
        String NewLine { set; get; }
        #endregion

        StringBuilder GenerateCode(String source);
    }
}
