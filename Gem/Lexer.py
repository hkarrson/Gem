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
    LexedSource = []
    if (len(argv) > 0):            
        with open(argv[0]) as fp:
            LexerInstance = BuildLexer()
            line = fp.readline()
            while line:
                if len(line.strip()) > 0:
                    stream = LexerInstance.lex(line)
                    token = None
                    try:
                        token = stream.next()
                    except StopIteration:
                        continue
                    while not token is None:
                        LexedSource.append(token)
                        try:
                            token = stream.next()
                        except StopIteration:
                            token = None
                line = fp.readline()
    return LexedSource;
