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

namespace CodeReformatter.Generators
{
    class AS2Generator : IGenerator
    {
        private Settings settingObject;
        private int currentTab;
        private StringBuilder CurrentBuffer;
        private String tabString;
        private String NEWLINE = "\n";
        private String TAB = "";
        private Boolean isInterface = false;

        /// <summary>
        /// Get the current Tab String
        /// </summary>
        public int CurrentTab
        {
            get { return this.currentTab; }
            set
            {
                this.currentTab = value;
                if (this.currentTab < 0) this.currentTab = 0;
                TAB = "";
                for (int i = 0; i < currentTab; i++)
                {
                    TAB += TabString;
                }
            }
        }

        public Settings SettingObject
        {
            set { this.settingObject = value; }
            get { return this.settingObject; }
        }

        public String TabString
        {
            get { return this.tabString; }
            set { this.tabString = value; }
        }

        public String NewLine
        {
            get { return this.NEWLINE; }
            set { this.NEWLINE = value; }
        }

        /// <summary>
        /// Parse the input string code using the antlr generated parser
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public StringBuilder GenerateCode(String source)
        {
            ASParser parser;

            CurrentBuffer = new StringBuilder();
            ANTLRStringStream input = new ANTLRStringStream(source);
            ASLexer lexer = new ASLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            parser = new ASParser(tokens);
            parser.SetInput(lexer, input);

            ParserRuleReturnScope result = parser.as2Program();

            CommonTree tree = (CommonTree)result.Tree;
            CurrentBuffer = new StringBuilder();
            ReformatNode(tree, true);
            return CurrentBuffer;
        }


        private void ReformatCode(ITree tree, Boolean endWithNewLine)
        {
            ReformatCode((CommonTree)tree, endWithNewLine);
        }

        private void ReformatCode(ITree tree)
        {
            ReformatCode((CommonTree)tree, false);
        }

        private void ReformatCode(CommonTree tree, Boolean endWithNewLine)
        {
            for (int i = 0; i < tree.ChildCount; i++)
            {
                CommonTree node = (CommonTree)tree.GetChild(i);
                ReformatNode(node, endWithNewLine);
            }
        }

        private void ReformatNode(ITree node, Boolean endWithNewLine)
        {
            ReformatNode((CommonTree)node, endWithNewLine);
        }

        private void ReformatNode(ITree node)
        {
            ReformatNode((CommonTree)node, false);
        }

