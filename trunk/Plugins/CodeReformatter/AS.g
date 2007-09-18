
grammar AS;

options {
	k              = 2;
	output         = AST;
	language       = CSharp;
	ASTLabelType   = CommonTree;
	TokenLabelType = CommonToken;
	backtrack      = true;
	memoize        = true;
}

tokens {
	COMPILATION_UNIT;
	TYPE_BLOCK; 
	METHOD_DEF; 
	VAR_DEF;
	ANNOTATIONS; 
	ANNOTATION; 
	ANNOTATION_PARAMS;
	MODIFIERS; 
	NAMESPACE_DEF;
	ACCESSOR_ROLE;
	CLASS_DEF; 
	INTERFACE_DEF;
	PARAMS;
	PARAM; 
	TYPE_SPEC;
	BLOCK; 
	ELIST;
	CONDITION; 
	ARGUMENTS;
	EXPR_STMNT;
	ENCPS_EXPR;
	VAR_INIT;
	METHOD_CALL; 
	PROPERTY_OR_IDENTIFIER; 
	PROPERTY_ACCESS; 
	TYPE_NAME;
	ARRAY_ACC;
	UNARY_PLUS; 
	UNARY_MINUS; 
	POST_INC; 
	POST_DEC; 
	PRE_INC; 
	PRE_DEC;
	ARRAY_LITERAL;
	ELEMENT; 
	OBJECT_LITERAL;
	OBJECT_FIELD; 
	FUNC_DEF;
	FOR_INIT; 
	FOR_CONDITION; 
	FOR_ITERATOR;
	FOR_EACH; 
	FOR_IN;
	SWITCH_STATEMENT_LIST;
	IDENTIFIER;
	DEFAULT_XML_NAMESPACE;
	SINGLELINE_COMMENT;
	MULTILINE_COMMENT;
	COMMENT_LIST;
	COMMENT_ENTRY;
	VIRTUAL_PLACEHOLDER;
	ANNOTATION_ASSIGN;
	AS2_COMPILATION_UNIT;
	IMETHOD_DEF;
}

scope InOperator {
	Boolean allowed;
}

@parser::header {
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
}

@lexer::header {
using System.IO;
using System.Diagnostics;

}

@parser::namespace {
CodeReformatter.Generators.Core
}

@lexer::namespace {
CodeReformatter.Generators.Core
}

// disable standard error handling; be strict

@rulecatch { 
	catch (NoViableAltException e)
	{
		Debug.WriteLine("NoValiable alt: token=" + e.Token + " (decision=" + e.decisionNumber + " state " + e.stateNumber + ")" + " decision=<<" + e.grammarDecisionDescription + ">>");
		throw e;
	}
	catch (MismatchedTokenException e)
	{
        Debug.WriteLine("[" + e.Line + ":" + e.Index + "]: " + e.Message + ". Unexpected " + e.UnexpectedType.ToString() +  "( expecting: "+ e.expecting + " )");
        throw e;
	}
	catch(RecognitionException e)
	{
		Debug.WriteLine("RecognitionException: " + e);
		throw e;
	}
}

@lexer::members {

	ASParser parser;
	public void SetInput(ASParser p)
	{
		this.parser = p;
	}
}

