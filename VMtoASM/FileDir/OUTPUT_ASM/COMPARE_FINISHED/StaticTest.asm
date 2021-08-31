@256
D=A
@SP
M=D

// 0: push constant 111

@111
D=A
@SP
A=M
M=D
@SP
M=M+1

// 1: push constant 333

@333
D=A
@SP
A=M
M=D
@SP
M=M+1

// 2: push constant 888

@888
D=A
@SP
A=M
M=D
@SP
M=M+1

// 3: pop static 8

@24
D=A
@R13
M=D
@SP
AM=M-1
D=M
@R13
A=M
M=D

// 4: pop static 3

@19
D=A
@R13
M=D
@SP
AM=M-1
D=M
@R13
A=M
M=D

// 5: pop static 1

@17
D=A
@R13
M=D
@SP
AM=M-1
D=M
@R13
A=M
M=D

// 6: push static 3

@19
D=M
@SP
A=M
M=D
@SP
M=M+1

// 7: push static 1

@17
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

// 9: push static 8

@24
D=M
@SP
A=M
M=D
@SP
M=M+1

// 10: add

@SP
AM=M-1
D=M
A=A-1
M=M+D
