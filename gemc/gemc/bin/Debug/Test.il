.assembly extern mscorlib {}
.assembly Test {}
.module Test.exe

.class Test.Program
extends [mscorlib]System.Object
{
 .method static void Main(string[] args)
 cil managed
 {
  .entrypoint
  ldstr "Hello World"
  call void [mscorlib]System.Console::
  WriteLine(string)
  ret
 }
}