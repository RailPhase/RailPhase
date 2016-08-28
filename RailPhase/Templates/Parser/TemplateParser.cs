// This code was generated by the Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, John Gough, QUT 2005-2014
// (see accompanying GPPGcopyright.rtf)

// GPPG version 1.5.2
// Machine:  golbat
// DateTime: 28.08.2016 14:20:16
// UserName: lukas
// Input file <TemplateParser.y - 28.08.2016 14:20:15>

// options: lines

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Text;
using QUT.Gppg;

namespace RailPhase.TemplateParser
{
internal enum Tokens {error=2,EOF=3,TEXT=4,TAG_START_BLOCK=5,TAG_START_ENDBLOCK=6,
    TAG_START_IF=7,TAG_START_ENDIF=8,TAG_START_ELSE=9,TAG_START_FOR=10,TAG_START_ENDFOR=11,TAG_START_USING=12,
    TAG_START_DATA=13,TAG_START_EXTENDS=14,TAG_END=15,KEY_IN=16,VALUE_START=17,VALUE_END=18};

[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
internal partial class Parser: ShiftReduceParser<string, LexLocation>
{
#pragma warning disable 649
  private static Dictionary<int, string> aliases;
#pragma warning restore 649
  private static Rule[] rules = new Rule[30];
  private static State[] states = new State[59];
  private static string[] nonTerms = new string[] {
      "root", "$accept", "content_opt", "content_list", "content", "text", "tag", 
      "tag_value", "tag_block", "tag_if", "tag_if_else", "tag_for", "tag_using", 
      "tag_data", "tag_extends", "expr", "varname", };

  static Parser() {
    states[0] = new State(new int[]{4,8,17,11,5,17,7,25,10,38,12,47,13,51,14,55,3,-3},new int[]{-1,1,-3,3,-4,4,-5,23,-6,6,-7,9,-8,10,-9,16,-10,24,-11,36,-12,37,-13,46,-14,50,-15,54});
    states[1] = new State(new int[]{3,2});
    states[2] = new State(-1);
    states[3] = new State(-2);
    states[4] = new State(new int[]{4,8,17,11,5,17,7,25,10,38,12,47,13,51,14,55,3,-4,6,-4,8,-4,9,-4,11,-4},new int[]{-5,5,-6,6,-7,9,-8,10,-9,16,-10,24,-11,36,-12,37,-13,46,-14,50,-15,54});
    states[5] = new State(-6);
    states[6] = new State(new int[]{4,7,17,-7,5,-7,7,-7,10,-7,12,-7,13,-7,14,-7,3,-7,6,-7,8,-7,9,-7,11,-7});
    states[7] = new State(-29);
    states[8] = new State(-28);
    states[9] = new State(-8);
    states[10] = new State(-9);
    states[11] = new State(new int[]{4,15},new int[]{-16,12});
    states[12] = new State(new int[]{18,13,4,14});
    states[13] = new State(-17);
    states[14] = new State(-27);
    states[15] = new State(-26);
    states[16] = new State(-10);
    states[17] = new State(new int[]{4,15},new int[]{-17,18,-16,58});
    states[18] = new State(new int[]{15,19});
    states[19] = new State(new int[]{4,8,17,11,5,17,7,25,10,38,12,47,13,51,14,55,6,-3},new int[]{-3,20,-4,4,-5,23,-6,6,-7,9,-8,10,-9,16,-10,24,-11,36,-12,37,-13,46,-14,50,-15,54});
    states[20] = new State(new int[]{6,21});
    states[21] = new State(new int[]{15,22});
    states[22] = new State(-18);
    states[23] = new State(-5);
    states[24] = new State(-11);
    states[25] = new State(new int[]{4,15},new int[]{-16,26});
    states[26] = new State(new int[]{15,27,4,14});
    states[27] = new State(new int[]{4,8,17,11,5,17,7,25,10,38,12,47,13,51,14,55,8,-3,9,-3},new int[]{-3,28,-4,4,-5,23,-6,6,-7,9,-8,10,-9,16,-10,24,-11,36,-12,37,-13,46,-14,50,-15,54});
    states[28] = new State(new int[]{8,29,9,31});
    states[29] = new State(new int[]{15,30});
    states[30] = new State(-19);
    states[31] = new State(new int[]{15,32});
    states[32] = new State(new int[]{4,8,17,11,5,17,7,25,10,38,12,47,13,51,14,55,8,-3},new int[]{-3,33,-4,4,-5,23,-6,6,-7,9,-8,10,-9,16,-10,24,-11,36,-12,37,-13,46,-14,50,-15,54});
    states[33] = new State(new int[]{8,34});
    states[34] = new State(new int[]{15,35});
    states[35] = new State(-20);
    states[36] = new State(-12);
    states[37] = new State(-13);
    states[38] = new State(new int[]{4,15},new int[]{-17,39,-16,58});
    states[39] = new State(new int[]{16,40});
    states[40] = new State(new int[]{4,15},new int[]{-16,41});
    states[41] = new State(new int[]{15,42,4,14});
    states[42] = new State(new int[]{4,8,17,11,5,17,7,25,10,38,12,47,13,51,14,55,11,-3},new int[]{-3,43,-4,4,-5,23,-6,6,-7,9,-8,10,-9,16,-10,24,-11,36,-12,37,-13,46,-14,50,-15,54});
    states[43] = new State(new int[]{11,44});
    states[44] = new State(new int[]{15,45});
    states[45] = new State(-21);
    states[46] = new State(-14);
    states[47] = new State(new int[]{4,15},new int[]{-16,48});
    states[48] = new State(new int[]{15,49,4,14});
    states[49] = new State(-22);
    states[50] = new State(-15);
    states[51] = new State(new int[]{4,15},new int[]{-16,52});
    states[52] = new State(new int[]{15,53,4,14});
    states[53] = new State(-23);
    states[54] = new State(-16);
    states[55] = new State(new int[]{4,15},new int[]{-16,56});
    states[56] = new State(new int[]{15,57,4,14});
    states[57] = new State(-24);
    states[58] = new State(new int[]{4,14,15,-25,16,-25});

    for (int sNo = 0; sNo < states.Length; sNo++) states[sNo].number = sNo;

    rules[1] = new Rule(-2, new int[]{-1,3});
    rules[2] = new Rule(-1, new int[]{-3});
    rules[3] = new Rule(-3, new int[]{});
    rules[4] = new Rule(-3, new int[]{-4});
    rules[5] = new Rule(-4, new int[]{-5});
    rules[6] = new Rule(-4, new int[]{-4,-5});
    rules[7] = new Rule(-5, new int[]{-6});
    rules[8] = new Rule(-5, new int[]{-7});
    rules[9] = new Rule(-7, new int[]{-8});
    rules[10] = new Rule(-7, new int[]{-9});
    rules[11] = new Rule(-7, new int[]{-10});
    rules[12] = new Rule(-7, new int[]{-11});
    rules[13] = new Rule(-7, new int[]{-12});
    rules[14] = new Rule(-7, new int[]{-13});
    rules[15] = new Rule(-7, new int[]{-14});
    rules[16] = new Rule(-7, new int[]{-15});
    rules[17] = new Rule(-8, new int[]{17,-16,18});
    rules[18] = new Rule(-9, new int[]{5,-17,15,-3,6,15});
    rules[19] = new Rule(-10, new int[]{7,-16,15,-3,8,15});
    rules[20] = new Rule(-11, new int[]{7,-16,15,-3,9,15,-3,8,15});
    rules[21] = new Rule(-12, new int[]{10,-17,16,-16,15,-3,11,15});
    rules[22] = new Rule(-13, new int[]{12,-16,15});
    rules[23] = new Rule(-14, new int[]{13,-16,15});
    rules[24] = new Rule(-15, new int[]{14,-16,15});
    rules[25] = new Rule(-17, new int[]{-16});
    rules[26] = new Rule(-16, new int[]{4});
    rules[27] = new Rule(-16, new int[]{-16,4});
    rules[28] = new Rule(-6, new int[]{4});
    rules[29] = new Rule(-6, new int[]{-6,4});
  }

  protected override void Initialize() {
    this.InitSpecialTokens((int)Tokens.error, (int)Tokens.EOF);
    this.InitStates(states);
    this.InitRules(rules);
    this.InitNonTerminals(nonTerms);
  }

  protected override void DoAction(int action)
  {
#pragma warning disable 162, 1522
    switch (action)
    {
      case 2: // root -> content_opt
#line 29 "TemplateParser.y"
  {
    this.ResultText = ValueStack[ValueStack.Depth-1];
  }
#line default
        break;
      case 6: // content_list -> content_list, content
#line 42 "TemplateParser.y"
    {
      CurrentSemanticValue = ValueStack[ValueStack.Depth-2] + "\n" + ValueStack[ValueStack.Depth-1];
    }
#line default
        break;
      case 7: // content -> text
#line 49 "TemplateParser.y"
   {
    CurrentSemanticValue = "output.Append(\""+EscapeText(ValueStack[ValueStack.Depth-1])+"\");";
   }
#line default
        break;
      case 17: // tag_value -> VALUE_START, expr, VALUE_END
#line 67 "TemplateParser.y"
  {
   CurrentSemanticValue = "output.Append(" + ValueStack[ValueStack.Depth-2] + ");";
  }
#line default
        break;
      case 18: // tag_block -> TAG_START_BLOCK, varname, TAG_END, content_opt, TAG_START_ENDBLOCK, 
               //              TAG_END
#line 73 "TemplateParser.y"
  {
    ResultBlocks[ValueStack[ValueStack.Depth-5]] = ValueStack[ValueStack.Depth-3];
    CurrentSemanticValue = "output.Append(blockRenderers[\"" + ValueStack[ValueStack.Depth-5] + "\"](Data, Context, blockRenderers));";
  }
#line default
        break;
      case 19: // tag_if -> TAG_START_IF, expr, TAG_END, content_opt, TAG_START_ENDIF, TAG_END
#line 80 "TemplateParser.y"
  {
   CurrentSemanticValue = "if( "+ValueStack[ValueStack.Depth-5]+" )\n{\n" + ValueStack[ValueStack.Depth-3] + "\n}";
  }
#line default
        break;
      case 20: // tag_if_else -> TAG_START_IF, expr, TAG_END, content_opt, TAG_START_ELSE, 
               //                TAG_END, content_opt, TAG_START_ENDIF, TAG_END
#line 86 "TemplateParser.y"
{
 CurrentSemanticValue = "if( "+ValueStack[ValueStack.Depth-8]+" )\n{\n" + ValueStack[ValueStack.Depth-6] + "\n}\nelse\n{\n" + ValueStack[ValueStack.Depth-3] + "\n}";
}
#line default
        break;
      case 21: // tag_for -> TAG_START_FOR, varname, KEY_IN, expr, TAG_END, content_opt, 
               //            TAG_START_ENDFOR, TAG_END
#line 91 "TemplateParser.y"
  {
   CurrentSemanticValue = "foreach( var " + ValueStack[ValueStack.Depth-7] + " in (" + ValueStack[ValueStack.Depth-5] + ") )" + "\n{\n" + ValueStack[ValueStack.Depth-3] + "\n}\n";
  }
#line default
        break;
      case 22: // tag_using -> TAG_START_USING, expr, TAG_END
#line 97 "TemplateParser.y"
  {
    ResultUsings.Add(ValueStack[ValueStack.Depth-2]);
    CurrentSemanticValue = "";
  }
#line default
        break;
      case 23: // tag_data -> TAG_START_DATA, expr, TAG_END
#line 104 "TemplateParser.y"
  {
    ResultDataType = ValueStack[ValueStack.Depth-2];
    CurrentSemanticValue = "";
  }
#line default
        break;
      case 24: // tag_extends -> TAG_START_EXTENDS, expr, TAG_END
#line 111 "TemplateParser.y"
  {
    ResultExtends = ValueStack[ValueStack.Depth-2];
    CurrentSemanticValue = "";
  }
#line default
        break;
      case 26: // expr -> TEXT
#line 120 "TemplateParser.y"
  {
    CurrentSemanticValue = ValueStack[ValueStack.Depth-1];
  }
#line default
        break;
      case 27: // expr -> expr, TEXT
#line 124 "TemplateParser.y"
  {
    CurrentSemanticValue = ValueStack[ValueStack.Depth-2] + ValueStack[ValueStack.Depth-1];
  }
#line default
        break;
      case 28: // text -> TEXT
#line 131 "TemplateParser.y"
  {
    CurrentSemanticValue = ValueStack[ValueStack.Depth-1];
  }
#line default
        break;
      case 29: // text -> text, TEXT
#line 135 "TemplateParser.y"
  {
    CurrentSemanticValue = ValueStack[ValueStack.Depth-2] + ValueStack[ValueStack.Depth-1];
  }
#line default
        break;
    }
#pragma warning restore 162, 1522
  }

  protected override string TerminalToString(int terminal)
  {
    if (aliases != null && aliases.ContainsKey(terminal))
        return aliases[terminal];
    else if (((Tokens)terminal).ToString() != terminal.ToString(CultureInfo.InvariantCulture))
        return ((Tokens)terminal).ToString();
    else
        return CharToString((char)terminal);
  }

#line 141 "TemplateParser.y"
#line default
}
}
