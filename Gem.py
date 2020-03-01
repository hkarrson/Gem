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

    def getNames(self, node, env):
        if node[0] == 'fun_def':
            name = node[2][1][0]
            body = node[3][1][1]
            env[name] = {'name': name, 'body': body}
            for i in range(0, len(body), 2):
                if (body[i][0] == 'fun_def'):
                    child = self.getNames(body[i], env[name])  
                    env[name][child['name']] = child              
            return env[name]
        return ['null']
    
    def walkTree(self, node):
        if isinstance(node, int):
            return node
        elif isinstance(node, str):
            return node
        elif node is None or len(node) < 1:
            return None
        elif node[0] == 'main':
            stmts = node[1][1]
            self.env['MethodNameTree'] = {}
            for i in range(0, len(stmts), 2):
                self.getNames(stmts[i], self.env['MethodNameTree'] )
            for i in range(0, len(stmts), 2):
                self.walkTree(stmts[i])
            return None
        elif node[0] == 'statements':
            stmts = node[1]
            for i in range(0, len(stmts), 2):
                self.walkTree(stmts[i])
        elif node[0] == 'fun_call':
            names = node[1][1]
            name = ""
            try:
                name += names[0] + "."
                namenode = self.env['MethodNameTree'][names[0]]
                if (len(names) > 2):
                    for i in range(2, len(names), 2):
                        name += names[i] + "."
                        namenode = namenode[names[i]]
                name = name[:-1]
            except KeyError:
                name = name[:-1]
                print("Method does not exist: ", end = '')
                print(name)
                return None
            body = namenode['body']
            for stmt in body:
                self.walkTree(stmt)
        elif node[0] == 'fun_def':
            return None
        else:
            print("Node not implemented: ", end = '')
            print(node)
            return None

if __name__ == '__main__':
    lexer = GemLexer()
    parser = GemParser()
    env = {}
    with open('ExampleApp/Main.gem', 'r') as fp:
        src = fp.read()
        tree = parser.parse(lexer.tokenize(src))
        GemExecute(tree, env) 
