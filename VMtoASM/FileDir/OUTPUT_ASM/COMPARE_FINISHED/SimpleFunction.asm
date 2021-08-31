@256
D=A
@SP
M=D

// 0: function simplefunction.test 2

(simplefunction.test)

// 1: push local 0

@LCL
D=M
@0
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1

// 2: push local 1

@LCL
D=M
@1
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1

// 3: add

@SP
AM=M-1
D=M
A=A-1
M=M+D

// 4: not

@SP
A=M-1
M=!M

// 5: push argument 0

@ARG
D=M
@0
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1

// 6: add

@SP
AM=M-1
D=M
A=A-1
M=M+D

// 7: push argument 1

@ARG
D=M
@1
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1

// 8: sub

@SP
AM=M-1
D=M
A=A-1
M=M-D
D=0

// 9: return

@LCL
D=M
@FRAME
M=D
@5
A=D-A
D=M
@RET
M=D
@ARG
D=M
@0
D=D+A
@R13
M=D
@SP
AM=M-1
D=M
@R13
A=M
M=D
@ARG
D=M
@SP
M=D+1
@FRAME
D=M-1
AM=D
D=M
@THAT
M=D
@FRAME
D=M-1
AM=D
D=M
@THIS
M=D
@FRAME
D=M-1
AM=D
D=M
@ARG
M=D
@FRAME
D=M-1
AM=D
D=M
@LCL
M=D
@RET
A=M
0;JMP
