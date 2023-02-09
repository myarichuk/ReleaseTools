lexer grammar ConventionalCommitLexer;
options { caseInsensitive = true; }

@members{
  private enum State{
    BeforeDescription,
    Description,
    Body,
    Footer
  }

  private State _currentState = State.BeforeDescription;
}

fragment CR: '\r';
fragment LF: '\n';

//common 'type' values
FEAT: 'feat';
FIX: 'fix';
DOCS: 'doc' | 'docs';
STYLE: 'style';
REFACTOR: 'refact' | 'refactor';
PERF: 'perf' | 'performance';
TEST: 'test' | 'testing';
CHORE: 'chore';
BUILD: 'build';
CI: 'ci';
BREAKING: 'break' | 'breaking';
SECURITY: 'security';
REVERT: 'revert';
CONFIG: 'config';
UPGRADE: 'upgrade';
DOWNGRADE: 'downgrade';
PIN: 'pin';


IDENTIFIER: 
    { _currentState != State.Body && 
      _currentState != State.Description }? 
    [a-z] [a-z_-]*;

WS: (' ' | '\t') -> skip;
LPAREN: '(' -> mode(Scope);
RPAREN: ')';
COLON: ':' WS* 
{
  if(_currentState == State.BeforeDescription)
  { _currentState = State.Description; }
}
-> mode(Text);

BREAKING_CHANGE_MARK: '!';
BREAKING_CHANGE_KEY: 'BREAKING CHANGE' | 'BREAKING-CHANGE';

NEWLINE: CR? LF
{
  if (_currentState == State.Body)
  { Mode(2); }
  if (_currentState == State.Footer)
  { Mode(3); }
};


NEXT_SECTION: WS* NEWLINE (WS* NEWLINE)+ WS*
{
    if(_currentState == State.Description) 
        { _currentState = State.Body; Mode(2); }
    else if (_currentState == State.Body)
        { _currentState = State.Footer; Mode(3); }
};

mode Scope;
SCOPE_WITESPACE: ' ' -> skip;
SCOPE: [a-z] [a-z0-9_/\\]* -> type(IDENTIFIER);
END_OF_SCOPE: ')' -> mode(DEFAULT_MODE), type(RPAREN);

mode Text;
TEXT: ~([\r\n] | [\n])+ -> mode(DEFAULT_MODE);

mode Footer;
KEY: IDENTIFIER -> type(IDENTIFIER), mode(DEFAULT_MODE);
