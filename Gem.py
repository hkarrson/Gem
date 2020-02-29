from sly import Lexer
from sly import Parser

class GemLexer(Lexer):
    tokens = { NAME, NUMBER, STRING, IF, THEN, ELSE, FOR, PUBLIC, HIDDEN, RETURN, TO, EQEQ }
    ignore = '\t '

    literals = { '=', '+', '-', '/', '*', '(', ')', '{', '}', '[', ']', ',', ';', '.' }
    
    IF = r'if'
    THEN = r'then'
    ELSE = r'else'
    FOR = r'for'
    PUBLIC = r'public'
    HIDDEN = r'hidden'
    RETURN = 'return'
    TO = 'to'
    NAME = r'[a-zA-Z_][a-zA-Z0-9_]*'
    STRING = r'\".*?\"'
    EQEQ = r'=='
    
    @_(r'\d+')
    def NUMBER(self, t):
        t.value = int(t.value)
        return t
    
    @_(r'\/\/.*')
    def COMMENT(self, t):
        pass

    @_(r'\n+')
    def newline(self, t):
        self.lineno = t.value.count('\n')

class GemParser(Parser):
    tokens = GemLexer.tokens

    precedence = (
        ('left', '+', '-'),
        ('left', '*', '/'),
        ('right', 'UMINUS'),
        )

    def __init__(self):
        self.env = { }

    @_('statements')
    def main(self, p):
        return ('main', p.statements)
    
    @_('"{" statements "}"')
    def block(self, p):
        return ('block', p.statements)
    
    @_('"{" "}"')
    def block(self, p):
        return ('block', ())
    
    @_('statements statement')
    def statements(self, p):
        return ('statements', p.statements[1] + (p.statement, 'next'))
    
    @_('statement')
    def statements(self, p):
        return ('statements', (p.statement, 'next'))
    
    @_('var_assign')
    def statement(self, p):
        return p.var_assign
    
    @_('FOR var_assign TO expr THEN block')
    def statement(self, p):
        return ('for', ('for_setup', p.var_assign, p.expr), p.block)
    
    @_('HIDDEN var_assign')
    def var_assign(self, p):
        return ('var_assign', 'hide_in_ide', p.var_assign)
    
    @_('PUBLIC var_assign')
    def var_assign(self, p):
        return ('var_assign', 'show_in_ide', p.var_assign)
    
    @_('NAME "(" ")"')
    def expr(self, p):
        return ('fun_call', p.NAME)
    
    @_('NAME')
    def expr(self, p):
        return ('var', p.NAME)
    
    @_('NUMBER')
    def expr(self, p):
        return ('num', p.NUMBER)

    @_('expr "+" expr')
    def expr(self, p):
        return ('add', p.expr0, p.expr1)
    
    @_('expr "-" expr')
    def expr(self, p):
        return ('sub', p.expr0, p.expr1)
    
    @_('expr "*" expr')
    def expr(self, p):
        return ('mul', p.expr0, p.expr1)
    
    @_('expr "/" expr')
    def expr(self, p):
        return ('div', p.expr0, p.expr1)
    
    @_('"-" expr %prec UMINUS')
    def expr(self, p):
        return p.expr
    
    @_('expr EQEQ expr')
    def expr(self, p):
        return ('eqeq', p.expr0, p.expr1)
    
    @_('NAME "=" expr ";"')
    def var_assign(self, p):
        return ('var_assign', 'basic', p.NAME, p.expr)
    
    @_('NAME "=" STRING ";"')
    def var_assign(self, p):
        return ('var_assign', 'basic', p.NAME, p.STRING)
    
    @_('HIDDEN NAME "(" ")" block')
    def statement(self, p):
        return ('fun_def', 'hide_in_ide', p.NAME, p.block)
    
    @_('PUBLIC NAME "(" ")" block')
    def statement(self, p):
        return ('fun_def', 'show_in_ide', p.NAME, p.block)
    
    @_('HIDDEN NAME block')
    def statement(self, p):
        return ('fun_def', 'hide_in_ide', p.NAME, p.block)
    
    @_('PUBLIC NAME block')
    def statement(self, p):
        return ('fun_def', 'show_in_ide', p.NAME, p.block)
    
    @_('IF expr THEN block ELSE block')
    def statement(self, p):
        return ('if', p.IF, p.expr, ('branch', p.block0, p.ELSE, p.block1))
    
    @_('RETURN expr ";"')
    def statement(self, p):
        return ('return', p.expr)
    
    @_('NAME "(" ")" ";"')
    def statement(self, p):
        return ('fun_call', p.NAME)
    
class GemExecute:

    def __init__(self, tree, env):
        self.env = env
        result = self.walkTree(tree)
        if result is not None and isinstance(result, int):
            print(result)
        if isinstance(result, str) and result[0] == '"':
            print(result)

if __name__ == '__main__':
    lexer = GemLexer()
    parser = GemParser()
    env = {}
    with open('ExampleApp/Main.gem', 'r') as fp:
        src = fp.read()
        for t in lexer.tokenize(src):
            print(t)
        tree = parser.parse(lexer.tokenize(src))
        print(tree)
        #GemExecute(tree, env) 
