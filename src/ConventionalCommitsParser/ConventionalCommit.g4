grammar ConventionalCommit;

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

COMMIT_TYPE: ('feat' | 'fix' | 'docs' | 'style' | 'refactor' | 'perf' | 'test' | 'chore' | 'revert' | 'config' | 'upgrade' | 'downgrade' | 'pin') WHITESPACE*;
SYMBOL: ["#$%&'*+,-./;<=>?@[\\\]^_`{|}~];
IDENTIFIER: (LETTER | NUMBER | SYMBOL) (LETTER | NUMBER | '-')*;
SENTENCE: (LETTER | NUMBER | SYMBOL) (LETTER | NUMBER | WHITESPACE | SYMBOL)*;
SECTION_SEPARATOR: (NEWLINE ('-' '-' '-'+) NEWLINE) | (NEWLINE NEWLINE+);
SEMICOLON: ':';

commitMessage: commitType commitScope? semicolon description (footer|body? footer?) EOF;

semicolon: SEMICOLON;
description:  WHITESPACE* value = (IDENTIFIER | SENTENCE);
commitType: value = COMMIT_TYPE isBreaking = EXCLAMATION?;
commitScope: LPAREN value = IDENTIFIER RPAREN isBreaking = EXCLAMATION? WHITESPACE*;

body: SECTION_SEPARATOR values += bodyLine* EOF?;
bodyLine: value = (IDENTIFIER | SENTENCE) NEWLINE?;

footer: SECTION_SEPARATOR values += footerItem+ EOF?;

footerItem: footerKey semicolon footerValue (NEWLINE+| EOF?);
footerValue: value = (IDENTIFIER | SENTENCE);
footerKey: value = (BREAKING_CHANGE | IDENTIFIER);