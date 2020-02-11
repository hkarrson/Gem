from rply import ParserGenerator

class Node(object):
    def __eq__(self, other):
        if not isinstance(other, Node):
            return NotImplemented
        return (type(self) is type(other) and
                self.__dict__ == other.__dict__)
    def __ne__(self, other):
        return not (self == other)

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

parser = pg.build()

def BuildAST(LexedSource):
    
    return None