        protected void ReformatNode(CommonTree node, Boolean endWithNewLine)
        {
            switch (node.Type)
            {
                case ASLexer.DOT:
                    CurrentBuffer.Append(fromDot(node));
                    break;

                case ASLexer.INTERFACE_DEF:
                    isInterface = true;
                    parseInterface(node);
                    break;

                case ASLexer.DO:
                    parseDoWhile(node);
                    break;

                case ASLexer.FUNC_DEF:
                    parseInlineFunction(node);
                    break;

                case ASLexer.ANNOTATIONS:
                    parseAnnotations(node);
                    break;

                case ASLexer.ANNOTATION_ASSIGN:
                    CurrentBuffer.Append(node.GetChild(0)+"="+node.GetChild(1));
                    break;

                case ASLexer.SWITCH:
                    CurrentBuffer.Append(node.Text);
                    CurrentBuffer.Append(settingObject.SPACE_BETWEEN_METHOD_CALL ? " (" : "(");
                    ReformatNode(node.GetChild(0));
                    CurrentBuffer.Append(settingObject.SPACE_BETWEEN_METHOD_CALL ? ") " : ")");
                    ReformatNode(node.GetChild(1));
                    break;

                case ASLexer.CASE:
                    CurrentBuffer.Append(node.Text + " ");
                    ReformatNode(node.GetChild(0));
                    CurrentBuffer.Append(":");
                    ReformatNode(node.GetChild(1));
                    break;

                case ASLexer.DEFAULT:
                    CurrentBuffer.Append(node.Text);
                    CurrentBuffer.Append(":");
                    ReformatNode(node.GetChild(0));
                    break;

                case ASLexer.SWITCH_STATEMENT_LIST:
                    parseSwitchStatementList(node);
                    break;

                case ASLexer.TYPEOF:
                    CurrentBuffer.Append("typeof");
                    if (settingObject.SPACE_BETWEEN_METHOD_CALL || (node.GetChild(0).Type != ASLexer.ENCPS_EXPR)) CurrentBuffer.Append(" ");
                    ReformatNode(node.GetChild(0));
                    if (settingObject.SPACE_BETWEEN_METHOD_CALL || (node.GetChild(0).Type != ASLexer.ENCPS_EXPR)) CurrentBuffer.Append(" ");
                    break;

                case ASLexer.INCLUDE_DIRECTIVE:
                    CurrentBuffer.Append(node.Text + " " + node.GetChild(0));
                    break;

                case ASLexer.BAND:
                    ReformatNode(node.GetChild(0));
                    if (settingObject.SPACE_BETWEEN_OPERATORS) CurrentBuffer.Append(" ");
                    CurrentBuffer.Append(node.Text);
                    if (settingObject.SPACE_BETWEEN_OPERATORS) CurrentBuffer.Append(" ");
                    ReformatNode(node.GetChild(1));
                    break;

                case ASLexer.COMMENT_LIST:
                    ReformatCode(node, endWithNewLine);
                    break;

                case ASLexer.COMMENT_ENTRY:
                    parseCommentEntry(node, true, endWithNewLine);
                    break;

                case ASLexer.SINGLELINE_COMMENT:
                    if(node.ChildCount > 0)
                        CurrentBuffer.Append(node.GetChild(0).Text.TrimEnd() + NEWLINE + TAB);
                    break;

                case ASLexer.MULTILINE_COMMENT:
                    parseMultilineComment(node, endWithNewLine);
                    break;

                case ASLexer.COMPILATION_UNIT:
                    parseCompilationUnit(node);
                    break;

                case ASLexer.IMPORT:
                    CurrentBuffer.Append("import " + fromIdentifier(node.GetChild(0)) + (endWithNewLine ? ";" + NEWLINE + TAB : ""));
                    break;

                case ASLexer.CLASS_DEF:
                    CurrentBuffer.Append(NEWLINE + TAB);
                    parseClass(node);
                    break;

                case ASLexer.VAR_DEF:               // class variable
                    parseVariableDefinition(node);
                    break;

                case ASLexer.VAR:                   // local variable
                    parseVariableDeclaration(node);
                    break;

                case ASLexer.DELETE:
                    CurrentBuffer.Append("delete ");
                    ReformatCode(node);
                    break;

                case ASLexer.ASSIGN:
                    parseAssign(node);
                    break;

                case ASLexer.ARRAY_ACC:
                    parseArrayAcc(node);
                    break;

                case ASLexer.UNARY_MINUS:
                    CurrentBuffer.Append("-");
                    if ( settingObject.SPACE_BETWEEN_OPERATORS ) CurrentBuffer.Append(" ");
                    ReformatNode(node.GetChild(0));
                    break;

                case ASLexer.UNARY_PLUS:
                    CurrentBuffer.Append("+");
                    if (settingObject.SPACE_BETWEEN_OPERATORS) CurrentBuffer.Append(" ");
                    ReformatNode(node.GetChild(0));
                    break;

                case ASLexer.STRING_LITERAL:
                case ASLexer.DECIMAL_LITERAL:
                case ASLexer.HEX_LITERAL:
                case ASLexer.FLOAT_LITERAL:
                case ASLexer.OCTAL_LITERAL:
                case ASLexer.IDENT:
                case ASLexer.FALSE:
                case ASLexer.TRUE:
                    CurrentBuffer.Append(node.Text);
                    break;

                case ASLexer.PROPERTY_OR_IDENTIFIER:
                    parsePropertyOrIdentifier(node);
                    break;

                case ASLexer.PLUS:
                case ASLexer.MINUS:
                case ASLexer.DIV:
                case ASLexer.STAR:
                case ASLexer.EQUAL:
                case ASLexer.LT:
                case ASLexer.GT:
                case ASLexer.GE:
                case ASLexer.LE:
                case ASLexer.LAND:
                case ASLexer.LOR:
                case ASLexer.NOT_EQUAL:
                case ASLexer.STRICT_NOT_EQUAL:
                case ASLexer.STRICT_EQUAL:
                case ASLexer.INSTANCEOF:
                case ASLexer.IS:
                case ASLexer.BSR:
                case ASLexer.SR:
                case ASLexer.MOD:
                    ReformatNode(node.GetChild(0));
                    CurrentBuffer.Append((settingObject.SPACE_BETWEEN_OPERATORS ? " " : "") + node.Text + (settingObject.SPACE_BETWEEN_OPERATORS ? " " : ""));
                    ReformatNode(node.GetChild(1));
                    break;

                case ASLexer.PLUS_ASSIGN:
                case ASLexer.MINUS_ASSIGN:
                case ASLexer.SL_ASSIGN:
                case ASLexer.SR_ASSIGN:
                case ASLexer.DIV_ASSIGN:
                case ASLexer.BAND_ASSIGN:
                case ASLexer.BOR_ASSIGN:
                case ASLexer.BSR_ASSIGN:
                case ASLexer.BXOR_ASSIGN:
                case ASLexer.LAND_ASSIGN:
                case ASLexer.LOR_ASSIGN:
                case ASLexer.MOD_ASSIGN:
                case ASLexer.STAR_ASSIGN:
                    if (settingObject.SPACE_BETWEEN_ASSIGN) CurrentBuffer.Append(" ");
                    CurrentBuffer.Append(node.Text);
                    if (settingObject.SPACE_BETWEEN_ASSIGN) CurrentBuffer.Append(" ");
                    break;

                case ASLexer.POST_DEC:
                case ASLexer.POST_INC:
                    ReformatNode(node.GetChild(0));
                    CurrentBuffer.Append(node.Text);
                    break;

                case ASLexer.PRE_DEC:
                case ASLexer.PRE_INC:
                    CurrentBuffer.Append(node.Text);
                    ReformatNode(node.GetChild(0));
                    break;

                case ASLexer.ARGUMENTS:
                    parseArguments(node);
                    break;

                case ASLexer.METHOD_CALL:
                    parseMethodCall(node);
                    break;

                case ASLexer.TYPE_SPEC:
                    parseTypeSpec(node);
                    break;

                case ASLexer.METHOD_DEF:
                    parseMethodDefinition(node);
                    break;

                case ASLexer.IMETHOD_DEF:
                    parseIMethodDefinition(node);
                    break;

                case ASLexer.PARAMS:
                    parseParams(node);
                    break;

                case ASLexer.PARAM:
                    parseParam(node);
                    break;

                case ASLexer.BLOCK:
                    parseBlock(node, endWithNewLine);
                    break;

                case ASLexer.RETURN:
                    CurrentBuffer.Append("return");
                    if (node.GetChild(0) != null)
                    {
                        CurrentBuffer.Append(" ");
                        ReformatNode(node.GetChild(0));
                    }
                    break;

                case ASLexer.BOR:
                    ReformatNode(node.GetChild(0));
                    CurrentBuffer.Append(settingObject.SPACE_BETWEEN_OPERATORS ? " | " : "|");
                    ReformatNode(node.GetChild(1));
                    break;

                case ASLexer.SL:
                    ReformatNode(node.GetChild(0));
                    CurrentBuffer.Append(settingObject.SPACE_BETWEEN_OPERATORS ? " << " : "<<");
                    ReformatNode(node.GetChild(1));
                    break;

                case ASLexer.QUESTION:
                    ReformatNode(node.GetChild(0));
                    CurrentBuffer.Append(settingObject.SPACE_BETWEEN_OPERATORS ? " ? " : "?");
                    ReformatNode(node.GetChild(1));
                    break;

                case ASLexer.COLON:
                    ReformatNode(node.GetChild(0));
                    CurrentBuffer.Append(settingObject.SPACE_BETWEEN_OPERATORS ? " : " : ":");
                    ReformatNode(node.GetChild(1));
                    break;

                case ASLexer.EXPR_STMNT:
                    ReformatNode(node.GetChild(0));
                    break;

                case ASLexer.ELIST:
                    parseExpressionList(node);
                    break;

                case ASLexer.ENCPS_EXPR:
                    CurrentBuffer.Append("(");
                    ReformatCode(node);
                    CurrentBuffer.Append(")");
                    break;

                case ASLexer.IF:
                    parseIfCondition(node);
                    break;

                case ASLexer.ELSE:
                    parseElseCondition(node);
                    break;

                case ASLexer.CONDITION:
                    parseCondition(node);
                    break;

                case ASLexer.WHILE:
                    parseWhile(node);
                    break;

                case ASLexer.FOR:
                    parseFor(node);
                    break;

                case ASLexer.FOR_IN:
                    parseForIn(node);
                    break;

                case ASLexer.FOR_INIT:
                    parseForInit(node);
                    CurrentBuffer.Append("; ");
                    break;

                case ASLexer.FOR_CONDITION:
                    parseForCondition(node);
                    CurrentBuffer.Append("; ");
                    break;

                case ASLexer.FOR_ITERATOR:
                    ReformatCode(node);
                    break;

                case ASLexer.BREAK:
                case ASLexer.CONTINUE:
                    CurrentBuffer.Append(node.Text);
                    break;

                case ASLexer.NULL:
                case ASLexer.UNDEFINED:
                    CurrentBuffer.Append(node.Text);
                    break;

                case ASLexer.COMMA:
                    CurrentBuffer.Append(node.Text + " ");
                    break;

                case ASLexer.NEW:
                    CurrentBuffer.Append("new ");
                    ReformatCode(node);
                    break;

                case ASLexer.LNOT:
                    CurrentBuffer.Append("!");
                    ReformatCode(node);
                    break;

                case ASLexer.ARRAY_LITERAL:
                    parseArrayLiteral(node);
                    break;

                case ASLexer.OBJECT_LITERAL:
                    parseObjectLiteral(node);
                    break;

                case ASLexer.ELEMENT:
                    parseElement(node);
                    break;

                case ASLexer.OBJECT_FIELD:
                    parseObjectField(node);
                    break;

                case ASLexer.PACKAGE:
                    parsePackage(node);
                    break;

                default:
                    Debug.WriteLine("ReformatCode: " + node.Type + ", " + node.Text + " - " + node.ToStringTree());
                    break;
            }
        }

