@256
D=A
@SP
M=D

// 0: push constant 7

@7
D=A
@SP
A=M
M=D
@SP
M=M+1

// 1: push constant 8

@8
D=A
@SP
A=M
M=D
@SP
M=M+1

// 2: add

@SP
AM=M-1
D=M
A=A-1
M=M+D
