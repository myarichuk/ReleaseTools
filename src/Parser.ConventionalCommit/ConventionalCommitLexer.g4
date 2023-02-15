lexer grammar ConventionalCommitLexer;

options {
  caseInsensitive=true;
}

fragment Cr: '\r';
fragment Lf: '\n';

Feat: 'feat';
Fix: 'fix';
Docs: 'doc' | 'docs';
Style: 'style';
Refactor: 'refact' | 'refactor';
Perf: 'perf' | 'performance';
Test: 'test' | 'testing';
Chore: 'chore';
Build: 'build';
Ci: 'ci';
Breaking: 'break' | 'breaking';
Security: 'security';
Revert: 'revert';
Config: 'config';
Upgrade: 'upgrade';
Downgrade: 'downgrade';
Pin: 'pin';

Identifier: [a-z] [a-z0-9_-]*;

ExclamationMark: '!';

Whitespace: ' ' | '\t';
LParen: Whitespace* '(' Whitespace* -> pushMode(Scope);
Colon: Whitespace* ':' Whitespace* -> pushMode(NonHeader);

Newline: Cr? Lf -> pushMode(NonHeader);

mode Scope;
Scope: Identifier -> type(Identifier);
RParen: Whitespace* ')' Whitespace* -> popMode;

mode NonHeader;
Word: ~[ :\t\n]+;
RNewline: Newline -> type(Newline);
RWhitespace: Whitespace -> type(Whitespace);
RColon: Colon -> type(Colon);