        private void parseCommentEntry(CommonTree node, Boolean startWithNewLine, Boolean endWithNewLine)
        {
            Console.WriteLine("parseCommentEntry: " + endWithNewLine);
            CommonTree nextNode;
            for (int i = 0; i < node.ChildCount; i++)
            {
                nextNode = (CommonTree)node.GetChild(i);
                if ((i == 0 && nextNode.Type == ASLexer.SINGLELINE_COMMENT) && startWithNewLine)
                    CurrentBuffer.Append(NEWLINE + TAB);

                if (nextNode.Type != ASLexer.COMMENT_ENTRY)
                {
                    ReformatNode(nextNode, endWithNewLine);
                }
                else
                {
                    parseCommentEntry(nextNode, false, endWithNewLine);
                }
            }
        }

        protected void parseElseCondition(CommonTree node)
        {
            CommonTree nextNode;
            if (settingObject.NEWLINE_BEFORE_ELSE)
                CurrentBuffer.Append(NEWLINE + TAB + "else ");
            else
                CurrentBuffer.Append(" else ");

            nextNode = (CommonTree)node.DeleteChild(0);

            if (nextNode.Type == ASLexer.COMMENT_LIST)
            {
                CurrentBuffer.Append(NEWLINE + TAB);
                ReformatCode(nextNode, false);
                nextNode = (CommonTree)node.DeleteChild(0);
            }

            if (nextNode.Type == ASLexer.BLOCK || nextNode.Type == ASLexer.IF)
            {
                ReformatNode(nextNode, settingObject.NEWLINE_AFTER_CONDITION);
            }
            else
            {
                AddBlock(nextNode, settingObject.NEWLINE_AFTER_CONDITION);
            }
        }

