parser grammar ConventionalCommitParser;
options { tokenVocab = ConventionalCommitLexer; }

type: value = Identifier      #OtherType
    | value =
      (   Feat
        | Fix
        | Docs
        | Style
        | Refactor
        | Perf
        | Test
        | Chore
        | Build
        | Ci
        | Breaking
        | Security
        | Revert
        | Config
        | Upgrade
        | Downgrade
        | Pin)               #RecorgnizedType
    ;

scope: LParen value = Identifier RParen;

description: textLine;

nextSection: Whitespace* Newline (Whitespace* Newline)+;

textLine: firstWord = Whitespace* Word (Whitespace+ restOfWords += Word)* Whitespace*;
body:  textLine (Newline textLine)*;

footerTuple: Whitespace* key = Word Whitespace* Colon Whitespace* value = textLine;
footer: footerTuple (Newline footerTuple)* (Newline | Whitespace)*;

commitMessage: 
    Whitespace* type Whitespace* 
    scope? Whitespace* 
    isBreaking = ExclamationMark? 
    Whitespace* Colon 
    Whitespace* description 
    (nextSection body?)? 
    (nextSection footer?)? EOF;