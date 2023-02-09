parser grammar ConventionalCommitParser;
options { tokenVocab = ConventionalCommitLexer; }

type: value = IDENTIFIER      #OtherType
    | value =
      (   FEAT
        | FIX
        | DOCS
        | STYLE
        | REFACTOR
        | PERF
        | TEST
        | CHORE
        | BUILD
        | CI
        | BREAKING
        | SECURITY
        | REVERT
        | CONFIG
        | UPGRADE
        | DOWNGRADE
        | PIN)               #RecorgnizedType
    ;

footerTuple: NEWLINE key = IDENTIFIER COLON value = TEXT;
footer: NEXT_SECTION footerTuple footerTuple*;

body: NEXT_SECTION NEWLINE TEXT;

commitMessage: 
    type 
    LPAREN scope = IDENTIFIER RPAREN COLON 
    description = TEXT
    body? 
    footer? EOF;