        protected void parseInterface(CommonTree node)
        {
            String st = NEWLINE + TAB;
            if (node.GetChild(0).ChildCount > 0)
            {
                st = fromModifiers(node.GetChild(0)) + " ";
            }
            st += "interface " + fromIdentifier(node.GetChild(1));

            if (node.GetChild(2).Type == ASLexer.EXTENDS)
            {
                st += " extends " + fromIdentifier(node.GetChild(2).GetChild(0));
                if (node.GetChild(3).Type == ASLexer.IMPLEMENTS)
                {
                    st += " implements " + fromImplements(node.GetChild(3));
                }
            }
            else if (node.GetChild(2).Type == ASLexer.IMPLEMENTS)
            {
                st += " implements " + fromImplements(node.GetChild(2));
            }
            CurrentBuffer.Append(st);

            if (node.GetChild(node.ChildCount - 1).Type == ASLexer.TYPE_BLOCK)
                parseTypeBlock((CommonTree)node.GetChild(node.ChildCount - 1));
        }

        protected void parseDoWhile(CommonTree node)
        {
            CommonTree nextNode = null;
            ITree commentNode   = null;

            CurrentBuffer.Append(node.Text);

            nextNode = (CommonTree)node.DeleteChild(0);

            // check if comment
            if (nextNode.Type == ASLexer.COMMENT_LIST)
            {
                commentNode = nextNode.DupTree();
                nextNode = (CommonTree)node.DeleteChild(0);
            }

            if (commentNode != null)
            {
                CurrentBuffer.Append(NEWLINE + TAB);
                ReformatCode(commentNode, false);
            }

            if (nextNode.Type == ASLexer.BLOCK)
            {
                ReformatNode(nextNode, settingObject.NEWLINE_AFTER_CONDITION);
            }
            else
            {
                AddBlock(nextNode, settingObject.NEWLINE_AFTER_CONDITION);
            }

            CurrentBuffer.Append(" while");
            CurrentBuffer.Append(settingObject.SPACE_BETWEEN_METHOD_CALL ? " (" : "(");
            ReformatNode(node.GetChild(0));
            CurrentBuffer.Append(")");
        }

        protected void parseInlineFunction(CommonTree node)
        {
            CommonTree nextNode = (CommonTree)node.DeleteChild(0);
            CurrentBuffer.Append("function");

            if (nextNode.Type == ASLexer.IDENT)
            {
                CurrentBuffer.Append(" " + nextNode.Text);
                nextNode = (CommonTree)node.DeleteChild(0);
            }

            CurrentBuffer.Append((settingObject.SPACE_BETWEEN_METHOD_CALL ? " " : "") + "(");
            ReformatNode(nextNode); // params
            CurrentBuffer.Append(")");

            nextNode = (CommonTree)node.DeleteChild(0);

            if (nextNode.Type == ASLexer.TYPE_SPEC)
            {
                ReformatNode(nextNode);
                nextNode = (CommonTree)node.DeleteChild(0);
            }

            if (nextNode != null)
            {
                if (settingObject.SPACE_BETWEEN_METHOD_CALL) CurrentBuffer.Append(" ");
                ReformatNode(nextNode, settingObject.NEWLINE_AFTER_METHOD);
            }

        }


        protected void parseAnnotations(CommonTree node)
        {
            CommonTree aParams;
            for (int i = 0; i < node.ChildCount; i++)
            {
                CurrentBuffer.Append(NEWLINE + TAB + "[");
                CurrentBuffer.Append(node.GetChild(i).GetChild(0).Text);
                if (node.GetChild(i).ChildCount > 1)
                {
                    CurrentBuffer.Append("(");
                    aParams = (CommonTree)node.GetChild(i).GetChild(1);
                    for (int k = 0; k < aParams.ChildCount; k++)
                    {
                        ReformatNode(aParams.GetChild(k));
                        if (k < aParams.ChildCount - 1)
                        {
                            CurrentBuffer.Append(",");
                        }
                    }
                    CurrentBuffer.Append(")");
                }
                CurrentBuffer.Append("]");
            }
        }

        protected void parseSwitchStatementList(CommonTree node)
        {
            CurrentTab++;
            for (int i = 0; i < node.ChildCount; i++)
            {
                CurrentBuffer.Append(NEWLINE + TAB);
                ReformatNode(node.GetChild(i), true);
                AppendSemiColon(node.GetChild(i));
            }
            CurrentTab--;
        }

        protected void parseMultilineComment(CommonTree node, Boolean addNewLine)
        {
            String comment = node.GetChild(0).Text;
            Regex reg = new Regex("[\n\r]+", RegexOptions.Multiline);
            string[] comments = reg.Split(comment);
            int i = 0;
            foreach (string line in comments)
            {
                CurrentBuffer.Append(NEWLINE + TAB+ (i > 0 ? " " : "") + line.TrimStart());
                i++;
            }

            if (addNewLine) CurrentBuffer.Append(NEWLINE + TAB);
        }

