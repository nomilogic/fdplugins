using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using CodeReformatter.Generators.Core;

namespace CodeReformatter.Generators
{
    interface IGenerator
    {
        #region Properties

        String TabString { set; get; }
        String NewLine { set; get; }
        ASParser Parser { get; }
        ASLexer Lexer { get; }

        #endregion

        #region Members

        GeneratorReturnScope Parse(String source);

        #endregion
    }
}
