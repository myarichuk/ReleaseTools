grammar ConventionalCommit;

fragment A : [aA];
fragment B : [bB];
fragment C : [cC];
fragment D : [dD];
fragment E : [eE];
fragment F : [fF];
fragment G : [gG];
fragment H : [hH];
fragment I : [iI];
fragment J : [jJ];
fragment K : [kK];
fragment L : [lL];
fragment M : [mM];
fragment N : [nN];
fragment O : [oO];
fragment P : [pP];
fragment Q : [qQ];
fragment R : [rR];
fragment S : [sS];
fragment T : [tT];
fragment U : [uU];
fragment V : [vV];
fragment W : [wW];
fragment X : [xX];
fragment Y : [yY];
fragment Z : [zZ];

WHITESPACE: [ \t\r] -> skip;
LPAREN: '(';
RPAREN: ')';

EXCLAMATION: '!';
BREAKING_CHANGE: 'BREAKING CHANGE' | 'BREAKING-CHANGE';

fragment CR_ASCII: '\r';
fragment CR_UNICODE: '\u000D';
fragment CR: CR_ASCII | CR_UNICODE;
fragment LF_ASCII: '\n';
fragment LF_UNICODE: '\u000A';
fragment LF: LF_ASCII | LF_UNICODE;

NEWLINE: CR? LF;

fragment NUMBER: [0-9];
fragment LETTER: [A-Za-z\u0080-\uFFFF];

COMMIT_TYPE: (F E A T | F I X | D O C S | S T Y L E | R E F A C T O R | P E R F | T E S T | C H O R E | R E V E R T | C O N F I G | U P G R A D E | D O W N G R A D E | P I N) WHITESPACE*;

SYMBOL: ["#$%&'*+,-./;<=>?@[\\\]^_`{|}~];

IDENTIFIER: (LETTER | NUMBER | SYMBOL) (LETTER | NUMBER | '-')*;

SENTENCE: (LETTER | NUMBER | SYMBOL) (LETTER | NUMBER | WHITESPACE | SYMBOL)*;

SECTION_SEPARATOR: NEWLINE NEWLINE+;

SEMICOLON: ':';

commitMessage: commitType commitScope? semicolon description (footer|body? footer?) EOF;

semicolon: SEMICOLON;

description:  WHITESPACE* value = (IDENTIFIER | SENTENCE);

commitType: value = COMMIT_TYPE isBreaking = EXCLAMATION?;

commitScope: LPAREN value = (IDENTIFIER | SENTENCE) RPAREN isBreaking = EXCLAMATION? WHITESPACE*;

body: SECTION_SEPARATOR values += bodyLine* EOF?;

bodyLine: value = (IDENTIFIER | SENTENCE) NEWLINE?;

footer: SECTION_SEPARATOR values += footerItem+ (SECTION_SEPARATOR* | NEWLINE*)? EOF?;

footerItem: footerKey semicolon footerValue (NEWLINE+| EOF?);

footerKey: value = (BREAKING_CHANGE | IDENTIFIER);
footerValue: value = (IDENTIFIER | SENTENCE);