lexer grammar ConventionalCommitLexer;

options {
  caseInsensitive=true;
}

fragment Cr: '\r';
fragment Lf: '\n';

Feat: 'feat' | 'feature' 's'?;
Fix: 'fix' | 'fixing' | 'fixes';
Docs: 'doc' | 'docs' | 'document' | 'documentation' ;
Style: 'style';
Refactor: 'refact' | 'refactor' | 'refactoring';
Perf: 'perf' | 'performance';
Test: 'test' 's'? | 'testing';
Chore: 'chore' 's'?;
Build: 'build';
Ci: 'ci' | 'cicd' | 'ci-cd';
Breaking: 'break' | 'breaking';
Security: 'security';
Revert: 'revert' | 'reverts';
Config: 'config' | 'configs';
Upgrade: 'upgrade' 's'? | 'upgrading' |;
Downgrade: 'downgrade' 's'? | 'downgrading';
Pin: 'pin' 's'? | 'pinning';

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