        /// <summary>
        /// It can be either a simple identifier or a property
        /// </summary>
        /// <param name="node"></param>
        protected void parsePropertyOrIdentifier(CommonTree node)
        {
            if (node.GetChild(0).Type == ASLexer.IDENT)
            {
                CurrentBuffer.Append(fromIdentifier(node));
            }
            else
            {
                String result = fromPropertyOrIdentifier(node);
                //ReformatCode(node);
                CurrentBuffer.Append(result);
            }
        }

        /// <summary>
        /// Main unit
        /// </summary>
        /// <param name="node"></param>
        protected void parseCompilationUnit(CommonTree node)
        {
            for (int i = 0; i < node.ChildCount; i++)
            {
                ReformatNode(node.GetChild(i), true);
            }
        }

        protected void parsePackage(CommonTree node)
        {
            CurrentBuffer.Append(NEWLINE + TAB + node.Text);
            if (node.ChildCount > 0)
            {
                if (node.GetChild(0).Type == ASLexer.IDENTIFIER)
                {
                    CurrentBuffer.Append(" " + fromIdentifier(node.GetChild(0)) + " ");
                    if (node.ChildCount > 1)
                    {
                        node.DeleteChild(0);
                        ReformatCode(node, settingObject.NEWLINE_AFTER_CLASS);
                    }
                }
                else if (node.GetChild(0).Type == ASLexer.BLOCK)
                {
                    node.DeleteChild(0);
                    ReformatCode(node, settingObject.NEWLINE_AFTER_CLASS);
                }

            }
        }

        protected void parseElement(CommonTree node)
        {
            ReformatCode(node);
        }

        /// <summary>
        /// [] Array initializer
        /// </summary>
        /// <param name="node"></param>
        protected void parseArrayLiteral(CommonTree node)
        {
            CurrentBuffer.Append("[");

            for (int i = 0; i < node.ChildCount; i++)
            {
                ReformatNode(node.GetChild(i));
            }

            CurrentBuffer.Append("]");
        }

        /// <summary>
        /// Object initialization '{}'
        /// </summary>
        /// <param name="node"></param>
        protected void parseObjectLiteral(CommonTree node)
        {
            if (node.ChildCount > 0)
                CurrentBuffer.Append(NEWLINE + TAB);

            CurrentBuffer.Append("{");
            CurrentTab++;

            for (int i = 0; i < node.ChildCount; i++)
            {
                CurrentBuffer.Append(NEWLINE + TAB);
                ReformatNode(node.GetChild(i));

                if (i < node.ChildCount - 1)
                    CurrentBuffer.Append(",");
            }

            CurrentTab--;
            CurrentBuffer.Append((node.ChildCount > 0 ? NEWLINE + TAB : "") + "}");
        }

        protected void parseObjectField(CommonTree node)
        {
            ReformatNode(node.GetChild(0));
            CurrentBuffer.Append(" : ");
            ReformatNode(node.GetChild(1));
        }

        /// <summary>
        /// Generic code Block
        /// </summary>
        /// <param name="node"></param>
        /// <param name="addNewline"></param>
        protected void parseBlock(CommonTree node, Boolean addNewline)
        {
            if (addNewline)
                CurrentBuffer.Append(NEWLINE + TAB);

            CurrentBuffer.Append("{");
            CurrentTab++;

            for (int i = 0; i < node.ChildCount; i++)
            {
                CurrentBuffer.Append(NEWLINE + TAB);
                ReformatNode(node.GetChild(i), true);
                AppendSemiColon(node.GetChild(i));
            }

            CurrentTab--;
            CurrentBuffer.Append(NEWLINE + TAB + "}");
        }

        protected void AddBlock(ITree node, Boolean addNewLine)
        {
            if (addNewLine)
                CurrentBuffer.Append(NEWLINE + TAB);

            CurrentBuffer.Append("{");
            CurrentTab++;

            CurrentBuffer.Append(NEWLINE + TAB);
            ReformatNode(node, true);
            AppendSemiColon(node);
            CurrentTab--;
            CurrentBuffer.Append(NEWLINE + TAB + "}");
        }

        protected void AddBlock(ITree[] nodes, Boolean addNewLine)
        {
            if (addNewLine)
                CurrentBuffer.Append(NEWLINE + TAB);
            CurrentBuffer.Append("{");
            CurrentTab++;
            foreach (ITree node in nodes)
            {
                CurrentBuffer.Append(NEWLINE + TAB);
                ReformatNode(node, false);
                AppendSemiColon(node);
            }
            CurrentTab--;
            CurrentBuffer.Append(NEWLINE + TAB + "}");
        }

        protected void parseExpressionList(CommonTree node)
        {
            for (int i = 0; i < node.ChildCount; i++)
            {
                ReformatNode(node.GetChild(i));
            }
        }

        protected void parseForCondition(CommonTree node)
        {
            ReformatNode(node.GetChild(0));
        }

        protected void parseForInit(CommonTree node)
        {
            ReformatNode(node.GetChild(0));
        }

