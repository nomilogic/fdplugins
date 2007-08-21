using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime.Tree;
using CodeReformatter.Generators.Core;
using Antlr.Runtime;
using System.IO;

using CodeReformatter;
using System.Diagnostics;

namespace CodeReformatter.Generators
{
    class Generator
    {
        #region Properties

        static public int currentTab = 0;
        static public StringBuilder CurrentBuffer;
        static protected String TAB = "";
        static protected Settings settingObject;

        /// <summary>
        /// Get the current Tab String
        /// </summary>
        public static int CurrentTab
        {
            get { return currentTab; }
            set
            {
                currentTab = value;
                if (currentTab < 0) currentTab = 0;
                TAB = "";
                for (int i = 0; i < currentTab; i++)
                {
                    TAB += TAB_STRING;
                }
            }
        }

        #endregion

        #region Options

        public static String NEWLINE    = "\n";
        public static String TAB_STRING = "\t";

        public static Settings SettingObject
        {
            set
            {
                settingObject = value;
            }
        }

        #endregion

        /// <summary>
        /// Parse the input string code using the antlr generated parser
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static StringBuilder GenerateCode(String source)
        {
            ASParser parser;

            CurrentBuffer = new StringBuilder();

            ANTLRStringStream input = new ANTLRStringStream(source);
            ASLexer lexer = new ASLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            parser = new ASParser(tokens);
            parser.SetInput(lexer, input);

            ASParser.compilationUnit_return result = parser.compilationUnit();

            CommonTree tree = (CommonTree)result.Tree;
            CurrentBuffer = new StringBuilder();
            ReformatNode(tree, true);
            return CurrentBuffer;
        }


        # region Private Static Methods

        private static void ReformatCode(ITree tree, Boolean endWithNewLine)
        {
            ReformatCode((CommonTree)tree, endWithNewLine);
        }

        public static void ReformatCode(ITree tree)
        {
            ReformatCode((CommonTree)tree, false);
        }

        public static void ReformatCode(CommonTree tree, Boolean endWithNewLine)
        {
            for (int i = 0; i < tree.ChildCount; i++)
            {
                CommonTree node = (CommonTree)tree.GetChild(i);
                ReformatNode(node, endWithNewLine);
            }
        }

        public static void ReformatNode(ITree node, Boolean endWithNewLine)
        {
            ReformatNode((CommonTree)node, endWithNewLine);
        }

        public static void ReformatNode(ITree node)
        {
            ReformatNode((CommonTree)node, false);
        }

