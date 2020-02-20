import sys
import Lexer
import AST

def main(argv):
    LexedSource = Lexer.LexerMain(argv)
    if not LexedSource is None:
        while True:
            try:
                print(LexedSource.next())
            except: break
        ASTInstance = AST.BuildAST(LexedSource)
    print(ASTInstance)
    
if __name__== "__main__":
    main(sys.argv[1:])