        /// <summary>
        /// For..in loop
        /// </summary>
        /// <param name="node"></param>
        protected void parseForIn(CommonTree node)
        {
            CurrentBuffer.Append("for");

            if (settingObject.SPACE_BETWEEN_METHOD_CALL)
                CurrentBuffer.Append(" ");

            CurrentBuffer.Append(settingObject.SPACE_BETWEEN_METHOD_CALL ? " (" : "(");
            ReformatNode(node.GetChild(0));
            CurrentBuffer.Append(" in ");
            ReformatNode(node.GetChild(1));
            CurrentBuffer.Append(settingObject.SPACE_BETWEEN_METHOD_CALL ? ") " : ")");

            if (node.ChildCount > 2)
            {
                if (node.GetChild(2).Type == ASLexer.BLOCK)
                    ReformatNode(node.GetChild(2), settingObject.NEWLINE_AFTER_CONDITION);
                else
                    AddBlock(node.GetChild(2), settingObject.NEWLINE_AFTER_CONDITION);
            }
        }

        protected void parseFor(CommonTree node)
        {
            CurrentBuffer.Append("for");
            CurrentBuffer.Append(settingObject.SPACE_BETWEEN_METHOD_CALL ? " (" : "(");
            for (int i = 0; i < 3; i++)
            {
                CommonTree newNode = (CommonTree)node.GetChild(i);
                if (newNode.Type != ASLexer.BLOCK)
                {
                    ReformatNode(newNode);
                }
            }
            CurrentBuffer.Append(settingObject.SPACE_BETWEEN_METHOD_CALL ? ") " : ")");

            if (node.ChildCount > 3)
            {
                if (node.GetChild(3).Type == ASLexer.BLOCK)
                    ReformatNode(node.GetChild(3), settingObject.NEWLINE_AFTER_CONDITION);
                else
                    AddBlock(node.GetChild(3), settingObject.NEWLINE_AFTER_CONDITION);
            }

        }

        protected void parseWhile(CommonTree node)
        {
            CurrentBuffer.Append("while");
            CurrentBuffer.Append(settingObject.SPACE_BETWEEN_METHOD_CALL ? " (" : "(");
            ReformatNode(node.GetChild(0));
            CurrentBuffer.Append(settingObject.SPACE_BETWEEN_METHOD_CALL ? ") " : ")");
            ReformatNode(node.GetChild(1), settingObject.NEWLINE_AFTER_CONDITION);
        }

        protected void parseCondition(CommonTree node)
        {
            ReformatNode(node.GetChild(0));
        }

        protected void parseIfCondition(CommonTree node)
        {
            CommonTree nextNode;
            ITree commentNode = null;
            CurrentBuffer.Append("if");
            CurrentBuffer.Append(settingObject.SPACE_BETWEEN_METHOD_CALL ? " (" : "(");
            ReformatNode(node.DeleteChild(0));
            CurrentBuffer.Append(settingObject.SPACE_BETWEEN_METHOD_CALL ? ") " : ")");

            nextNode = (CommonTree)node.DeleteChild(0);

            // check if comment
            if (nextNode.Type == ASLexer.COMMENT_LIST)
            {
                commentNode = nextNode.DupTree();
                nextNode = (CommonTree)node.DeleteChild(0);
            }

            if (commentNode != null)
            {
                CurrentBuffer.Append(NEWLINE + TAB);
                ReformatCode(commentNode, false);
            }

            if (nextNode.Type == ASLexer.BLOCK)
            {
                ReformatNode(nextNode, settingObject.NEWLINE_AFTER_CONDITION);
            }
            else
            {
                AddBlock(nextNode, settingObject.NEWLINE_AFTER_CONDITION);
            }

            if (node.ChildCount > 0)
            {
                nextNode = (CommonTree)node.DeleteChild(0);
                if (nextNode.Type == ASLexer.COMMENT_LIST)
                {
                    CurrentBuffer.Append(NEWLINE + TAB);
                    ReformatCode(nextNode, false);
                }

                if(node.ChildCount > 0)
               ReformatNode(node.GetChild(0));
        }
        }

        protected void parseVariableDeclaration(CommonTree node)
        {
            CurrentBuffer.Append(node.Text + " ");
            CurrentBuffer.Append(node.GetChild(0).Text);
            ReformatCode(node.GetChild(0));
        }

        protected void parseParam(CommonTree node)
        {
            ReformatNode(node.GetChild(0));
            if (node.ChildCount > 1)
                ReformatNode(node.GetChild(1));
        }

        protected void parseParams(CommonTree node)
        {
            if (node.ChildCount > 0)
            {
                for (int i = 0; i < node.ChildCount; i++)
                {
                    ReformatNode(node.GetChild(i));
                    if (i < node.ChildCount - 1)
                        CurrentBuffer.Append("," + (settingObject.SPACE_BETWEEN_ARGUMENTS ? " " : ""));
                }
            }
        }

