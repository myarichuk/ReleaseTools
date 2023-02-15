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

textLine: firstWord = Whitespace* Word (Whitespace+ restOfWords += Word)* Whitespace*;
body: Whitespace* Newline (Whitespace* Newline)+ textLine (Newline textLine)*;

footerTuple: key = Whitespace* Word Whitespace* Colon Whitespace* value = textLine Whitespace*;
footer: Whitespace* Newline (Whitespace* Newline)+ footerTuple (Newline footerTuple)* (Newline | Whitespace)*;

commitMessage: type scope? isBreaking = ExclamationMark? Colon description body? footer? EOF;