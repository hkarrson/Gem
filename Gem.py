from sly import Lexer
from sly import Parser

class GemLexer(Lexer):
    tokens = { ONENAME, NUMBER, STRING, PUBLIC, HIDDEN, RETURN, EQEQ }
    ignore = '\t '

    literals = { '=', '+', '-', '/', '*', '(', ')', '{', '}', '[', ']', ',', ';', '.' }
    
    PUBLIC = r'public'
    HIDDEN = r'hidden'
    RETURN = 'return'
    ONENAME = r'[a-zA-Z_][a-zA-Z0-9_]*'
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
    
    @_('HIDDEN var_assign')
    def var_assign(self, p):
        return ('var_assign', 'hide_in_ide', p.var_assign)
    
    @_('PUBLIC var_assign')
    def var_assign(self, p):
        return ('var_assign', 'show_in_ide', p.var_assign)
    
    @_('name "." ONENAME')
    def name(self, p):
        return ('name', p.name[1] + (p.ONENAME, '.'))
    
    @_('ONENAME')
    def name(self, p):
        return ('name', (p.ONENAME, '.'))
    
    @_('name "(" ")"')
    def expr(self, p):
        return ('fun_call', p.name)
    
    @_('name')
    def expr(self, p):
        return ('var', p.name)
    
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
    
    @_('name "=" expr ";"')
    def var_assign(self, p):
        return ('var_assign', 'basic', p.name, p.expr)
    
    @_('name "=" STRING ";"')
    def var_assign(self, p):
        return ('var_assign', 'basic', p.name, p.STRING)
    
    @_('HIDDEN name "(" ")" block')
    def statement(self, p):
        return ('fun_def', 'hide_in_ide', p.name, p.block)
    
    @_('PUBLIC name "(" ")" block')
    def statement(self, p):
        return ('fun_def', 'show_in_ide', p.name, p.block)
    
    @_('HIDDEN name block')
    def statement(self, p):
        return ('fun_def', 'hide_in_ide', p.name, p.block)
    
    @_('PUBLIC name block')
    def statement(self, p):
        return ('fun_def', 'show_in_ide', p.name, p.block)
    
    @_('RETURN expr ";"')
    def statement(self, p):
        return ('return', p.expr)
    
    @_('name "(" ")" ";"')
    def statement(self, p):
        return ('fun_call', p.name)
    
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
