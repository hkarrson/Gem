import sys
import Lexer

def main(argv):
    if (len(argv) > 0):            
        with open(argv[0]) as fp:
            LexerInstance = Lexer.BuildLexer()
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
                        print(token)
                        try:
                            token = stream.next()
                        except StopIteration:
                            token = None
                line = fp.readline()

if __name__== "__main__":
    main(sys.argv[1:])
