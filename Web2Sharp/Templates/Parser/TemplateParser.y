
%YYSTYPE string
%visibility internal
%partial
%namespace Web2Sharp.Templates.Parser
%output=TemplateParser.cs

%token TEXT
%token TAG_START_BLOCK
%token TAG_START_ENDBLOCK
%token TAG_START_IF
%token TAG_START_ENDIF
%token TAG_START_ELSE
%token TAG_START_FOR
%token TAG_START_ENDFOR
%token TAG_START_USING
%token TAG_START_CONTEXT
%token TAG_START_EXTENDS
%token TAG_END
%token KEY_IN
%token VALUE_START
%token VALUE_END

%start root

%%

root: content_opt
  {
    this.ResultText = $1;
  }
  ;

content_opt:
  |  /* empty */
  content_list
  ;

content_list:
  content
  | content_list content
    {
      $$ = $1 + "\n" + $2;
    }
  ;

content
  : text
   {
    $$ = "output.Append(\""+EscapeText($1)+"\");";
   }
  | tag
  ;

tag
 : tag_value
 | tag_block
 | tag_if
 | tag_if_else
 | tag_for
 | tag_using
 | tag_context
 | tag_extends
 ;

tag_value: VALUE_START expr VALUE_END
  {
   $$ = "output.Append(" + $2 + ");";
  }
  ;

tag_block: TAG_START_BLOCK varname TAG_END content_opt TAG_START_ENDBLOCK TAG_END
  {
    ResultBlocks[$2] = $4;
    $$ = "output.Append(blockRenderers[\"" + $2 + "\"](context, blockRenderers));";
  }
  ;

tag_if: TAG_START_IF expr TAG_END content_opt TAG_START_ENDIF TAG_END
  {
   $$ = "if( "+$2+" )\n{\n" + $4 + "\n}";
  }
  ;

tag_if_else: TAG_START_IF expr TAG_END content_opt  TAG_START_ELSE TAG_END content_opt TAG_START_ENDIF TAG_END
{
 $$ = "if( "+$2+" )\n{\n" + $4 + "\n}\nelse\n{\n" + $7 + "\n}";
}
;
tag_for: TAG_START_FOR varname KEY_IN expr TAG_END content_opt TAG_START_ENDFOR TAG_END
  {
   $$ = "foreach( var " + $2 + " in (" + $4 + ") )" + "\n{\n" + $6 + "\n}\n";
  }
  ;

tag_using: TAG_START_USING expr TAG_END
  {
    ResultUsings.Add($2);
    $$ = "";
  }
  ;

tag_context: TAG_START_CONTEXT expr TAG_END
  {
    ResultContextType = $2;
    $$ = "";
  }
  ;

tag_extends: TAG_START_EXTENDS expr TAG_END
  {
    ResultExtends = $2;
    $$ = "";
  }
  ;

varname: expr;
expr:
 TEXT
  {
    $$ = $1;
  }
 | expr TEXT
  {
    $$ = $1 + $2;
  }
 ;

text:
 TEXT
  {
    $$ = $1;
  }
 | text TEXT
  {
    $$ = $1 + $2;
  }
 ;

%%
