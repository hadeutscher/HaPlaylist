grammar HaGrammar;

// Parser rules
query : sequenceExpression EOF;
sequenceExpression :
            ratioExpression
            (THEN ratioExpression)*;
ratioExpression : 
            ( booleanExpression 
            | (NUMBER PART booleanExpression)+);
booleanExpression : 
           functionExpression
           | NOT booleanExpression
           | booleanExpression AND booleanExpression
           | booleanExpression OR booleanExpression
           | OPEN booleanExpression CLOSE
           | booleanExpression AND
           | TRUE
           | ID;
functionExpression : string IN ID;
string : ID | TEXT;

// Lexer rules
NOT : ('not ' | '!');
AND : (' and ' | ' ');
OR : ' or ';
OPEN : '(';
CLOSE : ')';
IN : ' in ';
PART : (' part ' | ' parts ');
THEN : ' then ';
TRUE : 'true';
NUMBER : ([0-9])+;
ID : ([a-z] | [A-Z] | [0-9] | '_')+;
TEXT : STRING_MARKER ([a-z] | [A-Z] | [0-9] | '_' | ' ')+ STRING_MARKER;
STRING_MARKER : '"';
SPACE : ' ' -> skip;
