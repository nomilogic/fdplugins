using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime.Tree;
using CodeReformatter.Generators.Core;
using Antlr.Runtime;
using System.IO;

using CodeReformatter;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections;

namespace CodeReformatter.Generators
{
    class AS2Generator : IGenerator
    {
        #region Properties

        private CodeReformatter.Generators.ReformatOptions options;
        private int currentTab;
        private StringBuilder CurrentBuffer;
        private String tabString = "\t";
        private String newline = "\n";
        private String tab = "";
        private StringBuilder buffer;
        private ASParser parser;
        private ASLexer lexer;

        public ASParser Parser
        {
            get { return this.parser; }
        }

        public ASLexer Lexer
        {
            get { return this.lexer; }
        }

        /// <summary>
        /// Get/Set the Tab Width
        /// </summary>
        public int CurrentTab
        {
            get { return this.currentTab; }
            set
            {
                this.currentTab = value;
                if (this.currentTab < 0) this.currentTab = 0;
                tab = "";
                for (int i = 0; i < currentTab; i++)
                {
                    tab += TabString;
                }
            }
        }

        public String TabString
        {
            get { return this.tabString; }
            set { this.tabString = value; }
        }

        public String NewLine
        {
            get { return this.newline; }
            set { this.newline = value; }
        }

        #endregion

        #region Implemented Members

        public AS2Generator()
        {
            options = new ReformatOptions();
        }

        public AS2Generator(ReformatOptions opt)
        {
            options = opt;
        }

        /// <summary>
        /// Parse and validate the input string.
        /// Throws RecognitionException
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public GeneratorReturnScope Parse(string source)
        {
            buffer = new StringBuilder();
            ANTLRStringStream input = new ANTLRStringStream(source);
            lexer = new ASLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            parser  = new ASParser(tokens, options);
            parser.NewLine   = NewLine;
            parser.TabString = TabString;
            parser.SetInput(lexer, input);
            lexer.SetInput(parser);
            ParserRuleReturnScope result = parser.compilationUnit(buffer);
            return new GeneratorReturnScope(result, buffer);
        }
        #endregion
    }

    class GeneratorReturnScope
    {
        public ParserRuleReturnScope Result;
        public StringBuilder Buffer;

        public GeneratorReturnScope(ParserRuleReturnScope result, StringBuilder buffer)
        {
            this.Result = result;
            this.Buffer = buffer;
        }
    }
}
