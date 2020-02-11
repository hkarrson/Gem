from rply import LexerGenerator

def BuildLexer():
    lg = LexerGenerator()
    lg.add("STRING", r"\".*\"")
    lg.ignore(r"\s+")
    lg.add("COMMA", r"\,")
    lg.add("METHOD", r"\:")
    lg.add("LBRACE", r"\{")
    lg.add("RETURN", r"return")
    lg.add("RBRACE", r"\}")
    lg.add("LPAREN", r"\(")
    lg.add("RPAREN", r"\)")
    lg.add("FOR", r"for")
    lg.add("EQUALS", r"\=")
    lg.add("NUMBER", r"\d+")
    lg.add("SEMICOLON", r"\;")
    lg.add("LESSTHAN", r"\<")
    lg.add("INCREMENT", r"\+\+")    
    lg.add("PLUS", r"\+")   
    lg.add("NAME", r"[a-zA-z_][a-zA-Z0-9_]*") 
    return lg.build()

def LexerMain(argv):    
    LexedSource = None
    if (len(argv) > 0):            
        with open(argv[0]) as fp:
            LexerInstance = BuildLexer()
            src = fp.read()
            LexedSource = LexerInstance.lex(src)
    return LexedSource;