        protected void parseMethodDefinition(CommonTree node)
        {
            CommonTree nextNode = (CommonTree)node.DeleteChild(0);
            CommonTree commentNode = null;
            CurrentBuffer.Append(NEWLINE + TAB);
            if(nextNode.ChildCount > 0)
                CurrentBuffer.Append(fromModifiers(nextNode) + " ");

            CurrentBuffer.Append("function");
            nextNode = (CommonTree)node.DeleteChild(0);
            
            if (nextNode.ChildCount > 0)
                CurrentBuffer.Append(" " + nextNode.GetChild(0).Text);

            nextNode = (CommonTree)node.DeleteChild(0);

            CurrentBuffer.Append(" ");
            ReformatNode(nextNode);
            CurrentBuffer.Append((settingObject.SPACE_BETWEEN_METHOD_CALL ? " " : "") + "(");
            nextNode = (CommonTree)node.DeleteChild(0);
            ReformatNode(nextNode);
            CurrentBuffer.Append(")");

            if (node.ChildCount > 0)
            {
                nextNode = (CommonTree)node.DeleteChild(0);
                if (nextNode.Type == ASLexer.COMMENT_LIST)
                {
                    commentNode = (CommonTree)nextNode.DupTree();
                    nextNode = (CommonTree)node.DeleteChild(0);
                    //CurrentBuffer.Append(NEWLINE + TAB);
                    ReformatCode(commentNode, false);
                }

                if (nextNode.Type == ASLexer.TYPE_SPEC)
                {
                    ReformatNode(nextNode);
                    nextNode = (CommonTree)node.DeleteChild(0);
                }

                if (nextNode.Type == ASLexer.COMMENT_LIST)
                {
                    commentNode = (CommonTree)nextNode.DupTree();
                    //CurrentBuffer.Append(NEWLINE + TAB);
                    ReformatCode(commentNode, false);

                    if (node.ChildCount > 0)
                        nextNode = (CommonTree)node.DeleteChild(0);
                else
                        nextNode = null;
                }

                if (nextNode != null)
                {
                    if (settingObject.SPACE_BETWEEN_METHOD_CALL) CurrentBuffer.Append(" ");
                    ReformatNode(nextNode, settingObject.NEWLINE_AFTER_METHOD);
                }
            }
                }

        /// <summary>
        /// Interface method definition. Interface method cannot
        /// have the block node
        /// </summary>
        /// <param name="node"></param>
        protected void parseIMethodDefinition(CommonTree node)
        {
            CommonTree nextNode = (CommonTree)node.DeleteChild(0);
            CommonTree commentNode = null;
            //CurrentBuffer.Append(NEWLINE + TAB);
            if (nextNode.ChildCount > 0)
                CurrentBuffer.Append(fromModifiers(nextNode) + " ");

            CurrentBuffer.Append("function");
            nextNode = (CommonTree)node.DeleteChild(0);

            if (nextNode.ChildCount > 0)
                CurrentBuffer.Append(" " + nextNode.GetChild(0).Text);

            nextNode = (CommonTree)node.DeleteChild(0);

            CurrentBuffer.Append(" ");
            ReformatNode(nextNode);
            CurrentBuffer.Append((settingObject.SPACE_BETWEEN_METHOD_CALL ? " " : "") + "(");
            nextNode = (CommonTree)node.DeleteChild(0);
            ReformatNode(nextNode);
            CurrentBuffer.Append(")");

            if (node.ChildCount > 0)
            {
                nextNode = (CommonTree)node.DeleteChild(0);
                if (nextNode.Type == ASLexer.COMMENT_LIST)
                {
                    commentNode = (CommonTree)nextNode.DupTree();
                    nextNode = (CommonTree)node.DeleteChild(0);
                    //CurrentBuffer.Append(NEWLINE + TAB);
                    ReformatCode(commentNode, false);
                }

                if (nextNode.Type == ASLexer.TYPE_SPEC)
                {
                    ReformatNode(nextNode);
                    if (node.ChildCount > 0)
                        nextNode = (CommonTree)node.DeleteChild(0);
                    else
                        nextNode = null;
                }

                if (nextNode != null && nextNode.Type == ASLexer.COMMENT_LIST)
                {
                    ReformatCode(nextNode, false);
                }
            }
        }

        protected void parseArguments(CommonTree node)
        {
            CurrentBuffer.Append("(");
            if (node.ChildCount > 0)
            {
                if (node.GetChild(0).ChildCount > 0)
                {
                    for (int i = 0; i < node.GetChild(0).ChildCount; i++)
                    {
                        ReformatNode(node.GetChild(0).GetChild(i));
                    }
                }
            }
            CurrentBuffer.Append(")");
        }

        protected void parseArrayAcc(CommonTree node)
        {
            ReformatNode(node.GetChild(0));
            CurrentBuffer.Append("[");
            ReformatNode(node.GetChild(1));
            CurrentBuffer.Append("]");
        }

        protected void parseMethodCall(CommonTree node)
        {
            ReformatNode(node.GetChild(0));
            CurrentBuffer.Append(settingObject.SPACE_BETWEEN_METHOD_CALL ? " " : "");
            ReformatNode(node.GetChild(1));
        }

        protected void parseAssign(CommonTree node)
        {
            if (settingObject.SPACE_BETWEEN_ASSIGN) CurrentBuffer.Append(" ");
            CurrentBuffer.Append("=");
            if (settingObject.SPACE_BETWEEN_ASSIGN) CurrentBuffer.Append(" ");
            ReformatCode(node);
        }

        protected void parseTypeSpec(CommonTree node)
        {
            CurrentBuffer.Append((settingObject.SPACE_BETWEEN_TYPE ? " " : "") + ":" + (settingObject.SPACE_BETWEEN_TYPE ? " " : ""));
            CurrentBuffer.Append(fromIdentifier(node.GetChild(0)));
        }

