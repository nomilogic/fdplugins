using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace CodeReformatter.Generators
{
    interface IGenerator
    {
        #region Properties

        String TabString { set; get; }
        String NewLine { set; get; }

        #endregion

        #region Members

        GeneratorReturnScope Parse(String source);

        #endregion
    }
}
