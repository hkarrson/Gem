import sys
import Lexer
import AST

def main(argv):
    LexedSource = Lexer.LexerMain(argv)
    if not LexedSource is None:
        ASTInstance = AST.BuildAST(LexedSource)
    print(ASTInstance)
    
if __name__== "__main__":
    main(sys.argv[1:])