        protected void parseVariableDefinition(CommonTree node)
        {
            if(node.GetChild(0).ChildCount > 0)
                CurrentBuffer.Append( fromModifiers( node.GetChild(0) ) + " ");

            CurrentBuffer.Append(node.GetChild(1).Text + " ");
            CurrentBuffer.Append(node.GetChild(2).Text);
            ReformatCode(node.GetChild(2));
        }

        /// <summary>
        /// Class definition
        /// </summary>
        /// <param name="tree"></param>
        protected void parseClass(CommonTree tree)
        {
            String st = NEWLINE + TAB;
            if (tree.GetChild(0).ChildCount > 0)
            {
                st = fromModifiers(tree.GetChild(0)) + " ";
            }
            st += "class " + fromIdentifier(tree.GetChild(1));

            if (tree.GetChild(2).Type == ASLexer.EXTENDS)
            {
                st += " extends " + fromIdentifier(tree.GetChild(2).GetChild(0));
                if (tree.GetChild(3).Type == ASLexer.IMPLEMENTS)
                {
                    st += " implements " + fromImplements(tree.GetChild(3));
                }
            }
            else if (tree.GetChild(2).Type == ASLexer.IMPLEMENTS)
            {
                st += " implements " + fromImplements(tree.GetChild(2));
            }
            CurrentBuffer.Append(st);

            if (tree.GetChild(tree.ChildCount - 1).Type == ASLexer.TYPE_BLOCK)
                parseTypeBlock( (CommonTree)tree.GetChild(tree.ChildCount - 1));
        }

        /// <summary>
        /// Class Body
        /// </summary>
        /// <param name="tree"></param>
        protected void parseTypeBlock(CommonTree tree)
        {
            CurrentBuffer.Append((settingObject.NEWLINE_AFTER_CLASS ? NEWLINE : " ") + TAB + "{");
            CurrentTab++;

            for (int i = 0; i < tree.ChildCount; i++)
            {
                CurrentBuffer.Append(NEWLINE + TAB);
                ReformatNode(tree.GetChild(i), true);
                AppendSemiColon(tree.GetChild(i));
            }

            CurrentTab--;
            CurrentBuffer.Append(NEWLINE + TAB + "}");
        }

        /// <summary>
        /// Append the SEMI ";" at the end of a line
        /// </summary>
        /// <param name="node"></param>
        protected void AppendSemiColon(ITree node)
        {
            if (
                node.Type != ASLexer.IF
                && node.Type != ASLexer.COMMENT_LIST
                && node.Type != ASLexer.COMMENT_ENTRY
                && node.Type != ASLexer.SINGLELINE_COMMENT
                && node.Type != ASLexer.MULTILINE_COMMENT
                && node.Type != ASLexer.FOR
                && node.Type != ASLexer.FOR_IN
                && node.Type != ASLexer.WHILE
                && node.Type != ASLexer.FOR_EACH
                && node.Type != ASLexer.METHOD_DEF
                && node.Type != ASLexer.CASE
                && node.Type != ASLexer.DEFAULT
                && node.Type != ASLexer.ANNOTATIONS
                && node.Type != ASLexer.INCLUDE_DIRECTIVE
                && node.Type != ASLexer.IMETHOD_DEF)
                CurrentBuffer.Append(";");
        }

        /// <summary>
        /// Return a dot separated identifier
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        protected String fromIdentifier(ITree tree)
        {
            tree = (CommonTree)tree;

            if (tree.Type == ASLexer.IDENT) return tree.Text;

            string[] buff = new string[tree.ChildCount];
            for (int i = 0; i < tree.ChildCount; i++)
            {
                buff[i] = tree.GetChild(i).Text;
            }
            return String.Join(".", buff);
        }

        /// <summary>
        /// Compute the MODIFIER node and return a string representation
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        protected String fromModifiers(ITree tree)
        {
            tree = (CommonTree)tree;
            string[] buff = new string[tree.ChildCount];
            for (int i = 0; i < tree.ChildCount; i++)
            {
                buff[i] = tree.GetChild(i).Text;
            }
            return String.Join(" ", buff);
        }

        /// <summary>
        /// Compute the Implements node and return a formatted string
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        protected String fromImplements(ITree tree)
        {
            tree = (CommonTree)tree;
            string[] st = new string[tree.ChildCount];
            for (int i = 0; i < tree.ChildCount; i++)
            {
                st[i] = fromIdentifier(tree.GetChild(i));
            }
            return String.Join(", ", st);
        }


        /// <summary>
        /// Reformat a DOT identifier
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected String fromDot(CommonTree node)
        {
            String text = "";
            if (node.GetChild(0).ChildCount > 0)
            {
                text += fromDot((CommonTree)node.GetChild(0));
                text += node.GetChild(0).Text + node.GetChild(1).Text;
            }
            else
            {
                text = node.GetChild(0).Text + node.Text + node.GetChild(1).Text + text;
            }
            return text;
        }


        protected String fromPropertyOrIdentifier(CommonTree node)
        {
            String text = "";
            if (node.GetChild(0).Type == ASLexer.PROPERTY_OR_IDENTIFIER)
            {
                text += fromPropertyOrIdentifier((CommonTree)node.GetChild(0));
                text += "." + node.GetChild(1).Text;
            }
            else
            {
                text = node.GetChild(0).Text + "." + node.GetChild(1).Text  + text;
            }
            return text;
        }
    }
}