        protected static void ReformatNode(CommonTree node, Boolean endWithNewLine)
        {
            switch (node.Type)
            {
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
                    if (settingObject.NEWLINE_BEFORE_ELSE)
                        CurrentBuffer.Append(NEWLINE + TAB + "else ");
                    else
                        CurrentBuffer.Append(" else ");

                    if (node.GetChild(0).Type == ASLexer.BLOCK || node.GetChild(0).Type == ASLexer.IF)
                    {
                        ReformatNode(node.GetChild(0), settingObject.NEWLINE_AFTER_CONDITION);
                    }
                    else
                    {
                        AddBlock(node.GetChild(0), settingObject.NEWLINE_AFTER_CONDITION);
                    }
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

        /// <summary>
        /// It can be either a simple identifier or a property
        /// </summary>
        /// <param name="node"></param>
        protected static void parsePropertyOrIdentifier(CommonTree node)
        {
            if (node.GetChild(0).Type == ASLexer.IDENT)
            {
                CurrentBuffer.Append(fromIdentifier(node));
            }
            else
            {
                ReformatCode(node);
            }
        }

        /// <summary>
        /// Main unit
        /// </summary>
        /// <param name="node"></param>
        protected static void parseCompilationUnit(CommonTree node)
        {
            for (int i = 0; i < node.ChildCount; i++)
            {
                ReformatNode(node.GetChild(i), true);
            }
        }

        protected static void parsePackage(CommonTree node)
        {
            CurrentBuffer.Append(node.Text);

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

        protected static void parseElement(CommonTree node)
        {
            ReformatCode(node);
        }

        /// <summary>
        /// [] Array initializer
        /// </summary>
        /// <param name="node"></param>
        protected static void parseArrayLiteral(CommonTree node)
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
        protected static void parseObjectLiteral(CommonTree node)
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

        protected static void parseObjectField(CommonTree node)
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
        protected static void parseBlock(CommonTree node, Boolean addNewline)
        {
            if (addNewline)
                CurrentBuffer.Append(NEWLINE + TAB);

            CurrentBuffer.Append("{");
            CurrentTab++;

            for (int i = 0; i < node.ChildCount; i++)
            {
                CurrentBuffer.Append(NEWLINE + TAB);
                ReformatNode(node.GetChild(i));

                if (node.GetChild(i).Type != ASLexer.IF
                    && node.GetChild(i).Type != ASLexer.FOR
                    && node.GetChild(i).Type != ASLexer.FOR_IN
                    && node.GetChild(i).Type != ASLexer.WHILE && node.GetChild(i).Type != ASLexer.FOR_EACH)
                    CurrentBuffer.Append(";");
            }

            CurrentTab--;
            CurrentBuffer.Append(NEWLINE + TAB + "}");
        }

        protected static void AddBlock(ITree node, Boolean addNewLine)
        {
            if (addNewLine)
                CurrentBuffer.Append(NEWLINE + TAB);

            CurrentBuffer.Append("{");
            CurrentTab++;

            CurrentBuffer.Append(NEWLINE + TAB);
            ReformatNode(node);
            if (node.Type != ASLexer.IF
                                && node.Type != ASLexer.FOR
                                && node.Type != ASLexer.FOR_IN
                                && node.Type != ASLexer.WHILE
                                && node.Type != ASLexer.FOR_EACH)
                CurrentBuffer.Append(";");

            CurrentTab--;
            CurrentBuffer.Append(NEWLINE + TAB + "}");
        }

        protected static void parseExpressionList(CommonTree node)
        {
            for (int i = 0; i < node.ChildCount; i++)
            {
                ReformatNode(node.GetChild(i));
            }
        }

        protected static void parseForCondition(CommonTree node)
        {
            ReformatNode(node.GetChild(0));
        }

        protected static void parseForInit(CommonTree node)
        {
            ReformatNode(node.GetChild(0));
        }

        /// <summary>
        /// For..in loop
        /// </summary>
        /// <param name="node"></param>
        protected static void parseForIn(CommonTree node)
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

        protected static void parseFor(CommonTree node)
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

        protected static void parseWhile(CommonTree node)
        {
            CurrentBuffer.Append("while");
            CurrentBuffer.Append(settingObject.SPACE_BETWEEN_METHOD_CALL ? " (" : "(");
            ReformatNode(node.GetChild(0));
            CurrentBuffer.Append(settingObject.SPACE_BETWEEN_METHOD_CALL ? ") " : ")");
            ReformatNode(node.GetChild(1), settingObject.NEWLINE_AFTER_CONDITION);
        }

        protected static void parseCondition(CommonTree node)
        {
            ReformatNode(node.GetChild(0));
        }

        protected static void parseIfCondition(CommonTree node)
        {
            CurrentBuffer.Append("if");
            CurrentBuffer.Append(settingObject.SPACE_BETWEEN_METHOD_CALL ? " (" : "(");
            ReformatNode(node.GetChild(0));
            CurrentBuffer.Append(settingObject.SPACE_BETWEEN_METHOD_CALL ? ") " : ")");

            if (node.GetChild(1).Type != ASLexer.BLOCK)
            {
                AddBlock(node.GetChild(1), settingObject.NEWLINE_AFTER_CONDITION);
            }
            else
            {
                ReformatNode(node.GetChild(1), settingObject.NEWLINE_AFTER_CONDITION);
            }

            if (node.ChildCount > 2)
                ReformatNode(node.GetChild(2));
        }

        protected static void parseVariableDeclaration(CommonTree node)
        {
            CurrentBuffer.Append(node.Text + " ");
            CurrentBuffer.Append(node.GetChild(0).Text);
            ReformatCode(node.GetChild(0));
        }

        protected static void parseParam(CommonTree node)
        {
            ReformatNode(node.GetChild(0));
            if (node.ChildCount > 1)
                ReformatNode(node.GetChild(1));
        }

        protected static void parseParams(CommonTree node)
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

        protected static void parseMethodDefinition(CommonTree node)
        {
            CommonTree blockNode;
            CurrentBuffer.Append(NEWLINE + TAB);
            if (node.GetChild(1).ChildCount > 0)
                CurrentBuffer.Append(fromModifiers(node.GetChild(1)) + " ");

            CurrentBuffer.Append("function");

            if (node.GetChild(2).ChildCount > 0)
                CurrentBuffer.Append(" " + node.GetChild(2).GetChild(0).Text);

            CurrentBuffer.Append(" ");
            ReformatNode(node.GetChild(3));
            CurrentBuffer.Append((settingObject.SPACE_BETWEEN_METHOD_CALL ? " " : "") + "(");
            ReformatNode(node.GetChild(4));
            CurrentBuffer.Append(")");

            if (node.GetChild(5).Type == ASLexer.TYPE_SPEC)
            {
                ReformatNode(node.GetChild(5));
                blockNode = (CommonTree)node.GetChild(6);
            }
            else
            {
                blockNode = (CommonTree)node.GetChild(5);
            }

            if (blockNode != null)
            {
                if (settingObject.SPACE_BETWEEN_METHOD_CALL) CurrentBuffer.Append(" ");
                ReformatNode(blockNode, settingObject.NEWLINE_AFTER_METHOD);
            }
        }

        protected static void parseArguments(CommonTree node)
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

        protected static void parseArrayAcc(CommonTree node)
        {
            ReformatNode(node.GetChild(0));
            CurrentBuffer.Append("[");
            ReformatNode(node.GetChild(1));
            CurrentBuffer.Append("]");
        }

        protected static void parseMethodCall(CommonTree node)
        {
            ReformatNode(node.GetChild(0));
            CurrentBuffer.Append(settingObject.SPACE_BETWEEN_METHOD_CALL ? " " : "");
            ReformatNode(node.GetChild(1));
        }

        protected static void parseAssign(CommonTree node)
        {
            if (settingObject.SPACE_BETWEEN_ASSIGN) CurrentBuffer.Append(" ");
            CurrentBuffer.Append("=");
            if (settingObject.SPACE_BETWEEN_ASSIGN) CurrentBuffer.Append(" ");
            ReformatCode(node);
        }

        protected static void parseTypeSpec(CommonTree node)
        {
            CurrentBuffer.Append((settingObject.SPACE_BETWEEN_TYPE ? " " : "") + ":" + (settingObject.SPACE_BETWEEN_TYPE ? " " : ""));
            CurrentBuffer.Append(fromIdentifier(node.GetChild(0)));
        }

        protected static void parseVariableDefinition(CommonTree node)
        {
            if (node.GetChild(1).ChildCount > 0)
                CurrentBuffer.Append(fromModifiers(node.GetChild(1)) + " ");

            CurrentBuffer.Append(node.GetChild(2).Text + " ");
            CurrentBuffer.Append(node.GetChild(3).Text);
            ReformatCode(node.GetChild(3));
        }

        /// <summary>
        /// Class definition
        /// </summary>
        /// <param name="tree"></param>
        protected static void parseClass(CommonTree tree)
        {
            String st = "";
            if (tree.GetChild(1).ChildCount > 0)
            {
                st = fromModifiers(tree.GetChild(1)) + " ";
            }
            st += "class " + fromIdentifier(tree.GetChild(2));

            if (tree.GetChild(3).Type == ASLexer.EXTENDS)
            {
                st += " extends " + fromIdentifier(tree.GetChild(3).GetChild(0));
                if (tree.GetChild(4).Type == ASLexer.IMPLEMENTS)
                {
                    st += " implements " + fromImplements(tree.GetChild(4));
                }
            }
            else if (tree.GetChild(3).Type == ASLexer.IMPLEMENTS)
            {
                st += " implements " + fromImplements(tree.GetChild(3));
            }
            CurrentBuffer.Append(st);

            if (tree.GetChild(tree.ChildCount - 1).Type == ASLexer.TYPE_BLOCK)
                parseTypeBlock((CommonTree)tree.GetChild(tree.ChildCount - 1));
        }

        /// <summary>
        /// Class Body
        /// </summary>
        /// <param name="tree"></param>
        protected static void parseTypeBlock(CommonTree tree)
        {
            CurrentBuffer.Append((settingObject.NEWLINE_AFTER_CLASS ? NEWLINE : " ") + TAB + "{");
            CurrentTab++;

            for (int i = 0; i < tree.ChildCount; i++)
            {
                CurrentBuffer.Append(NEWLINE + TAB);
                ReformatNode(tree.GetChild(i));
                if (tree.GetChild(i).Type != ASLexer.IF
                    && tree.GetChild(i).Type != ASLexer.FOR
                    && tree.GetChild(i).Type != ASLexer.FOR_IN
                    && tree.GetChild(i).Type != ASLexer.WHILE
                    && tree.GetChild(i).Type != ASLexer.FOR_EACH
                    && tree.GetChild(i).Type != ASLexer.METHOD_DEF)
                    CurrentBuffer.Append(";");
            }

            CurrentTab--;
            CurrentBuffer.Append(NEWLINE + TAB + "}");
        }

        /// <summary>
        /// Return a dot separated identifier
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        protected static String fromIdentifier(ITree tree)
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
        protected static String fromModifiers(ITree tree)
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
        protected static String fromImplements(ITree tree)
        {
            tree = (CommonTree)tree;
            string[] st = new string[tree.ChildCount];
            for (int i = 0; i < tree.ChildCount; i++)
            {
                st[i] = fromIdentifier(tree.GetChild(i));
            }
            return String.Join(", ", st);
        }

        #endregion


    }
}
