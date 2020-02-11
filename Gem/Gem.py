import sys
import Lexer
import AST

def main(argv):
    LexedSource = Lexer.LexerMain(argv)
    for Token in LexedSource:
        print(Token)
    ASTInstance = AST.BuildAST(LexedSource)
    print(ASTInstance)
    
if __name__== "__main__":
    main(sys.argv[1:])