@parser::members {

	#region Properties

	private ReformatOptions options = new ReformatOptions();
	private StringBuilder buffer;
	private List<string> importList;
	private int currentTab = 0;
    private String tabString = "\t";
    private String newline = "\n";
    private String tab = "";
    private Regex lineSplitterReg = new Regex("[\n\r]+", RegexOptions.Multiline);
	private ASLexer lexer;
	private ICharStream cs;
	public static int CHANNEL_PLACEHOLDER = 999;
	
	
    public ASParser(ITokenStream input, ReformatOptions opt)
        : base(input)
    {
        options = opt;
        InitializeCyclicDFAs();
        ruleMemo = new IDictionary[307 + 1];
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

	#region Members

	public void SetInput(ASLexer lexer, ICharStream cs) 
	{
		this.lexer = lexer;
		this.cs = cs;
	}
	
	public StringBuilder Buffer
	{
		get { return this.buffer; }
	}
	
    /// <summary>
    /// Return a dot separated identifier
    /// </summary>
    /// <param name="tree"></param>
    /// <returns></returns>
    protected static String fromIdentifier(ITree tree)
    {
        tree = (CommonTree)tree;
        if (tree.Type == ASLexer.IDENT || tree.Type == ASLexer.VOID || tree.Type == ASLexer.STAR) return tree.Text;
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
        List<string> mods = new List<string>();
        for (int i = 0; i < tree.ChildCount; i++)
        {
            mods.Add(tree.GetChild(i).Text);
        }
        mods.Sort();
        return String.Join(" ", mods.ToArray());
    }
    
    /// <summary>
    /// Insert a comment in the passed String builder
    /// </summary>
    /// <param name="tree"></param>    
    private void insertComment(ParserRuleReturnScope rule, Boolean newlineBefore, Boolean newlineAfter, int carriageReturns)
    {
		CommonTree tree;
		CommonTree comment;
		
		if(rule != null)
		{
			tree = (CommonTree)rule.Tree;   // COMMENT_LIST
			for(int i = 0; i < tree.ChildCount; i++)
			{
				if(newlineBefore && i == 0)
				{
					for(int n = 0; n < carriageReturns; n++)
					{
						buffer.Append(NewLine + tab);
					}
				}
			
				comment = (CommonTree)tree.GetChild(i); // COMMENT_ENTRY
				if(comment.GetChild(0).Type == ASLexer.MULTILINE_COMMENT)
				{
					string[] lines = lineSplitterReg.Split(comment.GetChild(0).GetChild(0).Text);
					int k = 0;
					foreach (string line in lines)
					{
						buffer.Append((k > 0 ? " " : "") + line.Trim() + (k < lines.Length-1 ? NewLine + tab : ""));
						k++;
					}				
				} else {
					buffer.Append(comment.GetChild(0).GetChild(0).Text.TrimEnd());
				}
                if(i < tree.ChildCount - 1 || newlineAfter)
                    buffer.Append(NewLine + tab);
			}
		}
    }

    private void insertComment(ParserRuleReturnScope rule, Boolean newlineBefore, Boolean newlineAfter)
    {
		insertComment(rule, newlineBefore, newlineAfter, 1);
    }
    
    
    private void insertComment(ParserRuleReturnScope rule)
    {
		insertComment(rule, false, false, 0);
    }
    
	/// <summary>
    /// Remove duplicates from a list of strings
    /// </summary>
    /// <param name="inputList"></param>
    /// <returns>List<string></returns>
	protected static List<string> removeDuplicates(List<string> inputList)
	{
		Dictionary<string, int> uniqueStore = new Dictionary<string, int>();
		List<string> finalList = new List<string>();
		foreach (string currValue in inputList)
		{
			if (!uniqueStore.ContainsKey(currValue))
			{
				uniqueStore.Add(currValue, 0);
				finalList.Add(currValue);
			}
		}
		return finalList;
	}
    
    
        
    /// <summay>
    /// Finalize the code generation
    /// </summary>
    /// <returns></returns>
    private void finalize()
    {
        // organize imports
        List<string>  importListUnique = removeDuplicates(importList);
        importListUnique.Sort();
        if(importListUnique.Count > 0)
        {
			foreach (string item in importListUnique)
			{
				buffer.Insert(0, NewLine + "import " + item + ";");
			}
        }
        
        // newline at the end of file
        buffer.Append(NewLine);
    }
    
    
	#endregion
}

compilationUnit[StringBuilder ret]
@init {
	buffer = ret;
}
@after {
	finalize();
}
	:	as2CompilationUnit -> ^(COMPILATION_UNIT as2CompilationUnit)
	;
	

as2CompilationUnit
@init {
	importList = new List<string>();
}
	:	(
			importDefinition
		|	annotations
		|	{ buffer.Append(NewLine + tab + NewLine + tab); } c=comments { insertComment(c); }
		)*
		as2Type
	;

as2Type
	:	mods=modifiers!
	(	as2ClassDefinition[$mods.tree]
	|	as2InterfaceDefinition[$mods.tree]
	)
	;	


endOfFile
	:	EOF!
	;

importDefinition
	:	IMPORT^
		ide=identifierStar			{ importList.Add(fromIdentifier((CommonTree)ide.Tree)); }
		semi
	;

semi
	:	SEMI!
	|
	;	

as2ClassDefinition[CommonTree mods]
	:	cl=CLASS					{ buffer.Append(NewLine + tab + cl.Text + " "); }
		ide=identifier
		ext=classExtendsClause	
		imp=implementsClause	{
									buffer.Append(options.NewlineAfterMethod ? NewLine + tab : " ");
									buffer.Append("{");
									CurrentTab++; 
								}
		typeBlock				{ 
									CurrentTab--;
									buffer.Append(NewLine + tab);
									buffer.Append("}");
								}
		
		-> ^(CLASS_DEF {$mods} identifier classExtendsClause implementsClause typeBlock)
	;
	

interfaceDefinition[CommonTree mods]
@init { CommonTree annos = null; }
	:	tk=INTERFACE				{ buffer.Append(NewLine + tab + NewLine + tab + tk.Text + " "); }
		ide=ident
		interfaceExtendsClause	{
									buffer.Append(options.NewlineAfterMethod ? NewLine + tab : "");
									buffer.Append("{");
									CurrentTab++; 
								}
		interfaceTypeBlock		{ 
									CurrentTab--;
									buffer.Append(NewLine + tab);
									buffer.Append("}");
								}
		-> ^(INTERFACE_DEF {$mods} ident interfaceExtendsClause interfaceTypeBlock)
	;

as2InterfaceDefinition[CommonTree mods]
@init { CommonTree annos = null; }
	:	INTERFACE identifier
		interfaceExtendsClause
		interfaceTypeBlock
		-> ^(INTERFACE_DEF {$mods} identifier interfaceExtendsClause interfaceTypeBlock)
	;

classExtendsClause
	:	(
			tk=EXTENDS^		{ buffer.Append(" " + tk.Text + " "); } 
			ide=identifier	
		)?
	;
interfaceExtendsClause
	:	(
			tk=EXTENDS^		{ buffer.Append(tk.Text + " "); } 
			ide=identifier	{ buffer.Append(" "); }
			( 
				COMMA!				{ buffer.Append(", "); }
				ide2=identifier		{ buffer.Append(" "); }
			)*
		)?
	;
	
implementsClause
	:	(IMPLEMENTS^		{ buffer.Append(" implements "); }
			ide=identifier	{ } 
			(
				COMMA!			{ buffer.Append(", "); }
				ide=identifier	{  }
			)*
		)?
	;
	
interfaceTypeBlock	
	:	LCURLY
		(interfaceTypeBlockEntry)*
		RCURLY
		-> ^(TYPE_BLOCK interfaceTypeBlockEntry*)
	;
	
typeBlock
	:	LCURLY
		(typeBlockEntry)*
		RCURLY
		-> ^(TYPE_BLOCK typeBlockEntry*)
	;

interfaceTypeBlockEntry
	:	{ buffer.Append(NewLine + tab); }
		m=modifiers!
		(	
			interfaceMethodDefinition[$m.tree]
		)
	|	importDefinition
	|	{ buffer.Append(NewLine + tab); } as2IncludeDirective
	|	{ buffer.Append(NewLine + tab); } annotations
	;

typeBlockEntry
	:	{ buffer.Append(NewLine + tab); }
		m=modifiers!
		(	variableDefinition[$m.tree]		{ buffer.Append(";"); }
		|	methodDefinition[$m.tree]
		)
	|	importDefinition
	|	{ buffer.Append(NewLine + tab); } as2IncludeDirective
	|	{ buffer.Append(NewLine + tab); } annotations
	|	{ buffer.Append(NewLine + tab + NewLine + tab); } c=comments { insertComment(c); }
	;

as2IncludeDirective
	:	INCLUDE_DIRECTIVE st=STRING_LITERAL	{	buffer.Append("#include " + ((CommonToken)st).Text); }
		-> ^(INCLUDE_DIRECTIVE STRING_LITERAL)
	;

includeDirective
	:	'include' STRING_LITERAL semi
	;

interfaceMethodDefinition[CommonTree mods]
	:	FUNCTION					{ buffer.Append("function "); }
		r=optionalAccessorRole
		ide=ident					{ buffer.Append(((CommonTree)ide.Tree).Text); }
		parameterDeclarationList
		type_exp=typeExpression?	{
										if(options.NewlineAfterMethod) buffer.Append(NewLine + tab);
										buffer.Append("{");
										CurrentTab++;
									}
		(semi)
		-> ^(IMETHOD_DEF {$mods}
		                optionalAccessorRole ident
						parameterDeclarationList
						typeExpression?
			)
	;


methodDefinition[CommonTree mods]
	:	{ 
			if(mods.ChildCount > 0)
				buffer.Append(fromModifiers(mods) + " ");
		}
		FUNCTION					{ buffer.Append("function "); }
		r=optionalAccessorRole
		ide=ident
									{ buffer.Append(((CommonTree)ide.Tree).Text); }
		
		parameterDeclarationList
		type_exp=typeExpression?
									{
										if(options.NewlineAfterMethod) buffer.Append(NewLine + tab);
										buffer.Append("{");
										CurrentTab++;
									}
		(
			c1=comments?			{ insertComment(c1, true, false); }
			block semi
		)
									{
										CurrentTab--;
										buffer.Append(NewLine + tab);
										buffer.Append("}");
										buffer.Append(NewLine + tab);
									}
		-> ^(METHOD_DEF {$mods}
		                optionalAccessorRole ident
						parameterDeclarationList
						typeExpression?
						block
			)
	;

optionalAccessorRole
	:	accessorRole?
		-> ^(ACCESSOR_ROLE accessorRole?)
	;

accessorRole
	:	GET		{ buffer.Append("get "); }
	|	SET		{ buffer.Append("set "); }
	;

variableDefinition[CommonTree mods]
scope InOperator;
@init {
	$InOperator::allowed = true;
}
	:	{ 
			if(mods.ChildCount > 0) buffer.Append(fromModifiers(mods) + " ");
		}
		decl=varOrConst				{ buffer.Append(((CommonTree)decl.Tree).Text + " "); }
		variableDeclarator
		(
									{	buffer.Append(";"); }
			COMMA					{ 
										buffer.Append(NewLine + tab); 
										if(mods.ChildCount > 0) buffer.Append(fromModifiers(mods) + " ");
										buffer.Append(((CommonTree)decl.Tree).Text + " ");
									}
			variableDeclarator
		)*
		semi
		-> ^(VAR_DEF {$mods} $decl variableDeclarator+)
	;

varOrConst
	:	VAR | CONST
	;

variableDeclarator
	:	ide=ident^					{ buffer.Append(((CommonTree)ide.Tree).Text);  }
		type_exp=typeExpression?	{
									}
		variableInitializer?
	;
	
declaration
	:	decl=varOrConst^				{ buffer.Append(((CommonTree)decl.Tree).Text + " "); }
		variableDeclarator
		declarationTail
	;

declarationTail
scope InOperator;
@init {
	$InOperator::allowed = true;
}
	:	(
			COMMA!	{ buffer.Append(options.SpaceBetweenArguments ? ", " : ","); }
			variableDeclarator
		)*
	;

variableInitializer
	:	ASSIGN^				{ 
								if(options.SpaceBetweenAssign) buffer.Append(" ");
								buffer.Append("=");
								if(options.SpaceBetweenAssign) buffer.Append(" ");
							}
		assignmentExpression
	;

// A list of formal parameters
// TODO: shouldn't the 'rest' parameter only be allowed in the last position?
parameterDeclarationList
	:	LPAREN			{ buffer.Append(options.SpaceBeforeMethodDef ? " (" : "("); }
		(	parameterDeclaration
			(
				COMMA	{ buffer.Append(options.SpaceBetweenArguments ? ", " : ","); } 
				parameterDeclaration
			)*
		)?
		RPAREN			{ buffer.Append(")"); }
		-> ^(PARAMS parameterDeclaration*)
	;


parameterDeclaration
	:	basicParameterDeclaration
	;

basicParameterDeclaration
	:	CONST? 
		ide=ident					{ buffer.Append(((CommonTree)ide.Tree).Text); }
		type_exp=typeExpression?	{  }
		parameterDefault?
		-> ^(PARAM CONST? ident typeExpression? parameterDefault?)
	;

parameterDefault
scope InOperator;
@init {
	$InOperator::allowed = true;
}
		// TODO: can we be more strict about allowed values?
	:	ASSIGN^ assignmentExpression
	;

block
	:	LCURLY
		blockEntry* 
		RCURLY
		-> ^(BLOCK blockEntry*)
	;

blockEntry
	:	{ buffer.Append(NewLine + tab); } statement
	;

condition
	:	LPAREN			{ buffer.Append(options.SpaceBeforeMethodDef ? " (" : "("); }
		expression 
		RPAREN			{ buffer.Append(")"); }
		-> ^(CONDITION expression)
	;

// cannot start with a function Definition

statement
	:	(LCURLY)=> block
	|	declarationStatement			{ buffer.Append(";"); }
	|	expressionStatement				{ buffer.Append(";"); }
	|	ifStatement
	|	forStatement
	|	whileStatement
	|	doWhileStatement
	|	withStatement
	|	switchStatement
	|	breakStatement				{ buffer.Append(";"); }
	|	continueStatement			{ buffer.Append(";"); }
	|	returnStatement				{ buffer.Append(";"); }
	|	throwStatement				{ buffer.Append(";"); }
	|	tryStatement
	|	SEMI!
	|	c=comments					{ insertComment(c); }
	;

declarationStatement
scope InOperator;
@init {
	$InOperator::allowed = true;
}
	:	declaration 
		semi
	;

expressionStatement
scope InOperator;
@init {
	$InOperator::allowed = true;
}
	:	expressionList semi
		-> ^(EXPR_STMNT expressionList)
	;
	
ifStatement
	:	IF^						{ buffer.Append("if");}
		condition				{ }
								{
									buffer.Append( (options.NewlineAfterCondition ? NewLine + tab : "") + "{"); 
									CurrentTab++;
									int next_test = input.LA(1);
									if(next_test != ASLexer.LCURLY)	buffer.Append(NewLine + tab);
								}
		c1=comments?				{ insertComment(c1, false, true); }
		statement
								{
									CurrentTab--;
									buffer.Append(NewLine + tab);
									buffer.Append("}");
								}
		(
			c2=comments?	{	insertComment(c2, true, true, 1);	} 
			(ELSE)=>elseClause
		)?
	;



elseClause
@init{
	int next_test   = -1;
	int next_test_2 = -1;
}
	:	ELSE^					{
									buffer.Append(options.NewlineBeforeElse ? NewLine + tab : " ");
									buffer.Append("else");
									
									next_test = input.LA(1);
									if(next_test == ASLexer.IF) {
										buffer.Append(" ");
									} else {
										buffer.Append( (options.NewlineAfterCondition ? NewLine + tab : " ") + "{"); 
										next_test_2 = input.LA(1);
										CurrentTab++;
										if(next_test_2 != ASLexer.LCURLY) buffer.Append(NewLine + tab);										
									}
								}
		statement				{
									if(next_test != ASLexer.IF) {
										CurrentTab--;
										buffer.Append(NewLine + tab);
										buffer.Append("}");
									} else {
									}
								}
	;

throwStatement
	:	'throw'^ { buffer.Append("throw "); } expression semi
	;

tryStatement
	:	'try'			{ 
							buffer.Append("try");
							buffer.Append((options.NewlineAfterCondition ? NewLine + tab : "") + "{"); 
							CurrentTab++;
						}
		block			{
							CurrentTab--;
							buffer.Append(NewLine + tab);
							buffer.Append("}");
						}
		( { buffer.Append((options.NewlineBeforeElse ? NewLine + tab : " ")); }  catchBlock)*
		( { buffer.Append((options.NewlineBeforeElse ? NewLine + tab : " ")); } finallyBlock)?
	;

catchBlock
	:	'catch'		{ buffer.Append("catch"); }
		LPAREN!		{ buffer.Append("("); }
		ide=ident	{ buffer.Append( ((CommonTree)ide.Tree).Text ); }
		typeExpression? 
		RPAREN!		{ 
						buffer.Append(")"); 
						buffer.Append((options.NewlineAfterCondition ? NewLine + tab : "") + "{"); 
						CurrentTab++;
					}
		block		{
						CurrentTab--;
						buffer.Append(NewLine + tab);
						buffer.Append("}");
					}
	;

finallyBlock
	:	'finally'	{ 
						buffer.Append("finally"); 
						buffer.Append((options.NewlineAfterCondition ? NewLine + tab : "") + "{"); 
						CurrentTab++;
					}
		block		{
						CurrentTab--;
						buffer.Append(NewLine + tab);
						buffer.Append("}");
					}
	;

returnStatement
	:	RETURN^ { buffer.Append("return "); } expression semi
	|	RETURN^ semi { buffer.Append("return"); }
	;
		
continueStatement
	:	CONTINUE^ semi	{ buffer.Append("continue"); }
	;

breakStatement
	:	BREAK^ semi		{ buffer.Append("break"); }
	;

switchStatement
	:	SWITCH^		{ buffer.Append("switch"); }
		(condition)
		switchBlock
	;

switchBlock
	:	LCURLY								{ 
												buffer.Append((options.NewlineAfterCondition ? NewLine + tab : "") + "{");
												CurrentTab++;
											}
		( {buffer.Append(NewLine + tab); } caseStatement)*
		( {buffer.Append(NewLine + tab); } defaultStatement)?
		RCURLY								{
												CurrentTab--;
												buffer.Append(NewLine + tab);
												buffer.Append("}");
											}
		-> ^(BLOCK caseStatement* defaultStatement?)
	;

caseStatement
	:	CASE^		{ buffer.Append("case "); } 
		expression 
		COLON!		{ 
						buffer.Append(":");  
						CurrentTab++;
					}
		l=switchStatementList
					{ CurrentTab--; }
	;
	
defaultStatement
	:	DEFAULT^	{ buffer.Append("default"); }
		COLON!		{ 
						buffer.Append(":"); 
						CurrentTab++;
					}
		l=switchStatementList
					{ CurrentTab--; }
	;

switchStatementList
	:	( {buffer.Append(NewLine + tab); } statement)* -> ^(SWITCH_STATEMENT_LIST statement*)
	;

forStatement
scope InOperator;
@init {
	$InOperator::allowed = false;
	int next_node = -1;
	int next_node_2 = -1;
}
	:	f=FOR			{ buffer.Append("for"); }
		LPAREN			{ buffer.Append(options.SpaceBeforeMethodDef ? " (" : "(");   }
		(	
			(forInClauseDecl IN)=>forInClause 
			RPAREN							{ 
												buffer.Append(")"); 
												buffer.Append((options.NewlineAfterCondition ? NewLine + tab : "") + "{"); 
												CurrentTab++;
												int next_test = input.LA(1);
												if(next_test != ASLexer.LCURLY)	buffer.Append(NewLine + tab);												
											}
			statement						{
												CurrentTab--;
												buffer.Append(NewLine + tab);
												buffer.Append("}");
											}
			-> ^(FOR_IN[$f] forInClause statement)

			|	traditionalForClause RPAREN { 
												buffer.Append(")"); 
												buffer.Append((options.NewlineAfterCondition ? NewLine + tab : "") + "{"); 
												CurrentTab++;
												int next_test_2 = input.LA(1);
												if(next_test_2 != ASLexer.LCURLY)	buffer.Append(NewLine + tab);												
											} 
				statement					{
												CurrentTab--;
												buffer.Append(NewLine + tab);
												buffer.Append("}");
											}
			-> ^($f traditionalForClause statement)
		)
	;

traditionalForClause
	:	a=forInit SEMI!	{ buffer.Append(options.SpaceBetweenArguments ? "; " : ";"); } // initializer
		b=forCond SEMI!	{ buffer.Append(options.SpaceBetweenArguments ? "; " : ";"); } // condition test
		c=forIter // updater
	;

forInClause
	:	forInClauseDecl IN! { buffer.Append(" in "); } forInClauseTail
	;

forInClauseDecl
scope InOperator;
@init {
	$InOperator::allowed = false;
}
	:	declaration 
	|	ide=ident			{ buffer.Append( ((CommonTree)ide.Tree).Text ); }
	;


forInClauseTail
scope InOperator;
@init {
	$InOperator::allowed = true;
}
	:	expressionList
	;

// The initializer for a for loop
forInit	
scope InOperator;
@init {
	$InOperator::allowed = false;
}
	:	(declaration | expressionList )?
		-> ^(FOR_INIT declaration? expressionList?)
	;

forCond
scope InOperator;
@init {
	$InOperator::allowed = true;
}
	:	expressionList?
		-> ^(FOR_CONDITION expressionList?)
	;

forIter
scope InOperator;
@init {
	$InOperator::allowed = true;
}
	:	expressionList?
		-> ^(FOR_ITERATOR expressionList?)
	;

whileStatement
	:	WHILE^			{ buffer.Append("while"); }
		condition		{
							buffer.Append((options.NewlineAfterCondition ? NewLine + tab : "") + "{"); 
							CurrentTab++;
							int next_test = input.LA(1);
							if(next_test != ASLexer.LCURLY)	buffer.Append(NewLine + tab);
						}
		(statement)		{
							CurrentTab--;
							buffer.Append(NewLine + tab);
							buffer.Append("}");
						}
	;

doWhileStatement
	:	DO^				{
							buffer.Append("do");
							buffer.Append((options.NewlineAfterCondition ? NewLine + tab : "") + "{"); 
							CurrentTab++;
							int next_test = input.LA(1);
							if(next_test != ASLexer.LCURLY)	buffer.Append(NewLine + tab);							
						}
		statement		{
							CurrentTab--;
							buffer.Append(NewLine + tab);
							buffer.Append("}");
						}
		WHILE!			{ buffer.Append(" while"); }
		(condition) 
		semi
	;

withStatement
	:	WITH^			{ buffer.Append("with"); }
		condition		{
							buffer.Append((options.NewlineAfterCondition ? NewLine + tab : "") + "{"); 
							CurrentTab++;
							int next_test = input.LA(1);
							if(next_test != ASLexer.LCURLY)	buffer.Append(NewLine + tab);
						}
		(statement)		{
							CurrentTab--;
							buffer.Append(NewLine + tab);
							buffer.Append("}");
						}
	;

typeExpression
	:	
		c=COLON			{ buffer.Append(options.SpaceBetweenType ? " : " : ":"); }
		(
			identifier 
			| VOID		{ buffer.Append("Void"); }
			| STAR		{ buffer.Append("*");	}
		)
		-> ^(TYPE_SPEC[$c] identifier? VOID? STAR?)
	;	

identifier 
	:	qualifiedIdent					{ }
		(	options{greedy=true;}
		: 	DOT { buffer.Append("."); } qualifiedIdent
		)*
		-> ^(IDENTIFIER qualifiedIdent+)
	;

qualifiedIdent
	:	ide=ident		{ buffer.Append( ((CommonTree)ide.Tree).Text ); }
	;

namespaceName
	:	IDENT | reservedNamespace
	;

reservedNamespace
	:	PUBLIC
	|	PRIVATE
	|	PROTECTED
	|	INTERNAL
	;

identifierStar
	:	ide=ident					
		(	options{greedy=true;}
		:	DOT ide2=ident			
		)* 
		(	DOT STAR				
		)?
		-> ^(IDENTIFIER ident+ STAR?)
	;
	
annotations
	:	(	
				( { buffer.Append(NewLine + tab); } annotation )
			|	( { buffer.Append(NewLine + tab); } includeDirective )
		)+
		-> ^(ANNOTATIONS annotation+)
	;

annotation
	:	LBRACK					{ buffer.Append("["); }
		ide=ident				{ buffer.Append(((CommonTree)ide.Tree).Text); }
		annotationParamList?
		RBRACK					{ buffer.Append("]"); }
		-> ^(ANNOTATION ident annotationParamList?)
	;

annotationParamList
	:
		LPAREN						{ buffer.Append("("); }
		(	annotationParam
			(
				COMMA				{ buffer.Append(","); }
				annotationParam
			)*
		)?
		RPAREN						{ buffer.Append(")"); }
		-> ^(ANNOTATION_PARAMS annotationParam*)
	;

annotationParam
	:
		ide1=ident ASSIGN cn1=constant { buffer.Append(((CommonTree)ide1.Tree).Text + "=" + ((CommonTree)cn1.Tree).Text); } -> ^(ASSIGN ident constant)
	|	ide2=ident ASSIGN ide3=ident { buffer.Append(((CommonTree)ide2.Tree).Text + "=" + ((CommonTree)ide3.Tree).Text); }	-> ^(ASSIGN ident ident)
	|	cn2=constant	{ buffer.Append(((CommonTree)cn2.Tree).Text); }	-> constant
	|	ide4=ident		{ buffer.Append(((CommonTree)ide4.Tree).Text); } -> ident
	;	


modifiers
	:	modifier*
		-> ^(MODIFIERS modifier*)
	;

modifier
	:	namespaceName
	|	STATIC
	|	'final'
	|	'enumerable'
	|	'explicit'
	|	'override'
	|	DYNAMIC
	|	'intrinsic'
	;

arguments
scope InOperator;
@init {
	$InOperator::allowed = true;
}
	:	LPAREN { buffer.Append("("); } expressionList RPAREN	{ buffer.Append(")");  }
	|	LPAREN RPAREN { buffer.Append("()"); }
	;
		

element
scope InOperator;
@init {
	$InOperator::allowed = true;
}
	:	assignmentExpression
		-> ^(ELEMENT assignmentExpression)
	;

// This is an initializer used to set up an array.
arrayLiteral
	:	LBRACK { buffer.Append("["); } elementList? RBRACK { buffer.Append("]"); }
		-> ^(ARRAY_LITERAL elementList?)
	;

elementList
	:	nonemptyElementList 
		(
			COMMA!						{ buffer.Append(options.SpaceBetweenArguments ? ", " : ","); }
			nonemptyElementList?
		)*
	;
	
nonemptyElementList
scope InOperator;
@init {
	$InOperator::allowed = true;
}
	:	assignmentExpression 
		(
			COMMA						{ buffer.Append(options.SpaceBetweenArguments ? ", " : ","); }
			assignmentExpression
		)*
	;	

// This is an initializer used to set up an object.
objectLiteral
@init{
	int next_token = -1;
	}
	:	LCURLY			{ 
							buffer.Append("{"); 
							next_token = input.LA(1);
							if(next_token != ASLexer.RCURLY && options.NewlineBetweenFields)
							{
								CurrentTab++;
								buffer.Append(NewLine + tab);
							}
						} 
		fieldList? 
		RCURLY			{ 
							if(next_token != ASLexer.RCURLY && options.NewlineBetweenFields)
							{
								CurrentTab--;
								buffer.Append(NewLine + tab);
							}
							buffer.Append("}"); 
						}
		-> ^(OBJECT_LITERAL fieldList?)
	;
	
fieldList
	:	literalField 
		(
			COMMA!						{
											if(options.NewlineBetweenFields)
											{
												buffer.Append("," + NewLine + tab);
											} else 
											{ 
												buffer.Append(options.SpaceBetweenArguments ? ", " : ","); 
											}
										} 
			literalField?
		)*
	;
	
literalField 
	: 	field=fieldName 
		COLON			{ buffer.Append( options.SpaceBetweenType ? " : " : ":"); }
		element
		-> ^(OBJECT_FIELD fieldName element)
	;
	
fieldName
	:	ide=ident		{ buffer.Append(((CommonTree)ide.Tree).Text ); }
	|	num=number		{ buffer.Append(((CommonTree)num.Tree).Text); }
	;

// the mother of all expressions
expression
scope InOperator;
@init {
	$InOperator::allowed = true;
}
	:	assignmentExpression
	;

// This is a list of expressions.
expressionList
	:	assignmentExpression 
		(
			COMMA					{buffer.Append(options.SpaceBetweenArguments ? ", " : ","); } 
			assignmentExpression
		)* -> ^(ELIST assignmentExpression (COMMA assignmentExpression)*)
	;

// assignment expression (level 13)
assignmentExpression
	:	c=conditionalExpression		{ }
	(	(assignmentOperator)=> op=assignmentOperator 
													{ 
														if(options.SpaceBetweenAssign) buffer.Append(" ");
														buffer.Append(((CommonTree)op.Tree).Text ); 
														if(options.SpaceBetweenAssign) buffer.Append(" "); 
													}
		assignmentExpression
	)*
	;

assignmentOperator
	:	ASSIGN
	| 	STAR_ASSIGN
	|	DIV_ASSIGN
	|	MOD_ASSIGN
	|	PLUS_ASSIGN
	|	MINUS_ASSIGN
	|	SL_ASSIGN
	|	SR_ASSIGN
	|	BSR_ASSIGN
	|	BAND_ASSIGN
	|	BXOR_ASSIGN
	|	BOR_ASSIGN
	|	LAND_ASSIGN
	|	LOR_ASSIGN
	;

// conditional test (level 12)
conditionalExpression
	:	(logicalOrExpression -> logicalOrExpression)
		(
			QUESTION	{ 
							if(options.SpaceBetweenOperators) buffer.Append(" ");
							buffer.Append("?"); 
							if(options.SpaceBetweenOperators) buffer.Append(" "); 
						}
			conditionalSubExpression
			-> ^(QUESTION $conditionalExpression conditionalSubExpression)
		)?
	;
conditionalSubExpression
	:	assignmentExpression 
		COLON^			{ buffer.Append(options.SpaceBetweenOperators ? " : " : ":"); }
		assignmentExpression
	;

// TODO: should 'and'/'or' have same precidence as '&&'/'||' ?

// logical or (||)  (level 11)
logicalOrExpression
	:	logicalAndExpression
		(
			op=logicalOrOperator^					{ 
														if(options.SpaceBetweenOperators) buffer.Append(" ");
														buffer.Append(((CommonTree)op.Tree).Text ); 
														if(options.SpaceBetweenOperators) buffer.Append(" "); 
													}
			logicalAndExpression
		)*
	;

logicalOrOperator
	:	LOR | 'or'
	;

// logical and (&&)  (level 10)
logicalAndExpression
	:	bitwiseOrExpression
		(
			op=logicalAndOperator^					{ 
														if(options.SpaceBetweenOperators) buffer.Append(" ");
														buffer.Append(((CommonTree)op.Tree).Text ); 
														if(options.SpaceBetweenOperators) buffer.Append(" "); 
													}
			bitwiseOrExpression
		)*
	;

logicalAndOperator
	:	LAND | 'and'
	;

// bitwise or non-short-circuiting or (|)  (level 9)
bitwiseOrExpression
	:	bitwiseXorExpression
		(BOR^ bitwiseXorExpression)*
	;

// exclusive or (^)  (level 8)
bitwiseXorExpression
	:	bitwiseAndExpression
		(BXOR^ bitwiseAndExpression)*
	;

// bitwise or non-short-circuiting and (&)  (level 7)
bitwiseAndExpression
	:	equalityExpression
		(BAND^ equalityExpression)*
	;

// equality/inequality (==/!=) (level 6)
equalityExpression
	:	relationalExpression
	(	
		op=equalityOperator^						{ 
														if(options.SpaceBetweenOperators) buffer.Append(" ");
														buffer.Append(((CommonTree)op.Tree).Text ); 
														if(options.SpaceBetweenOperators) buffer.Append(" "); 
													}
		relationalExpression
	)*
	;

equalityOperator
	:	STRICT_EQUAL | STRICT_NOT_EQUAL | NOT_EQUAL | EQUAL
	;
	
// boolean relational expressions (level 5)
relationalExpression
	:	shiftExpression
		(
			(relationalOperator)=> op=relationalOperator^	{ 
																buffer.Append(" ");
																buffer.Append(((CommonTree)op.Tree).Text ); 
																buffer.Append(" "); 
															}
			shiftExpression
		)*
	;

relationalOperator
	:	{$InOperator::allowed}? IN
	|	LT | GT | LE | GE | IS | AS | INSTANCEOF
	;

// bit shift expressions (level 4)
shiftExpression
	:	additiveExpression
		(
			op=shiftOperator^			{ 
											if(options.SpaceBetweenOperators) buffer.Append(" ");
											buffer.Append(((CommonTree)op.Tree).Text ); 
											if(options.SpaceBetweenOperators) buffer.Append(" "); 
										}
			additiveExpression
		)*
	;

shiftOperator
	:	SL | SR | BSR
	;

// binary addition/subtraction (level 3)
additiveExpression
	:	multiplicativeExpression
		(
			op=additiveOperator^ 		{ 
											if(options.SpaceBetweenOperators) buffer.Append(" ");
											buffer.Append(((CommonTree)op.Tree).Text ); 
											if(options.SpaceBetweenOperators) buffer.Append(" "); 
										}
			multiplicativeExpression
		)*
	;

additiveOperator
	:	PLUS | MINUS
	;

// multiplication/division/modulo (level 2)
multiplicativeExpression
	:	unaryExpression
		(	
			op=multiplicativeOperator^	{ 
											if(options.SpaceBetweenOperators) buffer.Append(" ");
											buffer.Append(((CommonTree)op.Tree).Text ); 
											if(options.SpaceBetweenOperators) buffer.Append(" "); 
										}
			unaryExpression
		)*
	;

multiplicativeOperator
	:	STAR | DIV | MOD
	;

//	(level 1)
unaryExpression
	:	iin=INC		{ buffer.Append(iin.Text); }	unaryExpression		-> ^(PRE_INC[$iin] unaryExpression)
	|	dde=DEC		{ buffer.Append(dde.Text); }	unaryExpression		-> ^(PRE_DEC[$dde] unaryExpression)
	|	tmin=MINUS	{ buffer.Append(tmin.Text);  }	unaryExpression		-> ^(UNARY_MINUS unaryExpression)
	|	tplus=PLUS	{ buffer.Append(tplus.Text); }	unaryExpression		-> ^(UNARY_PLUS unaryExpression)
	|	unaryExpressionNotPlusMinus
	;

unaryExpressionNotPlusMinus
	:	tk1=DELETE 	{ buffer.Append(tk1.Text + " ");  }	postfixExpression	-> ^(DELETE postfixExpression)
	|	tk2=VOID	{ buffer.Append(tk2.Text + " ");  }	unaryExpression	-> ^(VOID unaryExpression)
	|	tk3=TYPEOF	{ buffer.Append(tk3.Text + " ");  }	unaryExpression	-> ^(TYPEOF unaryExpression)
	|	tk4=LNOT	{ buffer.Append(tk4.Text + " ");  }	unaryExpression	-> ^(LNOT unaryExpression)
	|	tk5=BNOT	{ buffer.Append(tk5.Text + " ");  }	unaryExpression	-> ^(BNOT unaryExpression)
	|	postfixExpression
	;

// qualified names, array expressions, method invocation, post inc/dec
postfixExpression
	:	(primaryExpression -> primaryExpression)
		(	poi=propOrIdent[root_0, retval.start] { } -> $poi
		|	LBRACK { buffer.Append("["); } expression RBRACK { buffer.Append("]"); } -> ^(ARRAY_ACC $postfixExpression expression)
		|	arguments -> ^(METHOD_CALL $postfixExpression arguments)
		)*

		( 	iin=INC { buffer.Append(iin.Text); } -> ^(POST_INC[$iin] $postfixExpression)
	 	|	dde=DEC { buffer.Append(dde.Text); } -> ^(POST_DEC[$dde] $postfixExpression)
		)?
 	;

primaryExpression
	:	und=UNDEFINED				{ buffer.Append(und.Text); }
	|	c=constant					{ buffer.Append(((CommonTree)c.Tree).Text); }
	|	arrayLiteral
	|	objectLiteral
	|	functionDefinition
	|	newFullExpression
	|	encapsulatedExpression
	|	qualifiedIdent
	;


propOrIdent[CommonTree identPrimary, IToken startToken]
	:	
								{ retval.start = startToken; }
		DOT						{ buffer.Append("."); }
		propId=qualifiedIdent
		-> ^(PROPERTY_OR_IDENTIFIER {$identPrimary} $propId)
	;

constant
	:	number
	|	STRING_LITERAL
	|	TRUE
	|	FALSE
	|	NULL
	;

number	
	:	HEX_LITERAL
	|	DECIMAL_LITERAL
	|	OCTAL_LITERAL
	|	FLOAT_LITERAL
	;

	
newFullExpression
	:	n=NEW^					{ buffer.Append(n.Text + " "); }
		fullNewSubexpression	{ }
		arguments				{ }
	;

fullNewSubexpression
	:	(	primaryExpression -> primaryExpression
		)
		(	DOT { buffer.Append("."); }	 qualifiedIdent	-> ^(DOT $fullNewSubexpression qualifiedIdent)
		|	brackets				{ } -> ^(ARRAY_ACC $fullNewSubexpression brackets)
		)*
	;	


/** comments **/

// single line comment should be threated like a statement
// multiline comment can be found everywhere


// any comment
comments
	:	comment+	-> ^(COMMENT_LIST comment+)
	;

comment
	:	singleCommentStatement comment*	-> ^(COMMENT_ENTRY singleCommentStatement) comment*
	|	multiCommentStatement  comment*	-> ^(COMMENT_ENTRY multiCommentStatement) comment*
	;

singleCommentStatement
	:	SL_COMMENT -> ^(SINGLELINE_COMMENT SL_COMMENT)
	;
	
multiCommentStatement
	:	ML_COMMENT	-> ^(MULTILINE_COMMENT ML_COMMENT)
	;

/** end comments **/


brackets
@init {
	$InOperator::allowed = true;
}
	:	LBRACK			{ buffer.Append("["); }
		expressionList 
		RBRACK			{ buffer.Append("]"); }
	;

encapsulatedExpression
scope InOperator;
@init {
	$InOperator::allowed = true;
}
	:	LPAREN					{ buffer.Append("("); }
		assignmentExpression 
		RPAREN					{ buffer.Append(")"); }
		-> ^(ENCPS_EXPR assignmentExpression)
	;


functionDefinition
	:	f=FUNCTION { buffer.Append(f.Text + (options.SpaceBeforeMethodDef ? " " : "")); } 
		parameterDeclarationList 
		type_exp=typeExpression?
									{
										if(options.NewlineAfterMethod) buffer.Append(NewLine + tab);
										buffer.Append("{");
										CurrentTab++;
									}
		block
									{
										CurrentTab--;
										buffer.Append(NewLine + tab);
										buffer.Append("}");
										//buffer.Append(NewLine + tab);
									}
		-> ^(FUNC_DEF parameterDeclarationList typeExpression? block)
	;


ident
	:	IDENT
	|	i=USE -> IDENT[$i]
	|	i=XML -> IDENT[$i]
	|	i=DYNAMIC -> IDENT[$i]
	|	i=IS -> IDENT[$i]
	|	i=AS -> IDENT[$i]
	|	i=GET -> IDENT[$i]
	|	i=SET -> IDENT[$i]
	;
	


PACKAGE		:	'package';
PUBLIC		:	'public';
PRIVATE		:	'private';
PROTECTED	:	'protected';
INTERNAL	:	'internal';
FUNCTION	:	'function';
EXTENDS		:	'extends';
IMPLEMENTS	:	'implements';
VAR			:	'var';
STATIC		:	'static';
IF			:	'if';
IMPORT		:	'import';
FOR			:	'for';
EACH		:	'each';
IN			:	'in';
WHILE		:	'while';
DO			:	'do';
SWITCH		:	'switch';
CASE		:	'case';
DEFAULT		:	'default';
ELSE		:	'else';
CONST		:	'const';
CLASS		:	'class';
INTERFACE	:	'interface';
TRUE		:	'true';
FALSE		:	'false';
DYNAMIC		:	'dynamic';
USE			:	'use';
XML			:	'xml';
IS			:	'is';
AS			:	'as';
GET			:	'get';
SET			:	'set';
WITH		:	'with';
RETURN		:	'return';
CONTINUE	:	'continue';
BREAK		:	'break';
NULL		:	'null';
UNDEFINED   :   'undefined';
NEW			:	'new';
INSTANCEOF	: 'instanceof';
DELETE		: 'delete';
TYPEOF		: 'typeof';

// OPERATORS
QUESTION		:	'?'	;
LPAREN			:	'('	;
RPAREN			:	')'	;
LBRACK			:	'['	;
RBRACK			:	']'	;
LCURLY			:	'{'	;
RCURLY			:	'}'	;
COLON			:	':'	;
DBL_COLON		:	'::'	;
COMMA			:	','	;
ASSIGN			:	'='	;
EQUAL			:	'=='	;
STRICT_EQUAL	:	'==='	;
LNOT			:	'!'	;
BNOT			:	'~'	;
NOT_EQUAL		:	'!='	;
STRICT_NOT_EQUAL:	'!=='	;
DIV				:	'/'	;
DIV_ASSIGN		:	'/='	;
PLUS			:	'+'	;
PLUS_ASSIGN		:	'+='	;
INC				:	'++'	;
MINUS			:	'-'	;
MINUS_ASSIGN	:	'-='	;
DEC				:	'--'	;
STAR			:	'*'	;
STAR_ASSIGN		:	'*='	;
MOD				:	'%'	;
MOD_ASSIGN		:	'%='	;
SR				:	'>>'	;
SR_ASSIGN		:	'>>='	;
BSR				:	'>>>'	;
BSR_ASSIGN		:	'>>>='	;
GE				:	'>='	;
GT				:	'>'	;
SL				:	'<<'	;
SL_ASSIGN		:	'<<='	;
LE				:	'<='	;
LT				:	'<'	;
BXOR			:	'^'	;
BXOR_ASSIGN		:	'^='	;
BOR				:	'|'	;
BOR_ASSIGN		:	'|='	;
LOR				:	'||'	;
BAND			:	'&'	;
BAND_ASSIGN		:	'&='	;
LAND			:	'&&'	;
LAND_ASSIGN		:	'&&='	;
LOR_ASSIGN		:	'||='	;
E4X_ATTRI		:	'@'	; 
SEMI			:	';'	;
DOT				:	'.'	;
E4X_DESC		:	'..'	;
REST			:	'...'	;
VOID			:	'Void'	;

IDENT
	:	('A'..'Z' | 'a'..'z' | '_' | '$') ('a'..'z'|'A'..'Z'|'_'|'0'..'9'|'$')*
	;

STRING_LITERAL
	:	'"' (ESC|~('"'|'\\'|'\n'|'\r'))* '"'
	|	'\'' (ESC|~('\''|'\\'|'\n'|'\r'))* '\''
	;

HEX_LITERAL	:	'0' ('x'|'X') HEX_DIGIT+ ;

DECIMAL_LITERAL	:	('0' | '1'..'9' '0'..'9'*) ;

OCTAL_LITERAL	:	'0' ('0'..'7')+ ;

FLOAT_LITERAL
    :   ('0'..'9')+ '.' ('0'..'9')* EXPONENT?
    |   '.' ('0'..'9')+ EXPONENT?
	;


// whitespace -- ignored
WS	:	(
			' '
		|	'\t'
		|	'\f'
		)+
		{$channel=HIDDEN;}
	;
NL	
	:	(
			'\r' '\n'  	// DOS
		|	'\r'    	// Mac
		|	'\n'    	// Unix
		)
		{$channel=HIDDEN;}
	;
	
// skip BOM bytes
BOM	:	(	'\u00EF'  '\u00BB' '\u00BF'
		|	'\uFEFF'
		)
		{ $channel=HIDDEN; };

// might be better to filter this out as a preprocessing step
INCLUDE_DIRECTIVE
	:	'#include'
	;

// single-line comments
SL_COMMENT
	:	'//' (~('\n'|'\r'))* ('\n'|'\r'('\n')?)?
		{ 
			//$channel=HIDDEN;
		}
	;
// multiple-line comments
ML_COMMENT
@after {
}
	:	'/*' ( options {greedy=false;} : . )* '*/'
		{ 
			//$channel = HIDDEN;
		}
	;

fragment EXPONENT
	:	('e'|'E') ('+'|'-')? ('0'..'9')+
	;
fragment HEX_DIGIT
	:	('0'..'9'|'A'..'F'|'a'..'f')
	;

fragment OCT_DIGIT
	:	'0'..'7'
	;
	
fragment ESC
	:   CTRLCHAR_ESC
	|   UNICODE_ESC
	|   OCTAL_ESC
	;

fragment CTRLCHAR_ESC
	:	'\\' ('b'|'t'|'n'|'f'|'r'|'\"'|'\''|'\\')
	;

fragment OCTAL_ESC
	:   '\\' ('0'..'3') ('0'..'7') ('0'..'7')
	|   '\\' ('0'..'7') ('0'..'7')
	|   '\\' ('0'..'7')
	;

fragment UNICODE_ESC
	:   '\\' 'u' HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT
	;
