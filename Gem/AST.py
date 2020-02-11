from rply import ParserGenerator

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
parser = pg.build()

@pg.production("main : statements")
def main(s):
    return s[0]

def BuildAST(LexedSource):
    
    return None
