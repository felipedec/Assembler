@ECHO OFF

SET Executable=..\bin\Debug\Assembler.exe
IF NOT EXIST %Executable% SET Executable=..\bin\Release\Assembler.exe
IF NOT EXIST %Executable% GOTO Executable_NotFound

SET DeleteFiles=''

FOR %%f IN (Test_*.txt) DO (
	SET TestFile=%%f

	%Executable% -i %TestFile% -o Out%TestFile%

	SET DeleteFiles=%DeleteFiles% Out%TestFile%

	SET LastFile=%TestFile%

	FC TestResult%TestFile:~4% Out%TestFile% > nul
	IF ERRORLEVEL 1 GOTO Test_Error
)

DEL %DeleteFiles%


ECHO Teste finalizado com sucesso.
EXIT /B

:Executable_NotFound
ECHO Arquivo executavel nao encontrado.
EXIT /B

:Test_Error
Echo Teste falhou: %LastFile%
DEL %DeleteFiles%