from sly import Lexer

class GemLexer(Lexer):
    tokens = { NAME, NUMBER, STRING, IF, THEN, ELSE, FOR, PUBLIC, HIDDEN, END, RETURN, TO, EQEQ }
    ignore = '\t '

    literals = { '=', '+', '-', '/', '*', '(', ')', '{', '}', '[', ']', ',', ';', '.' }
    
    IF = r'if'
    THEN = r'then'
    ELSE = r'else'
    FOR = r'for'
    PUBLIC = r'public'
    HIDDEN = r'hidden'
    END = 'end'
    RETURN = 'return'
    TO = 'to'
    NAME = r'[a-zA-Z_][a-zA-Z0-9_]*'
    STRING = r'\".*?\"'
    EQEQ = r'=='
    
    @_(r'\d+')
    def NUMBER(self, t):
        t.value = int(t.value)
    
    @_(r'\/\/.*')
    def COMMENT(self, t):
        pass

    @_(r'\n+')
    def newline(self, t):
        self.lineno = t.value.count('\n')

if __name__ == '__main__':
    lexer = GemLexer()
    env = {}
    while True:
        try:
            text = input('gem > ')
        except EOFError:
            break
        if text:
            lex = lexer.tokenize(text)
            for token in lex:
                print(token)
                    
