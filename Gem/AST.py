from rply import ParserGenerator

class Node(object):
    def __eq__(self, other):
        if not isinstance(other, Node):
            return NotImplemented
        return (type(self) is type(other) and
                self.__dict__ == other.__dict__)
    def __ne__(self, other):
        return not (self == other)
class Block(Node):
    def __init__(self, statements):
        self.statements = statements
class Statement(Node):
    def __init__(self, expr):
        self.expr = expr
class Number(Node):
    def __init__(self, value):
        self.value = value
class Name(Node):
    def __init__(self, name):
        self.name = name
class String(Node):
    def __init__(self, text):
        self.text = text
class Return(Node):
    def __init__(self, expr):
        self.expr = expr

pg = ParserGenerator([
                       "STRING",
                       "COMMA",
                       "METHOD",
                       "LBRACE",
                       "RETURN",
                       "RBRACE",
                       "LPAREN",
                       "RPAREN",
                       "FOR",
                       "EQUALS",
                       "NUMBER",
                       "SEMICOLON",
                       "LESSTHAN",
                       "INCREMENT",
                       "PLUS",
                       "NAME"], cache_id="gem")

@pg.production("main : statements")
def main(s):
    return s[0]

@pg.production("statements : LBRACE statements statement RBRACE")
def statements(s):
    return Block(s[1].getastlist() + [s[2]])

@pg.production("statements : LBRACE statement RBRACE")
def statements_statement(s):
    return Block([s[1]])

@pg.production("statement : expr SEMICOLON")
def statement_expr(s):
    return Statement(s[0])

@pg.production("expr : NUMBER")
def expr_number(s):
    return Number(float(s[0].getstr()))

@pg.production("expr : STRING")
def expr_string(s):
    return String(s[0].getstr()[1:-1])

parser = pg.build()

def BuildAST(LexedSource):
    return parser.parse(LexedSource)
