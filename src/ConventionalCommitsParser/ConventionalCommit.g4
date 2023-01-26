grammar ConventionalCommit;

LPAREN: '(';
RPAREN: ')';

SEMICOLON: ':';

SEPARATOR: SEMICOLON | ' #';
BREAKING_CHANGE: 'BREAKING CHANGE' | 'BREAKING-CHANGE';

fragment ZWNBSP: '\uFEFF';
fragment FF: '\u000C';
fragment NBSP: '\u00A0';
fragment SP: '\u0020';
fragment ESP: '\u2000'..'\u200A';
fragment NNBSP: '\u202F';
fragment MMSP: '\u205F';
fragment IS: '\u3000';
fragment TAB_ASCII: '\t';
fragment TAB_UNICODE: '\u0009';
fragment TAB: TAB_ASCII | TAB_UNICODE;
fragment WSP: ' ';



EXCLAMATION: '!';

WHITESPACE: WSP
    | ZWNBSP
    | FF
    | NBSP
    | SP
    | TAB
    | ESP
    | NNBSP
    | MMSP
    | IS
    ;
NEWLINE: CR? LF;

fragment CR_ASCII: '\r';
fragment CR_UNICODE: '\u000D';
fragment CR: CR_ASCII | CR_UNICODE;
fragment LF_ASCII: '\n';
fragment LF_UNICODE: '\u000A';
fragment LF: LF_ASCII | LF_UNICODE;

fragment NUMBER: [0-9];
fragment LETTER: [A-Za-z\u0080-\uFFFF];
LETTER_OR_NUMBER: LETTER | NUMBER;
IDENTIFIER: LETTER LETTER_OR_NUMBER*;
CHARACTER: (LETTER | NUMBER | WHITESPACE);

fragment TEXT: CHARACTER*;
SINGLE_LINE_TEXT: {_input.La(0) != NEWLINE}? TEXT (NEWLINE | EOF);

MULTILINE_TEXT: (CHARACTER | NEWLINE)* (NEWLINE NEWLINE+);

commitMessage: identifier scope? semicolon description body? EOF;

semicolon: WHITESPACE* SEMICOLON WHITESPACE*;

description: value = SINGLE_LINE_TEXT { _input.La(1) == NEWLINE || _input.La(1) == EOF }?;
identifier: WHITESPACE* value = IDENTIFIER WHITESPACE*;
scope: WHITESPACE* LPAREN value = identifier RPAREN WHITESPACE*;
body: WHITESPACE* MULTILINE_TEXT WHITESPACE*;